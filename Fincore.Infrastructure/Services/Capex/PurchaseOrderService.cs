using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Fincore.Application.Constants;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Fincore.Infrastructure.Services.Capex
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;

        private const string PurchaseOrderCacheKey = "PurchaseOrder";


        public PurchaseOrderService(AppDbContext db,IMapper mapper,IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.cache = cache;
        }
        private async Task<string> GeneratePOCode()
        {
            var year = DateTime.Now.Year;


            var lastPO = await db.PurchaseOrders
                .OrderByDescending(x => x.POId)
                .FirstOrDefaultAsync();


            int nextNumber = 1;



            if (lastPO != null)
            {
                var lastNumber = lastPO.POCode
                    .Split('-')
                    .Last();


                nextNumber = int.Parse(lastNumber) + 1;
            }



            return $"PO-{year}-{nextNumber.ToString("D4")}";
        }

        public async Task<ApiResponse<PurchaseOrderDTO>> AddPurchaseOrder(PurchaseOrderDTO dto)
        {

            var prExists = await db.PurchaseRequisitions
                .AnyAsync(x => x.PurchaseRequisitionId == dto.PurchaseRequisitionId);


            if (!prExists)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Invalid Purchase Requisition",
                    "400",
                    "Purchase Requisition does not exist"
                );
            }



            var quotationExists = await db.Quotations
                .AnyAsync(x => x.QuotationId == dto.QuotationId);



            if (!quotationExists)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Invalid Quotation",
                    "400",
                    "Quotation does not exist"
                );
            }




            if (dto.RequiredTillDate.HasValue &&
               dto.RequiredTillDate.Value < DateTime.Today)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Invalid Required Date",
                    "400",
                    "Required Till Date cannot be before today"
                );
            }





            if (dto.Amount <= 0)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Invalid Amount",
                    "400",
                    "Purchase Order amount must be greater than zero"
                );
            }



            dto.POCode = await GeneratePOCode();

            var data = mapper.Map<PurchaseOrder>(dto);


            data.ApprovalStatus = ApprovalStatus.Draft;

            data.CreatedAt = DateTime.Now;

            data.ModifiedAt = DateTime.Now;



            await db.PurchaseOrders.AddAsync(data);


            var result = await db.SaveChangesAsync();



            if (result > 0)
            {

                cache.Remove(PurchaseOrderCacheKey);


                return ApiResponseHelper.SuccessRes(
                    mapper.Map<PurchaseOrderDTO>(data),
                    "Purchase Order Created Successfully"
                );

            }



            return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                "Purchase Order Not Created",
                "500",
                "Error while saving data"
            );

        }



        public async Task<ApiResponse<PurchaseOrderDTO>> GetPurchaseOrder(int id)
        {
            var data = await db.PurchaseOrders
                               .FirstOrDefaultAsync(x => x.POId == id);


            if (data == null)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }


            return ApiResponseHelper.SuccessRes(
                mapper.Map<PurchaseOrderDTO>(data),
                "Purchase Order Retrieved Successfully"
            );
        }



        public async Task<ApiResponse<List<PurchaseOrderDTO>>> GetPurchaseOrderByStatus(
    string status,
    int page,
    int pageSize)
        {
            var query = db.PurchaseOrders
                          .Where(x => x.ApprovalStatus == status);

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.POId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!data.Any())
            {
                return ApiResponseHelper.Failure<List<PurchaseOrderDTO>>(
                    "Purchase Orders Not Found",
                    "404",
                    "No Purchase Orders Found");
            }

            return ApiResponseHelper.SuccessRes(
                mapper.Map<List<PurchaseOrderDTO>>(data),
                "Purchase Orders Retrieved Successfully",
                totalRecords,
                new
                {
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    hasNextPage = page * pageSize < totalRecords,
                    hasPreviousPage = page > 1
                });
        }


        public async Task<ApiResponse<PurchaseOrderDTO>> UpdatePurchaseOrder(
            int id,
            PurchaseOrderDTO dto)
        {

            var data = await db.PurchaseOrders
                               .FirstOrDefaultAsync(x => x.POId == id);


            if (data == null)
            {
                if (data.ApprovalStatus == ApprovalStatus.Closed)
                {
                    return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                        "Cannot Update Closed PO",
                        "400",
                        "Closed Purchase Order cannot be updated"
                    );
                }
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }


            mapper.Map(dto, data);

            await db.SaveChangesAsync();


            cache.Remove(PurchaseOrderCacheKey);


            return ApiResponseHelper.SuccessRes(
                mapper.Map<PurchaseOrderDTO>(data),
                "Purchase Order Updated Successfully"
            );
        }



        public async Task<ApiResponse<PurchaseOrderDTO>> DeletePurchaseOrder(int id)
        {

            var data = await db.PurchaseOrders
                .FirstOrDefaultAsync(x => x.POId == id);



            if (data == null)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }



            // Cannot delete Approved PO
            if (data.ApprovalStatus == ApprovalStatus.Approved)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Cannot Delete Approved PO",
                    "400",
                    "Approved Purchase Order cannot be deleted"
                );
            }



            // Cannot delete Closed PO
            if (data.ApprovalStatus == ApprovalStatus.Closed)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Cannot Delete Closed PO",
                    "400",
                    "Closed Purchase Order cannot be deleted"
                );
            }




            // Check GRN generated or not
            var grnExists = await db.GRNs
                .AnyAsync(x => x.POId == id);



            if (grnExists)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Cannot Delete Purchase Order",
                    "400",
                    "GRN already generated for this Purchase Order"
                );
            }


            var result = mapper.Map<PurchaseOrderDTO>(data);


            db.PurchaseOrders.Remove(data);


            await db.SaveChangesAsync();



            cache.Remove(PurchaseOrderCacheKey);



            return ApiResponseHelper.SuccessRes(
                result,
                "Purchase Order Deleted Successfully"
            );

        }



        public async Task<ApiResponse<PurchaseOrderDTO>> ApprovePurchaseOrder(int id)
        {

            var data = await db.PurchaseOrders
                .FirstOrDefaultAsync(x => x.POId == id);



            if (data == null)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }



            if (data.ApprovalStatus == ApprovalStatus.Approved)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Already Approved",
                    "400",
                    "Purchase Order already approved"
                );
            }



            if (data.ApprovalStatus == ApprovalStatus.Cancelled)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Cancelled PO Cannot Approve",
                    "400",
                    "Cancelled Purchase Order cannot be approved"
                );
            }




            var hasItems = await db.PurchaseOrderItems
                .AnyAsync(x => x.POId == id);



            if (!hasItems)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Items Missing",
                    "400",
                    "Purchase Order must contain minimum one item"
                );
            }




            if (data.Amount <= 0)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Invalid Amount",
                    "400",
                    "Amount must be greater than zero"
                );
            }




            data.ApprovalStatus = ApprovalStatus.Approved;

            data.ModifiedAt = DateTime.Now;



            await db.SaveChangesAsync();


            cache.Remove(PurchaseOrderCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<PurchaseOrderDTO>(data),
                "Purchase Order Approved Successfully"
            );

        }
        public async Task<ApiResponse<List<PurchaseOrderDTO>>> GetPurchaseOrderByStatus(string status)
        {
            var data = await db.PurchaseOrders
                               .Where(x => x.ApprovalStatus == status)
                               .ToListAsync();

            if (!data.Any())
            {
                return ApiResponseHelper.Failure<List<PurchaseOrderDTO>>(
                    "Purchase Orders Not Found",
                    "404",
                    "No Purchase Orders Found"
                );
            }

            return ApiResponseHelper.SuccessRes(
                mapper.Map<List<PurchaseOrderDTO>>(data),
                "Purchase Orders Retrieved Successfully"
            );
        }


        public async Task<ApiResponse<PurchaseOrderDTO>> CancelPurchaseOrder(int id)
        {
            var data = await db.PurchaseOrders
                               .FirstOrDefaultAsync(x => x.POId == id);


            if (data == null)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }


            if (data.ApprovalStatus == ApprovalStatus.Cancelled)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Already Cancelled",
                    "400",
                    "PO is already cancelled"
                );
            }


            if (data.ApprovalStatus == ApprovalStatus.Closed)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Closed Purchase Order Cannot Be Cancelled",
                    "400",
                    "Closed PO cannot be cancelled"
                );
            }


            data.ApprovalStatus = ApprovalStatus.Cancelled;

            data.ModifiedAt = DateTime.Now;


            await db.SaveChangesAsync();


            cache.Remove(PurchaseOrderCacheKey);


            return ApiResponseHelper.SuccessRes(
                mapper.Map<PurchaseOrderDTO>(data),
                "Purchase Order Cancelled Successfully"
            );
        }

        public async Task<ApiResponse<PurchaseOrderDTO>> ClosePurchaseOrder(int id)
        {
            var data = await db.PurchaseOrders
                               .FirstOrDefaultAsync(x => x.POId == id);


            if (data == null)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }


            if (data.ApprovalStatus != ApprovalStatus.Approved)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Cannot Be Closed",
                    "400",
                    "Only Approved Purchase Order can be closed"
                );
            }


            data.ApprovalStatus = ApprovalStatus.Closed;

            data.ModifiedAt = DateTime.Now;


            await db.SaveChangesAsync();


            cache.Remove(PurchaseOrderCacheKey);


            return ApiResponseHelper.SuccessRes(
                mapper.Map<PurchaseOrderDTO>(data),
                "Purchase Order Closed Successfully"
            );
        }


        public async Task<byte[]> GeneratePurchaseOrderPdf(int id)
        {

            var data = await db.PurchaseOrders
                    .Include(x => x.PurchaseOrderItems)
                    .FirstOrDefaultAsync(x => x.POId == id);


            if (data == null)
            {
                return null;
            }



            var pdf = QuestPDF.Fluent.Document.Create(container =>
            {

                container.Page(page =>
                {

                    page.Size(PageSizes.A4);

                    page.Margin(30);



                    page.Header()
                    .Text("FINCORE ERP - PURCHASE ORDER")
                    .FontSize(20)
                    .Bold();



                    page.Content()
                .Column(column =>
                {

                    column.Item()
                    .Text($"PO Number : {data.POCode}");


                    column.Item()
                    .Text($"Order Date : {data.OrderDate:dd-MM-yyyy}");


                    column.Item()
                    .Text($"Required Till Date : {data.RequiredTillDate:dd-MM-yyyy}");



                    column.Item()
                    .PaddingTop(20)
                    .Text("Purchase Order Items")
                    .Bold();



                    column.Item()
                    .Table(table =>
                    {

                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });



                        table.Header(header =>
                        {
                            header.Cell().Text("Item");
                            header.Cell().Text("Description");
                            header.Cell().Text("Qty");
                            header.Cell().Text("Price");
                            header.Cell().Text("Total");
                        });



                        foreach (var item in data.PurchaseOrderItems)
                        {

                            table.Cell()
                            .Text(item.ItemName);


                            table.Cell()
                            .Text(item.ItemDescription);


                            table.Cell()
                            .Text(item.Quantity.ToString());


                            table.Cell()
                            .Text(item.UnitPrice.ToString());


                            table.Cell()
                            .Text(item.LineTotal.ToString());

                        }


                    });



                    column.Item()
                    .PaddingTop(20)
                    .Text($"Grand Total : {data.Amount}")
                    .Bold();

                });



                    page.Footer()
                    .AlignCenter()
                    .Text("Generated By FINCORE ERP");


                });


            })
            .GeneratePdf();



            return pdf;

        }


        public async Task<ApiResponse<List<PurchaseOrderDTO>>> FilterPurchaseOrders(PurchaseOrderFilterDTO filter)
        {
            var query = db.PurchaseOrders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(x => x.ApprovalStatus == filter.Status);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(x => x.OrderDate >= filter.FromDate);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(x => x.OrderDate <= filter.ToDate);
            }

            if (filter.MinAmount.HasValue)
            {
                query = query.Where(x => x.Amount >= filter.MinAmount);
            }

            if (filter.MaxAmount.HasValue)
            {
                query = query.Where(x => x.Amount <= filter.MaxAmount);
            }

            if (filter.CreatedBy.HasValue)
            {
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);
            }

            if (filter.ApprovedBy.HasValue)
            {
                query = query.Where(x => x.ApprovedBy == filter.ApprovedBy);
            }

            if (!string.IsNullOrWhiteSpace(filter.POCode))
            {
                query = query.Where(x => x.POCode.Contains(filter.POCode));
            }

            // Total Records
            var totalRecords = await query.CountAsync();

            // Pagination
            var data = await query
                .OrderByDescending(x => x.POId)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = mapper.Map<List<PurchaseOrderDTO>>(data);

            return ApiResponseHelper.SuccessRes(
                result,
                "Filtered Purchase Orders Retrieved Successfully",
                totalRecords,
                new
                {
                    page = filter.Page,
                    pageSize = filter.PageSize,
                    totalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                    hasNextPage = filter.Page * filter.PageSize < totalRecords,
                    hasPreviousPage = filter.Page > 1
                });
        }

        public async Task<ApiResponse<List<PurchaseOrderDTO>>> GetAllPurchaseOrder(
    int page,
    int pageSize)
        {
            string cacheKey = $"{PurchaseOrderCacheKey}_{page}_{pageSize}";


            // Check Cache
            if (cache.TryGetValue(cacheKey, out List<PurchaseOrderDTO> purchaseOrders))
            {
                var totalRecords = await db.PurchaseOrders.CountAsync();

                return ApiResponseHelper.SuccessRes(
                    purchaseOrders,
                    "Purchase Orders Retrieved Successfully",
                    totalRecords,
                    new
                    {
                        page,
                        pageSize,
                        totalPages = (int)Math.Ceiling(
                            (double)totalRecords / pageSize),
                        hasNextPage = page * pageSize < totalRecords,
                        hasPreviousPage = page > 1
                    });
            }


            // Database Data
            var data = await db.PurchaseOrders
                .OrderByDescending(x => x.POId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();



            if (!data.Any())
            {
                return ApiResponseHelper.Failure<List<PurchaseOrderDTO>>(
                    "Purchase Orders Not Found",
                    "PO_NOT_FOUND",
                    "No Purchase Order records found");
            }



            purchaseOrders = mapper.Map<List<PurchaseOrderDTO>>(data);



            // Store Cache
            cache.Set(
                cacheKey,
                purchaseOrders,
                TimeSpan.FromMinutes(5)
            );



            // Total Count
            var totalRecord = await db.PurchaseOrders.CountAsync();



            return ApiResponseHelper.SuccessRes(
                purchaseOrders,
                "Purchase Orders Retrieved Successfully",
                totalRecord,
                new
                {
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(
                        (double)totalRecord / pageSize),
                    hasNextPage = page * pageSize < totalRecord,
                    hasPreviousPage = page > 1
                });
        }
    }
}