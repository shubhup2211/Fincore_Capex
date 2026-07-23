using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class PurchaseOrderDTO
    {
        public int POId { get; set; }
        public string POCode { get; set; }
        public int PurchaseRequisitionId { get; set; }
        public int QuotationId { get; set; }
        public int RequestedBy { get; set; }
        public DateTime? RequiredTillDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public string ApprovalStatus { get; set; }
        public decimal Amount { get; set; }
        public int ApprovedBy { get; set; }
        public byte IsActive { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        
    }
}
