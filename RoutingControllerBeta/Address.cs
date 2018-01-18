using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class Address
    {
        string subNetworkCallSign;
        string autonomicNetworkCallSign;
        int portNumber;

        public Address(string subNetworkCallSign, string autonomicNetworkCallSign, int portNumber)
        {
            this.subNetworkCallSign = subNetworkCallSign;
            this.autonomicNetworkCallSign = autonomicNetworkCallSign;
            this.portNumber = portNumber;
        }
        public string getSubNetworkCallSign()
        {
            return subNetworkCallSign;
        }

        public string getAutonomicNetworkCallSign()
        {
            return autonomicNetworkCallSign;
        }

        public int getPortNumber()
        {
            return portNumber;
        }
    }
}
