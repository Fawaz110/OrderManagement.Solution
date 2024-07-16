using Core.Entities;
using Core.Repositories.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Errors;

namespace OrderManagement.Controllers
{
    public class InvoicesController : BaseApiController
    {
        private readonly IGenericRepository<Invoice> _invoiceRepository;

        public InvoicesController(
            IGenericRepository<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [HttpGet("{invoiceId}")]
        public async Task<ActionResult<Invoice>> GetById(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

            if (invoice is null)
                return NotFound(new ApiResponse(404));

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
        {
            var invoices = await _invoiceRepository.GetAllAsync();

            if (!invoices.Any())
                return NotFound(new ApiResponse(404));

            return Ok(invoices);
        }
    }
}
