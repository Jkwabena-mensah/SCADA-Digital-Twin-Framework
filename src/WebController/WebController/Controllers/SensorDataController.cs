using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebController.Data;
using WebController.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SensorDataController> _logger;

        public SensorDataController(ApplicationDbContext context, ILogger<SensorDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /* -------------- BASIC DATA RETRIEVAL ---------------- */
        /// <summary>
        /// GET: api/SensorData/latest?count=100
        /// </summary>
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<SensorReading>>> GetLatestReadings(
            [FromQuery] int count = 100,
            [FromQuery] string assetId = "")
        {
            try
            {
                var query = _context.SensorReadings.AsQueryable();
                if (!string.IsNullOrEmpty(assetId))
                    query = query.Where(r => r.AssetId == assetId);
                var readings = await query
                    .OrderByDescending(r => r.Timestamp)
                    .Take(count)
                    .OrderBy(r => r.Timestamp)
                    .ToListAsync();
                _logger.LogInformation($"Retrieved {readings.Count} latest readings");
                return Ok(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving latest readings: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// GET: api/SensorData/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SensorReading>> GetSensorReading(int id)
        {
            var reading = await _context.SensorReadings.FindAsync(id);
            if (reading == null)
                return NotFound(new { error = $"Reading with ID {id} not found" });
            return Ok(reading);
        }

        /* -------------- TIME-BASED QUERIES ---------------- */
        /// <summary>
        /// GET: api/SensorData/range?start=...&end=...
        /// </summary>
        [HttpGet("range")]
        public async Task<ActionResult> GetReadingsByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] string assetId = "")
        {
            if (start >= end)
                return BadRequest(new { error = "Start date must be before end date" });
            try
            {
                var query = _context.SensorReadings
                    .Where(r => r.Timestamp >= start && r.Timestamp <= end);
                if (!string.IsNullOrEmpty(assetId))
                    query = query.Where(r => r.AssetId == assetId);
                var readings = await query
                    .OrderBy(r => r.Timestamp)
                    .ToListAsync();
                return Ok(new
                {
                    start,
                    end,
                    count = readings.Count,
                    data = readings
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving range data: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// GET: api/SensorData/last-minutes?minutes=5
        /// </summary>
        [HttpGet("last-minutes")]
        public async Task<ActionResult> GetLastMinutes(
            [FromQuery] int minutes = 5,
            [FromQuery] string assetId = "")
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);
            var query = _context.SensorReadings
                .Where(r => r.Timestamp >= cutoffTime);
            if (!string.IsNullOrEmpty(assetId))
                query = query.Where(r => r.AssetId == assetId);
            var readings = await query
                .OrderBy(r => r.Timestamp)
                .ToListAsync();
            return Ok(new
            {
                timeWindow = $"{minutes} minutes",
                cutoffTime,
                count = readings.Count,
                data = readings
            });
        }

        /* -------------- STATISTICS & ANALYTICS ---------------- */
        /// <summary>
        /// GET: api/SensorData/stats
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult> GetStatistics([FromQuery] int minutes = 5)
        {
            try
            {
                var latestReading = await _context.SensorReadings
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefaultAsync();
                if (latestReading == null)
                    return Ok(new { message = "No data available" });
                var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);
                var recentReadings = await _context.SensorReadings
                    .Where(r => r.Timestamp >= cutoffTime)
                    .ToListAsync();
                int totalReadings = await _context.SensorReadings.CountAsync();
                var stats = new
                {
                    LatestReading = latestReading,
                    TimeWindow = $"Last {minutes} minutes",
                    AverageAmps = recentReadings.Any() ? Math.Round(recentReadings.Average(r => r.MotorAmps), 2) : 0,
                    AverageTemperature = recentReadings.Any() ? Math.Round(recentReadings.Average(r => r.Temperature), 2) : 0,
                    AverageVibration = recentReadings.Any() ? Math.Round(recentReadings.Average(r => r.Vibration), 3) : 0,
                    MaxAmps = recentReadings.Any() ? Math.Round(recentReadings.Max(r => r.MotorAmps), 2) : 0,
                    MaxTemperature = recentReadings.Any() ? Math.Round(recentReadings.Max(r => r.Temperature), 2) : 0,
                    MaxVibration = recentReadings.Any() ? Math.Round(recentReadings.Max(r => r.Vibration), 3) : 0,
                    MinAmps = recentReadings.Any() ? Math.Round(recentReadings.Min(r => r.MotorAmps), 2) : 0,
                    MinTemperature = recentReadings.Any() ? Math.Round(recentReadings.Min(r => r.Temperature), 2) : 0,
                    MinVibration = recentReadings.Any() ? Math.Round(recentReadings.Min(r => r.Vibration), 3) : 0,
                    TotalReadings = totalReadings,
                    RecentReadingsCount = recentReadings.Count,
                    Status = latestReading.Status
                };
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating statistics: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// GET: api/SensorData/aggregated
        /// </summary>
        [HttpGet("aggregated")]
        public async Task<ActionResult> GetAggregatedData(
            [FromQuery] string interval = "minute",
            [FromQuery] int minutes = 60)
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);
                var readings = await _context.SensorReadings
                    .Where(r => r.Timestamp >= cutoffTime)
                    .OrderBy(r => r.Timestamp)
                    .ToListAsync();
                if (!readings.Any())
                    return Ok(new { message = "No data available for aggregation" });
                IEnumerable<object> aggregated;
                switch (interval.ToLower())
                {
                    case "minute":
                        aggregated = readings
                            .GroupBy(r => new
                            {
                                r.Timestamp.Year,
                                r.Timestamp.Month,
                                r.Timestamp.Day,
                                r.Timestamp.Hour,
                                r.Timestamp.Minute
                            })
                            .Select(g => new
                            {
                                Timestamp = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, g.Key.Minute, 0),
                                AvgAmps = Math.Round(g.Average(r => r.MotorAmps), 2),
                                AvgTemperature = Math.Round(g.Average(r => r.Temperature), 2),
                                AvgVibration = Math.Round(g.Average(r => r.Vibration), 3),
                                Count = g.Count()
                            });
                        break;
                    case "hour":
                        aggregated = readings
                            .GroupBy(r => new
                            {
                                r.Timestamp.Year,
                                r.Timestamp.Month,
                                r.Timestamp.Day,
                                r.Timestamp.Hour
                            })
                            .Select(g => new
                            {
                                Timestamp = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
                                AvgAmps = Math.Round(g.Average(r => r.MotorAmps), 2),
                                AvgTemperature = Math.Round(g.Average(r => r.Temperature), 2),
                                AvgVibration = Math.Round(g.Average(r => r.Vibration), 3),
                                Count = g.Count()
                            });
                        break;
                    default:
                        return BadRequest(new { error = "Invalid interval. Use 'minute' or 'hour'" });
                }
                return Ok(new
                {
                    interval,
                    timeWindow = $"{minutes} minutes",
                    dataPoints = aggregated.Count(),
                    data = aggregated
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error aggregating data: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /* -------------- SYSTEM HEALTH & MONITORING ---------------- */
        /// <summary>
        /// GET: api/SensorData/health
        /// </summary>
        [HttpGet("health")]
        public async Task<ActionResult> GetSystemHealth()
        {
            try
            {
                var latestReading = await _context.SensorReadings
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefaultAsync();
                if (latestReading == null)
                    return Ok(new
                    {
                        Health = "NO_DATA",
                        Message = "No sensor data available",
                        Timestamp = DateTime.UtcNow
                    });
                const double TEMP_WARNING = 80.0;
                const double TEMP_CRITICAL = 90.0;
                const double AMPS_WARNING = 12.0;
                const double AMPS_CRITICAL = 13.0;
                const double VIB_WARNING = 0.4;
                const double VIB_CRITICAL = 0.5;
                var issues = new List<string>();
                var warnings = new List<string>();
                if (latestReading.Temperature >= TEMP_CRITICAL)
                    issues.Add($"CRITICAL: Temperature at {latestReading.Temperature:F1}°F (threshold: {TEMP_CRITICAL}°F)");
                else if (latestReading.Temperature >= TEMP_WARNING)
                    warnings.Add($"WARNING: Temperature at {latestReading.Temperature:F1}°F (threshold: {TEMP_WARNING}°F)");
                if (latestReading.MotorAmps >= AMPS_CRITICAL)
                    issues.Add($"CRITICAL: Motor current at {latestReading.MotorAmps:F2}A (threshold: {AMPS_CRITICAL}A)");
                else if (latestReading.MotorAmps >= AMPS_WARNING)
                    warnings.Add($"WARNING: Motor current at {latestReading.MotorAmps:F2}A (threshold: {AMPS_WARNING}A)");
                if (latestReading.Vibration >= VIB_CRITICAL)
                    issues.Add($"CRITICAL: Vibration at {latestReading.Vibration:F3}mm/s (threshold: {VIB_CRITICAL}mm/s)");
                else if (latestReading.Vibration >= VIB_WARNING)
                    warnings.Add($"WARNING: Vibration at {latestReading.Vibration:F3}mm/s (threshold: {VIB_WARNING}mm/s)");
                string health, message;
                if (issues.Any())
                {
                    health = "CRITICAL";
                    message = string.Join("; ", issues);
                }
                else if (warnings.Any())
                {
                    health = "WARNING";
                    message = string.Join("; ", warnings);
                }
                else
                {
                    health = "NORMAL";
                    message = "All parameters within normal operating range";
                }
                return Ok(new
                {
                    Health = health,
                    Message = message,
                    Timestamp = latestReading.Timestamp,
                    Issues = issues,
                    Warnings = warnings,
                    CurrentValues = new
                    {
                        latestReading.Temperature,
                        latestReading.MotorAmps,
                        latestReading.Vibration,
                        latestReading.Status
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking system health: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// GET: api/SensorData/alerts
        /// </summary>
        [HttpGet("alerts")]
        public async Task<ActionResult> GetAlerts([FromQuery] int hours = 24)
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-hours);
                var readings = await _context.SensorReadings
                    .Where(r => r.Timestamp >= cutoffTime)
                    .Where(r => r.Temperature > 80 || r.MotorAmps > 12 || r.Vibration > 0.4)
                    .OrderByDescending(r => r.Timestamp)
                    .ToListAsync();
                var alerts = readings.Select(r => new
                {
                    r.Id,
                    r.AssetId,
                    r.Timestamp,
                    Severity = DetermineSeverity(r),
                    Violations = GetViolations(r),
                    Values = new
                    {
                        r.Temperature,
                        r.MotorAmps,
                        r.Vibration
                    }
                });
                return Ok(new
                {
                    timeWindow = $"Last {hours} hours",
                    alertCount = alerts.Count(),
                    alerts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving alerts: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /* -------------- ASSET MANAGEMENT ---------------- */
        /// <summary>
        /// GET: api/SensorData/assets
        /// </summary>
        [HttpGet("assets")]
        public async Task<ActionResult> GetAssets()
        {
            try
            {
                var assets = await _context.SensorReadings
                    .GroupBy(r => r.AssetId)
                    .Select(g => new
                    {
                        AssetId = g.Key,
                        FirstReading = g.Min(r => r.Timestamp),
                        LastReading = g.Max(r => r.Timestamp),
                        TotalReadings = g.Count(),
                        LatestStatus = g.OrderByDescending(r => r.Timestamp)
                                        .FirstOrDefault()!.Status
                    })
                    .ToListAsync();
                return Ok(new
                {
                    assetCount = assets.Count,
                    assets
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving assets: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /* -------------- SYSTEM INFO ---------------- */
        /// <summary>
        /// GET: api/SensorData/info
        /// </summary>
        [HttpGet("info")]
        public ActionResult GetApiInfo()
        {
            return Ok(new
            {
                apiVersion = "1.0",
                projectName = "SCADA Digital Twin Framework",
                description = "Real-time industrial asset monitoring API",
                documentation = "https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework"
            });
        }

        /* -------------- HELPER METHODS ---------------- */
        private string DetermineSeverity(SensorReading reading)
        {
            if (reading.Temperature >= 90 || reading.MotorAmps >= 13 || reading.Vibration >= 0.5)
                return "CRITICAL";
            if (reading.Temperature >= 80 || reading.MotorAmps >= 12 || reading.Vibration >= 0.4)
                return "WARNING";
            return "NORMAL";
        }

        private List<string> GetViolations(SensorReading reading)
        {
            // --------- NULL GUARD – fixes CS8602 ---------
            if (reading is null) return new List<string>();
            var violations = new List<string>();
            if (reading.Temperature > 80)
                violations.Add($"Temperature: {reading.Temperature:F1}°F");
            if (reading.MotorAmps > 12)
                violations.Add($"Current: {reading.MotorAmps:F2}A");
            if (reading.Vibration > 0.4)
                violations.Add($"Vibration: {reading.Vibration:F3}mm/s");
            return violations;
        }
    }
}