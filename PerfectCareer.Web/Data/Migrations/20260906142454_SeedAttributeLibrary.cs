using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PerfectCareer.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAttributeLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AttributeDefinitions",
                columns: new[] { "Id", "Category", "DataType", "Description", "IsBuiltIn", "Name" },
                values: new object[,]
                {
                    { 1, 3, 1, "Candidate's first name.", true, "First Name" },
                    { 2, 3, 1, "Candidate's last name.", true, "Last Name" },
                    { 3, 3, 1, "Candidate's current location.", true, "Location" },
                    { 4, 3, 3, "Candidate's personal photo.", true, "Personal Photo" },
                    { 5, 1, 4, "Candidate's IELTS score.", false, "IELTS Score" },
                    { 6, 4, 8, "Candidate's presentation skill level.", false, "Presentation Skills" },
                    { 7, 3, 7, "Whether the candidate is available for remote work.", false, "Remote Work Availability" }
                });

            migrationBuilder.InsertData(
                table: "AttributeOptions",
                columns: new[] { "Id", "AttributeDefinitionId", "Label" },
                values: new object[,]
                {
                    { 1, 6, "Beginner" },
                    { 2, 6, "Intermediate" },
                    { 3, 6, "Advanced" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AttributeOptions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AttributeOptions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AttributeOptions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
