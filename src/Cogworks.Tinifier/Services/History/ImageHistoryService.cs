using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace Cogworks.Tinifier.Services.History;

public interface IImageHistoryService
{
  void Create(TImage image, byte[] originImage);

  TinifierImagesHistory Get(int id);

  void Delete(int id);
}

public class ImageHistoryService : IImageHistoryService
{
  private readonly IImageHistoryRepository _imageHistoryRepository;
  private readonly IHostingEnvironment _hostingEnvironment;

  public ImageHistoryService(IImageHistoryRepository imageHistoryRepository,
                              IHostingEnvironment hostingEnvironment)
  {
    _imageHistoryRepository = imageHistoryRepository;
    _hostingEnvironment = hostingEnvironment;
  }

  public void Create(TImage image, byte[] originImage)
  {
    #pragma warning disable CS0618
    var directoryPath = _hostingEnvironment.MapPathWebRoot(PackageConstants.TinifierTempFolder);
    #pragma warning restore CS0618

    Directory.CreateDirectory(directoryPath);

    var filePath = Path.Combine(directoryPath, image.Name);
    System.IO.File.WriteAllBytes(filePath, originImage);

    var model = new TinifierImagesHistory
    {
      ImageId = image.Id,
      OriginFilePath = filePath
    };

    _imageHistoryRepository.Create(model);
  }

  public TinifierImagesHistory Get(int id)
  {
    return _imageHistoryRepository.Get(id);
  }

  public void Delete(int id)
  {
    _imageHistoryRepository.Delete(id);
  }
}