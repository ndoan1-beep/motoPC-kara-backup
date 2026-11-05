# Redmine 8842 証跡収集ファイル

## 1. 目的

`Redmine_8842_テスト仕様書.md` に基づき、売変・廃棄伝票取消仕様変更のテスト証跡を収集する。

本ファイルでは、各テストケースごとに以下を管理する。

- 実施内容
- 使用データ
- 期待結果
- 画面証跡
- SQL確認ファイル
- SQL実行結果貼付欄
- 判定

## 2. 共通情報

| 項目 | 内容 |
|---|---|
| 対象機能 | 売変・廃棄伝票入力画面 伝票取消 |
| 対象イベント | `SalesChangeDisposalSlipInputUpdate.SlipDeleateButton_Click` |
| 対象Facade | `DelSARSlipNew` |
| 対象Business | `UpInsSlipNew`, `UpInsSlipErrorExecuteNew`, `UpInsSlipExecuteForDenyNew` |
| 対象DAO | `SalesChangeDisposalSlipEntryDataAccessComponent` |
| 参照テスト仕様書 | `Redmine_8842_テスト仕様書.md` |
| 参照テストデータ | `Redmine_8842_テストデータ.md` |
| SQL確認フォルダー | `確認SQL` |

## 3. 共通テストデータ

| パラメータ | 値 | 用途 |
|---|---:|---|
| `@CorporationCode` | `1000` | 法人コード |
| `@StoreCd` | `0000001001` | 店舗コード |
| `@SlipTypeCd` | `003` | 売変伝票種別。廃棄伝票確認時は `004` |
| `@SlipDiv` | `01` | 伝票区分 |
| `@SlipNo` | `8842000000001` | TC-02からTC-15用 |
| `@ApprovedSlipNo` | `8842000000002` | TC-16、TC-17用 |
| `@SlipBranchNo` | `1` | 元伝票枝番 |
| `@RedSlipBranchNo` | `2` | 赤伝枝番 |
| `@DisposalChangeDate` | `2026-07-01` | 売変・廃棄日 |
| `@InputMediaDiv` | `1` | 入力媒体区分 |
| `@InputBatchNo` | `RB884200001` | 通常伝票入力バッチ番号 |
| `@ApprovedInputBatchNo` | `RB884200002` | 承認済み伝票入力バッチ番号 |
| `@InputSequenceNo` | `1` | 入力SEQ番号 |
| `@InputDate` | `2026-07-01` | 入力日 |
| `@InputTime` | `101500` | 入力時刻 |
| `@DataType` | `01` | データ種別 |
| `@UptakeSequenceNo` | `1` | 取込SEQ番号 |
| `@BatchNo` | `RB884200001` | カバーシートバッチ番号 |
| `@BatchDate` | `2026-07-01` | カバーシートバッチ日付 |
| `@stockMonthly` | `202604` | TC-09期待日付算出用 |

## 4. SQL確認ファイル一覧

