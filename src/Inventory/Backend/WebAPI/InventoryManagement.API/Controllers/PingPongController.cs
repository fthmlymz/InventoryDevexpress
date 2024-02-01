using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingPongController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok($"Inventory management web api is working! {DateTime.UtcNow}");
        }
    }
}
