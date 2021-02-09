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
ALTER PROCEDURE [dbo].[ContactsAndInChargeLookups]
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
     

      CREATE TABLE #temp
        (
           [id_paciente]              [INT] primary key,
           [cedula]                   [VARCHAR](50) NOT NULL,
           
           [categoria]                [VARCHAR](45) NULL,
       
    
         
		  
        );


		CREATE TABLE #PatientsAndLookups
        (
           [id_paciente]              [INT] primary key,
           [InchargeOfLookup]		  INT NULL
		   
        );


      INSERT INTO #temp
                  ([id_paciente],
                   [cedula],
                   [categoria]
                 
				 )
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
             pxu.id_paciente = p.id_paciente)))
			 --Si no existe dentro de los usuarios que son duenos de cuenta y a su vez pacientes, traer los datos si es solamente un paciente que existe en la tabla de rm_pacientesxusuario, lo que indica que es un paciente bajo cuido de alguien
              OR EXISTS (SELECT pxu.id_paciente
                         FROM   rm_pacientesxusuario pxu
                         WHERE  pxu.id_paciente = p.id_paciente
                                AND pxu.status = 'AC')


	
	--------------------------------------------------
	----------------------------------------------

	    CREATE TABLE #temp2
        (
           [id_paciente]              [INT] primary key,
           [cedula]                   [VARCHAR](50) NOT NULL,
           
           [categoria]                [VARCHAR](45) NULL,
       
    
         
		  
        );



      INSERT INTO #temp2
                  ([id_paciente],
                   [cedula],
                   [categoria]
                 
				 )
      SELECT [id_paciente],
             [cedula],
             [categoria]
      FROM   #Temp
	  where categoria='S'
	  --and id_paciente='1017635'

	  --Select * from #temp2



	  ----
	  Select * from
	  (
	  Select t2.Cedula as cp,t1.Cedula as cc  from #temp t1
	  inner join rm_pacientesxusuario pxu
	  inner join #temp2 t2
	  on pxu.ID_Paciente=t2.id_paciente
	  on pxu.Usuario=t1.Cedula
	  where t2.id_paciente=pxu.ID_Paciente
	  --and pxu.Usuario=t1.cedula

	  )k
	  where k.cp='pacienteloymark01'
	----------------------
	------------------
						
      
      

  
	 
	-- declare @TableID int
	--  declare @lookupID int
	--  declare @hs varchar(200)

	--while exists (select Id_Paciente from #TEMP2)
	--begin

 --   select @TableID = (select top 1 [id_paciente]
 --                      from #TEMP2
 --                      )


	--				   --Select cedula from pacientes
	--				   --inner join rm_pacientesxusuario
	--				   --on rm_pacientesxusuario.ID_Paciente=pacientes.ID_Paciente
	--				   --where pacientes.Cedula=

	--				   Set @hs=(Select Usuario from rm_pacientesxusuario inner join #temp
	--				   on #temp.ID_Paciente=rm_pacientesxusuario.ID_Paciente
	--				   where rm_pacientesxusuario.ID_Paciente=@TableID)

	--				   Set @lookupId = (Select top 1 #temp.Id_Paciente from #temp
	--				   where #temp.Cedula=@hs)
					   
	--				   --y que sean dueños de una cuenta hacer el query

	--				   ------



	--				   ------

					  

	--				   Insert into #PatientsAndLookups (id_paciente,InchargeOfLookup) values (@TableID,@lookupId);
   

 --   delete #Temp2  where id_paciente = @TableID

	--end
 
      --Select * from #PatientsAndLookups
	  Drop table #temp
	  Drop table #temp2
	  drop table #PatientsAndLookups
  END 

  

  --448556 con AP
 