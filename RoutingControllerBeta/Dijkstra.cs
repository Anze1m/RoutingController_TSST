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
    class Dijkstra
    {
        Path currentPath;
        List<Path> calculatedPaths;
        RouterTable visitedRouters;
        string subNetworkCallSign;
        string autonomicNetworkCallSign;
        Router startingRouter;

        public Dijkstra(AddressBook addressBook, int myPortNumber, LinkTable linkTable, Router startingRouter, double requestedCapacity, string subNetworkCallSign, string autonomicNetworkCallSign, bool totalASDijkstra)
        {
            this.subNetworkCallSign = subNetworkCallSign;
            this.autonomicNetworkCallSign = autonomicNetworkCallSign;
            visitedRouters = new RouterTable();
            calculatedPaths = new List<Path>();
            this.startingRouter = startingRouter.copy();
            Router currentRouter = startingRouter;
            bool firstIteration = true;

            IPEndPoint managingEndPoint = new IPEndPoint(IPAddress.Any, 0);
            EndPoint manager = (EndPoint)managingEndPoint;
            Socket orderingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            if (myPortNumber != 0)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), myPortNumber);
                orderingSocket.Bind(endPoint);
            }
            

            while (true)
            {
                visitedRouters.addRouter(currentRouter);
                
                if (firstIteration)
                    calculatedPaths.Add(new Path(currentRouter));

                List<Link> currentLinks = new List<Link>();

                if (currentRouter.getSubNetworkCallSign().Equals(subNetworkCallSign))
                {
                    currentLinks = linkTable.getLinksFrom(currentRouter, requestedCapacity);
                }
                else
                {
                    InnerRoutingCommunication.Request request = new InnerRoutingCommunication.Request(currentRouter.getCallSign(), currentRouter.getSubNetworkCallSign(), currentRouter.getAutonomicNetworkCallSign(), requestedCapacity, myPortNumber);
                    int portNumber = addressBook.getAddressOf(currentRouter);

                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    MemoryStream memoryStream = new MemoryStream();
                    binaryFormatter.Serialize(memoryStream, request);
                    byte[] sendbuf = memoryStream.ToArray();
                    Socket send = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    send.SendTo(sendbuf, new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber));

                    
                    byte[] buffer = new byte[orderingSocket.SendBufferSize];
                    int readBytesNumber = orderingSocket.ReceiveFrom(buffer, ref manager);
                    byte[] receivedData = new byte[readBytesNumber];
                    Array.Copy(buffer, receivedData, readBytesNumber);
                    binaryFormatter = new BinaryFormatter();
                    memoryStream = new MemoryStream();
                    memoryStream.Write(receivedData, 0, receivedData.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    object o = (object)binaryFormatter.Deserialize(memoryStream);
                    InnerRoutingCommunication.Response response = (InnerRoutingCommunication.Response)o;
                    List<InnerRoutingCommunication.Destination> destinations = response.destinations;

                    foreach(InnerRoutingCommunication.Destination destination in destinations)
                    {
                        currentLinks.Add(new Link(currentRouter.getCallSign(), currentRouter.getSubNetworkCallSign(), currentRouter.getAutonomicNetworkCallSign(), destination.sourceInterface, destination.callSign, destination.subNetworkCallSign, destination.autonomicNetworkCallSign, destination.destinationInterface, destination.capacity, destination.weight));

                    }
                }

                if (currentLinks.Count != 0)
                {

                    if (firstIteration)
                    {
                        foreach (Link link in currentLinks)
                        {
                            calculatedPaths.Add(new Path(link));
                        }
                    }
                    else
                    {
                        foreach (Link link in currentLinks)
                        {
                            bool routerAlreadyReached = false;


                            for (int i = 0; i < calculatedPaths.Count; i++)
                            {
                                if (calculatedPaths[i].getDestinationRouter().isEqual(link.getDestinationRouter()))
                                {
                                    routerAlreadyReached = true;
                                    if (currentPath.getWeight() + link.getWeight() < calculatedPaths[i].getWeight())
                                    {
                                        calculatedPaths[i] = new Path(currentPath, link);
                                    }
                                }
                            }

                            if (!routerAlreadyReached)
                            {
                                calculatedPaths.Add(new Path(currentPath, link));
                            }
                        }
                    }
                }

                bool change = false;
                foreach (Path path in calculatedPaths)
                {
                    if (totalASDijkstra && myPortNumber != 0)
                    {
                        if (visitedRouters.doesNotContain(path.getDestinationRouter())
                            && autonomicNetworkCallSign.Equals(path.getDestinationRouter().getAutonomicNetworkCallSign()))
                        {
                            change = true;
                            currentPath = path;
                        }
                    }
                    else
                    {
                        if (visitedRouters.doesNotContain(path.getDestinationRouter())
                            && autonomicNetworkCallSign.Equals(path.getDestinationRouter().getAutonomicNetworkCallSign())
                            && subNetworkCallSign.Equals(path.getDestinationRouter().getSubNetworkCallSign()))
                        {
                            change = true;
                            currentPath = path;
                        }
                    }

                }
                if (currentPath != null)
                {
                    foreach (Path path in calculatedPaths)
                    {
                        if (totalASDijkstra && myPortNumber != 0)
                        {
                            if (path.getWeight() < currentPath.getWeight()
                                && visitedRouters.doesNotContain(path.getDestinationRouter())
                                && autonomicNetworkCallSign.Equals(path.getDestinationRouter().getAutonomicNetworkCallSign()))
                            {
                                currentPath = path;
                            }
                        }
                        else
                        {
                            if (path.getWeight() < currentPath.getWeight()
                                && visitedRouters.doesNotContain(path.getDestinationRouter())
                                && autonomicNetworkCallSign.Equals(path.getDestinationRouter().getAutonomicNetworkCallSign())
                                && subNetworkCallSign.Equals(path.getDestinationRouter().getSubNetworkCallSign()))
                            {
                                currentPath = path;
                            }
                        }

                    }

                    currentRouter = currentPath.getDestinationRouter();

                    firstIteration = false;


                }
                if (!change)
                {
                    //this.write();
                    orderingSocket.Close();
                    break;
                }
                    
            }

        }

        public List<Path> getExitingPaths()
        {
            List<Path> pathsToReturn = new List<Path>();
            
            foreach(Path path in calculatedPaths)
            {
                if (!(autonomicNetworkCallSign.Equals(path.getDestinationRouter().getAutonomicNetworkCallSign())
                    && subNetworkCallSign.Equals(path.getDestinationRouter().getSubNetworkCallSign())))
                {
                    pathsToReturn.Add(path);
                }
            }
            return pathsToReturn;
        }

        public Path getPathTo(Router finalRouter)
        {
            foreach (Path path in calculatedPaths)
                if (path.getDestinationRouter().isEqual(finalRouter))
                    return path;

            return this.getPathTo(finalRouter.getSubNetworkCallSign(), finalRouter.getAutonomicNetworkCallSign());
        }

        public Path getPathTo(string autonomicNetworkCallSign)
        {
            Path pathToReturn = new Path(startingRouter);

            foreach (Path path in calculatedPaths)
                if (path.getDestinationRouter().getAutonomicNetworkCallSign().Equals(autonomicNetworkCallSign))
                    pathToReturn = path;

            foreach (Path path in calculatedPaths)
                if (path.getDestinationRouter().getAutonomicNetworkCallSign().Equals(autonomicNetworkCallSign) && pathToReturn.getWeight() > path.getWeight())
                    pathToReturn = path;

            return pathToReturn;
        }

        public Path getPathTo(string subNetworkCallSign, string autonomicNetworkCallSign)
        {
            Path pathToReturn = new Path(startingRouter);

            foreach (Path path in calculatedPaths)
                if (path.getDestinationRouter().getAutonomicNetworkCallSign().Equals(autonomicNetworkCallSign) && path.getDestinationRouter().getSubNetworkCallSign().Equals(subNetworkCallSign))
                    pathToReturn = path;

            if(pathToReturn != null)
            foreach (Path path in calculatedPaths)
                if (path.getDestinationRouter().getAutonomicNetworkCallSign().Equals(autonomicNetworkCallSign) && pathToReturn.getWeight() > path.getWeight() && path.getDestinationRouter().getSubNetworkCallSign().Equals(subNetworkCallSign))
                    pathToReturn = path;

            return pathToReturn;
        }

        public void write()
        {
            for(int i = 0; i < calculatedPaths.Count; i++)
            {
                calculatedPaths[i].write();
            }
        }

    }
}
