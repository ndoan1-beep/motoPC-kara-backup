BEGIN TRANSACTION

-- TC-14 確認画面ありの場合
-- 期待結果:
--   取消ボタン押下時点ではDB更新・赤伝INSERTはまだ実行されない。
--   画面は SCRCSAR050208 へ遷移し、workEntity.ButtonId = '5' となる。

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000001';
DECLARE @InputBatchNo nvarchar(13) = N'RB884200001';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_Header' AS EvidenceName,
           SlipBranchNo, RedBlackDiv, SlipRevisionFlg, LogicalDelFlg
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY SlipBranchNo;

    SELECT 'Before_HeaderError' AS EvidenceName,
           SlipNo, FixDate, LogicalDelFlg
    FROM SARSlipHeaderError
    WHERE CorporationCode = @CorporationCode
      AND InputBatchNo = @InputBatchNo
      AND SlipNo = @SlipNo;
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
    (
        '1000', '01', '003', '8842000000001', 1, '2026-07-01',
        '0000001001', '0101', '0201', 10.00, 2000, 1500, '0', 1000,
        '2026-07-01', '2026-07-01', '', '0', '0', '0', '0', '1',
        'RB884200001', '1', '9999-12-31', '0',
        'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()
    );

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
        '9999-12-31', '0', '', '0', '0', '2026-07-01',
        '9999-12-31', '0', 'REDMINE8842', GETDATE(), 'REDMINE8842', GETDATE()
    );
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_ButtonClickOnly_Header' AS EvidenceName,
           SlipBranchNo, RedBlackDiv, SlipRevisionFlg, LogicalDelFlg,
           CASE WHEN SlipBranchNo = 1 AND RedBlackDiv = '0' AND LogicalDelFlg = '0' THEN 'OK' ELSE 'CHECK' END AS Judge
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY SlipBranchNo;

    SELECT 'After_ButtonClickOnly_RedHeaderCount' AS EvidenceName,
           COUNT(*) AS RedHeaderCount,
           CASE WHEN COUNT(*) = 0 THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
      AND RedBlackDiv = '1';
END

ROLLBACK TRANSACTION
