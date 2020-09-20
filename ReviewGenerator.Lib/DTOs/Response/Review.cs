using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewGenerator.Lib.DTOs.Response
{
    /// <summary>
    /// used to hold response data when a request to generate a fake review is processed
    /// </summary>
    public class Review
    {
        public string ReviewText { get; set; }
        public int ReviewRating { get; set; }
    }
}
