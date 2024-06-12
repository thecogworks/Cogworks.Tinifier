namespace Cogworks.Tinifier.Models.API;

public class OrganisedMediaModel
{
  public Media Media { get; set; }
  public IEnumerable<string> DestinationPath { get; set; } = new List<string>();
}