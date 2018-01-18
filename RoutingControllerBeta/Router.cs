using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class Router
    {
        string callSign;
        string subNetworkCallSign;
        string autonomicNetworkCallSign;

        public Router(string callSign, string subNetworkCallSign, string autonomicNetworkCallSign)
        {
            this.callSign = callSign;
            this.subNetworkCallSign = subNetworkCallSign;
            this.autonomicNetworkCallSign = autonomicNetworkCallSign;
        }

        public string getCallSign()
        {
            return callSign;
        }

        public string getSubNetworkCallSign()
        {
            return subNetworkCallSign;
        }

        public string getAutonomicNetworkCallSign()
        {
            return autonomicNetworkCallSign;
        }

        public bool isEqual(Router router)
        {
            if (!this.callSign.Equals(router.getCallSign()))
                return false;
            if (!this.subNetworkCallSign.Equals(router.getSubNetworkCallSign()))
                return false;
            if (!this.autonomicNetworkCallSign.Equals(router.getAutonomicNetworkCallSign()))
                return false;
            return true;
        }

        public Router copy()
        {
            return new Router(this.callSign, this.subNetworkCallSign, this.autonomicNetworkCallSign);
        }
    }
}
