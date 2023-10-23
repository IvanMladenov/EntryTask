using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XmlToJson.Exeptions
{
    public class JsonFileWriteException : Exception
    {
        public JsonFileWriteException() { }
        public JsonFileWriteException(string message) : base(message) { }
        public JsonFileWriteException(string message, Exception inner) : base(message, inner) { }
    }
}
