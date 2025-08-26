using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsCollection.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdjustCollectionArticleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionArticles_Articles_ArticlesId",
                table: "CollectionArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_CollectionArticles_Collections_CollectionsId",
                table: "CollectionArticles");

            migrationBuilder.RenameColumn(
                name: "CollectionsId",
                table: "CollectionArticles",
                newName: "ArticleId");

            migrationBuilder.RenameColumn(
                name: "ArticlesId",
                table: "CollectionArticles",
                newName: "CollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_CollectionArticles_CollectionsId",
                table: "CollectionArticles",
                newName: "IX_CollectionArticles_ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionArticles_Articles_ArticleId",
                table: "CollectionArticles",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionArticles_Collections_CollectionId",
                table: "CollectionArticles",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionArticles_Articles_ArticleId",
                table: "CollectionArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_CollectionArticles_Collections_CollectionId",
                table: "CollectionArticles");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "CollectionArticles",
                newName: "CollectionsId");

            migrationBuilder.RenameColumn(
                name: "CollectionId",
                table: "CollectionArticles",
                newName: "ArticlesId");

            migrationBuilder.RenameIndex(
                name: "IX_CollectionArticles_ArticleId",
                table: "CollectionArticles",
                newName: "IX_CollectionArticles_CollectionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionArticles_Articles_ArticlesId",
                table: "CollectionArticles",
                column: "ArticlesId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionArticles_Collections_CollectionsId",
                table: "CollectionArticles",
                column: "CollectionsId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
