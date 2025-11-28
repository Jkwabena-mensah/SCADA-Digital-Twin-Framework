# SCADA Digital Twin Framework

**Version**: 0.1.0  
**Last Updated**: November 06, 2025  
**Author**: Joseph Kwabena Mensah, PE-GhIE #09875  
**License**: [MIT](LICENSE)

[<image-card alt="Python 3.11" src="https://img.shields.io/badge/python-3.11-blue.svg" ></image-card>](https://www.python.org/downloads/release/python-3119/)  
[<image-card alt="paho-mqtt" src="https://img.shields.io/badge/mqtt-paho%202.1.0-green" ></image-card>](https://pypi.org/project/paho-mqtt/)  
[<image-card alt=".NET 9.0" src="https://img.shields.io/badge/.NET-9.0-blueviolet" ></image-card>](https://dotnet.microsoft.com/download/dotnet/9.0)  
[<image-card alt="MQTTnet" src="https://img.shields.io/badge/MQTTnet-5.0.1.1416-green" ></image-card>](https://github.com/chkr1011/MQTTnet)  
[<image-card alt="Tests" src="https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework/actions/workflows/test.yml/badge.svg" ></image-card>](https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework/actions/workflows/test.yml)  
[<image-card alt="Coverage" src="https://codecov.io/gh/Jkwabena-mensah/SCADA-Digital-Twin-Framework/branch/main/graph/badge.svg" ></image-card>](https://codecov.io/gh/Jkwabena-mensah/SCADA-Digital-Twin-Framework)  
[<image-card alt="License" src="https://img.shields.io/github/license/Jkwabena-mensah/SCADA-Digital-Twin-Framework" ></image-card>](LICENSE)

## Overview

The SCADA Digital Twin Framework is an open-source platform for simulating and monitoring industrial SCADA systems in real-time. Built with Python, ASP.NET Core, SQL, and MQTT, it provides a scalable solution for creating digital twins, progressing through defined development phases.

## Phases

### Phase 1: Project Setup
- **Status**: Completed
- **Details**: Established the initial project structure with directories (`src/PythonSimulator`, `src/WebController`, `docs`, `tests`) and placeholder files for a modular SCADA framework.
- **Technologies**: Python 3.11, Git

### Phase 2: Python Simulator (Milestone 1)
- **Status**: Completed
- **Details**:
  - Simulates real-time SCADA sensor data (Motor Amps, Temperature, Vibration) using MQTT.
  - Run: `python src/PythonSimulator/simulator.py` (requires Mosquitto broker).
  - Test: `python src/PythonSimulator/subscribe.py` to verify data.
- **Technologies**: paho-mqtt 2.1.0, Python 3.11
- **Future**: Enhanced integration with the ASP.NET Core backend.

### Phase 3: ASP.NET Core Backend (Milestone 2)
- **Status**: Completed
- **Details**:
  - Developed an ASP.NET Core Web App (`WebController`) in `src/WebController`.
  - Added packages: `Microsoft.EntityFrameworkCore.SqlServer` (9.0.10), `Microsoft.EntityFrameworkCore.InMemory` (9.0.10), `Microsoft.EntityFrameworkCore.Tools` (9.0.10), `MQTTnet` (5.0.1.1416).
  - Defined `SensorReading` model in `src/WebController/WebController/Models/SensorReading.cs` (Id, AssetId, Timestamp, MotorAmps, Temperature, Vibration, Status, with nullable fields).
  - Created `ApplicationDbContext` in `src/WebController/WebController/Data/ApplicationDbContext.cs` with an in-memory database in `Program.cs`.
  - Implemented `MqttSubscriberService` in `src/WebController/WebController/Services/MqttSubscriberService.cs` to subscribe to `scada/sensor/data` and store data.
  - Added `SensorDataController` in `src/WebController/WebController/Controllers/SensorDataController.cs` with endpoints: `/latest`, `/stats`, `/health`.
  - Migrated to MQTTnet v5 API, resolving namespace issues (e.g., `CS0234`).
  - Run: `dotnet run` from `src/WebController/WebController` (accessible at http://localhost:5267).
- **Technologies**: .NET 9.0, ASP.NET Core, Entity Framework Core, MQTTnet
- **Future**: Configure a SQL Server database and create migrations.

$finalPhase
