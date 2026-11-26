using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHybridApp2.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionAttendanceAndDiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SectionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StudentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RecordDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsPresent = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_attendance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "student_diary_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SectionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StudentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RecordDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Entry = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_diary_entries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_SectionId_RecordDate",
                table: "student_attendance",
                columns: new[] { "SectionId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_SectionId_StudentId_RecordDate",
                table: "student_attendance",
                columns: new[] { "SectionId", "StudentId", "RecordDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_diary_entries_SectionId_RecordDate",
                table: "student_diary_entries",
                columns: new[] { "SectionId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_student_diary_entries_SectionId_StudentId_RecordDate",
                table: "student_diary_entries",
                columns: new[] { "SectionId", "StudentId", "RecordDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_attendance");

            migrationBuilder.DropTable(
                name: "student_diary_entries");
        }
    }
}
