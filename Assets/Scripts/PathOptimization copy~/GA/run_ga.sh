#!/bin/bash

echo "============================================================"
echo "Trajectory Genetic Algorithm - C# Standalone Runner"
echo "============================================================"
echo

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "[ERROR] .NET SDK not found!"
    echo
    echo "Please install .NET 6.0 or later from:"
    echo "https://dotnet.microsoft.com/download"
    echo
    echo "After installation, restart your terminal and run this script again."
    echo
    exit 1
fi

echo "[OK] .NET SDK found: $(dotnet --version)"
echo

# Run the GA
echo "Starting optimization..."
echo
dotnet run "$@"

echo
echo "============================================================"
echo "Optimization complete!"
echo "============================================================"

