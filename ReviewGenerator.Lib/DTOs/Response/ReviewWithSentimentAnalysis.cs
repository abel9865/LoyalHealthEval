using System;
using System.Collections.Generic;
using System.Text;
using static ReviewGenerator.Lib.DTOs.enumerations;

namespace ReviewGenerator.Lib.DTOs.Response
{
    /// <summary>
    /// used to hold response data when a request to generate a fake review with sentiment analysis is processed
    /// </summary>
    public class ReviewWithSentimentAnalysis:Review
    {
        public enumPredictionSentiment Prediction { get; set; }
        public double Probability { get; set; }
    }
}
