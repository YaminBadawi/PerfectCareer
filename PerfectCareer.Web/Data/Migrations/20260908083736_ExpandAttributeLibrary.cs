using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PerfectCareer.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandAttributeLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AttributeDefinitions",
                columns: new[] { "Id", "Category", "DataType", "Description", "IsBuiltIn", "Name" },
                values: new object[,]
                {
                    { 8, 3, 2, "A concise overview of the candidate's professional background and career goals.", false, "Professional Summary" },
                    { 9, 2, 1, "The candidate's GitHub profile URL or username.", false, "GitHub Profile" },
                    { 10, 2, 4, "The candidate's total years of professional experience.", false, "Years of Experience" },
                    { 11, 3, 5, "The date when the candidate is available to start a new position.", false, "Available Start Date" },
                    { 12, 2, 6, "The start and end dates of the candidate's most relevant experience.", false, "Relevant Experience Period" },
                    { 13, 3, 7, "Whether the candidate is willing to relocate for a position.", false, "Open to Relocation" },
                    { 14, 1, 8, "The candidate's overall English proficiency level.", false, "English Level" }
                });

            migrationBuilder.InsertData(
                table: "AttributeOptions",
                columns: new[] { "Id", "AttributeDefinitionId", "Label" },
                values: new object[,]
                {
                    { 4, 14, "Beginner" },
                    { 5, 14, "Intermediate" },
                    { 6, 14, "Upper-Intermediate" },
                    { 7, 14, "Advanced" },
                    { 8, 14, "Fluent" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AttributeOptions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AttributeOptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AttributeOptions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AttributeOptions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AttributeOptions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AttributeDefinitions",
                keyColumn: "Id",
                keyValue: 14);
        }
    }
}
