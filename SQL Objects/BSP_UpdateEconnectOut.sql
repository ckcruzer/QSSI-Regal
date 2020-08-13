-- ================================================
-- Template generated from Template Explorer using:
-- Create Trigger (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- See additional Create Trigger templates for more
-- examples of different Trigger statements.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ric
-- Create date: 
-- Description:	
-- =============================================
CREATE TRIGGER dbo.BSP_UpdateEconnectOut 
   ON  dbo.BSP_Powerhouse_SETP 
   AFTER INSERT,UPDATE
AS 
BEGIN
	DECLARE @EnableIVItemAutoSync TINYINT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	SELECT TOP 1 @EnableIVItemAutoSync = BSP_EnableIVItemAutoSync FROM BSP_Powerhouse_SETP

	IF @EnableIVItemAutoSync = 1 
	BEGIN 
		UPDATE eConnect_Out_Setup SET INSERT_ENABLED = 1, UPDATE_ENABLED = 1 WHERE DOCTYPE = 'BSP_Item'
	END
	ELSE
	BEGIN
		UPDATE eConnect_Out_Setup SET INSERT_ENABLED = 0, UPDATE_ENABLED = 0 WHERE DOCTYPE = 'BSP_Item'
	END
END
GO
