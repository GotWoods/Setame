using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data
{
    public class Errors
    {
        public int ErrorCode { get; internal set; }
        public string Message { get; internal set; }
        public IReadOnlyList<string> Data => _underlyingData.AsReadOnly();


        private Errors(int errorCode, string message, params string[] data)
        {
            ErrorCode = errorCode;
            Message = message;
            _underlyingData = new List<string>();
            _underlyingData.AddRange(data);
        }
        
        public static Errors DuplicateName(string name) => new Errors(1000, "DuplicateName", name);
        private readonly List<string> _underlyingData;
    }


    public class WTF
    {
        public WTF()
        {
            Errors.DuplicateName("test");
        }
    }
}
