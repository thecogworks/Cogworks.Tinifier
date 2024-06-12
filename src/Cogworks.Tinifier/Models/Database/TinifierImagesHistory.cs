namespace Cogworks.Tinifier.Models.Database;

[TableName(PackageConstants.DbTinifierImageHistoryTable)]
[PrimaryKey("Id", AutoIncrement = true)]
[ExplicitColumns]
public class TinifierImagesHistory
{
  [PrimaryKeyColumn(AutoIncrement = true)]
  [Column("Id")]
  public int Id { get; set; }

  [Column("ImageId")]
  public string ImageId { get; set; } = string.Empty;

  [Column("OriginFilePath")]
  public string OriginFilePath { get; set; } = string.Empty;
}