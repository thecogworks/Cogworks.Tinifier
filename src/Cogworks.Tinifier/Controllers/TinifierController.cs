namespace Cogworks.Tinifier.Controllers;

public class TinifierController : UmbracoAuthorizedApiController
{
  private readonly IImageService _imageService;
  private readonly ITinyPNGConnector _tinyPngConnectorService;
  private readonly IHistoryService _historyService;
  private readonly ISettingsService _settingsService;
  private readonly IStateService _stateService;
  private readonly IValidationService _validationService;
  private readonly IImageCropperInfoService _imageCropperInfoService;
  private readonly MediaFileManager _mediaFileManager;
  private readonly IPublishedContentQuery _contentQuery;

  public TinifierController(IHistoryService historyService,
    ISettingsService settingsService,
    IValidationService validationService,
    IStateService stateService,
    ITinyPNGConnector tinyPngConnectorService,
    IImageService imageService,
    IPublishedContentQuery contentQuery,
    IImageCropperInfoService imageCropperInfoService,
    MediaFileManager mediaFileManager)
  {
    _historyService = historyService;
    _imageService = imageService;
    _settingsService = settingsService;
    _validationService = validationService;
    _stateService = stateService;
    _tinyPngConnectorService = tinyPngConnectorService;
    _contentQuery = contentQuery;
    _mediaFileManager = mediaFileManager;
    _imageCropperInfoService = imageCropperInfoService;
  }

  /// <summary>
  /// Get Image by id
  /// </summary>
  /// <param name="tImageId">Image Id</param>
  /// <returns>Response(StatusCode, {image, history}}</returns>
  [HttpGet]
  public IActionResult GetTImage(string tImageId)
  {
    TImage tImage;

    try
    {
      tImage = int.TryParse(tImageId, out var imageId)
        ? _imageService.GetImage(imageId)
        : _imageService.GetCropImage(SolutionExtensions.Base64Decode(tImageId));
    }
    catch (Exception ex)
    {
      return BadRequest(new TNotification("Tinifier Oops", ex.Message, EventMessageType.Error)
      {
        Sticky = true,
      });
    }

    var history = _historyService.GetImageHistory(tImage.Id);

    return Ok(new { tImage, history });
  }

  [HttpGet]
  public IActionResult Test()
  {
    return Ok("Success");
  }

  /// <summary>
  /// Tinify Image(s)
  /// </summary>
  /// <param name="imageRelativeUrls">Array of media items urls</param>
  /// <param name="mediaId">Media item id</param>
  /// <returns>Response(StatusCode, message)</returns>
  [HttpGet]
  public async Task<IActionResult> TinyTImages(string imageRelativeUrls)
  {
    _settingsService.CheckIfSettingExists();

    var listUrl = imageRelativeUrls.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

    if (!listUrl.HasAny())
    {
      return Ok("No Images Selected.");
    }

    return await TinifyImages(listUrl);
  }

  [HttpGet]
  public async Task<IActionResult> TinyTImage(int mediaId)
  {
    _settingsService.CheckIfSettingExists();

    IActionResult responseMessage;

    if (_validationService.IsFolder(mediaId))
    {
      responseMessage = await TinifyFolder(mediaId);
    }
    else
    {
      responseMessage = await TinifyImage(mediaId);
    }

    return responseMessage;
  }

  /// <summary>
  /// Tinify full Media folder
  /// </summary>
  /// <returns>Response(StatusCode, message)</returns>
  [HttpPut]
  public async Task<IActionResult> TinifyEverything()
  {
    var nonOptimizedImages = new List<TImage>();

    foreach (var image in _imageService.GetAllImages())
    {
      var imageHistory = _historyService.GetImageHistory(image.Id);

      if (imageHistory.HasValue() && imageHistory.IsOptimized)
        continue;

      nonOptimizedImages.Add(image);
    }

    GetAllPublishedContentAndGetImageCroppers(nonOptimizedImages);

    if (!nonOptimizedImages.HasAny())
    {
      return GetImageOptimizedReponse(true);
    }

    _stateService.CreateState(nonOptimizedImages.Count);

    return await CallTinyPngService(nonOptimizedImages);
  }

  /// <summary>
  /// Undo tinify
  /// </summary>
  /// <param name="mediaId"></param>
  /// <returns></returns>
  [HttpGet]
  public IActionResult UndoTinify([FromQuery] int mediaId)
  {
    try
    {
      _imageService.UndoTinify(mediaId);
    }
    catch (UndoTinifierException ex)
    {
      return BadRequest(new TNotification("Undo finished", ex.Message, EventMessageType.Info));
    }

    return Ok(new TNotification("Success", PackageConstants.UndoTinifyingFinished, EventMessageType.Success));
  }

  /// <summary>
  /// Tinify folder By Id
  /// </summary>
  /// <param name="folderId">Folder Id</param>
  /// <returns>Response(StatusCode, message)</returns>
  private async Task<IActionResult> TinifyFolder(int folderId)
  {
    var images = _imageService.GetFolderImages(folderId);
    var imagesList = _historyService.GetImagesWithoutHistory(images);

    if (!imagesList.HasAny())
    {
      return GetImageOptimizedReponse(true);
    }

    _stateService.CreateState(imagesList.Count);

    return await CallTinyPngService(imagesList);
  }

