"""
Test script for the Planet Position API
Demonstrates how to make requests to the API
"""

import requests
import json
from datetime import datetime

# API base URL
BASE_URL = "http://localhost:8000"


def test_health_check():
    """Test the health check endpoint"""
    print("\n" + "="*70)
    print("TEST 1: Health Check")
    print("="*70)
    
    response = requests.get(f"{BASE_URL}/health")
    
    print(f"Status Code: {response.status_code}")
    print(f"Response:")
    print(json.dumps(response.json(), indent=2))
    
    return response.status_code == 200


def test_get_all_coords():
    """Test getting all planet coordinates"""
    print("\n" + "="*70)
    print("TEST 2: Get All Planet Coordinates")
    print("="*70)
    
    params = {
        "date": "05-11-2025",
        "time": "14:30:00"
    }
    
    response = requests.get(f"{BASE_URL}/get_coords", params=params)
    
    print(f"Request URL: {response.url}")
    print(f"Status Code: {response.status_code}")
    
    if response.status_code == 200:
        data = response.json()
        print(f"\nDate: {data['date']}")
        print(f"Time: {data['time']}")
        print(f"Units: {data['units']}")
        print(f"\nPlanet Positions:")
        print(f"{'Planet':<12} {'X (m)':<20} {'Y (m)':<20} {'Z (m)':<20}")
        print("-" * 70)
        
        for planet, coords in data['planets'].items():
            print(f"{planet.capitalize():<12} {coords['x']:<20.2e} {coords['y']:<20.2e} {coords['z']:<20.2e}")
    else:
        print(f"Error: {response.text}")
    
    return response.status_code == 200


def test_get_single_planet():
    """Test getting a single planet's coordinates"""
    print("\n" + "="*70)
    print("TEST 3: Get Single Planet Coordinates (Earth)")
    print("="*70)
    
    planet = "earth"
    params = {
        "date": "01-01-2025",
        "time": "00:00:00"
    }
    
    response = requests.get(f"{BASE_URL}/get_coords/{planet}", params=params)
    
    print(f"Request URL: {response.url}")
    print(f"Status Code: {response.status_code}")
    
    if response.status_code == 200:
        data = response.json()
        print(f"\nPlanet: {data['planet'].capitalize()}")
        print(f"Date: {data['date']}")
        print(f"Time: {data['time']}")
        print(f"\nPosition:")
        print(f"  X: {data['position']['x']:.2e} metres")
        print(f"  Y: {data['position']['y']:.2e} metres")
        print(f"  Z: {data['position']['z']:.2e} metres")
        
        # Calculate distance from Sun
        x, y, z = data['position']['x'], data['position']['y'], data['position']['z']
        distance = (x**2 + y**2 + z**2)**0.5
        print(f"\nDistance from Sun: {distance:.2e} metres")
        print(f"Distance from Sun: {distance / 149597870700:.6f} AU")
    else:
        print(f"Error: {response.text}")
    
    return response.status_code == 200


def test_invalid_date():
    """Test error handling with invalid date"""
    print("\n" + "="*70)
    print("TEST 4: Error Handling - Invalid Date")
    print("="*70)
    
    params = {
        "date": "32-13-2025",  # Invalid date
        "time": "14:30:00"
    }
    
    response = requests.get(f"{BASE_URL}/get_coords", params=params)
    
    print(f"Request URL: {response.url}")
    print(f"Status Code: {response.status_code}")
    print(f"Response:")
    print(json.dumps(response.json(), indent=2))
    
    return response.status_code == 400


def test_invalid_planet():
    """Test error handling with invalid planet name"""
    print("\n" + "="*70)
    print("TEST 5: Error Handling - Invalid Planet")
    print("="*70)
    
    planet = "pluto"  # Not included
    params = {
        "date": "05-11-2025",
        "time": "14:30:00"
    }
    
    response = requests.get(f"{BASE_URL}/get_coords/{planet}", params=params)
    
    print(f"Request URL: {response.url}")
    print(f"Status Code: {response.status_code}")
    print(f"Response:")
    print(json.dumps(response.json(), indent=2))
    
    return response.status_code == 404


