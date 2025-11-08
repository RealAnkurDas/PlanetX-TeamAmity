#!/bin/bash

echo "============================================================"
echo "Justitia Trajectory GA - Bash Runner"
echo "============================================================"
echo ""
echo "Starting optimization..."
echo ""

dotnet run --project TrajectoryJustitia.csproj "$@"

echo ""
echo "============================================================"
echo "Complete! Now run: python plot_justitia_trajectory.py"
echo "============================================================"

