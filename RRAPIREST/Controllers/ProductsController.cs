using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RRAPIREST.Repo;
using RRAPIREST.Model;

namespace RRAPIREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepo productsRepo;

        public ProductsController(IProductsRepo productsRepo)
        {
            this.productsRepo = productsRepo;
        }

        [HttpPost("CreateDatabase")]
        public async Task<IActionResult> CreateDatabase()
        {
            var _result = await this.productsRepo.CreateDatabase("");
            if (_result != "")
            {
                return Ok(_result);
            }
            else
            {
                return BadRequest("Not created");
            }
        }

        [HttpGet("GetBySKU/{sku}")]
        public async Task<IActionResult> GetBySKU(string sku)
        {
            var product = await this.productsRepo.GetBySKU(sku);
            if (product != null)
            {
                return Ok(product);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
