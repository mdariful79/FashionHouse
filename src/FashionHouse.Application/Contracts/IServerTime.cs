using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionHouse.Application.Contracts
{
    public interface IServerTime
    {
        DateTime DateTime { get; }
    }
}
