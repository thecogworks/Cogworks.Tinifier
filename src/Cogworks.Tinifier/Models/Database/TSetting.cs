namespace Cogworks.Tinifier.Models.Database;

/// <summary>
/// User settings for database
/// </summary>
[TableName(PackageConstants.DbSettingsTable)]
[PrimaryKey("Id", AutoIncrement = true)]
[ExplicitColumns]
public class TSetting
{
  [PrimaryKeyColumn(AutoIncrement = true, Clustered = true)]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("ApiKey")]
  public string ApiKey { get; set; } = string.Empty;

  [Required]
  [Column("EnableOptimizationOnUpload")]
  public bool EnableOptimizationOnUpload { get; set; }

  [Required]
  [Column("EnableUndoOptimization")]
  public bool EnableUndoOptimization { get; set; }

  [Column("PreserveMetadata")]
  public bool PreserveMetadata { get; set; }

  [Column("CurrentMonthRequests")]
  public int CurrentMonthRequests { get; set; }
}