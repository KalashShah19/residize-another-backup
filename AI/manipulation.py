from sklearn.preprocessing import LabelEncoder
import pandas as pd

data = pd.read_csv('datasets/rent.csv')

distinct_cities = data['City'].unique()
num = data['City'].nunique()
count = data['Area Locality'].nunique()
floors = data['Floor'].nunique()
area_types = data['Area Type'].nunique()
area_type = data['Area Type'].unique()
status = data['Furnishing Status'].unique()
tenant = data['Tenant Preferred'].unique()
contact = data['Point of Contact'].unique()

print("Distinct cities :")
print(distinct_cities)
print("Number of areas :")
print(count)
print("Floors :")
print(floors)
print("Num of Cities :")
print(num)
print("Number of Area Types :")
print(area_types)
print("Area Types :")
print(area_type)
print("Furnishing Status :")
print(status)
print("Tenant Preferred :")
print(tenant)
print("Contact :")
print(contact)

# Manipulations

city_postal_code_mapping = {
    'Mumbai': 400001,
    'Kolkata': 700001,
    'Bangalore': 560001,
    'Delhi': 110001,
    'Chennai': 600001,
    'Hyderabad': 500001
}

data['Postal Code'] = data['City'].map(city_postal_code_mapping)
data = data.drop(columns=['Posted On'])
data = data.drop(columns=['Floor'])
data = data.drop(columns=['Area Locality'])
data = data.drop(columns=['City'])
# print(data)

data = data.rename(columns={'Size': 'Living Area'})

area_type_mapping = {'Super Area': 0, 'Carpet Area': 1, 'Built Area': 2}
data['Area Type'] = data['Area Type'].map(area_type_mapping)

furnishing_status_mapping = {'Furnished': 0, 'Semi-Furnished': 1, 'Unfurnished': 2}
data['Furnishing Status'] = data['Furnishing Status'].map(furnishing_status_mapping)

tenant_preferred_mapping = {'Bachelors': 0, 'Bachelors/Family': 1, 'Family': 2}
data['Tenant Preferred'] = data['Tenant Preferred'].map(tenant_preferred_mapping)

point_of_contact_mapping = {'Contact Owner': 0, 'Contact Agent': 1, 'Contact Builder': 2}
data['Point of Contact'] = data['Point of Contact'].map(point_of_contact_mapping)

print(data.head())

# data.to_csv('datasets/new_rent.csv', index=False)

# Correlation
correlation_matrix = data.corr()

correlation_with_rent = correlation_matrix['Rent'].sort_values(ascending=False)

print("Correlation with Rent:")
print(correlation_with_rent)