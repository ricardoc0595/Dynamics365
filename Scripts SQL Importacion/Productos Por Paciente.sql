USE [REPORTES]
GO
/****** Object:  StoredProcedure [dbo].[ContactsProduct]    Script Date: 03-Feb-21 3:31:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ContactsProduct]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here



--Select Dosis from	
--(
Select   mxp.Medicamento, mxp.Dosis,mxp.ID_Paciente from paciente_medicamento mxp 
inner join pacientes p 
On p.ID_Paciente=mxp.ID_Paciente
 WHERE   EXISTS (SELECT uc.usuario
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
             pxu.id_paciente = p.id_paciente
			 AND pxu.status = 'AC')))
              OR EXISTS (SELECT pxu.id_paciente
                         FROM   rm_pacientesxusuario pxu
                         WHERE  pxu.id_paciente = p.id_paciente
                                AND pxu.status = 'AC')
								
								
								--) k
								--group by Dosis




END
