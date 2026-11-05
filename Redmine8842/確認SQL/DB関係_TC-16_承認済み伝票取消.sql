BEGIN TRANSACTION

-- TC-16 承認済み伝票の場合
-- 期待結果:
--   承認済み伝票でも赤伝ヘッダ・赤伝明細が作成される。
--   訂正不可用更新が行われる。
--   CMCAccountBook DataID = '003' の登録・更新動作は既存仕様通りである。

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000002';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_ApprovedHeader' AS EvidenceName,
           SlipBranchNo, ApprovalStatus, RedBlackDiv, SlipRevisionFlg,
           LogicalDelFlg, TotalQty, SlipTotalDisposalAmt
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY SlipBranchNo;

    SELECT 'Before_ApprovedAccountBookCount' AS EvidenceName,
           COUNT(*) AS AccountBookCount
    FROM CMCAccountBook
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND DataID = '003'
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo;
END

-- テストデータのINSERT
BEGIN
    DELETE FROM SARSlipDetail WHERE CorporationCode = '1000' AND StoreCd = '0000001001' AND SlipNo = '8842000000002';
    DELETE FROM SARSlipHeader WHERE CorporationCode = '1000' AND StoreCd = '0000001001' AND SlipNo = '8842000000002';
    DELETE FROM CMCAccountBook
    WHERE CorporationCode = '1000'
      AND StoreCd = '0000001001'
      AND DataID = '003'
      AND SlipTypeCd = '003'
      AND SlipDiv = '01'
      AND SlipNo = '8842000000002';

    INSERT INTO SARSlipHeader
    (
        CorporationCode, SlipDiv, SlipTypeCd, SlipNo, SlipBranchNo, DisposalChangeDate,
        StoreCd, Category1Cd, Category2Cd, TotalQty, SlipOldTotalPrice, SlipNewTotalPrice,
        AutoSalesChangeFlg, SlipTotalDisposalAmt, SlipDate, PostingDate, ApproverCd,
        ApprovalStatus, ApprovalDate, SlipRevisionFlg, RedBlackDiv, CorrectionDiv, InputMediaDiv,
        InputBatchNo, InputSequenceNo, LogicalDelDate, LogicalDelFlg,
        CreateBy, CreateDate, UpdateBy, UpdateDate
    )
    VALUES
    ('1000', '01', '003', '8842000000002', 1, '2026-07-01',
     '0000001001', '0101', '0201', 10.00, 2000, 1500, '0', 1000,
     '2026-07-01', '2026-07-01', 'APPROVER01', '3', '2026-07-01', '1', '0', '0', '1',
     'RB884200002', '1', '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '01', '003', '8842000000002', 2, '2026-07-01',
     '0000001001', '0101', '0201', -10.00, 2000, 1500, '0', -1000,
     '2026-07-01', '2026-07-01', 'APPROVER01', '3', '2026-07-01', '1', '1', '0', '1',
     'RB884200002', '1', '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE());

    INSERT INTO SARSlipDetail
    (
        CorporationCode, SlipLineNo, StoreCd, SlipTypeCd, SlipBranchNo, SlipDiv,
        SlipNo, DisposalChangeDate, Category1Cd, Category2Cd, Category3Cd, Category4Cd,
        SKUCd, ScanningCd, DisposalChangeQty, SlipOldPrice, SlipNewPrice,
        SlipDisposalUnitPrica, SalesChangeDiv, Reason, SalesChangePrice,
        DisposalPriceAmt, LogicalDelDate, LogicalDelFlg, CreateBy, CreateDate, UpdateBy, UpdateDate
    )
    VALUES
    ('1000', '000001', '0000001001', '003', 1, '01', '8842000000002', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000001', '49000000000001',
     3.00, 200, 150, 100, '1', 'REDMINE8842', 150, 300, '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '000002', '0000001001', '003', 1, '01', '8842000000002', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000002', '49000000000002',
     5.00, 220, 180, 100, '1', 'REDMINE8842', 180, 500, '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '000001', '0000001001', '003', 2, '01', '8842000000002', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000001', '49000000000001',
     -3.00, 200, 150, 100, '1', 'REDMINE8842', 150, -300, '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('1000', '000002', '0000001001', '003', 2, '01', '8842000000002', '2026-07-01',
     '0101', '0201', '0301', '0401', '49000000000002', '49000000000002',
     -5.00, 220, 180, 100, '1', 'REDMINE8842', 180, -500, '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE());

    INSERT INTO CMCAccountBook
    (
        AccountBookRegistrationDate, SubSystem, UserCd, DataID, SlipDate,
        SlipTypeCd, SlipDiv, SlipNo, SlipLineNo, CorrectionDiv, InputBatchNo,
        InputMediaDiv, DataUnit, CorporationCode, StoreCd, Category1Cd, Category2Cd,
        Category3Cd, Category4Cd, SKUCd, ScanningCd, SalesChangeDiv, TargetQty,
        IncludeTaxCostAmt, PretaxSellPrice, BeforetaxSellPrice, InvControlDiv,
        InvPostingDate, AccountBookRemarks1, AccountBookRemarks2, UpdateScreenName,
        UpdateBatchName, LogicalDelDate, LogicalDelFlg, CreateBy, CreateDate, UpdateBy, UpdateDate
    )
    VALUES
    ('2026-07-01', 'SA', 'TESTUSER', '003', '2026-07-01',
     '003', '01', '8842000000002', '000001', '+', 'RB884200002',
     '1', '1', '1000', '0000001001', '0101', '0201',
     '0301', '0401', '49000000000001', '49000000000001', '1', 3.00,
     300, 450, 600, '1', '2026-07-01', 'Redmine8842', 'Before',
     'SCRCSAR050205', 'Redmine8842_Test', '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()),
    ('2026-07-01', 'SA', 'TESTUSER', '003', '2026-07-01',
     '003', '01', '8842000000002', '000002', '+', 'RB884200002',
     '1', '1', '1000', '0000001001', '0101', '0201',
     '0301', '0401', '49000000000002', '49000000000002', '1', 5.00,
     500, 900, 1100, '1', '2026-07-01', 'Redmine8842', 'Before',
     'SCRCSAR050205', 'Redmine8842_Test', '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE());
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_ApprovedHeader' AS EvidenceName,
           SlipBranchNo, ApprovalStatus, RedBlackDiv, SlipRevisionFlg,
           LogicalDelFlg, TotalQty, SlipTotalDisposalAmt,
           CASE
             WHEN SlipBranchNo = 1 AND SlipRevisionFlg = '1' AND LogicalDelFlg = '0' THEN 'OK_ORIGINAL'
             WHEN SlipBranchNo = 2 AND RedBlackDiv = '1' AND LogicalDelFlg = '0' THEN 'OK_RED'
             ELSE 'CHECK'
           END AS Judge
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY SlipBranchNo;

    SELECT 'After_ApprovedDetail' AS EvidenceName,
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
