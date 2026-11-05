# Redmine 8842 コード修正プラン

## 1. 目的

`Redmine_8842_変更仕様書.md` に基づき、「売変・廃棄の伝票取消」時の動作を以下へ変更する。

1. 元伝票を論理削除しない。
2. `SARSlipHeader` / `SARSlipDetail` に同一伝票番号の新規赤伝を登録する。
3. 変更仕様書 #4 のSQL例で指定された数量・金額項目だけを赤伝INSERT時にマイナス化する。
4. 伝票検索画面 `SCRCBYR040205` で元伝票と新規赤伝を検索可能にする。
5. 検索可能だが、元伝票と新規赤伝のどちらも「修正」ボタンを表示しない。
6. 大福帳 `CMCAccountBook` の `DataID = 'SAR003'` 登録・更新動作は既存仕様から変えない。

## 2. 対象ソース

MCPサーバーの `query_semantic_graph` / `read_source_file_range` で参照している【標準版】Branch2.2 のソースコードを確認した結果、修正対象は以下。

| 区分 | 対象 | ファイル | 修正要否 |
|---|---|---|---|
| UI | `SalesChangeDisposalSlipInputUpdate.SlipDeleateButton_Click` | `Sales\InputSalesSlip\InputSalesSlip.Web\Sales\InputSalesSlip\SalesChangeDisposalSlipInput\SalesChangeDisposalSlipInputUpdate_partial.cs` | 必要 |
| Facade | `SalesChangeDisposalSlipEntryFacade.DelSARSlipNew` | `Sales\InputSalesSlip\InputSalesSlip.Facade\SalesChangeDisposalSlipEntryNewFacade.cs` | 原則不要 |
| Business | `SalesChangeDisposalSlipEntryBusiness.UpInsSlipNew` | `Sales\InputSalesSlip\InputSalesSlip.Business\SalesChangeDisposalSlipEntryNewBusiness.cs` | 必要 |
| Business | `SalesChangeDisposalSlipEntryBusiness.UpInsSlipErrorExecuteNew` | 同上 | 必要 |
| Business | `SalesChangeDisposalSlipEntryBusiness.UpInsSlipExecuteForDenyNew` | 同上 | 原則不要 |
| Business | `SalesChangeDisposalSlipEntryBusiness.UpInsAccountBookNew` | 同上 | 変更禁止 |
| DAO | `SalesChangeDisposalSlipEntryDataAccessComponent` | `Sales\InputSalesSlip\InputSalesSlip.Business\SalesChangeDisposalSlipEntryNewDataAccessComponent.cs` | 必要 |

変更仕様書に記載されている以下のDAOメソッドは現行ソース上に存在しないため、新規追加する。

- `UpdateSlipRevisionFlgSARSlipHeaderNew`
- `UpdateFixDateSARSlipHeaderErrorNew`

## 3. 修正方針

### 3.1 UI: `SlipDeleateButton_Click`

現行は取消ボタン押下時に、Dataset上のエラートランへ `LogicalDelFlg = "1"` をセットしている。

修正前:

```csharp
foreach (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerRow in dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows)
{
    headerRow.LogicalDelFlg = "1";
}

foreach (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows)
{
    detailRow.LogicalDelFlg = "1";
}
```

修正後:

```csharp
foreach (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerRow in dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows)
{
    headerRow.LogicalDelFlg = "0";
}

foreach (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows)
{
    detailRow.LogicalDelFlg = "0";
}
```

補足:

- 未保存伝票チェック、確認画面遷移、`workEntity.CancelMode = Constants.OnFlag`、取消後一覧再表示は変更しない。
- Dataset上でも `LogicalDelFlg = "1"` を立てないことで、後続のBusiness、大福帳処理へ論理削除前提のデータを渡さない。

### 3.2 Facade: `DelSARSlipNew`

現行:

```csharp
workEntity.TranUpdateMode = "1";
slipEntryBusiness.UpInsSlipNew(workEntity, dsEntity, initEntity, coverSheetMasterEntity);
```

修正方針:

- Facadeは変更しない。
- `TranUpdateMode = "1"` と `UpInsSlipNew` 呼び出しは既存通り維持する。
- API仕様変更禁止のため、引数・戻り値・FacadeMethod IDは変更しない。

### 3.3 Business: `UpInsSlipErrorExecuteNew`

現行は取消モードでエラートランを論理削除している。

修正前:

```csharp
if (workEntity.CancelMode == Constants.OnFlag)
{
    dataAccessComponent.UpdateDelFlgSARSlipHeaderErrorNew(workEntity, dsEntity);
    dataAccessComponent.UpdateDelFlgSARSlipDetailErrorNew(workEntity, dsEntity);
}
else
{
    dataAccessComponent.UpInsSARSlipHeaderErrorNew(workEntity, dsEntity);
    dataAccessComponent.UpInsSARSlipDetailErrorNew(workEntity, dsEntity);
}
```

