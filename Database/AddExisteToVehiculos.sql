IF COL_LENGTH(N'dbo.Vehiculos', N'Existe') IS NULL
BEGIN
    ALTER TABLE [dbo].[Vehiculos]
        ADD [Existe] bit NOT NULL
            CONSTRAINT [DF_Vehiculos_Existe] DEFAULT (1);
END;
