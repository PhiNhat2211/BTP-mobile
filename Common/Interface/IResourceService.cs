using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Common.Interface
{
    public interface IResourceService
    {
        string GetResourceITV(string resourceKey, string messageGroup);
        string GetResourceRMG(string resourceKey, string messageGroup);
        string GetResourceECH(string resourceKey, string messageGroup);
        string GetResourceSC(string resourceKey, string messageGroup);
    }
}