| TC | SQL確認ファイル | 主な確認内容 |
|---|---|---|
| TC-01 | `確認SQL/UI関係_TC-01_未保存伝票取消.sql` | 未保存伝票で対象テーブルに更新・INSERTがないこと |
| TC-02 | `確認SQL/DB関係_TC-02_エラートラン論理削除なし.sql` | エラートランの `LogicalDelFlg = '0'` |
| TC-03 | `確認SQL/DB関係_TC-03_本伝票論理削除なし.sql` | 本伝票ヘッダ・明細の `LogicalDelFlg = '0'` |
| TC-04 | `確認SQL/DB関係_TC-04_赤伝ヘッダ作成.sql` | `SARSlipHeader` に赤伝枝番が作成されること |
| TC-05 | `確認SQL/DB関係_TC-05_赤伝ヘッダマイナス値.sql` | 赤伝ヘッダの数量・金額がマイナス値になること |
| TC-06 | `確認SQL/DB関係_TC-06_赤伝明細作成.sql` | `SARSlipDetail` に赤伝明細が作成されること |
| TC-07 | `確認SQL/DB関係_TC-07_赤伝明細マイナス値.sql` | 赤伝明細の数量・金額がマイナス値になること |
| TC-08 | `確認SQL/DB関係_TC-08_元伝票赤伝合計ゼロ.sql` | 元伝票 + 赤伝の対象項目合計が0になること |
| TC-09 | `確認SQL/DB関係_TC-09_ヘッダ訂正不可更新.sql` | `SARSlipHeader.SlipRevisionFlg = '1'` |
| TC-10 | `確認SQL/DB関係_TC-10_エラーヘッダFixDate更新.sql` | `SARSlipHeaderError.FixDate = '9999/12/31'` |
| TC-11 | `確認SQL/UI関係_TC-11_一覧検索で元伝票赤伝表示.sql` | 元伝票と赤伝が検索対象に残ること |
| TC-12 | `確認SQL/UI関係_TC-12_一覧修正ボタン非表示.sql` | 訂正不可判定用DB状態と修正ボタン非表示 |
| TC-13 | `確認SQL/DB関係_TC-13_カバーシート再集計.sql` | `SARCoverSheetMaster` の再集計結果 |
| TC-14 | `確認SQL/UI関係_TC-14_確認画面ありDB未更新.sql` | 確認画面遷移時点ではDB未更新であること |
| TC-15 | `確認SQL/UI関係_TC-15_確認画面なし即時取消.sql` | 確認画面なしで即時取消が行われること |
| TC-16 | `確認SQL/DB関係_TC-16_承認済み伝票取消.sql` | 承認済み伝票で赤伝作成・訂正不可更新が行われること |
| TC-17 | `確認SQL/DB関係_TC-17_大福帳DataID003.sql` | `CMCAccountBook.DataID = '003'` の既存動作維持 |

## 5. 証跡記入ルール

| 項目 | 記入内容 |
|---|---|
| 実施日時 | テスト実施日時 |
| 実施者 | テスト実施者 |
| 使用データ | TD-01、TD-02、TD-03、TD-04など |
| 画面証跡 | 画面キャプチャ、メッセージ、遷移先画面ID、ボタン表示状態 |
| SQL証跡 | SQL確認ファイルの実行結果を貼付 |
| 判定 | OK / NG / 保留 |
| 備考 | 差異、追加確認、再実施内容 |

SQL確認ファイルはすべて `BEGIN TRANSACTION` と `ROLLBACK TRANSACTION` を含む。実DBに反映が必要な検証を行う場合は、実施環境の運用ルールに従い、SQLのトランザクション制御を調整する。
各SQL確認ファイルの「テストデータのINSERT」には、対象TCの確認に必要なテストデータ投入SQLを記載する。TC-01は未保存伝票の確認であるため、対象業務テーブルにはINSERTせず、証跡用一時テーブルへセットアップ条件を記録する。

## 6. テストケース別証跡

### TC-01 保存されていない伝票を取消しようとした場合

| 項目 | 内容 |
|---|---|
| 使用データ | TD-01 |
| 操作 | 未保存状態の伝票で伝票取消ボタンを押下する |
| 期待結果 | `ERRCSARB0273` が表示される |
| DB期待結果 | `SARSlipHeaderError`, `SARSlipDetailError`, `SARSlipHeader`, `SARSlipDetail` に更新・INSERTがない |
| SQL確認ファイル | `確認SQL/UI関係_TC-01_未保存伝票取消.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 画面メッセージのキャプチャを添付する |

### TC-02 エラートランを論理削除しないこと

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 保存済み伝票を取消する |
| 期待結果 | `UpdateDelFlgSARSlipHeaderErrorNew`, `UpdateDelFlgSARSlipDetailErrorNew` が実行されない |
| DB期待結果 | `SARSlipHeaderError.LogicalDelFlg = '0'`, `SARSlipDetailError.LogicalDelFlg = '0'` |
| SQL確認ファイル | `確認SQL/DB関係_TC-02_エラートラン論理削除なし.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 取消前後のエラートラン状態を貼付する |

