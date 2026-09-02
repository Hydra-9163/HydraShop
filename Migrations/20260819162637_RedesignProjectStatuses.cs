using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MachineShopManager.Migrations
{
    /// <inheritdoc />
    public partial class RedesignProjectStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Status were persisted as enum integers. Remap old values before
            // the reduced enum is used: 0-3 -> EmFila, 4-8 -> remaining states.
            migrationBuilder.Sql("""
                UPDATE "Projects"
                SET "Status" = CASE "Status"
                    WHEN 0 THEN 0 WHEN 1 THEN 0 WHEN 2 THEN 0 WHEN 3 THEN 0
                    WHEN 4 THEN 1 WHEN 5 THEN 2 WHEN 6 THEN 3
                    WHEN 7 THEN 4 WHEN 8 THEN 5 ELSE "Status"
                END;

                UPDATE "ProjectHistories"
                SET "PreviousStatus" = CASE "PreviousStatus"
                    WHEN 0 THEN 0 WHEN 1 THEN 0 WHEN 2 THEN 0 WHEN 3 THEN 0
                    WHEN 4 THEN 1 WHEN 5 THEN 2 WHEN 6 THEN 3
                    WHEN 7 THEN 4 WHEN 8 THEN 5 ELSE "PreviousStatus"
                END,
                "NewStatus" = CASE "NewStatus"
                    WHEN 0 THEN 0 WHEN 1 THEN 0 WHEN 2 THEN 0 WHEN 3 THEN 0
                    WHEN 4 THEN 1 WHEN 5 THEN 2 WHEN 6 THEN 3
                    WHEN 7 THEN 4 WHEN 8 THEN 5 ELSE "NewStatus"
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "Projects"
                SET "Status" = CASE "Status"
                    WHEN 0 THEN 0 WHEN 1 THEN 4 WHEN 2 THEN 5
                    WHEN 3 THEN 6 WHEN 4 THEN 7 WHEN 5 THEN 8 ELSE "Status"
                END;

                UPDATE "ProjectHistories"
                SET "PreviousStatus" = CASE "PreviousStatus"
                    WHEN 0 THEN 0 WHEN 1 THEN 4 WHEN 2 THEN 5
                    WHEN 3 THEN 6 WHEN 4 THEN 7 WHEN 5 THEN 8 ELSE "PreviousStatus"
                END,
                "NewStatus" = CASE "NewStatus"
                    WHEN 0 THEN 0 WHEN 1 THEN 4 WHEN 2 THEN 5
                    WHEN 3 THEN 6 WHEN 4 THEN 7 WHEN 5 THEN 8 ELSE "NewStatus"
                END;
                """);
        }
    }
}
