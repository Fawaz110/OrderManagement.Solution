using Core.Entities;
using Core.Repositories.Contract;
using Core.Specifications.InvoiceSpecifications;
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
            var spec = new InvoiceWithOrderSpecifications(invoiceId);

            var invoice = await _invoiceRepository.GetWithSpecAsync(spec);

            if (invoice is null)
                return NotFound(new ApiResponse(404));

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
        {
            var spec = new InvoiceWithOrderSpecifications();

            var invoices = await _invoiceRepository.GetAllWithSpecAsync(spec);

            if (!invoices.Any())
                return NotFound(new ApiResponse(404));

            return Ok(invoices);
        }
    }
}
