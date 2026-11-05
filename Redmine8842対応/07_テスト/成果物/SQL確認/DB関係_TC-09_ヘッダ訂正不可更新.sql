BEGIN TRANSACTION

-- TC-09 SARSlipHeader の訂正不可用更新が行われること
-- 期待結果:
--   SARSlipHeader.SlipRevisionFlg = '1'

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000001';
DECLARE @stockMonthly nvarchar(6) = N'202604';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_SlipRevision' AS EvidenceName,
           SlipBranchNo, DisposalChangeDate, SlipRevisionFlg, RedBlackDiv, LogicalDelFlg
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
    ORDER BY SlipBranchNo;

    SELECT CONVERT(nvarchar(8), DATEADD(month, -1, CONVERT(date, @stockMonthly + '01')), 112) AS ExpectedDisposalChangeDate;
END

-- テストデータのINSERT
BEGIN
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
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_SlipRevision' AS EvidenceName,
           SlipBranchNo, DisposalChangeDate, SlipRevisionFlg, RedBlackDiv, LogicalDelFlg,
           CASE WHEN SlipRevisionFlg = '1' AND LogicalDelFlg = '0' THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND SlipDiv = @SlipDiv
      AND SlipTypeCd = @SlipTypeCd
      AND SlipNo = @SlipNo
      AND StoreCd = @StoreCd
    ORDER BY SlipBranchNo;
END

ROLLBACK TRANSACTION
