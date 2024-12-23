using Microsoft.AspNetCore.Mvc;
using Repository.Implementations;
using Repository.Interfaces;
using Repository.Models;
using Repository.Libraries;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectApiController : ControllerBase
    {
        public readonly IProjectRepository _IProjectRepository;
        public readonly PropertyRepository propertyRepository;

        public ProjectApiController(IProjectRepository IProjectRepository, PropertyRepository PropertyRepository)
        {

            _IProjectRepository = IProjectRepository;
            propertyRepository = PropertyRepository;
        }

        [HttpPost("AddProject")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddProject([FromForm] Combined.PropertyDetails model)
        {
            if (ModelState.IsValid)
            {
                var propertyDetailsPost = new Properties.PropertyDetails.Post
                {
                    PropertyAge = model.PropertyAge,
                    City = model.City,
                    Locality = model.Locality,
                    ReadytoMove = model.ReadytoMove,
                    Address = model.Address,
                    Pincode = model.Pincode,
                    UserId = model.UserId
                };

                try
                {
                    if (!ModelState.IsValid) return BadRequest("Please provide property details id");
                    int propertydetailsid = propertyRepository.AddPropertyDetails(propertyDetailsPost);

                    var Amenities = new Project.Amenities
                    {

                        AllAmenities = model.AllAmenities,
                        ProjectDetailsId = propertydetailsid,

                    };

                    var project = new Project.Post
                    {
                        ProjectName = model.ProjectName,
                        ProjectDetailsId = propertydetailsid,
                        Files = model.Files,
                        VideoFiles = model.VideoFiles,
                        BorchureFile = model.BorchureFile,

                    };
                    int ProjectId = _IProjectRepository.AddProject(project);
                    _IProjectRepository.AllAmenities(Amenities);



                    return Ok("Data submitted successfully.");
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
                }


            }
            else
            {
                return BadRequest("Invalid data.");
            }
        }

        [HttpDelete("DeleteProject")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProject(int ProjectId)
        {

            _IProjectRepository.DeleteProject(ProjectId);
            return Ok();
        }

        [HttpPost("UpdateProject")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProject(Project.Update model)
        {

            _IProjectRepository.UpdateProject(model);
            return Ok();
        }

        [HttpPost("AssetsMange")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AssetsMange(string path)
        {

            _IProjectRepository.AssetsMange(path);
            return Ok();
        }

        [HttpGet("GetProject")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProject(int id)
        {
            var project = _IProjectRepository.GetProjects(id);

            if (project == null)
            {
                return NotFound(); // Return a 404 if the project is not found
            }

            return Ok(project);
        }


        [HttpPost("ProjectName")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ProjectName([FromBody] string Name)
        {
            Console.WriteLine(Name);
            if (_IProjectRepository.DoesProjectExist(Name))
                throw new UserException("This Project Name is already registered.");

            return Ok();
        }

        [HttpGet("GetAllProjects")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllProjects()
        {
            var project = _IProjectRepository.GetAllProjects();

            if (project == null)
            {
                return NotFound(); // Return a 404 if the project is not found
            }

            return Ok(project);
        }

        [HttpGet("GetProjectByProjectId")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProjectByProjectId(int id)
        {
            var project = _IProjectRepository.GetProjectByProjectId(id);

            if (project == null)
            {
                return NotFound(); // Return a 404 if the project is not found
            }

            return Ok(project);
        }



        [HttpGet("GetPropertiesByProjectId")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPropertiesByProjectId(int projectId)
        {
            var properties = _IProjectRepository.GetPropertiesByProjectId(projectId);

            if (properties == null)
            {
                return NotFound(); // Return a 404 if the project is not found
            }

            return Ok(properties);
        }
        [HttpGet("GetProjectAmenitiesByProjectId")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProjectAmenitiesByProjectId(int projectId)
        {
            var amenities = _IProjectRepository.GetProjectAmenitiesByProjectId(projectId);

            if (amenities == null)
            {
                return NotFound(); // Return a 404 if the project is not found
            }

            return Ok(amenities);
        }

    }
}