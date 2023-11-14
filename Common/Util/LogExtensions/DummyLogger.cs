using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogExtensions
{
    internal sealed class DummyLogger : log4net.Repository.Hierarchy.Logger
    {
        internal DummyLogger(string name) : base(name)
        {
            base.Hierarchy = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
        }
    }
}
