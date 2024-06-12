namespace Cogworks.Tinifier.Repository.Statistic;

public interface IStatisticRepository
{
  void Create(TImageStatistic entity);

  TImageStatistic GetStatistic();

  long GetTotalSavedBytes();

  void Update(TImageStatistic entity);
}

public class StatisticRepository : IStatisticRepository
{
  private readonly IScopeProvider _scopeProvider;

  public StatisticRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  /// <summary>
  /// Create Statistic
  /// </summary>
  /// <param name="entity">TImageStatistic</param>
  public void Create(TImageStatistic entity)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    scope.Database.Insert(entity);
  }

  /// <summary>
  /// Get Statistic
  /// </summary>
  /// <returns>TImageStatistic</returns>
  public TImageStatistic GetStatistic()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    try
    {
      var query = new Sql("SELECT * FROM TinifierImagesStatistic");
      var queryResults = scope.Database.Fetch<TImageStatistic>(query).FirstOrDefault();
      return queryResults;
    }
    catch (Exception)
    {
      return null;
    }
  }

  public long GetTotalSavedBytes()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("select sum(originsize) - sum(optimizedsize) from [TinifierResponseHistory]");
    var queryResults = scope.Database.ExecuteScalar<long>(query);
    return queryResults;
  }

  /// <summary>
  /// Update Statistic
  /// </summary>
  /// <param name="entity">TImageStatistic</param>
  public void Update(TImageStatistic entity)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("UPDATE TinifierImagesStatistic SET NumberOfOptimizedImages = @0, TotalNumberOfImages = @1, TotalSavedBytes = @2", entity.NumberOfOptimizedImages, entity.TotalNumberOfImages, entity.TotalSavedBytes);
    scope.Database.Execute(query);
  }
}