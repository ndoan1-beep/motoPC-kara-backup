# 8842_02_現行動作サマリー

最新更新時刻：　2026年08月18日16:03:47

## 1. 全体サマリー

現行の売変・廃棄伝票取消処理では、UIで取消モードを設定し、Facade経由でBusinessへ登録処理を渡す。Businessは取消モードを検知すると赤伝モードへ切り替え、元伝票のヘッダー/明細に論理削除フラグを設定する。その後、赤伝登録と大福帳登録は既存経路で行われる。

Redmine 8842 は、このうち「元伝票へ `LogicalDelFlg` をセットする」部分を見直しつつ、赤伝登録、数量・金額相殺、検索表示、訂正不可制御、大福帳連携を維持・実現する要求である。

## 2. 取消時の現行動作

| 順序 | 現行処理 | MCP根拠 |
|---|---|---|
| 1 | UIで `CancelMode = Constants.OnFlag` を設定 | `SalesChangeDisposalSlipInputUpdate_partial.cs` 1536-1548 |
| 2 | `UpInsSARSlipNew(...)` をFacadeへ呼び出し | 同 2360-2370 |
| 3 | FacadeがBusinessの `UpInsSlipNew(...)` を呼び出し | `SalesChangeDisposalSlipEntryNewFacade.cs` 140-180 |
| 4 | Businessが取消モードを判定し `SlipUpdateMode = slipUpdateMode_Red` を設定 | `SalesChangeDisposalSlipEntryNewBusiness.cs` 2174-2210 |
| 5 | 通常/エラー伝票に削除フラグをセット | 同 2174-2210、2448-2463 |
| 6 | `slipUpdateMode_Red` の場合、赤伝登録処理を実行 | 同 2505-2515 |
| 7 | DataIDに応じて大福帳登録/赤伝大福帳登録を実行 | 同 2658-2992、6197-6265 |

## 3. 論理削除の現行仕様

現行取消時は次の4テーブルに更新が入る。

| 対象 | 更新内容 |
|---|---|
| `SARSlipHeader` | `LogicalDelFlg = '1'`, `LogicalDelDate`, `UpdateBy`, `UpdateDate` |
| `SARSlipDetail` | 同上 |
| `SARSlipHeaderError` | 同上 |
| `SARSlipDetailError` | 同上 |

DB接続は使用していない。上記はMCPで読んだDataAccess SQLを根拠とする。

## 4. 赤伝登録の現行仕様

| モード | 現行動作 |
|---|---|
| `slipUpdateMode_Black` | 黒伝を作成する。 |
| `slipUpdateMode_Red` | 赤伝を作成する。取消時はこのモードになる。 |
| `slipUpdateMode_RedBlack` | 黒伝と赤伝の両方を作成する。 |

赤伝登録本体は `UpInsSlipExecuteForDenyNew` である。

## 5. 数量・金額の現行扱い

検索明細処理では次の算出が行われる。

| 項目 | 現行扱い |
|---|---|
| `SalesChangePrice` | `(SlipOldPrice - SlipNewPrice) * DisposalChangeQty` |
| `DisposalPriceAmt` | `SlipDisposalUnitPrica * DisposalChangeQty` |

数量・金額相殺の実現方式は未確定であり、03_影響分析で期待値の確認が必要。

## 6. 検索画面の現行動作

| 区分 | 現行動作 |
|---|---|
| 検索元 | `SARSlipHeader` と `SARSlipHeaderError` を `UNION` したDataSetを使用。 |
| 修正ボタン | `SCRCSAR050306002` の1列目に「修正」ボタンを持つ。 |
| 非表示条件 | 在庫月度より過去、または `SlipCorrectionAvailabilityFlg == "0"` の場合に非表示。 |
| 訂正遷移 | ボタン押下時に選択行キーをセッションに格納し、`SCRCSAR050202` へ遷移。 |

## 7. 大福帳連携の現行動作

| 区分 | 現行動作 |
|---|---|
| 売変 | DataID `"03"` または `"02"` をコード区分から取得して登録。 |
| 廃棄 | DataID `"04"` を使用。廃棄時には売変DataID `"03"` 側も使用する経路がある。 |
| 赤伝 | `GenerateRedAccountBookDataSetNew` で `CorrectionDiv = "-"` を設定し、符号系項目を反転する。 |
| データ層候補 | `query_database_architecture` で `CMCDataIDControl`、`CMCAccountBook_SAR001` などが関連ストアドの影響テーブルとして検出された。 |

## 8. 現行動作から見た変更候補

| 候補 | 内容 | 注意点 |
|---|---|---|
| 論理削除更新の抑止/条件分岐 | 取消時の `UpdateDelFlg...New` 呼出をRedmine 8842対象で止める | 赤伝登録と大福帳登録を止めない。 |
| 訂正不可制御追加 | 元伝票・赤伝の `SlipCorrectionAvailabilityFlg` 相当を不可にする | UI非表示だけで足りるか確認。 |
| 検索条件確認 | `LogicalDelFlg` 条件が検索にあるか確認 | 元伝票と赤伝を表示する要件に直結。 |
| 数量・金額相殺 | 対象項目の符号/値を定義 | 業務期待値の確定が必要。 |
| DB実スキーマ確認 | DB接続可能後に実テーブル定義を確認 | MCP DB照会では個別テーブル定義が未検出。 |

## 9. 次フェーズへの引き継ぎ

- `UpdateDelFlg...New` 呼出抑止の影響範囲を分析する。
- `SlipCorrectionAvailabilityFlg` の生成元を確認する。
- `DataId "03"` が `SAR003` へ解決されることをDB/マスタで確認する。
- 数量・金額相殺の期待値を業務側へ確認する。
- DB接続可能後に実テーブル定義と既存データ補正要否を確認する。
