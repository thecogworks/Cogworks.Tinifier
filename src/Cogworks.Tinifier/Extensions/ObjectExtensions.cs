namespace Cogworks.Tinifier.Extensions;

public static class ObjectExtensions
{
  public static bool HasValue(this object obj)
  {
    return obj is not null;
  }
}