  /// <summary>
  /// Tinify Images by urls
  /// </summary>
  /// <param name="imagesRelativeUrls">Array of images urls</param>
  /// <returns>Response(StatusCode, message)</returns>
  private async Task<IActionResult> TinifyImages(IEnumerable<string> imagesRelativeUrls)
  {
    var nonOptimizedImages = new List<TImage>();

    foreach (var imageRelativeUrl in imagesRelativeUrls)
    {
      var image = _imageService.GetImage(imageRelativeUrl);
      var imageHistory = _historyService.GetImageHistory(image.Id);

      if (imageHistory.HasValue() && imageHistory.IsOptimized)
      {
        continue;
      }

      nonOptimizedImages.Add(image);
    }

    if (nonOptimizedImages.Count == 0)
    {
      return GetImageOptimizedReponse(true);
    }

    _stateService.CreateState(nonOptimizedImages.Count);

    return await CallTinyPngService(nonOptimizedImages);
  }

  /// <summary>
  /// Tinify image by Id
  /// </summary>
  /// <param name="imageId">Image Id</param>
  /// <returns>Response(StatusCode, message)</returns>
  [HttpGet]
  public async Task<IActionResult> TinifyImage(int imageId)
  {
    var imageById = _imageService.GetImage(imageId);
    var notOptimizedImage = _historyService.GetImageHistory(imageById.Id);

    if (notOptimizedImage.HasValue() && notOptimizedImage.IsOptimized)
    {
      return GetImageOptimizedReponse();
    }

    var nonOptimizedImages = new List<TImage> { imageById };

    _stateService.CreateState(nonOptimizedImages.Count);

    return await CallTinyPngService(nonOptimizedImages);
  }

  /// <summary>
  /// Create request to TinyPNG service and get response
  /// </summary>
  /// <param name="imagesList">Images that needs to be tinifing</param>
  /// <returns>Response(StatusCode, message)</returns>
  private async Task<IActionResult> CallTinyPngService(IEnumerable<TImage> imagesList)
  {
    var nonOptimizedImagesCount = 0;

    foreach (var tImage in imagesList)
    {
      var tinyResponse = await _tinyPngConnectorService.TinifyAsync(tImage, _mediaFileManager.FileSystem);

      if (tinyResponse.Output.Url == null)
      {
        _historyService.CreateResponseHistory(tImage.Id, tinyResponse);
        _stateService.UpdateState();

        nonOptimizedImagesCount++;
        continue;
      }

      _imageService.UpdateImageAfterSuccessfullRequest(tinyResponse, tImage, _mediaFileManager.FileSystem);
    }

    var n = imagesList.Count();
    var k = n - nonOptimizedImagesCount;

    if (n > 0)
    {
      _imageService.UpdateStatistics();
    }

    return GetSuccessResponse(k, n,
      nonOptimizedImagesCount == 0
        ? EventMessageType.Success
        : EventMessageType.Warning);
  }

  private IActionResult GetImageOptimizedReponse(bool isMultipleImages = false)
  {
    return Ok(new TNotification(PackageConstants.TinifyingFinished,
      isMultipleImages
        ? PackageConstants.AllImagesAlreadyOptimized
        : PackageConstants.AlreadyOptimized,
      EventMessageType.Info));
  }

  private IActionResult GetSuccessResponse(int optimized, int total, EventMessageType type)
  {
    return Ok(new TNotification(PackageConstants.TinifyingFinished,
      $"{optimized}/{total} images were optimized. Enjoy the package? Click the <a href=\"{PackageConstants.UmbracoMarketplaceUrl}\" target=\"_blank\">message</a> and rate us!", type)
            {
              Url = PackageConstants.UmbracoMarketplaceUrl
            });
  }

  private IEnumerable<IPublishedContent> GetAllPublishedContent()
  {
    // Get all published content
    var allPublishedContent = new List<IPublishedContent>();

    foreach (var publishedContentRoot in _contentQuery.ContentAtRoot())
    {
      allPublishedContent.AddRange(publishedContentRoot.DescendantsOrSelf());
    }

    return allPublishedContent;
  }

  private void TinifyImageCroppers(string path, List<TImage> nonOptimizedImages, TImageCropperInfo imageCropperInfo,
    string key)
  {
    var pathForFolder = path.Remove(path.LastIndexOf('/') + 1);
    _imageCropperInfoService.GetFilesAndTinify(pathForFolder, nonOptimizedImages, true);

    if (imageCropperInfo.HasValue())
    {
      _imageCropperInfoService.Update(key, path);
    }
    else
    {
      _imageCropperInfoService.Create(key, path);
    }
  }

  private void GetAllPublishedContentAndGetImageCroppers(List<TImage> nonOptimizedImages)
  {
    foreach (var content in GetAllPublishedContent())
    {
      var imageCroppers = content.Properties
        .Where(x => x.Alias.ToString().HasValue() && x.Alias.ToString().Contains("crops"));

      foreach (var crop in imageCroppers)
      {
        var key = string.Concat(content.Name, "-", crop.Alias);
        var imageCropperInfo = _imageCropperInfoService.Get(key);
        var imagePath = crop.Alias;

        //Wrong object
        if (!imageCropperInfo.HasValue() && !imagePath.HasValue())
        {
          continue;
        }

        //Cropped file was Deleted
        if (imageCropperInfo.HasValue() && !imagePath.HasValue())
        {
          _imageCropperInfoService.DeleteImageFromImageCropper(key, imageCropperInfo);
          continue;
        }

        //Cropped file was created or updated
        var json = JObject.Parse(imagePath);
        var path = json.GetValue("src")?.ToString() ?? string.Empty;
        try
        {
          _imageCropperInfoService.ValidateFileExtension(path);
        }
        catch (Exception)
        {
          continue;
        }

        TinifyImageCroppers(path, nonOptimizedImages, imageCropperInfo, key);
      }
    }
  }
}