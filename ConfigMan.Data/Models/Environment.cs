namespace ConfigMan.Data.Models;

public class Environment
{
    public string Name { get; set; }
    public List<Setting> Settings { get; set; } = new();
}