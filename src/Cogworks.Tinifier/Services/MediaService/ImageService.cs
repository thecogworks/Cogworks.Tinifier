namespace Cogworks.Tinifier.Services.MediaService;

public interface IImageService
{
  /// <summary>
  /// Get all images
  /// </summary>
  /// <returns>IEnumerable<TImage></returns>
  IEnumerable<TImage> GetAllImages();

  /// <summary>
  /// Get all images at the first/root level
  /// </summary>
  /// <returns>IEnumerable<TImage></returns>
  IEnumerable<Media> GetAllImagesAt(int folderId);

  /// <summary>
  /// Get Image By Id from Umbraco Media
  /// </summary>
  /// <param name="id">Image Id</param>
  /// <returns>TImage</returns>
  TImage GetImage(int id);

  /// <summary>
  /// Get image by path
  /// </summary>
  /// <param name="path">Image path</param>
  /// <returns>TImage</returns>
  TImage GetImage(string path);

  /// <summary>
  /// Moves an Media object to a new location
  /// </summary>
  /// <param name="media">media to move</param>
  /// <param name="parentId">id of a new location</param>
  void Move(Media image, int parentId);

  /// <summary>
  /// Get all optimized images
  /// </summary>
  /// <returns>IEnumerable of TImage</returns>
  IEnumerable<TImage> GetOptimizedImages();

  /// <summary>
  /// Get top 50 optimized images
  /// </summary>
  /// <returns></returns>
  IEnumerable<TImage> GetTopOptimizedImages();

  /// <summary>
  /// Get all images from specific folder
  /// </summary>
  /// <param name="folderId">Folder Id</param>
  /// <returns>IEnumerable of TImage</returns>
  IEnumerable<TImage> GetFolderImages(int folderId);

  /// <summary>
  /// Optimize image and update history
  /// </summary>
  /// <param name="image">TImage</param>
  Task OptimizeImageAsync(TImage image);

  /// <summary>
  ///  Update image and state if its image from folder
  /// </summary>
  /// <param name="tinyResponse">Response from TinyPNG</param>
  /// <param name="image">TImage</param>
  void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image, IFileSystem fs);

  void UpdateStatistics();

  TImage GetCropImage(string path);

  void UndoTinify(int mediaId);

  /// <summary>
  /// Backup media paths before they be moved
  /// </summary>
  /// <param name="media">Media to move</param>
  void BackupMediaPaths(IEnumerable<Media> media);
}

public class ImageService : IImageService
{
  private readonly IValidationService _validationService;
  private readonly IImageRepository _imageRepository;
  private readonly ITinyImageService _tinyImageService;
  private readonly ISettingsService _settingsService;
  private readonly IHistoryService _historyService;
  private readonly IStatisticService _statisticService;
  private readonly IStateService _stateService;
  private readonly IImageHistoryService _imageHistoryService;
  private readonly IMediaService _mediaService;
  private readonly ITinyPNGConnector _tinyPngConnectorService;
  private readonly IMediaHistoryRepository _mediaHistoryRepository;
  private readonly MediaFileManager _mediaFileManager;
  private readonly IUmbracoDbRepository _umbracoDbRepository;
  private readonly MediaUrlGeneratorCollection _mediaUrlGenerator;

  //private readonly IFileSystemRegistrationService _fileSystemRegistrationService;

  private readonly IHttpContextAccessor _httpContextAccessor;

  public ImageService(IImageRepository imageRepository,
                       IValidationService validationService,
                       ITinyImageService tinyImageService,
                       ISettingsService settingsService,
                       IHistoryService historyService,
                       IStatisticService statisticService,
                       IStateService stateService,
                       IImageHistoryService imageHistoryService,
                       IMediaService mediaService,
                       ITinyPNGConnector tinyPngConnectorService,
                       IMediaHistoryRepository mediaHistoryRepository,
                       IUmbracoDbRepository umbracoDbRepository,
                       MediaFileManager mediaFileManager,
                       MediaUrlGeneratorCollection mediaUrlGenerator,
                       IHttpContextAccessor httpContextAccessor)
  {
    _imageRepository = imageRepository;
    _validationService = validationService;
    _tinyImageService = tinyImageService;
    _settingsService = settingsService;
    _historyService = historyService;
    _statisticService = statisticService;
    _stateService = stateService;

    _mediaFileManager = mediaFileManager;

    _imageHistoryService = imageHistoryService;
    _mediaService = mediaService;
    _tinyPngConnectorService = tinyPngConnectorService;
    _mediaHistoryRepository = mediaHistoryRepository;
    _umbracoDbRepository = umbracoDbRepository;

    _httpContextAccessor = httpContextAccessor;
    _mediaUrlGenerator = mediaUrlGenerator;
  }

