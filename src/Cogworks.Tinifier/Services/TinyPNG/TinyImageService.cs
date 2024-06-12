namespace Cogworks.Tinifier.Services.TinyPNG;

public interface ITinyImageService
{
  /// <summary>
  /// Download image from url
  /// </summary>
  /// <param name="url">Image url</param>
  /// <returns>byte[]</returns>
  byte[] DownloadImage(string url);
}

public sealed class TinyImageService : ITinyImageService
{
  public byte[] DownloadImage(string url)
  {
    byte[] tinyImageBytes;

    using (var webClient = new HttpClient())
    {
      tinyImageBytes = webClient.GetByteArrayAsync(url).Result;
    }
    return tinyImageBytes;
  }
}