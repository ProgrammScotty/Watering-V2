using System;
using System.Collections.Generic;
using System.Text;

namespace Watering2.Utils
{
    public interface ICloseable
    {
        event EventHandler<EventArgs> RequestClose;
    }
}
