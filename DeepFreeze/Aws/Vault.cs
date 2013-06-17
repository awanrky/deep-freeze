using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepFreeze.Local;

namespace DeepFreeze.Aws
{
    public class Vault
    {
        public IEnumerable<Location> Locations { get; set; } 

        public Vault()
        {
            
        }
    }
}
