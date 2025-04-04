using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    // https://localhost:portnumber/api/students
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // GET: /api/students
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult GetStudents()
        {
            return Ok("All students");
        }
    }
}
