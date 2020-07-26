using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class TruckCreationParams
    {
        public long TruckGarageId { get; set; }

        /// <summary>
        /// Trunk capacity
        /// </summary>
        public int Capacity { get; set; }

        public uint DeliveringSpeedInSeconds { get; set; }
    }
}
