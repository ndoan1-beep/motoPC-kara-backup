BEGIN TRANSACTION

-- TC-12 画面一覧で元伝票と新規登録赤伝の「修正」ボタンが表示されないこと
-- DB証跡:
--   SARSlipHeader.SlipRevisionFlg = '1'
--   SARSlipHeaderError.FixDate = '9999/12/31'
-- 画面証跡:
--   一覧の修正ボタン非表示を画面キャプチャで添付する。

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000001';
DECLARE @InputMediaDiv nvarchar(1) = N'1';
DECLARE @InputBatchNo nvarchar(13) = N'RB884200001';
DECLARE @InputSequenceNo int = 1;
DECLARE @InputDate date = '2026-07-01';
DECLARE @InputTime nvarchar(6) = N'101500';
DECLARE @UptakeSequenceNo int = 1;

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_HeaderCorrectionAvailability' AS EvidenceName,
           SlipBranchNo, RedBlackDiv, SlipRevisionFlg, LogicalDelFlg
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
    ORDER BY SlipBranchNo;

    SELECT 'Before_HeaderErrorCorrectionAvailability' AS EvidenceName,
           SlipNo, FixDate, LogicalDelFlg
    FROM SARSlipHeaderError
    WHERE CorporationCode = @CorporationCode
      AND InputMediaDiv = @InputMediaDiv
      AND InputBatchNo = @InputBatchNo
      AND InputSequenceNo = @InputSequenceNo
      AND InputDate = @InputDate
      AND InputTime = @InputTime
      AND UptakeSequenceNo = @UptakeSequenceNo;
END

-- テストデータのINSERT
BEGIN
    DELETE FROM SARSlipHeaderError WHERE CorporationCode = '1000' AND InputBatchNo = 'RB884200001';
    DELETE FROM SARSlipHeader WHERE CorporationCode = '1000' AND StoreCd = '0000001001' AND SlipNo = '8842000000001';

    INSERT INTO SARSlipHeader
    (
        CorporationCode, SlipDiv, SlipTypeCd, SlipNo, SlipBranchNo, DisposalChangeDate,
        StoreCd, Category1Cd, Category2Cd, TotalQty, SlipOldTotalPrice, SlipNewTotalPrice,
        AutoSalesChangeFlg, SlipTotalDisposalAmt, SlipDate, PostingDate, ApproverCd,
        ApprovalStatus, SlipRevisionFlg, RedBlackDiv, CorrectionDiv, InputMediaDiv,
        InputBatchNo, InputSequenceNo, LogicalDelDate, LogicalDelFlg,
        CreateBy, CreateDate, UpdateBy, UpdateDate
    )
    VALUES
    ('1000', '01', '003', '8842000000001', 1, '2026-07-01',
     '0000001001', '0101', '0201', 10.00, 2000, 1500, '0', 1000,
     '2026-07-01', '2026-07-01', '', '0', '1', '0', '0', '1',
     'RB884200001', '1', '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '01', '003', '8842000000001', 2, '2026-07-01',
     '0000001001', '0101', '0201', -10.00, 2000, 1500, '0', -1000,
     '2026-07-01', '2026-07-01', '', '0', '1', '1', '0', '1',
     'RB884200001', '1', '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE());

    INSERT INTO SARSlipHeaderError
    (
        CorporationCode, InputMediaDiv, InputBatchNo, InputSequenceNo, InputDate, InputTime,
        DataType, UptakeSequenceNo, SlipDiv, SlipTypeCd, SlipNo, DisposalChangeDate,
        StoreCd, Category1Cd, Category2Cd, TotalQty, SlipOldTotalPrice, SlipNewTotalPrice,
        AutoSalesChangeFlg, SlipTotalDisposalAmt, SlipDate, ApproverCd, ApprovalStatus,
        ApprovalDate, ErrorDiv, ErrorMessage, RedBlackDiv, CorrectionDiv, FixDate,
        LogicalDelDate, LogicalDelFlg, CreateBy, CreateDate, UpdateBy, UpdateDate
    )
    VALUES
    (
        '1000', '1', 'RB884200001', '1', '2026-07-01', '101500',
        '01', 1, '01', '003', '8842000000001', '2026-07-01',
        '0000001001', '0101', '0201', 10.00, 2000, 1500,
        '0', 1000, '2026-07-01', '', '0',
        '9999-12-31', '0', '', '0', '0', '9999-12-31',
        '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()
    );
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_HeaderCorrectionAvailability' AS EvidenceName,
           SlipBranchNo, RedBlackDiv, SlipRevisionFlg, LogicalDelFlg,
           CASE WHEN SlipRevisionFlg = '1' AND LogicalDelFlg = '0' THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
    ORDER BY SlipBranchNo;

    SELECT 'After_HeaderErrorCorrectionAvailability' AS EvidenceName,
           SlipNo, FixDate, LogicalDelFlg,
           CASE
             WHEN CONVERT(nvarchar(10), FixDate, 111) = '9999/12/31' AND LogicalDelFlg = '0' THEN 'OK'
             ELSE 'NG'
           END AS Judge
    FROM SARSlipHeaderError
    WHERE CorporationCode = @CorporationCode
      AND InputMediaDiv = @InputMediaDiv
      AND InputBatchNo = @InputBatchNo
      AND InputSequenceNo = @InputSequenceNo
      AND InputDate = @InputDate
      AND InputTime = @InputTime
      AND UptakeSequenceNo = @UptakeSequenceNo;
END

ROLLBACK TRANSACTION
