using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EngConnect.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class coursereview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_review",
                columns: table => new
                {
                    course_review_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    learner_id = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("course_review_pkey", x => x.course_review_id);
                    table.ForeignKey(
                        name: "course_review_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "course_review_learner_id_fkey",
                        column: x => x.learner_id,
                        principalTable: "application_user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_review_course_id",
                table: "course_review",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_review_learner_id",
                table: "course_review",
                column: "learner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_review");
        }
    }
}
