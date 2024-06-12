namespace Cogworks.Tinifier.Models.Database;

[TableName(PackageConstants.DbTFileSystemProviderSettings)]
[PrimaryKey("Id", AutoIncrement = true)]
[ExplicitColumns]
public class TFileSystemProviderSettings
{
  [PrimaryKeyColumn(AutoIncrement = true)]
  [Column("Id")]
  public int Id { get; set; }

  [Column("Type")]
  public string Type { get; set; } = string.Empty;
}