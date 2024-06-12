namespace Cogworks.Tinifier.NotificationHandlers;

public class TinifierMediaSavingNotificationHandler : INotificationAsyncHandler<MediaSavingNotification>
{
  private readonly IStatisticService _statisticService;
  private readonly IHistoryService _historyService;

  public TinifierMediaSavingNotificationHandler(IStatisticService statisticService,
                                                IHistoryService historyService)
  {
    _statisticService = statisticService;
    _historyService = historyService;
  }

  public async Task HandleAsync(MediaSavingNotification notification, CancellationToken cancellationToken)
  {
    MediaSavingHelper.IsSavingInProgress = true;

    var mediaItems = notification.SavedEntities.Where(entity => PackageConstants.ImageAlias.InvariantEquals(entity.ContentType.Alias));
    if (mediaItems.Any())
    {
      _statisticService.UpdateStatistic();
    }
    foreach (var media in mediaItems)
    {
      _historyService.Delete(media.Id.ToString());
    }
  }
}