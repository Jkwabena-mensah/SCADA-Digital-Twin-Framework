# 🔌 SCADA Digital Twin API Documentation

Complete REST API documentation for the SCADA Digital Twin Framework.

## Base URL
```
http://localhost:5267/api/SensorData

## 📋 Table of Contents

1. [Data Retrieval Endpoints](#data-retrieval-endpoints)
2. [Analytics Endpoints](#analytics-endpoints)
3. [Monitoring Endpoints](#monitoring-endpoints)
4. [System Endpoints](#system-endpoints)
5. [Response Codes](#response-codes)
6. [Error Handling](#error-handling)


## Data Retrieval Endpoints

### Get Latest Readings
**Endpoint:** `GET /api/SensorData/latest`

**Description:** Retrieves the most recent sensor readings.

**Query Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| count | int | No | 100 | Number of readings to retrieve |
| assetId | string | No | "" | Filter by specific asset ID |

**Example Request:**
```http
GET /api/SensorData/latest?count=50&assetId=MOTOR_001
```

**Example Response:**
```json
[
  {
    "id": 1,
    "assetId": "MOTOR_001",
    "timestamp": "2024-11-07T10:30:00Z",
    "motorAmps": 10.5,
    "temperature": 75.2,
    "vibration": 0.25,
    "status": "RUNNING"
  }
]


### Get Reading by ID
**Endpoint:** `GET /api/SensorData/{id}`

**Description:** Retrieves a specific sensor reading by its ID.

**Path Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | int | Yes | Reading ID |

**Example Request:**
```http
GET /api/SensorData/42
```

**Example Response:**
```json
{
  "id": 42,
  "assetId": "MOTOR_001",
  "timestamp": "2024-11-07T10:30:00Z",
  "motorAmps": 10.5,
  "temperature": 75.2,
  "vibration": 0.25,
  "status": "RUNNING"
}


### Get Readings by Date Range
**Endpoint:** `GET /api/SensorData/range`

**Description:** Retrieves readings within a specific date/time range.

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| start | DateTime | Yes | Start date/time (ISO 8601) |
| end | DateTime | Yes | End date/time (ISO 8601) |
| assetId | string | No | Filter by specific asset ID |

**Example Request:**
```http
GET /api/SensorData/range?start=2024-11-01T00:00:00Z&end=2024-11-07T23:59:59Z
```

**Example Response:**
```json
{
  "start": "2024-11-01T00:00:00Z",
  "end": "2024-11-07T23:59:59Z",
  "count": 2500,
  "data": [...]
}
```

---

### Get Last N Minutes
**Endpoint:** `GET /api/SensorData/last-minutes`

**Description:** Retrieves readings from the last N minutes.

**Query Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| minutes | int | No | 5 | Number of minutes to look back |
| assetId | string | No | "" | Filter by specific asset ID |

**Example Request:**
```http
GET /api/SensorData/last-minutes?minutes=10
```

**Example Response:**
```json
{
  "timeWindow": "10 minutes",
  "cutoffTime": "2024-11-07T10:20:00Z",
  "count": 300,
  "data": [...]
}


## Analytics Endpoints

### Get Statistics
**Endpoint:** `GET /api/SensorData/stats`

**Description:** Retrieves comprehensive system statistics including averages, min/max values.

**Query Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| minutes | int | No | 5 | Time window for calculations |

**Example Request:**
```http
GET /api/SensorData/stats?minutes=5
```

