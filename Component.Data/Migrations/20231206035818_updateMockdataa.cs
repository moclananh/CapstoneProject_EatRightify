using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Component.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateMockdataa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"));

            migrationBuilder.DeleteData(
                table: "AppUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"), new Guid("69bd714f-9576-45ba-b5b7-f00649be00de") });

            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"));

            migrationBuilder.InsertData(
                table: "AppRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { new Guid("46f889a9-662d-4969-84f3-6ff4e199ecf5"), null, "Administrator role", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "AppUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("46f889a9-662d-4969-84f3-6ff4e199ecf5"), new Guid("93510e19-8812-482f-8f1b-e116cf8c9e38") });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("1ec8cb63-dc7e-492c-83b2-d02dc476061c"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3f6001e6-b76d-45f9-a381-10b0eb387b9c", "AQAAAAIAAYagAAAAEKYlTX3lrTURiw5kM5eomJw6tJ758J1mDlvvp51lVfyKc65zeYeZziQkFvklJDzQ+A==" });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("648d9797-a78f-4e71-bf5d-90196c3f4806"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "66c54c3f-c4a6-40b6-9ccf-68c3c41b5033", "AQAAAAIAAYagAAAAEMu7lPPwRktrE8QGhP7VJd4YwTOEVTIeZCj2o418cl4+antkD+8ueuS2jC/nPGBavg==" });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "AccessFailedCount", "AccumulatedPoints", "ConcurrencyStamp", "Dob", "Email", "EmailConfirmed", "FirstName", "IsBanned", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "VIP" },
                values: new object[] { new Guid("93510e19-8812-482f-8f1b-e116cf8c9e38"), 0, null, "ade241be-d444-4ddf-af98-5a9db16205d6", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@admin.com", true, "Admin", false, "minator", false, null, "admin@admin.com", "admin", "AQAAAAIAAYagAAAAEI1L/rjEqx8FQBKiHBvyDsaxqKzOyfWrjombTvjikQ13XYOouo1WU0Wdxepware8jA==", null, false, "", false, "admin", null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2023, 12, 6, 10, 58, 17, 887, DateTimeKind.Local).AddTicks(4326));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("46f889a9-662d-4969-84f3-6ff4e199ecf5"));

            migrationBuilder.DeleteData(
                table: "AppUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("46f889a9-662d-4969-84f3-6ff4e199ecf5"), new Guid("93510e19-8812-482f-8f1b-e116cf8c9e38") });

            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("93510e19-8812-482f-8f1b-e116cf8c9e38"));

            migrationBuilder.InsertData(
                table: "AppRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"), null, "Administrator role", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "AppUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"), new Guid("69bd714f-9576-45ba-b5b7-f00649be00de") });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("1ec8cb63-dc7e-492c-83b2-d02dc476061c"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7d6dc4dc-4c34-4634-86fe-229d6658c766", "AQAAAAIAAYagAAAAEIrK/AGWHIK6LAt917/w7Eb1psb9n9McQjczfrew7SUuG0HLIhr96IJ5wlEo+dgeJA==" });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("648d9797-a78f-4e71-bf5d-90196c3f4806"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e88d0d3e-7896-4c56-a671-19638ebed346", "AQAAAAIAAYagAAAAEFd6f0+v3XKKNAHEudDF6rUi6RS4NKNb1kEK5vgG+fRS+iGRc0llNV6rSES6/6Xy5g==" });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "AccessFailedCount", "AccumulatedPoints", "ConcurrencyStamp", "Dob", "Email", "EmailConfirmed", "FirstName", "IsBanned", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "VIP" },
                values: new object[] { new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"), 0, null, "9d8032cb-d03c-4341-af9f-d0dc6883b6f2", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@admin.com", true, "Admin", false, "minator", false, null, "admin@admin.com", "admin", "AQAAAAIAAYagAAAAEMEhszd4mlYEp0eWf+B5PXZCaxNP3iJeskhj5xE97Uuteg2GnH9tj+n7Uo6JCtC44Q==", null, false, "", false, "admin", null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2023, 12, 6, 10, 56, 5, 905, DateTimeKind.Local).AddTicks(3786));
        }
    }
}
