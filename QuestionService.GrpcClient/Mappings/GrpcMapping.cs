using AutoMapper;
using ProtoTimestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace QuestionService.GrpcClient.Mappings;

public class GrpcMapping : Profile
{
    public GrpcMapping()
    {
        CreateMap<DateTime, ProtoTimestamp>()
            .ConvertUsing(x =>
                ProtoTimestamp.FromDateTime(DateTime.SpecifyKind(x, DateTimeKind.Utc)));

        CreateMap<ProtoTimestamp, DateTime>().ConvertUsing(x => x.ToDateTime());
    }
}