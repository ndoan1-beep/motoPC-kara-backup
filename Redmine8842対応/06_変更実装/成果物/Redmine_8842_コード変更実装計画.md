# Redmine 8842 コード修正報告書

## 1. 実施概要

`Redmine_8842_変更仕様書.md` と `Redmine_8842_コード修正プラン.md` に基づき、【標準版】Branch2.2 の「売変・廃棄の伝票取消」処理を修正した。

主な変更内容は以下。

- 取消時に `LogicalDelFlg = '1'` をセットしない。
- 取消時に本伝票を論理削除せず、赤伝を新規作成する。
- 赤伝 INSERT 時、仕様書で指定された数量・金額項目だけをマイナス化する。
- 元伝票と赤伝を検索可能なまま、訂正不可にする。
- `SARSlipHeaderError.FixDate` を `9999/12/31` に更新する。
- `CMCAccountBook` / `UpInsAccountBookNew` は変更しない。

## 2. 修正ファイル一覧

| No | ファイル | クラス | 主な修正メソッド |
|---|---|---|---|
| 1 | `C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip\InputSalesSlip.Web\Sales\InputSalesSlip\SalesChangeDisposalSlipInput\SalesChangeDisposalSlipInputUpdate_partial.cs` | `SalesChangeDisposalSlipInputUpdate` | `SlipDeleateButton_Click` |
| 2 | `C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip\InputSalesSlip.Business\SalesChangeDisposalSlipEntryNewBusiness.cs` | `SalesChangeDisposalSlipEntryBusiness` | `UpInsSlipNew`, `UpInsSlipErrorExecuteNew` |
| 3 | `C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip\InputSalesSlip.Business\SalesChangeDisposalSlipEntryNewDataAccessComponent.cs` | `SalesChangeDisposalSlipEntryDataAccessComponent` | `UpdateSlipRevisionFlgSARSlipHeaderNew`, `UpdateFixDateSARSlipHeaderErrorNew`, 赤伝 INSERT SQL 作成メソッド |

## 3. 修正詳細

### 3.1 UI: `SalesChangeDisposalSlipInputUpdate_partial.cs`

クラス:

- `SalesChangeDisposalSlipInputUpdate`: 190行目

メソッド:

- `SlipDeleateButton_Click`: 1460行目

修正箇所:

- 1492行目: `SARSlipHeaderError` Dataset の `LogicalDelFlg` を `"0"` に維持。
- 1498行目: `SARSlipDetailError` Dataset の `LogicalDelFlg` を `"0"` に維持。

修正理由:

- 画面側 Dataset で `LogicalDelFlg = "1"` を立てると、後続処理が論理削除前提になるため。
- 仕様どおり、取消後も元伝票・エラートランを論理削除しないため。

### 3.2 Business: `SalesChangeDisposalSlipEntryNewBusiness.cs`

クラス:

- `SalesChangeDisposalSlipEntryBusiness`: 116行目

#### `UpInsSlipNew`

メソッド開始:

- 1957行目

修正箇所:

- 2197行目: 取消時に `UpInsSlipExecuteForDenyNew(workEntity, dsEntity)` を呼び、赤伝を作成。
- 2202行目: `UpdateSlipRevisionFlgSARSlipHeaderNew(workEntity, dsEntity)` を呼び、元伝票と赤伝を訂正不可に更新。
- 2203行目: `UpdateFixDateSARSlipHeaderErrorNew(workEntity, dsEntity)` を呼び、エラーヘッダの `FixDate` を更新。

修正前:

- 取消時に `UpdateDelFlgSARSlipHeaderNew` / `UpdateDelFlgSARSlipDetailNew` を呼び、本伝票を論理削除していた。

修正後:

- 本伝票は論理削除しない。
- 赤伝を INSERT する。
- 赤伝作成後に `SARSlipHeader.SlipRevisionFlg = '1'` と `SARSlipHeaderError.FixDate = '9999/12/31'` を更新する。

#### `UpInsSlipErrorExecuteNew`

メソッド開始:

- 2431行目

修正箇所:

- 2458行目: 取消時はエラートランを論理削除しないコメントを追加。
- 2459行目: `FixDate` 更新は赤伝 INSERT 後に `UpInsSlipNew` から実行する方針を明記。

修正前:

- 取消時に `UpdateDelFlgSARSlipHeaderErrorNew` / `UpdateDelFlgSARSlipDetailErrorNew` を呼び、エラートランを論理削除していた。

修正後:

- 取消時のエラートラン論理削除処理を実行しない。
- 通常更新時の `UpInsSARSlipHeaderErrorNew` / `UpInsSARSlipDetailErrorNew` は既存通り。

### 3.3 DAO: `SalesChangeDisposalSlipEntryNewDataAccessComponent.cs`

クラス:

- `SalesChangeDisposalSlipEntryDataAccessComponent`: 59行目

#### 追加メソッド: `UpdateSlipRevisionFlgSARSlipHeaderNew`

メソッド開始:

- 1360行目

主な SQL:

- 1376行目: `SlipRevisionFlg = '1'`
- 1386行目: `LogicalDelFlg = '0'` を条件に指定

目的:

- 同一伝票番号・同一店舗・同一売変廃棄日の元伝票と赤伝を、検索可能なまま訂正不可にする。
- `SlipBranchNo` は条件に入れない。

