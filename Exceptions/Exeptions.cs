using System.Runtime.CompilerServices;

namespace DataTransferApi.Exceptions
{
    public class NotFoundException:Exception
    {
        public NotFoundException(string message):base(message) 
        {
        }
    }

    public class GroupNotFoundException: NotFoundException
    {
        public string FileName { get; }
        public GroupNotFoundException(string message,string fName): base(message) 
        { 
            FileName = fName;
        }
    }

    public class CustFileNotFoundException : NotFoundException
    {
        public string FileName { get; }
        public CustFileNotFoundException(string message, string fName) : base(message)
        {
            FileName = fName;
        }
    }

    public class NotFullLoad:Exception
    {
        public NotFullLoad(string message):base(message) { }
    }
    
    public class NotFileFullLoad: NotFullLoad
    {
        public int Percentage { get; }
        public string FileName { get; }
        public NotFileFullLoad(string message,string fileName,int percentage ) : base(message) 
        { 
            FileName = fileName;
            Percentage = percentage;
        }
    }

    public class TokenException : Exception
    {
        public TokenException(string message) : base(message)
        {
        }
    }
    
    public class TokenTimeOutException:TokenException
    {
        public TokenTimeOutException(string message) : base(message)
        {
        }
    }

}
