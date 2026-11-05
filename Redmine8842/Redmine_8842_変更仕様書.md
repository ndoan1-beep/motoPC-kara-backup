# AI作業指示書
CCRでの「売変・廃棄の伝票取消でLogicalDelFlgがセットされる」という処理を修正
---

# 1. コンテキスト

## システム概要

本システムは小売業に関するシステムであり、販売システムの「伝票取消」という機能について、動作変更の仕様書を記載されています。

- Framework：.NET Framework 4.8
- Database：SQL Server 2019
- Architecture

```
UI / ASP.NET WebForms
 ↓
Facade
 ↓
Business
 ↓
DataAccessComponent（DAO）
 ↓
Inline SQL / DbCommand / SqlDataAdapter
 ↓
SQL Server
```

---

# 2. 作業目的（ゴール）

売変・廃棄の伝票取消を行うと：
ー　元伝票が削除されないようにする
ー　テーブルでいくつかの数量・金額が相殺される（#4のSQLで具体的に指定される）
ー　伝票検索画面（SCRCBYR040205）で元伝票と新規登録赤伝が検索できるが、その二つの伝票に対して「修正」ボタンが表示しない（訂正選択が不可）
ー　在庫サブシステムに連携するため、大福帳（CMCAccountBookのDataID = `SAR003`）の登録・更新の動作は変わらないようになる。
---

# 3. 現在の動作

伝票取消を行うと：
ー　データベースでLogicalDelFlgが'1'セットされる
ー　伝票検索画面（SCRCBYR040205）で取消伝票が検索できない
ー　取消済後、同じ伝票番号等で登録しようとすると「既に存在する」旨のエラーとなります。

具体的に、以下は現在の処理フロー

```
画面
SalesChangeDisposalSlipInputUpdate_partial.SlipDeleateButton_Click()
（伝票取消ボタン）

↓

SalesChangeDisposalSlipEntryFacade.DelSARSlipNew()
（BSFCSAR050800031）
※ workEntity.TranUpdateMode = "1"

↓

SalesChangeDisposalSlipEntryBusiness.UpInsSlipNew()
※ workEntity.CancelMode = "1" の取消モード

↓

SalesChangeDisposalSlipEntryBusiness.UpInsSlipErrorNew()
 / UpInsSlipErrorExecuteNew()

↓

SalesChangeDisposalSlipEntryDataAccessComponent.UpdateDelFlgSARSlipHeaderErrorNew()
SalesChangeDisposalSlipEntryDataAccessComponent.UpdateDelFlgSARSlipDetailErrorNew()

↓

UPDATE SARSlipHeaderError
UPDATE SARSlipDetailError
SET LogicalDelFlg = '1',
    LogicalDelDate = @LogicalDelDate

↓

SalesChangeDisposalSlipEntryDataAccessComponent.UpdateDelFlgSARSlipHeaderNew()
SalesChangeDisposalSlipEntryDataAccessComponent.UpdateDelFlgSARSlipDetailNew()

↓

UPDATE SARSlipHeader
UPDATE SARSlipDetail
SET LogicalDelFlg = '1',
    LogicalDelDate = @LogicalDelDate
```

---

# 4. 変更後の動作

ー　データベースでSARSlipHeader、SARSlipDetail及びSARSlipHeaderError、SARSlipDetailErrorのLogicalDelFlgが'1'にしないようになる。
ー　取り消したい伝票は黒い伝票（黒伝）になり、新規赤伝が登録される（同じ伝票番号）
ー　元伝票と新規登録赤伝が検索できるが、訂正できないようになる

具体的に、以下は理想的な処理フロー
```
画面
SalesChangeDisposalSlipInputUpdate_partial.SlipDeleateButton_Click()
（伝票取消ボタン）

↓

SalesChangeDisposalSlipEntryFacade.DelSARSlipNew()
（BSFCSAR050800031）


↓

SalesChangeDisposalSlipEntryBusiness.UpInsSlipNew()
ここを赤伝INSERTに変更します。
※ this.UpInsSlipExecuteForDenyNew(workEntity, dsEntity);

↓
SalesChangeDisposalSlipEntryDataAccessComponent.InsertSARSlipHeaderForDenyNew()
SalesChangeDisposalSlipEntryDataAccessComponent.InsertSARSlipDetailForDenyNew()

↓

INSERT INTO SARSlipHeader (...)
SELECT
    CorporationCode,
    SlipDiv,
    SlipTypeCd,
    SlipNo,
    @NextSlipBranchNo,
    DisposalChangeDate,
    StoreCd,
    Category1Cd,
    Category2Cd,
    -1 * TotalQty,
    SlipOldTotalPrice,
    SlipNewTotalPrice,
    AutoSalesChangeFlg,
    -1 * SlipTotalDisposalAmt,
    ...
    '1' AS RedBlackDiv,
    ...
    '0' AS LogicalDelFlg,
    ...
FROM SARSlipHeader
WHERE {SARSlipHeaderのプライマリーキー}


INSERT INTO SARSlipDetail (...)
SELECT
    CorporationCode,
    SlipLineNo,
    StoreCd,
    SlipTypeCd,
    SlipBranchNo + 1,
    SlipDiv,
    SlipNo,
    DisposalChangeDate,
    Category1Cd,
    Category2Cd,
    Category3Cd,
    Category4Cd,
    SKUCd,
    ScanningCd,
    -1 * DisposalChangeQty,
    SlipOldPrice,
    SlipNewPrice,
    SlipDisposalUnitPrica,
    ...
    -1 * DisposalPriceAmt,
    '0' AS LogicalDelFlg,
    ...
FROM SARSlipDetail
WHERE {SARSlipDetailのプライマリーキー}
```
↓

SalesChangeDisposalSlipEntryDataAccessComponent.UpdateSlipRevisionFlgSARSlipHeaderNew(workEntity, dsEntity)
SalesChangeDisposalSlipEntryDataAccessComponent.UpdateFixDateSARSlipHeaderErrorNew(workEntity, dsEntity)


↓

UPDATE SARSlipHeader
SET 
    SlipRevisionFlg = '1'
WHERE　{SARSlipHeaderのプライマリーキー}

UPDATE SARSlipHeaderError
SET 
    FixDate = '9999/12/31'
WHERE {SARSlipHeaderErrorのプライマリーキー}

---

# 5. 対象コンポネント
1. C#
- 対象イベント: `SalesChangeDisposalSlipInputUpdate`
- 対象Facade: `DelSARSlipNew`
- 対象Business: `UpInsSlipNew`, `UpInsSlipErrorExecuteNew`, `UpInsSlipExecuteForDenyNew`
- 対象DAO: `SalesChangeDisposalSlipEntryDataAccessComponent`

2. SQL
- `SARSlipHeader`
- `SARSlipDetail`
- `SARSlipHeaderError`
- `SARSlipDetailError`
- `CMCAccountBook`
---
# 6. 技術的制約

変更してはいけない内容

- DBスキーマ変更禁止
- Stored Procedureの引数変更禁止
- API仕様変更禁止
- SQLのテーブル構成には
`SARSlipHeader_Table.sql`, `SARSlipDetail_Table.sql`,
`SARSlipHeaderError_Table.sql`, `SARSlipDetailError_Table.sql`を参考します。

- 新しいNuGet Packageは追加しないこと。

---


# 7. テスト観点

確認すること

- SQL例外が発生しないこと
- 既存機能への影響がないこと

---

## 修正後

以下を報告すること。

1. 修正したファイル一覧
2. 修正理由
3. 変更内容
4. 影響範囲
5. 想定されるリスク
6. 必要な追加テスト