# TC-11 TestProcess Prompt

チケット番号:
Redmine #8842

作業種別:
テスト（Playwright UI）

目的:
取消済みの売変・廃棄伝票について、伝票検索画面またはカバーシート経由の一覧で、元伝票と新規登録赤伝の両方が検索できることをPlaywrightで確認する。

背景:
Redmine #8842の変更後仕様では、伝票取消時に元伝票を論理削除せず保持し、新しい枝番の赤伝を登録する。画面一覧では、元伝票と赤伝の両方を検索可能にする必要がある。

対象機能:
- 伝票検索画面 `SCRCBYR040205`
- カバーシート経由の伝票一覧
- 売変・廃棄伝票取消後の一覧表示

参照すべき資料:
- `Redmine_8842_テスト仕様書.md` の `TC-11`
- `Redmine8842.UnitTests/TestData.md` の 10行目（最新確定データ / TD-02相当）
- `Redmine8842_テストケース_流れ.md` の `Flow 11`

前提データ:
- `TestData.md` 10行目（TD-02）の保存済み売変伝票が取消済みであること。
- 主な検索キー:
- `@CorporationCode = 0001`
- `@StoreCd = 035`
- `@SlipTypeCd = 07`
- `@SlipDiv = 02`
- `@SlipNo = 010000017`
- 元伝票 `@SlipBranchNo = 1`
- 赤伝 `@RedSlipBranchNo = 2`
- `@DisposalChangeDate = 2026-07-24`
- 取消済みデータが未作成の場合は、TC-14またはTC-15相当の取消操作で事前作成する。

Playwright作業範囲:
- 既存のPlaywright構成を確認し、既存のテスト配置・fixture・認証方式に合わせて `TC-11` 用のUIテストを作成または実行する。
- 検索画面へ直接遷移できない場合は、メニューまたはカバーシート画面から遷移する。
- 一覧行の特定は、伝票番号、枝番、赤黒区分、店舗、日付など画面に表示される安定項目を組み合わせる。

テスト手順:
1. Playwrightで対象アプリへアクセスし、必要に応じてログインする。
2. 伝票検索画面 `SCRCBYR040205`、またはカバーシート経由の対象伝票一覧を開く。
3. 店舗 `035`、伝票種別 `07`、伝票区分 `02`、伝票番号 `010000017`、売変・廃棄日 `2026-07-24` で検索する。
4. 検索結果一覧に、元伝票の行が表示されることを確認する。
5. 検索結果一覧に、新規登録赤伝の行が表示されることを確認する。
6. 画面に枝番または赤黒区分が表示される場合、元伝票が枝番 `1`、赤伝が枝番 `2` または赤伝を示す区分で表示されることを確認する。
7. DB確認で、該当する元伝票と赤伝の `LogicalDelFlg = '0'` を確認する。

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

主な期待結果:
- 元伝票が一覧に表示される。
- 新規登録赤伝が一覧に表示される。
- 両方とも `LogicalDelFlg = '0'` のデータとして確認できる。
- 検索条件を満たす行が1件だけになる、または赤伝だけになる動作は不合格とする。

証跡:
- 検索条件入力後の画面スクリーンショット。
- 元伝票と赤伝の両方が表示された一覧スクリーンショット。
- Playwright trace、video、または実行ログ。
- DB確認SQLと実行結果。

制約:
- 検索結果の表示順には依存しない。
- 画面に `LogicalDelFlg` が表示されない場合は、画面では一覧表示を確認し、DBで `LogicalDelFlg` を確認する。
- 既存アプリケーションコードは変更しない。

完了条件:
- PlaywrightでTC-11の検索操作と一覧表示アサーションが実行できること。
- 元伝票と赤伝の両方が画面証跡に残っていること。
- DB確認により両方が論理削除されていないことを確認できること。


