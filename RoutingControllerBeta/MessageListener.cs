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
    class MessageListener
    {
        LinkTableUpdater linkTableUpdater;
        RouteRequestResolver routeRequestResolver;
        Socket orderingSocket;
        EndPoint manager;

        public MessageListener(LinkTableUpdater linkTableUpdater, RouteRequestResolver routeRequestResolver, int mainPort)
        {
            this.linkTableUpdater = linkTableUpdater;
            this.routeRequestResolver = routeRequestResolver;
            IPEndPoint managingEndPoint = new IPEndPoint(IPAddress.Any, 0);
            this.manager = (EndPoint)managingEndPoint;

            this.orderingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), mainPort);
            orderingSocket.Bind(endPoint);
        }

        public void run()
        {
            while (true)
            {
                Console.WriteLine("Waiting for message from CC");
                byte[] buffer = new byte[orderingSocket.SendBufferSize];
                int readBytesNumber = orderingSocket.ReceiveFrom(buffer, ref manager);
                byte[] receivedData = new byte[readBytesNumber];
                Array.Copy(buffer, receivedData, readBytesNumber);
                Console.WriteLine("Received message from CC");
                Communications.Message message = Communications.Serialization.Deserialize(receivedData);
                if(message.messageType.Equals("RouteRequest"))
                {
                    Thread routing = new Thread(() => routeRequestResolver.answerTo(receivedData));
                    routing.Start();
                }

                ///jakieś updatowanie tablicy na kiju
            }
        }
    }
}
