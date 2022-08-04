using Fathym.LCU.Services.StateAPIs.Durable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.TestStates
{
    public class TestState : LCUStateEntity
    {
        public virtual string Test { get; set; }
    }
}
