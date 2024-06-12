namespace Cogworks.Tinifier.Models.Database;

/// <summary>
/// Response history for database
/// </summary>
[TableName(PackageConstants.DbHistoryTable)]
[PrimaryKey("Id", AutoIncrement = true)]
[ExplicitColumns]
public class TinyPNGResponseHistory
{
  [PrimaryKeyColumn(AutoIncrement = true)]
  [Column("Id")]
  public int Id { get; set; }

  [Index(IndexTypes.NonClustered)]
  [Column("ImageId")]
  public string ImageId { get; set; } = string.Empty;

  [Column("OccuredAt")]
  public DateTime OccuredAt { get; set; }

  [Column("Ratio")]
  public double Ratio { get; set; }

  [Column("Error")]
  public string Error { get; set; } = string.Empty;

  [Column("IsOptimized")]
  public bool IsOptimized { get; set; }

  [Column("OriginSize")]
  public long OriginSize { get; set; }

  [Column("OptimizedSize")]
  public long OptimizedSize { get; set; }
}