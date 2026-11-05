# Redmine 8842 新規ユニットテスト作成報告書

## 1. 作成概要

既存の以下フォルダーは使用せず、新しい Redmine 8842 専用テストプロジェクトを作成した。

- `Sales.PublicBusiness.BusinessTest`
- `InputSalesSlip.BusinessTest`

新規作成先:

```text
C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip\Redmine8842.UnitTests
```

## 2. 技術構成

既存ソース `InputSalesSlip.Business.csproj` に合わせ、以下の形式で作成した。

| 項目 | 内容 |
|---|---|
| TargetFramework | `.NET Framework 4.8` |
| Project形式 | 旧 csproj / `ToolsVersion="12.0"` |
| OutputType | `Exe` |
| テスト方式 | コンソール型テストランナー |
| MSTest依存 | なし |
| Excel ODBC依存 | なし |
| NuGet依存 | なし |

既存 MSTest / Excel DataSource の問題を避けるため、テストランナーは外部テストフレームワークに依存しない形にした。

## 3. 作成ファイル

| ファイル | 内容 |
|---|---|
| `Redmine8842.UnitTests.csproj` | .NET Framework 4.8 のテストプロジェクト定義 |
| `Program.cs` | テスト起動処理、ソースルート解決 |
| `TestRunner.cs` | シンプルな PASS / FAIL 実行ランナー |
| `SourceRegressionTests.cs` | Redmine 8842 仕様に基づくテスト本体。UI ソース回帰テストを含む |
| `Redmine8842TestData.cs` | `Redmine_8842_テストデータ.md` に基づく期待値定義 |
| `Properties\AssemblyInfo.cs` | アセンブリ情報 |
| `README.md` | 実行方法と注意事項 |

## 4. 実装したテスト

| テスト仕様書TC | テスト内容 | 実装メソッド |
|---|---|---|
| TC-01 | 未保存伝票の取消時に `ERRCSARB0273` を表示し、取消ファサードを呼ばない | `UiCancelButtonRejectsUnsavedSlip` |
| TC-02 / TC-03 | UI 取消ボタン押下時にエラートラン・本伝票の論理削除フラグを `'0'` のままにし、取消モードを `On` にする | `UiCancelButtonSetsCancelModeAndPreservesLogicalDeleteOff` |
| TC-02 / TC-03 | 取消時にエラートラン・本伝票を論理削除しない | `BusinessCancelFlowCreatesRedSlipAndDoesNotDeleteOriginal`, `BusinessErrorCancelFlowDoesNotDeleteErrorTran` |
| TC-04 / TC-05 | `SARSlipHeader` 赤伝作成SQL、数量・金額マイナス化 | `HeaderRedSlipSqlMatchesSpec` |
| TC-06 / TC-07 | `SARSlipDetail` 赤伝明細作成SQL、数量・金額マイナス化 | `DetailRedSlipSqlMatchesSpec` |
| TC-08 | 元伝票 + 赤伝の対象項目合計が0 | `TestDataOffsetsAreCorrect` |
| TC-09 | `SARSlipHeader.SlipRevisionFlg = '1'` 更新SQL | `SlipRevisionUpdateSqlMatchesSpec` |
| TC-10 | `SARSlipHeaderError.FixDate = '9999/12/31'` 更新SQL | `FixDateUpdateSqlMatchesSpec` |
| TC-11 / TC-12 | 取消後の元伝票・赤伝が一覧検索対象に残り、訂正不可行の修正ボタンを非表示にする | `UiSearchResultKeepsCanceledSlipsVisibleAndHidesCorrectionButton` |
| TC-14 | 確認画面ありの場合、`ButtonId = "5"` を設定して `SCRCSAR050208` へ遷移し、DB更新系ファサードを即時実行しない | `UiConfirmFlowRedirectsWithoutExecutingDelete` |
| TC-15 | 確認画面なしの場合、`DelSARSlipNew` を即時実行し、完了メッセージ `MSGCSARI0028` を表示する | `UiDirectFlowExecutesDeleteFacadeAndShowsCompletion` |
| TC-16 / TC-17 | 大福帳フローを直接変更していないこと | `AccountBookFlowIsNotDirectlyChanged` |
| 共通 | 修正対象メソッドのブレース構造確認 | `ModifiedMethodsHaveBalancedBraces` |

## 4.1 再実行前の修正内容

初回実行時、`UpInsSlipNew` 内のコメントアウト済み旧コードに含まれる `{}` の影響で、テストコード側のメソッド抽出処理が途中で終了し、1件失敗した。

対応内容:

- `SourceRegressionTests.cs` の `ExtractMethod` / `ExtractBlock` を修正。
- `//` コメント、`/* */` コメント、文字列リテラル、文字リテラル内の `{}` を無視する `FindMatchingBrace` を追加。
- さらに、古いソース構造に対する保険として、次のメソッド宣言までを抽出する `FindNextMemberStart` フォールバックを追加。
- 実ソース側の Redmine 8842 修正内容は変更していない。

