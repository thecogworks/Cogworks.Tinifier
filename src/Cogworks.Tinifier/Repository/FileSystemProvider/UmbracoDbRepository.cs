namespace Cogworks.Tinifier.Repository.FileSystemProvider;

public interface IUmbracoDbRepository
{
  string GetMediaAbsoluteUrl(int id);

  List<int> GetTrashedNodes();

  List<UmbracoNode> GetNodesByName(string name);
}

public class UmbracoDbRepository : IUmbracoDbRepository
{
  private readonly IScopeProvider _scopeProvider;

  public UmbracoDbRepository(IScopeProvider scopeProvider)
  {
    _scopeProvider = scopeProvider;
  }

  public string GetMediaAbsoluteUrl(int id)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT Data FROM CmsContentNu WHERE NodeId = @0", id);

    var serializedRootObject = scope.Database.Fetch<string>(query).FirstOrDefault();
    if (serializedRootObject is null)
    {
      return "";
    }

    var rootObject = JSON.ToObject<RootObject>(serializedRootObject);
    if (rootObject?.Properties?.UmbracoFile is null)
    {
      Log.Error($"Tinifier - Error finding rootObject for id {id}");
    }
    var umbracoFile = rootObject.Properties.UmbracoFile.FirstOrDefault();
    if (umbracoFile is null)
    {
      return "";
    }
    var valModel = JSON.ToObject<Val>(umbracoFile.Val);
    return valModel.Src;
  }

  public List<int> GetTrashedNodes()
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT Id FROM UmbracoNode WHERE trashed = 1");
    var trashedIds = scope.Database.Fetch<int>(query);
    return trashedIds;
  }

  public List<UmbracoNode> GetNodesByName(string name)
  {
    using var scope = _scopeProvider.CreateScope(autoComplete: true);
    var query = new Sql("SELECT Id FROM UmbracoNode WHERE text = @0", name);
    var queryResults = scope.Database.Fetch<UmbracoNode>(query);
    return queryResults;
  }
}