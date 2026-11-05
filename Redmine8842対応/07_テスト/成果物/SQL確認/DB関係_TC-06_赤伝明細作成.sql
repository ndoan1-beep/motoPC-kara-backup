BEGIN TRANSACTION

-- TC-06 SARSlipDetail に新規赤伝明細が作成されること
-- 期待結果:
--   元明細と同じ行数の赤伝明細が追加される。
--   赤伝明細 LogicalDelFlg = '0'

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000001';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_DetailBranchList' AS EvidenceName,
           SlipBranchNo, SlipLineNo, DisposalChangeQty, DisposalPriceAmt, LogicalDelFlg
    FROM SARSlipDetail
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
    ORDER BY SlipBranchNo, SlipLineNo;
END

-- テストデータのINSERT
BEGIN
    DELETE FROM SARSlipDetail WHERE CorporationCode = '1000' AND StoreCd = '0000001001' AND SlipNo = '8842000000001';

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
    SELECT 'After_RedDetailCount' AS EvidenceName,
           COUNT(*) AS RedDetailCount,
           CASE WHEN COUNT(*) = 2 THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARSlipDetail
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
      AND SlipBranchNo = 2
      AND LogicalDelFlg = '0';

    SELECT 'After_DetailBranchList' AS EvidenceName,
           SlipBranchNo, SlipLineNo, DisposalChangeQty, DisposalPriceAmt, LogicalDelFlg
    FROM SARSlipDetail
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
    ORDER BY SlipBranchNo, SlipLineNo;
END

ROLLBACK TRANSACTION
