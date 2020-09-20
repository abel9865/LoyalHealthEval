using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ReviewGenerator.Lib.DTOs;
using ReviewGenerator.Lib.Helper;

namespace ReviewGenerator.Lib
{
    /// <summary>
    /// Used to load the  music intrument reviews downloaded from http://jmcauley.ucsd.edu/data/amazon
    /// </summary>
    public class TrainingDataLoader
    {
        public static List<RawReview> GetReviews()
        {
            List<RawReview> reviews = null;
            var assembly = Assembly.GetExecutingAssembly();
            var test = assembly.GetManifestResourceNames();
            var resourceName = "ReviewGenerator.Lib.RawReviewData.Musical_Instruments_5.json";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string fileContent = reader.ReadToEnd();
                reviews = JsonExtensions.FromNewlineDelimitedJson<RawReview>(new StringReader(fileContent)).ToList();

            }

            return reviews;
        }


    }
}
