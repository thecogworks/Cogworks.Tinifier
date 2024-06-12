namespace Cogworks.Tinifier.Repository.History;

public interface IHistoryRepository
{
  void Delete(string imageId);

  TinyPNGResponseHistory Get(string id);

  IEnumerable<TinyPNGResponseHistory> GetHistoryByPath(string path);

  void Create(TinyPNGResponseHistory entity);

  IEnumerable<TinyPNGResponseHistory> GetAll();
}

public class HistoryRepository : IHistoryRepository
{
  private IScopeProvider _scopeProvider;

  public HistoryRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  /// <summary>
  /// Get all tinifing histories from database
  /// </summary>
  /// <returns>IEnumerable of TinyPNGResponseHistory</returns>
  public IEnumerable<TinyPNGResponseHistory> GetAll()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete:true);
    // For testing is currently using sqlite script . For sql server script is okay use : DATEADD(day,-10,GETDATE()) DATE('now', '-10 day')
    var select = new Sql("SELECT * FROM TinifierResponseHistory WHERE OccuredAt >=  DATEADD(day,-10,GETDATE()) AND IsOptimized = 'true'");
    var queryResults = scope.Database.Fetch<TinyPNGResponseHistory>(select);
    return queryResults;
  }

  /// <summary>
  /// Get history by Id
  /// </summary>
  /// <param name="id">history Id</param>
  /// <returns>TinyPNGResponseHistory</returns>
  public TinyPNGResponseHistory Get(string id)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete:true);
    var query = new Sql("SELECT * FROM TinifierResponseHistory WHERE ImageId = @0", id);
    var queryResults = scope.Database.Fetch<TinyPNGResponseHistory>(query).FirstOrDefault();

    return queryResults;
  }

  public IEnumerable<TinyPNGResponseHistory> GetHistoryByPath(string path)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete:true);
    var query = new Sql("SELECT * FROM TinifierResponseHistory WHERE ImageId LIKE @0", $"%{path}%");
    var queryResults = scope.Database.Fetch<TinyPNGResponseHistory>(query);
    return queryResults;
  }

  /// <summary>
  /// Create history
  /// </summary>
  /// <param name="newItem">TinyPNGResponseHistory</param>
  public void Create(TinyPNGResponseHistory newItem)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete:true);
    scope.Database.Insert(newItem);
  }

  /// <summary>
  /// Delete history for image
  /// </summary>
  /// <param name="imageId">Image Id</param>
  public void Delete(string imageId)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("DELETE FROM TinifierResponseHistory WHERE ImageId = @0", imageId);
    scope.Database.Execute(query);
  }
}