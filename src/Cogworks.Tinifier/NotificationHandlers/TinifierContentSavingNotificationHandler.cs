namespace Cogworks.Tinifier.NotificationHandlers;

public class TinifierContentSavingNotificationHandler : INotificationHandler<ContentSavingNotification>
{
  private readonly IImageCropperInfoService _imageCropperInfoService;
  private readonly TSetting _settings;

  public TinifierContentSavingNotificationHandler(IImageCropperInfoService imageCropperInfoService,
                                                  ISettingsService settingsService)
  {
    _imageCropperInfoService = imageCropperInfoService;
    _settings = settingsService.GetSettings();
  }

  public void Handle(ContentSavingNotification notification)
  {
    if (_settings is null)
      return;

    foreach (var entity in notification.SavedEntities)
    {
      var imageCroppers = entity.Properties.Where(x => x.PropertyType.PropertyEditorAlias == Constants.PropertyEditors.Aliases.ImageCropper);

      foreach (var crop in imageCroppers)
      {
        var key = string.Concat(entity.Name, "-", crop.Alias);
        var imageCropperInfo = _imageCropperInfoService.Get(key);
        var imagePath = crop.GetValue();

        //Wrong object
        if (imageCropperInfo is null && imagePath is null)
          continue;

        //Cropped file was Deleted
        if (imageCropperInfo is not null && imagePath is null)
        {
          _imageCropperInfoService.DeleteImageFromImageCropper(key, imageCropperInfo);
          continue;
        }

        var json = JObject.Parse(imagePath?.ToString() ?? string.Empty);
        var path = json.GetValue("src")?.ToString() ?? string.Empty;

        //republish existed content
        if (imageCropperInfo is not null && imageCropperInfo.ImageId == path)
          continue;

        //Cropped file was created or updated
        _imageCropperInfoService.GetCropImagesAndTinify(key, imageCropperInfo, imagePath, _settings.EnableOptimizationOnUpload, path);
      }
    }
  }
}