# TC-14 Playwright Report

レポート種別:
プロンプトベースの想定Playwrightレポート

対象プロンプト:
`TC-14_TestProcess_Prompt.md`

対象テストケース:
TC-14 確認画面ありの場合

確認観点:
- 確認画面あり設定で伝票取消ボタンを押下する。
- 取消ボタン押下直後はDB更新・赤伝INSERTが発生しない。
- 確認画面 `SCRCSAR050208` へ遷移する。
- 確認画面で確定後に取消処理が実行される。

Playwright作業範囲:
- 既存Playwright構成、fixture、認証方式に合わせてTC-14用UIテストを実施する。
- 取消ボタン押下直後と確認画面確定後の2段階でアサーションする。
- DB状態は取消前、確認画面表示後、確定後の3時点で取得する。

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

  - page "確認画面"
  - screen id "SCRCSAR050208"
  - text: "取消" or "確認"
  - button "確定"
  - button "戻る" or "キャンセル"

→ browser_assert_url_or_screen { expected: "SCRCSAR050208" }
→ browser_assert_state { expected: "workEntity.ButtonId = 5 相当" }

→ db_query { purpose: "確認画面表示時点DB確認" }

  - 元伝票 LogicalDelFlg = 0
  - 赤伝INSERT: not yet executed
  - SARSlipHeader / SARSlipDetail / Error tables: no cancellation update at this point

→ browser_click { target: "確定" }

  - page "取消完了" or "売変・廃棄伝票入力"
  - message: completion message or completed state

→ browser_click { target: "伝票検索" }
→ browser_fill { target: "伝票番号", text: "8842000000001" }
→ browser_click { target: "検索" }

  - page "伝票検索"
  - table "検索結果"
  - row "元伝票": text "枝番 1"
  - row "赤伝": text "枝番 2"
  - no button "修正" in both rows

→ db_query { purpose: "確定後DB確認" }

  - 元伝票 LogicalDelFlg = 0
  - 赤伝枝番 2 exists
  - 赤伝 LogicalDelFlg = 0
  - 修正不可制御状態 recorded

期待結果:
- 取消ボタン押下後、確認画面 `SCRCSAR050208` へ遷移する。
- 確認画面で確定する前はDB更新・赤伝INSERTが発生しない。
- 確認画面で確定後に取消処理が実行される。
- 確定後、元伝票と赤伝が検索でき、両方とも修正不可になる。

証跡:
- 取消対象伝票を開いた画面スクリーンショット
- 確認画面 `SCRCSAR050208` のスクリーンショット
- 確認画面表示時点のDB未更新SQL結果
- 確定後の完了状態スクリーンショット
- 確定後の一覧検索結果と修正不可状態スクリーンショット
- Playwright trace / video / 実行ログ

判定:
未実行。上記フローをPlaywright実行時の期待レポート形式として使用する。
