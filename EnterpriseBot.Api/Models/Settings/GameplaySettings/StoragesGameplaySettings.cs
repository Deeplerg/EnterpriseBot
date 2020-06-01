namespace EnterpriseBot.Api.Models.Settings.GameplaySettings
{
    public class StoragesGameplaySettings
    {
        public IncomeStorageGameplaySettings Income { get; set; }
        public OutcomeStorageGameplaySettings Outcome { get; set; }
        public WorkerStorageGameplaySettings Worker { get; set; }
        public ShowcaseStorageGameplaySettings Showcase { get; set; }
        public PersonalStorageGameplaySettings Personal { get; set; }
        public TrunkStorageGameplaySettings Trunk { get; set; }
    }
}
