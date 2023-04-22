namespace ConfigMan.Service.Models;

public class CreateApplicationSettingRequest
{
    public Guid ApplicationId { get; set; }
    public List<EnvironmentSetting> Settings { get; set; }
}