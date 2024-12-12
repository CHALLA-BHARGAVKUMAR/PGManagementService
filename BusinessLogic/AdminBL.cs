using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PGManagementService.BusinessLogic;
using PGManagementService.Controllers;
using PGManagementService.Data;
using PGManagementService.Data.DTO;
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

        public async Task DeleteMemberAsync(int id)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                var room = _context.Rooms.Where(x => x.Id == member.RoomId).FirstOrDefault();
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Members.Remove(member);
                        await _context.SaveChangesAsync();
                        if (room.Occupancy > 0)
                        {
                            room.Occupancy -= 1;
                        }
                        if (room.AvailableBeds < room.Capacity)
                        {
                            room.AvailableBeds += 1;
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


            
        }

        // Room Management
        public async Task<IEnumerable<RoomResponse>> GetAllRoomsAsync()
        {
            var rooms = _context.Rooms
                                 .Select(x => new
                                 {
                                     x.Id,
                                     x.RoomNumber,
                                     x.Capacity,
                                     x.Type,
                                     x.Occupancy,
                                     x.AvailableBeds
                                 })
                                 .Select(x => new RoomResponse
                                 {
                                     Id = x.Id,
                                     RoomNo = x.RoomNumber,
                                     Capacity = x.Capacity,
                                     Occupied = x.Occupancy,
                                     Type = x.Type,
                                     BedsAvailable = x.AvailableBeds
                                 })
                                 .AsEnumerable();


            if (rooms.Any())
            {
                return rooms;
            }


            return null;
        }

        public async Task AddRoomAsync(RoomDTO roomDto)
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

        public async Task DeleteRoomAsync(int id)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Rooms.Remove(room);
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



        }

        public async Task AddRoomAsyncApi(RoomDTO roomDto)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;
            Room room = new();
            room.RoomNumber = roomDto.RoomNo;
            room.Capacity = Convert.ToInt32(roomDto.RoomType);
            room.Type = roomDto.RoomType.ToString();
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



