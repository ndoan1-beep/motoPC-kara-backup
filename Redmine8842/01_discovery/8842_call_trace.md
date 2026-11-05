# 8842_02_呼出経路

最新更新時刻：　2026年08月18日16:03:47

## 1. 伝票取消の呼出経路

```text
SalesChangeDisposalSlipInputUpdate_partial.cs
  ├─ workEntity.CancelMode = Constants.OnFlag
  └─ GetFacadeInstance().UpInsSARSlipNew(...)
      └─ SalesChangeDisposalSlipEntryNewFacade.UpInsSARSlipNew(...)
          └─ SalesChangeDisposalSlipEntryBusiness.UpInsSlipNew(...)
              ├─ CancelMode == Constants.OnFlag
              │   ├─ SlipUpdateMode = slipUpdateMode_Red
              │   ├─ UpdateDelFlgSARSlipHeaderNew(...)
              │   └─ UpdateDelFlgSARSlipDetailNew(...)
              ├─ ErrorTable経路
              │   ├─ UpdateDelFlgSARSlipHeaderErrorNew(...)
              │   └─ UpdateDelFlgSARSlipDetailErrorNew(...)
              └─ SlipUpdateMode == slipUpdateMode_Red
                  └─ UpInsSlipExecuteForDenyNew(...)
```

## 2. UI層

| ファイル | 処理 | MCP確認行 |
|---|---|---|
| `Sales/InputSalesSlip/InputSalesSlip.Web/Sales/InputSalesSlip/SalesChangeDisposalSlipInput/SalesChangeDisposalSlipInputUpdate_partial.cs` | `CancelMode = Constants.OnFlag` | 1536-1548 |
| 同上 | `UpInsSARSlipNew(...)` 呼出 | 2360-2370 |

## 3. Facade層

| ファイル | 処理 | MCP確認行 |
|---|---|---|
| `Sales/InputSalesSlip/InputSalesSlip.Facade/SalesChangeDisposalSlipEntryNewFacade.cs` | `UpInsSARSlipNew` からBusiness `UpInsSlipNew` 呼出 | 140-180 |

## 4. Business層

| ファイル | 処理 | MCP確認行 |
|---|---|---|
| `Sales/InputSalesSlip/InputSalesSlip.Business/SalesChangeDisposalSlipEntryNewBusiness.cs` | 取消判定、通常伝票論理削除 | 2174-2210 |
| 同上 | エラー伝票論理削除、赤伝登録分岐 | 2448-2515 |
| 同上 | 大福帳DataID判定、登録 | 2658-2992 |
| 同上 | 赤伝大福帳データ生成 | 6197-6265 |

## 5. DataAccess層

| ファイル | メソッド | SQL対象 | 現行更新 |
|---|---|---|---|
| `Sales/InputSalesSlip/InputSalesSlip.Business/SalesChangeDisposalSlipEntryNewDataAccessComponent.cs` | `UpdateDelFlgSARSlipHeaderNew` | `SARSlipHeader` | `LogicalDelFlg = '1'`, `LogicalDelDate`, `UpdateBy`, `UpdateDate` |
| 同上 | `UpdateDelFlgSARSlipDetailNew` | `SARSlipDetail` | 同上 |
| 同上 | `UpdateDelFlgSARSlipHeaderErrorNew` | `SARSlipHeaderError` | 同上 |
| 同上 | `UpdateDelFlgSARSlipDetailErrorNew` | `SARSlipDetailError` | 同上 |

MCP確認行: 1140-1375。

## 6. 伝票検索画面の呼出経路

```text
SalesChangeDisposalSlipSearchBusiness
  └─ SalesChangeDisposalSlipSearchHeaderProcessing(...)
      └─ SalesChangeDisposalSlipSearchGetHeader(...)
          └─ SalesChangeDisposalSlipSearchSelectHeader(...)
              └─ SlipHeaderListDataTableAdapter.FillBySlipHeaderListSearchCondition(...)
                  └─ SlipHeaderListDataSet
                      └─ SARSlipHeader UNION SARSlipHeaderError

SalesChangeDisposalSlipSearchResult_partial.cs
  ├─ SlipListGridViewへ検索結果を表示
  ├─ stockMonthly条件で修正ボタン非表示
  ├─ SlipCorrectionAvailabilityFlg == "0" で修正ボタン非表示
  └─ SlipCorrectionButton押下時、SCRCSAR050202へ遷移
```

## 7. 大福帳連携の呼出経路

```text
SalesChangeDisposalSlipEntryNewBusiness.cs
  ├─ 売変/廃棄の伝票種別をAP変数から取得
  ├─ DataIDをコード区分 DataId から取得
  │   ├─ 売変実績: "03"
  │   ├─ 売変その他: "02"
  │   └─ 廃棄: "04"
  ├─ ExtractAccountBookBaseStore(...)
  ├─ GenerateAccountBookDataSetNew(...)
  ├─ GenerateRedAccountBookDataSetNew(...)
  │   └─ CorrectionDiv = "-"
  └─ RegisterAccountBookForJournal(...)
```

## 8. データ層確認経路

```text
query_database_architecture("SARSlipHeader")
  └─ 個別DB資産未検出

query_database_architecture("SARSlip")
  └─ usp_SARSlipDateChangeAfterInventoryClose 系を検出

query_database_architecture("AccountBook")
  └─ usp_SARDeleteAccountBookUptake / usp_SARAccountBookUptakeSalesChange / usp_SARAccountBookCategory1Update 等を検出

query_database_architecture("CMCDataIDControl")
  └─ 売変大福帳取込系ストアドの影響テーブルとして検出
```

## 9. 影響分析で重点確認する分岐

| 分岐 | 確認内容 |
|---|---|
| `CancelMode == Constants.OnFlag` | 論理削除更新だけを抑止し、赤伝登録を維持できるか。 |
| ErrorTable経路 | 通常テーブルとErrorテーブルの両方を要求対象とするか。 |
| `SlipUpdateMode` | 赤伝登録モードを維持する必要がある。 |
| `SlipCorrectionAvailabilityFlg` | 元伝票・赤伝の訂正不可判定をどこで生成するか。 |
| DataID `"03"` | `SAR003` 相当の大福帳登録が維持されるか。 |
| DB接続不可 | MCPで未検出の実テーブル定義を後続でどう補完するか。 |
