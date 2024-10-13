using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace EmailCampaign.Infrastructure.Data.Repositories.Log
{
    public class JsonActivityLogRepository : IActivityLogRepository
    {
        private readonly string _filePath;
        private static List<ActivityLog> _logs = new List<ActivityLog>();

        public JsonActivityLogRepository(IConfiguration configuration)
        {
            _filePath = configuration["ActivityLogSettings:ActivityjsonFilePath"];
            //_filePath = "wwwroot/LogsFile/activity_log.json";
            if (string.IsNullOrEmpty(_filePath))
            {
                throw new ArgumentException("Activity log file path is not configured.");
            }

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            InitializedLogs();
        }


        public async Task LogActivityAsync(ActivityLog log)
        {

            _logs.Add(log);

            string updatedJson = JsonSerializer.Serialize(_logs, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            await File.WriteAllTextAsync(_filePath, updatedJson);
        }

        public List<ActivityLog> GetActivityLog()
        {
            return _logs;
        }



        /// <summary>
        /// 
        /// </summary>
        private void InitializedLogs()
        {
            if (File.Exists(_filePath))
            {
                string jsonContent =  File.ReadAllText(_filePath);
                _logs = JsonSerializer.Deserialize<List<ActivityLog>>(jsonContent) ?? new List<ActivityLog>();
            }
            else
            {
                _logs = new List<ActivityLog>();
            }
        }

    }
}
