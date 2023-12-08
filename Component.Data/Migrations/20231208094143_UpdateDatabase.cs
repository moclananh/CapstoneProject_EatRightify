using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Component.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfDisLike",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "NumberOfLike",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderCode",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("1ec8cb63-dc7e-492c-83b2-d02dc476061c"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cfb3bd83-c757-41d3-8636-5b96e6e7a3ae", "AQAAAAIAAYagAAAAEDSJdNvQ21kCGVeLuEA6/AtHQr6mpg6U4wR8bwyDL2CZ9el1oYZwEP9X9aVZhrpYQQ==" });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("648d9797-a78f-4e71-bf5d-90196c3f4806"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c4d63fca-1fa2-4aee-a41c-56d2116bc814", "AQAAAAIAAYagAAAAEFNp4FRBVxAXPPiFLnPqVi3glGsYRRHKEEe6MRbfRm34S+uyoXoK8i/XPkzCTdWbAg==" });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("93510e19-8812-482f-8f1b-e116cf8c9e38"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0e164dfe-0385-4775-942d-f28e749cd19c", "AQAAAAIAAYagAAAAEHE5h6N88ldspsS4ftFN+E/EAUlnHHj8ahYfIZDI5WVB7o9XAOeFpXBGUQKW/MshRw==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2023, 12, 8, 16, 41, 42, 898, DateTimeKind.Local).AddTicks(4046));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDisLike",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfLike",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("1ec8cb63-dc7e-492c-83b2-d02dc476061c"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a79156e8-12be-4dee-92da-d793baec4788", "AQAAAAIAAYagAAAAED2sXzZqkVgEKmHpDy+YLT+iOw4+D8Dd9r2/LMqOc4/uLfGtkqbeKiYxuz2IX3orjg==" });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("648d9797-a78f-4e71-bf5d-90196c3f4806"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "86d30d3e-1c70-4ee2-bf81-12efb4937fee", "AQAAAAIAAYagAAAAEL9OEase97aJ/Rvtrl+XnhwFU2inWOvgDBNpic1XMbsCT4K8bkf93fhJTDkWjva+0w==" });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("93510e19-8812-482f-8f1b-e116cf8c9e38"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c8477fb8-42a3-49fe-b8c2-d258c3319d3b", "AQAAAAIAAYagAAAAEJeZ9zq10SAbYp0fEkTKvLPHVWiMck4Lo0oFg3rpNjawd928a/dncc/M46dXwT2V2A==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2023, 12, 6, 17, 10, 11, 167, DateTimeKind.Local).AddTicks(8057));
        }
    }
}
