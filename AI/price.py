import joblib
import pandas as pd
import warnings
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestRegressor
from sklearn.metrics import mean_squared_error, r2_score

warnings.filterwarnings("ignore")

housing_data = pd.read_csv("datasets/price.csv")

selected_features = ["number of bedrooms", 
                     "number of bathrooms", 
                     "number of floors","living area",  
                     "Postal Code", "grade of the house",
                     "number of views",
                     "Area of the house(excluding basement)"]

X_price = housing_data[selected_features]
y_price = housing_data["Price"]

X_price = pd.get_dummies(X_price, columns=["Postal Code"], drop_first=True)

X_price.fillna(X_price.mean(), inplace=True)

X_price_train, X_price_test, y_price_train, y_price_test = train_test_split(X_price, y_price, test_size=0.3, random_state=42)

price_model = RandomForestRegressor(random_state=42)
price_model.fit(X_price_train, y_price_train)

price_predictions = price_model.predict(X_price_test)
price_rmse = mean_squared_error(y_price_test, price_predictions, squared=False)
price_r2 = r2_score(y_price_test, price_predictions)

# print(f"Price Prediction RMSE: {price_rmse}")
print(f"Price Accuracy (R²): {price_r2}")

sample_input = {
    "number of bedrooms": [2],
    "number of bathrooms": [2],
    "number of floor": [1],
    "number of floors": [1],
    "living area": [880],
    "Area of the house(excluding basement)": [880],
    "Postal Code": [122006],
    "grade of the house": [8],
    "number of views": [0],
}

sample_df = pd.DataFrame(sample_input)

sample_df = pd.get_dummies(sample_df, columns=["Postal Code"], drop_first=True)

missing_cols = set(X_price.columns) - set(sample_df.columns)
for col in missing_cols:
    sample_df[col] = 0
sample_df = sample_df[X_price.columns]

predicted_price = price_model.predict(sample_df)
print(f"Predicted Price: ₹{predicted_price[0]}")

joblib.dump(price_model, "models/price_model.pkl")