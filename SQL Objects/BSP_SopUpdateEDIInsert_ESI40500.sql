USE [TWO]
GO
/****** Object:  StoredProcedure [dbo].[BSP_SopUpdateEDIInsert]    Script Date: 8/10/2021 6:58:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ric
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[BSP_SopUpdateEDIInsert_ESI40500] 
	-- Add the parameters for the stored procedure here
	@I_ESIPKCID char(11),
	@I_ESICONTY char(11),
	@I_ESIUOFM1 char(3),
    @I_ESICONHT NUMERIC(19, 5),
	@I_ESICONWD NUMERIC(19, 5),
	@I_ESICONLN NUMERIC(19, 5),
	@I_ESICONCF NUMERIC(19, 5)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[ESI40500]
           ([ESIPKCID]
           ,[ESICONTY]
           ,[ESIUOFM1]
           ,[ESICONHT]
           ,[ESICONWD]
           ,[ESICONLN]
           ,[ESICONCF])
	 VALUES
           (@I_ESIPKCID
           ,@I_ESICONTY
           ,@I_ESIUOFM1
           ,@I_ESICONHT
           ,@I_ESICONWD
           ,@I_ESICONLN
           ,@I_ESICONCF)
END
