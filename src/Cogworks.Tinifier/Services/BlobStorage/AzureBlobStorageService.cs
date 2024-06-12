namespace Cogworks.Tinifier.Services.BlobStorage;

public interface IBlobStorage
{
  void SetDataForBlobStorage();

  byte[] GetBlob(string blobName);

  void PutBlob(string blobName, byte[] content);

  void DeleteBlob(string blobName);

  IEnumerable<BlobItem> GetAllBlobsInContainer();

  int CountBlobsInContainer();

  bool DoesContainerExist();

  bool DoesBlobExist(string blobName);
}

public class AzureBlobStorageService : IBlobStorage
{
  private BlobServiceClient _blobClient;
  private BlobContainerClient _blobContainer;
  private string _containerName;
  private readonly IWebHostEnvironment _hostingEnvironment;

  public AzureBlobStorageService(IWebHostEnvironment hostingEnvironment)
  {
    _hostingEnvironment = hostingEnvironment;
  }

  public void SetDataForBlobStorage()
  {
    if (_blobContainer is not null) return;
    var path = _hostingEnvironment.MapPathWebRoot("~/Web.config");
    var doc = new XmlDocument();
    doc.Load(path);
    var node = doc.SelectSingleNode("//appSettings");

    if (node is null) return;
    var keysNodes = node.SelectNodes("add");
    var dictionary = keysNodes.Cast<XmlNode>().ToDictionary(xmlNode => xmlNode.Attributes.GetNamedItem("key").Value,
      xmlNode => xmlNode.Attributes.GetNamedItem("value").Value);

    var connectionString = dictionary.First(x => x.Key == "AzureBlobFileSystem.ConnectionString:media").Value;
    var containerName = dictionary.First(x => x.Key == "AzureBlobFileSystem.ContainerName:media").Value;

    _blobClient = new BlobServiceClient(connectionString);

    _containerName = containerName;
    _blobContainer = _blobClient.GetBlobContainerClient(containerName);
  }

  public byte[] GetBlob(string blobName)
  {
    var blockBlob = _blobContainer.GetBlobClient(blobName);

    using var memoryStream = new MemoryStream();
    blockBlob.DownloadTo(memoryStream);
    var fileAsByteArray = memoryStream.ToArray();

    return fileAsByteArray;
  }

  public void PutBlob(string blobName, byte[] content)
  {
    var blockBlob = _blobContainer.GetBlobClient(blobName);

    using var stream = new MemoryStream(content, false);
    blockBlob.Upload(stream);
  }

  public void DeleteBlob(string blobName)
  {
    var blockBlob = _blobContainer.GetBlobClient(blobName);
    blockBlob.DeleteIfExists();
  }

  public IEnumerable<BlobItem> GetAllBlobsInContainer()
  {
    return _blobContainer.GetBlobs();
  }

  public int CountBlobsInContainer()
  {
    return _blobContainer.GetBlobs().Count();
  }

  public bool DoesContainerExist()
  {
    var containers = _blobClient.GetBlobContainers();
    return containers.Any(one => one.Name == _containerName);
  }

  public bool DoesBlobExist(string blobName)
  {
    var blockBlob = _blobContainer.GetBlobClient(blobName);
    return blockBlob.Exists();
  }
}