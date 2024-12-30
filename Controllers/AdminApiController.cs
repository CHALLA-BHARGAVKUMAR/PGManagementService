using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagementService.Data.DTO;
using PGManagementService.Interfaces;


namespace PGManagementService.Controllers
{

    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : Controller
    {
        private readonly ILogger<AdminApiController> _logger;
        private readonly IAdminBL _adminBL;


        public AdminApiController(IAdminBL adminBL, ILogger<AdminApiController> logger)
        {
            _adminBL = adminBL;
            _logger = logger;
        }

        #region Rooms CRUD

        [HttpGet("AllRooms")]
        public ActionResult<ApiResponse> AllRooms([FromQuery] PaginationRequestDto paginationRequest)
        {

            var rooms = _adminBL.GetAllRoomsAsync(paginationRequest);
            return Ok(rooms);

        }

        [HttpPost("AddRoom")]
        public async Task<ActionResult<ApiResponse>> AddRoom([FromBody] RoomRequest roomRequest)
        {


            if (!_adminBL.IsRoomNumberUnique(roomRequest.RoomNo))
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
            await _adminBL.AddRoomAsyncApi(roomRequest);
            return Ok(new ApiResponse
            {
                Result = true
            });
        }

        [HttpDelete("DeleteRoom")]
        public async Task<ActionResult<ApiResponse>> DeleteRoom(int roomId)
        {

            var result = _adminBL.DeleteRoom(roomId);
            return Ok(new ApiResponse
            {
                Result = result
            });
        }
        #endregion Rooms CRUD

        #region Members CRUD

        [HttpPost("AddMember")]
        public async Task<ActionResult<ApiResponse>> AddMember([FromBody] MemberRequest memberRequest)
        {
            var apiResponse = await _adminBL.AddMemberAsync(memberRequest);
            return Ok(apiResponse);
        }

        [HttpDelete("DeleteMember")]
        public async Task<ActionResult<ApiResponse>> DeleteMember(int memberId)
        {

            var result = await _adminBL.DeleteMemberAsync(memberId);
            return Ok(result);
        }

        #endregion Members CRUD

    }




}

