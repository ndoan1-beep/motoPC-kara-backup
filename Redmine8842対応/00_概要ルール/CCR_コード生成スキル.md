---
name: ccr-dotnet-code-generation
description: CCR既存コードベース向けに、.NET Framework / ASP.NET WebForms / Facade / Business / DataAccessComponent / Typed DataSet の既存作法を守ってC#コードを生成・修正するためのスキル。
---

# CCR .NETコード生成スキル

CCR既存コードベースに合わせて、C# / .NET Framework 4.x のコードを生成・修正するための実装ルールです。

このスキルは一般的な最新.NETのベストプラクティスではなく、CCRの既存構造、命名、コメント、DAO、Typed DataSet、SQL作成方式を優先します。

## When to Activate

- CCRのC#コードを新規作成する場合
- 既存のFacade / Business / DataAccessComponentを修正する場合
- ASP.NET WebForms画面、検索結果画面、入力画面のコードを修正する場合
- Typed DataSet / TableAdapter / DataAccessComponent / Inline SQLを扱う場合
- Redmine対応など、既存業務処理へ小さく安全に差分を入れる場合
- MCP `dotnet_semantic_index` で既存コードを調査し、その作法に合わせてコード生成する場合

## MCP調査ルール

コード生成前に、可能な限りMCPサーバー `dotnet_semantic_index` の検索・読み取り系ツールで既存実装を確認します。

- `query_semantic_graph` で対象機能、クラス、メソッド、周辺依存を検索する。
- `read_source_file_range` で対象メソッドと近接する既存実装を読む。
- `query_database_architecture` で対象テーブル、共通項目、キー、論理削除項目を確認する。
- ユーザーが明示的に禁止した場合、`refresh_codebase` は実行しない。
- MCPで取得できない情報は、`rg` 等のローカル検索で補助してよいが、推測は「未確認」として扱う。

```text
Good:
1. query_semantic_graph("SalesChangeDisposal")
2. read_source_file_range(対象Business / DataAccessComponent)
3. query_database_architecture("SARSlip")
4. 既存の呼出順、命名、SQL作成方式に合わせて最小修正する。

Bad:
1. 既存調査なしで新しい設計を作る。
2. refresh_codebaseを勝手に実行する。
3. EF Core、ASP.NET Core、DIコンテナ前提のコードへ置き換える。
```

## Core Principles

### 1. 既存アーキテクチャを最優先する

CCRでは、以下の層構造を崩さない。

```text
UI / ASP.NET WebForms
 ↓
Facade
 ↓
Business
 ↓
DataAccessComponent / TableAdapter
 ↓
Inline SQL / DbCommand / SqlDataAdapter
 ↓
SQL Server
```

```csharp
// Good: FacadeからBusinessを呼び出す
[FacadeMethod("BSFCSAR050800024")]
public virtual void InitStartNew(
   WorkEntity workEntity,
   InitialDisplayEntity initEntity,
   CoverSheetMasterEntity coverSheetMasterEntity,
   DsEntity dsEntity,
   out string isHeadquarters)
{
    ISalesChangeDisposalSlipEntryBusiness slipEntryBusiness =
        (ISalesChangeDisposalSlipEntryBusiness)ClassObjectUtil.GetBusinessInstance(
            this.salesChangeDisposalSlipEntryBusinessClassId,
            typeof(ISalesChangeDisposalSlipEntryBusiness));

    slipEntryBusiness.InitStartNew(
        workEntity,
        initEntity,
        coverSheetMasterEntity,
        dsEntity,
        out isHeadquarters);
}
```

```csharp
// Bad: Facadeを飛ばしてUIからDAOを直接呼び出す
public void Button_Click(object sender, EventArgs e)
{
    var dao = new SalesChangeDisposalSlipEntryDataAccessComponent();
    dao.UpdateDelFlgSARSlipHeaderNew(workEntity, dsEntity);
}
```

### 2. 最新.NETの流儀を無理に持ち込まない

CCR既存コードは .NET Framework、WebForms、Typed DataSet、DataAccessComponent が中心です。既存層で動いている処理に対して、ASP.NET Core、Minimal API、EF Core、records、nullable reference types、DIコンテナ登録、async/awaitを不用意に導入しない。

