using System.Diagnostics;
using System.Diagnostics.Metrics;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PGManagementService.Data.DTO;
using PGManagementService.Interfaces;
using PGManagementService.Models;

namespace PGManagementService.Controllers
{
    //[Authorize(Roles = "Admin")]
    //public class AdminController : Controller
    //{
    //    private readonly ILogger<AdminController> _logger;
    //    private readonly IAdminBL _adminBL;
    //    private readonly UserManager<ApplicationUser> _userManager;
    //    private readonly SignInManager<ApplicationUser> _signInManager;
    //    private readonly IValidator<RoomRequest> _validator;

    //    public AdminController(IAdminBL adminBL,ILogger<AdminController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IValidator<RoomRequest> validator)
    //    {
    //        _adminBL = adminBL;
    //        _logger = logger;
    //        _userManager = userManager;
    //        _signInManager = signInManager;
    //        _validator = validator;
    //    }



    //    // Member Management
    //    [HttpGet]
    //    public async Task<IActionResult> Members()
    //    {
    //        var members = await _adminBL.GetAllMembersAsync();
    //        return View(members);
    //    }

    //    [HttpGet]
    //    public async Task<IActionResult> AddMember()
    //    {
    //        ViewBag.Rooms = _adminBL.GetAllRoomsAsync();
    //        return View();
    //    }
         
    //    [HttpPost]
    //    public async Task<IActionResult> AddMember(Member member)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            // Create a new ApplicationUser using the phone number as the UserName
    //            var user = new ApplicationUser
    //            {
    //                UserName = member.Email, // Use phone number as the username
    //                PhoneNumber = member.PhoneNumber,
    //            };

    //            // Default password (you can customize this)
    //            var defaultPassword = "DefaultPassword123!";  // Set a default password for first login

    //            // Create the user with the default password
    //            var result = await _userManager.CreateAsync(user, defaultPassword);

    //            if (result.Succeeded)
    //            {
    //                // You can add roles if necessary
    //                await _userManager.AddToRoleAsync(user, "User");

    //                // Add the member to your internal system
    //                await _adminBL.AddMemberAsync(member);

    //                return RedirectToAction("Members");

    //            }
    //            else
    //            {
    //                // Add errors to ModelState if user creation failed
    //                foreach (var error in result.Errors)
    //                {
    //                    ModelState.AddModelError(string.Empty, error.Description);
    //                }
    //            }
    //        }

    //        // If model is invalid, return the view
    //        ViewBag.Rooms = _adminBL.GetAllRoomsAsync();
    //         return View(member);
    //    } 



    //    public async Task<IActionResult> EditMember(int id)
    //    {
    //        var member = await _adminBL.GetMemberByIdAsync(id);
    //        if (member == null) return NotFound();
    //        ViewBag.Rooms =  _adminBL.GetAllRoomsAsync();
    //        return View(member);
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> EditMember(Member member)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            await _adminBL.UpdateMemberAsync(member);
    //            return RedirectToAction("Members");
    //        }
    //        ViewBag.Rooms = _adminBL.GetAllRoomsAsync();
    //        return View(member);
    //    }

    //    public async Task<IActionResult> DeleteMember(int id)
    //    {
    //        await _adminBL.DeleteMemberAsync(id);
    //        return RedirectToAction("Members");
    //    }


    //    #region  Room Management

    //    [HttpGet]
    //    public async Task<IActionResult> Rooms()
    //    {
    //        var rooms =  _adminBL.GetAllRoomsAsync();

    //        return View(rooms);
    //    }

    //    [HttpGet]
    //    public IActionResult AddRoom()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> AddRoom(RoomRequest room)
    //    {
    //        // FluentValidation will automatically add errors to ModelState if validation fails
    //        var validationResult = _validator.Validate(room);

    //        if (!validationResult.IsValid)
    //        {
    //            // This will automatically populate the ModelState with errors
    //            foreach (var error in validationResult.Errors)
    //            {
    //                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    //            }            
    //        }
    //        await _adminBL.AddRoomAsync(room);
    //        return RedirectToAction("Rooms");// Return the view with validation errors
    //    }

    //    public async Task<IActionResult> DeleteRoom(int id)
    //    {
    //        _adminBL.DeleteRoom(id);
    //        return RedirectToAction("Rooms");
    //    }

    //    #endregion  Room Management
    //}
}

