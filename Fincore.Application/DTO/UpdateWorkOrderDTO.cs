using System;

namespace Fincore.Application.DTOs.WorkOrder
{
    public class UpdateWorkOrderDTO
    {
        public string WONumber { get; set; }

        public string Title { get; set; }

        public int VendorId { get; set; }

        public int OpexRequestId { get; set; }

        public decimal NetAmount { get; set; }

        public string Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? CompletedDate { get; set; }
    }
}