using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using QuestionService.DAL.Repositories;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Outbox.Interfaces.TopicProducers;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.FunctionalTests.Base.Exception;

public class ExceptionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private static IMock<IDbContextTransaction> GetExceptionMockTransaction(IDbContextTransaction originalTransaction)
    {
        var mockTransaction = new Mock<IDbContextTransaction>();

        mockTransaction.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(originalTransaction.RollbackAsync);
        mockTransaction.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new TestException());

        return mockTransaction;
    }

    private static async Task<IMock<IUnitOfWork>> GetExceptionMockUnitOfWork(IUnitOfWork originalUnitOfWork)
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var originalTransaction = await originalUnitOfWork.BeginTransactionAsync();
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(originalUnitOfWork.SaveChangesAsync);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetExceptionMockTransaction(originalTransaction).Object);
        mockUnitOfWork.Setup(x => x.Questions).Returns(originalUnitOfWork.Questions);
        mockUnitOfWork.Setup(x => x.Votes).Returns(originalUnitOfWork.Votes);

        return mockUnitOfWork;
    }

    private static IMock<ITopicProducerResolver> GetExceptionTopicProducerResolver()
    {
        var mockResolver = new Mock<ITopicProducerResolver>();

        mockResolver.Setup(x => x.GetProducerForType(It.IsAny<Type>())).Throws(new TestException());

        return mockResolver;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IUnitOfWork>();
            services.AddScoped<IUnitOfWork>(provider =>
            {
                //the dependencies from service provider only apply for this current scope
                //that is why we have to use ActivatorUtilities to transfer dependencies from this scope to callers' scope
                var unitOfWork = ActivatorUtilities.CreateInstance<UnitOfWork>(provider);
                var exceptionUnitOfWork = GetExceptionMockUnitOfWork(unitOfWork).GetAwaiter().GetResult().Object;

                return exceptionUnitOfWork;
            });

            services.RemoveAll<ITopicProducerResolver>();
            services.AddScoped<ITopicProducerResolver>(provider =>
            {
                var exceptionTopicProducerResolver = GetExceptionTopicProducerResolver().Object;

                return exceptionTopicProducerResolver;
            });
        });
    }
}