修正後:

```csharp
if (workEntity.CancelMode == Constants.OnFlag)
{
    // 取消時はエラートランを論理削除しない。
    // FixDate更新は赤伝INSERT後に UpInsSlipNew から実行する。
}
else
{
    dataAccessComponent.UpInsSARSlipHeaderErrorNew(workEntity, dsEntity);
    dataAccessComponent.UpInsSARSlipDetailErrorNew(workEntity, dsEntity);
}
```

補足:

- `SARSlipHeaderError` / `SARSlipDetailError` の `LogicalDelFlg` と `LogicalDelDate` は更新しない。
- `FixDate = '9999/12/31'` は変更仕様書の理想フローに合わせ、赤伝INSERT後に `UpdateFixDateSARSlipHeaderErrorNew` で更新する。

### 3.4 Business: `UpInsSlipNew`

現行は取消モードで本伝票を論理削除している。

修正前:

```csharp
if (workEntity.CancelMode == Constants.OnFlag)
{
    workEntity.SlipUpdateMode = slipUpdateMode_Red;

    using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
    {
        dataAccessComponent.UpdateDelFlgSARSlipHeaderNew(workEntity, dsEntity);
        dataAccessComponent.UpdateDelFlgSARSlipDetailNew(workEntity, dsEntity);
    }
}
else
{
    this.UpInsSlipExecuteNew(workEntity, dsEntity, initEntity);
}
```

修正後:

```csharp
if (workEntity.CancelMode == Constants.OnFlag)
{
    workEntity.SlipUpdateMode = slipUpdateMode_Red;

    // 伝票データ（赤伝）の作成
    this.UpInsSlipExecuteForDenyNew(workEntity, dsEntity);

    using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
    {
        dataAccessComponent.UpdateSlipRevisionFlgSARSlipHeaderNew(workEntity, dsEntity);
        dataAccessComponent.UpdateFixDateSARSlipHeaderErrorNew(workEntity, dsEntity);
    }
}
else
{
    this.UpInsSlipExecuteNew(workEntity, dsEntity, initEntity);
}
```

補足:

- `UpdateDelFlgSARSlipHeaderNew` / `UpdateDelFlgSARSlipDetailNew` は呼び出さない。
- 元伝票は黒伝として残し、`LogicalDelFlg = '0'` を維持する。
- 新規赤伝は既存の `UpInsSlipExecuteForDenyNew` を使って登録する。
- 訂正不可更新は、変更仕様書 #4 の理想フロー通り、赤伝INSERT後に実行する。
- `UpdateSlipRevisionFlgSARSlipHeaderNew` は元伝票と新規赤伝の両方を訂正不可にするため、同一伝票番号・同一店舗・同一日付の `SARSlipHeader` を対象に `SlipRevisionFlg = '1'` を更新する。
- 大福帳処理へ入る前に `workEntity.SlipUpdateMode = slipUpdateMode_Red` を維持する。`UpInsAccountBookNew` の内部ロジックは変更しない。

### 3.5 DAO: 赤伝ヘッダINSERT SQL

対象:

```csharp
CreateInsertSARSlipHeaderForDenyQueryTextNew()
```

修正前:

```sql
SELECT
    ...
    TotalQty,
    ...
    SlipTotalDisposalAmt,
    ...
    SlipRevisionFlg,
    '1',
    ...
    LogicalDelFlg,
    ...
FROM SARSlipHeader
```

修正後:

```sql
SELECT
    ...
    -1 * TotalQty,
    ...
    -1 * SlipTotalDisposalAmt,
    ...
    '1',
    '1',
    ...
    '0',
    ...
FROM SARSlipHeader
```

修正内容:

| 項目 | 修正前 | 修正後 |
|---|---|---|
| `TotalQty` | `TotalQty` | `-1 * TotalQty` |
| `SlipTotalDisposalAmt` | `SlipTotalDisposalAmt` | `-1 * SlipTotalDisposalAmt` |
| `SlipRevisionFlg` | `SlipRevisionFlg` | `'1'` |
| `RedBlackDiv` | `'1'` | `'1'` |
| `LogicalDelFlg` | `LogicalDelFlg` | `'0'` |

注意:

- 変更仕様書のSQL例で指定されていない金額・単価項目は反転しない。
- `SlipOldTotalPrice` / `SlipNewTotalPrice` はコピーのままにする。

### 3.6 DAO: 赤伝明細INSERT SQL

対象:

```csharp
CreateInsertSARSlipDetailForDenyFillQueryNew()
```

修正前:

