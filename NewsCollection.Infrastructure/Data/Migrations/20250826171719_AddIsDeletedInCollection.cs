using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsCollection.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedInCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Collections",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Collections");
        }
    }
}
