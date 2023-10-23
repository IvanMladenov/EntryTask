using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XmlToJson.Exeptions;
using XmlToJson.Interactors.Interfaces;

namespace XmlToJson.Interactors.Implementations
{
    public class XmlToJsonConverter : IConverter
    {
        public async Task<string> ConvertAsync(string xmlContent)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent)))
                {
                    XDocument xmlDoc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

                    var jsonSettings = new JsonSerializerSettings
                    {
                        Formatting = Newtonsoft.Json.Formatting.Indented
                    };

                    string jsonString = await Task.Run(() => JsonConvert.SerializeObject(xmlDoc, jsonSettings));

                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                throw new ConvertXmlToJsonException("Error converting XML to JSON", ex);
            }
        }

        public async Task ValidateInputAsync(string xmlContent)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent)))
                {
                    await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
                }
            }
            catch (XmlException ex)
            {
                throw new XmlException("Invalid XML data", ex);
            }
        }
    }
}