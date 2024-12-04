using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PGManagementService.Data.DTO;
using PGManagementService.Interfaces;
using PGManagementService.Models;

namespace PGManagementService.Controllers
{

    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : Controller
    {
        private readonly ILogger<AdminApiController> _logger;
        private readonly IAdminBL _adminBL;

        public AdminApiController(IAdminBL adminBL,ILogger<AdminApiController> logger)
        {
            _adminBL = adminBL;
            _logger = logger;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var members = await _adminBL.GetAllMembersAsync();
        //    return View(members);
        //}
        //// Member Management
        //[HttpGet]
        //public async Task<IActionResult> Members()
        //{
        //    var members = await _adminBL.GetAllMembersAsync();
        //    return View(members);
        //}

        //[HttpGet]
        //public async Task<IActionResult> AddMember()
        //{
        //    ViewBag.Rooms = await _adminBL.GetAllRoomsAsync();
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddMember(Member member)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _adminBL.AddMemberAsync(member);
        //        return RedirectToAction("Members");
        //    }
        //    ViewBag.Rooms = await _adminBL.GetAllRoomsAsync();
        //    return View(member);
        //}

        //public async Task<IActionResult> EditMember(int id)
        //{
        //    var member = await _adminBL.GetMemberByIdAsync(id);
        //    if (member == null) return NotFound();
        //    ViewBag.Rooms = await _adminBL.GetAllRoomsAsync();
        //    return View(member);
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditMember(Member member)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _adminBL.UpdateMemberAsync(member);
        //        return RedirectToAction("Members");
        //    }
        //    ViewBag.Rooms = await _adminBL.GetAllRoomsAsync();
        //    return View(member);
        //}

        //public async Task<IActionResult> DeleteMember(int id)
        //{
        //    await _adminBL.DeleteMemberAsync(id);
        //    return RedirectToAction("Members");
        //}

        [HttpGet]
        public async Task<IActionResult> Rooms()
        {
            var rooms = await _adminBL.GetAllRoomsAsync();
            return Ok(rooms);
        }

        //[HttpGet]
        //public IActionResult AddRoom()
        //{
        //    return View();
        //}

        [HttpPost("AddRoom")]
        public async Task<ActionResult<ApiResponse>> AddRoom(RoomDTO roomDto)
        {
            if(roomDto == null || string.IsNullOrWhiteSpace(roomDto.RoomNo) || string.IsNullOrEmpty(roomDto.RoomNo))
            {
                ErrorList errorList = new()
                {
                    ErrorCode = "NotValidData",
                    ErrorDescription = "Not valid RoomNo"
                };
                return BadRequest(new ApiResponse
                {
                    Error = errorList
                });
            }
            if (!_adminBL.IsRoomNumberUnique(roomDto.RoomNo))
            {
                ErrorList errorList = new()
                {
                    ErrorCode = "SameRoomNo",
                    ErrorDescription = "RoomNo should be unique"
                };
                return BadRequest(new ApiResponse
                {
                    Error = errorList
                });
            }
            await _adminBL.AddRoomAsyncApi(roomDto);
            return Ok(new ApiResponse
            {
                Result = true
            });
        }
    }
}

