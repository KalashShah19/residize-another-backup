using Repository.Models;

namespace Repository.Interfaces;

public interface IPropertyRepository
{
    public bool IsPropertyExist(int propertyid);

    public int DeleteProperty(int propertyid);

    public int UpdateProperty(Properties.UpdateDetails propertyDetails);

    public void AddProperty(Properties.Post property, int propertyDetailsid);

    public void GetProjectName(int propertyid);
    public int AddPropertyDetails(Properties.PropertyDetails.Post propertyDetails);
    public List<Properties.PropertyDetails.PropertyWithDetails> GetPropertyDetailsByUserId(int id);
    public List<Properties.ListGet> GetProperties();

}