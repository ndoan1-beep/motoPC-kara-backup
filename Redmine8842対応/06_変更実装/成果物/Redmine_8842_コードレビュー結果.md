# コードレビュー結果

## 1. 目的

本資料は、AIにより変更された Redmine 8842 対応コードについて、人間レビューで確認すべき項目を整理する。

参照元:

- `06_コード変更/成果物/Redmine_8842_コード修正報告書.md`

## 2. レビュー対象

| No | 対象ファイル | 主な確認対象 |
|---|---|---|
| 1 | `SalesChangeDisposalSlipInputUpdate_partial.cs` | 取消ボタン押下時の画面側 DataSet 更新 |
| 2 | `SalesChangeDisposalSlipEntryNewBusiness.cs` | 取消分岐、赤伝作成、論理削除抑止、訂正不可更新 |
| 3 | `SalesChangeDisposalSlipEntryNewDataAccessComponent.cs` | 追加DAO、赤伝INSERT SQL、ParameterList |

## 3. レビュー総合判定

| 項目 | 判定 | コメント |
|---|---|---|
| コード内容レビュー | 要人手確認 | AI変更後の静的確認は実施済みだが、人間による業務観点・副作用確認が必要。 |
| ビルド確認 | 未確認 | 修正報告書上、MSBuild未実施。Visual Studio / MSBuild環境で確認が必要。 |
| DB更新妥当性 | 要人手確認 | `LogicalDelFlg`、`SlipRevisionFlg`、`FixDate`、赤伝INSERTの更新仕様を重点確認する。 |
| 画面動作 | 要人手確認 | 元伝票・赤伝の検索可否、修正ボタン非表示を画面で確認する。 |
| 大福帳連携 | 要人手確認 | `CMCAccountBook` / `UpInsAccountBookNew` は未変更方針だが、既存通り動作することを確認する。 |

## 4. 人間レビュー必須項目

| ID | 優先度 | 確認項目 | 確認理由 |
|---|---|---|---|
| REV-001 | High | 取消時に `UpdateDelFlgSARSlipHeaderNew` / `UpdateDelFlgSARSlipDetailNew` が呼ばれないこと。 | 元伝票を論理削除しない要件の中核。 |
| REV-002 | High | 取消時に `UpdateDelFlgSARSlipHeaderErrorNew` / `UpdateDelFlgSARSlipDetailErrorNew` が呼ばれないこと。 | エラートラン側が論理削除されないことを確認するため。 |
| REV-003 | High | `UpInsSlipExecuteForDenyNew(workEntity, dsEntity)` が取消時に必ず呼ばれること。 | 赤伝を新規作成する要件の中核。 |
| REV-004 | High | 赤伝ヘッダINSERTで `TotalQty`、`SlipTotalDisposalAmt` のみが仕様どおりマイナス化されていること。 | 数量・金額相殺の対象項目誤りを防ぐため。 |
| REV-005 | High | 赤伝明細INSERTで `DisposalChangeQty`、`DisposalPriceAmt` のみが仕様どおりマイナス化されていること。 | 明細側の相殺誤りを防ぐため。 |
| REV-006 | High | 赤伝ヘッダ・明細の `LogicalDelFlg` が `'0'` で登録されること。 | 赤伝も検索可能にするため。 |
| REV-007 | High | `UpdateSlipRevisionFlgSARSlipHeaderNew` が元伝票と赤伝を訂正不可にする仕様として妥当か確認すること。 | `SlipBranchNo` を条件に含めないため、対象範囲確認が必要。 |
| REV-008 | High | `UpdateFixDateSARSlipHeaderErrorNew` で `FixDate = '9999/12/31'` とする仕様が妥当か確認すること。 | エラーヘッダ側の訂正不可表現として正しいか判断が必要。 |
| REV-009 | High | `CMCAccountBook` / `UpInsAccountBookNew` の既存経路が変更されていないこと。 | 在庫サブシステム連携を壊さないため。 |
| REV-010 | Medium | `SARSlipDetailError` を取消時に更新しないことで、既存画面・バッチに副作用がないこと。 | 修正報告書上の想定リスク。 |
| REV-011 | Medium | 追加ParameterListのパラメータ不足・順序不整合がないこと。 | SQL実行時エラーを防ぐため。 |
| REV-012 | Medium | 既存のコメント、命名、`New` サフィックス、DAO実装スタイルと整合していること。 | CCR既存コード規約維持のため。 |

## 5. ファイル別レビュー観点

### 5.1 `SalesChangeDisposalSlipInputUpdate_partial.cs`

