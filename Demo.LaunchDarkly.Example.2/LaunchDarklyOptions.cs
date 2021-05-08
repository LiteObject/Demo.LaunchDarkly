using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.LaunchDarkly.Example._2
{
    public class LaunchDarklyOptions
    {
        //public const string ConfigSectionKey = "LaunchDarkly";

        public string SdkKey { get; set; }

        public string FeatureFlagKey { get; set; }
    }
}
