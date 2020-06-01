namespace EnterpriseBot.Api.Models.Other
{
    public class LocalizedError
    {
        public ErrorSeverity ErrorSeverity { get; set; }

        public string RussianMessage { get; set; }
        public string EnglishMessage { get; set; }
    }
}
