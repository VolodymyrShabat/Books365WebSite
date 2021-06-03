using Microsoft.EntityFrameworkCore.Migrations;

namespace Books365WebSite.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__PagesRead__BookI__45F365D3",
                table: "PagesRead");

            migrationBuilder.DropForeignKey(
                name: "FK__PagesRead__UserI__44FF419A",
                table: "PagesRead");

            migrationBuilder.DropForeignKey(
                name: "FK__ReadingSt__BookI__4316F928",
                table: "ReadingStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_ReadingStatuses_AspNetUsers_UserId",
                table: "ReadingStatuses");

            migrationBuilder.DropTable(
                name: "AuthorBook");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropIndex(
                name: "IX_ReadingStatuses_BookId",
                table: "ReadingStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ReadingStatuses_UserId",
                table: "ReadingStatuses");

            migrationBuilder.DropIndex(
                name: "IX_PagesRead_BookId",
                table: "PagesRead");

            migrationBuilder.DropIndex(
                name: "IX_PagesRead_UserId",
                table: "PagesRead");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Book");

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.AuthorId);
                });

            migrationBuilder.CreateTable(
                name: "AuthorBook",
                columns: table => new
                {
                    AuthorsAuthorId = table.Column<int>(type: "int", nullable: false),
                    BooksIsbn = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorBook", x => new { x.AuthorsAuthorId, x.BooksIsbn });
                    table.ForeignKey(
                        name: "FK_AuthorBook_Author_AuthorsAuthorId",
                        column: x => x.AuthorsAuthorId,
                        principalTable: "Author",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorBook_Book_BooksIsbn",
                        column: x => x.BooksIsbn,
                        principalTable: "Book",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadingStatuses_BookId",
                table: "ReadingStatuses",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingStatuses_UserId",
                table: "ReadingStatuses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PagesRead_BookId",
                table: "PagesRead",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_PagesRead_UserId",
                table: "PagesRead",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorBook_BooksIsbn",
                table: "AuthorBook",
                column: "BooksIsbn");

            migrationBuilder.AddForeignKey(
                name: "FK__PagesRead__BookI__45F365D3",
                table: "PagesRead",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__PagesRead__UserI__44FF419A",
                table: "PagesRead",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ReadingSt__BookI__4316F928",
                table: "ReadingStatuses",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReadingStatuses_AspNetUsers_UserId",
                table: "ReadingStatuses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
