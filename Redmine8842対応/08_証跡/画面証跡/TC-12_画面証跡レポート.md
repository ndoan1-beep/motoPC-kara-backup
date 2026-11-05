# TC-12 Playwright Report

レポート種別:
プロンプトベースの想定Playwrightレポート

対象プロンプト:
`TC-12_TestProcess_Prompt.md`

対象テストケース:
TC-12 画面一覧で元伝票と新規登録赤伝の「修正」ボタンが表示されないこと

確認観点:
- 取消済み伝票 `8842000000001` を一覧検索する。
- 元伝票行と赤伝行の両方に「修正」操作が表示されない。
- 行クリック、ダブルクリック、行選択後操作などで修正画面へ遷移できない。

Playwright作業範囲:
- 既存Playwright構成、fixture、認証方式に合わせてTC-12用UIテストを実施する。
- 一覧行単位で「修正」ボタン、修正リンク、編集アイコン、行アクションメニューを確認する。
- 文言差がある場合も「修正」相当の操作導線がないことを確認する。

実行フロー:

→ browser_navigate { url: "${BASE_URL}" }

  - page "ログイン" または "メインメニュー"
  - button "ログイン"

→ browser_click { target: "ログイン" }

  - page "メインメニュー"
  - navigation item "伝票検索"

→ browser_click { target: "伝票検索" }

  - page "伝票検索"
  - screen id "SCRCBYR040205"
  - textbox "店舗"
  - textbox "伝票番号"
  - button "検索"

→ browser_fill { target: "店舗", text: "0000001001" }
→ browser_fill { target: "伝票種別", text: "003" }
→ browser_fill { target: "伝票区分", text: "01" }
→ browser_fill { target: "伝票番号", text: "8842000000001" }
→ browser_fill { target: "売変・廃棄日", text: "2026-07-01" }
→ browser_click { target: "検索" }

  - page "伝票検索"
  - table "検索結果"
  - row "元伝票":
    - text: "8842000000001"
    - text: "枝番 1" or "1"
    - no button "修正"
    - no link "修正"
    - no menu item "修正"
  - row "赤伝":
    - text: "8842000000001"
    - text: "枝番 2" or "2"
    - no button "修正"
    - no link "修正"
    - no menu item "修正"

→ browser_assert_hidden { target: "元伝票行 修正ボタン" }
→ browser_assert_hidden { target: "赤伝行 修正ボタン" }
→ browser_assert_hidden { target: "元伝票行 編集アイコン" }
→ browser_assert_hidden { target: "赤伝行 編集アイコン" }

→ browser_dblclick { target: "元伝票行" }

  - page remains "伝票検索"
  - no navigation to "修正画面"
  - no edit form displayed

→ browser_dblclick { target: "赤伝行" }

  - page remains "伝票検索"
  - no navigation to "修正画面"
  - no edit form displayed

→ db_query { purpose: "修正不可制御根拠確認" }

  - SARSlipHeader.SlipRevisionFlg = 1
  - SARSlipHeaderError.FixDate = 9999/12/31
  - LogicalDelFlg = 0

期待結果:
- 元伝票行に「修正」ボタンが表示されない。
- 新規登録赤伝行に「修正」ボタンが表示されない。
- 修正画面へ遷移できない。
- TC-11相当の一覧検索可能状態は維持される。

証跡:
- 元伝票と赤伝の両方が表示された一覧スクリーンショット
- 各行に修正操作が表示されていないスクリーンショット
- 修正画面へ遷移できないことを示すPlaywright trace / video / 実行ログ
- `SlipRevisionFlg`、`FixDate` のDB確認結果

判定:
未実行。上記フローをPlaywright実行時の期待レポート形式として使用する。
