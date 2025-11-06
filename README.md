# SCADA Digital Twin Framework

An Open-Source Python/ASP.NET Core Framework for Simulating and Monitoring Industrial SCADA Systems – built by **Joseph Kwabena Mensah, PE-GhIE #09875**.

[<image-card alt="Python 3.11" src="https://img.shields.io/badge/python-3.11-blue.svg" ></image-card>](https://www.python.org/downloads/release/python-3119/)  
[<image-card alt="paho-mqtt" src="https://img.shields.io/badge/mqtt-paho%202.1.0-green" ></image-card>](#)  
[<image-card alt=".NET 9.0" src="https://img.shields.io/badge/.NET-9.0-blueviolet" ></image-card>](#)  
[<image-card alt="MQTTnet" src="https://img.shields.io/badge/MQTTnet-5.0.1.1416-green" ></image-card>](#)  
[<image-card alt="Tests" src="https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework/actions/workflows/test.yml/badge.svg" ></image-card>](https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework/actions/workflows/test.yml)  
[<image-card alt="Coverage" src="https://codecov.io/gh/Jkwabena-mensah/SCADA-Digital-Twin-Framework/branch/main/graph/badge.svg" ></image-card>](https://codecov.io/gh/Jkwabena-mensah/SCADA-Digital-Twin-Framework)  
[<image-card alt="License" src="https://img.shields.io/github/license/Jkwabena-mensah/SCADA-Digital-Twin-Framework" ></image-card>](LICENSE)

## Overview
This project delivers a digital twin framework for SCADA (Supervisory Control and Data Acquisition) systems, enabling real-time simulation and monitoring of industrial processes. It progresses through defined phases, integrating Python for simulation, ASP.NET Core for backend management, and a web-based HMI dashboard.

## Phases

### Phase 1: Project Setup
- **Completed**: Initial project structure creation.
- **Details**: Established directories (`src/PythonSimulator`, `src/WebController`, `docs`, `tests`) and placeholder files for a modular SCADA framework.
- **Tech**: Python 3.11, Git.

### Phase 2: Python Simulator (Milestone 1)
- **Completed**: Real-time SCADA sensor simulation.
- **Details**:
  - Simulates Motor Amps, Temperature, and Vibration using MQTT.
  - Run: `python src/PythonSimulator/simulator.py` (requires Mosquitto broker).
  - Test: `python src/PythonSimulator/subscribe.py` for data verification.
  - Tech: paho-mqtt 2.1.0, Python 3.11.
- **Future**: Enhanced integration with the ASP.NET Core backend.

### Phase 3: ASP.NET Core Backend (Milestone 2)
- **Completed**: Development of ASP.NET Core backend for data management and MQTT integration.
- **Details**:
  - Created a new ASP.NET Core Web App named `WebController` in `src/WebController`.
  - Added packages: `Microsoft.EntityFrameworkCore.SqlServer` (9.0.10), `Microsoft.EntityFrameworkCore.InMemory` (9.0.10), `Microsoft.EntityFrameworkCore.Tools` (9.0.10), and `MQTTnet` (5.0.1.1416) for MQTT integration.
  - Defined a `SensorReading` data model in `src/WebController/WebController/Models/SensorReading.cs` (Id, AssetId, Timestamp, MotorAmps, Temperature, Vibration, Status), with nullable AssetId and Status.
  - Created an `ApplicationDbContext` in `src/WebController/WebController/Data/ApplicationDbContext.cs` with an in-memory database in `Program.cs`.
  - Implemented `MqttSubscriberService` in `src/WebController/WebController/Services/MqttSubscriberService.cs` to subscribe to `scada/sensor/data` and store data.
  - Added `SensorDataController` in `src/WebController/WebController/Controllers/SensorDataController.cs` with endpoints: `/latest`, `/stats`, `/health`.
  - Migrated to MQTTnet v5 API, resolving namespace issues (e.g., `CS0234`).
  - Run: `dotnet run` from `src/WebController/WebController` (accessible at http://localhost:5267).
  - Tech: .NET 9.0, ASP.NET Core, Entity Framework Core, MQTTnet.
- **Future**: Configure a SQL Server database and create migrations.

### Phase 4: Web Dashboard & Visualization (Chart.js Integration)
- **In Progress**: Develop a web dashboard using Razor Pages to visualize sensor data.
- **Goal**: Integrate Chart.js for real-time charts (e.g., line or bar charts) displaying Motor Amps, Temperature, and Vibration.
- **Current Step**: Step 12 - Created REST API (`SensorDataController`) for real-time data access.
- **Next Step**: Step 13 - Create the Main Dashboard Page with Razor and Chart.js.
- **Future**: Real-time data updates and user interaction features.

## Quick Start (Windows)

```powershell
git clone https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework.git
cd SCADA-Digital-Twin-Framework
py -3.11 -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r src/PythonSimulator/requirements.txt
# Install .NET 9.0 SDK if not present<a href="https://dotnet.microsoft.com/download/dotnet/9.0" target="_blank" rel="noopener noreferrer nofollow"></a>
# Start Mosquitto broker in another terminal
mosquitto
# In another terminal, run the Python simulator
python src/PythonSimulator/simulator.py
# In another terminal, navigate to the WebController and run the backend
cd src/WebController/WebController
dotnet restore
dotnet run