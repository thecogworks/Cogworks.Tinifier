using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

namespace Cogworks.Tinifier.Repository.Image;

public interface IImageRepository
{
  IEnumerable<Media> GetAll();

  IEnumerable<Media> GetAllAt(int id);

  void Move(IMedia media, int parentId);

  Media Get(int id);

  Task Update(int imageId, int actualSize);

  IEnumerable<Media> GetOptimizedItems();

  IEnumerable<TImage> GetTopOptimizedImages();

  IEnumerable<Media> GetItemsFromFolder(int folderId);

  int AmountOfItems();

  int AmountOfOptimizedItems();

  Media Get(string path);
}

public class ImageRepository : IImageRepository
{
  private readonly IScopeProvider _scopeProvider;
  private readonly IFileSystemProviderRepository _fileSystemProviderRepository;
  private readonly IMediaService _mediaService;
  private readonly IBlobStorage _blobStorage;
  private readonly IWebHostEnvironment _hostingEnvironment;

  public ImageRepository(IScopeProvider scopeProvider,
                          IMediaService mediaService,
                          IFileSystemProviderRepository fileSystemProviderRepository,
                          IWebHostEnvironment hostingEnvironment)
  {
    _scopeProvider = scopeProvider;
    _mediaService = mediaService;
    _fileSystemProviderRepository = fileSystemProviderRepository;
    _blobStorage = new AzureBlobStorageService(hostingEnvironment);
    _hostingEnvironment = hostingEnvironment;
  }

  /// <summary>
  /// Get all media
  /// </summary>
  /// <returns>IEnumerable of Media</returns>
  public IEnumerable<Media> GetAll()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT NodeId FROM UmbracoContentVersion");
    var nodeIds = scope.Database.Fetch<int>(query);

    var mediaItems = _mediaService.GetByIds(nodeIds)
      .Where(media => media.ContentType.Alias.Equals(PackageConstants.ImageAlias))?
      .Select(m => m as Media) ?? Enumerable.Empty<Media>();

    return mediaItems;
  }

  /// <summary>
  /// Gets a collection of IMedia objects by ParentId
  /// </summary>
  /// <returns>IEnumerable of Media</returns>
  public IEnumerable<Media> GetAllAt(int id)
  {
    var allImages = GetAll();
    var ourMedia = allImages.Where(media => media.ParentId == id).ToList();
    return ourMedia;
  }

  /// <summary>
  /// Get Media by Id
  /// </summary>
  /// <param name="id">Media Id</param>
  /// <returns>Media</returns>
  public Media Get(int id)
  {
    return _mediaService.GetById(id) as Media;
  }

  /// <summary>
  /// Get Media By path
  /// </summary>
  /// <param name="path">relative path</param>
  /// <returns>Media</returns>
  public Media Get(string path)
  {
    return _mediaService.GetMediaByPath(path) as Media;
  }

  /// <summary>
  /// Moves an IMedia object to a new location
  /// </summary>
  /// <param name="media">media to move</param>
  /// <param name="parentId">id of a new location</param>
  public void Move(IMedia media, int parentId)
  {
    _mediaService.Move(media, parentId);
  }

  /// <summary>
  /// Update Media
  /// </summary>
  /// <param name="id">Media Id</param>
  public async Task Update(int id, int actualSize)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    using var _ = scope.Notifications.Suppress();
    var mediaItem = _mediaService.GetById(id);
    if (mediaItem is not null)
    {
      mediaItem.SetValue("umbracoBytes", actualSize);
      mediaItem.UpdateDate = DateTime.UtcNow;
      _mediaService.Save(mediaItem);
    }
  }

  /// <summary>
  /// Get Optimized Images
  /// </summary>
  /// <returns>IEnumerable of Media</returns>
  public IEnumerable<Media> GetOptimizedItems()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT ImageId FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
    var historyIds = scope.Database.Fetch<string>(query);

    var pardesIds = new List<int>();
    foreach (var historyId in historyIds)
    {
      if (int.TryParse(historyId, out var parsedId))
        pardesIds.Add(parsedId);
    }

    var mediaItems = GetAll().
      Where(item => pardesIds.Contains(item.Id));

    return mediaItems.Select(item => item as Media).ToList();
  }

  /// <summary>
  /// Get Media from folder
  /// </summary>
  /// <param name="folderId">Folder Id</param>
  /// <returns>IEnumerable of Media</returns>
  public IEnumerable<Media> GetItemsFromFolder(int folderId)
  {
    var _mediaList = new List<Media>();
    var allImages = GetAll();
    var items = allImages.Where(media => media.ParentId == folderId).ToList();

    if (items.Any())
    {
      foreach (var media in items)
      {
        if (media.ContentType.Alias == PackageConstants.ImageAlias)
        {
          _mediaList.Add(media as Media);
        }
      }
      foreach (var media in items)
      {
        if (media.ContentType.Alias == PackageConstants.FolderAlias)
        {
          GetItemsFromFolder(media.Id);
        }
      }
    }
    return _mediaList;
  }

  /// <summary>
  /// Get Count of Images
  /// </summary>
  /// <returns>Number of Images</returns>
  public int AmountOfItems()
  {
    var numberOfImages = 0;
    var fileSystem = _fileSystemProviderRepository.GetFileSystem();

    if (fileSystem != null)
    {
      if (fileSystem.Type.Contains("PhysicalFileSystem"))
      {
        numberOfImages = Directory
            .EnumerateFiles(_hostingEnvironment.MapPathWebRoot("/media/"), "*.*", SearchOption.AllDirectories)
            .Count(file => !file.ToLower().EndsWith("config"));
      }
      else
      {
        _blobStorage.SetDataForBlobStorage();

        if (_blobStorage.DoesContainerExist())
        {
          numberOfImages = _blobStorage.CountBlobsInContainer();
        }
      }
    }

    return numberOfImages;
  }

  /// <summary>
  /// Get Count of Optimized Images
  /// </summary>
  /// <returns>Number of optimized Images</returns>
  public int AmountOfOptimizedItems()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT ImageId FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
    var historyIds = scope.Database.Fetch<string>(query);
    return historyIds.Count;
  }

  public IEnumerable<TImage> GetTopOptimizedImages()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT ImageId, OriginSize, OptimizedSize, OccuredAt FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
    var optimizedImages = scope.Database.Fetch<TopImagesModel>(query);

    var historyIds = optimizedImages.OrderByDescending(x => (x.OriginSize - x.OptimizedSize)).Take(50)
      .OrderByDescending(x => x.OccuredAt).Select(y => y.ImageId);

    var pardesIds = new List<int>();
    var croppedIds = new List<string>();

    foreach (var historyId in historyIds)
    {
      if (int.TryParse(historyId, out var parsedId))
        pardesIds.Add(parsedId);
      else
        croppedIds.Add(historyId);
    }

    var mediaItems = GetAll()
      .Where(item => pardesIds.Contains(item.Id));

    var images = mediaItems.Select(media => new TImage { Id = media.Id.ToString(), Name = media.Name ?? string.Empty, AbsoluteUrl = GetAbsoluteUrl(media as Media) }).ToList();
    images.AddRange(croppedIds.Select(crop => new TImage { Id = crop, Name = Path.GetFileName(crop), AbsoluteUrl = crop }));

    return images;
  }

  protected static string GetAbsoluteUrl(Media uMedia)
  {
    return uMedia.Path;
  }
}