using GenAIED_Sandesh.Interfaces;
using GenAIED_Sandesh.Services;
using Microsoft.AspNetCore.Mvc;

namespace GenAIED_Sandesh.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailClassifierController : ControllerBase
    {
       
        private readonly ILogger<EmailClassifierController> _logger;
        private readonly IModelTrainer _modelTrainer;
        private readonly IEmailExtractorService _emailExtractor;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IWebHostEnvironment _env;

        public EmailClassifierController(ILogger<EmailClassifierController> logger,
            IModelTrainer modelTrainer,
            IEmailExtractorService emailExtractor,
            IAppSettingsService appSettingsService,
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
            var trainingData=_appSettingsService.ReadAppSettings();
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
