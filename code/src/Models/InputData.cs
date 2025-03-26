
namespace GenAIED_Sandesh.Models
{
    public class InputData
    {
        public string Text { get; set; } // Input text
        public string RequestType { get; set; } // Primary label (e.g., TypeA, TypeB)
        public string SubRequestType { get; set; } // Secondary label (e.g., SubType1, SubType2)
    }  

}
