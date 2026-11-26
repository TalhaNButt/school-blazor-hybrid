using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHybridApp2.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolScopeToSectionRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_student_diary_entries_SectionId_RecordDate",
                table: "student_diary_entries");

            migrationBuilder.DropIndex(
                name: "IX_student_diary_entries_SectionId_StudentId_RecordDate",
                table: "student_diary_entries");

            migrationBuilder.DropIndex(
                name: "IX_student_attendance_SectionId_RecordDate",
                table: "student_attendance");

            migrationBuilder.DropIndex(
                name: "IX_student_attendance_SectionId_StudentId_RecordDate",
                table: "student_attendance");

            migrationBuilder.AddColumn<string>(
                name: "SchoolId",
                table: "student_diary_entries",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "oakridge-school");

            migrationBuilder.AddColumn<string>(
                name: "SchoolId",
                table: "student_attendance",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "oakridge-school");

            migrationBuilder.CreateIndex(
                name: "IX_student_diary_entries_SchoolId_SectionId_RecordDate",
                table: "student_diary_entries",
                columns: new[] { "SchoolId", "SectionId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_student_diary_entries_SchoolId_SectionId_StudentId_RecordDa~",
                table: "student_diary_entries",
                columns: new[] { "SchoolId", "SectionId", "StudentId", "RecordDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_SchoolId_SectionId_RecordDate",
                table: "student_attendance",
                columns: new[] { "SchoolId", "SectionId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_SchoolId_SectionId_StudentId_RecordDate",
                table: "student_attendance",
                columns: new[] { "SchoolId", "SectionId", "StudentId", "RecordDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_student_diary_entries_SchoolId_SectionId_RecordDate",
                table: "student_diary_entries");

            migrationBuilder.DropIndex(
                name: "IX_student_diary_entries_SchoolId_SectionId_StudentId_RecordDa~",
                table: "student_diary_entries");

            migrationBuilder.DropIndex(
                name: "IX_student_attendance_SchoolId_SectionId_RecordDate",
                table: "student_attendance");

            migrationBuilder.DropIndex(
                name: "IX_student_attendance_SchoolId_SectionId_StudentId_RecordDate",
                table: "student_attendance");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "student_diary_entries");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "student_attendance");

            migrationBuilder.CreateIndex(
                name: "IX_student_diary_entries_SectionId_RecordDate",
                table: "student_diary_entries",
                columns: new[] { "SectionId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_student_diary_entries_SectionId_StudentId_RecordDate",
                table: "student_diary_entries",
                columns: new[] { "SectionId", "StudentId", "RecordDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_SectionId_RecordDate",
                table: "student_attendance",
                columns: new[] { "SectionId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_SectionId_StudentId_RecordDate",
                table: "student_attendance",
                columns: new[] { "SectionId", "StudentId", "RecordDate" },
                unique: true);
        }
    }
}
