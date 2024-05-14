using XLocker.Response.Common;

namespace XLocker.Response.Maintance
{
    public class DiagnosticResponse : ABSDiagnostic
    {
        public ABSLocker Locker { get; set; } = null!;
    }
}
