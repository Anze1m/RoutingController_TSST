using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class LinkTableUpdater
    {
        LinkTable linkTable;
        private Dictionary<string, bool> devicesStates;
        private Dictionary<string, Timer> devicesTimers;
        Mutex linkTableMutex = new Mutex();
        Mutex consoleMutex;
        Mutex dictionaryMutex = new Mutex();
        int timeout;
        public LinkTableUpdater (LinkTable linkTable, int timeout, Mutex consoleMutex)
        {
            this.linkTable = linkTable;
            this.timeout = timeout;
            devicesTimers = new Dictionary<string, Timer>();
            devicesStates = new Dictionary<string, bool>();
            this.consoleMutex = consoleMutex;
        }

        public void update(byte[] receivedData)
        {
            LRMRCCommunications.LinkStateUpdate linkToUpdate = (LRMRCCommunications.LinkStateUpdate)Communications.Serialization.Deserialize(receivedData);
            Link receivedLink = new Link(linkToUpdate);
            string receivedCallSign = receivedLink.getSourceRouter().getCallSign();


            linkTableMutex.WaitOne();
            if (!devicesTimers.ContainsKey(receivedCallSign))
            {
                devicesStates.Add(receivedCallSign, true);
                devicesTimers.Add(receivedCallSign, new Timer(deviceDead, receivedCallSign, timeout, 0));
                consoleMutex.WaitOne();
                Logger.timestamp();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Router " + receivedCallSign + " is ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("up");
                Console.ForegroundColor = ConsoleColor.White;
                consoleMutex.ReleaseMutex();

            }
            else
            {
                if (devicesStates[receivedCallSign] == true)
                {
                    devicesTimers[receivedCallSign].Change(5000, 0);
                }
                else
                {
                    devicesStates[receivedCallSign] = true;
                    devicesTimers[receivedCallSign] = new Timer(deviceDead, receivedCallSign, timeout, 0);
                    consoleMutex.WaitOne();
                    Logger.timestamp();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Router " + receivedCallSign + " is ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("up");
                    Console.ForegroundColor = ConsoleColor.White;
                    consoleMutex.ReleaseMutex();
                }
            }


            bool change = linkTable.update(receivedLink);

            linkTableMutex.ReleaseMutex();
            if (change)
            {
                consoleMutex.WaitOne();
                Logger.timestamp();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Received LocalTopology ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("(From: " + receivedLink.getSourceRouter().getCallSign() + " To: " + receivedLink.getDestinationRouter().getCallSign() + " Capacity: " + receivedLink.getCapacity() + ")");
                consoleMutex.ReleaseMutex();
            }
            
        }

        private void deviceDead(object state)
        {
            string id = (string)state;


            linkTableMutex.WaitOne();
            devicesStates[id] = false;
            linkTable.delete(id);
            linkTableMutex.ReleaseMutex();

            consoleMutex.WaitOne();
            Logger.timestamp();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Router " + id + " is ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("down");
            Console.ForegroundColor = ConsoleColor.White;
            consoleMutex.ReleaseMutex();
        }
    }
}
