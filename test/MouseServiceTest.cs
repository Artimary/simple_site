using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class MouseServiceTests
    {
        [Fact]
        public async Task SaveDataAsync_CallsRepositoryAndLogger()
        {
            var mockRepository = new Mock<IMouseRepository>();
            var mockLogger = new Mock<ILogger<MouseService>>();
            var service = new MouseService(mockRepository.Object, mockLogger.Object);
            var events = new List<MouseEvent> { new MouseEvent { X = 1, Y = 2, T = 3 } };

            await service.SaveDataAsync(events);

            mockRepository.Verify(r => r.AddEventsAsync(events), Times.Once);
            mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Data saved in JSON: /app/data/mouse_events.json"),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Once);
        }
    }
}