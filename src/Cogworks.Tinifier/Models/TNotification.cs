namespace Cogworks.Tinifier.Models;

public class TNotification
{
  public string Headline { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;
  public bool Sticky { get; set; } = false;
  public string Type { get; set; } = "info";

  public TNotification(string text)
  {
    Message = text;
  }

  public TNotification(string header, string text)
      : this(text)
  {
    Headline = header;
  }

  public TNotification(string header, string text, EventMessageType type)
      : this(header, text)
  {
    this.Type = Enum.GetName(typeof(EventMessageType), type).ToLowerInvariant();
  }
}