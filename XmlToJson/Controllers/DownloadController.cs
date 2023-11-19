using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlToJson.Interactors.Interfaces;

namespace XmlToJson.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IDownload _downloader;
        private readonly IConfiguration _configuration;
        public DownloadController(IDownload downloader,
                                  IConfiguration configuration)
        {
            _downloader = downloader;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string fileName)
        {
            string directory = _configuration["AppSettings:OutputDirectory"];
            byte[] data = _downloader.GetBytes(directory, fileName);

            Response.Headers.Add("Content-Disposition", String.Format("attachment; filename={0}", fileName));
            Response.Headers.Add("Content-Type", "text/plain"); // Specify the content type (text/plain for a text file)

            return File(data, "text/plain");
        }
    }
}
