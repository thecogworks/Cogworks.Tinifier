namespace Cogworks.Tinifier.Repository.Settings;

public interface ISettingsRepository
{
  TSetting GetSettings();

  void Update(int currentMonthRequests);

  /// <summary>
  /// Create settings
  /// </summary>
  /// <param name="entity">TSetting</param>
  void Create(TSetting entity);
}

public class SettingsRepository : ISettingsRepository
{
  private IScopeProvider _scopeProvider;

  public SettingsRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  /// <summary>
  /// Get settings
  /// </summary>
  /// <returns></returns>
  public TSetting GetSettings()
  {
    try
    {
      using var scope = _scopeProvider.CreateScope(autoComplete: true);
      var query = scope.Database.Fetch<TSetting>("SELECT * FROM TinifierUserSettings ORDER BY Id DESC").FirstOrDefault() ?? new TSetting();
      return query;
    }
    catch
    {
      //Assume table hasn't been created yet
      return null;
    }
  }

  /// <summary>
  /// Create settings
  /// </summary>
  /// <param name="entity">TSetting</param>
  public void Create(TSetting entity)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    scope.Database.Insert<TSetting>(entity);
  }

  /// <summary>
  /// Update currentMonthRequests in settings
  /// </summary>
  /// <param name="currentMonthRequests">currentMonthRequests</param>
  public void Update(int currentMonthRequests)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    scope.Database.Execute(new Sql("UPDATE TinifierUserSettings SET CurrentMonthRequests = @0", currentMonthRequests));
  }
}