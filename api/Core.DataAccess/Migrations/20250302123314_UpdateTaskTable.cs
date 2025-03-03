using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_task_task_categories_task_category_id",
                table: "task");

            migrationBuilder.DropPrimaryKey(
                name: "pk_task",
                table: "task");

            migrationBuilder.RenameTable(
                name: "task",
                newName: "tasks");

            migrationBuilder.RenameIndex(
                name: "ix_task_task_category_id",
                table: "tasks",
                newName: "ix_tasks_task_category_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tasks",
                table: "tasks",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_task_categories_task_category_id",
                table: "tasks",
                column: "task_category_id",
                principalTable: "task_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tasks_task_categories_task_category_id",
                table: "tasks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tasks",
                table: "tasks");

            migrationBuilder.RenameTable(
                name: "tasks",
                newName: "task");

            migrationBuilder.RenameIndex(
                name: "ix_tasks_task_category_id",
                table: "task",
                newName: "ix_task_task_category_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_task",
                table: "task",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_task_task_categories_task_category_id",
                table: "task",
                column: "task_category_id",
                principalTable: "task_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
