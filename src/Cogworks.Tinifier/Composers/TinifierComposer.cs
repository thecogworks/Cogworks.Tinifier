namespace Cogworks.Tinifier.Composers;

public class TinifierComposer : IComposer
{
  public void Compose(IUmbracoBuilder builder)
  {
    var cogTinifierSettings = builder.Config
      .GetSection("CogTinifierSettings")
      .Get<TSetting>();

    builder.Services.AddTransient(_ => cogTinifierSettings);
    builder.Services.AddTransient<ITinyPNGConnector, TinyPNGConnectorService>();
    builder.Services.AddTransient<IValidationService, ValidationService>();
    builder.Services.AddTransient<ITinyImageService, TinyImageService>();
    builder.Services.AddTransient<IStatisticService, StatisticService>();
    builder.Services.AddTransient<ISettingsService, SettingsService>();
    builder.Services.AddTransient<IStateService, StateService>();
    builder.Services.AddTransient<IImageHistoryService, ImageHistoryService>();
    builder.Services.AddTransient<IHistoryService, HistoryService>();
    builder.Services.AddTransient<IImageService, ImageService>();
    builder.Services.AddTransient<IImageCropperInfoService, ImageCropperInfoService>();

    builder.Services.AddTransient<IStateRepository, StateRepository>();
    builder.Services.AddTransient<IImageRepository, ImageRepository>();
    builder.Services.AddTransient<IHistoryRepository, HistoryRepository>();
    builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
    builder.Services.AddTransient<IFileSystemProviderRepository, FileSystemProviderRepository>();
    builder.Services.AddTransient<IImageHistoryRepository, ImageHistoryRepository>();
    builder.Services.AddTransient<IStatisticRepository, StatisticRepository>();
    builder.Services.AddTransient<IMediaHistoryRepository, MediaHistoryRepository>();
    builder.Services.AddTransient<IUmbracoDbRepository, UmbracoDbRepository>();
    builder.Services.AddTransient<IImageCropperInfoRepository, ImageCropperInfoRepository>();

    builder.AddNotificationHandler<MenuRenderingNotification, TinifierMenuNotificationHandler>();
    builder.AddNotificationHandler<ContentSavingNotification, TinifierContentSavingNotificationHandler>();
    builder.AddNotificationAsyncHandler<MediaDeletingNotification, TinifierMediaDeletedNotificationHandler>();
    builder.AddNotificationAsyncHandler<MediaSavedNotification, TinifierMediaSavedNotificationHandler>();
    builder.AddNotificationAsyncHandler<MediaSavingNotification, TinifierMediaSavingNotificationHandler>();
  }
}