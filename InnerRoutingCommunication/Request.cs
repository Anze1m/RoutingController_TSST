using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerRoutingCommunication
{
    [Serializable]
    public class Request
    {
        public string callSign;
        public string subNetworkCallSign;
        public string autonomicNetworkCallSign;
        public double requestedCapacity;
        public int portNumber;

        public Request (string callSign, string subNetworkCallSign, string autonomicNetworkCallSign, double requestedCapacity, int portNumber)
        {
            this.callSign = callSign;
            this.subNetworkCallSign = subNetworkCallSign;
            this.autonomicNetworkCallSign = autonomicNetworkCallSign;
            this.requestedCapacity = requestedCapacity;
            this.portNumber = portNumber;
        }
    }
}