## 5. UIテストの扱い

今回の再実行では、UI 操作そのものをブラウザで行うテストではなく、UI ソースの回帰テストとして以下を自動テストに含めた。

- TC-01
- TC-11
- TC-12
- TC-14
- TC-15

確認対象は、取消ボタン処理、確認画面分岐、直接実行分岐、一覧の修正ボタン表示制御である。
実画面でのクリック操作、画面表示、DB反映の結合確認は、`Redmine_8842_テスト仕様書.md` の確認SQLと画面操作で別途実施する。

## 6. 実行方法

.NET Framework 4.8 Developer Pack / Targeting Pack と MSBuild がある環境で実行する。

```powershell
cd "C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip\Redmine8842.UnitTests"
MSBuild.exe Redmine8842.UnitTests.csproj /p:Configuration=Debug
.\bin\Debug\Redmine8842.UnitTests.exe
```

ソースルートを明示する場合:

```powershell
.\bin\Debug\Redmine8842.UnitTests.exe "C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip"
```

環境変数を使う場合:

```powershell
$env:REDMINE8842_INPUTSALESSLIP_ROOT = "C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip"
.\bin\Debug\Redmine8842.UnitTests.exe
```

## 7. 現環境でのビルド・テスト確認結果

### 7.1 .NET Framework 4.8 Targeting Pack 確認

以下のパスが存在することを確認した。

```text
C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8
C:\Program Files (x86)\Windows Kits\NETFXSDK\4.8
```

このため、現在の環境では `.NET Framework 4.8` の参照アセンブリを使用できる。

### 7.2 ビルド確認

実行コマンド:

```powershell
cd "C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip\Redmine8842.UnitTests"
dotnet build ".\Redmine8842.UnitTests.csproj" --no-restore
```

結果:

```text
Redmine8842.UnitTests -> C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip\Redmine8842.UnitTests\bin\Debug\Redmine8842.UnitTests.exe

ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:00.70
```

判定:

- テストコード作成: 完了
- 現環境でのビルド: 成功

### 7.3 テスト実行結果

実行コマンド:

```powershell
.\bin\Debug\Redmine8842.UnitTests.exe "C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip"
```

#### 7.3.1 修正実行前の全体証跡

UI ソース回帰テスト追加前の実行結果:

```text
Redmine 8842 Unit Tests
SourceRoot: C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip

[PASS] TC-02/TC-03 Business cancel flow creates red slip and does not call logical delete
[PASS] TC-02 Business error cancel flow does not call error logical delete
[PASS] TC-04/TC-05 SARSlipHeader red slip SQL follows Redmine 8842
[PASS] TC-06/TC-07 SARSlipDetail red slip SQL follows Redmine 8842
[PASS] TC-09 SARSlipHeader SlipRevision update SQL exists
[PASS] TC-10 SARSlipHeaderError FixDate update SQL exists
[PASS] TC-16/TC-17 AccountBook flow is not directly changed by Redmine 8842
[PASS] Test data TD-02 red slip values offset original values
[PASS] Modified source methods have balanced braces

Passed: 9
Failed: 0
```

#### 7.3.2 修正実行後の全体証跡

UI ソース回帰テスト追加後のビルド結果:

```text
Redmine8842.UnitTests -> C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip\Redmine8842.UnitTests\bin\Debug\Redmine8842.UnitTests.exe

ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:02.37
```

UI ソース回帰テスト追加後の実行結果:

```text
Redmine 8842 Unit Tests
SourceRoot: C:\CoreCenterRetail\【標準版】Branch2.2\Retail\Unisys\Sales\InputSalesSlip

[PASS] TC-01 UI cancel button rejects unsaved slip
[PASS] TC-02/TC-03 UI cancel button keeps logical delete flags off
[PASS] TC-14 UI confirm flow redirects without executing delete
[PASS] TC-15 UI direct flow executes delete facade and shows completion
[PASS] TC-11/TC-12 UI search result keeps canceled slips visible and hides correction button
[PASS] TC-02/TC-03 Business cancel flow creates red slip and does not call logical delete
[PASS] TC-02 Business error cancel flow does not call error logical delete
[PASS] TC-04/TC-05 SARSlipHeader red slip SQL follows Redmine 8842
[PASS] TC-06/TC-07 SARSlipDetail red slip SQL follows Redmine 8842
[PASS] TC-09 SARSlipHeader SlipRevision update SQL exists
[PASS] TC-10 SARSlipHeaderError FixDate update SQL exists
[PASS] TC-16/TC-17 AccountBook flow is not directly changed by Redmine 8842
[PASS] Test data TD-02 red slip values offset original values
[PASS] Modified source methods have balanced braces

Passed: 14
Failed: 0
```

