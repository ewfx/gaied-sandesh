using GenAIED_Sandesh.Interfaces;
using GenAIED_Sandesh.Models;
using GenAIED_Sandesh.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GenAIED_Sandesh.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailClassifierController : ControllerBase
    {
       
        private readonly ILogger<EmailClassifierController> _logger;
        private readonly IModelTrainer _modelTrainer;
        private readonly IEmailExtractorService _emailExtractor;
        private readonly IOptions<List<InputData>> _appSettingsService;
        private readonly IWebHostEnvironment _env;

        public EmailClassifierController(ILogger<EmailClassifierController> logger,
            IModelTrainer modelTrainer,
            IEmailExtractorService emailExtractor,
            IOptions<List<InputData>> appSettingsService,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _emailExtractor = emailExtractor;
            _modelTrainer = modelTrainer;
            _appSettingsService = appSettingsService;
            _env = env;
        }

        [HttpGet("CreateModelsAndSave")]
        public bool CreateModelsAndSave()
        {
            var trainingData=_appSettingsService.Value;
            _modelTrainer.CreateModelsAndSave(trainingData);
            return true;
        }

        [HttpGet("PredictData")]
        public bool PredictData()
        {
            _modelTrainer.PredictData();
            return true;
        }
    }
}
