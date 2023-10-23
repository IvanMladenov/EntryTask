using System;
using System.IO;
using System.Threading.Tasks;
using XmlToJson.Exeptions;
using XmlToJson.Interactors.Interfaces;

namespace XmlToJson.Interactors.Implementations
{
    public class FileWriter : IFileWriter
    {
        public async Task WriteToFileAsync(string directory, string fileName, string data)
        {
            try
            {
                Directory.CreateDirectory(directory);

                await File.WriteAllTextAsync(Path.Combine(directory, fileName), data);
            }
            catch (Exception ex)
            {
                throw new JsonFileWriteException($"Failed to write data on disk", ex);
            }
        }
    }
}
