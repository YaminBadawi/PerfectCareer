using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectCareer.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePhotoPublicId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonalPhotoPublicId",
                table: "CandidateProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalPhotoPublicId",
                table: "CandidateProfiles");
        }
    }
}