### TC-03 本伝票を論理削除しないこと

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 保存済み伝票を取消する |
| 期待結果 | `UpdateDelFlgSARSlipHeaderNew`, `UpdateDelFlgSARSlipDetailNew` が実行されない |
| DB期待結果 | 元伝票の `SARSlipHeader.LogicalDelFlg = '0'`, `SARSlipDetail.LogicalDelFlg = '0'` |
| SQL確認ファイル | `確認SQL/DB関係_TC-03_本伝票論理削除なし.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 元伝票枝番 `1` が残っていることを確認する |

### TC-04 `SARSlipHeader` に新規赤伝が作成されること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 対象伝票の取消を実行する |
| 期待結果 | `InsertSARSlipHeaderForDenyNew` が実行される |
| DB期待結果 | `SlipBranchNo = 2`, `RedBlackDiv = '1'`, `LogicalDelFlg = '0'` の赤伝ヘッダが作成される |
| SQL確認ファイル | `確認SQL/DB関係_TC-04_赤伝ヘッダ作成.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 元伝票枝番と赤伝枝番の両方を確認する |

### TC-05 `SARSlipHeader` のSQL指定項目がマイナス値になること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 対象伝票の取消を実行する |
| 期待結果 | 赤伝ヘッダの `TotalQty = -10`, `SlipTotalDisposalAmt = -1000` |
| DB期待結果 | SQL指定項目だけがマイナス値になる |
| SQL確認ファイル | `確認SQL/DB関係_TC-05_赤伝ヘッダマイナス値.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | マイナス指定外の項目は本TCの判定対象外 |

### TC-06 `SARSlipDetail` に新規赤伝明細が作成されること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 対象伝票の取消を実行する |
| 期待結果 | `InsertSARSlipDetailForDenyNew` が実行される |
| DB期待結果 | 元明細と同じ行数の赤伝明細が作成され、`LogicalDelFlg = '0'` |
| SQL確認ファイル | `確認SQL/DB関係_TC-06_赤伝明細作成.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 赤伝明細の `SlipBranchNo` が赤伝ヘッダと一致することを確認する |

### TC-07 `SARSlipDetail` のSQL指定項目がマイナス値になること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 対象伝票の取消を実行する |
| 期待結果 | 赤伝明細1行目 `DisposalChangeQty = -3`, `DisposalPriceAmt = -300`; 2行目 `-5`, `-500` |
| DB期待結果 | SQL指定項目だけがマイナス値になる |
| SQL確認ファイル | `確認SQL/DB関係_TC-07_赤伝明細マイナス値.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 単価項目は本TCの期待値対象外 |

### TC-08 SQL指定項目の元伝票と赤伝の合計が0になること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 対象伝票を取消後、元伝票 + 赤伝を集計する |
| 期待結果 | `SUM(TotalQty) = 0`, `SUM(SlipTotalDisposalAmt) = 0`, `SUM(DisposalChangeQty) = 0`, `SUM(DisposalPriceAmt) = 0` |
| DB期待結果 | ヘッダ・明細の対象項目が相殺される |
| SQL確認ファイル | `確認SQL/DB関係_TC-08_元伝票赤伝合計ゼロ.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | `LogicalDelFlg = '0'` のデータだけで集計する |

### TC-09 `SARSlipHeader` の訂正不可用更新が行われること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 対象伝票の取消を実行する |
| 期待結果 | `UpdateSlipRevisionFlgSARSlipHeaderNew` が実行される |
| DB期待結果 | `SARSlipHeader.SlipRevisionFlg = '1'`, `LogicalDelFlg = '0'` |
| SQL確認ファイル | `確認SQL/DB関係_TC-09_ヘッダ訂正不可更新.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | `@stockMonthly = 202604` の期待日付算出結果もSQL内で確認する |

### TC-10 `SARSlipHeaderError` の訂正不可用更新が行われること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 対象伝票の取消を実行する |
| 期待結果 | `UpdateSlipRevisionFlgSARSlipHeaderErrorNew` が実行される |
| DB期待結果 | `FixDate = '9999/12/31'`, `LogicalDelFlg = '0'` |
| SQL確認ファイル | `確認SQL/DB関係_TC-10_エラーヘッダFixDate更新.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | DB日付型の表示差異に注意する |

