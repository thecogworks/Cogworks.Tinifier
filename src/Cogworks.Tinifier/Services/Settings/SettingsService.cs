namespace Cogworks.Tinifier.Services.Settings;

public interface ISettingsService
{
  /// <summary>
  /// Add settings to database
  /// </summary>
  /// <param name="setting">TSetting</param>
  void CreateSettings(TSetting setting);

  /// <summary>
  /// Get settings for displaying
  /// </summary>
  /// <returns>TSetting</returns>
  TSetting GetSettings();

  /// <summary>
  /// Check if user has settings and ApiKey
  /// </summary>
  TSetting CheckIfSettingExists();

  /// <summary>
  /// Update number of available requests
  /// </summary>
  /// <param name="currentMonthRequests">number of user requests</param>
  void UpdateSettings(int currentMonthRequests);
}

public class SettingsService : ISettingsService
{
  private readonly ISettingsRepository _settingsRepository;
  private readonly TSetting _cogTinifierSettings;

  public SettingsService(ISettingsRepository settingsRepository, TSetting cogTinifierSettings)
  {
    _settingsRepository = settingsRepository;
    _cogTinifierSettings = cogTinifierSettings;
  }

  public void CreateSettings(TSetting setting)
  {
    _settingsRepository.Create(setting);
  }

  public TSetting GetSettings()
  {
    return _cogTinifierSettings.HasValue() ? _cogTinifierSettings : _settingsRepository.GetSettings();
  }

  public TSetting CheckIfSettingExists()
  {
    var setting = GetSettings();

    if (setting is null || !setting.ApiKey.HasValue())
    {
      throw new EntityNotFoundException(PackageConstants.ApiKeyNotFound);
    }

    return setting;
  }

  public void UpdateSettings(int currentMonthRequests)
  {
    _settingsRepository.Update(currentMonthRequests);
  }
}