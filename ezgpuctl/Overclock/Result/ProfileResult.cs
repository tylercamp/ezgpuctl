using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock.Result
{
    public class ProfileResult : IBehaviorResult
    {
        public ProfileResult(string profileName)
        {
            ProfileName = profileName;
        }

        public string ProfileName { get; }
    }
}