```sql
SELECT
    ...
    DisposalChangeQty,
    ...
    DisposalPriceAmt,
    ...
    LogicalDelFlg,
    ...
FROM SARSlipDetail
```

修正後:

```sql
SELECT
    ...
    -1 * DisposalChangeQty,
    ...
    -1 * DisposalPriceAmt,
    ...
    '0',
    ...
FROM SARSlipDetail
```

修正内容:

| 項目 | 修正前 | 修正後 |
|---|---|---|
| `DisposalChangeQty` | `DisposalChangeQty` | `-1 * DisposalChangeQty` |
| `DisposalPriceAmt` | `DisposalPriceAmt` | `-1 * DisposalPriceAmt` |
| `LogicalDelFlg` | `LogicalDelFlg` | `'0'` |

注意:

- `SlipOldPrice` / `SlipNewPrice` / `SlipDisposalUnitPrica` / `SalesChangePrice` はコピーのままにする。

### 3.7 DAO: `UpdateSlipRevisionFlgSARSlipHeaderNew` 追加

目的:

- 元伝票と新規赤伝を伝票検索画面で検索可能にしたまま、訂正選択不可にする。

追加メソッド:

```csharp
public virtual void UpdateSlipRevisionFlgSARSlipHeaderNew(WorkEntity workEntity, DsEntity dsEntity)
```

実行SQL:

```sql
UPDATE SARSlipHeader
   SET SlipRevisionFlg = '1',
       UpdateBy = @UpdateBy,
       UpdateDate = @UpdateDate
 WHERE CorporationCode = @CorporationCode
   AND SlipDiv = @SlipDiv
   AND SlipTypeCd = @SlipTypeCd
   AND SlipNo = @SlipNo
   AND DisposalChangeDate = @DisposalChangeDate
   AND StoreCd = @StoreCd
   AND LogicalDelFlg = '0'
```

パラメータ:

- `CorporationCode`
- `SlipDiv`
- `SlipTypeCd`
- `SlipNo`
- `DisposalChangeDate`
- `StoreCd`

補足:

- `SlipBranchNo` は条件に入れない。元伝票と新規赤伝の両方に `SlipRevisionFlg = '1'` を付けるため。
- 共通更新項目は既存 `SetCommonItemForUpdateNew` を使う。

### 3.8 DAO: `UpdateFixDateSARSlipHeaderErrorNew` 追加

目的:

- `SARSlipHeaderError.FixDate = '9999/12/31'` に更新し、エラートラン側でも訂正不可状態を表現する。

追加メソッド:

```csharp
public virtual void UpdateFixDateSARSlipHeaderErrorNew(WorkEntity workEntity, DsEntity dsEntity)
```

実行SQL:

```sql
UPDATE SARSlipHeaderError
   SET FixDate = '9999/12/31',
       UpdateBy = @UpdateBy,
       UpdateDate = @UpdateDate
 WHERE CorporationCode = @CorporationCode
   AND SlipDiv = @SlipDiv
   AND SlipTypeCd = @SlipTypeCd
   AND SlipNo = @SlipNo
   AND DisposalChangeDate = @DisposalChangeDate
   AND StoreCd = @StoreCd
```

パラメータ:

- `CorporationCode`
- `SlipDiv`
- `SlipTypeCd`
- `SlipNo`
- `DisposalChangeDate`
- `StoreCd`

補足:

- `UpdateDelFlgSARSlipHeaderErrorNew` と同じ検索条件を使用する。
- `LogicalDelFlg` / `LogicalDelDate` は更新しない。
- `SARSlipDetailError` は更新しない。
- 共通更新項目は既存 `SetCommonItemForUpdateNew` を使う。

## 4. 修正しない箇所

| 対象 | 理由 |
|---|---|
| DBスキーマ | 変更仕様書で変更禁止。 |
| Stored Procedure | 今回対象はC#内のInline SQL。 |
| Facadeシグネチャ / API仕様 | 変更仕様書で変更禁止。 |
| `UpInsSlipExecuteForDenyNew` | 既存の赤伝作成呼び出し口を再利用する。 |
| `UpInsAccountBookNew` | `CMCAccountBook.DataID = 'SAR003'` の登録・更新動作を変えないため。 |
| 二重取消防止 | 今回対象外。 |

## 5. 修正後フロー

```text
SalesChangeDisposalSlipInputUpdate_partial.SlipDeleateButton_Click
  -> DatasetのLogicalDelFlgを'1'にしない
  -> workEntity.CancelMode = Constants.OnFlag
  -> SalesChangeDisposalSlipEntryFacade.DelSARSlipNew
  -> SalesChangeDisposalSlipEntryBusiness.UpInsSlipNew
  -> UpInsSlipErrorNew / UpInsSlipErrorExecuteNew
       -> 取消時はエラートラン論理削除を行わない
  -> UpInsSlipExecuteForDenyNew
       -> InsertSARSlipHeaderForDenyNew
       -> InsertSARSlipDetailForDenyNew
  -> UpdateSlipRevisionFlgSARSlipHeaderNew
       -> SARSlipHeader.SlipRevisionFlg = '1'
  -> UpdateFixDateSARSlipHeaderErrorNew
       -> SARSlipHeaderError.FixDate = '9999/12/31'
  -> UpInsAccountBookNew
       -> 既存の大福帳登録・更新動作を維持
```