def test_compare_two_dates():
    """Test comparing planet positions at different times"""
    print("\n" + "="*70)
    print("TEST 6: Compare Positions at Different Times")
    print("="*70)
    
    # Get positions on two different dates
    date1 = "01-01-2025"
    date2 = "01-02-2025"
    time = "00:00:00"
    
    response1 = requests.get(f"{BASE_URL}/get_coords/mercury", params={"date": date1, "time": time})
    response2 = requests.get(f"{BASE_URL}/get_coords/mercury", params={"date": date2, "time": time})
    
    if response1.status_code == 200 and response2.status_code == 200:
        data1 = response1.json()
        data2 = response2.json()
        
        pos1 = data1['position']
        pos2 = data2['position']
        
        dx = pos2['x'] - pos1['x']
        dy = pos2['y'] - pos1['y']
        dz = pos2['z'] - pos1['z']
        
        distance = (dx**2 + dy**2 + dz**2)**0.5
        
        print(f"Mercury's position on {date1}:")
        print(f"  X: {pos1['x']:.2e} m")
        print(f"  Y: {pos1['y']:.2e} m")
        print(f"  Z: {pos1['z']:.2e} m")
        
        print(f"\nMercury's position on {date2}:")
        print(f"  X: {pos2['x']:.2e} m")
        print(f"  Y: {pos2['y']:.2e} m")
        print(f"  Z: {pos2['z']:.2e} m")
        
        print(f"\nMovement in one month:")
        print(f"  ΔX: {dx:.2e} m")
        print(f"  ΔY: {dy:.2e} m")
        print(f"  ΔZ: {dz:.2e} m")
        print(f"  Total distance: {distance:.2e} m")
        print(f"  Total distance: {distance / 1000:.2e} km")
        
        return True
    else:
        print("Error fetching data")
        return False


def test_current_time():
    """Test getting positions for current date and time"""
    print("\n" + "="*70)
    print("TEST 7: Get Positions for Current Date/Time")
    print("="*70)
    
    # Get current date and time
    now = datetime.now()
    date = now.strftime("%d-%m-%Y")
    time = now.strftime("%H:%M:%S")
    
    print(f"Current Date: {date}")
    print(f"Current Time: {time}")
    
    params = {
        "date": date,
        "time": time
    }
    
    response = requests.get(f"{BASE_URL}/get_coords", params=params)
    
    print(f"\nStatus Code: {response.status_code}")
    
    if response.status_code == 200:
        data = response.json()
        print(f"\nSuccessfully retrieved positions for {len(data['planets'])} planets")
        print(f"Earth's current position:")
        earth = data['planets']['earth']
        print(f"  X: {earth['x']:.2e} m")
        print(f"  Y: {earth['y']:.2e} m")
        print(f"  Z: {earth['z']:.2e} m")
        return True
    else:
        print(f"Error: {response.text}")
        return False


def main():
    """Run all tests"""
    print("\n" + "="*70)
    print("PLANET POSITION API - TEST SUITE")
    print("="*70)
    print("\nMake sure the API server is running on http://localhost:8000")
    print("Start it with: python api_server.py")
    
    input("\nPress Enter to start tests...")
    
    results = []
    
    try:
        # Run tests
        results.append(("Health Check", test_health_check()))
        results.append(("Get All Coordinates", test_get_all_coords()))
        results.append(("Get Single Planet", test_get_single_planet()))
        results.append(("Invalid Date Error", test_invalid_date()))
        results.append(("Invalid Planet Error", test_invalid_planet()))
        results.append(("Compare Two Dates", test_compare_two_dates()))
        results.append(("Current Time", test_current_time()))
        
        # Print summary
        print("\n" + "="*70)
        print("TEST SUMMARY")
        print("="*70)
        
        for test_name, passed in results:
            status = "✓ PASSED" if passed else "✗ FAILED"
            print(f"{test_name:<30} {status}")
        
        passed_count = sum(1 for _, passed in results if passed)
        total_count = len(results)
        
        print("\n" + "="*70)
        print(f"Total: {passed_count}/{total_count} tests passed")
        print("="*70 + "\n")
        
    except requests.exceptions.ConnectionError:
        print("\n" + "="*70)
        print("ERROR: Could not connect to API server")
        print("="*70)
        print("\nMake sure the server is running:")
        print("  python api_server.py")
        print("\nOr start it with uvicorn:")
        print("  uvicorn api_server:app --reload")
        print("="*70 + "\n")


if __name__ == "__main__":
    main()

