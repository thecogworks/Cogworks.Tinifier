namespace Cogworks.Tinifier.Infrastructure;

public class SolutionExtensions
{
  public static string Base64Decode(string base64EncodedData)
  {
    var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
    return Encoding.UTF8.GetString(base64EncodedBytes);
  }

  public static string Base64Encode(string plainText)
  {
    if (int.TryParse(plainText, out var number))
      return plainText;

    var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
    return Convert.ToBase64String(plainTextBytes);
  }

  public static byte[] ReadFully(Stream input)
  {
    byte[] buffer = new byte[16 * 1024];
    using (MemoryStream ms = new MemoryStream())
    {
      int read;
      while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
      {
        ms.Write(buffer, 0, read);
      }
      return ms.ToArray();
    }
  }
}