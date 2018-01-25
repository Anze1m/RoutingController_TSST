using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class RouteRequestResolver
    {
        AddressBook addressBook;
        int myPortNumber;
        LinkTable linkTable;
        string subNetworkCallSign;
        string autonomicNetworkCallSign;
        string name;
        Mutex consoleMutex;

        public RouteRequestResolver(AddressBook addressBook, int myPortNumber, LinkTable linkTable, string subNetworkCallSign, string autonomicNetworkCallSign, string name, Mutex consoleMutex)
        {
            this.consoleMutex = consoleMutex;
            this.addressBook = addressBook;
            this.myPortNumber = myPortNumber;
            this.linkTable = linkTable;
            this.subNetworkCallSign = subNetworkCallSign;
            this.autonomicNetworkCallSign = autonomicNetworkCallSign;
            this.name = name;
        }

        public void answerTo(byte[] receivedData)
        {
            consoleMutex.WaitOne();
            Logger.timestamp();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Received RouteTableQuery ");

            RCcommunications.RouteRequest request = (RCcommunications.RouteRequest)Communications.Serialization.Deserialize(receivedData);
            Router startingRouter = new Router(request.from, subNetworkCallSign, autonomicNetworkCallSign);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("(From: " + request.from + " To: " + request.to + " Capacity: " + request.bitrate + ")");
            consoleMutex.ReleaseMutex();

            Dijkstra dijkstra = new Dijkstra(addressBook, myPortNumber, linkTable.copy(), startingRouter, request.bitrate, subNetworkCallSign, autonomicNetworkCallSign, true);
            string destination = request.to;
            string[] destinationParams = destination.Split(';');
            Path pathToReturn = new Path(new Router(request.from, subNetworkCallSign, autonomicNetworkCallSign));
            if(destinationParams[0].Split('.')[0].Equals("AS"))
            {
                pathToReturn = dijkstra.getPathTo(destinationParams[0]);
            }
            if (destinationParams[0].Split('.')[0].Equals("R"))
            {
                Router finalRouter;
                if(destinationParams.Length>1)
                {
                    finalRouter = new Router(destinationParams[0], destinationParams[1], autonomicNetworkCallSign);
                }
                else
                {
                    finalRouter = new Router(destinationParams[0], subNetworkCallSign, autonomicNetworkCallSign);
                }
                pathToReturn = dijkstra.getPathTo(finalRouter);

            }
            List<RCcommunications.Hop> transformedPath = new List<RCcommunications.Hop>();
            

            if (!(pathToReturn == null))
            {
                foreach(Link link in pathToReturn.getPath())
                    {
                        if (link.getSourceRouter().getAutonomicNetworkCallSign().Equals(autonomicNetworkCallSign) && link.getSourceRouter().getSubNetworkCallSign().Equals(subNetworkCallSign))
                            transformedPath.Add(new RCcommunications.Hop(link.getSourceRouter().getCallSign(), link.getSourceInterface(), link.getDestinationInterface(), link.getSourceRouter().getAutonomicNetworkCallSign(), link.getSourceRouter().getSubNetworkCallSign()));
                        else
                            transformedPath.Add(new RCcommunications.Hop(link.getSourceRouter().getCallSign(), 0, 0, link.getSourceRouter().getAutonomicNetworkCallSign(), link.getSourceRouter().getSubNetworkCallSign()));
                    
                    }
            }
            
            if(pathToReturn.getPath().Count > 0 )
                transformedPath.Add(new RCcommunications.Hop(pathToReturn.getPath().Last().getDestinationRouter().getCallSign(), 0, 0, pathToReturn.getPath().Last().getDestinationRouter().getAutonomicNetworkCallSign(), pathToReturn.getPath().Last().getDestinationRouter().getSubNetworkCallSign()));
            

            RCcommunications.RouteResponse response = new RCcommunications.RouteResponse(name, myPortNumber, transformedPath, request.seq);
            byte[] sendbuf = Communications.Serialization.Serialize(response);
            Socket send = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            send.SendTo(sendbuf, new IPEndPoint(IPAddress.Parse("127.0.0.1"), request.senderPort));

            consoleMutex.WaitOne();
            Logger.timestamp();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sent RouteTableQueryResponse ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("(");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("#");
            Console.ForegroundColor = ConsoleColor.White;
            consoleMutex.ReleaseMutex();

            foreach (RCcommunications.Hop hop in transformedPath)
            {
                Console.Write(hop.routerID + ";" + hop.SN + ";" + hop.AS);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("#");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(")");
        }
    }
}
