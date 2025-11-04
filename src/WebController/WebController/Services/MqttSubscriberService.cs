using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using WebController.Data;
using WebController.Models;

namespace WebController.Services
{
    public class MqttSubscriberService : BackgroundService
    {
        private readonly ILogger<MqttSubscriberService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IMqttClient _mqttClient = null!;

        public MqttSubscriberService(
            ILogger<MqttSubscriberService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new MqttClientFactory(); // Changed from MqttFactory to MqttClientFactory
            _mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var payloadBytes = e.ApplicationMessage.Payload;
                var payload = Encoding.UTF8.GetString(payloadBytes);
                _logger.LogInformation($"Received: {payload}");
                try
                {
                    var sensorData = JsonSerializer.Deserialize<SensorReading>(payload);
                    if (sensorData != null)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                            await db.SensorReadings.AddAsync(sensorData);
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Deserialized sensor data is null.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                }
            };

            await _mqttClient.ConnectAsync(options, stoppingToken);

            await _mqttClient.SubscribeAsync(
                new MqttTopicFilterBuilder()
                    .WithTopic("scada/sensor/data")
                    .Build(),
                stoppingToken);

            _logger.LogInformation("MQTT Subscriber Service Started");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient != null)
            {
                await _mqttClient.DisconnectAsync();
            }
            await base.StopAsync(cancellationToken);
        }
    }
}