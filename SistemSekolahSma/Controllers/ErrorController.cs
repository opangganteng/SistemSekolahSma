using Microsoft.AspNetCore.Mvc;
using SistemSekolahSMA.ViewModels;
using System.Diagnostics;

namespace SistemSekolahSMA.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [Route("/Error/404")]
        public IActionResult PageNotFound()
        {
            return View("NotFound");
        }

        [Route("/Error/500")]
        public IActionResult InternalServerError()
        {
            return View("InternalServerError");
        }
    }
}