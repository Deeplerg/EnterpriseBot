namespace EnterpriseBot.VK.Models.Other
{
    public class HandleUpdateResult
    {
        public HandleUpdateResult() { }

        public HandleUpdateResult(bool success)
        {
            Successful = success;
        }

        public bool Successful { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string Result { get; set; }
    }
}
