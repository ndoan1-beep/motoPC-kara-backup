BEGIN TRANSACTION

-- TC-08 SQL指定項目の元伝票と赤伝の合計が0になること

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000001';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_HeaderSum' AS EvidenceName,
           SUM(TotalQty) AS TotalQtySum,
           SUM(SlipTotalDisposalAmt) AS SlipTotalDisposalAmtSum
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
      AND LogicalDelFlg = '0';

    SELECT 'Before_DetailSum' AS EvidenceName,
           SUM(DisposalChangeQty) AS DisposalChangeQtySum,
           SUM(DisposalPriceAmt) AS DisposalPriceAmtSum
    FROM SARSlipDetail
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
      AND LogicalDelFlg = '0';
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
    SELECT 'After_HeaderSum' AS EvidenceName,
           SUM(TotalQty) AS TotalQtySum,
           SUM(SlipTotalDisposalAmt) AS SlipTotalDisposalAmtSum,
           CASE WHEN SUM(TotalQty) = 0 AND SUM(SlipTotalDisposalAmt) = 0 THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
      AND LogicalDelFlg = '0';

    SELECT 'After_DetailSum' AS EvidenceName,
           SUM(DisposalChangeQty) AS DisposalChangeQtySum,
           SUM(DisposalPriceAmt) AS DisposalPriceAmtSum,
           CASE WHEN SUM(DisposalChangeQty) = 0 AND SUM(DisposalPriceAmt) = 0 THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARSlipDetail
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
      AND LogicalDelFlg = '0';
END

ROLLBACK TRANSACTION