```csharp
// Good: 既存のDataAccessComponentをusingで利用する
using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent =
    new SalesChangeDisposalSlipEntryDataAccessComponent())
{
    dataAccessComponent.UpdateDelFlgSARSlipHeaderNew(workEntity, dsEntity);
    dataAccessComponent.UpdateDelFlgSARSlipDetailNew(workEntity, dsEntity);
}
```

```csharp
// Bad: 既存方式を無視してEF Coreへ置き換える
await dbContext.SARSlipHeaders
    .Where(x => x.SlipNo == slipNo)
    .ExecuteUpdateAsync(x => x.SetProperty(y => y.LogicalDelFlg, "1"));
```

### 3. 差分は小さく、呼出順と副作用を守る

業務処理では、赤伝登録、大福帳登録、論理削除、検索、訂正可否などが密につながっています。修正時は「対象呼出だけ」を抑止・追加し、周辺の既存処理を止めない。

```csharp
// Good: 既存の赤伝モード設定を維持し、対象呼出だけ条件分岐する
if (workEntity.CancelMode == Constants.OnFlag)
{
    workEntity.SlipUpdateMode = slipUpdateMode_Red;

    if (!this.IsNoLogicalDeleteTargetNew(workEntity, dsEntity))
    {
        using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent =
            new SalesChangeDisposalSlipEntryDataAccessComponent())
        {
            dataAccessComponent.UpdateDelFlgSARSlipHeaderNew(workEntity, dsEntity);
            dataAccessComponent.UpdateDelFlgSARSlipDetailNew(workEntity, dsEntity);
        }
    }
}
```

```csharp
// Bad: CancelMode分岐全体を消して赤伝登録や大福帳登録まで変える
if (workEntity.CancelMode == Constants.OnFlag)
{
    return;
}
```

## Naming Rules

### 1. クラス名

既存の役割サフィックスを使います。

| 種別 | 命名例 | ルール |
|---|---|---|
| Facade | `SalesChangeDisposalSlipEntryFacade` | `...Facade` |
| Business | `SalesChangeDisposalSlipEntryBusiness` | `...Business` |
| Interface | `ISalesChangeDisposalSlipEntryBusiness` | `I...Business` |
| DAO | `SalesChangeDisposalSlipEntryDataAccessComponent` | `...DataAccessComponent` |
| DataSet | `SARSlipDetailDataSet` | `...DataSet` |
| TableAdapter | `SARSlipDetailTableAdapter` | `...TableAdapter` |

```csharp
// Good
public class SalesChangeDisposalSlipEntryBusiness : BusinessBase, ISalesChangeDisposalSlipEntryBusiness
{
}

// Bad
public class SalesChangeService
{
}
```

### 2. メソッド名

- 既存メソッドはPascalCaseを使う。
- 既存モジュールで `New` サフィックスが使われている場合は、同じ系列の追加メソッドにも合わせる。
- DB操作メソッドは `Fill...`, `Insert...`, `Update...`, `Delete...`, `UpIns...`, `Create...QueryText`, `Create...ParameterList` の既存語彙に合わせる。
- 業務IDやDAO IDがある場合は、XMLコメントまたはコメントに残す。

```csharp
// Good
public virtual void UpdateDelFlgSARSlipHeaderNew(WorkEntity workEntity, DsEntity dsEntity)
{
}

private DbParameter[] CreateUpdateDelFlgSARSlipHeaderParameterListNew(
    DbCommand command,
    WorkEntity workEntity,
    DsEntity dsEntity)
{
}
```

```csharp
// Bad
public void UpdateDeletedFlagForHeader(WorkEntity work, DsEntity ds)
{
}
```

### 3. 変数名

- ローカル変数は既存コードに合わせてcamelCaseを使う。
- DataSetやAdapterは型名の意味を残す。
- DB列名、DataSet列名、業務項目名は既存の英字名を崩さない。

```csharp
// Good
SlipHeaderListDataSet slipHeaderListDataSet = new SlipHeaderListDataSet();
SARSlipDetailDataSet sarSlipDetailDataSet = new SARSlipDetailDataSet();
string slipTypeCd = row.SlipTypeCd;

// Bad
var data = new SlipHeaderListDataSet();
var type = row.SlipTypeCd;
```

## Facade Pattern

Facadeは薄く保ち、Business呼出の中継に徹する。

- `[FacadeMethod("...")]` を維持する。
- `ClassObjectUtil.GetBusinessInstance` でBusinessインターフェースを取得する。
- Facade内に業務分岐、SQL、DataSet加工を置かない。
- 既存のBusiness class IDを使用する。

