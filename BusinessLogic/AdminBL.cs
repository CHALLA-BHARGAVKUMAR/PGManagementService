using Microsoft.EntityFrameworkCore;
using PGManagementService.BusinessLogic;
using PGManagementService.Data;
using PGManagementService.Interfaces;
using PGManagementService.Models;

namespace PGManagementService.BusinessLogic
{
    public class AdminBL : IAdminBL
    {
        private readonly PGManagementServiceDbContext _context;

        public AdminBL(PGManagementServiceDbContext context)
        {
            _context = context;
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
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMemberAsync(Member member)
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMemberAsync(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        // Room Management
        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms.Include(r => r.Members).ToListAsync();
        }

        public async Task AddRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
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
    }
}



