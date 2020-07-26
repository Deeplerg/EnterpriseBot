namespace EnterpriseBot.ApiWrapper.Models.Other
{
    public class ApiRequestInfo
    {
        public string CategoryAreaName { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string CategorySubAreaName { get; set; }
        public string CategoryName { get; set; }
        public string MethodName { get; set; }
    }
}
