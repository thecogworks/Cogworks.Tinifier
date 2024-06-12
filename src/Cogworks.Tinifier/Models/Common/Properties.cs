namespace Cogworks.Tinifier.Models.Common;

public class Properties
{
  public List<UmbracoFile> UmbracoFile { get; set; } = new List<UmbracoFile>();
  public List<UmbracoWidth> UmbracoWidth { get; set; } = new List<UmbracoWidth>();
  public List<UmbracoHeight> UmbracoHeight { get; set; } = new List<UmbracoHeight>();
  public List<UmbracoByte> UmbracoBytes { get; set; } = new List<UmbracoByte>();
  public List<UmbracoExtension> UmbracoExtension { get; set; } = new List<UmbracoExtension>();
}