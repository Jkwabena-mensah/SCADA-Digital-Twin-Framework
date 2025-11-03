import time
import random
import json
import paho.mqtt.client as mqtt
from datetime import datetime, timezone

# Configuration
MQTT_BROKER = "localhost"
MQTT_PORT = 1883
MQTT_TOPIC = "scada/sensor/data"
ASSET_ID = "MOTOR_001"

# Connect to MQTT Broker
client = mqtt.Client(protocol=mqtt.MQTTv311)


def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("✓ Connected to MQTT Broker!")
    else:
        print(f"✗ Failed to connect, return code {rc}")


client.on_connect = on_connect
try:
    client.connect(MQTT_BROKER, MQTT_PORT, 60)
    client.loop_start()
except Exception as e:
    print(f"✗ Could not connect to MQTT broker: {e}")
    exit(1)

# Simulation Loop
print(f"Starting SCADA Simulator for {ASSET_ID}...")
print("Press Ctrl+C to stop\n")
try:
    while True:
        # Generate realistic sensor data
        sensor_data = {
            "AssetId": ASSET_ID,
            "Timestamp": datetime.now(timezone.utc).isoformat(),
            "MotorAmps": round(random.uniform(8.5, 12.5), 2),
            "Temperature": round(random.uniform(65.0, 85.0), 2),
            "Vibration": round(random.uniform(0.1, 0.5), 3),
            "Status": "RUNNING",
        }

        # Publish to MQTT
        payload = json.dumps(sensor_data)
        client.publish(MQTT_TOPIC, payload)

        print(
            f"📊 {sensor_data['Timestamp'][:19]} | "
            f"Amps: {sensor_data['MotorAmps']}A | "
            f"Temp: {sensor_data['Temperature']}°F | "
            f"Vib: {sensor_data['Vibration']}mm/s"
        )

        time.sleep(2)  # Send data every 2 seconds
except KeyboardInterrupt:
    print("\n\n✓ Simulator stopped")
    client.loop_stop()
    client.disconnect()
