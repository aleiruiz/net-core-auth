using XLocker.Types;

namespace XLocker.Helpers
{
    public static class ServiceTypeOrder
    {
        public static bool ValidateNextStatus(ServiceStatus currentStatus, ServiceStatus nextStatus)
        {
            if (nextStatus == ServiceStatus.SN)
            {
                return true;
            }
            List<ServiceStatus> availableStatuses = new List<ServiceStatus>();
            switch (currentStatus)
            {
                case ServiceStatus.SC:
                    availableStatuses = new List<ServiceStatus> {
                        ServiceStatus.SF,
                        ServiceStatus.SD,
                    };
                    break;
                case ServiceStatus.SD:
                    availableStatuses = new List<ServiceStatus> {
                        ServiceStatus.SR,
                        ServiceStatus.SB,
                    };
                    break;
                case ServiceStatus.SB:
                    availableStatuses = new List<ServiceStatus> {
                        ServiceStatus.SV,
                        ServiceStatus.SDB,
                        ServiceStatus.SR,
                    };
                    break;
            }

            return availableStatuses.Contains(nextStatus);
        }
    }
}
