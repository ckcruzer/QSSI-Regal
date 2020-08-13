﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.PowerHouse.DynamicsGP.Integration.Domain
{
    public partial class SOP10100_DTO
    {
        public short SOPTYPE { get; set; }
        public string SOPNUMBE { get; set; }
        public short ORIGTYPE { get; set; }
        public string ORIGNUMB { get; set; }
        public string DOCID { get; set; }
        public DateTime DOCDATE { get; set; }
        public DateTime GLPOSTDT { get; set; }
        public DateTime QUOTEDAT { get; set; }
        public DateTime QUOEXPDA { get; set; }
        public DateTime ORDRDATE { get; set; }
        public DateTime INVODATE { get; set; }
        public DateTime BACKDATE { get; set; }
        public DateTime RETUDATE { get; set; }
        public DateTime ReqShipDate { get; set; }
        public DateTime FUFILDAT { get; set; }
        public DateTime ACTLSHIP { get; set; }
        public DateTime DISCDATE { get; set; }
        public DateTime DUEDATE { get; set; }
        public byte REPTING { get; set; }
        public short TRXFREQU { get; set; }
        public short TIMEREPD { get; set; }
        public short TIMETREP { get; set; }
        public short DYSTINCR { get; set; }
        public DateTime DTLSTREP { get; set; }
        public string DSTBTCH1 { get; set; }
        public string DSTBTCH2 { get; set; }
        public string USDOCID1 { get; set; }
        public string USDOCID2 { get; set; }
        public decimal DISCFRGT { get; set; }
        public decimal ORDAVFRT { get; set; }
        public decimal DISCMISC { get; set; }
        public decimal ORDAVMSC { get; set; }
        public decimal DISAVAMT { get; set; }
        public decimal ORDAVAMT { get; set; }
        public decimal DISCRTND { get; set; }
        public decimal ORDISRTD { get; set; }
        public decimal DISTKNAM { get; set; }
        public decimal ORDISTKN { get; set; }
        public short DSCPCTAM { get; set; }
        public decimal DSCDLRAM { get; set; }
        public decimal ORDDLRAT { get; set; }
        public decimal DISAVTKN { get; set; }
        public decimal ORDATKN { get; set; }
        public string PYMTRMID { get; set; }
        public string PRCLEVEL { get; set; }
        public string LOCNCODE { get; set; }
        public string BCHSOURC { get; set; }
        public string BACHNUMB { get; set; }
        public string CUSTNMBR { get; set; }
        public string CUSTNAME { get; set; }
        public string CSTPONBR { get; set; }
        public short PROSPECT { get; set; }
        public int MSTRNUMB { get; set; }
        public string PCKSLPNO { get; set; }
        public string PICTICNU { get; set; }
        public decimal MRKDNAMT { get; set; }
        public decimal ORMRKDAM { get; set; }
        public string PRBTADCD { get; set; }
        public string PRSTADCD { get; set; }
        public string CNTCPRSN { get; set; }
        public string ShipToName { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string ADDRESS3 { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIPCODE { get; set; }
        public string CCode { get; set; }
        public string COUNTRY { get; set; }
        public string PHNUMBR1 { get; set; }
        public string PHNUMBR2 { get; set; }
        public string PHONE3 { get; set; }
        public string FAXNUMBR { get; set; }
        public short COMAPPTO { get; set; }
        public decimal COMMAMNT { get; set; }
        public decimal OCOMMAMT { get; set; }
        public decimal CMMSLAMT { get; set; }
        public decimal ORCOSAMT { get; set; }
        public decimal NCOMAMNT { get; set; }
        public decimal ORNCMAMT { get; set; }
        public string SHIPMTHD { get; set; }
        public decimal TRDISAMT { get; set; }
        public decimal ORTDISAM { get; set; }
        public short TRDISPCT { get; set; }
        public decimal SUBTOTAL { get; set; }
        public decimal ORSUBTOT { get; set; }
        public decimal REMSUBTO { get; set; }
        public decimal OREMSUBT { get; set; }
        public decimal EXTDCOST { get; set; }
        public decimal OREXTCST { get; set; }
        public decimal FRTAMNT { get; set; }
        public decimal ORFRTAMT { get; set; }
        public decimal MISCAMNT { get; set; }
        public decimal ORMISCAMT { get; set; }
        public byte TXENGCLD { get; set; }
        public string TAXEXMT1 { get; set; }
        public string TAXEXMT2 { get; set; }
        public string TXRGNNUM { get; set; }
        public string TAXSCHID { get; set; }
        public short TXSCHSRC { get; set; }
        public byte BSIVCTTL { get; set; }
        public string FRTSCHID { get; set; }
        public decimal FRTTXAMT { get; set; }
        public decimal ORFRTTAX { get; set; }
        public short FRGTTXBL { get; set; }
        public string MSCSCHID { get; set; }
        public decimal MSCTXAMT { get; set; }
        public decimal ORMSCTAX { get; set; }
        public short MISCTXBL { get; set; }
        public decimal BKTFRTAM { get; set; }
        public decimal ORBKTFRT { get; set; }
        public decimal BKTMSCAM { get; set; }
        public decimal ORBKTMSC { get; set; }
        public decimal BCKTXAMT { get; set; }
        public decimal OBTAXAMT { get; set; }
        public decimal TXBTXAMT { get; set; }
        public decimal OTAXTAMT { get; set; }
        public decimal TAXAMNT { get; set; }
        public decimal ORTAXAMT { get; set; }
        public byte ECTRX { get; set; }
        public decimal DOCAMNT { get; set; }
        public decimal ORDOCAMT { get; set; }
        public decimal PYMTRCVD { get; set; }
        public decimal ORPMTRVD { get; set; }
        public decimal DEPRECVD { get; set; }
        public decimal ORDEPRVD { get; set; }
        public decimal CODAMNT { get; set; }
        public decimal ORCODAMT { get; set; }
        public decimal ACCTAMNT { get; set; }
        public decimal ORACTAMT { get; set; }
        public string SALSTERR { get; set; }
        public string SLPRSNID { get; set; }
        public string UPSZONE { get; set; }
        public short TIMESPRT { get; set; }
        public short PSTGSTUS { get; set; }
        public short VOIDSTTS { get; set; }
        public short ALLOCABY { get; set; }
        public decimal NOTEINDX { get; set; }
        public string CURNCYID { get; set; }
        public short CURRNIDX { get; set; }
        public string RATETPID { get; set; }
        public string EXGTBLID { get; set; }
        public decimal XCHGRATE { get; set; }
        public decimal DENXRATE { get; set; }
        public DateTime EXCHDATE { get; set; }
        public DateTime TIME1 { get; set; }
        public short RTCLCMTD { get; set; }
        public short MCTRXSTT { get; set; }
        public string TRXSORCE { get; set; }
        public byte[] SOPHDRE1 { get; set; }
        public byte[] SOPHDRE2 { get; set; }
        public byte[] SOPLNERR { get; set; }
        public byte[] SOPHDRFL { get; set; }
        public byte[] SOPMCERR { get; set; }
        public string COMMNTID { get; set; }
        public string REFRENCE { get; set; }
        public DateTime POSTEDDT { get; set; }
        public string PTDUSRID { get; set; }
        public string USER2ENT { get; set; }
        public DateTime CREATDDT { get; set; }
        public DateTime MODIFDT { get; set; }
        public DateTime Tax_Date { get; set; }
        public byte APLYWITH { get; set; }
        public decimal WITHHAMT { get; set; }
        public byte SHPPGDOC { get; set; }
        public byte CORRCTN { get; set; }
        public byte SIMPLIFD { get; set; }
        public byte CORRNXST { get; set; }
        public string DOCNCORR { get; set; }
        public short SEQNCORR { get; set; }
        public DateTime SALEDATE { get; set; }
        public byte[] SOPHDRE3 { get; set; }
        public byte EXCEPTIONALDEMAND { get; set; }
        public short Flags { get; set; }
        public decimal BackoutTradeDisc { get; set; }
        public decimal OrigBackoutTradeDisc { get; set; }
        public string GPSFOINTEGRATIONID { get; set; }
        public short INTEGRATIONSOURCE { get; set; }
        public string INTEGRATIONID { get; set; }
        public short SOPSTATUS { get; set; }
        public byte SHIPCOMPLETE { get; set; }
        public byte DIRECTDEBIT { get; set; }
        public short WorkflowApprStatCreditLm { get; set; }
        public short WorkflowPriorityCreditLm { get; set; }
        public short WorkflowApprStatusQuote { get; set; }
        public short WorkflowPriorityQuote { get; set; }
        public short Workflow_Status { get; set; }
        public short ContractExchangeRateStat { get; set; }
        public short Print_Phone_NumberGB { get; set; }
    }
}
