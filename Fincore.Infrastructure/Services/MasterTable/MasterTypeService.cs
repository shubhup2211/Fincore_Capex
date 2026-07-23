using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class MasterTypeService : IMasterType
{
     AppDbContext db;
     IMapper mapper;

    public MasterTypeService(AppDbContext db, IMapper mapper)
    {
        this.db = db;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<MasterTypeDto>>> GetAllMasterType(int page, int pageSize)
    {
        var totalRecords = await db.MasterTypes.CountAsync();

        var data = await db.MasterTypes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (!data.Any())
        {
            return ApiResponseHelper.Failure<List<MasterTypeDto>>
            (
                "Master Type Not Found",
                "404",
                "No Data Found"
            );
        }

        var res = mapper.Map<List<MasterTypeDto>>(data);

        return ApiResponseHelper.SuccessRes
        (
            res,
            "Master Type Fetch Successfully",
            totalRecords,
            new
            {
                page,
                pageSize
            }
        );
    }

    public async Task<ApiResponse<MasterTypeDto>> GetByIdMasterType(int id)
    {
        var data = await db.MasterTypes
            .FirstOrDefaultAsync(x => x.MasterTypeId == id);

        if (data == null)
        {
            return ApiResponseHelper.Failure<MasterTypeDto>
            (
                "Master Type Not Found",
                "404",
                "Invalid Master Type Id"
            );
        }

        var res = mapper.Map<MasterTypeDto>(data);

        return ApiResponseHelper.SuccessRes
        (
            res,
            "Master Type Fetch Successfully"
        );
    }

    public async Task<ApiResponse<MasterTypeDto>> AddMasterType(CreateMasterTypeDto dto)
    {
        var data = mapper.Map<MasterType>(dto);

        db.MasterTypes.Add(data);

        var result = await db.SaveChangesAsync();

        if (result <= 0)
        {
            return ApiResponseHelper.Failure<MasterTypeDto>
            (
                "Master Type Not Added",
                "500",
                "Failed to Add Master Type"
            );
        }

        var res = mapper.Map<MasterTypeDto>(data);

        return ApiResponseHelper.SuccessRes
        (
            res,
            "Master Type Added Successfully"
        );
    }

    public async Task<ApiResponse<MasterTypeDto>> UpdateMasterType(int id, UpdateMasterTypeDto dto)
    {
        var data = await db.MasterTypes
            .FirstOrDefaultAsync(x => x.MasterTypeId == id);

        if (data == null)
        {
            return ApiResponseHelper.Failure<MasterTypeDto>
            (
                "Master Type Not Found",
                "404",
                "Invalid Master Type Id"
            );
        }

        mapper.Map(dto, data);

        var result = await db.SaveChangesAsync();

        if (result <= 0)
        {
            return ApiResponseHelper.Failure<MasterTypeDto>
            (
                "Master Type Not Updated",
                "500",
                "Failed to Update Master Type"
            );
        }

        var res = mapper.Map<MasterTypeDto>(data);

        return ApiResponseHelper.SuccessRes
        (
            res,
            "Master Type Updated Successfully"
        );
    }

    public async Task<ApiResponse<bool>> DeleteMasterType(int id)
    {
        var data = await db.MasterTypes
            .FirstOrDefaultAsync(x => x.MasterTypeId == id);

        if (data == null)
        {
            return ApiResponseHelper.Failure<bool>
            (
                "Master Type Not Found",
                "404",
                "Invalid Master Type Id"
            );
        }

        db.MasterTypes.Remove(data);

        var result = await db.SaveChangesAsync();

        if (result <= 0)
        {
            return ApiResponseHelper.Failure<bool>
            (
                "Master Type Not Deleted",
                "500",
                "Failed to Delete Master Type"
            );
        }

        return ApiResponseHelper.SuccessRes
        (
            true,
            "Master Type Deleted Successfully"
        );
    }
}