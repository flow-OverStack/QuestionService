using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Comparers;

public class ViewComparer : IEqualityComparer<View>
{
    public bool Equals(View? x, View? y)
    {
        if (x == null || y == null) return false;

        return x.QuestionId == y.QuestionId &&
               x.UserId == y.UserId &&
               x.UserIp == y.UserIp &&
               x.UserFingerprint == y.UserFingerprint;
    }

    public int GetHashCode(View obj)
    {
        return HashCode.Combine(obj.QuestionId, obj.UserId, obj.UserIp, obj.UserFingerprint);
    }
}