using PGManagementService.Data.enums;

namespace PGManagementService.Data.DTO
{
    public class PaginationRequestDto
    {
        public int PageNumber { get; set; } = 1;      
        public int PageSize { get; set; } = 10;       
        public string SortBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; } = false;



    }   
}       
        
        