using Microsoft.AspNetCore.Mvc;
using Repository.Implementations;
using Repository.Libraries;
using Repository.Models;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyAPIController : ControllerBase
{
    private readonly PropertyRepository propertyRepository;
    private readonly ProjectRepository projectRepository;
    public PropertyAPIController(PropertyRepository propertyRepository, ProjectRepository projectRepository)
    {
        this.propertyRepository = propertyRepository;
        this.projectRepository = projectRepository;
    }

    [HttpPost("AddProperty")]
    public IActionResult AddProperty([FromForm] Properties.Post property, [FromForm] Properties.PropertyDetails.Post propertyDetails, [FromForm] List<int> ammenties)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest("Please provide property details id");
            int propertydetailsid = propertyRepository.AddPropertyDetails(propertyDetails);
            propertyRepository.AddProperty(property, propertydetailsid);
            projectRepository.AllAmenities(new() { AllAmenities = ammenties, ProjectDetailsId = propertydetailsid });
            return Ok("Property added successfully");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpPut("UpdateProperty")]
    public IActionResult UpdateProperty([FromForm] Properties.UpdateDetails property, [FromForm] Properties.PropertyDetails.Update propertyDetails)
    {
        try
        {
            int propertydetailid = propertyRepository.UpdateProperty(property);
            propertyRepository.UpdatePropertyDetails(propertyDetails, propertydetailid);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpDelete("DeleteFile")]
    public IActionResult DeleteFile(string path)
    {
        try
        {
            FileHandler.DeleteFile(path);
            return Ok("File deleted successfully");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpGet("GetFiles")]
    public IActionResult GetImages(string path)
    {
        try
        {
            string[] files = FileHandler.GetFiles(path);
            return Ok(files);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpDelete("DeleteProperty")]
    public IActionResult DeleteProperty(int propertyid)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest("Please provide a property id");
            int propertydetailsid = propertyRepository.DeleteProperty(propertyid);
            propertyRepository.DeletePropertyDetails(propertydetailsid);
            propertyRepository.DeleteAmenities(propertydetailsid);
            return Ok($"Property {propertyid} deleted successfully");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    // TODO
    [HttpDelete("DeleteImage")]
    public IActionResult DeleteImage(string path)
    {
        FileHandler.DeleteDirectory(path);
        return Ok();
    }

    [HttpGet("GetPropertyByUserId")]
    public IActionResult GetPropertyByUserId(int id)
    {
        try
        {
            return Ok(propertyRepository.GetPropertyDetailsByUserId(id));
        }
        catch (Exception ex)
        {
            return BadRequest("Cannot get the property" + ex.Message);
        }
    }

    [HttpPost("MarkasSold")]
    public IActionResult MarkasSold(int propertyid)
    {
        propertyRepository.SoldProperty(propertyid);
        return Ok();
    }

    [HttpPost("MarkasunSold")]
    public IActionResult MarkasunSold(int propertyid)
    {
        propertyRepository.unSoldProperty(propertyid);
        return Ok();
    }

    [HttpGet("ViewProperty")]
    public IActionResult ViewProperty()
    {
        try
        {
            var properties = propertyRepository.GetProperties();
            return Ok(properties);
        }
        catch (Exception ex)
        {
            return BadRequest("Not found: " + ex.Message);
        }
    }

    [HttpGet("GetPropertyById")]
    public IActionResult GetPropertyById(int id)
    {
        try
        {
            return Ok(propertyRepository.GetPropertyDetails(id));
        }
        catch (Exception ex)
        {
            return BadRequest("Cannot get the property" + ex.Message);
        }
    }
}