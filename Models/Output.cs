namespace GenAIED_Sandesh.Models
{
    public class PredictionOutput
    {
        public string EmailName {get;set;}
        public string EmailExtension {get;set;}
        public string EmailAttachments {get;set;}  
        public string ExtractedText {get;set;}
        public string PredictedRequestType {get;set;}
        public string PredictedRequestTypeConfidenceScore { get; set; }
        public string RequestTypeConfidenceScores {get;set;}    

        public string PredictedSubRequestType { get; set; }
        public string PredictedSubRequestTypeConfidenceScore { get; set; }
        public string SubRequestTypeConfidenceScores {get;set;}

    }
}