  public IEnumerable<TImage> Convert(IEnumerable<Media> items)
  {
    return items
        .Select(x => Convert(x))
        .Where(x => !string.IsNullOrEmpty(x.AbsoluteUrl)); //Skip images for which we were unable to fetch the Url
  }

  public TImage Convert(Media uMedia)
  {
    var image = new TImage()
    {
      Id = uMedia.Id.ToString(),
      Name = uMedia.Name,
      AbsoluteUrl = uMedia.GetUrl("umbracoFile", _mediaUrlGenerator)
    };

    if (string.IsNullOrEmpty(image.AbsoluteUrl))

    {
      image.AbsoluteUrl = _umbracoDbRepository.GetMediaAbsoluteUrl(uMedia.Id);
    }

    return image;
  }

  /// <summary>
  /// Update physical media file, method depens on FileSystemProvider
  /// </summary>
  /// <param name="image"></param>
  /// <param name="optimizedImageBytes"></param>
  protected void UpdateMedia(TImage image, byte[] optimizedImageBytes)
  {
    var path = _mediaFileManager.FileSystem.GetRelativePath(image.AbsoluteUrl);
    if (!_mediaFileManager.FileSystem.FileExists(path))
      throw new InvalidOperationException("Physical media file doesn't exist in " + path);
    using Stream stream = new MemoryStream(optimizedImageBytes);
    _mediaFileManager.FileSystem.AddFile(path, stream, true);
  }

  public IEnumerable<TImage> GetAllImages()
  {
    return Convert(_imageRepository.GetAll());
  }

  public IEnumerable<Media> GetAllImagesAt(int folderId)
  {
    return _imageRepository.GetAllAt(folderId);
  }

  public TImage GetCropImage(string path)
  {
    if (string.IsNullOrEmpty(path))
      throw new EntityNotFoundException();

    var fileExt = Path.GetExtension(path).ToUpper().Replace(".", string.Empty).Trim();
    if (!PackageConstants.SupportedExtensions.Contains(fileExt))
      throw new NotSupportedExtensionException(fileExt);

    var tImage = new TImage
    {
      Id = path,
      Name = Path.GetFileName(path),
      AbsoluteUrl = path
    };

    return tImage;
  }

  public IEnumerable<TImage> GetFolderImages(int folderId)
  {
    return Convert(_imageRepository.GetItemsFromFolder(folderId));
  }

  public TImage GetImage(int id)
  {
    return GetImage(_imageRepository.Get(id));
  }

  public TImage GetImage(string path)
  {
    return GetImage(_imageRepository.Get(path));
  }

  public IEnumerable<TImage> GetOptimizedImages()
  {
    return Convert(_imageRepository.GetOptimizedItems().OrderByDescending(x => x.UpdateDate));
  }

  public IEnumerable<TImage> GetTopOptimizedImages()
  {
    return _imageRepository.GetTopOptimizedImages();
  }

  public void Move(Media image, int parentId)
  {
    _imageRepository.Move(image, parentId);
  }

  public async Task OptimizeImageAsync(TImage image)
  {
    _stateService.CreateState(1);
    var tinyResponse = await _tinyPngConnectorService.TinifyAsync(image, _mediaFileManager.FileSystem);
    if (tinyResponse.Output.Url == null)
    {
      _historyService.CreateResponseHistory(image.Id, tinyResponse);
      return;
    }
    UpdateImageAfterSuccessfullRequest(tinyResponse, image, _mediaFileManager.FileSystem);
  }

