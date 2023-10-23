using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XmlToJson.Exeptions
{
    public class ConvertXmlToJsonException : Exception
    {
        public ConvertXmlToJsonException() { }
        public ConvertXmlToJsonException(string message) : base(message) { }
        public ConvertXmlToJsonException(string message, Exception inner) : base(message, inner) { }
    }
}
