using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "CreatedDate", "IsDeleted", "ManagerId", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Phòng Công nghệ", null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Phòng Nhân sự", null }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "BaseSalary", "CreatedDate", "IsDeleted", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 50000000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Giám đốc kỹ thuật", null },
                    { 2, 30000000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Trưởng phòng IT", null },
                    { 3, 15000000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Backend Developer", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "Description", "IsDeleted", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quản trị hệ thống", false, "Admin", null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ban Giám đốc", false, "Director", null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trưởng phòng", false, "Manager", null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nhân viên", false, "Employee", null }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "CreatedDate", "DepartmentId", "DoB", "Email", "FullName", "Gender", "IsDeleted", "JoinDate", "Phone", "PositionId", "Status", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@hrflow.com", "Nguyễn Minh Đức", "Male", false, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "Active", null },
                    { 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "director@hrflow.com", "Phạm Khánh Huyền", "Female", false, new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "Active", null },
                    { 3, null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "tuan.ma@hrflow.com", "Mai Anh Tuấn", "Male", false, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, "Active", null },
                    { 4, null, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "staff@hrflow.com", "Lê Nhân Viên", "Female", false, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, "Active", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Email", "EmployeeId", "IsActive", "IsDeleted", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedDate", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@hrflow.com", 1, true, false, "$2a$11$mC8ZfV6/5S8H1L7K1K6G8.K7K6G8.K7K6G8.K7K6G8.K7K6G8.", null, null, null, "admin" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "director@hrflow.com", 2, true, false, "$2a$11$mC8ZfV6/5S8H1L7K1K6G8.K7K6G8.K7K6G8.K7K6G8.K7K6G8.", null, null, null, "director" },
                    { 3, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager@hrflow.com", 3, true, false, "$2a$11$mC8ZfV6/5S8H1L7K1K6G8.K7K6G8.K7K6G8.K7K6G8.K7K6G8.", null, null, null, "manager" },
                    { 4, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "staff@hrflow.com", 4, true, false, "$2a$11$mC8ZfV6/5S8H1L7K1K6G8.K7K6G8.K7K6G8.K7K6G8.K7K6G8.", null, null, null, "staff" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 2 },
                    { 3, 3, 3 },
                    { 4, 4, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");
        }
    }
}
