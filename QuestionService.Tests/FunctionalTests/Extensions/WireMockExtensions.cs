using WireMock.Server;

namespace QuestionService.Tests.FunctionalTests.Extensions;

public static class WireMockExtensions
{
    public static void StopServer(this WireMockServer server)
    {
        server.Stop();
    }
}