using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class RCRequestHandler
    {
        int portNumber;
        LinkTable linkTable;
        string subNetworkCallSign;
        string autonomicNetworkCallSign;
        public RCRequestHandler(int portNumber, LinkTable linkTable, string subNetworkCallSign, string autonomicNetworkCallSign)
        {
            this.portNumber = portNumber;
            this.linkTable = linkTable;
            this.subNetworkCallSign = subNetworkCallSign;
            this.autonomicNetworkCallSign = autonomicNetworkCallSign;
        }

        public void run()
        {
            IPEndPoint managingEndPoint = new IPEndPoint(IPAddress.Any, 0);
            EndPoint manager = (EndPoint)managingEndPoint;
            Socket orderingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
            orderingSocket.Bind(endPoint);
            Socket send;
            while (true)
            {
                byte[] buffer = new byte[orderingSocket.SendBufferSize];
                int readBytesNumber = orderingSocket.ReceiveFrom(buffer, ref manager);
                byte[] receivedData = new byte[readBytesNumber];
                Array.Copy(buffer, receivedData, readBytesNumber);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(receivedData, 0, receivedData.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                object o = (object)binaryFormatter.Deserialize(memoryStream);
                InnerRoutingCommunication.Request request = (InnerRoutingCommunication.Request)o;

                Logger.timestamp();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Received RoutePathRequest ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("(EntryPoint: " + request.callSign + " Capacity: " + request.requestedCapacity + ")");

                Dijkstra dijkstra = new Dijkstra(new AddressBook(), 0, linkTable.copy(), new Router(request.callSign, request.subNetworkCallSign, request.autonomicNetworkCallSign), request.requestedCapacity, subNetworkCallSign, autonomicNetworkCallSign, false);
                List<Path> calculatedPaths = dijkstra.getExitingPaths();
                InnerRoutingCommunication.Response response = new InnerRoutingCommunication.Response();
                foreach(Path path in calculatedPaths)
                {
                    response.add(path.getDestinationRouter().getCallSign(), path.getDestinationRouter().getSubNetworkCallSign(), path.getDestinationRouter().getAutonomicNetworkCallSign(), path.getSourceInterface(), path.getDestinationInterface(), path.getCapacity(), path.getWeight());
                    //path.write();
                }
                binaryFormatter = new BinaryFormatter();
                memoryStream = new MemoryStream();
                binaryFormatter.Serialize(memoryStream, response);
                byte[] sendbuf = memoryStream.ToArray();
                send = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                send.SendTo(sendbuf, new IPEndPoint(IPAddress.Parse("127.0.0.1"), request.portNumber));
                Logger.timestamp();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Sent RoutePathResponse ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("(PossibleCrossings: " + calculatedPaths.Count + ")");
            }
        }
    }
}
