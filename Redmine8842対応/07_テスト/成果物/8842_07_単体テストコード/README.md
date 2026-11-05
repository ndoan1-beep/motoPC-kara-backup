# Redmine8842.UnitTests

Redmine 8842 の売変・廃棄伝票取消仕様変更を確認するための新規テストプロジェクトです。
既存の `InputSalesSlip.BusinessTest` / `Sales.PublicBusiness.BusinessTest` フォルダーは使用しません。

## 形式

- .NET Framework 4.8
- 旧 csproj 形式
- MSTest / Excel ODBC / NuGet 依存なし
- コンソール型テストランナー

## テスト範囲

`Redmine_8842_テスト仕様書.md` と `Redmine_8842_テストデータ.md` に基づき、DB 更新を行わないソース回帰テストとして確認します。
UI 操作そのものは行わず、UI コードの取消ボタン処理・確認画面分岐・一覧の修正ボタン表示制御をソースレベルで確認します。

主な確認内容:

- 取消時に本伝票・エラートランを論理削除しないこと
- UI の取消ボタン押下時、未保存伝票では `ERRCSARB0273` を表示し処理を中断すること
- UI の確認画面あり設定では `SCRCSAR050208` へ遷移し、取消ファサードを即時実行しないこと
- UI の確認画面なし設定では `DelSARSlipNew` を実行し、完了メッセージ `MSGCSARI0028` を表示すること
- 画面一覧では取消後の元伝票・赤伝を検索対象に残し、訂正不可行の修正ボタンを非表示にすること
- 取消時に赤伝作成フローへ進むこと
- 赤伝ヘッダ SQL で `TotalQty` / `SlipTotalDisposalAmt` がマイナス化されること
- 赤伝明細 SQL で `DisposalChangeQty` / `DisposalPriceAmt` がマイナス化されること
- `SARSlipHeader.SlipRevisionFlg = '1'` 更新 SQL が存在すること
- `SARSlipHeaderError.FixDate = '9999/12/31'` 更新 SQL が存在すること
- `CMCAccountBook.DataID = '003'` の既存フローを直接変更していないこと
- テストデータ上、元伝票と赤伝の対象項目が相殺されること

## 実行方法

Visual Studio / Build Tools の .NET Framework 4.8 Developer Pack がある環境で以下を実行します。

```powershell
MSBuild.exe Redmine8842.UnitTests.csproj /p:Configuration=Debug
.\bin\Debug\Redmine8842.UnitTests.exe
```

入力ルートを明示する場合:

```powershell
.\bin\Debug\Redmine8842.UnitTests.exe "C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip"
```

または環境変数を使用します。

```powershell
$env:REDMINE8842_INPUTSALESSLIP_ROOT = "C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip"
```

## 注意

このプロジェクトは DB を更新しません。DB を使う結合確認は `Redmine_8842_テスト仕様書.md` の確認 SQL と `Redmine_8842_テストデータ.md` を使用して別途実施します。
