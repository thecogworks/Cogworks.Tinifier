namespace Cogworks.Tinifier.Repository.State;

public interface IStateRepository
{
  IEnumerable<TState> GetAll();

  void Create(TState entity);

  TState Get(int status);

  void Update(TState entity);

  void Delete();
}

public class StateRepository : IStateRepository
{
  private IScopeProvider _scopeProvider;

  public StateRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  /// <summary>
  /// Create state
  /// </summary>
  /// <param name="entity">TState</param>
  public void Create(TState entity)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    scope.Database.Insert(entity);
  }

  /// <summary>
  /// Get all states
  /// </summary>
  /// <returns></returns>
  public IEnumerable<TState> GetAll()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT * FROM TinifierState");
    var states = scope.Database.Fetch<TState>(query);
    return states;
  }

  /// <summary>
  /// Get state with status
  /// </summary>
  /// <param name="status">status Id</param>
  /// <returns>TState</returns>
  public TState Get(int status)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    try
    {
      var query = new Sql("SELECT * FROM TinifierState WHERE Status = @0", status);
      var states = scope.Database.FirstOrDefault<TState>(query);
      return states;
    }
    catch (Exception)
    {
      return null;
    }
  }

  /// <summary>
  /// Update State
  /// </summary>
  /// <param name="entity">TState</param>
  public void Update(TState entity)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("UPDATE TinifierState SET CurrentImage = @0, AmounthOfImages = @1, Status = @2 WHERE Id = @3", entity.CurrentImage, entity.AmounthOfImages, entity.Status, entity.Id);
    scope.Database.Execute(query);
  }

  public void Delete()
  {
    DeleteTinifierTables();
    CreateTinifierTables();
  }

  #region Private

  private void CreateTinifierTables()
  {
  }

  private void DeleteTinifierTables()
  {
    var tables = new Dictionary<string, Type>
         {
             { PackageConstants.DbSettingsTable, typeof(TSetting) },
             { PackageConstants.DbHistoryTable, typeof(TinyPNGResponseHistory) },
             { PackageConstants.DbStatisticTable, typeof(TImageStatistic) },
             { PackageConstants.DbStateTable, typeof(TState) },
             { PackageConstants.MediaHistoryTable, typeof(TinifierMediaHistory) }
         };

    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    for (var i = 0; i < tables.Count; i++)
    {
      var queryDropQuery = new Sql($"DELETE FROM {tables.ElementAt(i).Key}");
      scope.Database.Execute(queryDropQuery);
    }
  }

  #endregion Private
}