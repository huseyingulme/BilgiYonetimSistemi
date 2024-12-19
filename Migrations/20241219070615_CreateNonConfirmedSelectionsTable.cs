using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilgiYonetimSistemi.Migrations
{
    /// <inheritdoc />
    public partial class CreateNonConfirmedSelectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseQuotas_Courses_CourseID",
                table: "CourseQuotas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentCourseSelections",
                table: "StudentCourseSelections");

            migrationBuilder.DropIndex(
                name: "IX_StudentCourseSelections_CourseID",
                table: "StudentCourseSelections");

            migrationBuilder.RenameColumn(
                name: "CourseID",
                table: "CourseQuotas",
                newName: "CourseId");

            migrationBuilder.RenameColumn(
                name: "RemainingCapacity",
                table: "CourseQuotas",
                newName: "RemainingQuota");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "CourseQuotas",
                newName: "Quota");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SelectionID",
                table: "StudentCourseSelections",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentCourseSelections",
                table: "StudentCourseSelections",
                columns: new[] { "CourseID", "StudentID" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transcripts_CourseID",
                table: "Transcripts",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Transcripts_StudentID",
                table: "Transcripts",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseQuotas_Courses_CourseId",
                table: "CourseQuotas",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transcripts_Courses_CourseID",
                table: "Transcripts",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transcripts_Students_StudentID",
                table: "Transcripts",
                column: "StudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseQuotas_Courses_CourseId",
                table: "CourseQuotas");

            migrationBuilder.DropForeignKey(
                name: "FK_Transcripts_Courses_CourseID",
                table: "Transcripts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transcripts_Students_StudentID",
                table: "Transcripts");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Transcripts_CourseID",
                table: "Transcripts");

            migrationBuilder.DropIndex(
                name: "IX_Transcripts_StudentID",
                table: "Transcripts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentCourseSelections",
                table: "StudentCourseSelections");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "CourseQuotas",
                newName: "CourseID");

            migrationBuilder.RenameColumn(
                name: "RemainingQuota",
                table: "CourseQuotas",
                newName: "RemainingCapacity");

            migrationBuilder.RenameColumn(
                name: "Quota",
                table: "CourseQuotas",
                newName: "Capacity");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "SelectionID",
                table: "StudentCourseSelections",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentCourseSelections",
                table: "StudentCourseSelections",
                column: "SelectionID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseSelections_CourseID",
                table: "StudentCourseSelections",
                column: "CourseID");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseQuotas_Courses_CourseID",
                table: "CourseQuotas",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
