# SCADA Digital Twin Framework

An Open-Source Python/ASP.NET Core Framework for Simulating and Monitoring Industrial SCADA Systems – built by **Joseph Kwabena Mensah, PE-GhIE #09875**.

[<image-card alt="Python 3.11" src="https://img.shields.io/badge/python-3.11-blue.svg" ></image-card>](https://www.python.org/downloads/release/python-3119/)
[<image-card alt="paho-mqtt" src="https://img.shields.io/badge/mqtt-paho%202.1.0-green" ></image-card>](#)
[<image-card alt="Tests" src="https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework/actions/workflows/test.yml/badge.svg" ></image-card>](https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework/actions/workflows/test.yml)
[<image-card alt="Coverage" src="https://codecov.io/gh/Jkwabena-mensah/SCADA-Digital-Twin-Framework/branch/main/graph/badge.svg" ></image-card>](https://codecov.io/gh/Jkwabena-mensah/SCADA-Digital-Twin-Framework)
[<image-card alt="License" src="https://img.shields.io/github/license/Jkwabena-mensah/SCADA-Digital-Twin-Framework" ></image-card>](LICENSE)

## Overview
This project delivers a digital twin framework for SCADA systems, enabling real-time simulation and monitoring of industrial processes. It progresses through defined phases, with Phase 1 establishing the foundation and Phase 2 introducing simulation capabilities.

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
  - Test: `python subscribe.py` for data verification.
  - Tech: paho-mqtt 2.1.0, Python 3.11.
- **Future**: Integration with ASP.NET Core.

## Quick Start (Windows)

```powershell
git clone https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework.git
cd SCADA-Digital-Twin-Framework
py -3.11 -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r src/PythonSimulator/requirements.txt
# Start Mosquitto broker
mosquitto
# In another terminal
python src/PythonSimulator/simulator.py
# Dashboard
uvicorn src/WebController/app:app --reload



## Contributing
Contributions are welcome! Please fork the repository, create a feature branch, and submit a pull request. See the [Contributing Guidelines](CONTRIBUTING.md) for details.


## Author
**Joseph Kwabena Mensah** – [LinkedIn](https://linkedin.com/in/joseph-kwabena-mensah) – joemenz143@gmail.com

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.