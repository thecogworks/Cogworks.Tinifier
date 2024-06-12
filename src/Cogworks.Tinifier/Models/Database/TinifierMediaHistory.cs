namespace Cogworks.Tinifier.Models.Database;

[TableName(PackageConstants.MediaHistoryTable)]
[ExplicitColumns]
public class TinifierMediaHistory
{
  [PrimaryKeyColumn(AutoIncrement = false, OnColumns = "MediaId,OrganizationRootFolderId")]
  [Required]
  [Column("MediaId")]
  public int MediaId { get; set; }

  [Required]
  [Column("FormerPath")]
  public string FormerPath { get; set; } = string.Empty;

  [PrimaryKeyColumn(AutoIncrement = false, OnColumns = "MediaId,OrganizationRootFolderId")]
  [Required]
  [Column("OrganizationRootFolderId")]
  public int OrganizationRootFolderId { get; set; }
}