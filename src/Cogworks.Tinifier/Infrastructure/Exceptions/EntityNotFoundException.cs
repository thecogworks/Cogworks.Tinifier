namespace Cogworks.Tinifier.Infrastructure.Exceptions;

public class EntityNotFoundException : Exception
{
  public EntityNotFoundException() : base(PackageConstants.ImageNotExists)
  {
  }

  public EntityNotFoundException(string message) : base(message)
  {
  }
}