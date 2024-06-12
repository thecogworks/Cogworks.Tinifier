namespace Cogworks.Tinifier.NotificationHandlers;

public class TinifierMediaSavedNotificationHandler : INotificationAsyncHandler<MediaSavedNotification>
{
  private readonly ISettingsService _settingsService;
  private readonly IStatisticService _statisticService;
  private readonly IImageService _imageService;
  private readonly IHistoryService _historyService;

  public TinifierMediaSavedNotificationHandler(ISettingsService settingsService,
                                                IStatisticService statisticService,
                                                IImageService imageService,
                                                IHistoryService historyService)
  {
    _settingsService = settingsService;
    _statisticService = statisticService;
    _imageService = imageService;
    _historyService = historyService;
  }

  public async Task HandleAsync(MediaSavedNotification notification, CancellationToken cancellationToken)
  {
    // // optimize on upload
    var settingService = _settingsService.GetSettings();
    if (settingService == null || settingService.EnableOptimizationOnUpload == false)
      return;

    var mediaItems = notification.SavedEntities.Where(entity => PackageConstants.ImageAlias.InvariantEquals(entity.ContentType.Alias));
    if (mediaItems.Any())
    {
      _statisticService.UpdateStatistic();
    }
    foreach (var media in mediaItems)
    {
      await OptimizeOnUploadAsync(media.Id, notification);
    }
  }

  private async Task OptimizeOnUploadAsync(int mediaItemId, MediaSavedNotification notification)
  {
    TImage image;

    try
    {
      image = _imageService.GetImage(mediaItemId);
    }
    catch (NotSupportedExtensionException ex)
    {
      notification.Messages.Add(new EventMessage(PackageConstants.ErrorCategory, ex.Message, EventMessageType.Error));
      throw;
    }

    var imageHistory = _historyService.GetImageHistory(image.Id);

    if (imageHistory == null)
    {
      await _imageService.OptimizeImageAsync(image);
    }
  }
}