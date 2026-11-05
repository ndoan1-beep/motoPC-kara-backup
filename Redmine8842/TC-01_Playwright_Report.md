# TC-01 Playwright Report

レポート種別:
プロンプトベースの想定Playwrightレポート

対象プロンプト:
`TC-01_TestProcess_Prompt.md`

対象テストケース:
TC-01 保存されていない伝票を取消しようとした場合

確認観点:
- 未保存伝票で伝票取消ボタンを押下する。
- エラー `ERRCSARB0273` が表示される。
- 確認画面、完了画面、完了メッセージ `MSGCSARI0028` に進まない。
- `SARSlipHeaderError`、`SARSlipDetailError`、`SARSlipHeader`、`SARSlipDetail` に更新・INSERTが発生しない。

Playwright作業範囲:
- 既存Playwright構成、fixture、認証方式に合わせてTC-01用UIテストを実施する。
- 対象URL、ログイン情報、DB接続情報は既存設定または環境変数を利用する。
- locatorは `getByRole`、`getByLabel`、表示テキストを優先する。

実行フロー:

→ browser_navigate { url: "${BASE_URL}" }

  - page "ログイン" または "メインメニュー"
  - textbox "ユーザーID"
  - textbox "パスワード"
  - button "ログイン"

→ browser_click { target: "ログイン" }

  - page "メインメニュー"
  - navigation item "売変・廃棄"
  - navigation item "伝票入力"

→ browser_click { target: "売変・廃棄伝票入力" }

  - page "売変・廃棄伝票入力"
  - heading "売変・廃棄伝票入力"
  - textbox "店舗"
  - textbox "伝票番号"
  - button "保存"
  - button "伝票取消"

→ browser_fill { target: "店舗", text: "0000001001" }
→ browser_fill { target: "伝票番号", text: "8842000000099" }
→ browser_fill { target: "売変・廃棄日", text: "2026-07-01" }

  - page "売変・廃棄伝票入力"
  - text: 未保存伝票入力中
  - button "伝票取消" enabled
  - button "保存" enabled

→ db_query { purpose: "取消前DB確認", slipNo: "8842000000099" }

  - SARSlipHeaderError: 0 rows
  - SARSlipDetailError: 0 rows
  - SARSlipHeader: 0 rows
  - SARSlipDetail: 0 rows

→ browser_click { target: "伝票取消" }

  - page "売変・廃棄伝票入力"
  - alert or message: "ERRCSARB0273"
  - button "OK" or close action

→ browser_assert_visible { text: "ERRCSARB0273" }
→ browser_assert_not_visible { text: "MSGCSARI0028" }
→ browser_assert_not_url { urlPattern: "**/SCRCSAR050208**" }

  - page "売変・廃棄伝票入力"
  - message "ERRCSARB0273"
  - no completion message "MSGCSARI0028"
  - no confirmation screen "SCRCSAR050208"

→ db_query { purpose: "取消後DB確認", slipNo: "8842000000099" }

  - SARSlipHeaderError: 0 rows
  - SARSlipDetailError: 0 rows
  - SARSlipHeader: 0 rows
  - SARSlipDetail: 0 rows

期待結果:
- `ERRCSARB0273` が表示される。
- 取消完了扱いにならない。
- 確認画面へ遷移しない。
- DB更新・INSERTが発生しない。

証跡:
- 取消ボタン押下前スクリーンショット
- `ERRCSARB0273` 表示後スクリーンショット
- Playwright trace / video / 実行ログ
- 取消前後のDB確認結果

判定:
未実行。上記フローをPlaywright実行時の期待レポート形式として使用する。
