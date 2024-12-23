using Microsoft.Extensions.Configuration;
using Npgsql;
using Repository.Interfaces;
using Repository.Libraries;
using Repository.Models;

namespace Repository.Implementations;

public class PropertyRepository : IPropertyRepository
{
    private readonly NpgsqlConnection connection;

    public PropertyRepository(IConfiguration configuration)
    {
        connection = new(configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING"));
        connection.Open();
    }

    public Properties.PropertyDetails.PropertyWithDetails GetPropertyDetails(int id)
    {
        Properties.PropertyDetails.PropertyWithDetails propertyWithDetails = new Properties.PropertyDetails.PropertyWithDetails();
        NpgsqlCommand detailCmd = new("SELECT * FROM t_properties p join t_details d on p.c_property_detail_id = d.c_property_detail_id where c_property_id = @id", connection);
        detailCmd.Parameters.AddWithValue("id", id);
        using var reader = detailCmd.ExecuteReader();
        if (reader.Read())
        {
            Properties.Get property = new()
            {
                PropertyId = id,
                ProjectId = reader["c_project_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["c_project_id"]),
                PropertyName = Convert.ToString(reader["c_property_name"]),
                PropertyAmount = Convert.ToInt32(reader["c_property_amount"]),
                InclusiveTax = Convert.ToBoolean(reader["c_inclusive_tax"]),
                PropertySize = Convert.ToInt32(reader["c_property_square_ft"]),
                Bedrooms = Convert.ToInt32(reader["c_bhk"]),
                Bathrooms = Convert.ToInt32(reader["c_bath"]),
                Floor = Convert.ToInt32(reader["c_floor"]),
                PropertyListingType = Convert.ToString(reader["c_property_listing_type"]),
                // PropertyPictures = FileHandler.GetFiles(Convert.ToString(reader["c_picture_path"])).Length == 0?FileHandler.GetFiles(Convert.ToString(reader["c_picture_path"])).ToList<string>():Array.Empty<string>().ToList(),
                VideoPath = "http://localhost:5293/" + Convert.ToString(reader["c_video_path"]),
                Furnished = Convert.ToString(reader["c_furnished"]),
                PropertyType = Convert.ToString(reader["c_property_type"]),
                AreaType = Convert.ToString(reader["c_area_type"]),
                PropertyDetailId = Convert.ToInt32(reader["c_property_detail_id"]),
                Sold = Convert.ToBoolean(reader["c_sold"]),
            };
            Properties.PropertyDetails.Get details = new()
            {
                DetailId = Convert.ToInt32(reader["c_property_detail_id"]),
                PropertyAge = Convert.ToInt32(reader["c_property_age"]),  // in years
                City = Convert.ToString(reader["c_city"]),
                Locality = Convert.ToString(reader["c_locality"]),
                ReadytoMove = Convert.ToBoolean(reader["c_availability_status"]),
                Pincode = Convert.ToInt32(reader["c_pin_code"]),
                Address = Convert.ToString(reader["c_address"]),
                PostedDate = Convert.ToDateTime(reader["c_posted_date"]),
                UserId = Convert.ToInt32(reader["c_user_id"]),
            };
            try
            {
                property.PropertyPictures = FileHandler.GetFiles(Convert.ToString(reader["c_picture_path"])).Length > 0
    ? FileHandler.GetFiles(Convert.ToString(reader["c_picture_path"])).ToList()
    : Array.Empty<string>().ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine("No found picture", ex.Message);
            }
            propertyWithDetails.Property = property;
            propertyWithDetails.Details = details;
        }
        else throw new Exception("Cannot get the property details");
        return propertyWithDetails;
    }

    public List<Properties.ListGet> GetProperties()
    {
        List<Properties.ListGet> properties = new();
        using NpgsqlCommand propertyComd = new(@"select p.c_property_id,p.c_project_id,pro.c_project_name,p.c_property_name,p.c_property_square_ft,p.c_property_amount,p.c_bhk,p.c_area_type,c_property_type,
		p.c_property_listing_type,p.c_furnished,p.c_picture_path,d.c_property_age,d.c_availability_status,d.c_address,d.c_locality,d.c_city
		from t_properties p  join t_details d on p.c_property_detail_id = d.c_property_detail_id left join t_projects pro on p.c_project_id = pro.c_project_id where p.c_sold = false", connection);
        using var reader = propertyComd.ExecuteReader();
        while (reader.Read())
        {
            Properties.ListGet property = new()
            {
                PropertyId = Convert.ToInt32(reader["c_property_id"]),
                ProjectId = reader["c_project_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["c_project_id"]),
                ProjectName = reader["c_project_name"] == DBNull.Value ? (string?)null : Convert.ToString(reader["c_project_name"]),
                PropertyName = Convert.ToString(reader["c_property_name"]),
                PropertyAmount = Convert.ToInt32(reader["c_property_amount"]),
                PropertySize = Convert.ToInt32(reader["c_property_square_ft"]),
                Bedrooms = Convert.ToInt32(reader["c_bhk"]),
                PropertyListingType = Convert.ToString(reader["c_property_listing_type"]),
                Furnished = Convert.ToString(reader["c_furnished"]),
                AreaType = Convert.ToString(reader["c_area_type"]),
                PropertyAge = Convert.ToInt32(reader["c_property_age"]),
                PropertyPicture = FileHandler.GetFirstImagePath(Convert.ToString(reader["c_picture_path"])),
                City = Convert.ToString(reader["c_city"]),
                Locality = Convert.ToString(reader["c_locality"]),
                ReadytoMove = Convert.ToBoolean(reader["c_availability_status"]),
                Address = Convert.ToString(reader["c_address"]),
                PropertyType = Convert.ToString(reader["c_property_type"])
            };
            properties.Add(property);
        }
        return properties;
    }
    private int GetCurrentPropertyId()
    {
        NpgsqlCommand command = new("SELECT last_value FROM t_properties_c_property_id_seq", connection);
        using NpgsqlDataReader reader = command.ExecuteReader();
        if (reader.Read()) return reader.GetInt16(0) + 1;
        throw new Exception("Failed to read properties data, Please try again.");
    }

    public void AddProperty(Properties.Post property, int propertyDetailsid)
    {
        string propertymediapath = FileHandler.StorePropertyImages(property.PropertyPictures, GetCurrentPropertyId());
        if (property.PropertyVideos != null) FileHandler.StorePropertyVideo(property.PropertyVideos, GetCurrentPropertyId());
        NpgsqlCommand addPropertyCommand = new("INSERT INTO t_properties(c_project_id, c_property_name, c_property_amount, c_property_square_ft, c_property_listing_type, c_picture_path, c_bhk, c_bath, c_floor, c_furnished, c_inclusive_tax, c_property_type, c_property_detail_id, c_area_type, c_video_path) VALUES(@projectid, @propertyname, @propertyamount, @propertysize, @listingtype, @picturesdirectory, @bedrooms, @bathrooms, @floor, @furnished, @tax, @propertytype, @propertydetailid, @areatype, @videopath)", connection);
        addPropertyCommand.Parameters.AddWithValue("projectid", property.ProjectId.HasValue ? property.ProjectId : DBNull.Value);
        addPropertyCommand.Parameters.AddWithValue("propertyname", property.PropertyName);
        addPropertyCommand.Parameters.AddWithValue("propertyamount", property.PropertyAmount);
        addPropertyCommand.Parameters.AddWithValue("propertysize", property.PropertySize);
        addPropertyCommand.Parameters.AddWithValue("listingtype", property.PropertyListingType);
        addPropertyCommand.Parameters.AddWithValue("picturesdirectory", propertymediapath);
        addPropertyCommand.Parameters.AddWithValue("bedrooms", property.Bedrooms);
        addPropertyCommand.Parameters.AddWithValue("bathrooms", property.Bathrooms);
        addPropertyCommand.Parameters.AddWithValue("floor", property.Floor);
        addPropertyCommand.Parameters.AddWithValue("furnished", property.Furnished);
        addPropertyCommand.Parameters.AddWithValue("tax", property.TaxInclusion);
        addPropertyCommand.Parameters.AddWithValue("propertytype", property.PropertyType);
        addPropertyCommand.Parameters.AddWithValue("propertydetailid", propertyDetailsid);
        addPropertyCommand.Parameters.AddWithValue("areatype", property.AreaType);
        addPropertyCommand.Parameters.AddWithValue("videopath", propertymediapath);
        addPropertyCommand.ExecuteNonQuery();
    }

    public void SoldProperty(int propertyid)
    {
        if (!IsPropertyExist(propertyid)) throw new Exception($"Property {propertyid} does not exist");
        NpgsqlCommand updatePropertyCommand = new("UPDATE t_properties SET c_sold = true WHERE c_property_id = @propertyid", connection);
        updatePropertyCommand.Parameters.AddWithValue("propertyid", propertyid);
        updatePropertyCommand.ExecuteNonQuery();
    }

    public void unSoldProperty(int propertyid)
    {
        if (!IsPropertyExist(propertyid)) throw new Exception($"Property {propertyid} does not exist");
        NpgsqlCommand updatePropertyCommand = new("UPDATE t_properties SET c_sold = false WHERE c_property_id = @propertyid", connection);
        updatePropertyCommand.Parameters.AddWithValue("propertyid", propertyid);
        updatePropertyCommand.ExecuteNonQuery();
    }
    public int DeleteProperty(int propertyid)
    {
        if (!IsPropertyExist(propertyid)) throw new Exception($"Property {propertyid} does not exist");
        NpgsqlCommand deletePropertyCommand = new("DELETE FROM t_properties where c_property_id = @propertyid RETURNING c_property_detail_id", connection);
        deletePropertyCommand.Parameters.AddWithValue("propertyid", propertyid);
        using NpgsqlDataReader reader = deletePropertyCommand.ExecuteReader();
        FileHandler.DeleteDirectory($"propertydata/{propertyid}");
        if (reader.Read()) return reader.GetInt16(0);
        throw new Exception("Cannot read properties data, please try again.");
    }

    public void DeleteAmenities(int propertydetailid)
    {
        NpgsqlCommand DeleteProjectAmenitiesCommand = new NpgsqlCommand(@"DELETE FROM t_property_amenities WHERE c_property_detail_id = @ProjectDetailsId", connection);
        DeleteProjectAmenitiesCommand.Parameters.AddWithValue("@ProjectDetailsId", propertydetailid);
        DeleteProjectAmenitiesCommand.ExecuteNonQuery();
    }

    public void GetProjectName(int propertyid)
    {
        throw new NotImplementedException();
    }

    public bool IsPropertyExist(int propertyid)
    {
        NpgsqlCommand checkPropertyCommand = new("SELECT * FROM t_properties WHERE c_property_id = @propertyid", connection);
        checkPropertyCommand.Parameters.AddWithValue("propertyid", propertyid);
        using NpgsqlDataReader reader = checkPropertyCommand.ExecuteReader();
        return reader.HasRows;
    }

    public int UpdateProperty(Properties.UpdateDetails property)
    {
        if (!IsPropertyExist(property.PropertyId)) throw new Exception($"Property {property.PropertyId} does not exist");
        NpgsqlCommand updatePropertyCommand = new("UPDATE t_properties SET c_property_name = @propertyname, c_property_amount = @propertyamount, c_property_square_ft = @propertysize, c_bhk = @bedrooms, c_bath = @bathrooms, c_floor = @floor, c_furnished = @furnished, c_inclusive_tax = @tax WHERE c_property_id = @propertyid RETURNING c_property_detail_id", connection);
        updatePropertyCommand.Parameters.AddWithValue("propertyname", property.PropertyName);
        updatePropertyCommand.Parameters.AddWithValue("propertyamount", property.PropertyAmount);
        updatePropertyCommand.Parameters.AddWithValue("propertysize", property.PropertySize);
        updatePropertyCommand.Parameters.AddWithValue("bedrooms", property.Bedrooms);
        updatePropertyCommand.Parameters.AddWithValue("bathrooms", property.Bathrooms);
        updatePropertyCommand.Parameters.AddWithValue("floor", property.Floor);
        updatePropertyCommand.Parameters.AddWithValue("furnished", property.Furnished);
        updatePropertyCommand.Parameters.AddWithValue("tax", property.TaxInclusion);
        updatePropertyCommand.Parameters.AddWithValue("propertyid", property.PropertyId);
        using NpgsqlDataReader reader = updatePropertyCommand.ExecuteReader();

        if (property.PropertyPictures != null) FileHandler.StorePropertyImages(property.PropertyPictures, property.PropertyId);
        if (property.PropertyVideos != null) FileHandler.StorePropertyVideo(property.PropertyVideos, property.PropertyId);
        if (reader.Read()) return reader.GetInt16(0);
        throw new Exception("Cannot update property details, please try again.");
    }

    // Property Details
    public int AddPropertyDetails(Properties.PropertyDetails.Post propertyDetails)
    {
        NpgsqlCommand addPropertyDetailsCommand = new("INSERT INTO t_details(c_property_age, c_availability_status, c_city, c_locality, c_address, c_pin_code, c_user_id) VALUES(@propertyage, @availability, @city, @locality, @address, @pincode, @userid) RETURNING c_property_detail_id", connection);
        addPropertyDetailsCommand.Parameters.AddWithValue("propertyage", propertyDetails.PropertyAge);
        addPropertyDetailsCommand.Parameters.AddWithValue("availability", propertyDetails.ReadytoMove);
        addPropertyDetailsCommand.Parameters.AddWithValue("city", propertyDetails.City);
        addPropertyDetailsCommand.Parameters.AddWithValue("locality", propertyDetails.Locality);
        addPropertyDetailsCommand.Parameters.AddWithValue("address", propertyDetails.Address);
        addPropertyDetailsCommand.Parameters.AddWithValue("pincode", propertyDetails.Pincode);
        addPropertyDetailsCommand.Parameters.AddWithValue("userid", propertyDetails.UserId);
        using NpgsqlDataReader reader = addPropertyDetailsCommand.ExecuteReader();
        if (reader.Read()) return reader.GetInt16(0);
        throw new Exception("Failed to read properties, Please try again !");
    }

    public bool IsPropertyDetailsExist(int propertydetailsid)
    {
        NpgsqlCommand checkPropertyDetailsCommand = new("SELECT * FROM t_details WHERE c_property_detail_id = @propertydetailsid", connection);
        checkPropertyDetailsCommand.Parameters.AddWithValue("propertydetailsid", propertydetailsid);
        using NpgsqlDataReader reader = checkPropertyDetailsCommand.ExecuteReader();
        return reader.HasRows;
    }

    public void DeletePropertyDetails(int propertydetailsid)
    {
        if (!IsPropertyDetailsExist(propertydetailsid)) throw new Exception($"Property details {propertydetailsid} does not exist");
        NpgsqlCommand deletePropertyDetails = new("DELETE FROM t_details WHERE c_property_detail_id = @propertydetailsid", connection);
        deletePropertyDetails.Parameters.AddWithValue("propertydetailsid", propertydetailsid);
        deletePropertyDetails.ExecuteNonQuery();
    }

    public void UpdatePropertyDetails(Properties.PropertyDetails.Update propertyDetails, int propertydetailsid)
    {
        if (!IsPropertyDetailsExist(propertydetailsid)) throw new Exception($"Property details {propertydetailsid} does not exist");
        NpgsqlCommand updatePropertyCommand = new("UPDATE t_details SET c_property_age = @propertyage, c_availability_status = @available, c_city = @city, c_locality = @locality, c_address = @address, c_pin_code = @pincode  WHERE c_property_detail_id = @propertydetailsid", connection);
        updatePropertyCommand.Parameters.AddWithValue("propertyage", propertyDetails.PropertyAge);
        updatePropertyCommand.Parameters.AddWithValue("available", propertyDetails.ReadytoMove);
        updatePropertyCommand.Parameters.AddWithValue("city", propertyDetails.City);
        updatePropertyCommand.Parameters.AddWithValue("locality", propertyDetails.Locality);
        updatePropertyCommand.Parameters.AddWithValue("address", propertyDetails.Address);
        updatePropertyCommand.Parameters.AddWithValue("pincode", propertyDetails.Pincode);
        updatePropertyCommand.Parameters.AddWithValue("propertydetailsid", propertydetailsid);
        updatePropertyCommand.ExecuteNonQuery();
    }
    public List<Properties.PropertyDetails.PropertyWithDetails> GetPropertyDetailsByUserId(int id)
    {
        List<Properties.PropertyDetails.PropertyWithDetails> properties = new();

        NpgsqlCommand detailCmd = new("SELECT * FROM t_properties p join t_details d on p.c_property_detail_id = d.c_property_detail_id where d.c_user_id = @id", connection);
        detailCmd.Parameters.AddWithValue("id", id);
        using var reader = detailCmd.ExecuteReader();
        while (reader.Read())
        {
            Properties.PropertyDetails.PropertyWithDetails propertyWithDetails = new Properties.PropertyDetails.PropertyWithDetails();
            Properties.Get property = new()
            {
                PropertyId = Convert.ToInt32(reader["c_property_id"]),
                ProjectId = reader["c_project_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["c_project_id"]),
                PropertyName = Convert.ToString(reader["c_property_name"]),
                PropertyAmount = Convert.ToInt32(reader["c_property_amount"]),
                InclusiveTax = Convert.ToBoolean(reader["c_inclusive_tax"]),
                PropertySize = Convert.ToInt32(reader["c_property_square_ft"]),
                Bedrooms = Convert.ToInt32(reader["c_bhk"]),
                Bathrooms = Convert.ToInt32(reader["c_bath"]),
                Floor = Convert.ToInt32(reader["c_floor"]),
                PropertyListingType = Convert.ToString(reader["c_property_listing_type"]),
                Furnished = Convert.ToString(reader["c_furnished"]),
                PropertyType = Convert.ToString(reader["c_property_type"]),
                AreaType = Convert.ToString(reader["c_area_type"]),
                PropertyDetailId = Convert.ToInt32(reader["c_property_detail_id"]),
                Sold = Convert.ToBoolean(reader["c_sold"]),
            };
            Properties.PropertyDetails.Get details = new()
            {
                DetailId = Convert.ToInt32(reader["c_property_detail_id"]),
                PropertyAge = Convert.ToInt32(reader["c_property_age"]),  // in years
                City = Convert.ToString(reader["c_city"]),
                Locality = Convert.ToString(reader["c_locality"]),
                ReadytoMove = Convert.ToBoolean(reader["c_availability_status"]),
                Pincode = Convert.ToInt32(reader["c_pin_code"]),
                Address = Convert.ToString(reader["c_address"]),
                PostedDate = Convert.ToDateTime(reader["c_posted_date"]),
                UserId = Convert.ToInt32(reader["c_user_id"]),
            };
            propertyWithDetails.Property = property;
            propertyWithDetails.Details = details;
            properties.Add(propertyWithDetails);
        }
        // else throw new Exception("Cannot get the property details");
        return properties;

    }
}
