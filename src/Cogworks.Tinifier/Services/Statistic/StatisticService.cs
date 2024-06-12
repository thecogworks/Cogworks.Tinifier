namespace Cogworks.Tinifier.Services.Statistic;

public interface IStatisticService
{
  /// <summary>
  /// Get statistic and display with chart
  /// </summary>
  /// <returns>TinifyImageStatistic</returns>
  TinifyImageStatistic GetStatistic();

  /// <summary>
  /// Update statistic
  /// </summary>
  /// <param name="savedBytes">SavedBytes for each request</param>
  void UpdateStatistic();

  void UpdateStatistic(int amount);
}

public class StatisticService : IStatisticService
{
  private readonly IImageRepository _imageRepository;
  private readonly IStatisticRepository _statisticRepository;

  public StatisticService(IImageRepository imageRepository, IStatisticRepository statisticRepository)
  {
    _imageRepository = imageRepository;
    _statisticRepository = statisticRepository;
  }

  private TImageStatistic CreateInitialStatistic()
  {
    var newStat = new TImageStatistic
    {
      TotalNumberOfImages = _imageRepository.AmountOfItems(),
      NumberOfOptimizedImages = _imageRepository.AmountOfOptimizedItems(),
      TotalSavedBytes = 0
    };

    _statisticRepository.Create(newStat);
    return newStat;
  }

  public TinifyImageStatistic GetStatistic()
  {
    var statistic = _statisticRepository.GetStatistic() ?? CreateInitialStatistic();

    var tImageStatistic = new TinifyImageStatistic
    {
      TotalOptimizedImages = statistic.NumberOfOptimizedImages,
      TotalOriginalImages = statistic.TotalNumberOfImages - statistic.NumberOfOptimizedImages <= 0 ? 0 : statistic.TotalNumberOfImages - statistic.NumberOfOptimizedImages,
      TotalSavedBytes = statistic.TotalSavedBytes
    };

    return tImageStatistic;
  }

  public void UpdateStatistic()
  {
    var statistic = _statisticRepository.GetStatistic() ?? CreateInitialStatistic();

    statistic.TotalNumberOfImages = _imageRepository.AmountOfItems();
    statistic.NumberOfOptimizedImages = _imageRepository.AmountOfOptimizedItems();
    statistic.TotalSavedBytes = _statisticRepository.GetTotalSavedBytes();

    _statisticRepository.Update(statistic);
  }

  public void UpdateStatistic(int amount)
  {
    var statistic = _statisticRepository.GetStatistic() ?? CreateInitialStatistic();

    statistic.TotalNumberOfImages = _imageRepository.AmountOfItems() - amount;
    statistic.NumberOfOptimizedImages = _imageRepository.AmountOfOptimizedItems();
    statistic.TotalSavedBytes = _statisticRepository.GetTotalSavedBytes();

    _statisticRepository.Update(statistic);
  }
}