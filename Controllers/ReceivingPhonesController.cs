using angular.Server.Model;
using ApiRestCuestionario.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Agrega esta línea

namespace angular.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceivingPhonesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReceivingPhonesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReceivingPhones()
        {
            var phones = await _context.ReceivingPhone.ToListAsync();
            return Ok(phones);
        }

        [HttpPost]
        public async Task<IActionResult> AddReceivingPhone([FromBody] ReceivingPhone phone)
        {
            _context.ReceivingPhone.Add(phone);
            await _context.SaveChangesAsync();
            return Ok(phone);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceivingPhoneStatus(int id, [FromBody] bool isActive)
        {
            var phone = await _context.ReceivingPhone.FindAsync(id);
            if (phone == null)
            {
                return NotFound();
            }
            phone.IsActive = isActive;
            await _context.SaveChangesAsync();
            return Ok(phone);
        }
    }

}
