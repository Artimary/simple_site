using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class MouseRepositoryTests
    {
        [Fact]
        public async Task AddEventsAsync_ShouldAddEventsToContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            var mockContext = new Mock<AppDbContext>(options);
            var mockDbSet = new Mock<DbSet<MouseData>>();
            mockContext.Setup(m => m.Data).Returns(mockDbSet.Object);

            var mockDatabase = new Mock<DatabaseFacade>(mockContext.Object);
            mockContext.Setup(m => m.Database).Returns(mockDatabase.Object);

            var testExecutionStrategy = new TestExecutionStrategy(mockContext.Object);
            mockDatabase.Setup(d => d.CreateExecutionStrategy()).Returns(testExecutionStrategy);

            var mockTransaction = new Mock<IDbContextTransaction>();
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockTransaction.Object);

            mockDbSet.Setup(m => m.AddRangeAsync(
                It.IsAny<IEnumerable<MouseData>>(),
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);

            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            mockContext.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<MouseData>>(), It.IsAny<CancellationToken>()))
                .Returns<IEnumerable<MouseData>, CancellationToken>((entities, ct) => mockDbSet.Object.AddRangeAsync(entities, ct));

            var mockContextFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockContextFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockContext.Object);
            
            var mockLogger = new Mock<ILogger<MouseRepository>>();
            var repository = new MouseRepository(mockContextFactory.Object, mockLogger.Object);
            var events = new List<MouseEvent> { new MouseEvent { X = 1, Y = 2, T = 3 } };

            await repository.AddEventsAsync(events);

            mockDbSet.Verify(m => m.AddRangeAsync(
                It.IsAny<IEnumerable<MouseData>>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());

            mockContext.Verify(m => m.SaveChangesAsync(
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        private class TestExecutionStrategy : IExecutionStrategy
        {
            private readonly DbContext _context;

            public TestExecutionStrategy(DbContext context)
            {
                _context = context;
            }

            public bool RetriesOnFailure => false;

            public TResult Execute<TState, TResult>(
                TState state,
                Func<DbContext, TState, TResult> operation,
                Func<DbContext, TState, ExecutionResult<TResult>>? verifySucceeded)
            {
                return operation(_context, state);
            }

            public async Task<TResult> ExecuteAsync<TState, TResult>(
                TState state,
                Func<DbContext, TState, CancellationToken, Task<TResult>> operation,
                Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded,
                CancellationToken cancellationToken = default)
            {
                return await operation(_context, state, cancellationToken);
            }
        }
    }
}