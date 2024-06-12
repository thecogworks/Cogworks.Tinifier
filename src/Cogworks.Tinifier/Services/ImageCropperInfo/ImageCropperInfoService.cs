namespace Cogworks.Tinifier.Services.ImageCropperInfo;

public interface IImageCropperInfoService
{
  TImageCropperInfo Get(string key);

  void Create(string key, string imageId);

  void Delete(string key);

  void Update(string key, string imageId);

  void ValidateFileExtension(string path);

  void DeleteImageFromImageCropper(string key, TImageCropperInfo imageCropperInfo);

  void GetFilesAndTinify(string pathForFolder, List<TImage> nonOptimizedImages = null, bool tinifyEverything = false);

  void GetCropImagesAndTinify(string key, TImageCropperInfo imageCropperInfo, object imagePath,
    bool enableCropsOptimization, string path);
}

public class ImageCropperInfoService : IImageCropperInfoService
{
  private readonly IImageCropperInfoRepository _imageCropperInfoRepository;
  private readonly IHistoryService _historyService;
  private readonly IStatisticService _statisticService;
  private readonly IImageService _imageService;
  private readonly IFileSystemProviderRepository _fileSystemProviderRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public ImageCropperInfoService(IWebHostEnvironment webHostEnvironment,
    IImageCropperInfoRepository imageCropperInfoRepository,
    IHistoryService historyService,
    IStatisticService statisticService,
    IImageService imageService,
    IFileSystemProviderRepository fileSystemProviderRepository)
  {
    _webHostEnvironment = webHostEnvironment;
    _imageCropperInfoRepository = imageCropperInfoRepository;
    _historyService = historyService;
    _statisticService = statisticService;
    _imageService = imageService;
    _fileSystemProviderRepository = fileSystemProviderRepository;
  }

  public void Create(string key, string imageId)
  {
    _imageCropperInfoRepository.Create(key, imageId);
  }

  public void Delete(string key)
  {
    _imageCropperInfoRepository.Delete(key);
  }

  public TImageCropperInfo Get(string key)
  {
    return _imageCropperInfoRepository.Get(key);
  }

  public void Update(string key, string imageId)
  {
    _imageCropperInfoRepository.Update(key, imageId);
  }

  public void GetFilesAndTinify(string pathForFolder, List<TImage> nonOptimizedImages = null,
    bool tinifyEverything = false)
  {
    var fileSystem = _fileSystemProviderRepository.GetFileSystem();

    if (!fileSystem.HasValue())
    {
      return;
    }

    var serverPathForFolder = _webHostEnvironment.MapPathWebRoot(pathForFolder);
    var directory = new DirectoryInfo(serverPathForFolder);
    var files = directory.GetFiles();

    IterateFilesAndTinifyFromMediaFolder(pathForFolder, files, nonOptimizedImages, tinifyEverything);
  }

  public void ValidateFileExtension(string path)
  {
    if (string.IsNullOrEmpty(path))
      throw new EntityNotFoundException();

    var fileExt = Path.GetExtension(path).ToUpper().Replace(".", string.Empty).Trim();
    if (!PackageConstants.SupportedExtensions.Contains(fileExt))
      throw new NotSupportedExtensionException(fileExt);
  }

  public void DeleteImageFromImageCropper(string key, TImageCropperInfo imageCropperInfo)
  {
    Delete(key);

    var pathForFolder = imageCropperInfo.ImageId.Remove(imageCropperInfo.ImageId.LastIndexOf('/') + 1);
    var histories = _historyService.GetHistoryByPath(pathForFolder);

    foreach (var history in histories)
      _historyService.Delete(history.ImageId);

    _fileSystemProviderRepository.GetFileSystem();

    _statisticService.UpdateStatistic();
  }

  public void GetCropImagesAndTinify(string key, TImageCropperInfo imageCropperInfo, object imagePath,
    bool enableCropsOptimization, string path)
  {
    var pathForFolder = path.Remove(path.LastIndexOf('/') + 1);
    DeleteAzureHistory(imageCropperInfo);

    var histories = _historyService.GetHistoryByPath(pathForFolder);
    foreach (var history in histories)
      _historyService.Delete(history.ImageId);

    if (enableCropsOptimization)
    {
      ValidateFileExtension(path);
      GetFilesAndTinify(pathForFolder);

      //Cropped file was Updated
      if (imageCropperInfo != null && imagePath != null)
        Update(key, path);

      //Cropped file was Created
      if (imageCropperInfo == null && imagePath != null)
        Create(key, path);
    }

    _statisticService.UpdateStatistic();
  }

  private void DeleteAzureHistory(TImageCropperInfo imageCropperInfo)
  {
    var fileSystem = _fileSystemProviderRepository.GetFileSystem();
    if (fileSystem != null && imageCropperInfo != null)
    {
      if (!fileSystem.Type.Contains("PhysicalFileSystem"))
      {
        var azurePath = imageCropperInfo.ImageId.Remove(imageCropperInfo.ImageId.LastIndexOf('/') + 1);

        var histories = _historyService.GetHistoryByPath(azurePath);
        foreach (var history in histories)
          _historyService.Delete(history.ImageId);
      }
    }
  }

  private void IterateFilesAndTinifyFromMediaFolder(string pathForFolder, IEnumerable<FileInfo> files,
    ICollection<TImage> nonOptimizedImages, bool tinifyEverything)
  {
    foreach (var file in files)
    {
      var image = new TImage
      {
        Id = Path.Combine(pathForFolder, file.Name),
        Name = file.Name,
        AbsoluteUrl = Path.Combine(pathForFolder, file.Name)
      };

      var imageHistory = _historyService.GetImageHistory(image.Id);
      if (imageHistory != null && imageHistory.IsOptimized)
        continue;

      if (tinifyEverything)
        nonOptimizedImages.Add(image);
      else
        _imageService.OptimizeImageAsync(image).GetAwaiter().GetResult();
    }
  }
}