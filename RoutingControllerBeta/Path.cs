using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class Path
    {
        Router sourceRouter;
        byte sourceInterface;
        Router destinationRouter;
        byte destinationInterface;
        List<Link> path;
        double weight;
        double capacity;

        public Path(Link link)
        {
            this.sourceRouter = link.getSourceRouter();
            this.sourceInterface = link.getSourceInterface();
            this.destinationRouter = link.getDestinationRouter();
            this.destinationInterface = link.getDestinationInterface();
            this.path = new List<Link>();
            this.path.Add(link);
            this.weight = link.getWeight();
            this.capacity = link.getCapacity();
        }

        public Path(Path path, Link link)
        {
            this.sourceRouter = path.getSourceRouter();
            this.sourceInterface = path.getSourceInterface();
            if (this.sourceInterface == 0)
                this.sourceInterface = link.getSourceInterface();
            this.destinationRouter = link.getDestinationRouter();
            this.destinationInterface = link.getDestinationInterface();
            this.path = new List<Link>();

            foreach (Link pathLink in path.getPath())
                this.path.Add(pathLink.copy());
            
            this.path.Add(link.copy());
            this.weight = path.getWeight() + link.getWeight();
            this.capacity = double.MaxValue;
            foreach(Link analisedLink in this.path)
            {
                if (analisedLink.getCapacity() < this.capacity)
                    this.capacity = analisedLink.getCapacity();
            }

        }

        public Path(Router router)
        {
            this.sourceRouter = router;
            this.sourceInterface = 0;
            this.destinationRouter = router;
            this.destinationInterface = 0;
            this.path = new List<Link>();
            this.weight = 0;
            this.capacity = double.MaxValue;
        }

        public Router getSourceRouter()
        {
            return sourceRouter;
        }

        public Router getDestinationRouter()
        {
            return destinationRouter;
        }
        public double getWeight()
        {
            return weight;
        }

        public double getCapacity()
        {
            return capacity;
        }

        public byte getSourceInterface()
        {
            return sourceInterface;
        }

        public byte getDestinationInterface()
        {
            return destinationInterface;
        }

        public List<Link> getPath()
        {
            return path;
        }

        public void write()
        {
            Console.Write("S: " + sourceRouter.getCallSign() + " D: " + destinationRouter.getCallSign() + " W: " + weight + " HC: " + path.Count + " C: " + capacity + " || ");

            foreach(Link link in path)
            {
                link.write();
                Console.Write(" ");
                
            }
            Console.WriteLine("  END");
        }
        


    }
}