期待結果:

- `SARSlipHeader`, `SARSlipDetail`, `SARSlipHeaderError`, `SARSlipDetailError` の `LogicalDelFlg` は `'1'` にならない。
- 元伝票は黒伝として残る。
- 新規赤伝が同一伝票番号・新しい枝番で登録される。
- 元伝票と赤伝は `SCRCBYR040205` で検索できる。
- 元伝票と赤伝は訂正できない。
- `CMCAccountBook.DataID = 'SAR003'` は既存通り登録・更新される。

## 6. テスト観点

`Redmine_8842_テスト仕様書.md` に沿って確認する。

1. 未保存伝票取消時、`ERRCSARB0273` が表示されDB更新しない。
2. `SARSlipHeaderError.LogicalDelFlg = '0'` のまま。
3. `SARSlipDetailError.LogicalDelFlg = '0'` のまま。
4. 元伝票 `SARSlipHeader.LogicalDelFlg = '0'` のまま。
5. 元明細 `SARSlipDetail.LogicalDelFlg = '0'` のまま。
6. 赤伝ヘッダが追加され、`RedBlackDiv = '1'`, `LogicalDelFlg = '0'`, `SlipRevisionFlg = '1'` になる。
7. 赤伝ヘッダの `TotalQty` と `SlipTotalDisposalAmt` がマイナスになる。
8. 赤伝明細の `DisposalChangeQty` と `DisposalPriceAmt` がマイナスになる。
9. 元伝票 + 赤伝の指定項目合計が0になる。
10. `SARSlipHeader.SlipRevisionFlg = '1'` になる。
11. `SARSlipHeaderError.FixDate = '9999/12/31'` になる。
12. `SCRCBYR040205` で元伝票と赤伝が検索できる。
13. 元伝票と赤伝の「修正」ボタンが表示されない。
14. 承認済み伝票取消時、大福帳 `CMCAccountBook.DataID = 'SAR003'` の登録・更新動作が既存仕様から変わらない。

## 7. 実装順序

1. `SlipDeleateButton_Click` のDataset `LogicalDelFlg = "1"` セットを `"0"` 維持に変更する。
2. `UpInsSlipErrorExecuteNew` の取消分岐から `UpdateDelFlgSARSlipHeaderErrorNew` / `UpdateDelFlgSARSlipDetailErrorNew` 呼び出しを外す。
3. `UpInsSlipNew` の取消分岐から `UpdateDelFlgSARSlipHeaderNew` / `UpdateDelFlgSARSlipDetailNew` 呼び出しを外す。
4. `UpInsSlipNew` の取消分岐で `UpInsSlipExecuteForDenyNew(workEntity, dsEntity)` を呼ぶ。
5. DAOに `UpdateSlipRevisionFlgSARSlipHeaderNew` を追加する。
6. DAOに `UpdateFixDateSARSlipHeaderErrorNew` を追加する。
7. `UpInsSlipNew` の赤伝INSERT後に上記2メソッドを呼ぶ。
8. `CreateInsertSARSlipHeaderForDenyQueryTextNew` のSELECT句を修正する。
9. `CreateInsertSARSlipDetailForDenyFillQueryNew` のSELECT句を修正する。
10. `UpInsAccountBookNew` に差分が出ていないことを確認する。
11. テスト仕様書のSQLでDB結果を確認する。

## 8. リスクと注意点

- `DataID` は変更仕様書に合わせて `SAR003` と扱う。実装上は既存の `CodeDivisionUtil.GetCodeDivisionValueName(CodeDivisionKey.DataId, "03", ...)` の戻り値を使い、固定文字列の追加は避ける。
- `UpdateSlipRevisionFlgSARSlipHeaderNew` は `SlipBranchNo` を条件に入れない。元伝票と赤伝の両方を訂正不可にするため。
- 赤伝INSERT SQLでマイナス化する項目は、仕様書 #4 にある `TotalQty`, `SlipTotalDisposalAmt`, `DisposalChangeQty`, `DisposalPriceAmt` のみに限定する。
- `LogicalDelFlg` はDataset、エラートラン、本伝票、赤伝のどこでも `'1'` にしない。
- `SARSlipDetailError` は取消時に更新しない。論理削除もFixDate更新も行わない。
- 二重取消防止は今回実装しない。
