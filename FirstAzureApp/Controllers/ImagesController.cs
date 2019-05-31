using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstAzureApp.Models;
using FirstAzureApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstAzureApp.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageStore imageStore;

        public ImagesController(IImageStore imageStore)
        {
            this.imageStore = imageStore;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile image)
        {
            
            if(image != null)
            {
                using (var stream = image.OpenReadStream())
                {
                    var imageId = await this.imageStore.SaveImage(stream);
                    return RedirectToAction("show", new { imageId });
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Show(string imageId)
        {
            var model = new ShowModel { Uri = this.imageStore.UriFor(imageId) };
            return View(model);
        }
    }
}