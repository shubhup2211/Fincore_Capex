using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services.MasterTable
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public CustomerService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<CustomerDto>>> GetAllCustomersAsync(
            int pageNumber,
            int pageSize)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 10;

                var totalRecords = await db.Customers.CountAsync();

                var customers = await db.Customers
                    .Include(x => x.User)
                    .Include(x => x.Company)
                    .OrderBy(x => x.CustomerId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var customerDtos =
                    mapper.Map<List<CustomerDto>>(customers);

                var metadata = new
                {
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(
                        totalRecords / (double)pageSize)
                };

                return ApiResponseHelper.SuccessRes(
                    customerDtos,
                    "Customers retrieved successfully.",
                    totalRecords,
                    metadata);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<List<CustomerDto>>(
                    "Failed to retrieve customers.",
                    "CUSTOMER_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(
            int id)
        {
            try
            {
                var customer = await db.Customers
                    .Include(x => x.User)
                    .Include(x => x.Company)
                    .FirstOrDefaultAsync(x => x.CustomerId == id);

                if (customer == null)
                {
                    return ApiResponseHelper.Failure<CustomerDto>(
                        "Customer not found.",
                        "CUSTOMER_NOT_FOUND",
                        $"Customer with ID {id} does not exist.");
                }

                var customerDto =
                    mapper.Map<CustomerDto>(customer);

                return ApiResponseHelper.SuccessRes(
                    customerDto,
                    "Customer retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<CustomerDto>(
                    "Failed to retrieve customer.",
                    "CUSTOMER_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<CustomerDto>> CreateCustomerAsync(
            CreateCustomerDto createCustomerDto)
        {
            try
            {
                var userExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId == createCustomerDto.UserId);

                if (!userExists)
                {
                    return ApiResponseHelper.Failure<CustomerDto>(
                        "User not found.",
                        "USER_NOT_FOUND",
                        $"User with ID {createCustomerDto.UserId} does not exist.");
                }

                var companyExists = await db.Companies
                    .AnyAsync(x =>
                        x.CompanyId == createCustomerDto.CompanyId);

                if (!companyExists)
                {
                    return ApiResponseHelper.Failure<CustomerDto>(
                        "Company not found.",
                        "COMPANY_NOT_FOUND",
                        $"Company with ID {createCustomerDto.CompanyId} does not exist.");
                }

                var customerCodeExists = await db.Customers
                    .AnyAsync(x =>
                        x.CustomerCode == createCustomerDto.CustomerCode);

                if (customerCodeExists)
                {
                    return ApiResponseHelper.Failure<CustomerDto>(
                        "Customer code already exists.",
                        "DUPLICATE_CUSTOMER_CODE",
                        $"Customer with code {createCustomerDto.CustomerCode} already exists.");
                }


                var customer =
                    mapper.Map<Customer>(createCustomerDto);

                customer.CustomerId = 0;

                await db.Customers.AddAsync(customer);
                await db.SaveChangesAsync();


                var createdCustomer = await db.Customers
                    .Include(x => x.User)
                    .Include(x => x.Company)
                    .FirstOrDefaultAsync(
                        x => x.CustomerId == customer.CustomerId);

                var result =
                    mapper.Map<CustomerDto>(createdCustomer);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Customer created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<CustomerDto>(
                    "Failed to create customer.",
                    "CUSTOMER_CREATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(
            int id,
            UpdateCustomerDto updateCustomerDto)
        {
            try
            {
                var customer = await db.Customers
                    .FirstOrDefaultAsync(
                        x => x.CustomerId == id);

                if (customer == null)
                {
                    return ApiResponseHelper.Failure<CustomerDto>(
                        "Customer not found.",
                        "CUSTOMER_NOT_FOUND",
                        $"Customer with ID {id} does not exist.");
                }

                var userExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId == updateCustomerDto.UserId);

                if (!userExists)
                {
                    return ApiResponseHelper.Failure<CustomerDto>(
                        "User not found.",
                        "USER_NOT_FOUND",
                        $"User with ID {updateCustomerDto.UserId} does not exist.");
                }

                var companyExists = await db.Companies
                    .AnyAsync(x =>
                        x.CompanyId == updateCustomerDto.CompanyId);

                if (!companyExists)
                {
                    return ApiResponseHelper.Failure<CustomerDto>(
                        "Company not found.",
                        "COMPANY_NOT_FOUND",
                        $"Company with ID {updateCustomerDto.CompanyId} does not exist.");
                }

                var customerCodeExists = await db.Customers
                    .AnyAsync(x =>
                        x.CustomerCode == updateCustomerDto.CustomerCode &&
                        x.CustomerId != id);

                if (customerCodeExists)
                {
                    return ApiResponseHelper.Failure<CustomerDto>(
                        "Customer code already exists.",
                        "DUPLICATE_CUSTOMER_CODE",
                        $"Customer with code {updateCustomerDto.CustomerCode} already exists.");
                }


                customer.CustomerCode =
                    updateCustomerDto.CustomerCode;

                customer.UserId =
                    updateCustomerDto.UserId;

                customer.CompanyId =
                    updateCustomerDto.CompanyId;

                customer.IsActive =
                    updateCustomerDto.IsActive;


                await db.SaveChangesAsync();


                var updatedCustomer = await db.Customers
                    .Include(x => x.User)
                    .Include(x => x.Company)
                    .FirstOrDefaultAsync(
                        x => x.CustomerId == id);

                var result =
                    mapper.Map<CustomerDto>(updatedCustomer);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Customer updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<CustomerDto>(
                    "Failed to update customer.",
                    "CUSTOMER_UPDATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await db.Customers
                    .FirstOrDefaultAsync(
                        x => x.CustomerId == id);

                if (customer == null)
                {
                    return ApiResponseHelper.Failure<bool>(
                        "Customer not found.",
                        "CUSTOMER_NOT_FOUND",
                        $"Customer with ID {id} does not exist.");
                }

                db.Customers.Remove(customer);

                await db.SaveChangesAsync();

                return ApiResponseHelper.SuccessRes(
                    true,
                    "Customer deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Failed to delete customer.",
                    "CUSTOMER_DELETE_ERROR",
                    ex.Message);
            }
        }
    }
}