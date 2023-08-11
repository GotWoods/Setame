namespace Setame.Data
{
    public class Errors
    {
        public string ErrorCode { get; internal set; }
        public string Message { get; internal set; }
        public IReadOnlyList<string> Data => _underlyingData.AsReadOnly();
        


        private Errors(string errorCode, string message, params string[] data)
        {
            ErrorCode = errorCode;
            Message = message;
            _underlyingData = new List<string>();
            _underlyingData.AddRange(data);
        }
        
        public static Errors DuplicateName(string name) => new Errors("Mx1000", "DuplicateName", name);
        public static Errors ApplicationNotFound(Guid applicationId) => new Errors("Mx1001", "ApplicationNotFound", applicationId.ToString());
        public static Errors VariableNotFoundRename(string name) => new Errors("Mx1002", "VariableNotFoundRename", name);
        public static Errors EnvironmentNotFound(string name) => new Errors("Mx1003", "EnvironmentNotFound", name);
        public static Errors VariableNotFound(string name) => new Errors("Mx1004", "VariableNotFound", name);
        public static Errors TokenNotFound => new Errors("Mx1005", "TokenNotFound");
        public static Errors TokenExpired => new Errors("Mx1006", "TokenExpired");
        public static Errors AuthenticationFailed => new Errors("Mx1007", "AuthenticationFailed");

        private readonly List<string> _underlyingData;

        public static Errors FromValidation(string errorCode, string message)
        {
            return new Errors(errorCode, message);

        }

        public override string ToString()
        {
            return string.Format(this.Message);
        }
    }
}
