# 売変・廃棄 伝票取消仕様変更 テスト仕様書

## 1. 概要

| 項目 | 内容 |
|---|---|
| 対象機能 | 売変・廃棄伝票入力画面 伝票取消 |
| 対象イベント | `SalesChangeDisposalSlipInputUpdate.SlipDeleateButton_Click` |
| 対象Facade | `DelSARSlipNew` |
| 対象Business | `UpInsSlipNew`, `UpInsSlipErrorExecuteNew`, `UpInsSlipExecuteForDenyNew` |
| 対象DAO | `SalesChangeDisposalSlipEntryDataAccessComponent` |
| 対象テーブル | `SARSlipHeaderError`, `SARSlipDetailError`, `SARSlipHeader`, `SARSlipDetail`, `CMCACCountBook` |
| 目的 | 伝票取消時に論理削除せず、元伝票を残したまま新規赤伝を登録し、画面一覧では元伝票と赤伝を検索可能かつ訂正不可にすること |

## 2. 変更後の期待動作

伝票取消時、以下の動作となること。

1. `SARSlipHeader.LogicalDelFlg`、`SARSlipDetail.LogicalDelFlg`、`SARSlipHeaderError.LogicalDelFlg`、`SARSlipDetailError.LogicalDelFlg` を `'1'` に更新しない。
2. 取消対象の元伝票は削除せず保持する。
3. `SARSlipHeader` に新規赤伝ヘッダを登録する。
4. `SARSlipDetail` に新規赤伝明細を登録する。
5. SQLで指定された項目のみマイナス値として登録する。
6. `SARSlipHeader.SlipRevisionFlg` を `'1'` に更新する。
7. `SARSlipHeaderError.FixDate` を `'9999/12/31'` に更新する。
8. 画面一覧で元伝票と新規登録赤伝を検索できる。
9. 画面一覧で元伝票と新規登録赤伝に対して「修正」ボタンを表示しない。
10. 確認画面ありの場合、取消ボタン押下時点ではDB更新を行わず、確認画面で確定後に取消処理を行う。
11. 取消対象の元伝票に基づいて、`CMCAccountBook`での動作は通常に動く。

## 3. 共通前提条件

| No | 前提条件 |
|---|---|
| 1 | 取消対象の売変または廃棄伝票が保存済みである |
| 2 | `SARSlipHeaderError` に対象伝票が存在する |
| 3 | `SARSlipDetailError` に対象伝票明細が存在する |
| 4 | `SARSlipHeader` に対象伝票の本伝票ヘッダが存在する |
| 5 | `SARSlipDetail` に対象伝票の本伝票明細が存在する |
| 6 | 対象伝票の `LogicalDelFlg` はすべて `'0'` である |
| 7 | 取消ボタン押下時に `workEntity.CancelMode = Constants.OnFlag` となる |
| 8 | `stockMonthly` がセッション `SalesChangeDisposalSlipSearchSessionKey.StockMonthly` から取得できる |

## 4. 確認SQLで使用する主なキー

### 4.1 `SARSlipHeader`

```sql
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND SlipBranchNo = @SlipBranchNo
  AND DisposalChangeDate = @DisposalChangeDate
  AND StoreCd = @StoreCd
```

### 4.2 `SARSlipDetail`

```sql
WHERE CorporationCode = @CorporationCode
  AND SlipLineNo = @SlipLineNo
  AND StoreCd = @StoreCd
  AND SlipTypeCd = @SlipTypeCd
  AND SlipBranchNo = @SlipBranchNo
  AND SlipDiv = @SlipDiv
  AND SlipNo = @SlipNo
  AND DisposalChangeDate = @DisposalChangeDate
```

### 4.3 `SARSlipHeaderError`

```sql
WHERE CorporationCode = @CorporationCode
  AND InputMediaDiv = @InputMediaDiv
  AND InputBatchNo = @InputBatchNo
  AND InputSequenceNo = @InputSequenceNo
  AND InputDate = @InputDate
  AND InputTime = @InputTime
  AND UptakeSequenceNo = @UptakeSequenceNo
```

### 4.4 `SARSlipDetailError`

```sql
WHERE CorporationCode = @CorporationCode
  AND InputMediaDiv = @InputMediaDiv
  AND InputBatchNo = @InputBatchNo
  AND InputSequenceNo = @InputSequenceNo
  AND InputSequenceLineNo = @InputSequenceLineNo
  AND InputDate = @InputDate
  AND InputTime = @InputTime
  AND DataType = @DataType
  AND UptakeSequenceNo = @UptakeSequenceNo
  AND UptakeSequenceLineNo = @UptakeSequenceLineNo
```

