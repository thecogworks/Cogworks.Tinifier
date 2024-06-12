namespace Cogworks.Tinifier.Controllers;

public class TinifierSettingsController : UmbracoAuthorizedApiController
{
  private readonly ISettingsService _settingsService;

  public TinifierSettingsController(ISettingsService settingsService)
  {
    _settingsService = settingsService;
  }

  /// <summary>
  /// Get user settings
  /// </summary>
  /// <returns>Response(StatusCode, settings)</returns>
  [HttpGet]
  public IActionResult GetTSetting()
  {
    var tsetting = _settingsService.GetSettings();
    return Ok(tsetting);
  }

  /// <summary>
  /// Post user settings
  /// </summary>
  /// <param name="setting">TSetting</param>
  /// <returns>Response(StatusCode, message)</returns>
  [HttpPost]
  public IActionResult PostTSetting(TSetting setting)
  {
    if (ModelState.IsValid)
    {
      _settingsService.CreateSettings(setting);
      return StatusCode((int)HttpStatusCode.Created, new TNotification("Created", PackageConstants.ApiKeyMessage, EventMessageType.Success));
    }

    return BadRequest(new TNotification("Ooops", PackageConstants.ApiKeyError, EventMessageType.Error));
  }
}