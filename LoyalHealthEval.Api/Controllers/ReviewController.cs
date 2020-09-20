using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyalHealthEval.Api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReviewGenerator.Lib;

namespace LoyalHealthEval.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {

        private readonly ILogger<ReviewController> _logger;

        public ReviewController(ILogger<ReviewController> logger)
        {
            _logger = logger;
        }

        
        [HttpGet("GenerateReview")]
        [ActionName("GenerateReview")] 
        public IActionResult GenerateReview()
        {
            try
            {
                var review = ReviewGenerator.Lib.ReviewGenerator.GenerateReview();
                return Ok(review);
            }
            catch (Exception ex)
            {
                return ex.HandleException(HttpContext);
            }
        }


        [HttpGet("GenerateReviewWithSentimentAnalysis")]
        [ActionName("GenerateReviewWithSentimentAnalysis")]
        public IActionResult GenerateReviewWithSentimentAnalysis()
        {
            try
            {
                var review = ReviewGenerator.Lib.ReviewGenerator.GenerateReviewWithSentimentAnalysis();
                return Ok(review);
            }
            catch (Exception ex)
            {
                return ex.HandleException(HttpContext);
            }
        }


    }
}