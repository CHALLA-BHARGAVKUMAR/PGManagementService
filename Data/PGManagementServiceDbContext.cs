using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PGManagementService.Models;
using Query = PGManagementService.Models.Query;

namespace PGManagementService.Data
{

    public class PGManagementServiceDbContext : IdentityDbContext<ApplicationUser>
    {
        public PGManagementServiceDbContext(DbContextOptions<PGManagementServiceDbContext> options)
        : base(options)
        {
            Console.WriteLine("DbContext instance created!");
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UtilityBill> UtilityBills { get; set; }
        public DbSet<Query> Queries { get; set; }
        public DbSet<Report> Reports { get; set; }
    }

} 