```csharp
// Good
[FacadeMethod("BSFCSAR050800030")]
public virtual void UpInsSARSlipNew(
   WorkEntity workEntity,
   DsEntity dsEntity,
   InitialDisplayEntity initEntity)
{
    ISalesChangeDisposalSlipEntryBusiness slipEntryBusiness =
        (ISalesChangeDisposalSlipEntryBusiness)ClassObjectUtil.GetBusinessInstance(
            this.salesChangeDisposalSlipEntryBusinessClassId,
            typeof(ISalesChangeDisposalSlipEntryBusiness));

    slipEntryBusiness.UpInsSlipNew(workEntity, dsEntity, initEntity);
}
```

```csharp
// Bad
[FacadeMethod("BSFCSAR050800030")]
public virtual void UpInsSARSlipNew(...)
{
    if (workEntity.CancelMode == Constants.OnFlag)
    {
        // Facadeで業務更新やSQLを組み立てない
    }
}
```

## Business Pattern

### 1. 入力チェック

Businessメソッドでは、既存コードに合わせて冒頭で必須入力チェックを行う。

```csharp
// Good
#region 必須入力チェック

if (workEntity == null)
{
    throw new ArgumentNullException("workEntity");
}

if (dsEntity == null)
{
    throw new ArgumentNullException("dsEntity");
}

#endregion
```

```csharp
// Bad
if (workEntity == null) return;
```

### 2. 業務エラー

業務エラーは既存の `RetailBusinessException.CreateInstance(ErrorKey...)` を使う。

```csharp
// Good
if (sarSlipDetailDataSet.SARSlipDetail.Rows.Count.Equals(0))
{
    throw RetailBusinessException.CreateInstance(ErrorKey.ERRCSARB0167);
}

// Bad
throw new Exception("Data not found.");
```

### 3. 定数・AP変数・コード区分

フラグや区分値は直値を増やさず、既存の `Constants`、`APVariableUtil`、`CodeDivisionUtil` を優先する。

```csharp
// Good
if (workEntity.CancelMode == Constants.OnFlag)
{
    workEntity.SlipUpdateMode = slipUpdateMode_Red;
}

string slipTypeCdSalesChange = APVariableUtil.GetAPVariableValue(
    APVariableListKey.SAR_SALESCHANGE_SLIPTYPECD,
    headerRow.CorporationCode);

string dataID = CodeDivisionUtil.GetCodeDivisionValueName(
    CodeDivisionKey.DataId,
    "03",
    UserProfile.CorporationCode);
```

```csharp
// Bad
if (workEntity.CancelMode == "1")
{
    workEntity.SlipUpdateMode = "RED";
}
```

## DataAccessComponent Pattern

### 1. SQLはStringBuilderで構築する

既存DAOは `StringBuilder` と `AppendLine` でSQLを組み立てる。変数値は必ずパラメータ化する。

```csharp
// Good
StringBuilder updateCommandText = new StringBuilder();
updateCommandText.AppendLine(" UPDATE SARSlipHeader ");
updateCommandText.AppendLine("    SET LogicalDelFlg = '1',");
updateCommandText.AppendLine("        LogicalDelDate = @LogicalDelDate, ");
updateCommandText.AppendLine("        UpdateBy = @UpdateBy, ");
updateCommandText.AppendLine("        UpdateDate = @UpdateDate ");
updateCommandText.AppendLine(" WHERE");
updateCommandText.AppendLine("        CorporationCode = @CorporationCode");
updateCommandText.AppendLine("   AND  SlipDiv = @SlipDiv");
updateCommandText.AppendLine("   AND  SlipTypeCd = @SlipTypeCd");
updateCommandText.AppendLine("   AND  SlipNo = @SlipNo");
```

```csharp
// Bad
string sql = "UPDATE SARSlipHeader SET LogicalDelFlg = '1' WHERE SlipNo = '" + slipNo + "'";
```

### 2. DbCommandと接続管理

接続状態を保存し、開いた場合だけ閉じる。

```csharp
// Good
using (DbCommand command = this.Connection.CreateCommand())
{
    command.CommandText = updateCommandText.ToString();
    command.Parameters.AddRange(this.CreateUpdateParameterListNew(command, workEntity, dsEntity));
    this.SetCommonItemForUpdateNew(command);

    ConnectionState prevState = this.Connection.State;

    try
    {
        if (this.Connection.State != ConnectionState.Open)
        {
            this.Connection.Open();
        }

        command.ExecuteNonQuery();
    }
    finally
    {
        if (prevState == ConnectionState.Closed)
        {
            this.Connection.Close();
        }
    }
}
```

