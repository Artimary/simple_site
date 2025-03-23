using Application.Interfaces;
using Controllers;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Save_CallsMouseServiceAndLogs()
        {
            var mockMouseService = new Mock<IMouseService>();
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockMouseService.Object, mockLogger.Object);
            var events = new List<MouseEvent> { new MouseEvent { X = 1, Y = 2, T = 3 } };

            var result = await controller.Save(events);

            mockMouseService.Verify(m => m.SaveDataAsync(events), Times.Once);
            mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v!.ToString().Contains($"Received {events.Count} events")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Once);

            if (events.Any())
            {
                var firstEvent = events.First();
                var expectedMessage = $"First event details: X={firstEvent.X}, Y={firstEvent.Y}, Time={firstEvent.T}";
                mockLogger.Verify(l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v!.ToString() == expectedMessage),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Once);
            }
            Assert.IsType<OkResult>(result);
        }
    }
}