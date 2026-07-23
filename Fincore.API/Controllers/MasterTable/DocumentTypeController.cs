using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.MasterTable
{
    [Route("api/v2/[Action]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]

    public class DocumentTypeController : ControllerBase
    {
        IDocumentTypeService repo;
        public DocumentTypeController(IDocumentTypeService repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocumentType(int page = 1, int pageSize = 10)
        {
            var data = await repo.GetAllDocumentType(page, pageSize);
            return Ok(data);
        }


        [HttpGet]
        public async Task<IActionResult> GetByIdDocumentType(int id) 
        {
            var data=await repo.GetByIdDocumentType(id);
            if (data == null) 
            {
            return NotFound();
            }
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> AddDocumentType(CreateDocumentTypeDto dto) 
        {
            var data=await repo.AddDocumentType(dto);
            return Ok(data);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateDocumentType(int id, UpdateDocumentTypeDto dto)
        {
            var data = await repo.UpdateDocumentType(id, dto);
            return Ok(data);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            var data = await repo.DeleteDocumentType(id);

            if (data==null)
            {
                return NotFound(data);
            }

            return Ok(data);
        }
    }
}
