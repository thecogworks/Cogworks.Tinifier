namespace Cogworks.Tinifier.Services.Validation;

public interface IValidationService
{
  /// <summary>
  /// Check folder this or Image
  /// </summary>
  /// <param name="mediaId">Media Id</param>
  /// <returns>If this folder or not</returns>
  bool IsFolder(int mediaId);

  /// <summary>
  /// Check concurrent folder optimizing
  /// </summary>
  void ValidateConcurrentOptimizing();

  /// <summary>
  /// Check Image Extension
  /// </summary>
  /// <param name="source"></param>
  /// <param name="media"></param>
  /// <returns>Supported exception or not</returns>
  void ValidateExtension(Media media);
}

public class ValidationService : IValidationService
{
  private readonly IStateRepository _stateRepository;
  private readonly IImageRepository _imageRepository;

  public ValidationService(IStateRepository stateRepository, IImageRepository imageRepository)
  {
    _stateRepository = stateRepository;
    _imageRepository = imageRepository;
  }

  public void ValidateConcurrentOptimizing()
  {
    var state = _stateRepository.Get((int)Statuses.InProgress);

    if (state is not null)
      throw new ConcurrentOptimizingException(PackageConstants.ConcurrentOptimizing);
  }

  public bool IsFolder(int itemId)
  {
    var item = _imageRepository.Get(itemId);

    return string.Equals(item?.ContentType.Alias, PackageConstants.FolderAlias, StringComparison.OrdinalIgnoreCase);
  }

  public void ValidateExtension(Media media)
  {
    if (media is null)
      throw new EntityNotFoundException();
    if (!media.HasProperty("umbracoExtension"))
      throw new NotSupportedExtensionException();
    var fileExt = media.GetValue<string>("umbracoExtension") ?? string.Empty;
    if (PackageConstants.SupportedExtensions.Any(supportedExt => string.Equals(supportedExt, fileExt, StringComparison.OrdinalIgnoreCase)))
    {
      return;
    }
    throw new NotSupportedExtensionException(fileExt);
  }
}