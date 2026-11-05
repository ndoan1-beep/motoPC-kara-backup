BEGIN TRANSACTION

-- TC-15 確認画面なしの場合
-- 期待結果:
--   DelSARSlipNew が即時実行され、TC-02からTC-13相当のDB結果になる。
--   画面に MSGCSARI0028 が表示される。

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000001';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_ImmediateCancel_Header' AS EvidenceName,
           SlipBranchNo, RedBlackDiv, SlipRevisionFlg, LogicalDelFlg, TotalQty, SlipTotalDisposalAmt
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY SlipBranchNo;
END

-- テストデータのINSERT
BEGIN
    DELETE FROM SARSlipDetail WHERE CorporationCode = '1000' AND StoreCd = '0000001001' AND SlipNo = '8842000000001';
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

    INSERT INTO SARSlipDetail
    (
        CorporationCode, SlipLineNo, StoreCd, SlipTypeCd, SlipBranchNo, SlipDiv,
        SlipNo, DisposalChangeDate, Category1Cd, Category2Cd, Category3Cd, Category4Cd,
        SKUCd, ScanningCd, DisposalChangeQty, SlipOldPrice, SlipNewPrice,
        SlipDisposalUnitPrica, SalesChangeDiv, Reason, SalesChangePrice,
        DisposalPriceAmt, LogicalDelDate, LogicalDelFlg, CreateBy, CreateDate, UpdateBy, UpdateDate
    )
    VALUES
    ('1000', '000001', '0000001001', '003', 1, '01', '8842000000001', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000001', '49000000000001',
     3.00, 200, 150, 100, '1', 'REDMINE8842', 150, 300, '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '000002', '0000001001', '003', 1, '01', '8842000000001', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000002', '49000000000002',
     5.00, 220, 180, 100, '1', 'REDMINE8842', 180, 500, '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '000001', '0000001001', '003', 2, '01', '8842000000001', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000001', '49000000000001',
     -3.00, 200, 150, 100, '1', 'REDMINE8842', 150, -300, '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '000002', '0000001001', '003', 2, '01', '8842000000001', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000002', '49000000000002',
     -5.00, 220, 180, 100, '1', 'REDMINE8842', 180, -500, '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE());
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_ImmediateCancel_Header' AS EvidenceName,
           SlipBranchNo, RedBlackDiv, SlipRevisionFlg, LogicalDelFlg, TotalQty, SlipTotalDisposalAmt,
           CASE
             WHEN SlipBranchNo = 1 AND LogicalDelFlg = '0' AND SlipRevisionFlg = '1' THEN 'OK_ORIGINAL'
             WHEN SlipBranchNo = 2 AND RedBlackDiv = '1' AND LogicalDelFlg = '0' AND TotalQty = -10 AND SlipTotalDisposalAmt = -1000 THEN 'OK_RED'
             ELSE 'CHECK'
           END AS Judge
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY SlipBranchNo;

    SELECT 'After_ImmediateCancel_Detail' AS EvidenceName,
           SlipBranchNo, SlipLineNo, LogicalDelFlg, DisposalChangeQty, DisposalPriceAmt
    FROM SARSlipDetail
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY SlipBranchNo, SlipLineNo;
END

ROLLBACK TRANSACTION
