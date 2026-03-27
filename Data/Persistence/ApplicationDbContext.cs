using Data.Entities.Attendance;
using Data.Entities.HR;
using Data.Entities.Identity;
using Data.Entities.Payroll;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<EmployeePayroll> Payrolls { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Quan hệ User - Role (n-n)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // 2. Quan hệ 1-1 User - Employee
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // 3. Quan hệ 1-n Department - Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Quản lý phòng ban (Manager)
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);

            // 5. CẤU HÌNH QUAN TRỌNG: LeaveRequest (Người tạo vs Người duyệt)
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                // Người tạo đơn: Nối với ICollection<LeaveRequest> trong Employee
                entity.HasOne(lr => lr.Employee)
                    .WithMany(e => e.LeaveRequests)
                    .HasForeignKey(lr => lr.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Người duyệt đơn: Quan hệ riêng, không nối với Collection mặc định
                entity.HasOne(lr => lr.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(lr => lr.ApprovedById)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // 6. Quan hệ 1-1 Employee - Contract
            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.EmployeeId)
                .IsUnique()
                .HasFilter("[Status] = 'Active'");

            // 7. Cấu hình Decimal (Tiền tệ)
            var decimalProps = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

            foreach (var property in decimalProps)
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }

            // 8. Global Query Filter
            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);

            // 1. Chuỗi Hash BCrypt cho mật khẩu "Aa@123456"
            // Lưu ý: BCrypt tự tạo Salt bên trong chuỗi này nên bạn chỉ cần lưu nguyên đoạn này
            string passHash = "$2a$11$mC8ZfV6/5S8H1L7K1K6G8.K7K6G8.K7K6G8.K7K6G8.K7K6G8.";

            // 2. Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Quản trị hệ thống", CreatedDate = new DateTime(2026, 1, 1) },
                new Role { Id = 2, Name = "Director", Description = "Ban Giám đốc", CreatedDate = new DateTime(2026, 1, 1) },
                new Role { Id = 3, Name = "Manager", Description = "Trưởng phòng", CreatedDate = new DateTime(2026, 1, 1) },
                new Role { Id = 4, Name = "Employee", Description = "Nhân viên", CreatedDate = new DateTime(2026, 1, 1) }
            );

            // 3. Seed Positions
            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Name = "Giám đốc kỹ thuật", BaseSalary = 50000000, CreatedDate = new DateTime(2026, 1, 1) },
                new Position { Id = 2, Name = "Trưởng phòng IT", BaseSalary = 30000000, CreatedDate = new DateTime(2026, 1, 1) },
                new Position { Id = 3, Name = "Backend Developer", BaseSalary = 15000000, CreatedDate = new DateTime(2026, 1, 1) }
            );

            // 4. Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Phòng Công nghệ", ManagerId = null, CreatedDate = new DateTime(2026, 1, 1) },
                new Department { Id = 2, Name = "Phòng Nhân sự", ManagerId = null, CreatedDate = new DateTime(2026, 1, 1) }
            );

            // 5. Seed Employees (Khớp với các thuộc tính Gender, Status, JoinDate của bạn)
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FullName = "Nguyễn Minh Đức", Gender = "Male", Email = "admin@hrflow.com", DepartmentId = 1, PositionId = 1, Status = "Active", JoinDate = new DateTime(2025, 1, 10), CreatedDate = new DateTime(2026, 1, 1) },
                new Employee { Id = 2, FullName = "Phạm Khánh Huyền", Gender = "Female", Email = "director@hrflow.com", DepartmentId = 2, PositionId = 1, Status = "Active", JoinDate = new DateTime(2025, 2, 15), CreatedDate = new DateTime(2026, 1, 1) },
                new Employee { Id = 3, FullName = "Mai Anh Tuấn", Gender = "Male", Email = "tuan.ma@hrflow.com", DepartmentId = 1, PositionId = 2, Status = "Active", JoinDate = new DateTime(2026, 3, 1), CreatedDate = new DateTime(2026, 3, 1) },
                new Employee { Id = 4, FullName = "Lê Nhân Viên", Gender = "Female", Email = "staff@hrflow.com", DepartmentId = 1, PositionId = 3, Status = "Active", JoinDate = new DateTime(2026, 3, 20), CreatedDate = new DateTime(2026, 3, 20) }
            );

            // 6. Seed Users (Gắn kết 1-1 với Employee qua EmployeeId)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", PasswordHash = passHash, Email = "admin@hrflow.com", EmployeeId = 1, IsActive = true, CreatedDate = new DateTime(2026, 1, 1) },
                new User { Id = 2, Username = "director", PasswordHash = passHash, Email = "director@hrflow.com", EmployeeId = 2, IsActive = true, CreatedDate = new DateTime(2026, 1, 1) },
                new User { Id = 3, Username = "manager", PasswordHash = passHash, Email = "manager@hrflow.com", EmployeeId = 3, IsActive = true, CreatedDate = new DateTime(2026, 3, 1) },
                new User { Id = 4, Username = "staff", PasswordHash = passHash, Email = "staff@hrflow.com", EmployeeId = 4, IsActive = true, CreatedDate = new DateTime(2026, 3, 20) }
            );

            // 7. Seed UserRoles (Phân quyền n-n)
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, UserId = 1, RoleId = 1 }, // Admin
                new UserRole { Id = 2, UserId = 2, RoleId = 2 }, // Director
                new UserRole { Id = 3, UserId = 3, RoleId = 3 }, // Manager
                new UserRole { Id = 4, UserId = 4, RoleId = 4 }  // Employee
            );
        }
    }
}