namespace EnterpriseBot.Api.Models.Settings.GameplaySettings //!!! Settings.GameplaySettings
{
    public class GameplaySettings
    {
        public PlayerGameplaySettings Player { get; set; }
        public StoragesGameplaySettings Storages { get; set; }
    }
}
