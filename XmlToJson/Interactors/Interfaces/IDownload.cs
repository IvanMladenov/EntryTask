using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XmlToJson.Interactors.Interfaces
{
    public interface IDownload
    {
        byte[] GetBytes(string directory, string fileName);
    }
}