### TC-11 画面一覧で元伝票と新規登録赤伝が検索できること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 取消後、伝票検索画面またはカバーシート経由で対象伝票を検索する |
| 期待結果 | 元伝票と赤伝が一覧に表示される |
| DB期待結果 | 元伝票・赤伝とも `LogicalDelFlg = '0'` |
| SQL確認ファイル | `確認SQL/UI関係_TC-11_一覧検索で元伝票赤伝表示.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 一覧画面キャプチャで2枝番分の存在を補足する |

### TC-12 画面一覧で元伝票と新規登録赤伝の「修正」ボタンが表示されないこと

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 取消後、一覧で対象伝票を検索する |
| 期待結果 | 元伝票行・赤伝行の「修正」ボタンが表示されない |
| DB期待結果 | `SlipRevisionFlg = '1'`, `FixDate = '9999/12/31'` により訂正不可判定となる |
| SQL確認ファイル | `確認SQL/UI関係_TC-12_一覧修正ボタン非表示.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 修正画面へ遷移できないことも画面で確認する |

### TC-13 カバーシートが再集計されること

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | 取消後にカバーシート情報を確認する |
| 期待結果 | `TotalPageQty`, `InputPageCount`, `ErrorNo`, `ApprovalCount` が現行ロジック通り再計算される |
| DB期待結果 | `SARCoverSheetMaster.LogicalDelFlg` は変更しない |
| SQL確認ファイル | `確認SQL/DB関係_TC-13_カバーシート再集計.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 既存仕様値と差異がある場合は現行ロジック確認を行う |

### TC-14 確認画面ありの場合

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | `SAR_CheckDisplay_Off` により確認画面へ遷移する設定で伝票取消ボタンを押下する |
| 期待結果 | `workEntity.ButtonId = "5"`, `SCRCSAR050208` へ遷移する |
| DB期待結果 | 取消ボタン押下時点ではDB更新・赤伝INSERTは未実行 |
| SQL確認ファイル | `確認SQL/UI関係_TC-14_確認画面ありDB未更新.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 確認画面で確定後はTC-02からTC-13の結果になる |

### TC-15 確認画面なしの場合

| 項目 | 内容 |
|---|---|
| 使用データ | TD-02 |
| 操作 | `SAR_CheckDisplay_Off` により直接実行する設定で伝票取消ボタンを押下する |
| 期待結果 | `DelSARSlipNew` が即時実行され、`MSGCSARI0028` が表示される |
| DB期待結果 | TC-02からTC-13相当のDB結果になる |
| SQL確認ファイル | `確認SQL/UI関係_TC-15_確認画面なし即時取消.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 完了メッセージの画面キャプチャを添付する |

### TC-16 承認済み伝票の場合

| 項目 | 内容 |
|---|---|
| 使用データ | TD-03 |
| 操作 | `ApprovalStatus = '3'` の伝票を取消する |
| 期待結果 | 赤伝ヘッダ・赤伝明細が作成され、訂正不可用更新が行われる |
| DB期待結果 | `SARSlipHeader`, `SARSlipDetail` に赤伝が作成され、大福帳は既存仕様通り |
| SQL確認ファイル | `確認SQL/DB関係_TC-16_承認済み伝票取消.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | `CMCAccountBook` の詳細はTC-17で確認する |

### TC-17 `CMCAccountBook` の `DataID = '003'` 登録・更新動作が変わらないこと

| 項目 | 内容 |
|---|---|
| 使用データ | TD-03 |
| 操作 | 承認済み伝票の取消前後で大福帳を確認する |
| 期待結果 | 今回変更により、`CMCAccountBook` 側で想定外の追加・削除・論理削除が発生しない |
| DB期待結果 | `DataID = '003'` の件数・`LogicalDelFlg` が既存仕様通り |
| SQL確認ファイル | `確認SQL/DB関係_TC-17_大福帳DataID003.sql` |
| 画面証跡 | 未記入 |
| SQL証跡 | 未記入 |
| 判定 | 未実施 |
| 備考 | 大福帳の詳細登録仕様は `ICmcAccountBookComponent` 側の既存仕様に従う |

## 7. 最終判定

| 項目 | 内容 |
|---|---|
| 全体判定 | 未実施 |
| NG件数 | 未記入 |
| 保留件数 | 未記入 |
| 再実施要否 | 未記入 |
| 備考 | 未記入 |

