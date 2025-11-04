# SCADA Digital Twin Framework

This project delivers a digital twin framework for SCADA (Supervisory Control and Data Acquisition) systems, enabling real-time simulation and monitoring of industrial processes. It progresses through defined phases, integrating Python for simulation, ASP.NET Core for backend management, and upcoming web visualization.

## Project Overview

- **Languages**: C# (ASP.NET Core), Python (tests/simulator)
- **Framework**: .NET 9.0
- **Dependencies**: 
  - `Microsoft.EntityFrameworkCore.SqlServer` (9.0.10)
  - `Microsoft.EntityFrameworkCore.InMemory` (9.0.10)
  - `MQTTnet` (5.0.1.1416)
- **Database**: In-memory (for testing), SQL Server (configurable)
- **Real-time Data**: MQTT integration for sensor data
- **GitHub**: [Jkwabena-mensah/SCADA-Digital-Twin-Framework](https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework)

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
  - Added packages: `Microsoft.EntityFrameworkCore.SqlServer` (9.0.10) for database support, `Microsoft.EntityFrameworkCore.Tools` (9.0.10) for migrations, `Microsoft.EntityFrameworkCore.InMemory` (9.0.10) for testing, and `MQTTnet` (5.0.1.1416) for MQTT integration.
  - Defined a `SensorReading` data model in `src/WebController/WebController/Models/SensorReading.cs` to store sensor data (Id, AssetId, Timestamp, MotorAmps, Temperature, Vibration, Status), with AssetId and Status as nullable fields.
  - Created an `ApplicationDbContext` in `src/WebController/WebController/Data/ApplicationDbContext.cs` to manage the database, registered with an in-memory database in `Program.cs`.
  - Developed `MqttSubscriberService` in `src/WebController/WebController/Services/MqttSubscriberService.cs` to subscribe to MQTT topics (e.g., `scada/sensor/data`) and store data in the database.
  - Resolved namespace issues (e.g., `CS0234`) by migrating to MQTTnet v5 API, removing deprecated `MQTTnet.Client`, and adjusting payload handling.
  - Run: `dotnet run` from `src/WebController/WebController` (accessible at http://localhost:5000).
  - Tech: .NET 9.0, ASP.NET Core, Entity Framework Core, MQTTnet.
- **Future**: Configure a SQL Server database, create migrations, and optimize MQTT integration.

### Phase 4: Web Dashboard & Visualization (Chart.js Integration)
- **Upcoming**: Develop a web dashboard using Razor Pages to visualize sensor data.
- **Goal**: Integrate Chart.js for real-time charts (e.g., line or bar charts) displaying sensor readings.
- **Next Steps**: Create views to render charts, connect to the `SensorReadings` data, and update the service to push data to the frontend.

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