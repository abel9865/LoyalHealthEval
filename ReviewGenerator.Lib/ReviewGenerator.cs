using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markov;
using Microsoft.ML;
using Microsoft.ML.Data;
using ReviewGenerator.Lib.DTOs.Response;
using ReviewGenerator.Lib.DTOs.SentimentAnalysis;
using static Microsoft.ML.DataOperationsCatalog;
using static ReviewGenerator.Lib.DTOs.enumerations;

namespace ReviewGenerator.Lib
{
    public class ReviewGenerator
    {
        public static MarkovChain<string> _markovChain;
        private static ITransformer _sentimentAnalysisModel;
        private static MLContext _mlContext = new MLContext();


        //load raw reviews, load markov generator and sentiment Analysis model 
        public static void Init()
        {

            //load raw reviews 
            var reviews = TrainingDataLoader.GetReviews();
            //var filteredReviews = reviews.Where(x => !x.reviewText.Contains(')') && x.reviewText.Split('.').Count()<=2).Skip(skipRandom).Take(20).ToList();
            var filteredReviews = reviews.Where(x => !x.ReviewText.Contains(')') && x.ReviewText.Split('.').Count() <= 1).Take(20).ToList();

            //initialize and load reviews to Markov generator
            _markovChain = new MarkovChain<string>(1);

            Random random = new Random();
            int skipRandom = random.Next(500, 9000);

            foreach (var review in filteredReviews)
            {
                _markovChain.Add(review.ReviewText.Split(' '), 1);
            }


            //Train sentiment analysis model
            var sentimentData = (from x in filteredReviews
                                 select new SentimentData()
                                 {
                                     SentimentText = x.ReviewText,
                                     Sentiment = x.SentimentIndicator
                                 }).ToList();
            TrainTestData splitDataView = LoadSentimentData(_mlContext, sentimentData);
            _sentimentAnalysisModel = BuildAndTrainSentimentModel(_mlContext, splitDataView.TrainSet);
            
            
            //Evaluate(_mlContext, _sentimentAnalysisModel, splitDataView.TestSet);
            //var sentimentAnalysisResult = PredictReviewSentiment(_mlContext, _sentimentAnalysisModel, "This is good");
           
        }



        public static Review GenerateReview()
        {

            var review = GenerateReviewText();
            Random random = new Random();
            return new Review() { ReviewText = review, ReviewRating = random.Next(1, 5) };
        }

        public static ReviewWithSentimentAnalysis GenerateReviewWithSentimentAnalysis()
        {
            var review = GenerateReviewText();
            var reviewWithSentimentAnalysis = PredictReviewSentiment(_mlContext, _sentimentAnalysisModel, review);
            return reviewWithSentimentAnalysis;
        }


        public static string GenerateReviewText()
        {
            var rand = new Random();

            var inferredFakeReview = string.Join(" ", _markovChain.Chain(rand));
            var fakeReview = ToCorrectSentenceCase(inferredFakeReview);
            return fakeReview;
        }

        #region Sentiment Analysis

        public static TrainTestData LoadSentimentData(MLContext mlContext, List<SentimentData> reviews)
        {
            IDataView dataView = _mlContext.Data.LoadFromEnumerable<SentimentData>(reviews);
            TrainTestData splitDataView = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            return splitDataView;
        }

        public static ITransformer BuildAndTrainSentimentModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            //Create and Train the Model
            var model = estimator.Fit(splitTrainSet);
            //End of training

            return model;
        }

        public static void EvaluateSentimentModel(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            IDataView predictions = model.Transform(splitTestSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

        }

        private static ReviewWithSentimentAnalysis PredictReviewSentiment(MLContext mlContext, ITransformer model, string sentimentText)
        {
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = sentimentText
            };

            var resultPrediction = predictionFunction.Predict(sampleStatement);

            var reviewWithSentimentAnalysis = new ReviewWithSentimentAnalysis()
            {
                ReviewText = resultPrediction.SentimentText,
                Prediction = (Convert.ToBoolean(resultPrediction.Prediction) ? enumPredictionSentiment.Positive : enumPredictionSentiment.Negative),
                Probability = resultPrediction.Probability
            };

            //logic uses to set rating based on sentiment analysis
            switch (reviewWithSentimentAnalysis.Prediction)
            {
                case enumPredictionSentiment.Positive:
                    if (reviewWithSentimentAnalysis.Probability > 0.8)
                    {
                        reviewWithSentimentAnalysis.ReviewRating = 5;
                    }
                    else if (reviewWithSentimentAnalysis.Probability > 0.6 && reviewWithSentimentAnalysis.Probability < 0.8)
                    {
                        reviewWithSentimentAnalysis.ReviewRating = 4;
                    }
                    else
                    {
                        reviewWithSentimentAnalysis.ReviewRating = 3;
                    }
                    break;
                case enumPredictionSentiment.Negative:
                    if (reviewWithSentimentAnalysis.Probability > 0.8)
                    {
                        reviewWithSentimentAnalysis.ReviewRating = 1;
                    }
                    else if (reviewWithSentimentAnalysis.Probability > 0.6 && reviewWithSentimentAnalysis.Probability < 0.8)
                    {
                        reviewWithSentimentAnalysis.ReviewRating = 2;
                    }
                    else
                    {
                        reviewWithSentimentAnalysis.ReviewRating = 3;
                    }
                    break;
            }

            // var result = ($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");
            return reviewWithSentimentAnalysis;
        }


        #endregion


        #region Sentence Formatter
        private static string ToCorrectSentenceCase(string someString)
        {
            var sb = new StringBuilder(someString.Length);
            bool wasPeriodLastSeen = true; // We want first letter to be capitalized
            foreach (var c in someString)
            {
                if (wasPeriodLastSeen && !char.IsWhiteSpace(c))
                {
                    sb.Append(char.ToUpper(c));
                    wasPeriodLastSeen = false;
                }
                else
                {

                    if (c == '.')  // you may want to expand this to other punctuation
                        wasPeriodLastSeen = true;
                    sb.Append(char.ToLower(c));
                }
            }

            return sb.ToString();
        }
        #endregion


    }
}
