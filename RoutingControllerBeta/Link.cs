using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class Link
    {
        Router sourceRouter;
        byte sourceInterface;
        Router destinationRouter;
        byte destinationInterface;
        double capacity;
        double weight;

        public Link (string sourceCallSign, string sourceSubNetworkCallSign, string sourceAutonomicNetworkCallSign, byte sourceInterface, 
            string destinationCallSign, string destinationSubNetworkCallSign, string destinationAutonomicNetworkCallSign, byte destinationInterface,
            double capacity)
        {
            this.sourceRouter = new Router(sourceCallSign, sourceSubNetworkCallSign, sourceAutonomicNetworkCallSign);
            this.sourceInterface = sourceInterface;
            this.destinationRouter = new Router(destinationCallSign, destinationSubNetworkCallSign, destinationAutonomicNetworkCallSign);
            this.destinationInterface = destinationInterface;
            this.capacity = capacity;
            this.weight = 1 / capacity;
        }

        public Link(string sourceCallSign, string sourceSubNetworkCallSign, string sourceAutonomicNetworkCallSign, byte sourceInterface,
            string destinationCallSign, string destinationSubNetworkCallSign, string destinationAutonomicNetworkCallSign, byte destinationInterface,
            double capacity, double weight)
        {
            this.sourceRouter = new Router(sourceCallSign, sourceSubNetworkCallSign, sourceAutonomicNetworkCallSign);
            this.sourceInterface = sourceInterface;
            this.destinationRouter = new Router(destinationCallSign, destinationSubNetworkCallSign, destinationAutonomicNetworkCallSign);
            this.destinationInterface = destinationInterface;
            this.capacity = capacity;
            this.weight = weight;
        }

        public double getWeight()
        {
            return weight;
        }

        public double getCapacity()
        {
            return capacity;
        }

        public Router getSourceRouter()
        {
            return sourceRouter;
        }

        public Router getDestinationRouter()
        {
            return destinationRouter;
        }

        public byte getSourceInterface()
        {
            return sourceInterface;
        }

        public byte getDestinationInterface()
        {
            return destinationInterface;
        }

        public Link copy()
        {
            return new Link(sourceRouter.getCallSign(), sourceRouter.getSubNetworkCallSign(), sourceRouter.getAutonomicNetworkCallSign(), sourceInterface, destinationRouter.getCallSign(), destinationRouter.getSubNetworkCallSign(), destinationRouter.getAutonomicNetworkCallSign(), destinationInterface, capacity, weight);
        }

        public void write()
        {
            Console.Write(sourceRouter.getCallSign() + destinationRouter.getCallSign() + weight);
        }
    }
}
