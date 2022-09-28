using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.Threading.Tasks;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;

        public ServicesController(IServiceRepository serviceRepository
                                 ,IFlashMessage flashMessage
                                 ,IConverterHelper converterHelper
            )
        {
            _serviceRepository = serviceRepository;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
        }

        public IActionResult Index()
        {           
           return View(_serviceRepository.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Discount = model.Discount,
                };

                try
                {
                    await _serviceRepository.CreateAsync(service);
                    _flashMessage.Confirmation("The service was created with success.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was a problem creating the service. "+ex.InnerException.Message);
                    return View(model);
                }                               
            }

            return View(model);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var service = await _serviceRepository.GetByIdAsync(id.Value);

            if(service == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToServiceViewModel(service);

            if(model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = _converterHelper.ToService(model, false);

                if(service == null)
                {
                    _flashMessage.Danger("There was an error updating the service.");
                    return View(model);
                }

                try
                {
                    await _serviceRepository.UpdateAsync(service);
                    _flashMessage.Confirmation("The service was updated with success.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error updating the service.");
                    return View(model);                    
                }               

            }

            return View(model);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var service = await _serviceRepository.GetByIdAsync(id.Value);

            if (service == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToServiceViewModel(service);

            if(model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _serviceRepository.GetByIdAsync(id.Value);

            if (service == null)
            {
                return NotFound();
            }

            try
            {
                await _serviceRepository.DeleteAsync(service);
                _flashMessage.Confirmation("The service was deleted with success.");               
            }
            catch (Exception ex)
            {
                _flashMessage.Danger("There was an error deleting the service. "+ ex.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
