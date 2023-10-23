using System.IO;
using System.Threading.Tasks;
using XmlToJson.Interactors.Implementations;
using Xunit;

namespace XmlToJson.Tests
{
    public class FileWritterTests
    {
        [Fact]
        public async Task WriteToFileAsync_Success_CheckFileExists()
        {
            var directory = "C:/XmlToJsonConverter/TestsFolder";
            var fileName = "output.json";
            var data = "{test: 'test'}";

            var fileWriter = new FileWriter();

            try
            {
                await fileWriter.WriteToFileAsync(directory, fileName, data);

                var filePath = Path.Combine(directory, fileName);
                Assert.True(File.Exists(filePath));
            }
            finally
            {
                var filePath = Path.Combine(directory, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

    }
}
