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

        public void delete(string id)
        {
            bool change = false;
            do
            {
                for (int i = 0; i < linkList.Count; i++)
                {
                    change = false;
                    if (linkList[i].getSourceRouter().getCallSign().Equals(id) || linkList[i].getDestinationRouter().getCallSign().Equals(id))
                    {
                        change = true;
                        linkList.RemoveAt(i);
                        break;
                    }
                }
            } while (change);
        }

        public bool update(Link linkToUpdate)
        {
            foreach(Link link in linkList)
            {
                if (!link.isEqual(linkToUpdate))
                    continue;
                return false;
            }

            bool updateMade = false;

            foreach(Link link in linkList)
            {
                updateMade = link.updateCapacity(linkToUpdate);
                if (updateMade)
                    break;
            }

            if (!updateMade)
                linkList.Add(linkToUpdate.copy());

            return true;
        }

        public int getNumberOfLinksFrom(string callSign)
        {
            int count = 0;

            foreach(Link link in linkList)
            {
                if(link.getSourceRouter().getCallSign().Equals(callSign))
                {
                    count++;
                }
            }

            return count;
        }

        public void refresh(string start, List<string> destinations)
        {
            bool repeat = false;
            int index = -1;
            do
            {
                index = -1;
                repeat = false;
                foreach (Link link in linkList)
                {
                    if (link.getSourceRouter().getCallSign().Equals(start))
                    {
                        bool lackOf = true;
                        foreach (string destination in destinations)
                        {
                            if (destination.Equals(link.getDestinationRouter().getCallSign()))
                            {
                                lackOf = false;
                            }
                        }
                        if (lackOf)
                        {
                            repeat = true;
                            index = linkList.IndexOf(link);
                            break;
                        }
                    }
                }
                if (index >= 0)
                {
                    linkList[index].write();
                    linkList.RemoveAt(index);
                }
            } while (repeat);
        }

    }

    
}
