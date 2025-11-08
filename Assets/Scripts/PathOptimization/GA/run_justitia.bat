@echo off
echo ============================================================
echo Justitia Trajectory GA - Windows Runner
echo ============================================================
echo.

dotnet run --project TrajectoryJustitia.csproj %*

echo.
echo ============================================================
echo Complete! Now run: python plot_justitia_trajectory.py
echo ============================================================
pause

