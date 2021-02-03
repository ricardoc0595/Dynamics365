USE [REPORTES]

go

/****** Object:  StoredProcedure [dbo].[ContactTest]    Script Date: 01-Feb-21 9:48:06 AM ******/
SET ansi_nulls ON

go

SET quoted_identifier ON

go

-- =============================================
-- Author:    
-- Create date: 
-- Description:  
-- =============================================
ALTER PROCEDURE [dbo].[Contacttest]
-- Add the parameters for the stored procedure here
AS
  BEGIN -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET nocount ON;

      DECLARE @tu VARCHAR(2);
      DECLARE @k INT;

      -- Insert statements for procedure here
      /****** Script for SelectTopNRows command from SSMS  ******/
      SELECT interes,
             descripcion
      INTO   #tempintereses
      FROM   intereses

      CREATE TABLE #temp
        (
           [id_paciente]              [INT] NULL,
           [cedula]                   [VARCHAR](50) NOT NULL,
           [nombre]                   [VARCHAR](50) NOT NULL,
           [apellido1]                [VARCHAR](50) NULL,
           [apellido2]                [VARCHAR](50) NULL,
           [genero]                   [VARCHAR](10) NULL,
           [telefono]                 [VARCHAR](50) NOT NULL,
           [telefono2]                [VARCHAR](50) NULL,
           [provincia]                [VARCHAR](50) NULL,
           [canton]                   [VARCHAR](50) NULL,
           [distrito]                 [VARCHAR](50) NULL,
           [direccion]                [VARCHAR](500) NULL,
           [fechanacimiento]          [VARCHAR](50) NULL,
           [pais]                     [VARCHAR](50) NOT NULL,
           [email]                    [VARCHAR](250) NULL,
           [registrado]               [SMALLDATETIME] NULL,
           [esmenordeedad]            [INT] NULL,
           [createdate]               [SMALLDATETIME] NULL,
           [categoria]                [VARCHAR](45) NULL,
           [tipoterminosycondiciones] [VARCHAR](45) NULL,
           [intereses]                [VARCHAR](600) NULL,
           [hasnoemail]               [BIT] NULL,
           [ischildcontact]           [BIT] NULL,
           [usertype]                 [VARCHAR](2) NULL,
           [idtype]                   INT NULL,
        );

      INSERT INTO #temp
                  ([id_paciente],
                   [cedula],
                   [nombre],
                   [apellido1],
                   [apellido2],
                   [genero],
                   [telefono],
                   [telefono2],
                   [provincia],
                   [canton],
                   [distrito],
                   [direccion],
                   [fechanacimiento],
                   [pais],
                   [email],
                   [registrado],
                   [esmenordeedad],
                   [createdate],
                   [categoria],
                   [tipoterminosycondiciones],
                   [intereses],
                   [hasnoemail],
                   [ischildcontact],
                   [usertype],
                   [idtype])
      SELECT [id_paciente],
             [cedula],
             [nombre],
             [apellido1],
             [apellido2],
             [genero],
             [telefono],
             [telefono2],
             [provincia],
             [canton],
             [distrito],
             [direccion],
             [fechanacimiento],
             [pais],
             [email],
             [registrado],
             [esmenordeedad],
             [createdate],
             [categoria],
             [tipoterminosycondiciones],
             --''           AS intereses,
             STUFF((SELECT DISTINCT ';' + intrsts.Descripcion + '[//]' FROM rm_interesesxpaciente ixp,#TempIntereses intrsts  where ixp.ID_Paciente =p.ID_Paciente and intrsts.Interes=ixp.Interes FOR XML PATH('')), 1 ,1, '') AS [intereses],
             (SELECT CASE
                       WHEN p.email LIKE '%@aboxplan.com%' THEN 1
                       ELSE 0
                     END) AS HasNoEmail,
             (SELECT CASE
                       WHEN p.categoria = 'S' THEN 1
                       ELSE 0
                     END) AS [IsChildContact],
             --(Select CASE WHEN p.Categoria='P' and (Select Count(Usuario) from rm_pacientesxusuario where Usuario=p.Cedula)>1
             --and ((SELECT TOP 1 TipoUsuario from rm_pacientesxusuario where ID_Paciente=p.ID_Paciente)='01') THEN 'CU' ELSE 'TU' END)  as [UserType]
             (SELECT CASE
                       WHEN p.categoria = 'P'
                            AND (SELECT Count(usuario)
                                 FROM   rm_pacientesxusuario
                                 WHERE  usuario = p.cedula) > 1
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente) = '01' )
                     THEN
                       'TU'
                       -- en la tabla rm_pacientesxusuario el usuario tutor tiene valor 01 y el que tiene bajo cuido es el que tiene el valor de 03
                       WHEN p.categoria = 'P'
                            AND (SELECT Count(usuario)
                                 FROM   rm_pacientesxusuario
                                 WHERE  usuario = p.cedula) > 1
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente) = '02' )
                     THEN
                       'CU'
                       WHEN p.categoria = 'P'
                            AND (SELECT Count(usuario)
                                 FROM   rm_pacientesxusuario
                                 WHERE  usuario = p.cedula) > 1
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente) = '03' )
                     THEN
                       'TU'
                       WHEN p.categoria = 'P'
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente) = '01' )
                     THEN
                       'PA'
                       WHEN p.categoria = 'P'
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente) = '05' )
                     THEN
                       'OI'
                       WHEN p.categoria = 'S' THEN 'BC'
                     END) AS [UserType],
             (SELECT CASE
                       WHEN Isnumeric(p.cedula) = 1 THEN 1
                       WHEN Try_convert(uniqueidentifier, p.cedula) IS NOT NULL
                             OR p.cedula LIKE '%PME%' THEN 3
                       ELSE 2
                     END) AS [IdType]
      FROM   pacientes p
      WHERE  EXISTS (SELECT uc.usuario
                     FROM   usuarios_cuentas uc
                     WHERE  uc.usuario = p.cedula
                            AND p.id_paciente IN
                                (SELECT p.id_paciente
                                 FROM   usuarios_cuentas uc
                                 WHERE  uc.usuario = p.cedula
                                        AND
                                p.cedula = uc.usuario
                                        AND EXISTS
                                        (SELECT p.id_paciente
                                         FROM
                                            rm_pacientesxusuario
                                            pxu
                                                   WHERE
             pxu.id_paciente = p.id_paciente)))
              OR EXISTS (SELECT pxu.id_paciente
                         FROM   rm_pacientesxusuario pxu
                         WHERE  pxu.id_paciente = p.id_paciente
                                AND pxu.status = 'AC')

      --where p.Email like '%loymark%'
      DROP TABLE #tempintereses

      SELECT *
      FROM   #temp --where Cedula='1006'
  END 