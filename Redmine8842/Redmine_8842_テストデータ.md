# Redmine 8842 テストデータ

## 1. 目的

`Redmine_8842_テスト仕様書.md` のTC-01からTC-17を実施するためのテストデータ定義。

本ファイルでは、DB投入前の元伝票データ、取消実行後に期待する赤伝データ、大福帳確認用データ、確認SQLで使用するパラメータを定義する。

## 2. 共通パラメータ

| パラメータ | 値 | 用途 |
|---|---:|---|
| `@CorporationCode` | `1000` | 法人コード |
| `@StoreCd` | `0000001001` | 店舗コード |
| `@SlipTypeCd` | `003` | 売変伝票種別。廃棄伝票ケースでは `004` に置き換える |
| `@SlipDiv` | `01` | 伝票区分 |
| `@SlipNo` | `8842000000001` | TC-02からTC-15用の対象伝票番号 |
| `@ApprovedSlipNo` | `8842000000002` | TC-16、TC-17用の承認済み対象伝票番号 |
| `@SlipBranchNo` | `1` | 取消前の元伝票枝番 |
| `@RedSlipBranchNo` | `2` | 取消後に作成される赤伝枝番 |
| `@DisposalChangeDate` | `2026-07-01` | 売変・廃棄日 |
| `@SlipDate` | `2026-07-01` | 伝票日付 |
| `@InputMediaDiv` | `1` | 入力媒体区分 |
| `@InputBatchNo` | `RB884200001` | 入力バッチ番号 |
| `@InputSequenceNo` | `1` | 入力SEQ番号 |
| `@InputDate` | `2026-07-01` | 入力日 |
| `@InputTime` | `101500` | 入力時刻 |
| `@DataType` | `01` | データ種別 |
| `@UptakeSequenceNo` | `1` | 取込SEQ番号 |
| `@BatchNo` | `RB884200001` | カバーシートバッチ番号 |
| `@BatchDate` | `2026-07-01` | カバーシートバッチ日付 |
| `@stockMonthly` | `202604` | TC-09で現行テスト仕様書に残っている期待日付算出用 |

TC-09の期待日付:

```sql
SELECT CONVERT(nvarchar(8), DATEADD(month, -1, CONVERT(date, '202604' + '01')), 112) AS ExpectedDisposalChangeDate;
-- ExpectedDisposalChangeDate = 20260301
```

## 3. テストデータセット一覧

| データセット | 対象TC | 説明 |
|---|---|---|
| TD-01 | TC-01 | 保存されていない伝票。`SARSlipHeaderError` が0件の状態 |
| TD-02 | TC-02からTC-15 | 通常の保存済み売変伝票。2明細を持つ |
| TD-03 | TC-16、TC-17 | 承認済み売変伝票。大福帳 `DataID = '003'` 確認用 |
| TD-04 | 回帰確認 | 廃棄伝票。`@SlipTypeCd = '004'` に置換して使用する |

## 4. TD-01 保存されていない伝票

TC-01用。画面上で未保存の伝票を作成し、DB側には対象伝票のエラートランを作成しない。

| テーブル | 条件 |
|---|---|
| `SARSlipHeaderError` | `@SlipNo = '8842000000099'` に該当する行が存在しない |
| `SARSlipDetailError` | `@SlipNo = '8842000000099'` に該当する行が存在しない |
| `SARSlipHeader` | `@SlipNo = '8842000000099'` に該当する行が存在しない |
| `SARSlipDetail` | `@SlipNo = '8842000000099'` に該当する行が存在しない |

期待結果:

| 項目 | 値 |
|---|---|
| エラーメッセージ | `ERRCSARB0273` |
| DB更新 | なし |
| 赤伝INSERT | なし |

## 5. TD-02 通常の保存済み売変伝票

TC-02からTC-15の基本データ。取消前は、エラートランと本伝票がすべて `LogicalDelFlg = '0'` で存在する。

### 5.1 `SARSlipHeaderError`

