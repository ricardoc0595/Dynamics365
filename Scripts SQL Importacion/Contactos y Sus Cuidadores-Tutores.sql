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
ALTER PROCEDURE [dbo].[Contactsandinchargelookups]
-- Add the parameters for the stored procedure here
AS
  BEGIN -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET nocount ON;

      -- Insert statements for procedure here
      /****** Script for SelectTopNRows command from SSMS  ******/
      CREATE TABLE #temp
        (
           [id_paciente] [INT] PRIMARY KEY,
           [cedula]      [VARCHAR](50) NOT NULL,
           [categoria]   [VARCHAR](45) NULL,
        );

      INSERT INTO #temp
                  ([id_paciente],
                   [cedula],
                   [categoria])
      SELECT [id_paciente],
             [cedula],
             [categoria]
      FROM   pacientes p
      WHERE
        --Buscar usuarios que se encuentran tanto en la tabla de cuentas como la de pacientes y que a su vez, ese mismo usuario se encuentre dentro de la tabla 
        --de relacion pacientesxusuario
        EXISTS (SELECT uc.usuario
                FROM   usuarios_cuentas uc
                WHERE  uc.usuario = p.cedula
                       AND p.id_paciente IN (SELECT p.id_paciente
                                             FROM   usuarios_cuentas uc
                                             WHERE
                           uc.usuario = p.cedula
                           AND p.cedula = uc.usuario
                           AND EXISTS
                               --Buscar que exista en la tabla de rm_pacientesxusuario, esto confirma que es un usuario dueño de una cuenta y que es paciente activo
                               (SELECT p.id_paciente
                                FROM   rm_pacientesxusuario
                                       pxu
                                WHERE  pxu.id_paciente =
                               p.id_paciente)))
         --Si no existe dentro de los usuarios que son duenos de cuenta y a su vez pacientes, traer los datos si es solamente un paciente que existe en la tabla de rm_pacientesxusuario, lo que indica que es un paciente bajo cuido de alguien
         OR EXISTS (SELECT pxu.id_paciente
                    FROM   rm_pacientesxusuario pxu
                    WHERE  pxu.id_paciente = p.id_paciente
                           AND pxu.status = 'AC')

      --------------------------------------------------
      ----------------------------------------------
      CREATE TABLE #temp2
        (
           [id_paciente] [INT] PRIMARY KEY,
           [cedula]      [VARCHAR](50) NOT NULL,
           [categoria]   [VARCHAR](45) NULL,
        );

      INSERT INTO #temp2
                  ([id_paciente],
                   [cedula],
                   [categoria])
      SELECT [id_paciente],
             [cedula],
             [categoria]
      FROM   #temp
      WHERE  categoria = 'S'

      --and id_paciente='1017635'
      --Select count(*) from #temp2 --where cedula='306370176'
      ------
      --Select idPaciente,idACargo,count(*) from
      --(
      --La idea de este select es tomar de todos los pacientes obtenidos mediante el query que filtra por usuarios duenos de cuentas y pacientes y
      --obtener para aquellos que son Categoria 'S' el ID del paciente/cuenta que está cuidándolos.
      --Mediante la tabla rm_pacientesxusuario se puede obtener al paciente/cuenta cuidador/tutor tomando la cedula y buscando en la tabla temporal de todos los pacientes y cuentas
      --obtenido anteriormente
      SELECT t2.id_paciente AS idPaciente,
             t1.id_paciente AS idACargo
      FROM   #temp t1
             INNER JOIN rm_pacientesxusuario pxu
                        INNER JOIN #temp2 t2
                                ON pxu.id_paciente = t2.id_paciente
                     ON pxu.usuario = t1.cedula
      WHERE  t2.id_paciente = pxu.id_paciente

      --)k
      --group by idPaciente,idACargo
      --having count(*)>1
      ----------------------
      ------------------
	  --
      --Select * from #PatientsAndLookups
      DROP TABLE #temp

      DROP TABLE #temp2
  END 