**Example Response:**
```json
{
  "latestReading": {...},
  "timeWindow": "Last 5 minutes",
  "averageAmps": 10.25,
  "averageTemperature": 75.8,
  "averageVibration": 0.287,
  "maxAmps": 11.5,
  "maxTemperature": 78.2,
  "maxVibration": 0.35,
  "minAmps": 9.1,
  "minTemperature": 73.5,
  "minVibration": 0.22,
  "totalReadings": 15420,
  "recentReadingsCount": 150,
  "status": "RUNNING"
}


### Get Aggregated Data
**Endpoint:** `GET /api/SensorData/aggregated`

**Description:** Retrieves time-series data aggregated by specified interval.

**Query Parameters:**
| Parameter | Type | Required | Default | Options | Description |
|-----------|------|----------|---------|---------|-------------|
| interval | string | No | "minute" | minute, hour | Aggregation interval |
| minutes | int | No | 60 | - | Time window to aggregate |

**Example Request:**
```http
GET /api/SensorData/aggregated?interval=minute&minutes=30
```

**Example Response:**
```json
{
  "interval": "minute",
  "timeWindow": "30 minutes",
  "dataPoints": 30,
  "data": [
    {
      "timestamp": "2024-11-07T10:00:00Z",
      "avgAmps": 10.15,
      "avgTemperature": 75.2,
      "avgVibration": 0.28,
      "count": 30
    }
  ]
}
```

---

## Monitoring Endpoints

### Get System Health
**Endpoint:** `GET /api/SensorData/health`

**Description:** Retrieves current system health status with threshold violation checks.

**Health Thresholds:**
- **Temperature:** Warning: 80°F, Critical: 90°F
- **Motor Amps:** Warning: 12A, Critical: 13A
- **Vibration:** Warning: 0.4 mm/s, Critical: 0.5 mm/s

**Example Request:**
```http
GET /api/SensorData/health
```

**Example Response:**
```json
{
  "health": "WARNING",
  "message": "WARNING: Temperature at 82.5°F (threshold: 80.0°F)",
  "timestamp": "2024-11-07T10:30:00Z",
  "issues": [],
  "warnings": [
    "WARNING: Temperature at 82.5°F (threshold: 80.0°F)"
  ],
  "currentValues": {
    "temperature": 82.5,
    "motorAmps": 10.5,
    "vibration": 0.28,
    "status": "RUNNING"
  }
}
```

**Health Status Values:**
- `NORMAL` - All parameters within operating range
- `WARNING` - One or more parameters exceed warning thresholds
- `CRITICAL` - One or more parameters exceed critical thresholds
- `NO_DATA` - No sensor data available

---

### Get Alerts History
**Endpoint:** `GET /api/SensorData/alerts`

**Description:** Retrieves history of threshold violations (alerts).

**Query Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| hours | int | No | 24 | Number of hours to look back |

**Example Request:**
```http
GET /api/SensorData/alerts?hours=24
```

**Example Response:**
```json
{
  "timeWindow": "Last 24 hours",
  "alertCount": 15,
  "alerts": [
    {
      "id": 1523,
      "assetId": "MOTOR_001",
      "timestamp": "2024-11-07T10:25:00Z",
      "severity": "WARNING",
      "violations": [
        "Temperature: 81.2°F"
      ],
      "values": {
        "temperature": 81.2,
        "motorAmps": 10.8,
        "vibration": 0.32
      }
    }
  ]
}


### Get Assets List
**Endpoint:** `GET /api/SensorData/assets`

**Description:** Retrieves list of all monitored assets with summary information.

**Example Request:**
```http
GET /api/SensorData/assets
```

**Example Response:**
```json
{
  "assetCount": 1,
  "assets": [
    {
      "assetId": "MOTOR_001",
      "firstReading": "2024-11-01T00:00:00Z",
      "lastReading": "2024-11-07T10:30:00Z",
      "totalReadings": 15420,
      "latestStatus": "RUNNING"
    }
  ]

```


## System Endpoints

### Get API Info
**Endpoint:** `GET /api/SensorData/info`

**Description:** Retrieves API information and available endpoints documentation.

**Example Request:**
```http
GET /api/SensorData/info
```

**Example Response:**
```json
{
  "apiVersion": "1.0",
  "projectName": "SCADA Digital Twin Framework",
  "description": "Real-time industrial asset monitoring API",
  "endpoints": {
    "dataRetrieval": [...],
    "analytics": [...],
    "monitoring": [...],
    "system": [...]
  },
  "documentation": "https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework"
}
```



## Response Codes

| Code | Status | Description |
|------|--------|-------------|
| 200 | OK | Request successful |
| 400 | Bad Request | Invalid parameters or malformed request |
| 404 | Not Found | Resource not found |
| 500 | Internal Server Error | Server error occurred |

---

## Error Handling

All error responses follow this format:
```json
{
  "error": "Error message describing what went wrong"
}
```

**Example Error Response:**
```json
{
  "error": "Reading with ID 9999 not found"
}
```



## Rate Limiting

Currently, there are no rate limits imposed. For production deployment, consider implementing rate limiting using ASP.NET Core middleware.

---

## Testing the API

### Using cURL
```bash
# Get latest readings
curl -X GET "http://localhost:5267/api/SensorData/latest?count=10"

# Get system health
curl -X GET "http://localhost:5267/api/SensorData/health"

# Get statistics
curl -X GET "http://localhost:5267/api/SensorData/stats?minutes=5"
```

### Using the Built-in API Test Page
Navigate to: `http://localhost:5267/ApiTest`

### Using Postman
Import the Postman collection from: `docs/api-collection.json`

---

## Additional Notes

- All timestamps are in UTC and follow ISO 8601 format  
- Numeric values are rounded to appropriate decimal places  
- The API supports CORS for development purposes  
- Authentication/Authorization can be added in future versions  

---

## Support

For issues, questions, or contributions, please visit:  
https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework
```

