using Moq;
using Serilog;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class LoggerConfiguration
{
    public static ILogger GetLogger()
    {
        var mockLogger = new Mock<ILogger>();

        mockLogger.Setup(x => x.Information(It.IsAny<string>()));
        mockLogger.Setup(x => x.Warning(It.IsAny<string>()));
        mockLogger.Setup(x => x.Error(It.IsAny<string>()));

        return mockLogger.Object;
    }
}