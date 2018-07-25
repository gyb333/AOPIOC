using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Globals
    {
        static Globals()
        {
            Configuration = ConfigurationManager.OpenExeConfiguration($"{typeof(Globals).Assembly.GetName().Name}.dll");
            
            //AutoMapper.Mapper.Initialize((cfg) => cfg.AddProfile<ModelProfiles>());
        }
        public static Configuration Configuration
        {
            get; private set;
        }

    }
}
