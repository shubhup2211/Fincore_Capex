using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO2
{
    public class PRDTOGet2
    {
        public int PurchaseRequisitionId { get; set; }
        public string PRNumber { get; set; }
        public int? CapexRequestId { get; set; }
        public string PRTitle { get; set; }
        public decimal? Amount { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequiredTillDate { get; set; }
        public string RequiredRole { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? ApprovedBy { get; set; }
        public byte IsActive { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
