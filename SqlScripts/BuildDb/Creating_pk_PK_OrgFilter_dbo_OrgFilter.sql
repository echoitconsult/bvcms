ALTER TABLE [dbo].[OrgFilter] ADD CONSTRAINT [PK_OrgFilter] PRIMARY KEY CLUSTERED  ([QueryId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
