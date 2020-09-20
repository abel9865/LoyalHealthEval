using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace ReviewGenerator.Lib.DTOs.SentimentAnalysis
{
    public class SentimentData
    {
        public string SentimentText;
        //value is either positive or negative - 1(positive) 0(negative)
        [ColumnName("Label")]
        public bool Sentiment;
    }
}
