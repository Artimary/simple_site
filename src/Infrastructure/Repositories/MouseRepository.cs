using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public class MouseRepository : IMouseRepository
    {
        private readonly IDbContextFactory<AppDbContext> _context;
        private readonly ILogger<MouseRepository> _logger;

        public MouseRepository(IDbContextFactory<AppDbContext> context, ILogger<MouseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddEventsAsync(List<MouseEvent> events)
        {
            await using var context = await _context.CreateDbContextAsync();
            
            var strategy = context.Database.CreateExecutionStrategy();
            
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation($"Try to save {events.Count} events");
                    
                    var mouseDataList = events.Select(e => new MouseData
                    {
                        EventJson = JsonSerializer.Serialize(e)
                    }).ToList();

                    await context.AddRangeAsync(mouseDataList);
                    var result = await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation($"Saved {result} events");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to save in DB");
                    throw;
                }
            });
        }
    }
}