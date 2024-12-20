using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Repository.Interfaces;
using Repository.Libraries;
using Repository.Models;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAPIController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        public AdminAPIController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        [HttpGet("TotalUsers")]
        public IActionResult TotalUsers()
        {
            try
            {
               
                int users = _adminRepository.TotalUsers();

                if (users == null)
                {
                    return NotFound("No users found.");
                }

                return Ok(users); 
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        [HttpGet("TotalProperties")]
        public IActionResult TotalProperties()
        {
            try
            {
               
                int properties = _adminRepository.TotalProperties();

                if (properties == null)
                {
                    return NotFound("No properties found.");
                }

                return Ok(properties); 
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}