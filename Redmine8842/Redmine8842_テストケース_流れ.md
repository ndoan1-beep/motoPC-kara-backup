# Redmine 8842 ユニットテスト実施フロー案

## Summary
`Redmine_8842_テスト仕様書.md` に沿ってユニットテスト化する場合、主なテスト対象は「取消ボタンからBusiness/DAOへ渡る状態」「論理削除を呼ばないこと」「赤伝INSERT用データ/SQLが正しいこと」「訂正不可用の更新が呼ばれること」です。  
DB実更新・画面表示そのものはユニットテストではモック/フェイクで検証し、必要に応じて結合テストへ分離します。

## Test Flow

### Flow 1: 保存されていない伝票の取消
- Arrange: `SARSlipHeaderError.Rows.Count = 0` の `DsEntity` を用意する。
- Act: `SlipDeleateButton_Click` 相当の取消処理を呼ぶ。
- Assert:
  - `ERRCSARB0273` 相当のエラーが設定される。
  - `DelSARSlipNew` が呼ばれない。
  - DAO更新/INSERT系メソッドが一切呼ばれない。

### Flow 2: エラートラン論理削除を行わない
- Arrange: 保存済みの `SARSlipHeaderError` / `SARSlipDetailError` を持つ取消対象データを用意する。
- Act: 取消処理を実行する。
- Assert:
  - `UpdateDelFlgSARSlipHeaderErrorNew` が呼ばれない。
  - `UpdateDelFlgSARSlipDetailErrorNew` が呼ばれない。
  - `LogicalDelFlg` は `'0'` のまま。

### Flow 3: 本伝票論理削除を行わない
- Arrange: `SARSlipHeader` / `SARSlipDetail` に元伝票が存在する前提データを用意する。
- Act: `CancelMode = On` で `UpInsSlipNew` 相当を呼ぶ。
- Assert:
  - `UpdateDelFlgSARSlipHeaderNew` が呼ばれない。
  - `UpdateDelFlgSARSlipDetailNew` が呼ばれない。
  - 元伝票の `LogicalDelFlg` は `'0'` のまま。

### Flow 4: `SARSlipHeader` 新規赤伝作成
- Arrange: 元伝票ヘッダを用意し、次枝番を返すDAOフェイクを設定する。
- Act: `UpInsSlipExecuteForDenyNew` または赤伝作成処理を呼ぶ。
- Assert:
  - `InsertSARSlipHeaderForDenyNew` が呼ばれる。
  - 新枝番の赤伝が作成対象になる。
  - `RedBlackDiv = '1'`、`LogicalDelFlg = '0'`。

### Flow 5: `SARSlipHeader` SQL指定項目のマイナス化
- Arrange: 元伝票 `TotalQty = 10`, `SlipTotalDisposalAmt = 1000`。
- Act: ヘッダ赤伝INSERT SQL生成、またはINSERT用値生成を呼ぶ。
- Assert:
  - 赤伝 `TotalQty = -10`。
  - 赤伝 `SlipTotalDisposalAmt = -1000`。
  - SQL例でマイナス指定されていない項目はコピーのまま。

### Flow 6: `SARSlipDetail` 新規赤伝明細作成
- Arrange: 元明細を複数行用意する。
- Act: 明細赤伝作成処理を呼ぶ。
- Assert:
  - `InsertSARSlipDetailForDenyNew` が呼ばれる。
  - 元明細と同じ行数の赤伝明細が作成対象になる。
  - 赤伝明細の `SlipBranchNo` は赤伝ヘッダと一致する。
  - `LogicalDelFlg = '0'`。

### Flow 7: `SARSlipDetail` SQL指定項目のマイナス化
- Arrange: 元明細1行目 `DisposalChangeQty = 3`, `DisposalPriceAmt = 300`、2行目 `5`, `500`。
- Act: 明細赤伝INSERT SQL生成、またはINSERT用値生成を呼ぶ。
- Assert:
  - 赤伝明細1行目 `DisposalChangeQty = -3`, `DisposalPriceAmt = -300`。
  - 赤伝明細2行目 `DisposalChangeQty = -5`, `DisposalPriceAmt = -500`。
  - SQL例でマイナス指定されていない単価項目はコピーのまま。