## 5. テストケース

### TC-01 保存されていない伝票を取消しようとした場合

| 項目 | 内容 |
|---|---|
| 条件 | `dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count <= 0` |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | エラーメッセージ `ERRCSARB0273` が表示される |
| DB期待結果 | `SARSlipHeaderError`, `SARSlipDetailError`, `SARSlipHeader`, `SARSlipDetail` に更新・INSERTが発生しない |

### TC-02 エラートランを論理削除しないこと

| 項目 | 内容 |
|---|---|
| 条件 | 保存済み伝票を取消する |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `UpdateDelFlgSARSlipHeaderErrorNew` が実行されない |
| 期待結果 | `UpdateDelFlgSARSlipDetailErrorNew` が実行されない |
| DB確認 | 対象伝票の `SARSlipHeaderError.LogicalDelFlg = '0'` のまま |
| DB確認 | 対象伝票の `SARSlipDetailError.LogicalDelFlg = '0'` のまま |

確認SQL例:

```sql
SELECT LogicalDelFlg
FROM SARSlipHeaderError
WHERE CorporationCode = @CorporationCode
  AND InputMediaDiv = @InputMediaDiv
  AND InputBatchNo = @InputBatchNo
  AND InputSequenceNo = @InputSequenceNo
  AND InputDate = @InputDate
  AND InputTime = @InputTime
  AND UptakeSequenceNo = @UptakeSequenceNo;
```

```sql
SELECT LogicalDelFlg
FROM SARSlipDetailError
WHERE CorporationCode = @CorporationCode
  AND InputMediaDiv = @InputMediaDiv
  AND InputBatchNo = @InputBatchNo
  AND InputSequenceNo = @InputSequenceNo
  AND InputDate = @InputDate
  AND InputTime = @InputTime
  AND DataType = @DataType
  AND UptakeSequenceNo = @UptakeSequenceNo;
```

### TC-03 本伝票を論理削除しないこと

| 項目 | 内容 |
|---|---|
| 条件 | 保存済み伝票を取消する |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `UpdateDelFlgSARSlipHeaderNew` が実行されない |
| 期待結果 | `UpdateDelFlgSARSlipDetailNew` が実行されない |
| DB確認 | 元伝票の `SARSlipHeader.LogicalDelFlg = '0'` のまま |
| DB確認 | 元伝票の `SARSlipDetail.LogicalDelFlg = '0'` のまま |

確認SQL例:

```sql
SELECT SlipBranchNo, RedBlackDiv, LogicalDelFlg
FROM SARSlipHeader
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
ORDER BY SlipBranchNo;
```

```sql
SELECT SlipBranchNo, SlipLineNo, LogicalDelFlg
FROM SARSlipDetail
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
ORDER BY SlipBranchNo, SlipLineNo;
```

### TC-04 `SARSlipHeader` に新規赤伝が作成されること

| 項目 | 内容 |
|---|---|
| 条件 | 対象伝票の取消を実行する |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `InsertSARSlipHeaderForDenyNew` が実行される |
| DB確認 | `SARSlipHeader` に新しい枝番のレコードが追加される |
| DB確認 | 赤伝の `RedBlackDiv = '1'` |
| DB確認 | 赤伝の `LogicalDelFlg = '0'` |
| DB確認 | 赤伝の `SlipBranchNo` は元伝票より新しい枝番 |

確認SQL例:

```sql
SELECT SlipBranchNo, RedBlackDiv, LogicalDelFlg, TotalQty, SlipTotalDisposalAmt
FROM SARSlipHeader
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
ORDER BY SlipBranchNo;
```

### TC-05 `SARSlipHeader` のSQL指定項目がマイナス値になること

| 項目 | 内容 |
|---|---|
| 条件 | 元伝票の `TotalQty = 10` |
| 条件 | 元伝票の `SlipTotalDisposalAmt = 1000` |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | 赤伝の `TotalQty = -10` |
| 期待結果 | 赤伝の `SlipTotalDisposalAmt = -1000` |
| 注意 | SQL例でマイナス指定されていない金額・単価項目は本テストの期待値対象外 |

確認SQL例:

```sql
SELECT SlipBranchNo, RedBlackDiv, TotalQty, SlipTotalDisposalAmt
FROM SARSlipHeader
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
ORDER BY SlipBranchNo;
```

