namespace Cogworks.Tinifier.Models.API;

public class TinyResponse
{
  /// <summary>
  /// Input nonOptimized image
  /// </summary>
  public TinyInput Input { get; set; } = new TinyInput();

  /// <summary>
  /// Output optimized image
  /// </summary>
  public TinyOutput Output { get; set; } = new TinyOutput();
}