```csharp
// Bad
this.Connection.Open();
command.ExecuteNonQuery();
this.Connection.Close();
```

### 3. パラメータ作成は専用メソッドへ分離する

DAOメソッド内にパラメータ追加を散らさず、既存の `Create...ParameterList...` 方式に合わせる。

```csharp
// Good
command.Parameters.AddRange(
    this.CreateUpdateDelFlgSARSlipHeaderParameterListNew(command, workEntity, dsEntity));

// Bad
command.Parameters.Add(new SqlParameter("@CorporationCode", corporationCode));
command.Parameters.Add(new SqlParameter("@SlipNo", slipNo));
command.Parameters.Add(new SqlParameter("@StoreCd", storeCd));
```

必要な場合は既存メソッドの中で `CreateDbParameter` や `SqlParameter` の使い分けに合わせる。

## Typed DataSet / TableAdapter Pattern

Typed DataSetを使う既存箇所では、そのままDataSet / Row / TableAdapterの型を使う。

```csharp
// Good
using (SARSlipDetailTableAdapter sarSlipDetailTableAdapter = new SARSlipDetailTableAdapter())
{
    sarSlipDetailTableAdapter.FillBySlipDetailListSearchCondition(
        corporationCD,
        storeCD,
        slipTypeCD,
        slipBranchNo,
        slipDiv,
        slipNo,
        disposalChangeDate,
        sarSlipDetailDataSet);
}

if (sarSlipDetailDataSet.SARSlipDetail.Rows.Count.Equals(0))
{
    throw RetailBusinessException.CreateInstance(ErrorKey.ERRCSARB0167);
}
```

```csharp
// Bad
var details = repository.FindDetails(corporationCD, slipNo);
if (!details.Any())
{
    throw new InvalidOperationException();
}
```

Rowを作る場合はTyped Rowを使い、列名や計算式は既存項目を維持する。

```csharp
// Good
SlipDetailListDataSet.SARSlipDetailRow sliprow =
    (SlipDetailListDataSet.SARSlipDetailRow)slipDetailListDataSet.SARSlipDetail.NewRow();

sliprow.DisposalChangeQty =
    decimal.Parse(sarSlipDetailErrorDataSet.SARSlipDetailError.Rows[i]["DisposalChangeQty"].ToString());
sliprow.SalesChangePrice =
    (sliprow.SlipOldPrice - sliprow.SlipNewPrice) * sliprow.DisposalChangeQty;
sliprow.DisposalPriceAmt =
    sliprow.SlipDisposalUnitPrica * sliprow.DisposalChangeQty;
```

## DB and SQL Rules

### 1. 論理削除項目

CCRの多くのテーブルでは、以下の共通項目を持つ。

| 項目 | 意味 |
|---|---|
| `LogicalDelFlg` | 論理削除フラグ。boolではなく文字列区分として扱う。 |
| `LogicalDelDate` | 論理削除日時。未設定値は `9999/12/31` 系が多い。 |
| `CreateBy` / `CreateDate` | 作成者 / 作成日 |
| `UpdateBy` / `UpdateDate` | 更新者 / 更新日 |
| `SystemRowversion` | 行バージョン |

```csharp
// Good: 既存の共通項目をまとめて扱う
updateCommandText.AppendLine("    SET LogicalDelFlg = '1',");
updateCommandText.AppendLine("        LogicalDelDate = @LogicalDelDate, ");
updateCommandText.AppendLine("        UpdateBy = @UpdateBy, ");
updateCommandText.AppendLine("        UpdateDate = @UpdateDate ");

// Bad: LogicalDelFlgだけを単独で更新し、監査項目を無視する
updateCommandText.AppendLine(" SET LogicalDelFlg = '1' ");
```

### 2. キー条件を削らない

更新SQLでは既存キー条件を省略しない。`CorporationCode`、`SlipDiv`、`SlipTypeCd`、`SlipNo`、`DisposalChangeDate`、`StoreCd` など、既存WHERE条件を維持する。

