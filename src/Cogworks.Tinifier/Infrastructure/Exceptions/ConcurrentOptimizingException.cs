namespace Cogworks.Tinifier.Infrastructure.Exceptions;

public class ConcurrentOptimizingException : Exception
{
  public ConcurrentOptimizingException(string message) : base(message)
  {
  }
}