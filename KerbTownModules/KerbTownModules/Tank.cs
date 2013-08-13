using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KerbTownModules
{
    public class Tank
    {
        public Part part;
        public PartResource partResource;
        public string fuelName;
        public string fuelGUIName;

        public Tank()
        {
        }

        public Tank(Part _part, PartResource _partResource, string _fuelGUIName)
        {
            part = _part;
            partResource = _partResource;
            fuelGUIName = _fuelGUIName;
        }
    }
}
