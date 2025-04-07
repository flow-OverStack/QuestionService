using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ViewEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "Question");

            migrationBuilder.CreateTable(
                name: "View",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    UserIp = table.Column<string>(type: "text", nullable: true),
                    UserFingerprint = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View", x => x.Id);
                    table.CheckConstraint("CK_View_UserId_Or_UserIpAndFingerprint", "(UserId IS NOT NULL OR (UserFingerprint IS NOT NULL AND UserIp IS NOT NULL))");
                    table.ForeignKey(
                        name: "FK_View_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_View_QuestionId",
                table: "View",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "View");

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Question",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
