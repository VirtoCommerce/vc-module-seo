using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.Seo.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class Seo_RedirectRule_PriorityAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "RedirectRule",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RedirectRuleType",
                table: "RedirectRule",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "RedirectRule");

            migrationBuilder.DropColumn(
                name: "RedirectRuleType",
                table: "RedirectRule");
        }
    }
}