  public void UndoTinify(int mediaId)
  {
    byte[] imageBytes;
    var originImage = _imageHistoryService.Get(mediaId);

    if (originImage != null)
    {
      var mediaFile = _mediaService.GetById(mediaId) as Media;

      if (System.IO.File.Exists(originImage.OriginFilePath))
      {
        using (var file = new FileStream(originImage.OriginFilePath, FileMode.Open))
          imageBytes = SolutionExtensions.ReadFully(file);

        if (Directory.Exists(Path.GetDirectoryName(originImage.OriginFilePath)))
          System.IO.File.Delete(originImage.OriginFilePath);

        var image = new TImage
        {
          Id = mediaId.ToString(),
          Name = mediaFile.Name,
          AbsoluteUrl = mediaFile.GetUrl("umbracoFile", _mediaUrlGenerator)
        };

        // update physical file
        UpdateMedia(image, imageBytes);
        // update umbraco media attributes
        _imageRepository.Update(mediaId, imageBytes.Length);
        _historyService.Delete(mediaId.ToString());
        // update statistic
        _statisticService.UpdateStatistic();
        //delete image history
        _imageHistoryService.Delete(mediaId);
      }
    }
    else
    {
      throw new UndoTinifierException("Image not optimized or Undo tinify not enabled");
    }
  }

  public void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image, IFileSystem fs)
  {
    int.TryParse(image.Id, out var id);

    // download optimized image
    var tImageBytes = _tinyImageService.DownloadImage(tinyResponse.Output.Url);

    // preserve image metadata
    if (_settingsService.GetSettings().PreserveMetadata)
    {
      var originImageBytes = image.ToBytes(fs);
      PreserveImageMetadata(originImageBytes, ref tImageBytes);
    }

    // httpContext is null when optimization on upload
    // https://our.umbraco.org/projects/backoffice-extensions/tinifier/bugs/90472-error-systemargumentnullexception-value-cannot-be-null
    if (_httpContextAccessor.HttpContext == null)
    {
      var context = new DefaultHttpContext();
      context.Request.Path = "dummy.aspx";
      context.Response.Body = new MemoryStream();
      _httpContextAccessor.HttpContext = context;
    }

    // update physical file
    UpdateMedia(image, tImageBytes);
    // update history
    _historyService.CreateResponseHistory(image.Id, tinyResponse);

    // update umbraco media attributes, is not working the update of properties of the image
    _imageRepository.Update(id, tinyResponse.Output.Size);

    // update statistic
    //Try to limit the amount of times the statistics are gathered
    //_statisticService.UpdateStatistic();
    // update tinifying state
    _stateService.UpdateState();
  }

  public void UpdateStatistics()
  {
    _statisticService.UpdateStatistic();
  }

  private TImage GetImage(Media uMedia)
  {
    _validationService.ValidateExtension(uMedia);
    return Convert(uMedia);
  }

  protected void PreserveImageMetadata(byte[] originImage, ref byte[] optimizedImage)
  {
    #pragma warning disable CA1416
    var originImg = (Image)new ImageConverter().ConvertFrom(originImage)!;
    var optimisedImg = (Image)new ImageConverter().ConvertFrom(optimizedImage)!;

    if (!optimisedImg.HasValue() || !originImg.HasValue())
    {
      return;
    }

    var srcPropertyItems = originImg.PropertyItems;

    foreach (var item in srcPropertyItems)
    {
      optimisedImg.SetPropertyItem(item);
    }

    using var ms = new MemoryStream();
    optimisedImg.Save(ms, optimisedImg.RawFormat);
    optimizedImage = ms.ToArray();
    #pragma warning restore CA1416
  }

  public void BackupMediaPaths(IEnumerable<Media> media)
  {
    foreach (var m in media)
    {
      var mediaHistory = new TinifierMediaHistory
      {
        MediaId = m.Id,
        FormerPath = m.Path,
        OrganizationRootFolderId = m.ParentId
      };
      _mediaHistoryRepository.Create(mediaHistory);
    }
  }
}