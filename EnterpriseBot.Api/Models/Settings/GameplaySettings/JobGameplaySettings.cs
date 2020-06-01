namespace EnterpriseBot.Api.Models.Settings.GameplaySettings
{
    public class JobGameplaySettings
    {
        public decimal DefaultSpeedModifier { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MinSpeedModifier { get; set; }
        public decimal DiminishModifierStep { get; set; }
        public decimal DiminishModifierSalaryIncrease { get; set; }
        public int MaxWorkingHoursPlayer { get; set; }
        public int MaxWorkingHoursBot { get; set; }
        public decimal BotMaintenanceSalary { get; set; }
    }
}
