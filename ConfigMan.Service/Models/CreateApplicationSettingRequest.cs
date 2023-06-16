namespace ConfigMan.Service.Models;

public class CreateApplicationSettingRequest
{
    public string ApplicationName{ get; set; }
    public List<EnvironmentSetting> Settings { get; set; }
}