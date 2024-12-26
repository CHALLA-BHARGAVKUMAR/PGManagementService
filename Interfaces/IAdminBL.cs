using PGManagementService.Data.DTO;
using PGManagementService.Models;

namespace PGManagementService.Interfaces
{
    public interface IAdminBL
    {
        Task<IEnumerable<Member>> GetAllMembersAsync();
        Task<Member> GetMemberByIdAsync(int id);
        Task AddMemberAsync(Member member);
        Task<ApiResponse> AddMemberAsync(MemberRequest memberRequest);
        Task UpdateMemberAsync(Member member);
        Task DeleteMemberAsync(int id);

        bool DeleteRoom(int id);

        IEnumerable<RoomResponse> GetAllRoomsAsync();
        Task AddRoomAsync(RoomRequest room);

        public Task AddRoomAsyncApi(RoomRequest roomDto);

        Task<IEnumerable<Payment>> GetPaymentsByMemberAsync(int memberId);
        Task<IEnumerable<Query>> GetQueriesByMemberAsync(int memberId);
        Task RespondToQueryAsync(int queryId, string status, DateTime? resolvedDate);
        bool IsRoomNumberUnique(string roomNo);

        Task<List<Member>> GetAllHostlersAsync();
    }
}
