using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class WorkOrderRepository : GenericRepository<WorkOrder>, IWorkOrderRepository
    {
        private readonly DataContext _context;

        public WorkOrderRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetActiveWorkOrdersNumber()
        {
            var workOrders = await _context.WorkOrders.Where(wo => wo.Status != "Closed").ToListAsync();

            return workOrders.Count;
        }

        public IQueryable GetAllWorkOrders()
        {
            return _context.WorkOrders
                .Include(wo => wo.ServiceDoneBy)
                .Include(wo => wo.UpdatedBy)
                .Include(wo => wo.CreatedBy)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Mechanic)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Estimate)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Brand)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Model);
        }

        public async Task<int> GetOpenedWorkOrdersAsync()
        {
            var workOrders = _context.WorkOrders.Where(wo => wo.Status == "Opened");
            return workOrders.Count();
        }

        public async Task<WorkOrder> GetWorkOrderByAppointmentIdAsync(int appointmentId)
        {
            return await _context.WorkOrders
                .Include(wo => wo.ServiceDoneBy)
                .Include(wo => wo.UpdatedBy)
                .Include(wo => wo.CreatedBy)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Mechanic)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Estimate)
                        .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Brand)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Model)
                .Where(wo => wo.Appointment.Id == appointmentId).FirstOrDefaultAsync();
        }

        public async Task<WorkOrder> GetWorkOrderByIdAsync(int id)
        {
            return await _context.WorkOrders                
                .Include(wo => wo.ServiceDoneBy)
                .Include(wo => wo.UpdatedBy)
                .Include(wo => wo.CreatedBy)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Mechanic)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Estimate)
                        .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Brand)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Model)
                .Where(wo => wo.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<WorkOrderChartDataModel>> GetWorkOrdersChartAsync(int month)
        {
            List<WorkOrderChartDataModel> list = new List<WorkOrderChartDataModel>();
            string[] status = new string[3] { "Closed", "Opened", "Done" };
            string[] color = new string[3] { "#990000", "#FFA500", "#9EB23B" };
            int id = 0;

            foreach(string statusItem in status)
            {
                var workOrders = await _context.WorkOrders.Where(wo => wo.Status == statusItem && wo.OrderDateStart.Month == month && wo.OrderDateStart.Year == DateTime.UtcNow.Year).ToListAsync();

                list.Add(new WorkOrderChartDataModel
                {
                    Status = statusItem,
                    Quantity = workOrders.Count(),
                    Color = color[id]
                });

                id++;
            }

            return list;
        }
    }
}
