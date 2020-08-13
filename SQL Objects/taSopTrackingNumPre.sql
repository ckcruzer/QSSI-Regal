/****** Object:  StoredProcedure [dbo].[taSopTrackingNumPre]    Script Date: 6/26/2020 10:25:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
 ALTER procedure [dbo].[taSopTrackingNumPre]  @I_vSOPTYPE smallint output,   @I_vSOPNUMBE char(21) output,    @I_vTracking_Number char(40) output,  @I_vType smallint output,   @I_vRequesterTrx smallint output,  @I_vUSRDEFND1 char(50) output,   @I_vUSRDEFND2 char(50) output,   @I_vUSRDEFND3 char(50) output,   @I_vUSRDEFND4 varchar(8000) output,  @I_vUSRDEFND5 varchar(8000) output,  @O_iErrorState int output,   @oErrString varchar(255) output    as  set nocount on  
 
 
 DELETE SOP10107 WHERE SOPNUMBE = @I_vSOPNUMBE AND SOPTYPE = @I_vSOPTYPE AND Tracking_Number = @I_vTracking_Number

 
 select @O_iErrorState = 0  return (@O_iErrorState)   