using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PGManagementService.Data.DTO;
using PGManagementService.Interfaces;
using PGManagementService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PGManagementService.ViewComponents
{
    public class AdminDashboardViewComponent : ViewComponent
    {
        private readonly IAdminBL _adminBl;

        public AdminDashboardViewComponent(IAdminBL adminBl)
        {
            _adminBl = adminBl;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var members = await _adminBl.GetAllHostlersAsync();
            var totalHostlers = members.Count; 
            var paymentsDue = members.SelectMany(m => m.Payments).Count(p =>!p.IsPaid); 
            var paymentsCompleted = members.SelectMany(m => m.Payments).Count(p => p.IsPaid); 
            var occupiedBeds = members.Count; 
            var availableBeds = members.Sum(m => m.Room.Capacity) - occupiedBeds;
            var allHostlers = new AdminDashboardViewResponse
            {
                TotalHostlers = totalHostlers,
                PaymentsDue = paymentsDue,
                PaymentsCompleted = paymentsCompleted,
                OccupiedBeds = occupiedBeds,
                AvailableBeds = availableBeds
            };
            return View(allHostlers);
        }
    }
}



