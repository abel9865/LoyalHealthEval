using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace ReviewGenerator.Lib.DTOs.SentimentAnalysis
{
    public class SentimentPrediction: SentimentData
    {
        //prediction class used after model training
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        //the likelihood of the text having positive or negative sentiment
        public float Probability { get; set; }
        //raw score calculated by model
        public float Score { get; set; }
    }
}
