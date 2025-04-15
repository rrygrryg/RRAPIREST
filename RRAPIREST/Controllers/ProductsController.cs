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
                return BadRequest(_result);
            }
        }

        [HttpGet("GetBySKU/{sku}")]
        public async Task<IActionResult> GetBySKU(string sku)
        {
            var _listproduct = await this.productsRepo.getBySKU(sku);
            if (_listproduct != null)
            {
                return Ok(_listproduct);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
