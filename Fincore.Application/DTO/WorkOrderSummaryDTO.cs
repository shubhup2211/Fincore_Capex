namespace Fincore.Application.DTOs.WorkOrder
{
    public class WorkOrderSummaryDTO
    {
        public int TotalWorkOrders { get; set; }

        public int PendingWorkOrders { get; set; }

        public int CompletedWorkOrders { get; set; }

        public decimal TotalNetAmount { get; set; }
    }
}