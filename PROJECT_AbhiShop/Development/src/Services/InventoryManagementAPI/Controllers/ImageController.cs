using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Image")]
    public class ImageController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ImageController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetMyImage(int id)
        {
           var root = _env.WebRootPath;
           var imagePath = Path.Combine(root+"/Images/"+id+".jpeg");
           var convertToBytes = System.IO.File.ReadAllBytes(imagePath);
           return File(convertToBytes,"image/jpeg");
        }
    }
}
