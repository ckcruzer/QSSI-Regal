
/****** Object:  View [dbo].[BSP_ItemExtenderV]    Script Date: 5/29/2020 11:18:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[BSP_Powerhouse_ItemExtV]
AS
SELECT        MFGCOST01_Key1 AS ITEMNMBR, MFGCOST01_219_ProductType AS PRODUCTCLASS, DEX_ROW_ID
FROM            (SELECT        dbo.EXT01100.Extender_Key_Values_1 AS MFGCOST01_Key1, B219.MFGCOST01_219_ProductType, EXT01100.DEX_ROW_ID
                          FROM            dbo.EXT01100 LEFT OUTER JOIN
                                                        (SELECT        Extender_Record_ID, STRGA255 AS MFGCOST01_219_ProductType
                                                          FROM            dbo.EXT01101
                                                          WHERE        (Field_ID = 219)) AS B219 ON dbo.EXT01100.Extender_Record_ID = B219.Extender_Record_ID
                          WHERE        (dbo.EXT01100.Extender_Window_ID = 'MFGCOST01')) AS A1

GO
