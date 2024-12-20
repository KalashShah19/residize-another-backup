import requests
import json

url = 'http://127.0.0.1:5000/predict_rent'

data = {
    'BHK': 1,
    'Living Area': 800,
    'Area Type': 1,  # Carpet Area (encoded value)
    'Furnishing Status': 0,  # Furnished (encoded value)
    'Tenant Preferred': 1,  # Bachelors/Family (encoded value)
    'Bathroom': 1,
    'Point of Contact': 0,  # Contact Owner (encoded value)
    'Postal Code': 700001   # Postal code
}

response = requests.post(url, json=data)

if response.status_code == 200:
    result = response.json()
    print(f"Predicted Rent: {result['rent']}")
else:
    print(f"Error: {response.status_code}, {response.text}")