# TC-15 Playwright Report

レポート種別:
プロンプトベースの想定Playwrightレポート

対象プロンプト:
`TC-15_TestProcess_Prompt.md`

対象テストケース:
TC-15 確認画面なしの場合

確認観点:
- 確認画面なし設定で伝票取消ボタンを押下する。
- 確認画面 `SCRCSAR050208` へ遷移しない。
- `DelSARSlipNew` 相当の取消処理が即時実行される。
- 完了メッセージ `MSGCSARI0028` が表示される。
- 取消後、元伝票と赤伝が検索可能かつ修正不可である。

Playwright作業範囲:
- 既存Playwright構成、fixture、認証方式に合わせてTC-15用UIテストを実施する。
- 取消ボタン押下後に確認画面へ遷移しないこと、完了メッセージが表示されることを検証する。
- DB確認はTC-02からTC-13相当の最小セットを実施する。

実行フロー:

→ browser_navigate { url: "${BASE_URL}" }

  - page "ログイン" または "メインメニュー"
  - button "ログイン"

→ browser_click { target: "ログイン" }

  - page "メインメニュー"
  - navigation item "売変・廃棄伝票入力"

→ browser_click { target: "売変・廃棄伝票入力" }

  - page "売変・廃棄伝票入力"
  - heading "売変・廃棄伝票入力"
  - textbox "店舗"
  - textbox "伝票番号"
  - button "検索" or "読込"
  - button "伝票取消"

→ browser_fill { target: "店舗", text: "0000001001" }
→ browser_fill { target: "伝票番号", text: "8842000000001" }
→ browser_fill { target: "売変・廃棄日", text: "2026-07-01" }
→ browser_click { target: "検索" }

  - page "売変・廃棄伝票入力"
  - text: "8842000000001"
  - text: "枝番 1" or "1"
  - button "伝票取消" enabled

→ db_query { purpose: "取消前DBスナップショット" }

  - 元伝票: exists
  - 元伝票 LogicalDelFlg = 0
  - 赤伝枝番 2: not exists or precondition state recorded

→ browser_click { target: "伝票取消" }

  - page "売変・廃棄伝票入力" or "完了状態"
  - no screen id "SCRCSAR050208"
  - message "MSGCSARI0028"

→ browser_assert_not_url { urlPattern: "**/SCRCSAR050208**" }
→ browser_assert_visible { text: "MSGCSARI0028" }

→ db_query { purpose: "取消後DB確認" }

  - 元伝票 LogicalDelFlg = 0
  - 赤伝枝番 2 exists
  - 赤伝 RedBlackDiv = 1
  - 赤伝 LogicalDelFlg = 0
  - 修正不可制御状態 recorded

→ browser_click { target: "伝票検索" }
→ browser_fill { target: "店舗", text: "0000001001" }
→ browser_fill { target: "伝票種別", text: "003" }
→ browser_fill { target: "伝票区分", text: "01" }
→ browser_fill { target: "伝票番号", text: "8842000000001" }
→ browser_fill { target: "売変・廃棄日", text: "2026-07-01" }
→ browser_click { target: "検索" }

  - page "伝票検索"
  - screen id "SCRCBYR040205"
  - table "検索結果"
  - row "元伝票":
    - text: "8842000000001"
    - text: "枝番 1" or "1"
    - no button "修正"
  - row "赤伝":
    - text: "8842000000001"
    - text: "枝番 2" or "2"
    - no button "修正"

→ browser_assert_visible { target: "検索結果 row", text: "8842000000001 / 枝番 1" }
→ browser_assert_visible { target: "検索結果 row", text: "8842000000001 / 枝番 2" }
→ browser_assert_hidden { target: "元伝票行 修正ボタン" }
→ browser_assert_hidden { target: "赤伝行 修正ボタン" }

期待結果:
- 確認画面 `SCRCSAR050208` へ遷移しない。
- 完了メッセージ `MSGCSARI0028` が表示される。
- 元伝票は `LogicalDelFlg = '0'` のまま残る。
- 新規赤伝が作成される。
- 元伝票と赤伝が検索可能で、両方とも修正不可になる。

証跡:
- 取消対象伝票を開いた画面スクリーンショット
- `MSGCSARI0028` 表示スクリーンショット
- 確認画面へ遷移していないことを示すURLまたは画面IDログ
- 取消前後のDB確認SQLと実行結果
- 一覧検索結果と修正不可状態スクリーンショット
- Playwright trace / video / 実行ログ

判定:
未実行。上記フローをPlaywright実行時の期待レポート形式として使用する。
