namespace Cogworks.Tinifier.Models.Database;

/// <summary>
/// State table with information about current tinifing
/// </summary>
[TableName(PackageConstants.DbStateTable)]
[PrimaryKey("Id", AutoIncrement = true)]
public class TState
{
  [PrimaryKeyColumn(AutoIncrement = true, Clustered = true)]
  [Column("Id")]
  public int Id { get; set; }

  [Column("CurrentImage")]
  public int CurrentImage { get; set; }

  [Column("AmounthOfImages")]
  public int AmounthOfImages { get; set; }

  [Column("Status")]
  public int Status { get; set; }

  [Ignore]
  public Statuses StatusType
  {
    get => (Statuses)Status;
    set => Status = (int)value;
  }
}