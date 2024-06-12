namespace Cogworks.Tinifier.Models.Database;

[TableName(PackageConstants.DbTImageCropperInfoTable)]
[ExplicitColumns]
public class TImageCropperInfo
{
  [Required]
  [Column("Key")]
  public string Key { get; set; } = string.Empty;

  [Column("ImageId")]
  public string ImageId { get; set; } = string.Empty;
}