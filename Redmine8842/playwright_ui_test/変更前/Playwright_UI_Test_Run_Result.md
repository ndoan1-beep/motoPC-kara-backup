# Playwright UI Test Run Result

実行日: 2026-10-01
指定BASE_URL: http://127.0.0.1:8080/CoreCenter/
ログインユーザ: 0003123490

## Search Condition

`SCRCSAR050215` で以下の条件を使用した。

- `部門コード = 017`
- `入力媒体区分 = 1:画面`
- `期間` は From/To とも空欄
- `伝票種別・区分` は未選択

## Result From Specified BASE_URL

`127.0.0.1:8080/CoreCenter` から検索を実行すると、検索後に以下へ遷移した。

`http://crmapv01:8080/CoreCenter/login.aspx?...SalesChangeDisposalSlipSearchResult.aspx...`

その後ログインすると `不正な画面操作を行いました。` になった。
`crmapv01` に事前ログインしてから `127.0.0.1` 側で再検索しても、検索後は `不正な画面操作` になった。

## Result From crmapv01

同じ条件で `crmapv01:8080/CoreCenter` の `SCRCSAR050215` を直接開いて検索した場合、検索結果画面へ遷移できた。

結果画面:

- URL: `SalesChangeDisposalSlipSearchResult.aspx`
- Title: `売変廃棄伝票検索 (SCRCSAR050225)`

検索結果には対象伝票が表示された。

- 売変・廃棄日: `2026/07/24`
- 伝票種別: `07`
- 伝票種別名称: `値下`
- 伝票区分: `02`
- 伝票区分名称: `売変伝票`
- 伝票番号: `010000017`
- 店舗コード: `035`
- 部門コード: `017`
- 合計数量: `3`
- 金額: `1,350`
- 入力バッチ番号: `0200000003`

`修正` ボタンは表示されていた。
`修正` を押下すると画面は遷移せず、`！該当伝票は既に承認されています。！` が表示された。

伝票番号リンク `010000017` は detail 画面 `SCRCSAR050235` を開けた。
Detail 画面では伝票情報が readonly 表示され、`伝票取消` は表示されなかった。

## Evidence

- `TC00_login_before_submit.png`
- `TC00_crmapv01_prelogin_menu.png`
- `TC11_12_search_initial_blank_condition_snapshot.yml`
- `TC11_12_search_dept017_blank_period_type_before_search.png`
- `TC11_12_after_search_redirect_login_crmapv01.png`
- `TC11_12_after_crmapv01_relogin_illegal_operation.png`
- `TC11_12_retry_after_crmapv01_prelogin_before_search.png`
- `TC11_12_retry_after_crmapv01_prelogin_illegal_operation.png`
- `TC11_12_crmapv01_direct_before_search.png`
- `TC11_12_crmapv01_direct_search_result.png`
- `TC11_12_result_list_found_010000017.png`
- `TC12_click_correction_approved_message.png`
- `TC11_detail_010000017.png`

## Notes

検索条件自体は正しく、`crmapv01` 直接実行では対象伝票を取得できた。
現在の blocker は、指定 BASE_URL `127.0.0.1` から検索結果へ遷移する際に host が `crmapv01` へ変わり、session が維持されず `不正な画面操作` になる点。
