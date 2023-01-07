namespace Services.PortalService
{
    public interface IPortalService
    {
        void LinkOneWay(IPortal portalA, IPortal portalB);
        void LinkTwoWay(IPortal portalA, IPortal portalB);
        void UnLink(params IPortal[] portals);
    }
}