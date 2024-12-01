using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagementService.Interfaces;
using PGManagementService.Models;

namespace PGManagementService.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminBL _adminBL;

        public AdminController(IAdminBL adminBL,ILogger<AdminController> logger)
        {
            _adminBL = adminBL;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _adminBL.GetAllMembersAsync();
            return View(members);
        }
        // Member Management
        [HttpGet]
        public async Task<IActionResult> Members()
        {
            var members = await _adminBL.GetAllMembersAsync();
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> AddMember()
        {
            ViewBag.Rooms = await _adminBL.GetAllRoomsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(Member member)
        {
            if (ModelState.IsValid)
            {
                await _adminBL.AddMemberAsync(member);
                return RedirectToAction("Members");
            }
            ViewBag.Rooms = await _adminBL.GetAllRoomsAsync();
            return View(member);
        }

        public async Task<IActionResult> EditMember(int id)
        {
            var member = await _adminBL.GetMemberByIdAsync(id);
            if (member == null) return NotFound();
            ViewBag.Rooms = await _adminBL.GetAllRoomsAsync();
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(Member member)
        {
            if (ModelState.IsValid)
            {
                await _adminBL.UpdateMemberAsync(member);
                return RedirectToAction("Members");
            }
            ViewBag.Rooms = await _adminBL.GetAllRoomsAsync();
            return View(member);
        }

        public async Task<IActionResult> DeleteMember(int id)
        {
            await _adminBL.DeleteMemberAsync(id);
            return RedirectToAction("Members");
        }

        [HttpGet]
        // Room Management
        public async Task<IActionResult> Rooms()
        {
            var rooms = await _adminBL.GetAllRoomsAsync();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult AddRoom()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                await _adminBL.AddRoomAsync(room);
                return RedirectToAction("Rooms");
            }
            return View(room);
        }
    }
}

