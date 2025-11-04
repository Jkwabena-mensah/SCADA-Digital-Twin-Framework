# SCADA Digital Twin Framework

An Open-Source Python/ASP.NET Core Framework for Simulating and Monitoring Industrial SCADA Systems – built by **Joseph Kwabena Mensah, PE-GhIE #09875**.

[![Python 3.11](https://img.shields.io/badge/python-3.11-blue.svg)](https://www.python.org/downloads/release/python-3119/)
[![paho-mqtt](https://img.shields.io/badge/mqtt-paho%202.1.0-green)](#)
[![Tests](https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework/actions/workflows/test.yml/badge.svg)](https://github.com/Jkwabena-mensah/SCADA-Digital-Twin-Framework/actions/workflows/test.yml)
[![Coverage](https://codecov.io/gh/Jkwabena-mensah/SCADA-Digital-Twin-Framework/branch/main/graph/badge.svg)](https://codecov.io/gh/Jkwabena-mensah/SCADA-Digital-Twin-Framework)
[![License](https://img.shields.io/github/license/Jkwabena-mensah/SCADA-Digital-Twin-Framework)](LICENSE)

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