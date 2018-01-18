using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerRoutingCommunication
{
    [Serializable]
    public class Response
    {
        public List<Destination> destinations;

        public Response()
        {
            destinations = new List<Destination>();
        }

        public void add(string callSign, string subNetworkCallSign, string autonomicNetworkCallSign, byte sourceInterface, byte destinationInterface, double capacity, double weight)
        {
            destinations.Add(new Destination(callSign, subNetworkCallSign, autonomicNetworkCallSign, sourceInterface, destinationInterface, capacity, weight));
        }
    }

    [Serializable]
    public class Destination
    {
        public string callSign;
        public string subNetworkCallSign;
        public string autonomicNetworkCallSign;
        public double weight;
        public double capacity;
        public byte sourceInterface;
        public byte destinationInterface;

        public Destination(string callSign, string subNetworkCallSign, string autonomicNetworkCallSign, byte sourceInterface, byte destinationInterface, double capacity, double weight)
        {
            this.callSign = callSign;
            this.subNetworkCallSign = subNetworkCallSign;
            this.autonomicNetworkCallSign = autonomicNetworkCallSign;
            this.capacity = capacity;
            this.weight = weight;
            this.sourceInterface = sourceInterface;
            this.destinationInterface = destinationInterface;
        }
    }
}
