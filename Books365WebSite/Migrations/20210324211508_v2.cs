using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Books365WebSite.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Book",
                columns: table => new
                {
                    ISBN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Pages = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISBN", x => x.ISBN);
                });

            migrationBuilder.CreateTable(
                name: "Books_Authors",
                columns: table => new
                {
                    AuthorId = table.Column<int>(type: "int", nullable: true),
                    BookId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__Books_Aut__Autho__267ABA7A",
                        column: x => x.AuthorId,
                        principalTable: "Author",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Books_Aut__BookI__276EDEB3",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagesRead",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    DateStarted = table.Column<DateTime>(type: "datetime", nullable: true),
                    PagesRead = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__PagesRead__BookI__45F365D3",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__PagesRead__UserI__44FF419A",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReadingStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ReadingSt__BookI__4316F928",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReadingStatuses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_Authors_AuthorId",
                table: "Books_Authors",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Authors_BookId",
                table: "Books_Authors",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_PagesRead_BookId",
                table: "PagesRead",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_PagesRead_UserId",
                table: "PagesRead",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingStatuses_BookId",
                table: "ReadingStatuses",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingStatuses_UserId",
                table: "ReadingStatuses",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books_Authors");

            migrationBuilder.DropTable(
                name: "PagesRead");

            migrationBuilder.DropTable(
                name: "ReadingStatuses");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropTable(
                name: "Book");
        }
    }
}
