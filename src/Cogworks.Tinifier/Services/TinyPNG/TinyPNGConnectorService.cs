namespace Cogworks.Tinifier.Services.TinyPNG;

public interface ITinyPNGConnector
{
  /// <summary>
  /// Send Image to TinyPNG Service
  /// </summary>
  /// <param name="imageUrl">Image url</param>
  /// <returns>Task<TinyResponse></returns>
  Task<TinyResponse> TinifyAsync(TImage tImage, IFileSystem fs);
}

public class TinyPNGConnectorService : ITinyPNGConnector
{
  private readonly string _tinifyAddress;
  private readonly ISettingsService _settingsService;
  private readonly IFileSystemProviderRepository _fileSystemProviderRepository;
  private readonly IImageHistoryService _imageHistoryService;

  public TinyPNGConnectorService(ISettingsService settingsService, IFileSystemProviderRepository fileSystemProviderRepository,
      IImageHistoryService imageHistoryService)
  {
    _tinifyAddress = PackageConstants.TinyPngUrl;
    _settingsService = settingsService;
    _fileSystemProviderRepository = fileSystemProviderRepository;
    _imageHistoryService = imageHistoryService;
  }

  public async Task<TinyResponse> TinifyAsync(TImage tImage, IFileSystem fs)
  {
    TinyResponse tinyResponse;
    var path = tImage.AbsoluteUrl;
    int.TryParse(tImage.Id, out var id);
    var settings = _settingsService.GetSettings();

    try
    {
      var imageBytes = GetImageBytesFromPath(tImage, fs, path);
      tinyResponse = await TinifyByteArrayAsync(imageBytes);

      if (id > 0 && settings.EnableUndoOptimization)
        _imageHistoryService.Create(tImage, imageBytes);
    }
    catch (Exception)
    {
      tinyResponse = new TinyResponse
      {
        Output = new TinyOutput
        {
          Error = PackageConstants.ImageDeleted,
          IsOptimized = false
        }
      };
    }

    return tinyResponse;
  }

  private byte[] GetImageBytesFromPath(TImage tImage, IFileSystem fs, string path)
  {
    byte[] imageBytes;
    var fileSystem = _fileSystemProviderRepository.GetFileSystem();
    if (fileSystem != null)
    {
      if (fileSystem.Type.Contains("PhysicalFileSystem"))
        path = fs.GetRelativePath(tImage.AbsoluteUrl);
    }

    using (var file = fs.OpenFile(path))
    {
      imageBytes = SolutionExtensions.ReadFully(file);
    }

    return imageBytes;
  }

  private async Task<TinyResponse> TinifyByteArrayAsync(byte[] imageByteArray)
  {
    TinyResponse tinyResponse;

    var byteContent = new ByteArrayContent(imageByteArray);

    try
    {
      var responseResult = await CreateRequestAsync(byteContent).ConfigureAwait(false);
      tinyResponse = JsonConvert.DeserializeObject<TinyResponse>(responseResult);
      tinyResponse.Output.IsOptimized = true;
    }
    catch (HttpRequestException ex)
    {
      tinyResponse = new TinyResponse
      {
        Output = new TinyOutput
        {
          Error = ex.Message,
          IsOptimized = false
        }
      };
    }

    return tinyResponse;
  }

  private async Task<string> CreateRequestAsync<T>(T inputData)
  {
    HttpResponseMessage response;
    var apiKey = _settingsService.GetSettings().ApiKey;
    var authKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(PackageConstants.ApiKey + apiKey));

    using (var client = new HttpClient())
    {
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(PackageConstants.BasicAuth, authKey);
      client.DefaultRequestHeaders.TryAddWithoutValidation(PackageConstants.ContentTypeHeader, PackageConstants.ContentType);
      client.BaseAddress = new Uri(_tinifyAddress);

      try
      {
        response = await client.PostAsync(PackageConstants.TinyPngUri, inputData as HttpContent).ConfigureAwait(false);
      }
      catch (TaskCanceledException)
      {
        throw new HttpRequestException(PackageConstants.TooBigImage);
      }

      if (!response.IsSuccessStatusCode)
      {
        var message = (int)response.StatusCode + response.ReasonPhrase;
        throw new HttpRequestException(message);
      }
    }

    var currentMonthRequests = GetHeaderValue(response);
    _settingsService.UpdateSettings(currentMonthRequests);

    var responseResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    return responseResult;
  }

  private int GetHeaderValue(HttpResponseMessage response)
  {
    var headerValues = response.Headers.GetValues(PackageConstants.TinyPngHeader);
    var compressionHeader = headerValues.FirstOrDefault();

    if (compressionHeader == null)
    {
      throw new HttpRequestException(HttpStatusCode.BadRequest + PackageConstants.BadRequest);
    }

    var currentMonthRequests = int.Parse(compressionHeader);
    return currentMonthRequests;
  }
}