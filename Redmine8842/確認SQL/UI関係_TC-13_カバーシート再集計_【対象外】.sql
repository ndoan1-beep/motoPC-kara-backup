BEGIN TRANSACTION

-- TC-13 カバーシートが再集計されること
-- 期待結果:
--   SARCoverSheetMaster の集計項目が現行ロジック通り更新される。
--   LogicalDelFlg は変更しない。

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @BatchNo nvarchar(13) = N'RB884200001';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @BatchDate date = '2026-07-01';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_CoverSheet' AS EvidenceName,
           TotalPageQty, InputPageCount, ErrorNo, ApprovalCount,
           LogicalDelFlg, UpdateBy, UpdateDate
    FROM SARCoverSheetMaster
    WHERE CorporationCode = @CorporationCode
      AND BatchNo = @BatchNo
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND BatchDate = @BatchDate;
END

-- テストデータのINSERT
BEGIN
    DELETE FROM SARCoverSheetMaster
    WHERE CorporationCode = '1000'
      AND BatchNo = 'RB884200001'
      AND StoreCd = '0000001001'
      AND SlipTypeCd = '003'
      AND SlipDiv = '01'
      AND BatchDate = '2026-07-01';

    INSERT INTO SARCoverSheetMaster
    (
        CorporationCode,
        BatchNo,
        StoreCd,
        SlipTypeCd,
        SlipDiv,
        BatchDate,
        TotalPageQty,
        InputPageCount,
        ErrorNo,
        ApprovalCount,
        LogicalDelFlg,
        CreateBy,
        CreateDate,
        UpdateBy,
        UpdateDate
    )
    VALUES
    (
        '1000',
        'RB884200001',
        '0000001001',
        '003',
        '01',
        '2026-07-01',
        1,
        1,
        0,
        0,
        '0',
        'REDMINE8842',
        GETDATE(),
        'REDMINE8842',
        GETDATE()
    );
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_CoverSheet' AS EvidenceName,
           TotalPageQty, InputPageCount, ErrorNo, ApprovalCount,
           LogicalDelFlg, UpdateBy, UpdateDate,
           CASE WHEN LogicalDelFlg = '0' THEN 'OK' ELSE 'NG' END AS Judge
    FROM SARCoverSheetMaster
    WHERE CorporationCode = @CorporationCode
      AND BatchNo = @BatchNo
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND BatchDate = @BatchDate;
END

ROLLBACK TRANSACTION