#### 追加メソッド: `UpdateFixDateSARSlipHeaderErrorNew`

メソッド開始:

- 1427行目

主な SQL:

- 1443行目: `FixDate = '9999/12/31'`

目的:

- エラーヘッダ側でも訂正不可状態を表現する。
- `LogicalDelFlg` / `LogicalDelDate` は更新しない。

#### 赤伝ヘッダ INSERT SQL

対象メソッド:

- `CreateInsertSARSlipHeaderForDenyQueryTextNew`

修正箇所:

- 3270行目: `TotalQty` を `-1 * TotalQty` に変更。
- 3274行目: `SlipTotalDisposalAmt` を `-1 * SlipTotalDisposalAmt` に変更。
- 3282行目: `SlipRevisionFlg` の値を `'1'` に変更。
- 3283行目: `RedBlackDiv` は既存通り `'1'`。
- 3288行目: `LogicalDelFlg` の値を `'0'` に変更。

注意:

- INSERT の列リスト側は列名のまま維持。
- SELECT 値リスト側だけを変更。

#### 赤伝明細 INSERT SQL

対象メソッド:

- `CreateInsertSARSlipDetailForDenyFillQueryNew`

修正箇所:

- 3369行目: `DisposalChangeQty` を `-1 * DisposalChangeQty` に変更。
- 3376行目: `DisposalPriceAmt` を `-1 * DisposalPriceAmt` に変更。
- 3387行目: `LogicalDelFlg` の値を `'0'` に変更。

注意:

- INSERT の列リスト側は列名のまま維持。
- SELECT 値リスト側だけを変更。

#### 追加 ParameterList メソッド

追加メソッド:

- 4464行目: `CreateUpdateSlipRevisionFlgSARSlipHeaderParameterListNew`
- 4493行目: `CreateUpdateFixDateSARSlipHeaderErrorParameterListNew`

使用パラメータ:

- `@CorporationCode`
- `@SlipDiv`
- `@SlipTypeCd`
- `@SlipNo`
- `@DisposalChangeDate`
- `@StoreCd`

## 4. 変更しなかった箇所

- `SalesChangeDisposalSlipEntryFacade.DelSARSlipNew`
  - Facade の API / 引数 / 戻り値は変更しない方針のため。
- `SalesChangeDisposalSlipEntryBusiness.UpInsAccountBookNew`
  - 大福帳 `CMCAccountBook.DataID = 'SAR003'` の登録・更新動作を既存通り維持するため。
- `UpInsSlipExecuteForDenyNew`
  - 既存の赤伝作成フローを再利用するため、呼び出し元だけ変更。
- 二重取消防止
  - 今回の実装対象外。

## 5. 静的確認結果

確認済み:

- `SlipDeleateButton_Click` の対象 Dataset 更新は `LogicalDelFlg = "0"`。
- `UpInsSlipNew` の取消分岐では `UpdateDelFlgSARSlipHeaderNew` / `UpdateDelFlgSARSlipDetailNew` を呼ばない。
- `UpInsSlipErrorExecuteNew` の取消分岐では `UpdateDelFlgSARSlipHeaderErrorNew` / `UpdateDelFlgSARSlipDetailErrorNew` を呼ばない。
- 赤伝 INSERT SQL は INSERT 列リストではなく SELECT 値リスト側だけを変更。
- 追加 DAO メソッドと ParameterList メソッドは 1 回ずつ存在。
- 追加コード内に不正な `\"` エスケープが残っていない。

未実施:

- MSBuild によるビルド確認。

未実施理由:

- 作業環境の PATH 上に `msbuild` が存在しなかったため。
- 対象は .NET Framework 4.8 の標準版実ソースであり、`dotnet build` ではなく Visual Studio / MSBuild 環境での確認が必要。

## 6. 影響範囲

- 売変・廃棄伝票取消処理。
- `SARSlipHeader`
- `SARSlipDetail`
- `SARSlipHeaderError`
- `SARSlipDetailError`
- 伝票検索画面 `SCRCBYR040205` の検索結果および訂正可否。
- 大福帳連携処理は既存動作維持の前提。

## 7. 想定リスク

- `UpdateSlipRevisionFlgSARSlipHeaderNew` は `SlipBranchNo` を条件に入れないため、同一キー配下の `LogicalDelFlg = '0'` のヘッダ全件が訂正不可になる。これは仕様通りだが、想定外の枝番データがある場合は影響確認が必要。
- `SARSlipDetailError` は取消時に更新しないため、既存画面やバッチが `SARSlipDetailError` の削除状態を前提にしていないか確認が必要。
- ビルド未実施のため、標準版の Visual Studio / MSBuild 環境でコンパイル確認が必要。

## 8. 必要な追加テスト

- `Redmine_8842_テスト仕様書.md` に沿った DB 確認。
- 元伝票の `LogicalDelFlg = '0'` 維持確認。
- 赤伝ヘッダ・明細の追加確認。
- 赤伝の対象数量・金額項目のマイナス化確認。
- 元伝票と赤伝の `SlipRevisionFlg = '1'` 確認。
- `SARSlipHeaderError.FixDate = '9999/12/31'` 確認。
- 伝票検索画面 `SCRCBYR040205` で元伝票と赤伝が検索でき、修正ボタンが表示されないこと。
- 承認済み伝票取消時、`CMCAccountBook.DataID = 'SAR003'` の登録・更新動作が既存通りであること。
