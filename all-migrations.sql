IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'00000000000000_CreateIdentitySchema', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903084223_AddCandidateProfile'
)
BEGIN
    CREATE TABLE [CandidateProfiles] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Location] nvarchar(150) NOT NULL,
        [PersonalPhotoUrl] nvarchar(2048) NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_CandidateProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CandidateProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903084223_AddCandidateProfile'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CandidateProfiles_UserId] ON [CandidateProfiles] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260903084223_AddCandidateProfile'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260903084223_AddCandidateProfile', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906141532_AddAttributeLibrary'
)
BEGIN
    CREATE TABLE [AttributeDefinitions] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [Category] int NOT NULL,
        [DataType] int NOT NULL,
        [IsBuiltIn] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_AttributeDefinitions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906141532_AddAttributeLibrary'
)
BEGIN
    CREATE TABLE [AttributeOptions] (
        [Id] int NOT NULL IDENTITY,
        [AttributeDefinitionId] int NOT NULL,
        [Label] nvarchar(150) NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_AttributeOptions] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_AttributeOptions_AttributeDefinitionId_Id] UNIQUE ([AttributeDefinitionId], [Id]),
        CONSTRAINT [FK_AttributeOptions_AttributeDefinitions_AttributeDefinitionId] FOREIGN KEY ([AttributeDefinitionId]) REFERENCES [AttributeDefinitions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906141532_AddAttributeLibrary'
)
BEGIN
    CREATE TABLE [CandidateAttributeValues] (
        [Id] int NOT NULL IDENTITY,
        [CandidateProfileId] int NOT NULL,
        [AttributeDefinitionId] int NOT NULL,
        [TextValue] nvarchar(max) NULL,
        [NumberValue] decimal(18,4) NULL,
        [DateValue] date NULL,
        [PeriodStart] date NULL,
        [PeriodEnd] date NULL,
        [BooleanValue] bit NULL,
        [SelectedOptionId] int NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_CandidateAttributeValues] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CandidateAttributeValues_AttributeDefinitions_AttributeDefinitionId] FOREIGN KEY ([AttributeDefinitionId]) REFERENCES [AttributeDefinitions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CandidateAttributeValues_AttributeOptions_AttributeDefinitionId_SelectedOptionId] FOREIGN KEY ([AttributeDefinitionId], [SelectedOptionId]) REFERENCES [AttributeOptions] ([AttributeDefinitionId], [Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CandidateAttributeValues_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906141532_AddAttributeLibrary'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AttributeDefinitions_Name] ON [AttributeDefinitions] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906141532_AddAttributeLibrary'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AttributeOptions_AttributeDefinitionId_Label] ON [AttributeOptions] ([AttributeDefinitionId], [Label]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906141532_AddAttributeLibrary'
)
BEGIN
    CREATE INDEX [IX_CandidateAttributeValues_AttributeDefinitionId_SelectedOptionId] ON [CandidateAttributeValues] ([AttributeDefinitionId], [SelectedOptionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906141532_AddAttributeLibrary'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CandidateAttributeValues_CandidateProfileId_AttributeDefinitionId] ON [CandidateAttributeValues] ([CandidateProfileId], [AttributeDefinitionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906141532_AddAttributeLibrary'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260906141532_AddAttributeLibrary', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906142454_SeedAttributeLibrary'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Category', N'DataType', N'Description', N'IsBuiltIn', N'Name') AND [object_id] = OBJECT_ID(N'[AttributeDefinitions]'))
        SET IDENTITY_INSERT [AttributeDefinitions] ON;
    EXEC(N'INSERT INTO [AttributeDefinitions] ([Id], [Category], [DataType], [Description], [IsBuiltIn], [Name])
    VALUES (1, 3, 1, N''Candidate''''s first name.'', CAST(1 AS bit), N''First Name''),
    (2, 3, 1, N''Candidate''''s last name.'', CAST(1 AS bit), N''Last Name''),
    (3, 3, 1, N''Candidate''''s current location.'', CAST(1 AS bit), N''Location''),
    (4, 3, 3, N''Candidate''''s personal photo.'', CAST(1 AS bit), N''Personal Photo''),
    (5, 1, 4, N''Candidate''''s IELTS score.'', CAST(0 AS bit), N''IELTS Score''),
    (6, 4, 8, N''Candidate''''s presentation skill level.'', CAST(0 AS bit), N''Presentation Skills''),
    (7, 3, 7, N''Whether the candidate is available for remote work.'', CAST(0 AS bit), N''Remote Work Availability'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Category', N'DataType', N'Description', N'IsBuiltIn', N'Name') AND [object_id] = OBJECT_ID(N'[AttributeDefinitions]'))
        SET IDENTITY_INSERT [AttributeDefinitions] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906142454_SeedAttributeLibrary'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AttributeDefinitionId', N'Label') AND [object_id] = OBJECT_ID(N'[AttributeOptions]'))
        SET IDENTITY_INSERT [AttributeOptions] ON;
    EXEC(N'INSERT INTO [AttributeOptions] ([Id], [AttributeDefinitionId], [Label])
    VALUES (1, 6, N''Beginner''),
    (2, 6, N''Intermediate''),
    (3, 6, N''Advanced'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AttributeDefinitionId', N'Label') AND [object_id] = OBJECT_ID(N'[AttributeOptions]'))
        SET IDENTITY_INSERT [AttributeOptions] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260906142454_SeedAttributeLibrary'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260906142454_SeedAttributeLibrary', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907103711_AddProfilePhotoPublicId'
)
BEGIN
    ALTER TABLE [CandidateProfiles] ADD [PersonalPhotoPublicId] nvarchar(255) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907103711_AddProfilePhotoPublicId'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260907103711_AddProfilePhotoPublicId', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908083736_ExpandAttributeLibrary'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Category', N'DataType', N'Description', N'IsBuiltIn', N'Name') AND [object_id] = OBJECT_ID(N'[AttributeDefinitions]'))
        SET IDENTITY_INSERT [AttributeDefinitions] ON;
    EXEC(N'INSERT INTO [AttributeDefinitions] ([Id], [Category], [DataType], [Description], [IsBuiltIn], [Name])
    VALUES (8, 3, 2, N''A concise overview of the candidate''''s professional background and career goals.'', CAST(0 AS bit), N''Professional Summary''),
    (9, 2, 1, N''The candidate''''s GitHub profile URL or username.'', CAST(0 AS bit), N''GitHub Profile''),
    (10, 2, 4, N''The candidate''''s total years of professional experience.'', CAST(0 AS bit), N''Years of Experience''),
    (11, 3, 5, N''The date when the candidate is available to start a new position.'', CAST(0 AS bit), N''Available Start Date''),
    (12, 2, 6, N''The start and end dates of the candidate''''s most relevant experience.'', CAST(0 AS bit), N''Relevant Experience Period''),
    (13, 3, 7, N''Whether the candidate is willing to relocate for a position.'', CAST(0 AS bit), N''Open to Relocation''),
    (14, 1, 8, N''The candidate''''s overall English proficiency level.'', CAST(0 AS bit), N''English Level'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Category', N'DataType', N'Description', N'IsBuiltIn', N'Name') AND [object_id] = OBJECT_ID(N'[AttributeDefinitions]'))
        SET IDENTITY_INSERT [AttributeDefinitions] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908083736_ExpandAttributeLibrary'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AttributeDefinitionId', N'Label') AND [object_id] = OBJECT_ID(N'[AttributeOptions]'))
        SET IDENTITY_INSERT [AttributeOptions] ON;
    EXEC(N'INSERT INTO [AttributeOptions] ([Id], [AttributeDefinitionId], [Label])
    VALUES (4, 14, N''Beginner''),
    (5, 14, N''Intermediate''),
    (6, 14, N''Upper-Intermediate''),
    (7, 14, N''Advanced''),
    (8, 14, N''Fluent'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AttributeDefinitionId', N'Label') AND [object_id] = OBJECT_ID(N'[AttributeOptions]'))
        SET IDENTITY_INSERT [AttributeOptions] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908083736_ExpandAttributeLibrary'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908083736_ExpandAttributeLibrary', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909110401_AddCandidateAttributeUsage'
)
BEGIN
    CREATE TABLE [CandidateAttributeUsages] (
        [CandidateProfileId] int NOT NULL,
        [AttributeDefinitionId] int NOT NULL,
        [LastUsedAtUtc] datetimeoffset NOT NULL,
        CONSTRAINT [PK_CandidateAttributeUsages] PRIMARY KEY ([CandidateProfileId], [AttributeDefinitionId]),
        CONSTRAINT [FK_CandidateAttributeUsages_AttributeDefinitions_AttributeDefinitionId] FOREIGN KEY ([AttributeDefinitionId]) REFERENCES [AttributeDefinitions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CandidateAttributeUsages_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909110401_AddCandidateAttributeUsage'
)
BEGIN
    CREATE INDEX [IX_CandidateAttributeUsages_AttributeDefinitionId] ON [CandidateAttributeUsages] ([AttributeDefinitionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909110401_AddCandidateAttributeUsage'
)
BEGIN
    CREATE INDEX [IX_CandidateAttributeUsages_CandidateProfileId_LastUsedAtUtc] ON [CandidateAttributeUsages] ([CandidateProfileId], [LastUsedAtUtc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909110401_AddCandidateAttributeUsage'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909110401_AddCandidateAttributeUsage', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915083718_AddCandidateProjects'
)
BEGIN
    CREATE TABLE [CandidateProjects] (
        [Id] int NOT NULL IDENTITY,
        [CandidateProfileId] int NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_CandidateProjects] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_CandidateProjects_Period] CHECK ([EndDate] >= [StartDate]),
        CONSTRAINT [FK_CandidateProjects_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915083718_AddCandidateProjects'
)
BEGIN
    CREATE TABLE [TechnologyTags] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_TechnologyTags] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915083718_AddCandidateProjects'
)
BEGIN
    CREATE TABLE [CandidateProjectTechnologyTags] (
        [CandidateProjectId] int NOT NULL,
        [TechnologyTagId] int NOT NULL,
        CONSTRAINT [PK_CandidateProjectTechnologyTags] PRIMARY KEY ([CandidateProjectId], [TechnologyTagId]),
        CONSTRAINT [FK_CandidateProjectTechnologyTags_CandidateProjects_CandidateProjectId] FOREIGN KEY ([CandidateProjectId]) REFERENCES [CandidateProjects] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CandidateProjectTechnologyTags_TechnologyTags_TechnologyTagId] FOREIGN KEY ([TechnologyTagId]) REFERENCES [TechnologyTags] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915083718_AddCandidateProjects'
)
BEGIN
    CREATE INDEX [IX_CandidateProjects_CandidateProfileId] ON [CandidateProjects] ([CandidateProfileId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915083718_AddCandidateProjects'
)
BEGIN
    CREATE INDEX [IX_CandidateProjectTechnologyTags_TechnologyTagId] ON [CandidateProjectTechnologyTags] ([TechnologyTagId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915083718_AddCandidateProjects'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TechnologyTags_Name] ON [TechnologyTags] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915083718_AddCandidateProjects'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260915083718_AddCandidateProjects', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    CREATE TABLE [Positions] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(160) NOT NULL,
        [Location] nvarchar(160) NOT NULL,
        [EmploymentType] nvarchar(60) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [UpdatedAtUtc] datetimeoffset NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Positions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    CREATE TABLE [PositionTechnologyTags] (
        [PositionId] int NOT NULL,
        [TechnologyTagId] int NOT NULL,
        CONSTRAINT [PK_PositionTechnologyTags] PRIMARY KEY ([PositionId], [TechnologyTagId]),
        CONSTRAINT [FK_PositionTechnologyTags_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PositionTechnologyTags_TechnologyTags_TechnologyTagId] FOREIGN KEY ([TechnologyTagId]) REFERENCES [TechnologyTags] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    CREATE TABLE [PositionTemplateAttributes] (
        [PositionId] int NOT NULL,
        [AttributeDefinitionId] int NOT NULL,
        [DisplayOrder] int NOT NULL,
        CONSTRAINT [PK_PositionTemplateAttributes] PRIMARY KEY ([PositionId], [AttributeDefinitionId]),
        CONSTRAINT [FK_PositionTemplateAttributes_AttributeDefinitions_AttributeDefinitionId] FOREIGN KEY ([AttributeDefinitionId]) REFERENCES [AttributeDefinitions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PositionTemplateAttributes_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'Description', N'EmploymentType', N'IsActive', N'Location', N'Title', N'UpdatedAtUtc') AND [object_id] = OBJECT_ID(N'[Positions]'))
        SET IDENTITY_INSERT [Positions] ON;
    EXEC(N'INSERT INTO [Positions] ([Id], [CreatedAtUtc], [Description], [EmploymentType], [IsActive], [Location], [Title], [UpdatedAtUtc])
    VALUES (1, ''2026-09-20T08:00:00.0000000+00:00'', N''Build and maintain modern web applications using ASP.NET Core and Entity Framework Core.'', N''Full-time'', CAST(1 AS bit), N''Riyadh, Saudi Arabia'', N''Junior ASP.NET Core Developer'', ''2026-09-20T08:00:00.0000000+00:00''),
    (2, ''2026-09-20T09:00:00.0000000+00:00'', N''Create responsive, accessible user interfaces for the Perfect Career platform.'', N''Full-time'', CAST(1 AS bit), N''Remote'', N''Frontend Developer'', ''2026-09-20T09:00:00.0000000+00:00'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'Description', N'EmploymentType', N'IsActive', N'Location', N'Title', N'UpdatedAtUtc') AND [object_id] = OBJECT_ID(N'[Positions]'))
        SET IDENTITY_INSERT [Positions] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AttributeDefinitionId', N'PositionId', N'DisplayOrder') AND [object_id] = OBJECT_ID(N'[PositionTemplateAttributes]'))
        SET IDENTITY_INSERT [PositionTemplateAttributes] ON;
    EXEC(N'INSERT INTO [PositionTemplateAttributes] ([AttributeDefinitionId], [PositionId], [DisplayOrder])
    VALUES (8, 1, 1),
    (9, 1, 2),
    (10, 1, 3),
    (14, 1, 4),
    (8, 2, 1),
    (9, 2, 2),
    (14, 2, 3)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AttributeDefinitionId', N'PositionId', N'DisplayOrder') AND [object_id] = OBJECT_ID(N'[PositionTemplateAttributes]'))
        SET IDENTITY_INSERT [PositionTemplateAttributes] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
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
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    CREATE INDEX [IX_Positions_IsActive_UpdatedAtUtc] ON [Positions] ([IsActive], [UpdatedAtUtc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    CREATE INDEX [IX_PositionTechnologyTags_TechnologyTagId] ON [PositionTechnologyTags] ([TechnologyTagId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    CREATE INDEX [IX_PositionTemplateAttributes_AttributeDefinitionId] ON [PositionTemplateAttributes] ([AttributeDefinitionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PositionTemplateAttributes_PositionId_DisplayOrder] ON [PositionTemplateAttributes] ([PositionId], [DisplayOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920091347_AddPositions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920091347_AddPositions', N'8.0.30');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920092514_AddCandidateCvs'
)
BEGIN
    CREATE TABLE [CandidateCvs] (
        [Id] int NOT NULL IDENTITY,
        [CandidateProfileId] int NOT NULL,
        [PositionId] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [UpdatedAtUtc] datetimeoffset NOT NULL,
        [PublishedAtUtc] datetimeoffset NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_CandidateCvs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CandidateCvs_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CandidateCvs_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920092514_AddCandidateCvs'
)
BEGIN
    CREATE TABLE [CandidateCvProjects] (
        [CandidateCvId] int NOT NULL,
        [CandidateProjectId] int NOT NULL,
        CONSTRAINT [PK_CandidateCvProjects] PRIMARY KEY ([CandidateCvId], [CandidateProjectId]),
        CONSTRAINT [FK_CandidateCvProjects_CandidateCvs_CandidateCvId] FOREIGN KEY ([CandidateCvId]) REFERENCES [CandidateCvs] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CandidateCvProjects_CandidateProjects_CandidateProjectId] FOREIGN KEY ([CandidateProjectId]) REFERENCES [CandidateProjects] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920092514_AddCandidateCvs'
)
BEGIN
    CREATE INDEX [IX_CandidateCvProjects_CandidateProjectId] ON [CandidateCvProjects] ([CandidateProjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920092514_AddCandidateCvs'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CandidateCvs_CandidateProfileId_PositionId] ON [CandidateCvs] ([CandidateProfileId], [PositionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920092514_AddCandidateCvs'
)
BEGIN
    CREATE INDEX [IX_CandidateCvs_PositionId] ON [CandidateCvs] ([PositionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920092514_AddCandidateCvs'
)
BEGIN
    CREATE INDEX [IX_CandidateCvs_Status_UpdatedAtUtc] ON [CandidateCvs] ([Status], [UpdatedAtUtc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920092514_AddCandidateCvs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920092514_AddCandidateCvs', N'8.0.30');
END;
GO

COMMIT;
GO

