using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XmlToJson.Interactors.Interfaces
{
    public interface IFileWriter
    {
        Task WriteToFileAsync(string directory, string fileName, string data);
    }
}
