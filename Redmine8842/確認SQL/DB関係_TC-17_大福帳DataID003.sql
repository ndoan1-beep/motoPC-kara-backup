BEGIN TRANSACTION

-- TC-17 CMCAccountBook の DataID = '003' 登録・更新動作が変わらないこと
-- 期待結果:
--   今回変更により、CMCAccountBook側で想定外の追加・削除・論理削除が発生しない。
--   詳細仕様は ICmcAccountBookComponent 側の既存仕様に従う。

DECLARE @CorporationCode nvarchar(4) = N'1000';
DECLARE @StoreCd nvarchar(10) = N'0000001001';
DECLARE @SlipTypeCd nvarchar(3) = N'003';
DECLARE @SlipDiv nvarchar(2) = N'01';
DECLARE @SlipNo nvarchar(13) = N'8842000000002';

-- 変更前の確認SQLのデータ抽出
BEGIN
    SELECT 'Before_AccountBookCount' AS EvidenceName,
           COUNT(*) AS BeforeAccountBookCount
    FROM CMCAccountBook
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND DataID = '003'
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo;

    SELECT 'Before_AccountBookDetail' AS EvidenceName,
           AccountBookSequenceNo,
           AccountBookRegistrationDate,
           SubSystem,
           UserCd,
           DataID,
           SlipDate,
           SlipTypeCd,
           SlipDiv,
           SlipNo,
           SlipLineNo,
           CorrectionDiv,
           InputBatchNo,
           InputMediaDiv,
           DataUnit,
           CorporationCode,
           StoreCd,
           SupplierCd,
           Category1Cd,
           Category2Cd,
           Category3Cd,
           Category4Cd,
           SKUCd,
           ScanningCd,
           SalesChangeDiv,
           TargetQty,
           IncludeTaxCostAmt,
           PretaxSellPrice,
           BeforetaxSellPrice,
           InvControlDiv,
           InvPostingDate,
           AccountBookRemarks1,
           AccountBookRemarks2,
           UpdateScreenName,
           UpdateBatchName,
           LogicalDelFlg,
           LogicalDelDate,
           CreateBy,
           CreateDate,
           UpdateBy,
           UpdateDate
    FROM CMCAccountBook
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND DataID = '003'
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY AccountBookSequenceNo, SlipLineNo;
END

-- テストデータのINSERT
BEGIN
    DELETE FROM CMCAccountBook
    WHERE CorporationCode = '1000'
      AND StoreCd = '0000001001'
      AND DataID = '003'
      AND SlipTypeCd = '003'
      AND SlipDiv = '01'
      AND SlipNo = '8842000000002';

    INSERT INTO CMCAccountBook
    (
        AccountBookRegistrationDate,
        SubSystem,
        UserCd,
        DataID,
        SlipDate,
        SlipTypeCd,
        SlipDiv,
        SlipNo,
        SlipLineNo,
        CorrectionDiv,
        InputBatchNo,
        InputMediaDiv,
        DataUnit,
        CorporationCode,
        StoreCd,
        Category1Cd,
        Category2Cd,
        Category3Cd,
        Category4Cd,
        SKUCd,
        ScanningCd,
        SalesChangeDiv,
        TargetQty,
        IncludeTaxCostAmt,
        PretaxSellPrice,
        BeforetaxSellPrice,
        InvControlDiv,
        InvPostingDate,
        AccountBookRemarks1,
        AccountBookRemarks2,
        UpdateScreenName,
        UpdateBatchName,
        LogicalDelDate,
        LogicalDelFlg,
        CreateBy,
        CreateDate,
        UpdateBy,
        UpdateDate
    )
    VALUES
    (
        '2026-07-01',
        'SA',
        'TESTUSER',
        '003',
        '2026-07-01',
        '003',
        '01',
        '8842000000002',
        '000001',
        '+',
        'RB884200002',
        '1',
        '1',
        '1000',
        '0000001001',
        '0101',
        '0201',
        '0301',
        '0401',
        '49000000000001',
        '49000000000001',
        '1',
        3.00,
        300,
        450,
        600,
        '1',
        '2026-07-01',
        'Redmine8842',
        'Before',
        'SCRCSAR050205',
        'Redmine8842_Test',
        '9999-12-31',
        '0',
        'REDMINE8842',
        GETDATE(),
        'REDMINE8842',
        GETDATE()
    ),
    (
        '2026-07-01',
        'SA',
        'TESTUSER',
        '003',
        '2026-07-01',
        '003',
        '01',
        '8842000000002',
        '000002',
        '+',
        'RB884200002',
        '1',
        '1',
        '1000',
        '0000001001',
        '0101',
        '0201',
        '0301',
        '0401',
        '49000000000002',
        '49000000000002',
        '1',
        5.00,
        500,
        900,
        1100,
        '1',
        '2026-07-01',
        'Redmine8842',
        'Before',
        'SCRCSAR050205',
        'Redmine8842_Test',
        '9999-12-31',
        '0',
        'REDMINE8842',
        GETDATE(),
        'REDMINE8842',
        GETDATE()
    );
END

-- 変更後の確認SQLのデータ抽出
BEGIN
    SELECT 'After_AccountBookCount' AS EvidenceName,
           COUNT(*) AS AfterAccountBookCount
    FROM CMCAccountBook
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND DataID = '003'
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo;

    SELECT 'After_AccountBookLogicalDeleteState' AS EvidenceName,
           LogicalDelFlg,
           COUNT(*) AS AccountBookCount
    FROM CMCAccountBook
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND DataID = '003'
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    GROUP BY LogicalDelFlg
    ORDER BY LogicalDelFlg;

    SELECT 'After_AccountBookDetail' AS EvidenceName,
           AccountBookSequenceNo,
           AccountBookRegistrationDate,
           SubSystem,
           UserCd,
           DataID,
           SlipDate,
           SlipTypeCd,
           SlipDiv,
           SlipNo,
           SlipLineNo,
           CorrectionDiv,
           InputBatchNo,
           InputMediaDiv,
           DataUnit,
           CorporationCode,
           StoreCd,
           SupplierCd,
           Category1Cd,
           Category2Cd,
           Category3Cd,
           Category4Cd,
           SKUCd,
           ScanningCd,
           SalesChangeDiv,
           TargetQty,
           IncludeTaxCostAmt,
           PretaxSellPrice,
           BeforetaxSellPrice,
           InvControlDiv,
           InvPostingDate,
           AccountBookRemarks1,
           AccountBookRemarks2,
           UpdateScreenName,
           UpdateBatchName,
           LogicalDelFlg,
           LogicalDelDate,
           CreateBy,
           CreateDate,
           UpdateBy,
           UpdateDate
    FROM CMCAccountBook
    WHERE CorporationCode = @CorporationCode
      AND StoreCd = @StoreCd
      AND DataID = '003'
      AND SlipTypeCd = @SlipTypeCd
      AND SlipDiv = @SlipDiv
      AND SlipNo = @SlipNo
    ORDER BY AccountBookSequenceNo, SlipLineNo;
END

ROLLBACK TRANSACTION
