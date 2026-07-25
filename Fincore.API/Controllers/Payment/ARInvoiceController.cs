using AutoMapper;
using Fincore.Application.DTO.Payment;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class ARInvoiceController : ControllerBase
    {


        private readonly AppDbContext dbContext;

        private readonly IMapper mapper;

        public ARInvoiceController(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetAll_AR_Invoice()
        {
            var AllAR_Invoice = dbContext.ARInvoices.ToList();

            return Ok(AllAR_Invoice);
        }




        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetARInvoiceById(int id)
        {
            var rev = dbContext.ARInvoices.Find(id);

            if (rev is null)
            {
                return NotFound();
            }

            return Ok(rev);
        }



        [HttpPost]
        public IActionResult AddARInvoice(ARInvoiceDTO addARInvoicesDTO)
        {
          
            var res = mapper.Map<ARInvoice>(addARInvoicesDTO);

            dbContext.ARInvoices.Add(res);
            dbContext.SaveChanges();
            return Ok(res);
        }


        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UptARInvoice(int id, ARInvoiceDTO uptAR)
        {
            var rev = dbContext.ARInvoices.Find(id);

            if (rev == null)
            {
                return NotFound();
            }

            mapper.Map(uptAR, rev);  

            dbContext.SaveChanges();

            return Ok(rev);
        }



        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteARInvoice(int id)
        {
            var rev = dbContext.ARInvoices.Find(id);

            if (rev is null)
            {
                return NotFound();
            }

            dbContext.ARInvoices.Remove(rev);
            dbContext.SaveChanges();

            return Ok();
        }













    }
}
