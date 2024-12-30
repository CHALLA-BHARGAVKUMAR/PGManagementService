using System.Diagnostics.Metrics;
using System.DirectoryServices.Protocols;
using System.Reflection;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PGManagementService.BusinessLogic;
using PGManagementService.Controllers;
using PGManagementService.Data;
using PGManagementService.Data.DTO;
using PGManagementService.Helpers;
using PGManagementService.Interfaces;
using PGManagementService.Models;

namespace PGManagementService.BusinessLogic
{
    public class AdminBL : IAdminBL
    {
        private readonly PGManagementServiceDbContext _context;
        private readonly ILogger<AdminBL> _logger;
        private const string className = "AdminBL";
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminBL(PGManagementServiceDbContext context,ILogger<AdminBL> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            Console.WriteLine("AdminBL instance created!");
        }
        // Member Management
        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            return await _context.Members.Include(m => m.Room).ToListAsync();
        }

        public async Task<Member> GetMemberByIdAsync(int id)
        {
            return await _context.Members
                .Include(m => m.Room)
                .Include(m => m.Payments)
                .Include(m => m.Queries)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddMemberAsync(Member member)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;
            var room = _context.Rooms.Where(x => x.Id == member.RoomId).FirstOrDefault();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Members.Add(member);
                    await _context.SaveChangesAsync();
                    if (room.Occupancy < room.Capacity)
                    {
                        room.Occupancy += 1;
                    }
                    if (room.AvailableBeds > 0)
                    {
                        room.AvailableBeds -= 1;
                    }

                    _context.Rooms.Update(room);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"{className}: {methodName}  Exception : {ex}");
                }
            }
        }


        public async Task<ApiResponse> AddMemberAsync(MemberRequest memberRequest)
        {
            var apiResponse = new ApiResponse();
            var errList = new ErrorList();
            var methodName = MethodBase.GetCurrentMethod()?.Name;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                   

                    var room = await _context.Rooms.Where(x => x.Id == memberRequest.RoomId).FirstOrDefaultAsync();

                    if (room == null)
                    {
                        errList.ErrorCode = "RoomNotFound";
                        errList.ErrorDescription = "No Room Found"; 
                        apiResponse.Error=errList;
                        throw new Exception("Room not found"); 
                    }
                    else if (room.Occupancy == room.Capacity) {
                        errList.ErrorCode = "RoomFilled";
                        errList.ErrorDescription = "No Available Beds";
                        apiResponse.Error = errList;
                        throw new Exception("Total Room filled");
                    }

                    if (room.Occupancy < room.Capacity)
                    {
                        room.Occupancy++;
                    }


                    if (room.AvailableBeds > 0)
                    {
                        room.AvailableBeds--;
                    }

                    var member = memberRequest.MapTo<MemberRequest, Member>();

                    var addMembertoRoom = room.Members;
                    if(addMembertoRoom != null)
                    {
                        addMembertoRoom.Add(member);
                    }
                    else
                    {
                        addMembertoRoom = new List<Member>
                        {
                            member
                        };
                    }

                    _context.Entry(room).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                   

                    _context.Members.Add(member);

                    await _context.SaveChangesAsync();

                    // Create user
                    var user = new ApplicationUser
                    {
                        UserName = memberRequest.Email,
                        PhoneNumber = memberRequest.PhoneNumber
                    };

                    var defaultPassword = "DefaultPassword123!";
                    var createUserResult = await _userManager.CreateAsync(user, defaultPassword);

                    if (createUserResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    else
                    {
                        _logger.LogError($"{methodName}: Failed to create user. Errors: {createUserResult.Errors}");
                        apiResponse.Error = createUserResult.Errors;
                        throw new Exception("Failed to create user");
                        
                    }

                    transaction.Commit();
                    apiResponse.Result = true;
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"{methodName} Exception: {ex}");
                    return apiResponse;
                }
            }
        }
 

        public async Task UpdateMemberAsync(Member member)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Members.Update(member);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"{className}: {methodName}  Exception : {ex}");
                }
            }
        }

        public async Task<ApiResponse> DeleteMemberAsync(int id)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;
            var apiResponse = new ApiResponse();
            var errList = new ErrorList();
 
            using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var member = await _context.Members.FindAsync(id);

                        if(member == null)
                        {
                            errList.ErrorCode = "NotFound";
                            errList.ErrorDescription = "Member Id is Not Correct";
                            apiResponse.Error = errList;
                            throw new Exception("Not Found");
                        }

                        DeleteUserLoginDetails(new List<Member>
                                                {
                                                    member
                                                });

                        _context.Members.Remove(member);
                        await _context.SaveChangesAsync();

                        var room = _context.Rooms.Where(x => x.Id == member.RoomId).FirstOrDefault();

                        if (room.Occupancy > 0)
                        {
                            room.Occupancy -= 1;
                        }
                        if (room.AvailableBeds < room.Capacity)
                        {
                            room.AvailableBeds += 1;
                        }
                        _context.Entry(room).State = EntityState.Modified;
                        _context.Rooms.Update(room);
                        await _context.SaveChangesAsync();                        

                        await transaction.CommitAsync();
                        apiResponse.Result = true;
                        return apiResponse;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError($"{className}: {methodName}  Exception : {ex}");
                        return apiResponse;
                    }
            }



            
        }

        private async void DeleteUserLoginDetails(IList<Member> members)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                    try
                    {
                        foreach (var member in members)
                        {
                            var user = await _userManager.FindByNameAsync(member.Email);
                            var res = await _userManager.DeleteAsync(user);
                            if (res.Succeeded)
                            {
                                continue;
                            }
                            else
                            {
                                throw new Exception(res.Errors.ToString());
                            }
                        }
                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError($"AdminBL : DeleteUserLoginDetails Exception : {ex}");
                        throw;
                    }
            }         
        }

        #region Room Management
        public PaginatedResult<RoomResponse> GetAllRoomsAsync(PaginationRequestDto paginationRequest)
        {
            var paginatedData = _context.Rooms.GetPaginatedDataAsync(
                                pageNumber: paginationRequest.PageNumber,
                                pageSize: paginationRequest.PageSize,
                                sortBy: paginationRequest.SortBy,
                                sortDescending: paginationRequest.SortDescending,
                                selector: x => new RoomResponse
                                {
                                    Id = x.Id,
                                    RoomNo = x.RoomNumber,
                                    Capacity = x.Capacity,
                                    Occupied = x.Occupancy,
                                    Type = x.Type,
                                    BedsAvailable = x.AvailableBeds,
                                    Members = x.Members.ToList()
                                }
                            ).Result;

            return paginatedData;
        }

        public async Task AddRoomAsync(RoomRequest roomDto)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;

            var room = new Room
            {
                RoomNumber = roomDto.RoomNo,
                Type = roomDto.RoomType.ToString(),
                Capacity = (int)roomDto.RoomType,
                AvailableBeds = (int)roomDto.RoomType
                
            };
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Rooms.Add(room);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"{className}: {methodName}  Exception : {ex}");
                }
            }            
            
        }

        public bool DeleteRoom(int id)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name; 
            var result = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var room = _context.Rooms.Where(x=>x.Id == id).FirstOrDefault();
                    if(room!= null)
                    {
                        var members = room.Members;
                        if (members?.Any() == true)
                        {
                            DeleteUserLoginDetails(members.ToList());
                        }                        

                        _context.Rooms.Remove(room);
                    }
                    
                    result =_context.SaveChanges();

                    transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                     transaction.Rollback();
                    _logger.LogError($"{className}: {methodName}  Exception : {ex}");
                        
                }
            }
            return result > 0;
            



        }

        public async Task AddRoomAsyncApi(RoomRequest roomDto)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;
            Room room = new();
            room.RoomNumber = roomDto.RoomNo;
            room.Capacity = Convert.ToInt32(roomDto.RoomType);
            room.Type = roomDto.RoomType.ToString();
            room.AvailableBeds = room.Capacity;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Rooms.Add(room);
                    await _context.SaveChangesAsync();


                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"{className}: {methodName}  Exception : {ex}");
                }
            }

        }

        #endregion Room Management



        // Payment and Query Management
        public async Task<IEnumerable<Payment>> GetPaymentsByMemberAsync(int memberId)
        {
            return await _context.Payments.Where(p => p.MemberId == memberId).ToListAsync();
        }

        public async Task<IEnumerable<Query>> GetQueriesByMemberAsync(int memberId)
        {
            return await _context.Queries.Where(q => q.MemberId == memberId).ToListAsync();
        }

        public async Task RespondToQueryAsync(int queryId, string status, DateTime? resolvedDate)
        {
            var query = await _context.Queries.FindAsync(queryId);
            if (query != null)
            {
                query.Status = status;
                query.ResolvedDate = resolvedDate;
                await _context.SaveChangesAsync();
            }
        }


        public bool IsRoomNumberUnique(string roomNo)
        {
            return _context.Rooms.All(x => x.RoomNumber.ToLower() != roomNo.ToLower());
        }


        public async Task<List<Member>> GetAllHostlersAsync()
        {
            var members = await _context.Members.Include(m => m.Room).Include(m => m.Payments).ToListAsync();

            return members;
        }



    }
}



