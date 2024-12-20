import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestRegressor
from sklearn.preprocessing import StandardScaler
from sklearn.metrics import mean_squared_error, r2_score
import joblib

data = pd.read_csv('datasets/new_rent.csv')

X = data.drop('Rent', axis=1)

y = data['Rent']

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)


model = RandomForestRegressor(n_estimators=100, random_state=42)
model.fit(X_train_scaled, y_train)

y_pred = model.predict(X_test_scaled)

mse = mean_squared_error(y_test, y_pred)
r2 = r2_score(y_test, y_pred)

print(f"Mean Squared Error: {mse}")
print(f"R-squared: {r2}")

joblib.dump(model, 'models/rent_model.pkl')
joblib.dump(scaler, 'models/scaler.pkl')

new_data = pd.DataFrame({
    'BHK': [1],
    'Living Area': [800],
    'Area Type': [1],  # Carpet Area (your encoding)
    'Furnishing Status': [0],  # Furnished (your encoding)
    'Tenant Preferred': [0],  # Bachelors/Family (your encoding)
    'Bathroom': [1],
    'Point of Contact': [0],  # Contact Owner (your encoding)
    'Postal Code': [400001]   # Mumbai postal code
})

new_data_scaled = scaler.transform(new_data)

predicted_rent = model.predict(new_data_scaled)

print(f"Predicted Rent for new data: {predicted_rent[0]}")
