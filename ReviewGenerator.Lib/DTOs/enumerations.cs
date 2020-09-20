using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewGenerator.Lib.DTOs
{
    public class enumerations
    {
        /// <summary>
        /// used during sentiment analysis to predict whether a review is positive or negative
        /// </summary>
        public enum enumPredictionSentiment
        {
            Negative,
            Positive
        }
    }
}
