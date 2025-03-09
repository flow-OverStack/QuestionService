using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace QuestionService.Grpc.Mapping;

public class GrpcMapping : Profile
{
    public GrpcMapping()
    {
        CreateMap<DateTime, Timestamp>()
            .ConvertUsing(x =>
                Timestamp.FromDateTime(DateTime.SpecifyKind(x, DateTimeKind.Utc)));

        CreateMap<Timestamp, DateTime>().ConvertUsing(x => x.ToDateTime());
    }
}