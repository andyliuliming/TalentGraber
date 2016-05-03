USE [talentgrabergold]
GO

--------TalentCandidates ---------------------
CREATE NONCLUSTERED INDEX [IX_TalentCandidates_0]
    ON [dbo].[TalentCandidates]([Login] ASC);
GO

--------ContributerToRepoes ---------------------
CREATE NONCLUSTERED INDEX [IX_ContributerToRepoes_0]
    ON [dbo].[ContributerToRepoes]([RepoId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ContributerToRepoes_1]
    ON [dbo].[ContributerToRepoes]([TalentCandidateId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ContributerToRepoes_3]
    ON [dbo].[ContributerToRepoes]([RepoId] ASC, [TalentCandidateId] ASC);
GO