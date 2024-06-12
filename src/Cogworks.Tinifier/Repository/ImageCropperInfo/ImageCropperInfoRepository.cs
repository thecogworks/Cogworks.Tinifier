namespace Cogworks.Tinifier.Repository.ImageCropperInfo;

public interface IImageCropperInfoRepository
{
  TImageCropperInfo Get(string key);

  void Create(string key, string imageId);

  void Delete(string key);

  void Update(string key, string imageId);
}

public class ImageCropperInfoRepository : IImageCropperInfoRepository
{
  private readonly IScopeProvider _scopeProvider;

  public ImageCropperInfoRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  public TImageCropperInfo Get(string key)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT * FROM TinifierImageCropperInfo WHERE [Key] = @0", key);
    var queryResults = scope.Database.Fetch<TImageCropperInfo>(query).FirstOrDefault();
    return queryResults;
  }

  public void Create(string key, string imageId)
  {
    var info = new TImageCropperInfo
    {
      Key = key,
      ImageId = imageId
    };

    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    scope.Database.Insert(info);
  }

  public void Delete(string key)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("DELETE FROM TinifierImageCropperInfo WHERE [Key] = @0", key);
    scope.Database.Execute(query);
  }

  public void Update(string key, string imageId)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("UPDATE TinifierImageCropperInfo SET ImageId = @0 WHERE [Key] = @1", imageId, key);
    scope.Database.Execute(query);
  }
}