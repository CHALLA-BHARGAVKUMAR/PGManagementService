using FluentValidation;
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
        private readonly IValidator<MemberRequest> _memberValidator;
        private readonly IValidator<RoomRequest> _roomValidator;

        public AdminApiController(IAdminBL adminBL, ILogger<AdminApiController> logger, IValidator<MemberRequest> memberValidator, IValidator<RoomRequest> roomValidator)
        {
            _adminBL = adminBL;
            _logger = logger;
            _memberValidator = memberValidator;
            _roomValidator = roomValidator;
        }

        #region Rooms CRUD

        [HttpGet("AllRooms")]
        public ActionResult<ApiResponse> AllRooms()
        {
            var rooms = _adminBL.GetAllRoomsAsync();

            if (rooms == null)
            {
                ErrorList errorList = new()
                {
                    ErrorCode = "NoRooms",
                    ErrorDescription = "Ro Rooms Available"
                };
                return BadRequest(new ApiResponse
                {
                    Error = errorList
                });
            }
            return Ok(new ApiResponse
            {
                Result = rooms
            });
        }

        [HttpPost("AddRoom")]
        public async Task<ActionResult<ApiResponse>> AddRoom([FromBody] RoomRequest roomRequest)
        {
            var validator = _roomValidator.Validate(roomRequest);
            if (!validator.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Error = validator.Errors
                });
            }

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

            var validator = await _memberValidator.ValidateAsync(memberRequest);
            if (!validator.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Error = validator.Errors
                });
            }
            var apiResponse = await _adminBL.AddMemberAsync(memberRequest);
            return Ok(apiResponse);
        }

        #endregion Members CRUD

    }




}

