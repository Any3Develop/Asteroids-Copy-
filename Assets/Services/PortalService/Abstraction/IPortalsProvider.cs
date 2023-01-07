using System.Collections.Generic;

namespace Services.PortalService
{
    public interface IPortalsProvider
    {
        IEnumerable<IPortal> GetPortals();
    }
}