/****** Object:  StoredProcedure [dbo].[BSP_SopUpdateEDIInsert]    Script Date: 4/26/2020 6:29:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ric
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[BSP_SopUpdateEDIInsert] 
	-- Add the parameters for the stored procedure here
	@I_vSOPTYPE smallint,
	@I_vSOPNUMBE char(21),
	@I_vLNITMSEQ int,
    @I_vVS_Sequence_Number int,
    @I_vVS_Carton_ASNSTR char(41),
	@I_vVS_Pallet_ASNSTR char(41),
    @I_vVS_Product_Code char(21),
	@I_vVS_QTY_PackedASN numeric(19,5),
	@I_vVS_QTY_PackedThisSlip numeric(19,5),
	@I_vVS_Carton_Weight int,
    @I_vVS_Total_Weight numeric(19,5),
    @I_vVS_Date_Shipped datetime,
    @I_vVS_Shipment_Number char(21),
    @I_vVS_Ship_Carrier_Used char(41),
    @I_vVS_Pickup_Number char(41),
    @I_vVS_TT_Number char(25),
    @I_vVS_Seal_Number char(17),
    @I_vVS_Trailer_Number char(17),
    @I_vVS_Load_Number char(17),
    @I_vVS_PRO_Number char(41),
    @I_vVS_SCC18 char(21),
    @I_vVS_SCAC char(15),
    @I_vVS_Carrier_Name char(15),
    @I_vVS_Carton_Dimensions char(15),
    @I_vVS_Carrier_Type smallint,
    @I_vVS_Payment_Method smallint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[VSASN01]
           ([SOPTYPE]
           ,[SOPNUMBE]
           ,[LNITMSEQ]
           ,[VS_Sequence_Number]
           ,[VS_Carton_ASNSTR]
           ,[VS_Pallet_ASNSTR]
           ,[VS_Product_Code]
           ,[VS_QTY_PackedASN]
           ,[VS_QTY_PackedThisSlip]
           ,[VS_Carton_Weight]
           ,[VS_Total_Weight]
           ,[VS_Date_Shipped]
           ,[VS_Shipment_Number]
           ,[VS_Ship_Carrier_Used]
           ,[VS_Pickup_Number]
           ,[VS_TT_Number]
           ,[VS_Seal_Number]
           ,[VS_Trailer_Number]
           ,[VS_Load_Number]
           ,[VS_PRO_Number]
           ,[VS_SCC18]
           ,[VS_SCAC]
           ,[VS_Carrier_Name]
           ,[VS_Carton_Dimensions]
           ,[VS_Carrier_Type]
           ,[VS_Payment_Method])
	 VALUES
           (@I_vSOPTYPE,
           @I_vSOPNUMBE,
           @I_vLNITMSEQ,
           @I_vVS_Sequence_Number,
           @I_vVS_Carton_ASNSTR,
           @I_vVS_Pallet_ASNSTR,
           @I_vVS_Product_Code,
           @I_vVS_QTY_PackedASN,
           @I_vVS_QTY_PackedThisSlip,
           @I_vVS_Carton_Weight,
           @I_vVS_Total_Weight,
           @I_vVS_Date_Shipped,
           @I_vVS_Shipment_Number,
           @I_vVS_Ship_Carrier_Used,
           @I_vVS_Pickup_Number,
           @I_vVS_TT_Number,
           @I_vVS_Seal_Number,
           @I_vVS_Trailer_Number,
           @I_vVS_Load_Number,
           @I_vVS_PRO_Number,
           @I_vVS_SCC18,
           @I_vVS_SCAC,
           @I_vVS_Carrier_Name,
           @I_vVS_Carton_Dimensions,
           @I_vVS_Carrier_Type,
           @I_vVS_Payment_Method)
END
