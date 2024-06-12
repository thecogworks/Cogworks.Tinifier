namespace Cogworks.Tinifier.Models.API;

public class TinyInput
{
  /// <summary>
  /// Origin size of image
  /// </summary>
  public int Size { get; set; }

  /// <summary>
  /// Type of Image
  /// </summary>
  public string Type { get; set; } = string.Empty;
}