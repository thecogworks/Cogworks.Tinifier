namespace Cogworks.Tinifier.Infrastructure;

public static class PublishedContentExtensions
{
  public static bool HasValue(this IPublishedContent content)
    => content is not null;
}