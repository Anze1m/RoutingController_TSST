using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class LinkTableUpdater
    {
        LinkTable linkTable;
        public LinkTableUpdater (LinkTable linkTable)
        {
            this.linkTable = linkTable;
        }

        public void update(byte[] receivedData)
        {
            Logger.timestamp();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Received LocalTopology ");
            
        }
    }
}
