using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class AddressBook
    {
        List<Address> addresses;

        public AddressBook()
        {
            addresses = new List<Address>();
        }

        public AddressBook(string addressesFilePath)
        {
            addresses = new List<Address>();

            try
            {
                string[] addressesDefinitions = File.ReadAllLines(addressesFilePath);
                foreach(string addressDefinition in addressesDefinitions)
                {
                    string[] definition = addressDefinition.Split('_');
                    addresses.Add(new Address(definition[0], definition[1], Int32.Parse(definition[2])));
                }

            }
            catch (FileNotFoundException ex)
            {
            }
        }

        public void add(Address address)
        {
            this.addresses.Add(address);
        }

        public int getAddressOf(Router router)
        {
            foreach(Address address in addresses)
            {
                if (router.getSubNetworkCallSign().Equals(address.getSubNetworkCallSign()) && router.getAutonomicNetworkCallSign().Equals(address.getAutonomicNetworkCallSign()))
                    return address.getPortNumber();

            }
            return 0;
        }
    }
}