```csharp
// Good
updateCommandText.AppendLine(" WHERE");
updateCommandText.AppendLine("        CorporationCode = @CorporationCode");
updateCommandText.AppendLine("   AND  SlipDiv = @SlipDiv");
updateCommandText.AppendLine("   AND  SlipTypeCd = @SlipTypeCd");
updateCommandText.AppendLine("   AND  SlipNo = @SlipNo");
updateCommandText.AppendLine("   AND  DisposalChangeDate = @DisposalChangeDate");
updateCommandText.AppendLine("   AND  StoreCd = @StoreCd");
```

```csharp
// Bad
updateCommandText.AppendLine(" WHERE SlipNo = @SlipNo");
```

### 3. 大福帳連携

大福帳関連処理では `DataID`、`CorrectionDiv`、`SlipTypeCd`、`SlipDiv`、`SlipNo`、`StoreCd` などが連携に影響する。既存経路を変更しない。

```csharp
// Good: 既存のDataID取得と登録経路を維持する
string dataID = CodeDivisionUtil.GetCodeDivisionValueName(
    CodeDivisionKey.DataId,
    "03",
    UserProfile.CorporationCode);

cmcAccountBookComponent.RegisterAccountBookForJournal(
    this.GenerateRedAccountBookDataSetNew(redSlipDataset, coverSheetMasterEntity));
```

```csharp
// Bad: DataIDを固定文字列として登録処理へ直渡しする
cmcAccountBookComponent.RegisterAccountBookForJournal(
    this.GenerateAccountBookDataSetNew(workEntity, dsEntity, "SAR003", coverSheetMasterEntity));
```

## Comment Rules

### 1. 既存コメントを尊重する

CCRコードには修正履歴コメント、業務ID、処理説明コメントが多く存在します。既存コメントを不用意に削除しない。

```csharp
// Good: 既存履歴を残し、今回修正範囲を明示する
// 2026.10.21 Redmine8842 Start 売変・廃棄取消時は元伝票を論理削除しない
if (!this.IsNoLogicalDeleteTargetNew(workEntity, dsEntity))
{
    dataAccessComponent.UpdateDelFlgSARSlipHeaderNew(workEntity, dsEntity);
    dataAccessComponent.UpdateDelFlgSARSlipDetailNew(workEntity, dsEntity);
}
// 2026.10.21 Redmine8842 End
```

```csharp
// Bad: 理由なしで既存処理を削除する
// dataAccessComponent.UpdateDelFlgSARSlipHeaderNew(workEntity, dsEntity);
// dataAccessComponent.UpdateDelFlgSARSlipDetailNew(workEntity, dsEntity);
```

### 2. コメントは「なぜ」を書く

処理内容をそのまま読むだけのコメントは増やさない。業務理由、既存制約、副作用回避を書く。

```csharp
// Good
// 赤伝登録と大福帳登録は既存通り実行する必要があるため、論理削除更新のみ抑止する。

// Bad
// if文開始
```

### 3. XMLコメントは既存粒度に合わせる

public / virtual メソッドには `<summary>`、必要に応じて `<remarks>`、`<param>` を付ける。

```csharp
/// <summary>
/// 売変・廃棄取消時に元伝票を論理削除対象とするか判定する。
/// </summary>
/// <param name="workEntity">ワークエンティティ</param>
/// <param name="dsEntity">DsEntity</param>
/// <returns>論理削除更新を抑止する場合はtrue</returns>
private bool IsNoLogicalDeleteTargetNew(WorkEntity workEntity, DsEntity dsEntity)
{
}
```

## Modification Pattern

既存コード修正時は、以下の順で考える。

1. MCPまたは検索で既存の同種実装を探す。
2. 同じ層、同じクラス、同じ命名に合わせる。
3. 既存メソッド本体を大きく壊さず、条件分岐や小さなprivateメソッドで差分を閉じ込める。
4. DAO SQLを変える前に、Business側の呼出条件で抑止できるか検討する。
5. 赤伝、大福帳、検索、訂正不可、監査項目の副作用を確認する。

```csharp
// Good: 既存メソッド本体を残し、呼出側で対象条件を制御する
if (workEntity.CancelMode == Constants.OnFlag)
{
    workEntity.SlipUpdateMode = slipUpdateMode_Red;

    if (!this.IsNoLogicalDeleteTargetNew(workEntity, dsEntity))
    {
        using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent =
            new SalesChangeDisposalSlipEntryDataAccessComponent())
        {
            dataAccessComponent.UpdateDelFlgSARSlipHeaderNew(workEntity, dsEntity);
            dataAccessComponent.UpdateDelFlgSARSlipDetailNew(workEntity, dsEntity);
        }
    }
}
```

