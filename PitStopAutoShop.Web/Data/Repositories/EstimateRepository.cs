using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class EstimateRepository : GenericRepository<Estimate>, IEstimateRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public EstimateRepository(DataContext context, IUserHelper userHelper,
            ICustomerRepository customerRepository, IVehicleRepository vehicleRepository) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<Response> ConfirmEstimateAsync(string username,int customerId,int vehicleId,string faultdescription)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);

            if(user == null)
            {
                return new Response { IsSuccess = false }; 
            }

            var estimateTemps = await _context.EstimateDetailTemps.Include(e => e.Service).Where(e => e.CustomerId == customerId && e.VehicleId == vehicleId).ToListAsync();

            if(estimateTemps == null || estimateTemps.Count == 0)
            {
                return new Response { IsSuccess = false }; 
            }

            var customer = await _customerRepository.GetCustomerWithUserByIdAsync(customerId);

            if(customer == null)
            {
                return new Response { IsSuccess = false }; 
            }

            var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(vehicleId);

            if(vehicle == null)
            {
                return new Response { IsSuccess = false }; 
            }

            var details = estimateTemps.Select(e => new EstimateDetail
            {
                Price = e.Price,
                Service = e.Service,
                Quantity = e.Quantity,
                CustomerId = e.CustomerId,
                VehicleId = e.VehicleId
            }).ToList();

            var estimate = new Estimate
            {
                EstimateDate = DateTime.UtcNow,
                CreatedBy = user,
                Services = details,
                Customer = customer,
                Vehicle = vehicle,
                HasAppointment = false,
                FaultDescription = faultdescription
            };

           
            await CreateAsync(estimate);
            
            
            _context.EstimateDetailTemps.RemoveRange(estimateTemps);
            await _context.SaveChangesAsync();
            return new Response { IsSuccess = true };
        }

        public async Task CreateEstimateDetailTemp(EstimateDetailTemp estimateDetailTemp)
        {
            _context.EstimateDetailTemps.Add(estimateDetailTemp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var estimateDetailTemp = await _context.EstimateDetailTemps.FindAsync(id);

            if(estimateDetailTemp == null)
            {
                return;
            }

            _context.EstimateDetailTemps.Remove(estimateDetailTemp);
            await _context.SaveChangesAsync();
        }

        public IQueryable GetAllEstimates()
        {
            return _context.Estimates.Include(e => e.Customer)
                                     .Include(e => e.Vehicle)
                                     .Include(e => e.CreatedBy)
                                     .Include(e => e.Services)
                                     .ThenInclude(es => es.Service)
                                     .OrderBy(d => d.EstimateDate);
        }

        public async Task<IQueryable<EstimateDetailTemp>> GetDetailTempsAsync(int vehicleId,int customerId)
        {      
            var detailtemps =  _context.EstimateDetailTemps.Include(s => s.Service).Where(o => o.CustomerId == customerId && o.VehicleId == vehicleId).OrderBy(o => o.Service.Name);
            return detailtemps;
        }

        public async Task<Estimate> GetEstimateWithDetailsByIdAsync(int value)
        {
            if(value == 0)
            {
                return null;
            }

            var estimate = await _context.Estimates.Include(e => e.Vehicle).ThenInclude(e => e.Brand).ThenInclude(e => e.Models)
                                    //.Include(e => e.Vehicle).ThenInclude(e=> e.Model)
                                    .Include(e => e.Customer)
                                    .Include(e=>e.Services).ThenInclude(e => e.Service)
                                    .Where(ed => ed.Id == value).FirstAsync();

            if(estimate == null)
            {
                return null;
            }

            return estimate;
        }

        public async Task<EstimateDetailTemp> GetEstimateDetailTempAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if(user == null)
            {
                return null;
            }

            var estimate = await _context.EstimateDetailTemps.Where(edt => edt.User == user).FirstOrDefaultAsync();

            if (estimate == null)
            {
                return null;
            }

            return estimate;
        }

        public async Task<EstimateDetailTemp> GetEstimateDetailTempByIdAsync(int id)
        {
            var estimate = await _context.EstimateDetailTemps.Where(edt => edt.Id == id).FirstOrDefaultAsync();
            return estimate;
        }

        public async Task<EstimateDetailTemp> GetEstimateDetailTempWithVehicleIdAsync(string name, Vehicle vehicle)
        {
            var user = await _userHelper.GetUserByEmailAsync(name);

            if (user == null)
            {
                return null;
            }

            var estimate = await _context.EstimateDetailTemps.Where(edt => edt.User == user && edt.VehicleId == vehicle.Id).FirstAsync();

            if (estimate == null)
            {
                return null;
            }

            return estimate;
        }

        public async Task ModifyEstimateDetailTempQuantityAsync(int id, double quantity)
        {
            var estimateDetailTemp = await _context.EstimateDetailTemps.FindAsync(id);

            if(estimateDetailTemp == null)
            {
                return;
            }

            estimateDetailTemp.Quantity += quantity;
            if(estimateDetailTemp.Quantity > 0)
            {
                _context.EstimateDetailTemps.Update(estimateDetailTemp);
                await _context.SaveChangesAsync();
            }


        }

        public async Task<int> DeleteEstimateDetailsAsync(int id)
        {
            if(id == 0)
            {
                 return 0;
            }

            var estimateDetails = await _context.EstimateDetails.Where(ed => ed.EstimateId == id).ToListAsync();

            if(estimateDetails == null)
            {
                return 0;
            }

            try
            {
                _context.RemoveRange(estimateDetails);
                await _context.SaveChangesAsync();
                return estimateDetails.Count;
            }
            catch (Exception)
            {
                return 0;
            }          
        }

        public async Task CreateEstimatesDetailsTemps(IEnumerable<EstimateDetailTemp> estimateDetailTemps)
        {
          await _context.EstimateDetailTemps.AddRangeAsync(estimateDetailTemps);
          await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteEstimateDetailTempsAsync(int vehicleId, int customerId)
        {
            var temps = await _context.EstimateDetailTemps.Where(edt => edt.VehicleId == vehicleId && edt.CustomerId == customerId).ToListAsync();

            if(temps == null)
            {
                return 0;
            }

            try
            {
                _context.EstimateDetailTemps.RemoveRange(temps);
                await _context.SaveChangesAsync();
                return temps.Count;
            }
            catch 
            {
                return 0;                
            }            
        }

        public async Task<Response> UpdateEstimateAsync(string username, int customerId, int vehicleId, string faultdescription)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);

            if (user == null)
            {
                return new Response { IsSuccess = false};
            }

            var estimateTemps = await _context.EstimateDetailTemps.Include(e => e.Service).Where(e => e.CustomerId == customerId && e.VehicleId == vehicleId).ToListAsync();

            if (estimateTemps == null || estimateTemps.Count == 0)
            {
                return new Response { IsSuccess = false };
            }

            var customer = await _customerRepository.GetCustomerWithUserByIdAsync(customerId);

            if (customer == null)
            {
                return new Response { IsSuccess = false };
            }

            var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(vehicleId);

            if (vehicle == null)
            {
                return new Response { IsSuccess = false };
            }

            var estimate = await _context.Estimates
                .Include(e => e.Services)
                .Include(e => e.CreatedBy)
                .Include(e => e.Customer)
                .Include(e => e.Vehicle)
                .Where(e => e.Id == estimateTemps.FirstOrDefault().EstimateId).FirstAsync();

            if(estimate == null)
            {
                return new Response { IsSuccess = false };
            }

            int nOfDeletedEstimateDetails = 0;

            try
            {
                _context.EstimateDetails.RemoveRange(estimate.Services);
                nOfDeletedEstimateDetails = estimate.Services.Count();
            }
            catch 
            {
                return new Response { IsSuccess = false };
            }            

            var details = estimateTemps.Select(e => new EstimateDetail
            {
                Price = e.Price,
                Service = e.Service,
                Quantity = e.Quantity,
                CustomerId = e.CustomerId,
                VehicleId = e.VehicleId
            }).ToList();

            estimate.Services = details;
            estimate.FaultDescription = faultdescription;
            
            await UpdateAsync(estimate);

            bool asWorkOrder = false;
           
            try
            {
                var appointment = await _context.Appointments.Where(e => e.Estimate.Id == estimate.Id).FirstAsync();
                var workOrder = await _context.WorkOrders.Where(wo => wo.Appointment.Id == appointment.Id).FirstAsync();
                if (workOrder != null)
                {
                    asWorkOrder = true;
                }
            }
            catch 
            {
                asWorkOrder= false;
            }         
                       


            _context.EstimateDetailTemps.RemoveRange(estimateTemps);
            await _context.SaveChangesAsync();
            return new Response { IsSuccess = true, Results = asWorkOrder };
        }

        public async Task<Estimate> GetCreatedEstimateAsync(string userId, int customerId, int vehicleId)
        {
            var user = await _userHelper.GetUserByEmailAsync(userId);

            if(user == null)
            {
                return null;
            }

            var estimate = await _context.Estimates
                .Include(e => e.Customer).Include(e => e.Vehicle)
                .Where(e => e.Customer.Id == customerId && e.Vehicle.Id == vehicleId && e.CreatedBy.Id == user.Id).OrderBy(e => e.Id).LastAsync();

            return estimate;
        }
    }
}
