using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectCareer.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateAttributeUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CandidateAttributeUsages",
                columns: table => new
                {
                    CandidateProfileId = table.Column<int>(type: "int", nullable: false),
                    AttributeDefinitionId = table.Column<int>(type: "int", nullable: false),
                    LastUsedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateAttributeUsages", x => new { x.CandidateProfileId, x.AttributeDefinitionId });
                    table.ForeignKey(
                        name: "FK_CandidateAttributeUsages_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateAttributeUsages_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAttributeUsages_AttributeDefinitionId",
                table: "CandidateAttributeUsages",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAttributeUsages_CandidateProfileId_LastUsedAtUtc",
                table: "CandidateAttributeUsages",
                columns: new[] { "CandidateProfileId", "LastUsedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateAttributeUsages");
        }
    }
}
