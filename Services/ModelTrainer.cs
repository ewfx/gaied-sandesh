using GenAIED_Sandesh.Interfaces;
using GenAIED_Sandesh.Models;
using Microsoft.ML;

namespace GenAIED_Sandesh.Services
{
    public class ModelTrainer:IModelTrainer
    {
        private readonly IWebHostEnvironment _env;
        private readonly IEmailExtractorService _emailExtractorService;
        public ModelTrainer(IWebHostEnvironment env, IEmailExtractorService emailExtractorService)
        {
            _env = env;
            _emailExtractorService = emailExtractorService;
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

        public void PredictData()
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
                var requestTypePrediction = requestTypePredictionEngine.Predict(new InputData { Text = data });
                var subRequestTypePrediction = subRequestTypePredictionEngine.Predict(new InputData { Text = data });

                // Step 10: Display results
                Console.WriteLine($"Text: {data}");
                Console.WriteLine($"Predicted RequestType: {requestTypePrediction.PredictedRequestType}");
                Console.WriteLine($"RequestType Scores: [{string.Join(", ", requestTypePrediction.RequestTypeScores)}]");
                Console.WriteLine($"Predicted SubRequestType: {subRequestTypePrediction.PredictedSubRequestType}");
                Console.WriteLine($"SubRequestType Scores: [{string.Join(", ", subRequestTypePrediction.SubRequestTypeScores)}]");
            }

        }
    }
}
