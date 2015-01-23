using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlasmaShaft
{
    public interface IStoppable
    {
        bool Stopped { get; }
        void Stop();
        void Continue();
    }
}
