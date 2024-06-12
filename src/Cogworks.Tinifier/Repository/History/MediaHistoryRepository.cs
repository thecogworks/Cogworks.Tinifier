namespace Cogworks.Tinifier.Repository.History;

public interface IMediaHistoryRepository
{
  IEnumerable<TinifierMediaHistory> GetAll();

  TinifierMediaHistory Get(int id);

  void Create(TinifierMediaHistory entity);

  void Delete(int Id);

  void DeleteAll(int baseFolderId);
}

public class MediaHistoryRepository : IMediaHistoryRepository
{
  private readonly string _tableName = "TinifierMediaHistories";
  private readonly IScopeProvider _scopeProvider;

  public MediaHistoryRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  /// <summary>
  /// Insert new record into the DB
  /// </summary>
  /// <param name="entity">Model to inserting</param>
  public void Create(TinifierMediaHistory entity)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    scope.Database.Insert(entity);
  }

  /// <summary>
  /// Clear all history
  /// </summary>
  /// <param name="id">Id of a media</param>
  public void Delete(int id)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql($"DELETE FROM {_tableName} WHERE MediaId = {id}");
    scope.Database.Execute(query);
  }

  /// <summary>
  /// Clear all history
  /// </summary>
  public void DeleteAll()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql($"DELETE FROM {_tableName}");
    scope.Database.Execute(query);
  }

  /// <summary>
  /// Get former state of a certain media
  /// </summary>
  /// <param name="id">Id of a media</param>
  /// <returns>TinifierMediaHistory</returns>
  public TinifierMediaHistory Get(int id)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql($"SELECT MediaId, FormerPath, OrganizationRootFolderId FROM {_tableName} WHERE MediaId = {id}");
    var queryResults = scope.Database.FirstOrDefault<TinifierMediaHistory>(query);
    return queryResults;
  }

  /// <summary>
  /// Get former state of all media
  /// </summary>
  /// <returns>IEnumerable of TinifierMediaHistory</returns>
  public IEnumerable<TinifierMediaHistory> GetAll()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql($"SELECT MediaId, FormerPath, OrganizationRootFolderId FROM {_tableName}");
    var queryResults = scope.Database.Fetch<TinifierMediaHistory>(query);
    return queryResults;
  }

  public void DeleteAll(int baseFolderId)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql($"DELETE FROM {_tableName} WHERE OrganizationRootFolderId = {baseFolderId}");
    scope.Database.Execute(query);
  }
}