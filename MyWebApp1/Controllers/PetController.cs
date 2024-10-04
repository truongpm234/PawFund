using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApp1.Models;
using MyWebApp1.Services;

namespace MyWebApp1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        // Khai báo biến _petService
        private readonly PetService _petService;

        // Khởi tạo _petService thông qua constructor
        public PetsController(PetService petService)
        {
            _petService = petService;
        }

        [HttpPost]
        [Route("AddNewPet")]
        public IActionResult AddNewPet([FromBody] PetDTO petDTO)
        {
            try
            {
                var newPet = _petService.AddNewPet(petDTO);
                return Ok(newPet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