| カラム | 値 |
|---|---:|
| `CorporationCode` | `1000` |
| `InputMediaDiv` | `1` |
| `InputBatchNo` | `RB884200001` |
| `InputSequenceNo` | `1` |
| `InputDate` | `2026-07-01` |
| `InputTime` | `101500` |
| `DataType` | `01` |
| `UptakeSequenceNo` | `1` |
| `SlipDiv` | `01` |
| `SlipTypeCd` | `003` |
| `SlipNo` | `8842000000001` |
| `DisposalChangeDate` | `2026-07-01` |
| `StoreCd` | `0000001001` |
| `Category1Cd` | `0101` |
| `Category2Cd` | `0201` |
| `TotalQty` | `10.00` |
| `SlipOldTotalPrice` | `2000` |
| `SlipNewTotalPrice` | `1500` |
| `AutoSalesChangeFlg` | `0` |
| `SlipTotalDisposalAmt` | `1000` |
| `SlipDate` | `2026-07-01` |
| `ApproverCd` | `` |
| `ApprovalStatus` | `0` |
| `ApprovalDate` | `9999-12-31` |
| `ErrorDiv` | `0` |
| `ErrorMessage` | `` |
| `RedBlackDiv` | `0` |
| `CorrectionDiv` | `0` |
| `FixDate` | `2026-07-01` |
| `LogicalDelFlg` | `0` |
| `LogicalDelDate` | `9999-12-31` |

### 5.2 `SARSlipDetailError`

| 行 | `InputSequenceLineNo` | `UptakeSequenceLineNo` | `SlipLineNo` | `SKUCd` | `ScanningCd` | `DisposalChangeQty` | `SlipOldPrice` | `SlipNewPrice` | `SlipDisposalUnitPrica` | `SalesChangePrice` | `DisposalPriceAmt` | `LogicalDelFlg` |
|---:|---:|---:|---:|---|---|---:|---:|---:|---:|---:|---:|---|
| 1 | `1` | `1` | `000001` | `49000000000001` | `49000000000001` | `3.00` | `200` | `150` | `100` | `150` | `300` | `0` |
| 2 | `2` | `2` | `000002` | `49000000000002` | `49000000000002` | `5.00` | `220` | `180` | `100` | `180` | `500` | `0` |

共通値:

| カラム | 値 |
|---|---|
| `CorporationCode` | `1000` |
| `InputMediaDiv` | `1` |
| `InputBatchNo` | `RB884200001` |
| `InputSequenceNo` | `1` |
| `InputDate` | `2026-07-01` |
| `InputTime` | `101500` |
| `DataType` | `01` |
| `UptakeSequenceNo` | `1` |
| `StoreCd` | `0000001001` |
| `SlipTypeCd` | `003` |
| `SlipDiv` | `01` |
| `SlipNo` | `8842000000001` |
| `DisposalChangeDate` | `2026-07-01` |
| `Category1Cd` | `0101` |
| `Category2Cd` | `0201` |
| `Category3Cd` | `0301` |
| `Category4Cd` | `0401` |
| `SalesChangeDiv` | `1` |
| `Reason` | `REDMINE8842` |
| `ErrorDiv` | `0` |

### 5.3 `SARSlipHeader`

取消前の元伝票。

| カラム | 値 |
|---|---:|
| `CorporationCode` | `1000` |
| `SlipDiv` | `01` |
| `SlipTypeCd` | `003` |
| `SlipNo` | `8842000000001` |
| `SlipBranchNo` | `1` |
| `DisposalChangeDate` | `2026-07-01` |
| `StoreCd` | `0000001001` |
| `Category1Cd` | `0101` |
| `Category2Cd` | `0201` |
| `TotalQty` | `10.00` |
| `SlipOldTotalPrice` | `2000` |
| `SlipNewTotalPrice` | `1500` |
| `AutoSalesChangeFlg` | `0` |
| `SlipTotalDisposalAmt` | `1000` |
| `SlipDate` | `2026-07-01` |
| `PostingDate` | `2026-07-01` |
| `ApproverCd` | `` |
| `ApprovalStatus` | `0` |
| `SlipRevisionFlg` | `0` |
| `RedBlackDiv` | `0` |
| `CorrectionDiv` | `0` |
| `InputMediaDiv` | `1` |
| `InputBatchNo` | `RB884200001` |
| `InputSequenceNo` | `1` |
| `LogicalDelFlg` | `0` |
| `LogicalDelDate` | `9999-12-31` |

### 5.4 `SARSlipDetail`

取消前の元伝票明細。

