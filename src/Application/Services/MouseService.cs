using System.Text.Json;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class MouseService : IMouseService
    {
        private readonly IMouseRepository _repository;
        private readonly ILogger<MouseService> _logger;
    private const string JsonFilePath = "/app/data/mouse_events.json";

        public MouseService(IMouseRepository repository, ILogger<MouseService> logger)
        {
            _repository = repository;
            _logger = logger;

            Directory.CreateDirectory(Path.GetDirectoryName(JsonFilePath)!);
        }

        public async Task SaveDataAsync(List<MouseEvent> events)
        {
            await _repository.AddEventsAsync(events);

            var jsonData = JsonSerializer.Serialize(events, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            await File.AppendAllTextAsync(JsonFilePath, jsonData + Environment.NewLine);
            _logger.LogInformation($"Data saved in JSON: {JsonFilePath}");
        }
    }
}