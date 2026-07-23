using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.IMasterTable
{
    public interface IMasterType
    {
        Task<ApiResponse<List<MasterTypeDto>>> GetAllMasterType(int page, int pageSize);
        Task<ApiResponse<MasterTypeDto>> GetByIdMasterType(int id);
        Task<ApiResponse<MasterTypeDto>> AddMasterType(CreateMasterTypeDto dto);
        Task<ApiResponse<MasterTypeDto>> UpdateMasterType(int id, UpdateMasterTypeDto dto);
        Task<ApiResponse<bool>> DeleteMasterType(int id);
    }
}
