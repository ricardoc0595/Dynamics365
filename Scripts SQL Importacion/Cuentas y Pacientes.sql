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

	  DECLARE @IdDynamicsPatientUserType VARCHAR(36);
	   DECLARE @IdDynamicsTutorUserType VARCHAR(36);
	    DECLARE @IdDynamicsCaretakerUserType VARCHAR(36);
		 DECLARE @IdDynamicsOtherInterestUserType VARCHAR(36);
		  DECLARE @IdDynamicsPatientUndercareUserType VARCHAR(36);

		 SET @IdDynamicsPatientUserType='15810a1e-c8d1-ea11-a812-000d3a33f637';
		 SET @IdDynamicsTutorUserType='f4761324-c8d1-ea11-a812-000d3a33f637';
		 SET @IdDynamicsCaretakerUserType='fab60b2a-c8d1-ea11-a812-000d3a33f637';
		 SET @IdDynamicsOtherInterestUserType='30f90330-c8d1-ea11-a812-000d3a33f637';
		 SET @IdDynamicsPatientUndercareUserType='dc9a7b9d-5366-eb11-a812-002248029573';

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
           [createdate]               [SMALLDATETIME] NULL,
           [categoria]                [VARCHAR](45) NULL,
           [intereses]                [VARCHAR](600) NULL,
           [hasnoemail]               [BIT] NULL,
           [ischildcontact]           [BIT] NULL,
           [usertype]                 [VARCHAR](36) NULL,
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
                   [createdate],
                   [categoria],
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
             --(Select case when isnumeric(p.Provincia)=1 then p.Provincia else '' end)
			(Select case when isnumeric(p.Provincia)=1 then p.Provincia else '' end) as [provincia],
             (Select case when isnumeric(p.Canton)=1 then p.Canton else '' end) as [canton],
              (Select case when isnumeric(p.Distrito)=1 then p.Distrito else '' end) as [distrito],
             [direccion],
             [fechanacimiento],
             [pais],
             [email],
             [registrado],
             [createdate],
             [categoria],
             ''           AS intereses,
             --STUFF((SELECT DISTINCT ';' + intrsts.Descripcion + '[//]' FROM rm_interesesxpaciente ixp,#TempIntereses intrsts  where ixp.ID_Paciente =p.ID_Paciente and intrsts.Interes=ixp.Interes FOR XML PATH('')), 1 ,1, '') AS [intereses],
             (SELECT CASE
                       WHEN p.email LIKE '%@aboxplan.com%' THEN 1
                       ELSE 0
                     END) AS HasNoEmail,
             (SELECT CASE
                       WHEN p.categoria = 'S' THEN 1
                       ELSE 0
                     END) AS [IsChildContact],
             --(Select CASE WHEN p.Categoria='P' and (Select Count(Usuario) from rm_pacientesxusuario where Usuario=p.Cedula)>1
             --and ((SELECT TOP 1 TipoUsuario from rm_pacientesxusuario where ID_Paciente=p.ID_Paciente)='01') THEN @IdDynamicsCaretakerUserType ELSE @IdDynamicsTutorUserType END)  as [UserType]
             (SELECT CASE


					   --Si es un usuario categoria P, que es dueño de una cuenta y en la tabla de rm_pacientesxusuario existe más de un registro con la misma cedula, es un usuario cuidador o tutor, donde uno es el dueño de la cuenta (tutor o cuidador) y el otro el paciente
                       WHEN p.categoria = 'P'
                            AND (SELECT Count(usuario)
                                 FROM   rm_pacientesxusuario
                                 WHERE  usuario = p.cedula
								 and status='AC') > 1
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente
								   and status='AC') = '01' )
                     THEN
                       @IdDynamicsTutorUserType
                       -- en la tabla rm_pacientesxusuario el usuario tutor tiene valor 01 y el que tiene bajo cuido es el que tiene el valor de 03
                       WHEN p.categoria = 'P'
                            AND (SELECT Count(usuario)
                                 FROM   rm_pacientesxusuario
                                 WHERE  usuario = p.cedula
								 and status='AC') > 1
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente
								   and status='AC') = '02' )
                     THEN
                       @IdDynamicsCaretakerUserType
					   --Si es un usuario categoria P, que es dueño de una cuenta y en la tabla de rm_pacientesxusuario existe más de un registro con la misma cedula, es un usuario cuidador o tutor, donde uno es el dueño de la cuenta (tutor o cuidador) y el otro el paciente
                       WHEN p.categoria = 'P'
                            AND (SELECT Count(usuario)
                                 FROM   rm_pacientesxusuario
                                 WHERE  usuario = p.cedula
								 and status='AC') > 1
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente
								   and status='AC') = '03' )
                     THEN
                       @IdDynamicsTutorUserType
					   --Si es un usuario categoria P que es dueño de una cuenta, y en la tabla de rm_pacientesxusuario al buscar por ID de paciente retorna que el tipo de usuario es 01, entonces es un usuario de cuenta de tipo paciente sin ningun paciente bajo su cuido
                       WHEN p.categoria = 'P'
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente
								   and status='AC') = '01' )
                     THEN
                       @IdDynamicsPatientUserType
					   --Si es un usuario categoria P que es dueño de una cuenta, y en la tabla de rm_pacientesxusuario al buscar por ID de paciente retorna que el tipo de usuario es 05, entonces es un usuario de cuenta de tipo otro interes sin ningun paciente bajo su cuido
                       WHEN p.categoria = 'P'
                            AND ( (SELECT TOP 1 tipousuario
                                   FROM   rm_pacientesxusuario
                                   WHERE  id_paciente = p.id_paciente
								   and status='AC') = '05' )
                     THEN
                       @IdDynamicsOtherInterestUserType
					   --Si es un paciente con categoria S, es un paciente bajo cuido de alguien, a nivel de Dynamics se tomarán todos como el tipo "paciente bajo cuido"
                       WHEN p.categoria = 'S' THEN @IdDynamicsPatientUndercareUserType
                     END) AS [UserType],
             (SELECT CASE
                       WHEN Isnumeric(p.cedula) = 1 THEN 1
                       WHEN Try_convert(uniqueidentifier, p.cedula) IS NOT NULL
                             OR p.cedula LIKE '%PME%' THEN 3
                       ELSE 2
                     END) AS [IdType]
      FROM   pacientes p
      WHERE 
	  --Buscar usuarios que se encuentran tanto en la tabla de cuentas como la de pacientes y que a su vez, ese mismo usuario se encuentre dentro de la tabla 
	  --de relacion pacientesxusuario
	  EXISTS (SELECT uc.usuario
                     FROM   usuarios_cuentas uc
                     WHERE  uc.usuario = p.cedula
                            AND p.id_paciente IN
                                (SELECT p.id_paciente
                                 FROM   usuarios_cuentas uc
                                 WHERE  uc.usuario = p.cedula
                                        AND
                                p.cedula = uc.usuario
                                        AND EXISTS
										--Buscar que exista en la tabla de rm_pacientesxusuario, esto confirma que es un usuario dueño de una cuenta y que es paciente activo
                                        (SELECT p.id_paciente
                                         FROM
                                            rm_pacientesxusuario
                                            pxu
                                                   WHERE
             pxu.id_paciente = p.id_paciente
			 AND pxu.status = 'AC')))
			 --Si no existe dentro de los usuarios que son duenos de cuenta y a su vez pacientes, traer los datos si es solamente un paciente que existe en la tabla de rm_pacientesxusuario, lo que indica que es un paciente bajo cuido de alguien
              OR EXISTS (SELECT pxu.id_paciente
                         FROM   rm_pacientesxusuario pxu
                         WHERE  pxu.id_paciente = p.id_paciente
                                AND pxu.status = 'AC')


      
      DROP TABLE #tempintereses

      SELECT *
      FROM   #temp --where cedula='LOYMARKADMIN001'
  END 

  

  --448556 con AP
 