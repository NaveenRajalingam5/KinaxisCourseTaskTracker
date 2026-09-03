using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinaxisCourseTaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLessonPrerequisite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Lessons_PrerequisiteLessonId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_PrerequisiteLessonId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "PrerequisiteLessonId",
                table: "Lessons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrerequisiteLessonId",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_PrerequisiteLessonId",
                table: "Lessons",
                column: "PrerequisiteLessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Lessons_PrerequisiteLessonId",
                table: "Lessons",
                column: "PrerequisiteLessonId",
                principalTable: "Lessons",
                principalColumn: "Id");
        }
    }
}
