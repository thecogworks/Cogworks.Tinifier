namespace Cogworks.Tinifier.Models.Common;

public class UmbracoNode
{
  public int Id { get; set; }
  public string UniqueId { get; set; } = string.Empty;
  public int ParentId { get; set; }
  public int Level { get; set; }
  public string Path { get; set; } = string.Empty;
  public int SortOrder { get; set; }
  public bool Trashed { get; set; }
  public string Text { get; set; } = string.Empty;
}