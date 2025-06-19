using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.Seo.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Seo_Initial_BrokenLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrokenLink",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Permalink = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    StoreId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RedirectUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    HitCount = table.Column<int>(type: "int", nullable: false),
                    LastHitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokenLink", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrokenLink_Permalink_StoreId_LanguageCode",
                table: "BrokenLink",
                columns: new[] { "Permalink", "StoreId", "LanguageCode" },
                unique: true,
                filter: "[StoreId] IS NOT NULL AND [Permalink] IS NOT NULL AND [LanguageCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrokenLink");
        }
    }
}
