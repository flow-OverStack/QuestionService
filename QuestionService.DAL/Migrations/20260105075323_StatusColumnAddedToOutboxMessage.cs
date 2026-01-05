using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StatusColumnAddedToOutboxMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OutboxMessage",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_OutboxMessage_ProcessedAt_Status",
                table: "OutboxMessage",
                sql: "(\"Status\" = 2 AND \"ProcessedAt\" IS NOT NULL) OR \"Status\" <> 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OutboxMessage_Status_Enum",
                table: "OutboxMessage",
                sql: "\"Status\" IN (0,1,2,3)");
            
            migrationBuilder.Sql("""
                                 UPDATE public."OutboxMessage"
                                 SET "Status" = CASE
                                    WHEN "ProcessedAt" IS NOT NULL THEN 2 
                                    WHEN "RetryCount" IS NOT NULL THEN 1 
                                 ELSE 0                                 
                                 END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OutboxMessage_ProcessedAt_Status",
                table: "OutboxMessage");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OutboxMessage_Status_Enum",
                table: "OutboxMessage");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OutboxMessage");
        }
    }
}