### TC-06 `SARSlipDetail` に新規赤伝明細が作成されること

| 項目 | 内容 |
|---|---|
| 条件 | 対象伝票に明細が複数行存在する |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `InsertSARSlipDetailForDenyNew` が実行される |
| DB確認 | 元明細と同じ行数の赤伝明細が追加される |
| DB確認 | 赤伝明細の `LogicalDelFlg = '0'` |
| DB確認 | 赤伝明細の `SlipBranchNo` は赤伝ヘッダと一致する |

確認SQL例:

```sql
SELECT SlipBranchNo, SlipLineNo, DisposalChangeQty, DisposalPriceAmt, LogicalDelFlg
FROM SARSlipDetail
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
ORDER BY SlipBranchNo, SlipLineNo;
```

### TC-07 `SARSlipDetail` のSQL指定項目がマイナス値になること

| 項目 | 内容 |
|---|---|
| 条件 | 元明細1行目 `DisposalChangeQty = 3` |
| 条件 | 元明細1行目 `DisposalPriceAmt = 300` |
| 条件 | 元明細2行目 `DisposalChangeQty = 5` |
| 条件 | 元明細2行目 `DisposalPriceAmt = 500` |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | 赤伝明細1行目 `DisposalChangeQty = -3` |
| 期待結果 | 赤伝明細1行目 `DisposalPriceAmt = -300` |
| 期待結果 | 赤伝明細2行目 `DisposalChangeQty = -5` |
| 期待結果 | 赤伝明細2行目 `DisposalPriceAmt = -500` |
| 注意 | SQL例でマイナス指定されていない単価項目は本テストの期待値対象外 |

確認SQL例:

```sql
SELECT SlipBranchNo, SlipLineNo, DisposalChangeQty, DisposalPriceAmt
FROM SARSlipDetail
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
ORDER BY SlipBranchNo, SlipLineNo;
```

### TC-08 SQL指定項目の元伝票と赤伝の合計が0になること

| 項目 | 内容 |
|---|---|
| 条件 | 対象伝票を取消済みである |
| 操作 | 元伝票 + 赤伝を集計する |
| 期待結果 | `SUM(TotalQty) = 0` |
| 期待結果 | `SUM(SlipTotalDisposalAmt) = 0` |
| 期待結果 | `SUM(DisposalChangeQty) = 0` |
| 期待結果 | `SUM(DisposalPriceAmt) = 0` |

確認SQL例:

```sql
SELECT SUM(TotalQty) AS TotalQtySum,
       SUM(SlipTotalDisposalAmt) AS SlipTotalDisposalAmtSum
FROM SARSlipHeader
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
  AND LogicalDelFlg = '0';
```

```sql
SELECT SUM(DisposalChangeQty) AS DisposalChangeQtySum,
       SUM(DisposalPriceAmt) AS DisposalPriceAmtSum
FROM SARSlipDetail
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
  AND LogicalDelFlg = '0';
```

### TC-09 `SARSlipHeader` の訂正不可用更新が行われること

| 項目 | 内容 |
|---|---|
| 条件 | 対象伝票を取消する |
| 条件 | `stockMonthly = yyyyMM` が取得できている |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `UpdateSlipRevisionFlgSARSlipHeaderNew` が実行される |
| DB確認 | 更新対象の `SlipRevisionFlg = '1'` |
| DB確認 | 更新対象の `DisposalChangeDate` が `stockMonthly` の前月1日相当になる |

確認SQL例:

```sql
SELECT SlipBranchNo,
       DisposalChangeDate,
       SlipRevisionFlg,
       RedBlackDiv,
       LogicalDelFlg
FROM SARSlipHeader
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
ORDER BY SlipBranchNo;
```

期待日付算出SQL例:

```sql
SELECT CONVERT(nvarchar(8), DATEADD(month, -1, CONVERT(date, @stockMonthly + '01')), 112) AS ExpectedDisposalChangeDate;
```

### TC-10 `SARSlipHeaderError` の訂正不可用更新が行われること

| 項目 | 内容 |
|---|---|
| 条件 | 対象伝票を取消する |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `UpdateSlipRevisionFlgSARSlipHeaderErrorNew` が実行される |
| DB確認 | 更新対象の `FixDate = '9999/12/31'` |
| DB確認 | `LogicalDelFlg = '0'` のまま |

確認SQL例:

