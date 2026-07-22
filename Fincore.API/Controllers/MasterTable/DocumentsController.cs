using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.MasterTable
{
    [Route("api/v1/[Action]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class DocumentsController : ControllerBase
    {
        IDocumentService repo;

        public DocumentsController(IDocumentService repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> FetchDocuments(int page=1,int pageSize=10)
        {
            var data = await repo.GetAll(page,pageSize);


            return Ok(new{status = true,message = "Documents Fetch Successfully",Data = data});
        }

        [HttpGet]
        public async Task<IActionResult> DocumentGetById(int id)
        {
            var data = await repo.DocumentGetById(id);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(new{status = true,message = "Document Fetch Successfully",Data = data});
        }


        [HttpPost]
        public async Task<IActionResult> AddDocument(CreateDocumentDto dto)
        {
            var data = await repo.AddDocument(dto);

            return Ok(new
            {
                status = true,
                message = "Document Added Successfully",
                Data = data
            });
        }

        [HttpPut]

        public async Task<IActionResult> UpadateDocument(int id, UpdateDocumentDto dto) 
        {
            var data = await repo.UpdateDocument(id,dto);
            return Ok(new { status = true, message = "Document Update Successfully", Data = data });

        }


        [HttpDelete]

        public async Task<IActionResult> DeleteDocument(int id) 
        {
            var data = await repo.DeleteDocument(id);
            if (!data) 
            {
            return NotFound(new { status = false, message = "Document Not Found" });
            
            }
            return Ok(new { status = true, message = "Document Delete Successfully" });
        }
    
    
    }


}