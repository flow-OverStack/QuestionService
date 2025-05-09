using System.Linq.Expressions;
using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Comparers;

/// <summary>
///     View comparer that compares views by their content
/// </summary>
public class UniqueViewComparer : IEqualityComparer<View>
{
    public bool Equals(View? x, View? y)
    {
        if (x == null || y == null) return false;

        return ViewsEqual(x, y);
    }

    public int GetHashCode(View obj)
    {
        return HashCode.Combine(obj.QuestionId, obj.UserId, obj.UserIp, obj.UserFingerprint);
    }

    public static Expression<Func<View, bool>> ViewEquals(View y)
    {
        return x =>
            x.QuestionId == y.QuestionId &&
            x.UserId == y.UserId &&
            x.UserIp == y.UserIp &&
            x.UserFingerprint == y.UserFingerprint;
    }

    private static bool ViewsEqual(View x, View y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);

        return ViewEquals(y).Compile()(x);
    }
}