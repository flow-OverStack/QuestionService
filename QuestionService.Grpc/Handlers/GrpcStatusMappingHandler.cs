using System.Net;
using Grpc.Core;

namespace QuestionService.Grpc.Handlers;

public class GrpcStatusMappingHandler : DelegatingHandler
{
    private const string GrpcStatusHeaderName = "grpc-status";

    private static readonly IReadOnlyDictionary<StatusCode, HttpStatusCode> StatusCodeMapping =
        new Dictionary<StatusCode, HttpStatusCode>
        {
            { StatusCode.OK, HttpStatusCode.OK },
            { StatusCode.Cancelled, HttpStatusCode.RequestTimeout },
            { StatusCode.Unknown, HttpStatusCode.InternalServerError },
            { StatusCode.InvalidArgument, HttpStatusCode.BadRequest },
            { StatusCode.DeadlineExceeded, HttpStatusCode.RequestTimeout },
            { StatusCode.NotFound, HttpStatusCode.NotFound },
            { StatusCode.AlreadyExists, HttpStatusCode.Conflict },
            { StatusCode.PermissionDenied, HttpStatusCode.Forbidden },
            { StatusCode.Unauthenticated, HttpStatusCode.Unauthorized },
            { StatusCode.ResourceExhausted, HttpStatusCode.TooManyRequests },
            { StatusCode.FailedPrecondition, HttpStatusCode.BadRequest },
            { StatusCode.Aborted, HttpStatusCode.Conflict },
            { StatusCode.OutOfRange, HttpStatusCode.BadRequest },
            { StatusCode.Unimplemented, HttpStatusCode.NotImplemented },
            { StatusCode.Internal, HttpStatusCode.InternalServerError },
            { StatusCode.Unavailable, HttpStatusCode.ServiceUnavailable },
            { StatusCode.DataLoss, HttpStatusCode.InternalServerError }
        };

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        var grpcStatusStr = response.Headers.GetValues(GrpcStatusHeaderName).FirstOrDefault();

        if (!Enum.TryParse<StatusCode>(grpcStatusStr, out var grpcStatus)) return response;

        var httpStatus = StatusCodeMapping.GetValueOrDefault(grpcStatus, HttpStatusCode.InternalServerError);

        response.StatusCode = httpStatus;
        return response;
    }
}