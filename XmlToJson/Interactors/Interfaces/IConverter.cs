using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XmlToJson.Interactors.Interfaces
{
    public interface IConverter
    {
        Task<string> ConvertAsync(string input);
        Task ValidateInputAsync(string input);
    }
}
