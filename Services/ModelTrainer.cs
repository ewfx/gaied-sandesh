using GenAIED_Sandesh.Interfaces;
using GenAIED_Sandesh.Models;
using Microsoft.ML;

namespace GenAIED_Sandesh.Services
{
    public class ModelTrainer:IModelTrainer
    {
        private readonly IWebHostEnvironment _env;
        private readonly IEmailExtractorService _emailExtractorService;
        private readonly ILabelMapper _labelMapper;
        public ModelTrainer(IWebHostEnvironment env, IEmailExtractorService emailExtractorService, ILabelMapper labelMapper)
        {
            _env = env;
            _emailExtractorService = emailExtractorService;
            _labelMapper = labelMapper;
        }
        public void CreateModelsAndSave(List<InputData> data)
        {
            var mlContext = new MLContext(seed: 0);

            // Step 3: Load data into IDataView
            var dataView = mlContext.Data.LoadFromEnumerable(data);

            // Step 4: Define the pipeline for RequestType
            var requestTypePipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(InputData.Text)) // Convert text to features
                .Append(mlContext.Transforms.Conversion.MapValueToKey("RequestType")) // Convert RequestType to key
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "RequestType", featureColumnName: "Features")) // Train RequestType model
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel")); // Convert predicted key back to original label

            // Step 5: Train the RequestType model
            Console.WriteLine("Training RequestType model...");
            var requestTypeModel = requestTypePipeline.Fit(dataView);

            var path = _env.ContentRootPath+ "/AIModel";
            string modelPath = Path.Combine(path, "requestTypeModel.zip");
            mlContext.Model.Save(requestTypeModel, dataView.Schema, modelPath);

            // Step 6: Define the pipeline for SubRequestType
            var subRequestTypePipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(InputData.Text)) // Convert text to features
                .Append(mlContext.Transforms.Conversion.MapValueToKey("SubRequestType")) // Convert SubRequestType to key
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "SubRequestType", featureColumnName: "Features")) // Train SubRequestType model
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel")); // Convert predicted key back to original label

            // Step 7: Train the SubRequestType model
            Console.WriteLine("Training SubRequestType model...");
            var subRequestTypeModel = subRequestTypePipeline.Fit(dataView);

            modelPath = Path.Combine(path, "subRequestTypeModel.zip");
            mlContext.Model.Save(subRequestTypeModel, dataView.Schema, modelPath);

        }

        public List<PredictionOutput> PredictData()
        {
            var mlContext = new MLContext(seed: 0);
            var path=_env.ContentRootPath;
            string rModelPath = Path.Combine(path, "AIModel/requestTypeModel.zip");
            var requestTypeModel = mlContext.Model.Load(rModelPath, out var rModelSchema);
            string sRModelPath = Path.Combine(path, "AIModel/subRequestTypeModel.zip");
            var sRequestTypeModel = mlContext.Model.Load(sRModelPath, out var srModelSchema);
            // Step 8: Create prediction engines
            var requestTypePredictionEngine = mlContext.Model.CreatePredictionEngine<InputData, RequestTypePrediction>(requestTypeModel);
            var subRequestTypePredictionEngine = mlContext.Model.CreatePredictionEngine<InputData, SubRequestTypePrediction>(sRequestTypeModel);

            var listText = _emailExtractorService.ReadEmailsAndAttachment();
            // Step 9: Test a prediction
            foreach (var data in listText)
            {
                var requestTypePrediction = requestTypePredictionEngine.Predict(new InputData { Text = data.ExtractedText });
                var subRequestTypePrediction = subRequestTypePredictionEngine.Predict(new InputData { Text = data.ExtractedText });

                // Step 10: Display results
                Console.WriteLine($"\nInput Text: \"{data}\"");
                Console.WriteLine("\n-----------------------");
                Console.WriteLine("Request Type Prediction:");
                Console.WriteLine("-----------------------");
                Console.WriteLine($"Predicted: {requestTypePrediction.PredictedRequestType}");
                Console.WriteLine($"Confidence: {requestTypePrediction.RequestTypeScores.Max():P2}");

               

                // Get and sort RequestType scores in descending order
                var sortedRequestScores = requestTypePrediction.RequestTypeScores
                    .Select((value, index) => new
                    {
                        Label = _labelMapper.GetRequestTypeLabel(index),
                        Score = value
                    })
                    .OrderByDescending(x => x.Score)
                    .ToList();

                Console.WriteLine("\nAll RequestType Scores (Descending Order):");
                foreach (var score in sortedRequestScores)
                {
                    Console.WriteLine($"- {score.Label}: {score.Score:P2}");
                }
                data.PredictedRequestType = requestTypePrediction.PredictedRequestType;
                data.PredictedRequestTypeConfidenceScore = requestTypePrediction.RequestTypeScores.Max().ToString();
                data.RequestTypeConfidenceScores = string.Join(",", sortedRequestScores);


                Console.WriteLine("\n--------------------------");
                Console.WriteLine("SubRequest Type Prediction:");
                Console.WriteLine("--------------------------");
                Console.WriteLine($"Predicted: {subRequestTypePrediction.PredictedSubRequestType}");
                Console.WriteLine($"Confidence: {subRequestTypePrediction.SubRequestTypeScores.Max():P2}");              


                // Get and sort SubRequestType scores in descending order
                var sortedSubRequestScores = subRequestTypePrediction.SubRequestTypeScores
                    .Select((value, index) => new
                    {
                        Label = _labelMapper.GetSubRequestTypeLabel(index),
                        Score = value
                    })
                    .OrderByDescending(x => x.Score)
                    .ToList();

                Console.WriteLine("\nAll SubRequestType Scores (Descending Order):");
                foreach (var score in sortedSubRequestScores)
                {
                    Console.WriteLine($"- {score.Label}: {score.Score:P2}");
                }

                data.PredictedSubRequestType = subRequestTypePrediction.PredictedSubRequestType;
                data.PredictedRequestTypeConfidenceScore = subRequestTypePrediction.SubRequestTypeScores.Max().ToString();
                data.SubRequestTypeConfidenceScores = string.Join(",", sortedSubRequestScores);
                Console.WriteLine("\n========================================\n");               
            }
             return listText;
        }
    }
}
