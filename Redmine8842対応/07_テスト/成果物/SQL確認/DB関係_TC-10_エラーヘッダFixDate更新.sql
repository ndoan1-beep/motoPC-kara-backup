BEGIN TRANSACTION

-- TC-10 SARSlipHeaderError の訂正不可用更新が行われること
-- 期待結果:
--   FixDate = '9999/12/31' または DB日付型表示で 9999-12-31
--   LogicalDelFlg = '0'

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @InputMediaDiv nvarchar(1) = N'1';
DECLARE @InputBatchNo nvarchar(13) = N'RB884200001';
DECLARE @InputSequenceNo int = 1;
DECLARE @InputDate date = '2026-07-01';
DECLARE @InputTime nvarchar(6) = N'101500';
DECLARE @UptakeSequenceNo int = 1;

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_HeaderErrorFixDate' AS EvidenceName,
           SlipNo, FixDate, LogicalDelFlg, LogicalDelDate
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
    SELECT 'After_HeaderErrorFixDate' AS EvidenceName,
           SlipNo, FixDate, LogicalDelFlg, LogicalDelDate,
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
