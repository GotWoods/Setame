namespace ConfigMan.Service.Models
{
    public class SetupRequest
    {
        public string AdminEmailAddress { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
