namespace Cogworks.Tinifier.Repository.History;

public interface IImageHistoryRepository
{
  IEnumerable<TinifierImagesHistory> GetAll();

  TinifierImagesHistory Get(int id);

  void Create(TinifierImagesHistory entity);

  void Delete(int id);
}

public class ImageHistoryRepository : IImageHistoryRepository
{
  private IScopeProvider _scopeProvider;

  public ImageHistoryRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  public IEnumerable<TinifierImagesHistory> GetAll()
  {
    throw new System.NotImplementedException();
  }

  public TinifierImagesHistory Get(int id)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT * FROM TinifierImageHistory WHERE [ImageId] = @0", id.ToString());
    var queryResults = scope.Database.Fetch<TinifierImagesHistory>(query).FirstOrDefault();
    return queryResults;
  }

  public void Create(TinifierImagesHistory entity)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    scope.Database.Insert(entity);
  }

  public void Delete(int id)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("DELETE FROM TinifierImageHistory WHERE [ImageId] = @0", id.ToString());
    scope.Database.Execute(query);
  }
}