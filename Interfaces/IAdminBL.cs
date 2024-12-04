using PGManagementService.Data.DTO;
using PGManagementService.Models;

namespace PGManagementService.Interfaces
{
    public interface IAdminBL
    {
        Task<IEnumerable<Member>> GetAllMembersAsync();
        Task<Member> GetMemberByIdAsync(int id);
        Task AddMemberAsync(Member member);
        Task UpdateMemberAsync(Member member);
        Task DeleteMemberAsync(int id);

        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task AddRoomAsync(Room room);

        public Task AddRoomAsyncApi(RoomDTO roomDto);

        Task<IEnumerable<Payment>> GetPaymentsByMemberAsync(int memberId);
        Task<IEnumerable<Query>> GetQueriesByMemberAsync(int memberId);
        Task RespondToQueryAsync(int queryId, string status, DateTime? resolvedDate);
        bool IsRoomNumberUnique(string roomNo);
    }
}
