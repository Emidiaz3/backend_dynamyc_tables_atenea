using ApiRestCuestionario.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : ControllerBase
    {
        private readonly AppDbContext context;

        public TemplateController(AppDbContext context) {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetTemplateItems()
        {
            var response = await context.Template.ToListAsync();
            return Ok(response);

          
        }
    }
}
