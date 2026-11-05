# TC-11 Playwright Report

レポート種別:
プロンプトベースの想定Playwrightレポート

対象プロンプト:
`TC-11_TestProcess_Prompt.md`

対象テストケース:
TC-11 画面一覧で元伝票と新規登録赤伝が検索できること

確認観点:
- 取消済み伝票 `8842000000001` を伝票検索画面またはカバーシート経由で検索する。
- 元伝票と新規登録赤伝の両方が一覧に表示される。
- DB上で両方とも `LogicalDelFlg = '0'` である。

Playwright作業範囲:
- 既存Playwright構成、fixture、認証方式に合わせてTC-11用UIテストを実施する。
- 伝票検索画面へ直接遷移できない場合は、メニューまたはカバーシート画面から遷移する。
- 一覧行は伝票番号、枝番、赤黒区分、店舗、日付など安定項目で特定する。

実行フロー:

→ browser_navigate { url: "${BASE_URL}" }

  - page "ログイン" または "メインメニュー"
  - button "ログイン"

→ browser_click { target: "ログイン" }

  - page "メインメニュー"
  - navigation item "伝票検索"
  - navigation item "カバーシート"

→ browser_click { target: "伝票検索" }

  - page "伝票検索"
  - screen id "SCRCBYR040205"
  - textbox "店舗"
  - textbox "伝票番号"
  - textbox "売変・廃棄日"
  - button "検索"

→ browser_fill { target: "店舗", text: "0000001001" }
→ browser_fill { target: "伝票種別", text: "003" }
→ browser_fill { target: "伝票区分", text: "01" }
→ browser_fill { target: "伝票番号", text: "8842000000001" }
→ browser_fill { target: "売変・廃棄日", text: "2026-07-01" }

  - page "伝票検索"
  - search condition "店舗: 0000001001"
  - search condition "伝票種別: 003"
  - search condition "伝票区分: 01"
  - search condition "伝票番号: 8842000000001"

→ browser_click { target: "検索" }

  - page "伝票検索"
  - table "検索結果"
  - row:
    - text: "8842000000001"
    - text: "枝番 1" or "1"
    - text: "黒伝" or "RedBlackDiv 0"
  - row:
    - text: "8842000000001"
    - text: "枝番 2" or "2"
    - text: "赤伝" or "RedBlackDiv 1"

→ browser_assert_visible { target: "検索結果 row", text: "8842000000001 / 枝番 1" }
→ browser_assert_visible { target: "検索結果 row", text: "8842000000001 / 枝番 2" }

→ db_query { purpose: "一覧表示対象DB確認" }

```sql
SELECT SlipBranchNo, RedBlackDiv, LogicalDelFlg, SlipRevisionFlg
FROM SARSlipHeader
WHERE CorporationCode = '1000'
  AND SlipDiv = '01'
  AND SlipTypeCd = '003'
  AND SlipNo = '8842000000001'
  AND StoreCd = '0000001001'
ORDER BY SlipBranchNo;
```

  - row: SlipBranchNo = 1, LogicalDelFlg = 0
  - row: SlipBranchNo = 2, LogicalDelFlg = 0

期待結果:
- 元伝票が一覧に表示される。
- 新規登録赤伝が一覧に表示される。
- 両方とも `LogicalDelFlg = '0'` として確認できる。

証跡:
- 検索条件入力後スクリーンショット
- 元伝票と赤伝の両方が表示された一覧スクリーンショット
- Playwright trace / video / 実行ログ
- DB確認SQLと実行結果

判定:
未実行。上記フローをPlaywright実行時の期待レポート形式として使用する。
