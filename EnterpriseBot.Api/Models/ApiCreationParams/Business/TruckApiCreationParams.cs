namespace EnterpriseBot.Api.Models.ApiCreationParams.Business
{
    public class TruckApiCreationParams
    {
        public long TruckGarageId { get; set; }

        /// <summary>
        /// Trunk capacity
        /// </summary>
        public int Capacity { get; set; }

        public uint DeliveringSpeedInSeconds { get; set; }
    }
}
