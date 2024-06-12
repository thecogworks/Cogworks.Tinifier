namespace Cogworks.Tinifier.Models.Database;

[TableName(PackageConstants.DbStatisticTable)]
[ExplicitColumns]
public class TImageStatistic
{
  [Column("TotalNumberOfImages")]
  public int TotalNumberOfImages { get; set; }

  [Column("NumberOfOptimizedImages")]
  public int NumberOfOptimizedImages { get; set; }

  [Column("TotalSavedBytes")]
  public long TotalSavedBytes { get; set; }
}