| 行 | `SlipLineNo` | `SlipBranchNo` | `SKUCd` | `ScanningCd` | `DisposalChangeQty` | `SlipOldPrice` | `SlipNewPrice` | `SlipDisposalUnitPrica` | `SalesChangePrice` | `DisposalPriceAmt` | `LogicalDelFlg` |
|---:|---:|---:|---|---|---:|---:|---:|---:|---:|---:|---|
| 1 | `000001` | `1` | `49000000000001` | `49000000000001` | `3.00` | `200` | `150` | `100` | `150` | `300` | `0` |
| 2 | `000002` | `1` | `49000000000002` | `49000000000002` | `5.00` | `220` | `180` | `100` | `180` | `500` | `0` |

共通値は `SARSlipDetailError` と同じ。ただし `InputMediaDiv` などのエラー専用キーは持たない。

### 5.5 `SARCoverSheetMaster`

カバーシート再集計確認用。テーブル定義が本フォルダにないため、テスト仕様書の確認SQLで使用する項目だけを定義する。

| カラム | 値 |
|---|---:|
| `CorporationCode` | `1000` |
| `BatchNo` | `RB884200001` |
| `StoreCd` | `0000001001` |
| `SlipTypeCd` | `003` |
| `SlipDiv` | `01` |
| `BatchDate` | `2026-07-01` |
| `TotalPageQty` | `1` |
| `InputPageCount` | `1` |
| `ErrorNo` | `0` |
| `ApprovalCount` | `0` |
| `LogicalDelFlg` | `0` |

## 6. TD-02 取消後期待データ

TC-02からTC-15で確認する取消後の期待値。

### 6.1 論理削除されないテーブル

| テーブル | 対象 | 期待値 |
|---|---|---|
| `SARSlipHeaderError` | `SlipNo = '8842000000001'` | `LogicalDelFlg = '0'` |
| `SARSlipDetailError` | `SlipNo = '8842000000001'` の2行 | `LogicalDelFlg = '0'` |
| `SARSlipHeader` | 元伝票 `SlipBranchNo = 1` | `LogicalDelFlg = '0'` |
| `SARSlipDetail` | 元明細 `SlipBranchNo = 1` の2行 | `LogicalDelFlg = '0'` |

### 6.2 訂正不可更新

| テーブル | 対象 | 取消後期待値 |
|---|---|---|
| `SARSlipHeader` | 元伝票 `SlipBranchNo = 1` | `SlipRevisionFlg = '1'` |
| `SARSlipHeaderError` | 対象エラーヘッダ | `FixDate = '9999-12-31'` |

### 6.3 `SARSlipHeader` 赤伝

取消後、以下の赤伝ヘッダが追加される。

| カラム | 期待値 |
|---|---:|
| `CorporationCode` | `1000` |
| `SlipDiv` | `01` |
| `SlipTypeCd` | `003` |
| `SlipNo` | `8842000000001` |
| `SlipBranchNo` | `2` |
| `DisposalChangeDate` | `2026-07-01` |
| `StoreCd` | `0000001001` |
| `Category1Cd` | `0101` |
| `Category2Cd` | `0201` |
| `TotalQty` | `-10.00` |
| `SlipOldTotalPrice` | `2000` |
| `SlipNewTotalPrice` | `1500` |
| `AutoSalesChangeFlg` | `0` |
| `SlipTotalDisposalAmt` | `-1000` |
| `SlipRevisionFlg` | `1` |
| `RedBlackDiv` | `1` |
| `CorrectionDiv` | `0` |
| `LogicalDelFlg` | `0` |

### 6.4 `SARSlipDetail` 赤伝明細

取消後、元明細と同じ2行の赤伝明細が追加される。

| 行 | `SlipLineNo` | `SlipBranchNo` | `SKUCd` | `DisposalChangeQty` | `DisposalPriceAmt` | `LogicalDelFlg` |
|---:|---:|---:|---|---:|---:|---|
| 1 | `000001` | `2` | `49000000000001` | `-3.00` | `-300` | `0` |
| 2 | `000002` | `2` | `49000000000002` | `-5.00` | `-500` | `0` |

### 6.5 集計期待値

| 集計対象 | 期待値 |
|---|---:|
| `SUM(SARSlipHeader.TotalQty)` | `0.00` |
| `SUM(SARSlipHeader.SlipTotalDisposalAmt)` | `0` |
| `SUM(SARSlipDetail.DisposalChangeQty)` | `0.00` |
| `SUM(SARSlipDetail.DisposalPriceAmt)` | `0` |

## 7. TD-03 承認済み伝票 + 大福帳確認用データ

