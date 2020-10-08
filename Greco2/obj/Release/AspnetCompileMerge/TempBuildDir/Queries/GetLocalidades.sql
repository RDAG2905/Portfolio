--USE Greco2

--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetLocalidad 
@nombre varchar(80),
@provinciaId int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT loc.Id,loc.Nombre,prov.Nombre,loc.Activo 
	from tLocalidades loc
	inner join
	tProvincias prov
	on loc.PROVINCIA_ID = prov.Id
END
--GO
