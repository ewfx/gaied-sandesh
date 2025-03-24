using GenAIED_Sandesh.Interfaces;
using GenAIED_Sandesh.Models;
using Microsoft.Extensions.Configuration;

namespace GenAIED_Sandesh.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        public List<InputData> ReadAppSettings()
        {
            var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            var bankingQueries = config.GetSection("BankingQueries")
                                      .Get<List<InputData>>();
            return bankingQueries;
        }
    }
}
