using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using XmlToJson.Exeptions;
using XmlToJson.Interactors.Interfaces;
using XmlToJson.Models.Enums;

namespace XmlToJson.Controllers
{
    public class ConvertController : Controller
    {
        private readonly IConverter _xmlToJson;
        private readonly IFileWriter _fileWriter;
        private readonly IConfiguration _configuration;
        public ConvertController(IConverter xmlToJson,
                                IConfiguration configuration,
                                IFileWriter fileWriter)
        {
            _xmlToJson = xmlToJson;
            _configuration = configuration;
            _fileWriter = fileWriter;
        }

        [HttpPost]
        public async Task<IActionResult> Convert([FromForm] IFormFile xmlFile, [FromForm] string filename)
        {
            if (xmlFile == null || xmlFile.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded." });
            }

            if (string.IsNullOrWhiteSpace(filename) || !Regex.IsMatch(filename, "^[a-zA-Z0-9-]+$"))
            {
                return BadRequest(new { error = "Filename must contain only letters and digits." });
            }

            try
            {
                string outputDirectory = _configuration["AppSettings:OutputDirectory"];
                string fullFileName = $"{filename}.{FileSuffixEnum.Json.ToString().ToLower()}";
                

                using (var reader = new StreamReader(xmlFile.OpenReadStream()))
                {
                    var xmlContent = await reader.ReadToEndAsync();

                    await _xmlToJson.ValidateInputAsync(xmlContent);

                    var jsonContent = await _xmlToJson.ConvertAsync(xmlContent);
                    await _fileWriter.WriteToFileAsync(outputDirectory, fullFileName, jsonContent);

                    return Json(new { message = "Conversion successful." });
                }
            }
            catch (JsonFileWriteException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ConvertXmlToJsonException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (XmlException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { error = "An unexpected error occurred." });
            }
        }
    }
}
