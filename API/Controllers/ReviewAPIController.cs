using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Repository.Interfaces;
using Repository.Libraries;
using Repository.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewAPIController : ControllerBase
    {
        private readonly IReviewsRepository _reviewRepository;
        public ReviewAPIController(IReviewsRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpPost("AddReviews")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddReviews([FromForm] Reviews.Post reviews)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please fill out the form properly.");
                }

                _reviewRepository.AddReviews(reviews);
                return Ok("Your review has been added...");
            }
            catch (UserException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("GetReviews")]
        public IActionResult GetReviews(int id)
        {
            try
            {

                var reviews = _reviewRepository.ViewLatestReviews(id);

                if (reviews == null || !reviews.Any())
                {
                    return NotFound("No reviews found.");
                }

                return Ok(reviews);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [HttpPost("DeleteReview")]
        public IActionResult DeleteReview(int id)
        {
            try
            {
                if (_reviewRepository.RemoveReview(id)) return Ok("Review has been removed");
            }
            catch (Exception e) { return BadRequest("Error: " + e.Message); }
            return BadRequest("Unexpected Error");
        }

        [HttpPut("UpdateReview")]
        public IActionResult UpdateReview([FromForm] Reviews.Post review)
        {
            try
            {
                _reviewRepository.UpdateReview(review);
                return Ok();
            }
            catch (Exception e) { return BadRequest("Error: " + e.Message); }
        }
    }
}