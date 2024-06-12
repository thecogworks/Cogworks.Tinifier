namespace Cogworks.Tinifier.Controllers;

public class TinifierStateController : UmbracoAuthorizedApiController
{
  private readonly IStateService _stateService;

  public TinifierStateController(IStateService stateService)
  {
    _stateService = stateService;
  }

  /// <summary>
  /// Get current tinifing state
  /// </summary>
  /// <returns>Response(StatusCode, state)</returns>
  [HttpGet]
  public IActionResult GetCurrentTinifingState()
  {
    var state = _stateService.GetState();
    return Ok(state);
  }

  [HttpDelete]
  public IActionResult DeleteActiveState()
  {
    _stateService.Delete();
    return Ok();
  }
}