namespace Cogworks.Tinifier.Models.API;

/// <summary>
/// Response from TinyPng Service
/// </summary>
public class TinyOutput
{
  /// <summary>
  /// Compressed size of image
  /// </summary>
  public int Size { get; set; }

  /// <summary>
  /// Image type
  /// </summary>
  public string Type { get; set; } = string.Empty;

  /// <summary>
  /// Image width
  /// </summary>
  public int Width { get; set; }

  /// <summary>
  /// Image height
  /// </summary>
  public int Height { get; set; }

  /// <summary>
  /// Compressing ratio
  /// </summary>
  public double Ratio { get; set; }

  /// <summary>
  /// Image url for downloading
  /// </summary>
  public string Url { get; set; } = string.Empty;

  /// <summary>
  /// If tinyPng returns error
  /// </summary>
  public string Error { get; set; } = string.Empty;

  /// <summary>
  /// If image was optimized
  /// </summary>
  public bool IsOptimized { get; set; }
}