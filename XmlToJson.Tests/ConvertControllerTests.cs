using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlToJson.Controllers;
using XmlToJson.Exeptions;
using XmlToJson.Interactors.Interfaces;
using Xunit;

namespace XmlToJson.Tests
{
    public class ConvertControllerTests
    {
        [Fact]
        public async Task Convert_NoFileUploaded_ReturnsBadRequest()
        {
            var mockConverter = new Mock<IConverter>();
            var mockFileWriter = new Mock<IFileWriter>();
            var mockConfiguration = new Mock<IConfiguration>();

            var controller = new ConvertController(mockConverter.Object,
                                                   mockConfiguration.Object,
                                                   mockFileWriter.Object);

            var result = await controller.Convert(null, null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Convert_InvalidInput_ReturnsError()
        {
            var mockConverter = new Mock<IConverter>();
            var mockFileWriter = new Mock<IFileWriter>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConverter.Setup(converter => converter.ValidateInputAsync(It.IsAny<string>())).Verifiable();
            mockConverter.Setup(converter => converter.ConvertAsync(It.IsAny<string>())).ReturnsAsync("Valid JSON Data");
            mockFileWriter.Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var controller = new ConvertController(mockConverter.Object, mockConfiguration.Object, mockFileWriter.Object);

            var response = await controller.Convert(Mock.Of<IFormFile>(), "validFilename") as BadRequestObjectResult;
            var message = response.Value.GetType().GetProperty("error").GetValue(response.Value);

            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("No file uploaded.", message);
        }

        [Fact]
        public async Task Convert_ValidInput_ReturnsSuccess()
        {
            var mockConverter = new Mock<IConverter>();
            var mockFileWriter = new Mock<IFileWriter>();
            var mockConfiguration = new Mock<IConfiguration>();
            var xmlContent = "<root><data>Sample XML Content</data></root>";

            var file = new Mock<IFormFile>();
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            file.Setup(f => f.OpenReadStream()).Returns(fileStream);
            file.Setup(f => f.Length).Returns(fileStream.Length);

            mockConverter.Setup(converter => converter.ValidateInputAsync(xmlContent)).Verifiable();
            mockConverter.Setup(converter => converter.ConvertAsync(xmlContent)).ReturnsAsync("Valid JSON Data");
            mockFileWriter.Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var controller = new ConvertController(mockConverter.Object, mockConfiguration.Object, mockFileWriter.Object);

            var result = await controller.Convert(file.Object, "validFilename") as JsonResult;
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value);

            Assert.Equal("Conversion successful.", message);
        }
        [Fact]
        public async Task Convert_InvalidFilename_ReturnsErrorMessage()
        {
            var mockConverter = new Mock<IConverter>();
            var mockFileWriter = new Mock<IFileWriter>();
            var mockConfiguration = new Mock<IConfiguration>();
            var xmlContent = "<root><data>Sample XML Content</data></root>";

            var file = new Mock<IFormFile>();
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            file.Setup(f => f.OpenReadStream()).Returns(fileStream);
            file.Setup(f => f.Length).Returns(fileStream.Length);

            mockConverter.Setup(converter => converter.ValidateInputAsync(xmlContent)).Verifiable();
            mockConverter.Setup(converter => converter.ConvertAsync(xmlContent)).ReturnsAsync("Valid JSON Data");
            mockFileWriter.Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var invalidFilename = "Invalid$Filename"; // Invalid filename with special character(s)
            var controller = new ConvertController(mockConverter.Object, mockConfiguration.Object, mockFileWriter.Object);

            var result = await controller.Convert(file.Object, invalidFilename) as BadRequestObjectResult;
            var error = result.Value.GetType().GetProperty("error")?.GetValue(result.Value);

            Assert.Equal("Filename must contain only letters and digits.", error);
        }

        [Fact]
        public async Task Convert_JsonFileWriteException_ReturnsError()
        {
            var mockConverter = new Mock<IConverter>();
            var mockFileWriter = new Mock<IFileWriter>();
            var mockConfiguration = new Mock<IConfiguration>();
            var xmlContent = "<root><data>Sample XML Content</data></root>";

            var file = new Mock<IFormFile>();
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            file.Setup(f => f.OpenReadStream()).Returns(fileStream);
            file.Setup(f => f.Length).Returns(fileStream.Length);

            mockConverter.Setup(converter => converter.ValidateInputAsync(It.IsAny<string>())).Verifiable();
            mockConverter.Setup(converter => converter.ConvertAsync(It.IsAny<string>())).ReturnsAsync("Valid JSON Data");
            mockFileWriter.Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new JsonFileWriteException("Error writing JSON file"));

            var controller = new ConvertController(mockConverter.Object, mockConfiguration.Object, mockFileWriter.Object);

            var response = await controller.Convert(file.Object, "validFilename") as BadRequestObjectResult;
            var message = response.Value.GetType().GetProperty("error").GetValue(response.Value);

            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Error writing JSON file", message);
        }
        [Fact]
        public async Task Convert_XmlValidationException_ReturnsError()
        {
            var mockConverter = new Mock<IConverter>();
            var mockFileWriter = new Mock<IFileWriter>();
            var mockConfiguration = new Mock<IConfiguration>();
            var xmlContent = "<root><data>Sample XML Content</data></root>";

            var file = new Mock<IFormFile>();
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            file.Setup(f => f.OpenReadStream()).Returns(fileStream);
            file.Setup(f => f.Length).Returns(fileStream.Length);

            mockConverter.Setup(converter => converter.ValidateInputAsync(It.IsAny<string>()))
                .ThrowsAsync(new XmlException("Invalid XML data"));

            var controller = new ConvertController(mockConverter.Object, mockConfiguration.Object, mockFileWriter.Object);

            var response = await controller.Convert(file.Object, "validFilename") as BadRequestObjectResult;
            var message = response.Value.GetType().GetProperty("error").GetValue(response.Value);

            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Invalid XML data", message);
        }
        [Fact]
        public async Task Convert_ConvertXmlToJsonException_ReturnsError()
        {
            var mockConverter = new Mock<IConverter>();
            var mockFileWriter = new Mock<IFileWriter>();
            var mockConfiguration = new Mock<IConfiguration>();
            var xmlContent = "<root><data>Sample XML Content</data></root>";

            var file = new Mock<IFormFile>();
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            file.Setup(f => f.OpenReadStream()).Returns(fileStream);
            file.Setup(f => f.Length).Returns(fileStream.Length);

            mockConverter.Setup(converter => converter.ConvertAsync(It.IsAny<string>()))
                .ThrowsAsync(new ConvertXmlToJsonException("Error converting XML to JSON"));

            var controller = new ConvertController(mockConverter.Object, mockConfiguration.Object, mockFileWriter.Object);

            var response = await controller.Convert(file.Object, "validFilename") as BadRequestObjectResult;
            var message = response.Value.GetType().GetProperty("error").GetValue(response.Value);

            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Error converting XML to JSON", message);
        }

        [Fact]
        public async Task Convert_UnexpectedExeption_ReturnsError()
        {
            var mockConverter = new Mock<IConverter>();
            var mockFileWriter = new Mock<IFileWriter>();
            var mockConfiguration = new Mock<IConfiguration>();
            var xmlContent = "<root><data>Sample XML Content</data></root>";

            var file = new Mock<IFormFile>();
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            file.Setup(f => f.OpenReadStream()).Returns(fileStream);
            file.Setup(f => f.Length).Returns(fileStream.Length);

            mockConverter.Setup(converter => converter.ConvertAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new ConvertController(mockConverter.Object, mockConfiguration.Object, mockFileWriter.Object);

            var response = await controller.Convert(file.Object, "validFilename") as BadRequestObjectResult;
            var message = response.Value.GetType().GetProperty("error").GetValue(response.Value);

            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("An unexpected error occurred.", message);
        }

    }
}
