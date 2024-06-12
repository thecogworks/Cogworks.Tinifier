namespace Cogworks.Tinifier.Controllers;

public class TinifierImagesStatisticController : UmbracoAuthorizedApiController
{
  private readonly ISettingsService _settingsService;
  private readonly IStatisticService _statisticService;
  private readonly IHistoryService _historyService;

  public TinifierImagesStatisticController(ISettingsService settingsService, IStatisticService statisticService,
      IHistoryService historyService)
  {
    _settingsService = settingsService;
    _statisticService = statisticService;
    _historyService = historyService;
  }

  /// <summary>
  /// Get Images Statistic
  /// </summary>
  /// <returns>Response(StatusCode, {statistic, tsettings, history, requestLimit})</returns>
  [HttpGet]
  public IActionResult GetStatistic()
  {
    var statistic = _statisticService.GetStatistic();
    var tsetting = _settingsService.GetSettings();
    var history = _historyService.GetStatisticByDays();
    return Ok( new { statistic, tsetting, history, PackageConstants.MonthlyRequestsLimit });
  }
}