namespace Cogworks.Tinifier.Extensions;

public static class StringExtensions
{
  public static bool HasValue(this string input)
    => !string.IsNullOrWhiteSpace(input);
}