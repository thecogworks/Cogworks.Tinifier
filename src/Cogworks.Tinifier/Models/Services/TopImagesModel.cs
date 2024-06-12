namespace Cogworks.Tinifier.Models.Services;

public class TopImagesModel
{
  public string ImageId { get; set; } = string.Empty;

  public long OriginSize { get; set; }

  public long OptimizedSize { get; set; }

  public DateTime OccuredAt { get; set; }
}