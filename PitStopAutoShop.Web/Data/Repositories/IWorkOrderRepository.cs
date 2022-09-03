﻿using PitStopAutoShop.Web.Data.Entities;
using System.Linq;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IWorkOrderRepository : IGenericRepository<WorkOrder>
    {
        IQueryable GetAllWorkOrders();

    }
}