```sql
SELECT FixDate, LogicalDelFlg
FROM SARSlipHeaderError
WHERE CorporationCode = @CorporationCode
  AND InputMediaDiv = @InputMediaDiv
  AND InputBatchNo = @InputBatchNo
  AND InputSequenceNo = @InputSequenceNo
  AND InputDate = @InputDate
  AND InputTime = @InputTime
  AND UptakeSequenceNo = @UptakeSequenceNo;
```

### TC-11 画面一覧で元伝票と新規登録赤伝が検索できること

| 項目 | 内容 |
|---|---|
| 条件 | 対象伝票を取消済みである |
| 操作 | 伝票検索画面またはカバーシート経由で対象伝票を検索する |
| 期待結果 | 元伝票が一覧に表示される |
| 期待結果 | 新規登録赤伝が一覧に表示される |
| 期待結果 | 両方とも `LogicalDelFlg = '0'` のデータとして確認できる |

確認SQL例:

```sql
SELECT SlipBranchNo, RedBlackDiv, LogicalDelFlg, SlipRevisionFlg
FROM SARSlipHeader
WHERE CorporationCode = @CorporationCode
  AND SlipDiv = @SlipDiv
  AND SlipTypeCd = @SlipTypeCd
  AND SlipNo = @SlipNo
  AND StoreCd = @StoreCd
ORDER BY SlipBranchNo;
```

### TC-12 画面一覧で元伝票と新規登録赤伝の「修正」ボタンが表示されないこと

| 項目 | 内容 |
|---|---|
| 条件 | 対象伝票を取消済みである |
| 操作 | 伝票検索画面またはカバーシート経由で対象伝票を検索する |
| 期待結果 | 元伝票行に「修正」ボタンが表示されない |
| 期待結果 | 新規登録赤伝行に「修正」ボタンが表示されない |
| 期待結果 | 修正画面へ遷移できない |
| 補足 | 判定条件は変更仕様書の `SlipRevisionFlg = '1'` および `FixDate = '9999/12/31'` 更新結果に従う |

### TC-13 カバーシートが再集計されること

| 項目 | 内容 |
|---|---|
| 条件 | 伝票取消が成功している |
| 操作 | 取消後にカバーシート情報を確認する |
| 期待結果 | `SARCoverSheetMaster` が更新される |
| 期待結果 | `TotalPageQty`, `InputPageCount`, `ErrorNo`, `ApprovalCount` が現行ロジック通り再計算される |
| 注意 | カバーシートの `LogicalDelFlg` は変更しない |

確認SQL例:

```sql
SELECT TotalPageQty,
       InputPageCount,
       ErrorNo,
       ApprovalCount,
       LogicalDelFlg,
       UpdateBy,
       UpdateDate
FROM SARCoverSheetMaster
WHERE CorporationCode = @CorporationCode
  AND BatchNo = @BatchNo
  AND StoreCd = @StoreCd
  AND SlipTypeCd = @SlipTypeCd
  AND SlipDiv = @SlipDiv
  AND BatchDate = @BatchDate;
```

### TC-14 確認画面ありの場合

| 項目 | 内容 |
|---|---|
| 条件 | `SAR_CheckDisplay_Off` により確認画面へ遷移する設定 |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `workEntity.ButtonId = "5"` |
| 期待結果 | `SCRCSAR050208` へ遷移する |
| DB期待結果 | この時点ではDB更新・赤伝INSERTはまだ実行されない |
| 後続期待 | 確認画面で確定後にTC-02からTC-13の動作となる |

### TC-15 確認画面なしの場合

| 項目 | 内容 |
|---|---|
| 条件 | `SAR_CheckDisplay_Off` により直接実行する設定 |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `DelSARSlipNew` が即時実行される |
| 期待結果 | TC-02からTC-13の動作となる |
| 期待結果 | 完了メッセージ `MSGCSARI0028` が表示される |

### TC-16 承認済み伝票の場合

| 項目 | 内容 |
|---|---|
| 条件 | `ApprovalStatus = '3'` の伝票を取消する |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `SARSlipHeader` に赤伝が作成される |
| 期待結果 | `SARSlipDetail` に赤伝明細が作成される |
| 期待結果 | 訂正不可用更新が行われる |
| 期待結果 | 大福帳 `CMCAccountBook.DataID = '003'` の登録・更新動作が既存仕様通りである |
| 注意 | 大福帳の詳細登録ロジックは `ICmcAccountBookComponent` 内部のため、DB確認は `CMCAccountBook` 側で行う |

### TC-17 `CMCAccountBook` の `DataID = '003'` 登録・更新動作が変わらないこと