#### 7.3.3 テストケース別の前後証跡

| テストケース | 修正実行前の証跡 | 修正実行後の証跡 | 判定 |
|---|---|---|---|
| TC-01 | UIテスト未登録。修正実行前の出力に該当PASS行なし | `[PASS] TC-01 UI cancel button rejects unsaved slip` | PASS |
| TC-02 UI | UIテスト未登録。修正実行前の出力に該当PASS行なし | `[PASS] TC-02/TC-03 UI cancel button keeps logical delete flags off` | PASS |
| TC-02 Business | `[PASS] TC-02/TC-03 Business cancel flow creates red slip and does not call logical delete` / `[PASS] TC-02 Business error cancel flow does not call error logical delete` | `[PASS] TC-02/TC-03 Business cancel flow creates red slip and does not call logical delete` / `[PASS] TC-02 Business error cancel flow does not call error logical delete` | PASS |
| TC-03 UI | UIテスト未登録。修正実行前の出力に該当PASS行なし | `[PASS] TC-02/TC-03 UI cancel button keeps logical delete flags off` | PASS |
| TC-03 Business | `[PASS] TC-02/TC-03 Business cancel flow creates red slip and does not call logical delete` | `[PASS] TC-02/TC-03 Business cancel flow creates red slip and does not call logical delete` | PASS |
| TC-04 | `[PASS] TC-04/TC-05 SARSlipHeader red slip SQL follows Redmine 8842` | `[PASS] TC-04/TC-05 SARSlipHeader red slip SQL follows Redmine 8842` | PASS |
| TC-05 | `[PASS] TC-04/TC-05 SARSlipHeader red slip SQL follows Redmine 8842` | `[PASS] TC-04/TC-05 SARSlipHeader red slip SQL follows Redmine 8842` | PASS |
| TC-06 | `[PASS] TC-06/TC-07 SARSlipDetail red slip SQL follows Redmine 8842` | `[PASS] TC-06/TC-07 SARSlipDetail red slip SQL follows Redmine 8842` | PASS |
| TC-07 | `[PASS] TC-06/TC-07 SARSlipDetail red slip SQL follows Redmine 8842` | `[PASS] TC-06/TC-07 SARSlipDetail red slip SQL follows Redmine 8842` | PASS |
| TC-08 | `[PASS] Test data TD-02 red slip values offset original values` | `[PASS] Test data TD-02 red slip values offset original values` | PASS |
| TC-09 | `[PASS] TC-09 SARSlipHeader SlipRevision update SQL exists` | `[PASS] TC-09 SARSlipHeader SlipRevision update SQL exists` | PASS |
| TC-10 | `[PASS] TC-10 SARSlipHeaderError FixDate update SQL exists` | `[PASS] TC-10 SARSlipHeaderError FixDate update SQL exists` | PASS |
| TC-11 | UIテスト未登録。修正実行前の出力に該当PASS行なし | `[PASS] TC-11/TC-12 UI search result keeps canceled slips visible and hides correction button` | PASS |
| TC-12 | UIテスト未登録。修正実行前の出力に該当PASS行なし | `[PASS] TC-11/TC-12 UI search result keeps canceled slips visible and hides correction button` | PASS |
| TC-14 | UIテスト未登録。修正実行前の出力に該当PASS行なし | `[PASS] TC-14 UI confirm flow redirects without executing delete` | PASS |
| TC-15 | UIテスト未登録。修正実行前の出力に該当PASS行なし | `[PASS] TC-15 UI direct flow executes delete facade and shows completion` | PASS |
| TC-16 | `[PASS] TC-16/TC-17 AccountBook flow is not directly changed by Redmine 8842` | `[PASS] TC-16/TC-17 AccountBook flow is not directly changed by Redmine 8842` | PASS |
| TC-17 | `[PASS] TC-16/TC-17 AccountBook flow is not directly changed by Redmine 8842` | `[PASS] TC-16/TC-17 AccountBook flow is not directly changed by Redmine 8842` | PASS |
| 共通 | `[PASS] Modified source methods have balanced braces` | `[PASS] Modified source methods have balanced braces` | PASS |

判定:

- ユニットテスト実行: 成功
- UIに関するテストケースを含めたユニットテスト実行: 成功
- 修正実行前 PASS: 9
- 修正実行前 FAIL: 0
- 修正実行後 PASS: 14
- 修正実行後 FAIL: 0
- 初回の `8 PASS / 1 FAIL` は、テストコード側のメソッド抽出ロジック修正により解消済み。

## 8. 補足

このテストプロジェクトは DB 更新を行わない。

DBを使う結合確認は、以下を使用して別途実施する。

- `Redmine_8842_テスト仕様書.md`
- `Redmine_8842_テストデータ.md`
- 各TCの確認SQL
