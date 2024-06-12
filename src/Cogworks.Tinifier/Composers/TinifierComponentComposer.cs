using Umbraco.Cms.Core.Scoping;

namespace Cogworks.Tinifier.Composers;

public class TinifierComponentComposer : ComponentComposer<TinifierComponent>
{
}

public class TinifierComponent : IComponent
{
  private readonly ICoreScopeProvider _coreScopeProvider;
  private readonly IMigrationPlanExecutor _migrationBuilder;
  private readonly IKeyValueService _keyValueService;
  private readonly IRuntimeState _runtimeState;

  public TinifierComponent(ICoreScopeProvider coreScopeProvider,
                    IMigrationPlanExecutor migrationBuilder,
                    IKeyValueService keyValueService,
                    IRuntimeState runtimeState)
  {
    _coreScopeProvider = coreScopeProvider;
    _migrationBuilder = migrationBuilder;
    _keyValueService = keyValueService;
    _runtimeState = runtimeState;
  }

  public void Initialize()
  {
    if (_runtimeState.Level < RuntimeLevel.Run)
    {
      return;
    }
    var migrationPlan = new MigrationPlan("TinifierMigrationPlan3");

    migrationPlan.From(string.Empty)
        .To<MigrationCreateTables>("tinifier-migration-db3");

    var upgrader = new Upgrader(migrationPlan);
    upgrader.Execute(_migrationBuilder, _coreScopeProvider, _keyValueService);
  }

  public void Terminate()
  {
  }
}

public class MigrationCreateTables : MigrationBase
{
  public MigrationCreateTables(IMigrationContext context) : base(context)
  {
  }

  protected override void Migrate()
  {
    if (!TableExists(PackageConstants.DbSettingsTable))
      Create.Table<TSetting>().Do();

    if (!TableExists(PackageConstants.DbHistoryTable))
      Create.Table<TinyPNGResponseHistory>().Do();

    if (!TableExists(PackageConstants.DbStatisticTable))
      Create.Table<TImageStatistic>().Do();

    if (!TableExists(PackageConstants.DbStateTable))
      Create.Table<TState>().Do();

    if (!TableExists(PackageConstants.MediaHistoryTable))
      Create.Table<TinifierMediaHistory>().Do();

    if (!TableExists(PackageConstants.DbTImageCropperInfoTable))
      Create.Table<TImageCropperInfo>().Do();

    if (!TableExists(PackageConstants.DbTFileSystemProviderSettings))
      Create.Table<TFileSystemProviderSettings>().Do();

    if (!TableExists(PackageConstants.DbTinifierImageHistoryTable))
      Create.Table<TinifierImagesHistory>().Do();
  }
}