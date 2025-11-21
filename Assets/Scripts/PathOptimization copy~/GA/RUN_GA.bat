@echo off
REM Batch script to run the Trajectory GA

echo ============================================================
echo Trajectory Genetic Algorithm - C# Standalone Runner
echo ============================================================
echo.

REM Check if dotnet is installed
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found!
    echo.
    echo Please install .NET 6.0 or later from:
    echo https://dotnet.microsoft.com/download
    echo.
    echo After installation, restart your terminal and run this script again.
    echo.
    pause
    exit /b 1
)

echo [OK] .NET SDK found: 
dotnet --version
echo.

REM Run the GA
echo Starting optimization...
echo.
dotnet run %*

echo.
echo ============================================================
echo Optimization complete!
echo ============================================================
pause

