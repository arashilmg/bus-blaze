using Google.Protobuf.WellKnownTypes;

namespace BizCover.Api.Renewals.ProtoMappers;

public static class DateTimeExtension
{
    public static Timestamp ToGrpcTimestamp(this DateTime dateTime)
        => Timestamp.FromDateTime(dateTime);
}
