BEGIN TRANSACTION

-- TC-01 保存されていない伝票を取消しようとした場合
-- 期待結果:
--   1. 画面に ERRCSARB0273 が表示される。
--   2. 対象SlipNoに対してDB更新・赤伝INSERTが発生しない。

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000099';
DECLARE @InputBatchNo nvarchar(13) = N'RB884200099';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_SARSlipHeaderError' AS EvidenceName, COUNT(*) AS RowCount
    FROM SARSlipHeaderError
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo;

    SELECT 'Before_SARSlipDetailError' AS EvidenceName, COUNT(*) AS RowCount
    FROM SARSlipDetailError
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo;

    SELECT 'Before_SARSlipHeader' AS EvidenceName, COUNT(*) AS RowCount
    FROM SARSlipHeader
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo;

    SELECT 'Before_SARSlipDetail' AS EvidenceName, COUNT(*) AS RowCount
    FROM SARSlipDetail
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo;
END

-- テストデータのINSERT
BEGIN
    -- 本TCは未保存伝票を画面で作成するため、対象テーブルにはINSERTしない。
    -- 代わりに、証跡用の一時テーブルへ「対象DBデータを作らない」条件を記録する。
    DECLARE @TC01SetupEvidence TABLE
    (
        EvidenceName nvarchar(50),
        TargetSlipNo nvarchar(13),
        SetupPolicy nvarchar(200)
    );

    INSERT INTO @TC01SetupEvidence
    (
        EvidenceName,
        TargetSlipNo,
        SetupPolicy
    )
    VALUES
    (
        N'TC-01_Setup',
        @SlipNo,
        N'未保存伝票のため、SARSlipHeaderError/SARSlipDetailError/SARSlipHeader/SARSlipDetail へ対象SlipNoをINSERTしない'
    );

    SELECT *
    FROM @TC01SetupEvidence;
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_AllTargetTables' AS EvidenceName,
           (SELECT COUNT(*) FROM SARSlipHeaderError WHERE CorporationCode = @CorporationCode AND SlipNo = @SlipNo) AS HeaderErrorCount,
           (SELECT COUNT(*) FROM SARSlipDetailError WHERE CorporationCode = @CorporationCode AND SlipNo = @SlipNo) AS DetailErrorCount,
           (SELECT COUNT(*) FROM SARSlipHeader WHERE CorporationCode = @CorporationCode AND SlipNo = @SlipNo) AS HeaderCount,
           (SELECT COUNT(*) FROM SARSlipDetail WHERE CorporationCode = @CorporationCode AND SlipNo = @SlipNo) AS DetailCount;
END

ROLLBACK TRANSACTION
