using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewGenerator.Lib.DTOs
{
    /// <summary>
    /// this class is used to hold the  music intrument reviews downloaded from http://jmcauley.ucsd.edu/data/amazon
    /// </summary>
    public class RawReview
    {
        public string ReviewerID { get; set; }
        public string Asin { get; set; }
        public string ReviewerName { get; set; }
        public int[] Helpful { get; set; }
        public string ReviewText { get; set; }
        public double Overall { get; set; }
        public string Summary { get; set; }
        private bool _sentimentIndicator = false;
        public bool SentimentIndicator
        {
            get
            {
                if (Convert.ToInt32(Overall) >= 3)
                {
                    _sentimentIndicator = true;
                }
                return _sentimentIndicator;
            }
        }
    }
}
