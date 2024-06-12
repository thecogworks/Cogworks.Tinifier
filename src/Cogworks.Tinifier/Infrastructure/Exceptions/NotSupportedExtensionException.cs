namespace Cogworks.Tinifier.Infrastructure.Exceptions;

public class NotSupportedExtensionException : Exception
{
  private const string _message = "Extension \"{0}\" is not supported. We support: {1}";

  public NotSupportedExtensionException(string extension = "unknown") : base(string.Format(_message,
                                                                                            extension,
                                                                                            string.Join(", ", PackageConstants.SupportedExtensions)))
  {
  }
}