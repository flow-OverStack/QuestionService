using WireMock.Server;

namespace QuestionService.Tests.FunctionalTests.Extensions;

internal static class WireMockExtensions
{
    public static void StopServer(this WireMockServer server)
    {
        server.Stop();
    }
}