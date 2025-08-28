using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsCollection.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFrequencyToUserSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionFrequency",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "UserSubscriptions",
                type: "text",
                nullable: false,
                defaultValue: "daily");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "UserSubscriptions");

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionFrequency",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "daily");
        }
    }
}
