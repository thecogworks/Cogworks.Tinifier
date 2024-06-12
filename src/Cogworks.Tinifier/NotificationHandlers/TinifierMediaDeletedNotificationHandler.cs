namespace Cogworks.Tinifier.NotificationHandlers;

public class TinifierMediaDeletedNotificationHandler : INotificationAsyncHandler<MediaDeletingNotification>
{
  private readonly IStatisticService _statisticService;
  private readonly IHistoryService _historyService;

  public TinifierMediaDeletedNotificationHandler(IStatisticService statisticService,
                                                 IHistoryService historyService)
  {
    _statisticService = statisticService;
    _historyService = historyService;
  }

  public async Task HandleAsync(MediaDeletingNotification notification, CancellationToken cancellationToken)
  {
    foreach (var item in notification.DeletedEntities)
    {
      _historyService.Delete(item.Id.ToString());
    }

    _statisticService.UpdateStatistic(notification.DeletedEntities.Count());
  }
}