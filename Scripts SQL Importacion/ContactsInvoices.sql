-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE ContactsInvoices 
	-- Add the parameters for the stored procedure here
	


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Select new_purchasemethod from(

Select
f.Id_Factura as 'new_idaboxinvoice',
f.Id_Paciente as 'customerid_contact@odata.bind',
f.Id_Farmacia as 'new_Pharmacy@odata.bind',
f.Factura as 'new_invoicenumber',
(Select case when f.Estado like '%aprobad%' then 1 
  when f.Estado like '%rechazad%' then 2 
  when f.Estado like '%pendiente%' then 3
  when f.Estado like '%anulad%' then 4 end) as 'new_invoicestatus',
f.Motivo as 'new_statusreason',
(Select case when f.MedioCompra='farmacia' then 1
when f.MedioCompra='domicilio' then 2 end) as 'new_purchasemethod',
f.Fecha as 'new_purchasedate',
f.MontoTotal as 'new_totalamount',
f.FacturaImagenUrl as 'new_aboximageurl',
f.TiempoRevision1 as 'new_revisiontime1',
f.TiempoRevision2 as 'new_revisiontime2',
p.Pais as 'new_InvoiceCountry@odata.bind',
(Select ID_Producto as id,Cantidad as quantity, Precio as price, Estado as status from factura_producto where ID_Factura=f.ID_Factura for json auto ) as 'new_productsselectedjson',
(Select top 1 Producto as productName,Cantidad as quantity, Precio as price from factura_producto_otros where ID_Factura=f.ID_Factura for json auto ) as 'new_nonaboxproductsselectedjson'



	
	
--	SELECT 

--f.Id_Factura ,
--f.Id_Paciente,
--f.Id_Farmacia,
--f.Factura ,
--f.Estado ,
--f.Motivo, 
--f.MedioCompra, 
--f.Fecha, 
--f.MontoTotal,
--f.FacturaImagenUrl ,
--f.TiempoRevision1 ,
--f.TiempoRevision2,
--(Select ID_Producto as id,Cantidad as quantity, Precio as price, Estado as status from factura_producto where ID_Factura=f.ID_Factura for json auto ) as ProductosAbox,
--(Select top 1 Producto as productName,Cantidad as quantity, Precio as price from factura_producto_otros where ID_Factura=f.ID_Factura for json auto ) as ProductosNoAbox

--1170616 facturaloymarkignorar  id paciente 1017635

from facturas f
inner join pacientes p
on p.ID_Paciente=f.ID_Paciente


where 

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
								--group by new_purchasemethod
								

	
END
GO