| 項目 | 内容 |
|---|---|
| 条件 | `ApprovalStatus = '3'` の売変・廃棄伝票を取消する |
| 条件 | 対象伝票に紐づく `CMCAccountBook.DataID = '003'` の登録結果を確認できる |
| 操作 | 取消前に対象伝票の `CMCAccountBook` 件数・主要項目を取得する |
| 操作 | 伝票取消ボタンを押下する |
| 期待結果 | `SARSlipHeader` と `SARSlipDetail` には新規赤伝が作成される |
| 期待結果 | `CMCAccountBook` の `DataID = '003'` の登録・更新動作は既存仕様から変わらない |
| 期待結果 | 今回変更により、`CMCAccountBook` 側で想定外の追加・削除・論理削除が発生しない |
| DB確認 | 取消前後で対象伝票の `DataID = '003'` レコード件数が既存仕様通りである |
| DB確認 | 対象レコードの `LogicalDelFlg` が既存仕様通りである |
| 注意 | `CMCAccountBook` の詳細登録仕様は `ICmcAccountBookComponent` 側の既存仕様に従う |

取消前確認SQL例

```sql
SELECT COUNT(*) AS BeforeAccountBookCount
FROM CMCAccountBook
WHERE CorporationCode = @CorporationCode
  AND StoreCd = @StoreCd
  AND DataID = '003'
  AND SlipTypeCd = @SlipTypeCd
  AND SlipDiv = @SlipDiv
  AND SlipNo = @SlipNo;
```

取消後確認SQL例

```sql
SELECT AccountBookSequenceNo,
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
```

取消前後差分確認SQL例

```sql
SELECT COUNT(*) AS AfterAccountBookCount
FROM CMCAccountBook
WHERE CorporationCode = @CorporationCode
  AND StoreCd = @StoreCd
  AND DataID = '003'
  AND SlipTypeCd = @SlipTypeCd
  AND SlipDiv = @SlipDiv
  AND SlipNo = @SlipNo;
```

論理削除状態確認SQL例

```sql
SELECT LogicalDelFlg,
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
```

## 6. 回帰確認観点

| No | 確認観点 | 期待結果 |
|---|---|---|
| 1 | 通常の新規保存 | 既存通り保存できる |
| 2 | 通常の修正保存 | 既存通り赤黒伝が作成される |
| 3 | 明細追加 | 既存通り明細追加できる |
| 4 | 明細削除 | 既存通り明細削除できる |
| 5 | 売変伝票取消 | 論理削除せず赤伝が登録される |
| 6 | 廃棄伝票取消 | 論理削除せず赤伝が登録される |
| 7 | カバーシート検索後の取消 | 取消後に一覧再表示できる |
| 8 | 伝票検索後の取消 | 取消後に一覧再表示できる |
| 9 | 取消後の検索 | 元伝票と新規登録赤伝が検索できる |
| 10 | 取消後の修正 | 元伝票と新規登録赤伝の修正ボタンが表示されない |
| 11 | 大福帳 `DataID = '003'` の登録・更新 | `CMCAccountBook` の登録・更新動作が既存仕様から変わらない |

## 7. 重要確認ポイント

今回の仕様変更では、`LogicalDelFlg` を `'1'` に更新しないため、元伝票と新規登録赤伝が検索対象として残る。

重点的に確認すること。

1. 元伝票が論理削除されないこと。
2. 新規登録赤伝が追加されること。
3. SQL例で指定された項目がマイナス値になること。
4. SQL例で指定された項目の元伝票 + 赤伝の合計が0になること。
5. 元伝票と新規登録赤伝が画面一覧で検索できること。
6. 元伝票と新規登録赤伝の「修正」ボタンが表示されないこと。
7. 赤伝の `SlipBranchNo` が元伝票と重複しないこと。
8. 赤伝の `RedBlackDiv` が赤伝を示す値になっていること。
9. `SARSlipHeader.SlipRevisionFlg` と `SARSlipHeaderError.FixDate` が仕様通り更新されること。
10. カバーシートの集計結果が仕様通りであること。
11. 大福帳 `CMCAccountBook.DataID = '003'` の登録・更新動作が既存仕様から変わらないこと。

## 8. 後続検討

以下は今回のテスト対象外とし、後続仕様として別途検討する。

| 項目 | 内容 |
|---|---|
| 二重取消防止 | 同じ伝票に対して再度取消を実行できないようにする制御 |
| 大福帳側の詳細項目マッピング検証 | `ICmcAccountBookComponent` 内部の項目変換・登録内容の詳細確認 |
