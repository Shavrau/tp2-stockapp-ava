using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;

        public SuppliersController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        [HttpGet]
        [Authorize(Policy = "userPolicy")] 
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAll()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "userPolicy")] 
        public async Task<ActionResult<Supplier>> GetById(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpPost]
        [Authorize(Policy = "adminPolicy")] 
        public async Task<ActionResult<Supplier>> Create([FromBody] Supplier supplier)
        {
            if (supplier == null)
            {
                return BadRequest();
            }

            await _supplierRepository.AddAsync(supplier);
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "adminPolicy")]
        public async Task<IActionResult> Update(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return BadRequest();
            }

            var existingSupplier = await _supplierRepository.GetByIdAsync(id);
            if (existingSupplier == null)
            {
                return NotFound();
            }

            await _supplierRepository.UpdateAsync(supplier);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "adminPolicy")] 
        public async Task<IActionResult> Delete(int id)
        {
            var existingSupplier = await _supplierRepository.GetByIdAsync(id);
            if (existingSupplier == null)
            {
                return NotFound();
            }

            await _supplierRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        [Authorize(Policy = "userPolicy")]
        public async Task<ActionResult<IEnumerable<Supplier>>> SearchSuppliers([FromQuery] string name, [FromQuery] string contactEmail)
        {
            var suppliers = await _supplierRepository.SearchAsync(name, contactEmail);
            return Ok(suppliers);
        }
    }
}
