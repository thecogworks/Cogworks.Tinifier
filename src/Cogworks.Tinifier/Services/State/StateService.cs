namespace Cogworks.Tinifier.Services.State;

public interface IStateService
{
  /// <summary>
  /// Get state from database to show for user
  /// </summary>
  /// <returns>TState</returns>
  TState GetState();

  /// <summary>
  /// Update state
  /// </summary>
  void UpdateState();

  /// <summary>
  /// Create state
  /// </summary>
  /// <param name="numberOfImages">Number of Images</param>
  void CreateState(int numberOfImages);

  /// <summary>
  /// Delete active state
  /// </summary>
  void Delete();
}

public class StateService : IStateService
{
  private readonly IStateRepository _stateRepository;

  public StateService(IStateRepository stateRepository)
  {
    _stateRepository = stateRepository;
  }

  public void CreateState(int numberOfImages)
  {
    var state = new TState
    {
      CurrentImage = 0,
      AmounthOfImages = numberOfImages,
      StatusType = Statuses.InProgress
    };

    _stateRepository.Create(state);
  }

  public TState GetState()
  {
    return _stateRepository.Get((int)Statuses.InProgress);
  }

  public void UpdateState()
  {
    var state = _stateRepository.Get((int)Statuses.InProgress);

    if (state != null)
    {
      if (state.CurrentImage < state.AmounthOfImages)
        state.CurrentImage++;

      if (state.CurrentImage == state.AmounthOfImages)
        state.StatusType = Statuses.Done;

      _stateRepository.Update(state);
    }
  }

  public void Delete()
  {
    _stateRepository.Delete();
  }
}