TC-16、TC-17用。基本構造はTD-02と同じだが、伝票番号と承認状態、大福帳データを変える。

### 7.1 差分値

| 項目 | TD-02 | TD-03 |
|---|---:|---:|
| `SlipNo` | `8842000000001` | `8842000000002` |
| `InputBatchNo` | `RB884200001` | `RB884200002` |
| `ApprovalStatus` | `0` | `3` |
| `ApproverCd` | `` | `APPROVER01` |
| `ApprovalDate` | `9999-12-31` | `2026-07-01` |
| `TotalQty` | `10.00` | `10.00` |
| `SlipTotalDisposalAmt` | `1000` | `1000` |

### 7.2 取消前 `CMCAccountBook`

`DataID = '003'` の大福帳確認用。取消前に2行存在する。

| 行 | `DataID` | `SlipTypeCd` | `SlipDiv` | `SlipNo` | `SlipLineNo` | `CorrectionDiv` | `CorporationCode` | `StoreCd` | `SKUCd` | `TargetQty` | `IncludeTaxCostAmt` | `PretaxSellPrice` | `BeforetaxSellPrice` | `LogicalDelFlg` |
|---:|---|---|---|---|---:|---|---|---|---|---:|---:|---:|---:|---|
| 1 | `003` | `003` | `01` | `8842000000002` | `000001` | `+` | `1000` | `0000001001` | `49000000000001` | `3.00` | `300` | `450` | `600` | `0` |
| 2 | `003` | `003` | `01` | `8842000000002` | `000002` | `+` | `1000` | `0000001001` | `49000000000002` | `5.00` | `500` | `900` | `1100` | `0` |

共通値:

| カラム | 値 |
|---|---|
| `AccountBookRegistrationDate` | `2026-07-01` |
| `SubSystem` | `SA` |
| `UserCd` | `TESTUSER` |
| `SlipDate` | `2026-07-01` |
| `InputBatchNo` | `RB884200002` |
| `InputMediaDiv` | `1` |
| `DataUnit` | `1` |
| `Category1Cd` | `0101` |
| `Category2Cd` | `0201` |
| `Category3Cd` | `0301` |
| `Category4Cd` | `0401` |
| `SalesChangeDiv` | `1` |
| `InvControlDiv` | `1` |
| `InvPostingDate` | `2026-07-01` |
| `UpdateScreenName` | `SCRCSAR050205` |
| `UpdateBatchName` | `Redmine8842_Test` |

### 7.3 取消後 `CMCAccountBook` 期待値

テスト仕様書のTC-17では、大福帳の詳細登録仕様は `ICmcAccountBookComponent` 側の既存仕様に従う。したがって、このテストデータでは「今回変更による想定外の追加・削除・論理削除がないこと」を確認する。

| 確認項目 | 期待値 |
|---|---|
| 取消前 `DataID = '003'` 件数 | `2` |
| 取消後 `DataID = '003'` 件数 | 既存仕様通り。今回変更だけで想定外に増減しない |
| `LogicalDelFlg` | 既存仕様通り。今回変更だけで `'1'` にしない |
| `SlipNo` | `8842000000002` |
| `DataID` | `003` |

## 8. TD-04 廃棄伝票データ

売変伝票と同じ構造で、以下だけ置き換える。

| 項目 | 値 |
|---|---:|
| `SlipTypeCd` | `004` |
| `SlipNo` | `8842000000003` |
| `InputBatchNo` | `RB884200003` |
| `SlipTotalDisposalAmt` | `1000` |
| `DisposalPriceAmt` 1行目 | `300` |
| `DisposalPriceAmt` 2行目 | `500` |

取消後期待値:

| 項目 | 値 |
|---|---:|
| 赤伝 `SlipBranchNo` | `2` |
| 赤伝 `TotalQty` | `-10.00` |
| 赤伝 `SlipTotalDisposalAmt` | `-1000` |
| 赤伝明細1 `DisposalChangeQty` | `-3.00` |
| 赤伝明細1 `DisposalPriceAmt` | `-300` |
| 赤伝明細2 `DisposalChangeQty` | `-5.00` |
| 赤伝明細2 `DisposalPriceAmt` | `-500` |
| 全テーブル `LogicalDelFlg` | `0` |

## 9. SQL投入テンプレート

