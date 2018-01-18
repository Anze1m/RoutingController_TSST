using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class LinkTable
    {
        List<Link> linkList;

        public LinkTable()
        {
            linkList = new List<Link>();
        }
        public List<Link> getLinksFrom(Router router, double requestedCapacity)
        {
            List<Link> linksToReturn = new List<Link>();

            foreach(Link link in linkList)
            {
                if(link.getSourceRouter().isEqual(router) && link.getCapacity()>=requestedCapacity)
                {
                    linksToReturn.Add(link);
                }
            }

            return linksToReturn;
        }

        public void add(Link link)
        {
            linkList.Add(link);
        }

        public LinkTable copy()
        {
            LinkTable linkTableToReturn = new LinkTable();
            foreach(Link link in linkList)
            {
                linkTableToReturn.add(link.copy());
            }
            return linkTableToReturn;
        }
    }

    
}
