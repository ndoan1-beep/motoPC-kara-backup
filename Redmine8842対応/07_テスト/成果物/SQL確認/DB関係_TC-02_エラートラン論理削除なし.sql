BEGIN TRANSACTION

-- TC-02 エラートランを論理削除しないこと
-- 期待結果:
--   SARSlipHeaderError.LogicalDelFlg = '0'
--   SARSlipDetailError.LogicalDelFlg = '0'

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @InputMediaDiv nvarchar(1) = N'1';
DECLARE @InputBatchNo nvarchar(13) = N'RB884200001';
DECLARE @InputSequenceNo int = 1;
DECLARE @InputDate date = '2026-07-01';
DECLARE @InputTime nvarchar(6) = N'101500';
DECLARE @DataType nvarchar(2) = N'01';
DECLARE @UptakeSequenceNo int = 1;

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_SARSlipHeaderError' AS EvidenceName,
           CorporationCode, InputMediaDiv, InputBatchNo, InputSequenceNo,
           InputDate, InputTime, UptakeSequenceNo, SlipNo, FixDate, LogicalDelFlg, LogicalDelDate
    FROM SARSlipHeaderError
    WHERE CorporationCode = @CorporationCode
      AND InputMediaDiv = @InputMediaDiv
      AND InputBatchNo = @InputBatchNo
      AND InputSequenceNo = @InputSequenceNo
      AND InputDate = @InputDate
      AND InputTime = @InputTime
      AND UptakeSequenceNo = @UptakeSequenceNo;

    SELECT 'Before_SARSlipDetailError' AS EvidenceName,
           CorporationCode, InputMediaDiv, InputBatchNo, InputSequenceNo,
           InputSequenceLineNo, InputDate, InputTime, DataType,
           UptakeSequenceNo, UptakeSequenceLineNo, SlipNo, SlipLineNo, LogicalDelFlg, LogicalDelDate
    FROM SARSlipDetailError
    WHERE CorporationCode = @CorporationCode
      AND InputMediaDiv = @InputMediaDiv
      AND InputBatchNo = @InputBatchNo
      AND InputSequenceNo = @InputSequenceNo
      AND InputDate = @InputDate
      AND InputTime = @InputTime
      AND DataType = @DataType
      AND UptakeSequenceNo = @UptakeSequenceNo
    ORDER BY InputSequenceLineNo, UptakeSequenceLineNo;
END

-- テストデータのINSERT
BEGIN
    DELETE FROM SARSlipDetailError WHERE CorporationCode = '1000' AND InputBatchNo = 'RB884200001';
    DELETE FROM SARSlipHeaderError WHERE CorporationCode = '1000' AND InputBatchNo = 'RB884200001';

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

    INSERT INTO SARSlipDetailError
    (
        CorporationCode, InputMediaDiv, InputBatchNo, InputSequenceNo, InputSequenceLineNo,
        InputDate, InputTime, DataType, UptakeSequenceNo, UptakeSequenceLineNo,
        SlipLineNo, StoreCd, SlipTypeCd, SlipDiv, SlipNo, DisposalChangeDate,
        Category1Cd, Category2Cd, Category3Cd, Category4Cd, SKUCd, ScanningCd,
        DisposalChangeQty, SlipOldPrice, SlipNewPrice, SlipDisposalUnitPrica,
        SalesChangeDiv, Reason, SalesChangePrice, DisposalPriceAmt, ErrorDiv,
        ErrorMessage, LogicalDelDate, LogicalDelFlg, CreateBy, CreateDate, UpdateBy, UpdateDate
    )
    VALUES
    ('1000', '1', 'RB884200001', '1', 1, '2026-07-01', '101500', '01', 1, 1,
     '000001', '0000001001', '003', '01', '8842000000001', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000001', '49000000000001',
     3.00, 200, 150, 100, '1', 'REDMINE8842', 150, 300, '0',
     '', '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '1', 'RB884200001', '1', 2, '2026-07-01', '101500', '01', 1, 2,
     '000002', '0000001001', '003', '01', '8842000000001', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000002', '49000000000002',
     5.00, 220, 180, 100, '1', 'REDMINE8842', 180, 500, '0',
     '', '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE());
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_SARSlipHeaderError' AS EvidenceName,
           SlipNo, FixDate, LogicalDelFlg, LogicalDelDate,
           CASE WHEN LogicalDelFlg = '0' THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARSlipHeaderError
    WHERE CorporationCode = @CorporationCode
      AND InputMediaDiv = @InputMediaDiv
      AND InputBatchNo = @InputBatchNo
      AND InputSequenceNo = @InputSequenceNo
      AND InputDate = @InputDate
      AND InputTime = @InputTime
      AND UptakeSequenceNo = @UptakeSequenceNo;

    SELECT 'After_SARSlipDetailError' AS EvidenceName,
           SlipNo, SlipLineNo, LogicalDelFlg, LogicalDelDate,
           CASE WHEN LogicalDelFlg = '0' THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARSlipDetailError
    WHERE CorporationCode = @CorporationCode
      AND InputMediaDiv = @InputMediaDiv
      AND InputBatchNo = @InputBatchNo
      AND InputSequenceNo = @InputSequenceNo
      AND InputDate = @InputDate
      AND InputTime = @InputTime
      AND DataType = @DataType
      AND UptakeSequenceNo = @UptakeSequenceNo
    ORDER BY InputSequenceLineNo, UptakeSequenceLineNo;
END

ROLLBACK TRANSACTION
