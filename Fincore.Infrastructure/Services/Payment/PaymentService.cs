using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Payment;
using Fincore.Application.Interfaces.IPayment;
using Fincore.Domain.Enums;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace Fincore.Infrastructure.Services.PaymentModule
{
    public class PaymentService : IPaymentService
    {
        IMemoryCache cache;
        AppDbContext db;
        IMapper mapper;

        public PaymentService(AppDbContext db, IMapper mapper, IMemoryCache cache)
        {
            this.cache = cache;
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddPaymentAsync(PaymentPostDTO dto)
        {
            
            var lastPayment = await db.Payments
                .OrderByDescending(x => x.PaymentId)
                .FirstOrDefaultAsync();

            int nextId = (lastPayment?.PaymentId ?? 0) + 1;

            string paymentNumber = $"PAY-{DateTime.Now.Year}-{nextId:D5}";

            var payment = mapper.Map<Payment>(dto);

            payment.PaymentNumber = paymentNumber;
            payment.ApprovalStatus = "Pending";
            payment.ReconciledFlag = false;
            payment.CreatedAt = DateTime.Now;
            payment.ModifiedAt = DateTime.Now;

            if (dto.PaymentType.ToString() == "AP")
            {
                var invoice = await db.APInvoices
                    .FirstOrDefaultAsync(x => x.APInvoiceId == dto.APInvoiceId);

                if (invoice == null)
                    throw new Exception("AP Invoice not found.");

                if(invoice.ApprovalStatus!="Approved")
                {
                    throw new Exception("Payment can only be made for approved AP invoices.");
                }

                payment.VendorId = invoice.VendorId;
                payment.Amount = invoice.Amount;
                payment.APInvoiceId = invoice.APInvoiceId;
                payment.ARInvoiceId = null;
            }
            else
            {
                var invoice = await db.ARInvoices
                    .FirstOrDefaultAsync(x => x.ARInvoiceId == dto.ARInvoiceId);

                if (invoice == null)
                    throw new Exception("AR Invoice not found.");

               
                payment.Amount = invoice.Amount;
                payment.CustomerId = invoice.CustomerId;
                payment.ARInvoiceId = invoice.ARInvoiceId;
                payment.APInvoiceId = null;
            }

            payment.PaymentType = dto.PaymentType.ToString();

            await db.Payments.AddAsync(payment);
            await db.SaveChangesAsync();

            cache.Remove("Payments");
        }

        public async Task DeletePaymentAsync(int id)
        {
            var data = await db.Payments.Where(x=>x.PaymentId==id).FirstOrDefaultAsync();
            if (data == null)
                throw new Exception("Payment not found.");

            
            if (data.ApprovalStatus=="Approved")
            {
                throw new Exception("Cannot delete Approved Payment record");
            }
            else
            {
                db.Payments.Remove(data);
                await db.SaveChangesAsync();
                cache.Remove("Payments");
            }
            


        }

        public async Task<ApiResponse<List<PaymentGetDTO>>> GetAllPayment(int page, int pageSize)
        {
            string cacheKey = $"Payments_{page}_{pageSize}";
            if (cache.TryGetValue(cacheKey, out List<PaymentGetDTO> payments))
            {
               return ApiResponseHelper.SuccessRes(payments, "Payments Fetch Successfully!", await db.Payments.CountAsync());
            }
            var data = await db.Payments.OrderBy(x => x.PaymentId).Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
            var result = mapper.Map<List<PaymentGetDTO>>(data);

            cache.Set(
    cacheKey,
    result,
    new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    });


            return ApiResponseHelper.SuccessRes(result, "Payments Fetch Successfully!", await db.Payments.CountAsync());
        }

        public async Task<ApiResponse<PaymentGetDTO>> GetPaymentById(int id)
        {
            string cacheKey = $"Payment_{id}";
            if (cache.TryGetValue(cacheKey, out PaymentGetDTO payments))
            {
                return ApiResponseHelper.SuccessRes(payments, "Payments Fetch Successfully!", await db.Payments.CountAsync());
            }
            var data = await db.Payments.Where(x=>x.PaymentId == id).FirstOrDefaultAsync();
            if (data == null)
                throw new Exception("Payment not found.");
            var result = mapper.Map<PaymentGetDTO>(data);
            return ApiResponseHelper.SuccessRes(result, $"Payment Data Fetched of id {id}", await db.Payments.Where(x => x.PaymentId == id).CountAsync());

        }

        public async Task<ApiResponse<List<PaymentGetDTO>>> GetPaymentStatus(PaymentStatus ps, int page, int pageSize)
        {
            string payStatus = ps.ToString();
            string cacheKey = $"Payment_Status_{payStatus}_{page}_{pageSize}";

            if (cache.TryGetValue(cacheKey, out List<PaymentGetDTO> payments))
            {
                return ApiResponseHelper.SuccessRes(
                    payments,
                    $"Payments of type {payStatus} fetched successfully!",
                    await db.Payments.CountAsync(x => x.ApprovalStatus == payStatus)
                );
            }

            var data = await db.Payments
                .Where(x => x.ApprovalStatus == payStatus)
                .OrderBy(x => x.PaymentId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = mapper.Map<List<PaymentGetDTO>>(data);

            cache.Set(
                cacheKey,
                result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return ApiResponseHelper.SuccessRes(
                result,
                $"Payments of type {payStatus} fetched successfully!",
                await db.Payments.CountAsync(x => x.ApprovalStatus == payStatus)
            );
        }

        public async Task<ApiResponse<List<PaymentGetDTO>>> GetPaymentType(PaymentType pt, int page, int pageSize)
        {
            string payType = pt.ToString();
            string cacheKey = $"Payment_{pt}_{page}_{pageSize}";

            if (cache.TryGetValue(cacheKey, out List<PaymentGetDTO> payments))
            {
                return ApiResponseHelper.SuccessRes(
                    payments,
                    $"Payments of type {payType} fetched successfully!",
                    await db.Payments.CountAsync(x => x.PaymentType == payType)
                );
            }

            var data = await db.Payments
                .Where(x => x.PaymentType == payType)
                .OrderBy(x => x.PaymentId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = mapper.Map<List<PaymentGetDTO>>(data);

            cache.Set(
                cacheKey,
                result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return ApiResponseHelper.SuccessRes(
                result,
                $"Payments of type {payType} fetched successfully!",
                await db.Payments.CountAsync(x => x.PaymentType == payType)
            );
        }

        public async Task UpdateApproval(int id)
        {

            var data = await db.Payments.Where(x => x.PaymentId == id).FirstOrDefaultAsync();
            if (data != null)
            {
                if (data.ApprovalStatus == "Approved")
                    throw new Exception("Payment is already approved.");
                data.ApprovedBy = 1;
                data.ApprovalStatus = "Approved";
                //change by using jwt
            }
            else
            {
                throw new Exception("RECORD NOT FOUND");
            }
            
            await db.SaveChangesAsync();

        }

        public async Task UpdatePaymentAsync(int id, PaymentUpdateDTO dto)
        {
            var payment = await db.Payments
                .FirstOrDefaultAsync(x => x.PaymentId == id);

            if (payment == null)
            {
                throw new Exception("Payment not found.");
            }

            if (payment.ApprovalStatus == "Approved")
            {
                throw new Exception("Approved payments cannot be updated.");
            }

            payment.PaymentMethod = dto.PaymentMethod;
            payment.PaymentDate = dto.PaymentDate;
            payment.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            cache.Remove("Payments");
        }

        public async Task UpdateReconcile(int id)
        {
            var data = await db.Payments
                .FirstOrDefaultAsync(x => x.PaymentId == id);

            if (data == null)
            {
                throw new Exception("Payment not found.");
            }

            if (data.ApprovalStatus != "Approved")
            {
                throw new Exception("Payment must be approved before reconciliation.");
            }

            if ((bool)data.ReconciledFlag)
            {
                throw new Exception("Payment is already reconciled.");
            }

            data.ReconciledFlag = true;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            cache.Remove("Payments");
        }
    }
}