| ID | 確認項目 |
|---|---|
| UI-001 | `SlipDeleateButton_Click` で `SARSlipHeaderError` の `LogicalDelFlg` が `"0"` 維持になっていること。 |
| UI-002 | `SARSlipDetailError` の `LogicalDelFlg` も `"0"` 維持になっていること。 |
| UI-003 | 画面側DataSetだけでなく、Business側処理との整合が取れていること。 |
| UI-004 | 取消ボタン押下後の遷移、メッセージ、セッション情報に副作用がないこと。 |

### 5.2 `SalesChangeDisposalSlipEntryNewBusiness.cs`

| ID | 確認項目 |
|---|---|
| BS-001 | `UpInsSlipNew` の取消分岐で、赤伝作成、訂正不可更新、`FixDate` 更新の呼出順が妥当であること。 |
| BS-002 | 取消時に本伝票・エラートランの論理削除処理が実行されないこと。 |
| BS-003 | 通常更新時の既存処理に影響がないこと。 |
| BS-004 | 承認済み伝票、承認不要モード、カバーシート経由など既存分岐への副作用がないこと。 |
| BS-005 | `UpInsSlipErrorExecuteNew` の取消時コメントと実処理が矛盾していないこと。 |

### 5.3 `SalesChangeDisposalSlipEntryNewDataAccessComponent.cs`

| ID | 確認項目 |
|---|---|
| DAO-001 | `UpdateSlipRevisionFlgSARSlipHeaderNew` のWHERE条件が仕様どおりであること。 |
| DAO-002 | `UpdateSlipRevisionFlgSARSlipHeaderNew` で `LogicalDelFlg = '0'` 条件が入っていること。 |
| DAO-003 | `SlipBranchNo` を条件に入れないことが業務仕様として正しいこと。 |
| DAO-004 | `UpdateFixDateSARSlipHeaderErrorNew` のWHERE条件が対象を過不足なく絞れていること。 |
| DAO-005 | 赤伝ヘッダINSERTの列リストとSELECT値リストの対応がずれていないこと。 |
| DAO-006 | 赤伝明細INSERTの列リストとSELECT値リストの対応がずれていないこと。 |
| DAO-007 | 追加ParameterListで `@CorporationCode`、`@SlipDiv`、`@SlipTypeCd`、`@SlipNo`、`@DisposalChangeDate`、`@StoreCd` が正しく設定されていること。 |
| DAO-008 | SQL文字列に不正なエスケープ、引用符、カンマ不足がないこと。 |

## 6. 動作確認で人間が見るべき項目

| ID | テスト観点 | 期待結果 |
|---|---|---|
| TEST-001 | 売変伝票取消 | 元伝票の `LogicalDelFlg` が `'0'` のまま維持され、赤伝が作成される。 |
| TEST-002 | 廃棄伝票取消 | 元伝票の `LogicalDelFlg` が `'0'` のまま維持され、赤伝が作成される。 |
| TEST-003 | 赤伝数量・金額 | 指定項目だけがマイナス化される。 |
| TEST-004 | 伝票検索 | 元伝票と赤伝の両方が検索結果に表示される。 |
| TEST-005 | 修正ボタン | 元伝票と赤伝の修正ボタンが表示されない。 |
| TEST-006 | 訂正直接遷移 | 画面操作以外の経路でも訂正可能にならないことを確認する。 |
| TEST-007 | 大福帳 | `CMCAccountBook.DataID = 'SAR003'` の登録・更新が既存通りである。 |
| TEST-008 | 通常登録・通常修正 | 売変・廃棄取消以外の既存動作が変わらない。 |

## 7. 残リスク

| ID | リスク | 対応 |
|---|---|---|
| RISK-001 | MSBuild未実施のため、コンパイルエラーが残っている可能性がある。 | Visual Studio / MSBuild環境でビルド確認する。 |
| RISK-002 | `SlipBranchNo` を条件に入れない更新が想定外の枝番へ影響する可能性がある。 | 業務仕様とテストデータで対象範囲を確認する。 |
| RISK-003 | `SARSlipDetailError` 未更新が既存処理の前提と異なる可能性がある。 | 検索、再取消、承認、バッチ処理の回帰確認を行う。 |
| RISK-004 | 大福帳処理は未変更でも、赤伝登録内容変更により連携結果が変わる可能性がある。 | `CMCAccountBook` の登録結果をDBで確認する。 |

## 8. 人間レビュー完了条件

- 対象3ファイルの差分レビューが完了していること。
- Visual Studio / MSBuildでビルドが成功していること。
- 売変・廃棄それぞれの取消テストが完了していること。
- DB確認で、元伝票、赤伝、エラーヘッダ、大福帳の期待値が確認できていること。
- 残リスクに対する対応方針が決定していること。

