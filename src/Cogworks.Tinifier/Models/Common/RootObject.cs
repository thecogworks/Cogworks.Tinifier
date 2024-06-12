namespace Cogworks.Tinifier.Models.Common;

public class RootObject
{
  public Properties Properties { get; set; } = new Properties();
  public CultureData CultureData { get; set; } = new CultureData();
  public string UrlSegment { get; set; } = string.Empty;
}