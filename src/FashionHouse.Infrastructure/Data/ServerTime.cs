using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionHouse.Infrastructure.Data
{
    public class ServerTime : IServerTime
    {
        public DateTime DateTime
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}
