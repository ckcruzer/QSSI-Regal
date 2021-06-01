/****** Object:  StoredProcedure [dbo].[taSopHdrIvcInsertPre]    Script Date: 6/1/2021 6:37:09 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


ALTER procedure [dbo].[taSopHdrIvcInsertPre] @I_vSOPTYPE smallint output, 
  @I_vDOCID char(15) output, 
  @I_vSOPNUMBE char(21) output, 
  @I_vORIGNUMB char(21) output, 
  @I_vORIGTYPE smallint output, 
  @I_vTAXSCHID char(15) output, 
  @I_vFRTSCHID char(15) output, 
  @I_vMSCSCHID char(15) output, 
  @I_vSHIPMTHD char(15) output, 
  @I_vTAXAMNT numeric(19, 5) output, 
  @I_vLOCNCODE char(10) output, 
  @I_vDOCDATE datetime output, 
  @I_vFREIGHT numeric(19, 5) output, 
  @I_vMISCAMNT numeric(19, 5) output, 
  @I_vTRDISAMT numeric(19, 5) output, 
  @I_vTRADEPCT numeric(19, 2) output, 
  @I_vDISTKNAM numeric(19, 5) output, 
  @I_vMRKDNAMT numeric(19, 5) output, 
  @I_vCUSTNMBR char(15) output, 
  @I_vCUSTNAME char(64) output, 
  @I_vCSTPONBR char(20) output, 
  @I_vShipToName char(64) output, 
  @I_vADDRESS1 char(60) output, 
  @I_vADDRESS2 char(60) output, 
  @I_vADDRESS3 char(60) output, 
  @I_vCNTCPRSN char(60) output, 
  @I_vFAXNUMBR char(21) output, 
  @I_vCITY char(35) output, 
  @I_vSTATE char(29) output, 
  @I_vZIPCODE char(10) output, 
  @I_vCOUNTRY char(60) output, 
  @I_vPHNUMBR1 char(21) output, 
  @I_vPHNUMBR2 char(21) output, 
  @I_vPHNUMBR3 char(21) output, 
  @I_vPrint_Phone_NumberGB smallint output, 
  @I_vSUBTOTAL numeric(19, 5) output, 
  @I_vDOCAMNT numeric(19, 5) output, 
  @I_vPYMTRCVD numeric(19, 5) output, 
  @I_vSALSTERR char(15) output, 
  @I_vSLPRSNID char(15) output, 
  @I_vUPSZONE char(3) output, 
  @I_vUSER2ENT char(15) output, 
  @I_vBACHNUMB char(15) output, 
  @I_vPRBTADCD char(15) output, 
  @I_vPRSTADCD char(15) output, 
  @I_vFRTTXAMT numeric(19, 5) output, 
  @I_vMSCTXAMT numeric(19, 5) output, 
  @I_vORDRDATE datetime output, 
  @I_vMSTRNUMB int output, 
  @I_vPYMTRMID char(20) output, 
  @I_vDUEDATE datetime output, 
  @I_vDISCDATE datetime output, 
  @I_vREFRENCE char(30) output, 
  @I_vUSINGHEADERLEVELTAXES int output, 
  @I_vBatchCHEKBKID char(15) output, 
  @I_vCREATECOMM smallint output, 
  @I_vCOMMAMNT numeric(19, 2) output, 
  @I_vCOMPRCNT numeric(19, 2) output, 
  @I_vCREATEDIST smallint output, 
  @I_vCREATETAXES smallint output, 
  @I_vDEFTAXSCHDS smallint output, 
  @I_vCURNCYID char(15) output, 
  @I_vXCHGRATE numeric(19, 7) output, 
  @I_vRATETPID char(15) output, 
  @I_vEXPNDATE datetime output, 
  @I_vEXCHDATE datetime output, 
  @I_vEXGTBDSC char(30) output, 
  @I_vEXTBLSRC char(50) output, 
  @I_vRATEEXPR smallint output, 
  @I_vDYSTINCR smallint output, 
  @I_vRATEVARC numeric(19, 7) output, 
  @I_vTRXDTDEF smallint output, 
  @I_vRTCLCMTD smallint output, 
  @I_vPRVDSLMT smallint output, 
  @I_vDATELMTS smallint output, 
  @I_vTIME1 datetime output, 
  @I_vDISAVAMT numeric(19, 2) output, 
  @I_vDSCDLRAM numeric(19, 5) output, 
  @I_vDSCPCTAM numeric(19, 2) output, 
  @I_vFREIGTBLE int output, 
  @I_vMISCTBLE int output, 
  @I_vCOMMNTID char(15) output, 
  @I_vCOMMENT_1 char(50) output, 
  @I_vCOMMENT_2 char(50) output, 
  @I_vCOMMENT_3 char(50) output, 
  @I_vCOMMENT_4 char(50) output, 
  @I_vGPSFOINTEGRATIONID char(30) output, 
  @I_vINTEGRATIONSOURCE smallint output, 
  @I_vINTEGRATIONID char(30) output, 
  @I_vReqShipDate datetime output, 
  @I_vRequesterTrx smallint output, 
  @I_vCKCreditLimit tinyint output, 
  @I_vCKHOLD tinyint output, 
  @I_vUpdateExisting tinyint output, 
  @I_vQUOEXPDA datetime output, 
  @I_vQUOTEDAT datetime output, 
  @I_vINVODATE datetime output, 
  @I_vBACKDATE datetime output, 
  @I_vRETUDATE datetime output, 
  @I_vCMMTTEXT varchar(500) output, 
  @I_vPRCLEVEL char(10) output, 
  @I_vDEFPRICING tinyint output, 
  @I_vTAXEXMT1 char(25) output, 
  @I_vTAXEXMT2 char(25) output, 
  @I_vTXRGNNUM char(25) output, 
  @I_vREPTING tinyint output, 
  @I_vTRXFREQU smallint output, 
  @I_vTIMETREP smallint output, 
  @I_vQUOTEDYSTINCR smallint output, 
  @I_vNOTETEXT varchar(8000) output, 
  @I_vUSRDEFND1 char(50) output, 
  @I_vUSRDEFND2 char(50) output, 
  @I_vUSRDEFND3 char(50) output, 
  @I_vUSRDEFND4 varchar(8000) output, 
  @I_vUSRDEFND5 varchar(8000) output, 
  @O_iErrorState int output, 
  @oErrString varchar(255) output as 
SET
  NOCOUNT ON

DECLARE @FUNCRIDX INT

SELECT @O_iErrorState = 0 

SELECT TOP 1 @FUNCRIDX = FUNCRIDX FROM mc40000 (nolock)

SELECT @I_vPYMTRCVD = ISNULL(SUM(AMNTPAID), 0) FROM SOP10103 
WHERE SOPTYPE = @I_vSOPTYPE AND SOPNUMBE = @I_vSOPNUMBE 

--Update the line items currindx
UPDATE SOP10200
SET CURRNIDX = @FUNCRIDX
WHERE SOPNUMBE = @I_vSOPNUMBE
AND SOPTYPE = @I_vSOPTYPE
--AND CMPNTSEQ = 0
AND CURRNIDX = 0
  
RETURN (@O_iErrorState)

GO


