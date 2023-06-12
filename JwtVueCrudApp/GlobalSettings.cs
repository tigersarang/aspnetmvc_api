namespace JwtVueCrudApp
{
    public class GlobalSettings
    {
        public  static GlobalSettings Instance { get; set; } = new GlobalSettings();
        public string ProductContentPath { get; set; }

    }
}