### Flow 8: 元伝票 + 赤伝の相殺
- Arrange: 元伝票と赤伝のヘッダ/明細データをフェイクDBまたは集計用コレクションに用意する。
- Act: 集計処理または検証ヘルパーを実行する。
- Assert:
  - `SUM(TotalQty) = 0`。
  - `SUM(SlipTotalDisposalAmt) = 0`。
  - `SUM(DisposalChangeQty) = 0`。
  - `SUM(DisposalPriceAmt) = 0`。

### Flow 9: `SARSlipHeader` 訂正不可用更新
- Arrange: `stockMonthly = yyyyMM` を持つ `WorkEntity` または入力値を用意する。
- Act: `UpdateSlipRevisionFlgSARSlipHeaderNew` 相当を呼ぶ。
- Assert:
  - `SlipRevisionFlg = '1'` に更新される。
  - `DisposalChangeDate` は `DATEADD(month, -1, @stockMonthly + '01')` 相当になる。

### Flow 10: `SARSlipHeaderError` 訂正不可用更新
- Arrange: 対象 `SARSlipHeaderError` を用意する。
- Act: `UpdateSlipRevisionFlgSARSlipHeaderErrorNew` 相当を呼ぶ。
- Assert:
  - `FixDate = '9999/12/31'` に更新される。
  - `LogicalDelFlg = '0'` のまま。

### Flow 11: 一覧検索対象として残ること
- Arrange: 元伝票と新規赤伝を検索結果用フェイクデータに用意する。
- Act: 伝票一覧取得処理を呼ぶ。
- Assert:
  - 元伝票が検索結果に含まれる。
  - 新規赤伝が検索結果に含まれる。
  - 両方とも `LogicalDelFlg = '0'`。

### Flow 12: 修正ボタン非表示
- Arrange: `SlipRevisionFlg = '1'` または `FixDate = '9999/12/31'` の検索結果行を用意する。
- Act: 一覧行のボタン表示制御処理を呼ぶ。
- Assert:
  - 元伝票行の「修正」ボタンが表示されない。
  - 新規赤伝行の「修正」ボタンが表示されない。
  - 修正画面遷移処理が呼ばれない。

### Flow 13: 確認画面あり
- Arrange: `SAR_CheckDisplay_Off` が確認画面遷移になる条件を用意する。
- Act: 取消ボタン処理を呼ぶ。
- Assert:
  - `workEntity.ButtonId = "5"`。
  - `SCRCSAR050208` へ遷移する。
  - DB更新/赤伝INSERTはこの時点では呼ばれない。

### Flow 14: 確認画面なし
- Arrange: `SAR_CheckDisplay_Off` が直接実行になる条件を用意する。
- Act: 取消ボタン処理を呼ぶ。
- Assert:
  - `DelSARSlipNew` が即時呼ばれる。
  - Flow 2からFlow 12相当の処理が実行される。
  - `MSGCSARI0028` 相当の完了メッセージが設定される。

### Flow 15: 承認済み伝票
- Arrange: `ApprovalStatus = '3'` の取消対象伝票を用意する。
- Act: 取消処理を呼ぶ。
- Assert:
  - 赤伝ヘッダ/明細が作成される。
  - 訂正不可用更新が行われる。
  - 大福帳更新コンポーネント呼び出しが発生する。

## Test Boundaries
- ユニットテストではDAO/Facade/外部コンポーネントをモック化し、呼び出し有無・引数・生成SQL/生成値を検証する。
- 実DBへのINSERT/UPDATE、画面に実際にボタンが表示されるかは結合テストまたは画面テストで確認する。
- 大福帳 `ICmcAccountBookComponent` の内部登録内容は今回のユニットテスト対象外とする。

## Assumptions
- `stockMonthly` はテスト入力として直接渡せる形、または `WorkEntity` 等に格納済みの形で扱う。
- 修正ボタン非表示判定は、変更仕様書に従い `SlipRevisionFlg = '1'` および `FixDate = '9999/12/31'` の更新結果を使う。
- 二重取消防止は後続検討のため、今回のユニットテストフローには含めない。
