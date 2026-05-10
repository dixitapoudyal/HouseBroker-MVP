using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.App.Auth.Interfaces
{
    public interface ICommissionService
    {
        Task<decimal> CalculateAsync(decimal price);
    }
}
