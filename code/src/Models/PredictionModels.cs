using Microsoft.ML.Data;

namespace GenAIED_Sandesh.Models
{
    public class PredictionModels
    {
    }
    public class RequestTypePrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedRequestType { get; set; } // Predicted RequestType

        [ColumnName("Score")]
        public float[] RequestTypeScores { get; set; } // Confidence scores for RequestType

    }

    // Define output prediction class for SubRequestType
    public class SubRequestTypePrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedSubRequestType { get; set; } // Predicted SubRequestType

        [ColumnName("Score")]
        public float[] SubRequestTypeScores { get; set; } // Confidence scores for SubRequestType
    }
}
