using Core.Entities;
using Core.Repositories.Contract;
using Core.Specifications.InvoiceSpecifications;
using Core.UnitsOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Errors;

namespace OrderManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InvoicesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{invoiceId}")]
        public async Task<ActionResult<Invoice>> GetById(int invoiceId)
        {
            var spec = new InvoiceWithOrderSpecifications(invoiceId);

            var invoice = await _unitOfWork.Repository<Invoice>().GetWithSpecAsync(spec);

            if (invoice is null)
                return NotFound(new ApiResponse(404));

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
        {
            var spec = new InvoiceWithOrderSpecifications();

            var invoices = await _unitOfWork.Repository<Invoice>().GetAllWithSpecAsync(spec);

            if (!invoices.Any())
                return NotFound(new ApiResponse(404));

            return Ok(invoices);
        }
    }
}
