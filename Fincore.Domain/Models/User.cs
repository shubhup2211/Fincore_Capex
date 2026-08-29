using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fincore.Domain.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; }

        [Column("User Category")]
        public string UserCategory { get; set; }

        [StringLength(12)]
        public string Phone { get; set; }

        public DateTime? LastLogin { get; set; }
        public string RefreshToken { get; set; }

        public bool Is2FAEnabled { get; set; } = false;
        public string? TwoFactorSecretKey { get; set; }

        [Required]
        public byte IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        [Required]
        [ForeignKey("ModifiedByUser")]
        public int ModifiedBy { get; set; }
        public User ModifiedByUser { get; set; }


        // Navigation Properties
        public List<CapexRequest> CapexApprovals { get; set; }
        public List<Company> CompaniesCreated { get; set; }
        public List<Company> CompaniesModified { get; set; }
        public List<Department> DepartmentsCreated { get; set; }
        public List<Department> DepartmentsModified { get; set; }
        public List<Permission> PermissionsCreated { get; set; }
        public List<Permission> PermissionsModified { get; set; }
        public List<Role> RolesCreated { get; set; }
        public List<Role> RolesModified { get; set; }
        public List<Vendor> VendorsCreated { get; set; }
        public List<Vendor> VendorsModified { get; set; }
        public List<DocumentType> DocumentTypesCreated { get; set; }
        public List<DocumentType> DocumentTypesModified { get; set; }
        public List<Document> Documents { get; set; }
        public List<VendorCategory> VendorCategoriesCreated { get; set; }
        public List<VendorCategory> VendorCategoriesModified { get; set; }
        public List<BudgetCategory> BudgetCategoriesCreated { get; set; }
        public List<BudgetCategory> BudgetCategoriesModified { get; set; }
        public List<Budget> BudgetsCreated { get; set; }
        public List<Budget> BudgetsModified { get; set; }
        public List<BudgetLine> BudgetLinesCreated { get; set; }
        public List<BudgetLine> BudgetLinesModified { get; set; }
        public List<ApprovalFlow> ApprovalFlowsCreated { get; set; }
        public List<ApprovalFlow> ApprovalFlowsModified { get; set; }
        public List<CapexRequest> CapexRequestsRequested { get; set; }
        public List<CapexRequest> CapexRequestsApproved { get; set; }
        public List<PurchaseRequisition> PurchaseRequisitionsRequested { get; set; }
        public List<PurchaseRequisition> PurchaseRequisitionsApproved { get; set; }
        public List<PurchaseRequisition> PurchaseRequisitionsCreated { get; set; }
        public List<PurchaseRequisition> PurchaseRequisitionsModified { get; set; }
        public List<PurchaseOrder> PurchaseOrdersRequested { get; set; }
        public List<PurchaseOrder> PurchaseOrdersApproved { get; set; }
        public List<PurchaseOrder> PurchaseOrdersCreated { get; set; }
        public List<PurchaseOrder> PurchaseOrdersModified { get; set; }
        public List<GRN> GRNsReceived { get; set; }
        public List<GRN> GRNsQualityChecked { get; set; }
        public List<OpexRequest> OpexRequestsRequested { get; set; }
        public List<OpexRequest> OpexRequestsApproved { get; set; }
        public List<ExpenseClaim> ExpenseClaimsClaimed { get; set; }
        public List<ExpenseClaim> ExpenseClaimsApproved { get; set; }
        public List<WorkOrder> WorkOrdersCreated { get; set; }
        public List<VendorSelection> VendorSelectionsSelected { get; set; }
        public List<APInvoice> APInvoicesApproved { get; set; }
        public List<Payment> PaymentsApproved { get; set; }
        public List<JournalEntry> JournalEntriesCreated { get; set; }
        public List<AccountMaster> AccountMastersCreated { get; set; }
        public List<AccountMaster> AccountMastersModified { get; set; }
        public List<RevenueEntry> RevenueEntriesCreated { get; set; }
        public List<RevenueEntry> RevenueEntriesModified { get; set; }
        public List<AuditLog> AuditLogs { get; set; }
        public List<UserActivityLog> UserActivityLogs { get; set; }
        public List<NotificationLog> NotificationLogs { get; set; }
        public List<ApprovalLog> ApprovalLogs { get; set; }
    }
}
