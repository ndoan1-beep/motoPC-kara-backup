# 要求受付資料の補足
CCRでの「売変・廃棄の伝票取消でLogicalDelFlgがセットされる」という処理を修正
---

# 1. コンテキスト

## システム概要

- Framework：.NET Framework 4.8
- Database：SQL Server 2019
- Architecture

```
UI / ASP.NET WebForms
 ↓
Facade
 ↓
Business
 ↓
DataAccessComponent（DAO）
 ↓
Inline SQL / DbCommand / SqlDataAdapter
 ↓
SQL Server
```

---

# 2. 作業目的（ゴール）

本システムは小売業に関するシステムであり、販売システムの「伝票取消」という機能について、変更動作を記載されています。

売変・廃棄の伝票取消を行うと：
ー　元伝票が削除されないようにする
ー　テーブルでいくつかの数量・金額が相殺される
ー　伝票検索画面で元伝票と新規登録赤伝が検索できるが、その二つの伝票に対して「修正」ボタンが表示しない（訂正選択が不可）
ー　在庫サブシステムに連携するため、大福帳（CMCAccountBookのDataID = `SAR003`）の登録・更新の動作は元々残り、変わらないようになる。

---