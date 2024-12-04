using System.Reflection;
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

        public AdminBL(PGManagementServiceDbContext context,ILogger<AdminBL> logger)
        {
            _context = context;
            _logger = logger;
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

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Members.Add(member);
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
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Members.Remove(member);
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
        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms.Include(r => r.Members).ToListAsync();
        }

        public async Task AddRoomAsync(Room room)
        {
            var methodName = MethodBase.GetCurrentMethod()?.Name;

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
    }
}



