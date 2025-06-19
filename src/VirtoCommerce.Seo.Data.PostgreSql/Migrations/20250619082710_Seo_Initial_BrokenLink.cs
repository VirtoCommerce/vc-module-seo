using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.Seo.Data.PostgreSql.Migrations
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
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    HitCount = table.Column<int>(type: "integer", nullable: false),
                    Permalink = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    StoreId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RedirectUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LanguageCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastAttemptTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokenLink", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrokenLink_StoreId_Permalink_LanguageCode",
                table: "BrokenLink",
                columns: new[] { "StoreId", "Permalink", "LanguageCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrokenLink");
        }
    }
}
