using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectCareer.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CandidateProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateProjects", x => x.Id);
                    table.CheckConstraint("CK_CandidateProjects_Period", "[EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_CandidateProjects_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnologyTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnologyTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateProjectTechnologyTags",
                columns: table => new
                {
                    CandidateProjectId = table.Column<int>(type: "int", nullable: false),
                    TechnologyTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateProjectTechnologyTags", x => new { x.CandidateProjectId, x.TechnologyTagId });
                    table.ForeignKey(
                        name: "FK_CandidateProjectTechnologyTags_CandidateProjects_CandidateProjectId",
                        column: x => x.CandidateProjectId,
                        principalTable: "CandidateProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateProjectTechnologyTags_TechnologyTags_TechnologyTagId",
                        column: x => x.TechnologyTagId,
                        principalTable: "TechnologyTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProjects_CandidateProfileId",
                table: "CandidateProjects",
                column: "CandidateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProjectTechnologyTags_TechnologyTagId",
                table: "CandidateProjectTechnologyTags",
                column: "TechnologyTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnologyTags_Name",
                table: "TechnologyTags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateProjectTechnologyTags");

            migrationBuilder.DropTable(
                name: "CandidateProjects");

            migrationBuilder.DropTable(
                name: "TechnologyTags");
        }
    }
}
