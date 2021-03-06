﻿using System;
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

        public Link(LRMRCCommunications.LinkStateUpdate update)
        {
            this.sourceRouter = new Router(update.beginNode.id, update.beginNode.snId, update.beginNode.asId);
            this.sourceInterface = update.beginSNPP;
            this.destinationRouter = new Router(update.endNode.id, update.endNode.snId, update.endNode.asId);
            this.destinationInterface = update.endSNPP;
            this.capacity = update.capacity;
            this.weight = (1/(double)update.capacity);
        }

        public Link(LRMRCCommunications.Link update)
        {
            this.sourceRouter = new Router(update.beginNode.id, update.beginNode.snId, update.beginNode.asId);
            this.sourceInterface = update.beginSNPP;
            this.destinationRouter = new Router(update.endNode.id, update.endNode.snId, update.endNode.asId);
            this.destinationInterface = update.endSNPP;
            this.capacity = update.capacity;
            this.weight = (1 / (double)update.capacity);
        }

        public bool isEqual(Link linkToCompare)
        {
            if (!this.sourceRouter.isEqual(linkToCompare.getSourceRouter()))
                return false;

            if (!this.destinationRouter.isEqual(linkToCompare.getDestinationRouter()))
                return false;

            if (this.sourceInterface != linkToCompare.getSourceInterface())
                return false;

            if (this.destinationInterface != linkToCompare.getDestinationInterface())
                return false;

            if (this.capacity != linkToCompare.getCapacity())
                return false;

            if (this.weight != linkToCompare.getWeight())
                return false;

            return true;
        }

        public bool updateCapacity(Link linkToCompare)
        {
            if(this.sourceRouter.isEqual(linkToCompare.getSourceRouter()) && this.destinationRouter.isEqual(linkToCompare.getDestinationRouter()) && this.sourceInterface == linkToCompare.getSourceInterface() && this.destinationInterface == linkToCompare.getDestinationInterface())
            {
                this.capacity = linkToCompare.getCapacity();
                this.weight = linkToCompare.getWeight();
                return true;
            }
            return false;
        }
    }
}
