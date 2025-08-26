﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsCollection.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveContentFieldInArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
