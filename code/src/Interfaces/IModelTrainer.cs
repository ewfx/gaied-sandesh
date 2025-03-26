using GenAIED_Sandesh.Models;

namespace GenAIED_Sandesh.Interfaces
{
    public interface IModelTrainer
    {
        public void CreateModelsAndSave(List<InputData> data);
        List<PredictionOutput> PredictData();
    }
}
