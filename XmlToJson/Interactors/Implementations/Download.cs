using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XmlToJson.Interactors.Interfaces;

namespace XmlToJson.Interactors.Implementations
{
    public class Download : IDownload
    {
        public byte[] GetBytes(string directory, string fileName)
        {
            string path = Path.Combine(directory, fileName);

            if (CheckForExistingFile(path))
            {
                return File.ReadAllBytes(path);
            }
                
            throw new NotImplementedException();
        }

        private bool CheckForExistingFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return true;
            }

            return false;
        }
    }
}
