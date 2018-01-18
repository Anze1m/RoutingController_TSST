using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class RouterTable
    {
        List<Router> routerList;

        public void addRouter (Router router)
        {
            routerList.Add(router);
        }
        public bool doesNotContain(Router searchedRouter)
        {
            foreach(Router router in routerList)
            {
                if (router.isEqual(searchedRouter))
                    return false;
            }
            return true;
        }
        public RouterTable()
        {
            routerList = new List<Router>();
        }
    }
}
