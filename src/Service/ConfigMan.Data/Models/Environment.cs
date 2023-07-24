namespace ConfigMan.Data.Models;

public class Environment
{
    public string Name { get; set; } = string.Empty;
    public List<Setting> Settings { get; set; } = new();
}