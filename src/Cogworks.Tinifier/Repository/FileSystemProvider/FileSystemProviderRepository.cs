using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

namespace Cogworks.Tinifier.Repository.FileSystemProvider;

public interface IFileSystemProviderRepository
{
  TFileSystemProviderSettings GetFileSystem();

  void Delete();

  void Create(string type);
}

public class FileSystemProviderRepository : IFileSystemProviderRepository
{
  private readonly IScopeProvider _scopeProvider;

  public FileSystemProviderRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  public void Create(string type)
  {
    var settings = new TFileSystemProviderSettings
    {
      Type = type
    };

    try
    {
      using var scope = _scopeProvider.CreateScope(autoComplete: true);
      scope.Database.Insert(settings);
    }
    catch
    {
      //Assume table doesn't exist yet
    }
  }

  public TFileSystemProviderSettings GetFileSystem()
  {
    try
    {
      using var scope = _scopeProvider.CreateScope(autoComplete: true);
      var query = new Sql("SELECT * FROM TinifierFileSystemProviderSettings");
      var queryResults = scope.Database.Fetch<TFileSystemProviderSettings>(query).FirstOrDefault();
      return queryResults;
    }
    catch (Exception)
    {
      return new TFileSystemProviderSettings
      {
        Type = "PhysicalFileSystem"
      };
    }
  }

  public void Delete()
  {
    try
    {
      using var scope = _scopeProvider.CreateScope(autoComplete: true);
      var query = new Sql("DELETE FROM TinifierFileSystemProviderSettings");
      scope.Database.Execute(query);
    }
    catch
    {
      //Assume table doesn't exist yet
    }
  }
}