```csharp
// Bad: DAOメソッドを空実装にして他機能まで影響させる
public virtual void UpdateDelFlgSARSlipHeaderNew(WorkEntity workEntity, DsEntity dsEntity)
{
}
```

## UI / WebForms Rules

- 画面コードでは既存のSessionKey、Redirect、GridViewイベント方式に合わせる。
- ボタン表示制御は既存のDataSet列やフラグを優先する。
- UIだけで禁止せず、直接遷移やサーバー側処理の必要性も確認する。

```csharp
// Good: 既存SessionKeyへ必要値をセットして既存画面遷移を維持する
this.SetSession(SalesChangeDisposalSlipSearchSessionKey.SASlipCorrectionNo, row.SlipNo);
this.SetSession(SalesChangeDisposalSlipSearchSessionKey.SASlipCorrectionStoreCD, row.StoreCd);
this.Redirect(strInputID, null);
```

```csharp
// Bad: 既存Session設計を使わず、QueryStringだけで渡す
Response.Redirect("Input.aspx?slipNo=" + row.SlipNo);
```

## Anti-Patterns to Avoid

| Anti-Pattern | Fix |
|---|---|
| 既存調査なしで新しい設計を作る | MCPと`rg`で同種実装を確認する |
| `refresh_codebase`を禁止されているのに実行する | 検索・読み取り系MCPツールだけ使う |
| EF Core / ASP.NET Coreへ置換する | 既存のDataAccessComponent / Typed DataSetを使う |
| SQL文字列へ値を連結する | `@Parameter` とParameterListを使う |
| Facadeに業務ロジックを書く | Businessへ委譲する |
| BusinessからUI Sessionを直接操作する | UI層に閉じる |
| DAOメソッドを空にする | 呼出側条件で影響範囲を限定する |
| `LogicalDelFlg`だけを見て完了判断する | `LogicalDelDate`、検索条件、赤伝、大福帳、訂正不可も確認する |
| 既存履歴コメントを削除する | 必要な修正コメントをStart/Endで追記する |
| `var data` のような曖昧名を増やす | 既存型名・業務名が分かる名前にする |

## Generated Code Checklist

コード生成後は以下を確認する。

- 対象層は正しいか。UI / Facade / Business / DataAccessComponent の責務を越えていないか。
- 既存の命名、`New` サフィックス、業務ID、FacadeMethod IDを崩していないか。
- `ArgumentNullException("paramName")` 形式の入力チェックを入れているか。
- `Constants.OnFlag` / `OffFlag`、AP変数、コード区分を利用しているか。
- SQLはパラメータ化されているか。
- 接続Open/Closeは既存方式で保護されているか。
- DataSet / TableAdapterは`using`で破棄されているか。
- 大福帳、赤伝、論理削除、検索、訂正不可の副作用を確認したか。
- コメントは日本語で、業務理由や修正理由を説明しているか。
- GOOD/BADの比較で、既存作法との差が説明できるか。

## MCP調査メモ

このスキル作成時は、`refresh_codebase` を実行せず、以下を使用してCCRコードベースの作法を確認した。

- `query_semantic_graph`: `SalesChangeDisposal` 周辺のFacade / Business / Web / DataAccessComponentを確認。
- `read_source_file_range`: 売変・廃棄伝票取消、赤伝登録、大福帳登録、論理削除更新、検索結果画面の既存コードを確認。
- `query_database_architecture`: `SARSlip`、`CMCAccountBook` 周辺のテーブル定義、共通項目、論理削除項目を確認。

確認した代表的な既存パターン:

- `CancelMode == Constants.OnFlag` の取消分岐。
- `SlipUpdateMode = slipUpdateMode_Red` による赤伝モード。
- `UpdateDelFlgSARSlipHeaderNew` / `UpdateDelFlgSARSlipDetailNew` による論理削除更新。
- `GenerateRedAccountBookDataSetNew` と `RegisterAccountBookForJournal` による赤伝大福帳登録。
- `StringBuilder.AppendLine`、`DbCommand`、ParameterList、`SetCommonItemForUpdateNew` によるDAO実装。
- Typed DataSet / TableAdapter による検索・登録更新。
