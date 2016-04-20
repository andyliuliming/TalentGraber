SET IDENTITY_INSERT [dbo].[GithubAccounts] ON
INSERT INTO [dbo].[GithubAccounts] ([Id], [UserName], [Password]) VALUES (1, N'talentextract@outlook.com', N'Quattro!888')
INSERT INTO [dbo].[GithubAccounts] ([Id], [UserName], [Password]) VALUES (3, N'talentextract2@outlook.com', N'Quattro!888')
INSERT INTO [dbo].[GithubAccounts] ([Id], [UserName], [Password]) VALUES (4, N'talentextract3@outlook.com', N'Quattro!888')
INSERT INTO [dbo].[GithubAccounts] ([Id], [UserName], [Password]) VALUES (6, N'talentextract4@outlook.com', N'Quattro!888')
SET IDENTITY_INSERT [dbo].[GithubAccounts] OFF

SET IDENTITY_INSERT [dbo].[GithubRepoes] ON
INSERT INTO [dbo].[GithubRepoes] ([Id], [Url]) VALUES (1, N'https://github.com/Azure/azure-linux-extensions.git')
INSERT INTO [dbo].[GithubRepoes] ([Id], [Url]) VALUES (2, N'https://github.com/cloudfoundry/cf-release.git')
SET IDENTITY_INSERT [dbo].[GithubRepoes] OFF

SET IDENTITY_INSERT [dbo].[TalentGraberUsers] ON
INSERT INTO [dbo].[TalentGraberUsers] ([Id], [UserName], [Password]) VALUES (1, N'a', N'a')
SET IDENTITY_INSERT [dbo].[TalentGraberUsers] OFF
