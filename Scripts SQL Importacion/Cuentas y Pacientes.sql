USE [REPORTES]
GO
/****** Object:  StoredProcedure [dbo].[ContactTest]    Script Date: 01-Feb-21 9:48:06 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	-- =============================================
	-- Author:		<Author,,Name>
	-- Create date: <Create Date,,>
	-- Description:	<Description,,>
	-- =============================================
	ALTER PROCEDURE [dbo].[ContactTest] -- Add the parameters for the stored procedure here
	AS 
	BEGIN -- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET
	NOCOUNT ON;

-- Insert statements for procedure here
/****** Script for SelectTopNRows command from SSMS  ******/
Select Interes,Descripcion INTO #TempIntereses from intereses


CREATE TABLE #TEMP (
[ID_Paciente] [int] NULL,
[Cedula] [varchar](50) NOT NULL,
[Nombre] [varchar](50) NOT NULL,
[Apellido1] [varchar](50) NULL,
[Apellido2] [varchar](50) NULL,
[Genero] [varchar](10) NULL,
[Telefono] [varchar](50) NOT NULL,
[Telefono2] [varchar](50) NULL,
[Provincia] [varchar](50) NULL,
[Canton] [varchar](50) NULL,
[Distrito] [varchar](50) NULL,
[Direccion] [varchar](500) NULL,
[FechaNacimiento] [varchar](50) NULL,
[Pais] [varchar](50) NOT NULL,
[Email] [varchar](250) NULL,
[Registrado] [smalldatetime] NULL,
[EsMenorDeEdad] [int] NULL,
[createDate] [smalldatetime] NULL,
[Categoria] [varchar](45) NULL,
[TipoTerminosYCondiciones] [varchar](45) NULL,
[intereses] [varchar](600) NULL,
[HasNoEmail] [bit] NULL,
[IsMinor][bit] NULL
);

INSERT INTO
	#TEMP ([ID_Paciente],
	[Cedula],
	[Nombre],
	[Apellido1],
	[Apellido2],
	[Genero],
	[Telefono],
	[Telefono2],
	[Provincia],
	[Canton],
	[Distrito],
	[Direccion],
	[FechaNacimiento],
	[Pais],
	[Email],
	[Registrado],
	[EsMenorDeEdad],
	[createDate],
	[Categoria],
	[TipoTerminosYCondiciones],
	[intereses],
	[HasNoEmail],
	[IsMinor]
)


SELECT 
[ID_Paciente],
	[Cedula],
	[Nombre],
	[Apellido1],
	[Apellido2],
	[Genero],
	[Telefono],
	[Telefono2],
	[Provincia],
	[Canton],
	[Distrito],
	[Direccion],
	[FechaNacimiento],
	[Pais],
	[Email],
	[Registrado],
	[EsMenorDeEdad],
	[createDate],
	[Categoria],
	[TipoTerminosYCondiciones], 
	--STUFF((SELECT DISTINCT ';' + SUBSTRING(Interes, PATINDEX('%[^0]%', Interes+'.'), LEN(Interes)) FROM rm_interesesxpaciente ixp  where ixp.ID_Paciente =p.ID_Paciente FOR XML PATH('')), 1 ,1, '') AS [intereses]
	STUFF((SELECT DISTINCT ';' + intrsts.Descripcion + '[//]' FROM rm_interesesxpaciente ixp,#TempIntereses intrsts  where ixp.ID_Paciente =p.ID_Paciente and intrsts.Interes=ixp.Interes FOR XML PATH('')), 1 ,1, '') AS [intereses],

	--Identificar filtros de usuarios, hay repetidos, otros con Cedulas que tienen algo concatenado como "Migracion" a la cedula.
	--Filtro con Categoria P, hay un usuario de Pikarona que aparece 3 veces, pero el sistema solo utiliza uno.
	--api_UpdateUserType de que es este userId??
	--Sacar IdType
	(SELECT CASE WHEN p.Email like '%@aboxplan.com%' THEN 1 ELSE 0 END) as HasNoEmail,
	(Select CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER, p.Cedula) IS NOT NULL THEN 1 ELSE 0 END) as IsMinor
FROM pacientes p
--where p.Email like '%loymark%'


	Drop table #TempIntereses
	
	Select * from #TEMP ;


	--Buscar en pacientesxusuario si hay mas de una ocurrencia de un tipo de usuario
--	SELECT  ID_Paciente, TipoUsuario, COUNT(*)
--FROM rm_pacientesxusuario
--GROUP BY ID_Paciente, TipoUsuario
--HAVING COUNT(*) > 1

END
