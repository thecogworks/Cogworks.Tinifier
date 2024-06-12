namespace Cogworks.Tinifier.Extensions;

public static class EnumerableExtensions
{
  public static bool HasAny<T>(this IEnumerable<T> items)
  {
    return items is not null && items.Any();
  }
}