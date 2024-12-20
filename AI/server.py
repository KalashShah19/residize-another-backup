from flask import Flask, request, jsonify
import joblib
from flask_cors import CORS
import pandas as pd

model = joblib.load("models/price_model.pkl")
rent_model = joblib.load('models/rent_model.pkl')
scaler = joblib.load('models/scaler.pkl')

app = Flask(__name__)
CORS(app)

def round_to_nearest_hundred(number):
    remainder = number % 100
    if remainder < 50:
        rounded_number = number - remainder
    else:
        rounded_number = number + (100 - remainder)
    return rounded_number

@app.route("/predict_price", methods=["POST"])
def predict_price():
    try:
        data = request.get_json()
        print(data)
        bedrooms = data["number of bedrooms"]
        bathrooms = data["number of bathrooms"]
        floors = data["number of floors"]
        living_area = data["living area"]
        postal_code = str(data["Postal Code"])

        sample_df = pd.DataFrame({
            "number of bedrooms": [bedrooms],
            "number of bathrooms": [bathrooms],
            "number of floors": [floors],
            "living area": [living_area],
            "Postal Code": [postal_code],
            "grade of the house": [7],
            "number of views": [0],
        })

        sample_df = pd.get_dummies(sample_df, columns=["Postal Code"], drop_first=True)

        missing_cols = set(model.feature_names_in_.tolist()) - set(sample_df.columns)
        for col in missing_cols:
            sample_df[col] = 0

        sample_df = sample_df[model.feature_names_in_.tolist()]

        predicted_price = model.predict(sample_df)[0]
        
        predicted_price = round_to_nearest_hundred(predicted_price)

        return jsonify({"price": predicted_price})

    except KeyError as e:
        return jsonify({"error": f"Error: {e}"}), 400
    

@app.route('/predict_rent', methods=['POST'])
def predict_rent():
    try:
        data = request.get_json()
        print(data)

        bhk = data['BHK']
        living_area = data['LivingArea']
        area_type = data['AreaType']
        furnishing_status = data['FurnishingStatus']
        tenant_preferred = data['TenantPreferred']
        bathroom = data['Bathrooms']
        point_of_contact = data['PointOfContact']
        postal_code = data['PostalCode']

        input_data = pd.DataFrame({
            'BHK': [bhk],
            'Living Area': [living_area],
            'Area Type': [area_type],
            'Furnishing Status': [furnishing_status],
            'Tenant Preferred': [tenant_preferred],
            'Bathroom': [bathroom],
            'Point of Contact': [point_of_contact],
            'Postal Code': [postal_code]
        })

        input_data_scaled = scaler.transform(input_data)

        predicted_rent = round_to_nearest_hundred(rent_model.predict(input_data_scaled))

        return jsonify({'rent': predicted_rent[0]})

    except Exception as e:
        return jsonify({'error': str(e)})
    
if __name__ == "__main__":
    app.run(debug=True)