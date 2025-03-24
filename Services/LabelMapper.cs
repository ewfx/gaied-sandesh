using GenAIED_Sandesh.Interfaces;
using GenAIED_Sandesh.Models;
using Microsoft.Extensions.Options;

namespace GenAIED_Sandesh.Services
{
    public class LabelMapper: ILabelMapper
    {
        private readonly List<string> _requestTypes;
        private readonly List<string> _subRequestTypes;
        private readonly IOptions<List<InputData>> _appSettingsService;
        public LabelMapper(IOptions<List<InputData>> appSettingsService)
        {
            _appSettingsService = appSettingsService;
            // Extract all unique RequestTypes and SubRequestTypes
            _requestTypes = _appSettingsService.Value.Select(q => q.RequestType).Distinct().ToList();
            _subRequestTypes = _appSettingsService.Value.Select(q => q.SubRequestType).Distinct().ToList();
        }

        public string GetRequestTypeLabel(int index)
        {
            return (index >= 0 && index < _requestTypes.Count)
                   ? _requestTypes[index]
                   : "Unknown";
        }

        public string GetSubRequestTypeLabel(int index)
        {
            return (index >= 0 && index < _subRequestTypes.Count)
                   ? _subRequestTypes[index]
                   : "Unknown";
        }
    }
}
