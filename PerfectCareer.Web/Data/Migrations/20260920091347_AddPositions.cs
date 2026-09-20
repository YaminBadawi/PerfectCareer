using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace PerfectCareer.Web.Data.Migrations
{
    public partial class AddPositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(
                            type: "int",
                            nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    Title = table.Column<string>(
                        type: "nvarchar(160)",
                        maxLength: 160,
                        nullable: false),

                    Location = table.Column<string>(
                        type: "nvarchar(160)",
                        maxLength: 160,
                        nullable: false),

                    EmploymentType = table.Column<string>(
                        type: "nvarchar(60)",
                        maxLength: 60,
                        nullable: false),

                    Description = table.Column<string>(
                        type: "nvarchar(max)",
                        maxLength: 5000,
                        nullable: false),

                    IsActive = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    CreatedAtUtc = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false),

                    UpdatedAtUtc = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false),

                    RowVersion = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Positions",
                        x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionTechnologyTags",
                columns: table => new
                {
                    PositionId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    TechnologyTagId = table.Column<int>(
                        type: "int",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_PositionTechnologyTags",
                        x => new
                        {
                            x.PositionId,
                            x.TechnologyTagId
                        });

                    table.ForeignKey(
                        name: "FK_PositionTechnologyTags_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_PositionTechnologyTags_TechnologyTags_TechnologyTagId",
                        column: x => x.TechnologyTagId,
                        principalTable: "TechnologyTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionTemplateAttributes",
                columns: table => new
                {
                    PositionId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    AttributeDefinitionId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    DisplayOrder = table.Column<int>(
                        type: "int",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_PositionTemplateAttributes",
                        x => new
                        {
                            x.PositionId,
                            x.AttributeDefinitionId
                        });

                    table.ForeignKey(
                        name: "FK_PositionTemplateAttributes_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_PositionTemplateAttributes_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[]
                {
                    "Id",
                    "CreatedAtUtc",
                    "Description",
                    "EmploymentType",
                    "IsActive",
                    "Location",
                    "Title",
                    "UpdatedAtUtc"
                },
                values: new object[,]
                {
                    {
                        1,
                        new DateTimeOffset(
                            new DateTime(
                                2026, 9, 20, 8, 0, 0,
                                DateTimeKind.Unspecified),
                            TimeSpan.Zero),
                        "Build and maintain modern web applications using ASP.NET Core and Entity Framework Core.",
                        "Full-time",
                        true,
                        "Riyadh, Saudi Arabia",
                        "Junior ASP.NET Core Developer",
                        new DateTimeOffset(
                            new DateTime(
                                2026, 9, 20, 8, 0, 0,
                                DateTimeKind.Unspecified),
                            TimeSpan.Zero)
                    },
                    {
                        2,
                        new DateTimeOffset(
                            new DateTime(
                                2026, 9, 20, 9, 0, 0,
                                DateTimeKind.Unspecified),
                            TimeSpan.Zero),
                        "Create responsive, accessible user interfaces for the Perfect Career platform.",
                        "Full-time",
                        true,
                        "Remote",
                        "Frontend Developer",
                        new DateTimeOffset(
                            new DateTime(
                                2026, 9, 20, 9, 0, 0,
                                DateTimeKind.Unspecified),
                            TimeSpan.Zero)
                    }
                });

            migrationBuilder.InsertData(
                table: "PositionTemplateAttributes",
                columns: new[]
                {
                    "AttributeDefinitionId",
                    "PositionId",
                    "DisplayOrder"
                },
                values: new object[,]
                {
                    { 8, 1, 1 },
                    { 9, 1, 2 },
                    { 10, 1, 3 },
                    { 14, 1, 4 },
                    { 8, 2, 1 },
                    { 9, 2, 2 },
                    { 14, 2, 3 }
                });

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[TechnologyTags]
                    WHERE [Name] = N'C#'
                )
                BEGIN
                    INSERT INTO [dbo].[TechnologyTags] ([Name])
                    VALUES (N'C#');
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[TechnologyTags]
                    WHERE [Name] = N'ASP.NET Core'
                )
                BEGIN
                    INSERT INTO [dbo].[TechnologyTags] ([Name])
                    VALUES (N'ASP.NET Core');
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[TechnologyTags]
                    WHERE [Name] = N'SQL Server'
                )
                BEGIN
                    INSERT INTO [dbo].[TechnologyTags] ([Name])
                    VALUES (N'SQL Server');
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[TechnologyTags]
                    WHERE [Name] = N'JavaScript'
                )
                BEGIN
                    INSERT INTO [dbo].[TechnologyTags] ([Name])
                    VALUES (N'JavaScript');
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[TechnologyTags]
                    WHERE [Name] = N'React'
                )
                BEGIN
                    INSERT INTO [dbo].[TechnologyTags] ([Name])
                    VALUES (N'React');
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[TechnologyTags]
                    WHERE [Name] = N'CSS'
                )
                BEGIN
                    INSERT INTO [dbo].[TechnologyTags] ([Name])
                    VALUES (N'CSS');
                END;

                INSERT INTO [dbo].[PositionTechnologyTags]
                    ([PositionId], [TechnologyTagId])
                SELECT
                    1,
                    tag.[Id]
                FROM [dbo].[TechnologyTags] AS tag
                WHERE tag.[Name] IN (
                    N'C#',
                    N'ASP.NET Core',
                    N'SQL Server'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[PositionTechnologyTags] AS positionTag
                    WHERE positionTag.[PositionId] = 1
                      AND positionTag.[TechnologyTagId] = tag.[Id]
                );

                INSERT INTO [dbo].[PositionTechnologyTags]
                    ([PositionId], [TechnologyTagId])
                SELECT
                    2,
                    tag.[Id]
                FROM [dbo].[TechnologyTags] AS tag
                WHERE tag.[Name] IN (
                    N'JavaScript',
                    N'React',
                    N'CSS'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[PositionTechnologyTags] AS positionTag
                    WHERE positionTag.[PositionId] = 2
                      AND positionTag.[TechnologyTagId] = tag.[Id]
                );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_IsActive_UpdatedAtUtc",
                table: "Positions",
                columns: new[]
                {
                    "IsActive",
                    "UpdatedAtUtc"
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionTechnologyTags_TechnologyTagId",
                table: "PositionTechnologyTags",
                column: "TechnologyTagId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionTemplateAttributes_AttributeDefinitionId",
                table: "PositionTemplateAttributes",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionTemplateAttributes_PositionId_DisplayOrder",
                table: "PositionTemplateAttributes",
                columns: new[]
                {
                    "PositionId",
                    "DisplayOrder"
                },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionTechnologyTags");

            migrationBuilder.DropTable(
                name: "PositionTemplateAttributes");

            migrationBuilder.DropTable(
                name: "Positions");
        }
    }
}