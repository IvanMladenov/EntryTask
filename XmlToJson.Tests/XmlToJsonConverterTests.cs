using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Xml;
using XmlToJson.Exeptions;
using XmlToJson.Interactors.Implementations;
using Xunit;

namespace XmlToJson.Tests
{
    public class XmlToJsonConverterTests
    {
        [Fact]
        public async Task ValidateInputAsync_ValidXml_DoesNotThrowException()
        {
            var converter = new XmlToJsonConverter();
            var validXml = "<root><element>test</element></root>";

            try
            {
                await converter.ValidateInputAsync(validXml);
            }
            catch (XmlException)
            {
                Assert.True(false, "Expected no XmlException to be thrown.");
            }
        }

        [Fact]
        public async Task ValidateInputAsync_InvalidXml_ThrowsXmlException()
        {
            var converter = new XmlToJsonConverter();
            var invalidXml = "<root><element>test</root>";

            await Assert.ThrowsAsync<XmlException>(() => converter.ValidateInputAsync(invalidXml));
        }

        [Fact]
        public async Task ConvertAsync_ValidXml_ReturnsValidJson()
        {
            var converter = new XmlToJsonConverter();
            var validXml = "<root><element>test</element></root>";
            var expectedJson = "{\"root\":{\"element\":\"test\"}}";

            var actualJson = await converter.ConvertAsync(validXml);

            var expectedJObject = JsonConvert.DeserializeObject<JObject>(expectedJson);
            var actualJObject = JsonConvert.DeserializeObject<JObject>(actualJson);

            Assert.Equal(expectedJObject, actualJObject);
        }

        [Fact]
        public async Task ConvertAsync_InvalidXml_ThrowsConvertXmlToJsonException()
        {
            var converter = new XmlToJsonConverter();
            var invalidXml = "<root><element>test</root>";

            await Assert.ThrowsAsync<ConvertXmlToJsonException>(() => converter.ConvertAsync(invalidXml));
        }
    }
}
