using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces.Log;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Repositories.Log
{
    public class JsonErrorLogRepository : IErrorLogRepository
    {
        private readonly string _filePath;
        private static List<ErrorLog> _logs = new List<ErrorLog>();


        public JsonErrorLogRepository(IConfiguration configuration)
        {
            _filePath = configuration["ActivityLogSettings:ErrorJsonFilePath"];

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

        public async Task ErrorLogAsync(ErrorLog log)
        {
            
            _logs.Add(log);

            string updatedJson = JsonSerializer.Serialize(_logs, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            await File.WriteAllTextAsync(_filePath, updatedJson);
        }


        public List<ErrorLog> GetErrorLog()
        {
            return _logs;
        }



        /// <summary>
        /// use to load data in constructor
        /// </summary>
        private void InitializedLogs()
        {
            if (File.Exists(_filePath))
            {
                string jsonContent = File.ReadAllText(_filePath);
                _logs = JsonSerializer.Deserialize<List<ErrorLog>>(jsonContent) ?? new List<ErrorLog>();
            }
            else
            {
                _logs = new List<ErrorLog>();
            }
        }

    }
}
