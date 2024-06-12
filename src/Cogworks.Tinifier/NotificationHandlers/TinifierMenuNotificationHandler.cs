namespace Cogworks.Tinifier.NotificationHandlers;

public class TinifierMenuNotificationHandler : INotificationHandler<MenuRenderingNotification>
{
  private readonly IUmbracoContextFactory _umbracoContextFactory;

  public TinifierMenuNotificationHandler(IUmbracoContextFactory umbracoContextFactory)
  {
    _umbracoContextFactory = umbracoContextFactory;
  }

  public void Handle(MenuRenderingNotification notification)
  {
    using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
    var contentItem = umbracoContextReference.UmbracoContext!.Media!.GetById(int.Parse(notification.NodeId));

    if (!string.Equals(notification.TreeAlias, PackageConstants.MediaAlias, StringComparison.OrdinalIgnoreCase) ||
        contentItem is null) return;

    var menuItemTinifyButton = new MenuItem(PackageConstants.TinifierButton, PackageConstants.TinifierButtonCaption)
    {
      Icon = PackageConstants.MenuIcon,
      OpensDialog = true,
      UseLegacyIcon = false
    };

    menuItemTinifyButton.AdditionalData.Add("tinifyImage",
      $"/umbraco/backoffice/api/Tinifier/TinyTImage/TinyTImage?mediaId={contentItem.Id}");
    menuItemTinifyButton.LaunchDialogView(PackageConstants.TinyTImageRoute,
      $"{PackageConstants.SectionName} {contentItem.Id}");
    notification.Menu.Items.Add(menuItemTinifyButton);

    if (contentItem.ContentType.Alias == PackageConstants.FolderContentType) return;
    var menuItemUndoTinifyButton =
      new MenuItem(PackageConstants.UndoTinifierButton, PackageConstants.UndoTinifierButtonCaption)
      {
        Icon = PackageConstants.UndoTinifyIcon,
        OpensDialog = true,
        UseLegacyIcon = false
      };
    menuItemUndoTinifyButton.AdditionalData.Add("undoTinifyImage",
      $"/umbraco/backoffice/api/Tinifier/UndoTinify?mediaId={contentItem.Id}");
    menuItemUndoTinifyButton.LaunchDialogView(PackageConstants.UndoTinyTImageRoute,
      PackageConstants.UndoTinifierButtonCaption);
    notification.Menu.Items.Add(menuItemUndoTinifyButton);

    var menuItemSettingsButton = new MenuItem(PackageConstants.StatsButton, PackageConstants.StatsButtonCaption)
    {
      Icon = PackageConstants.MenuSettingsIcon,
      OpensDialog = true,
      UseLegacyIcon = false
    };
    menuItemSettingsButton.AdditionalData.Add("imageStats",
      $"/umbraco/backoffice/api/Tinifier/GetTImage?timageId={contentItem.Id}");
    menuItemSettingsButton.LaunchDialogView(PackageConstants.TinySettingsRoute, PackageConstants.StatsDialogCaption);
    notification.Menu.Items.Add(menuItemSettingsButton);
  }
}