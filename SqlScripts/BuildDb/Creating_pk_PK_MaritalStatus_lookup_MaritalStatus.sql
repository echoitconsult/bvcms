ALTER TABLE [lookup].[MaritalStatus] ADD CONSTRAINT [PK_MaritalStatus] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
