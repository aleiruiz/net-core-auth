using XLocker.DTOs.Common;
using XLocker.Types;

namespace XLocker.DTOs.Diagnostic
{
    public class GetDiagnosticDTO : QueryParamsDTO
    {
        public ConnectionState? ConnectionState { get; set; }
        public string? LockerId { get; set; }

    }
}