このテンプレートは、既存テストDBに最低限の確認用データを作るためのサンプル。実環境でNOT NULL列や外部マスタ整合が追加で必要な場合は、既存の保存済み伝票をコピーして本ファイルの値へ置換する。

### 9.1 事前削除

```sql
DELETE FROM SARSlipDetail
WHERE CorporationCode = '1000'
  AND StoreCd = '0000001001'
  AND SlipNo IN ('8842000000001', '8842000000002', '8842000000003', '8842000000099');

DELETE FROM SARSlipHeader
WHERE CorporationCode = '1000'
  AND StoreCd = '0000001001'
  AND SlipNo IN ('8842000000001', '8842000000002', '8842000000003', '8842000000099');

DELETE FROM SARSlipDetailError
WHERE CorporationCode = '1000'
  AND InputBatchNo IN ('RB884200001', 'RB884200002', 'RB884200003');

DELETE FROM SARSlipHeaderError
WHERE CorporationCode = '1000'
  AND InputBatchNo IN ('RB884200001', 'RB884200002', 'RB884200003');

DELETE FROM CMCAccountBook
WHERE CorporationCode = '1000'
  AND StoreCd = '0000001001'
  AND SlipNo IN ('8842000000001', '8842000000002', '8842000000003')
  AND DataID = '003';
```

### 9.2 期待値確認SQL

```sql
SELECT SlipBranchNo, RedBlackDiv, LogicalDelFlg, SlipRevisionFlg, TotalQty, SlipTotalDisposalAmt
FROM SARSlipHeader
WHERE CorporationCode = '1000'
  AND SlipDiv = '01'
  AND SlipTypeCd = '003'
  AND SlipNo = '8842000000001'
  AND StoreCd = '0000001001'
ORDER BY SlipBranchNo;
```

```sql
SELECT SlipBranchNo, SlipLineNo, LogicalDelFlg, DisposalChangeQty, DisposalPriceAmt
FROM SARSlipDetail
WHERE CorporationCode = '1000'
  AND SlipDiv = '01'
  AND SlipTypeCd = '003'
  AND SlipNo = '8842000000001'
  AND StoreCd = '0000001001'
ORDER BY SlipBranchNo, SlipLineNo;
```

```sql
SELECT FixDate, LogicalDelFlg
FROM SARSlipHeaderError
WHERE CorporationCode = '1000'
  AND InputMediaDiv = '1'
  AND InputBatchNo = 'RB884200001'
  AND InputSequenceNo = '1'
  AND InputDate = '2026-07-01'
  AND InputTime = '101500'
  AND UptakeSequenceNo = 1;
```

```sql
SELECT COUNT(*) AS AccountBookCount
FROM CMCAccountBook
WHERE CorporationCode = '1000'
  AND StoreCd = '0000001001'
  AND DataID = '003'
  AND SlipTypeCd = '003'
  AND SlipDiv = '01'
  AND SlipNo = '8842000000002';
```

## 10. TC別データ対応表

| TC | 使用データ | 主な確認 |
|---|---|---|
| TC-01 | TD-01 | 未保存伝票で `ERRCSARB0273` |
| TC-02 | TD-02 | エラートラン `LogicalDelFlg = '0'` |
| TC-03 | TD-02 | 本伝票 `LogicalDelFlg = '0'` |
| TC-04 | TD-02 | `SARSlipHeader` 赤伝追加 |
| TC-05 | TD-02 | ヘッダ数量・金額がマイナス |
| TC-06 | TD-02 | `SARSlipDetail` 赤伝明細追加 |
| TC-07 | TD-02 | 明細数量・金額がマイナス |
| TC-08 | TD-02 | 元伝票 + 赤伝の合計が0 |
| TC-09 | TD-02 | `SlipRevisionFlg = '1'`、必要に応じて `stockMonthly` 期待値 |
| TC-10 | TD-02 | `FixDate = '9999-12-31'` |
| TC-11 | TD-02 | 元伝票と赤伝を検索できる |
| TC-12 | TD-02 | 元伝票と赤伝を訂正できない |
| TC-13 | TD-02 | カバーシート再集計 |
| TC-14 | TD-02 | 確認画面ありでは押下時点でDB未更新 |
| TC-15 | TD-02 | 確認画面なしでは即時取消 |
| TC-16 | TD-03 | 承認済み伝票で赤伝作成 + 大福帳既存動作 |
| TC-17 | TD-03 | `CMCAccountBook.DataID = '003'` の既存動作維持 |
