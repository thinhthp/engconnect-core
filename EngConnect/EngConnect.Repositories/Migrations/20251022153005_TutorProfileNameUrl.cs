using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngConnect.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class TutorProfileNameUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nickname",
                table: "tutor_profile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profile_picture_url",
                table: "tutor_profile",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "tutor_id",
                table: "course",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nickname",
                table: "tutor_profile");

            migrationBuilder.DropColumn(
                name: "profile_picture_url",
                table: "tutor_profile");

            migrationBuilder.AlterColumn<string>(
                name: "tutor_id",
                table: "course",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
