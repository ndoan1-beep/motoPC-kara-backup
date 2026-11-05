using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using NihonUnisys.CoreCenter.Core.User;
using NihonUnisys.CoreCenter.Retail.Buying.Buying.Business;
using NihonUnisys.CoreCenter.Retail.Common;
using NihonUnisys.CoreCenter.Retail.Common.Resources;
using NihonUnisys.CoreCenter.Core;
using System.Linq;

// 【修正履歴】
//   2011.12.06 関根 大福帳の時刻・時間帯に値を更新する
//
//   2011.12.19 小林 伝票取消の不具合を修正。
//
//   2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応
//
//   2012.01.16 関根 行削除分が更新されてしまう不具合対応
//
//   2012.01.21 関根 伝票取消時の障害対応
//
//   2012.06.26 小林 入力ＳＥＱ番号の対応漏れの対応
//
//   2012.06.26 小林 複数伝票登録し修正した場合に最大のInputSequenceNoの伝票しか取得されないのを修正
//
//   2012.06.26 小林 バッチ番号チェックからバッチ店舗を外し法人、伝票種別、伝票区分で一意とする
//
//   2012.06.26 小林 伝票取消対応(削除フラグをセットする)
//
//   2012.06.26 小林 カバーシートへの更新情報収集方法を修正
//
//   2012.06.26 小林 カバーシートのキー情報にヘッダー情報をセットしているので修正
//
//   2012.06.26 小林 カバーシートの入力枚数を画面入力値へ変更
//
//   2012.08.24 SlipLineNoの採番不具合修正
//
//   2014.01.07 関根 対象店舗の営業時間帯を取得出来ない場合は、９９９店舗の時間帯を取得する。
//
//   2014.03.18 関根 効率改善対応（商品名取得変更）
//
//   2014.04.30 関根 入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
//
//   2014.04.30 関根 伝票明細のソート順対応（行番号が２桁以上の場合の対応）
//
//   2014.07.14 関根 商品名から規格名を削除する（売変指示書に合わせる）
//  
//   2015.12.14 李　IN版を標準版のDataAccessComponentに統合


namespace NihonUnisys.CoreCenter.Retail.Sales.InputSalesSlip.Business
{
    /// <summary>
    /// SalesChangeDisposalSlipEntryDataAccessComponent
    /// </summary>
    public partial class SalesChangeDisposalSlipEntryDataAccessComponent : RetailDataAccessBase
    {
        /// <summary>
        /// バッチ番号チェックする。(DAOCSAR05010)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="coverSheetMasterDataset">カバーシート情報Ｄｓ</param>
        /// <param name="byrBatchNumberMasterDataSet">バッチ番号Ds</param>
        public virtual void ChkBatchNoNew(
            WorkEntity workEntity,
            CoverSheetMasterDataset coverSheetMasterDataset,
            BYRBatchNumberMasterDataSet byrBatchNumberMasterDataSet)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (coverSheetMasterDataset == null)
            {
                throw new ArgumentNullException("coverSheetMasterDataset");
            }

            if (byrBatchNumberMasterDataSet == null)
            {
                throw new ArgumentNullException("byrBatchNumberMasterDataSet");
            }
            #endregion 引数チェック

            // コマンドの作成(Command)
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成(Connection)
                // ただし、今回は自動生成で作られたConnectionを再利用する。
                if (coverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
                {
                    dbCommand.Connection = this.Connection;
                    dbCommand.CommandText = this.CreateChkBatchNoFillQueryNew();

                    // パラメータの設定
                    dbCommand.Parameters.AddRange(this.CreateChkBatchNoDbParameterListNew(dbCommand, coverSheetMasterDataset.CoverSheetMasterDataTable1[0]));

                    // アダプターの作成(Adapter)
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        // アダプターにコマンドをセットしてあげる。
                        adapter.SelectCommand = (SqlCommand)dbCommand;

                        // 対象のテーブルをクリアする。
                        byrBatchNumberMasterDataSet.BYRBatchNumberMaster.Clear();

                        // Fillメソッドを呼び出す。
                        // Fillメソッドは取得件数を戻り値で返してくれる
                        adapter.Fill(byrBatchNumberMasterDataSet.BYRBatchNumberMaster);
                    }
                }
            }
        }

        /// <summary>
        /// カバー保存(DAOCSAR05011)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpInsSARCoverSheetMasterNew(
            WorkEntity workEntity,
            DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            switch (workEntity.CoverExecuteMode)
            {
                case "0":
                    this.InsSARCoverSheetMasterNew(dsEntity.CoverSheetMasterDataset);
                    break;
                case "1":
                    this.UpSARCoverSheetMaster1New(dsEntity.CoverSheetMasterDataset);
                    break;
                case "2":
                    this.UpSARCoverSheetMaster2New(dsEntity.CoverSheetMasterDataset);
                    break;
            }
        }

        /// <summary>
        /// 伝票ヘッダ検索(DAOCSAR05012)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void FillSARSlipHeaderErrorNew(
            WorkEntity workEntity,
            DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            // コマンドの作成(Command)
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成(Connection)
                // ただし、今回は自動生成で作られたConnectionを再利用する。
                if (dsEntity.CoverSheetMasterDataset != null &&
                    dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Any() &&
                    dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
                {
                    dbCommand.Connection = this.Connection;
                    dbCommand.CommandText = this.CreateFillSARSlipHeaderErrorFillQueryNew(workEntity);

                    // パラメータの設定
                    dbCommand.Parameters.AddRange(this.CreateFillSARSlipHeaderErrorParameterListNew(dbCommand, dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0], workEntity));

                    // アダプターの作成(Adapter)
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        // アダプターにコマンドをセットしてあげる。
                        adapter.SelectCommand = (SqlCommand)dbCommand;
                        dsEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();

                        // Fillメソッドを呼び出す。
                        // Fillメソッドは取得件数を戻り値で返してくれる
                        adapter.Fill(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError);
                    }
                }
            }
        }

        /// <summary>
        /// 伝票明細検索(DAOCSAR05013)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void FillSARSlipDetailErrorNew(
            WorkEntity workEntity,
            DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            // コマンドの作成(Command)
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成(Connection)
                // ただし、今回は自動生成で作られたConnectionを再利用する。
                if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
                {
                    dbCommand.Connection = this.Connection;
                    dbCommand.CommandText = this.CreateFillSARSlipDetailErrorFillQueryNew(workEntity);

                    // パラメータの設定
                    dbCommand.Parameters.AddRange(this.CreateFillSARSlipDetailErrorParameterListNew(dbCommand, dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0], workEntity));

                    // アダプターの作成(Adapter)
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        // アダプターにコマンドをセットしてあげる。
                        adapter.SelectCommand = (SqlCommand)dbCommand;
                        dsEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();

                        // Fillメソッドを呼び出す。
                        // Fillメソッドは取得件数を戻り値で返してくれる
                        adapter.Fill(dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError);
                    }
                }
            }
        }

        /// <summary>
        /// カバー承認依頼(DAOCSAR05015)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        public virtual void UpdateApprovalSARCoverSheetMasterNew(WorkEntity workEntity, DsEntity dsEntity, InitialDisplayEntity initEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARCoverSheetMaster ");
            updateCommandText.AppendLine(" SET ApprovalRequester = @ApprovalRequester,");
            updateCommandText.AppendLine("  RequestApprovalDate = @RequestApprovalDate, ");
            updateCommandText.AppendLine("  ApprovalStatus = @ApprovalStatus, ");
            updateCommandText.AppendLine("  LastUpdateDate = @LastUpdateDate, ");
            updateCommandText.AppendLine("  UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("  UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine("WHERE");
            updateCommandText.AppendLine(" CorporationCode = @CorporationCode");
            updateCommandText.AppendLine(" AND BatchNo = @BatchNo");
            updateCommandText.AppendLine(" AND StoreCd = @StoreCd");
            updateCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            updateCommandText.AppendLine(" AND BatchDate = @BatchDate");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateApprovalSARCoverSheetMasterParameterListNew(command, initEntity, dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0]));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }

        /// <summary>
        /// 伝票承認依頼(DAOCSAR05016)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        public virtual void UpdateApprovalSARSlipHeaderErrorNew(WorkEntity workEntity, DsEntity dsEntity, InitialDisplayEntity initEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARSlipHeaderError ");
            updateCommandText.AppendLine(" SET ApprovalRequester = @ApprovalRequester,");
            updateCommandText.AppendLine("  RequestApprovalDate = @RequestApprovalDate, ");
            updateCommandText.AppendLine("  ApprovalStatus = @ApprovalStatus, ");
            updateCommandText.AppendLine("  UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("  UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine("WHERE");
            updateCommandText.AppendLine(" CorporationCode = @CorporationCode");
            updateCommandText.AppendLine(" AND InputMediaDiv = @InputMediaDiv");
            updateCommandText.AppendLine(" AND InputBatchNo = @InputBatchNo");
            updateCommandText.AppendLine(" AND InputSequenceNo = @InputSequenceNo");
            updateCommandText.AppendLine(" AND InputDate = @InputDate");
            updateCommandText.AppendLine(" AND InputTime = @InputTime");
            updateCommandText.AppendLine(" AND DataType = @DataType");
            updateCommandText.AppendLine(" AND UptakeSequenceNo = @UptakeSequenceNo");
            updateCommandText.AppendLine(" AND StoreCd = @StoreCd");
            updateCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            updateCommandText.AppendLine(" AND SlipNo = @SlipNo");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateApprovalSARSlipHeaderErrorParameterListNew(command, initEntity, dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[0]));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }

        /// <summary>
        /// トランヘッダ既存データ更新(DAOCSAR05017)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpdateCorrectionDivSARSlipHeaderErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARSlipHeaderError ");
            updateCommandText.AppendLine(" SET CorrectionDiv = '1'");
            updateCommandText.AppendLine("  UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("  UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine("WHERE");
            updateCommandText.AppendLine(" CorporationCode = @CorporationCode");
            updateCommandText.AppendLine(" AND InputMediaDiv = @InputMediaDiv");
            updateCommandText.AppendLine(" AND InputBatchNo = @InputBatchNo");
            updateCommandText.AppendLine(" AND InputSequenceNo = @InputSequenceNo");
            updateCommandText.AppendLine(" AND InputDate = @InputDate");
            updateCommandText.AppendLine(" AND InputTime = @InputTime");
            updateCommandText.AppendLine(" AND DataType = @DataType");
            updateCommandText.AppendLine(" AND UptakeSequenceNo = @UptakeSequenceNo");
            updateCommandText.AppendLine(" AND StoreCd = @StoreCd");
            updateCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            updateCommandText.AppendLine(" AND SlipNo = @SlipNo");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateCorrectionDivSARSlipHeaderErrorParameterListNew(command, dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[0]));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }

        /// <summary>
        /// 伝票ヘッダ既存データ更新(DAOCSAR05018)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpdateCorrectionDivSARSlipHeaderNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARSlipHeader ");
            updateCommandText.AppendLine(" SET CorrectionDiv = '1'");
            updateCommandText.AppendLine("  UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("  UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine("WHERE");
            updateCommandText.AppendLine(" CorporationCode = @CorporationCode");
            updateCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            updateCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine(" AND SlipNo = @SlipNo");
            updateCommandText.AppendLine(" AND SlipBranchNo = @SlipBranchNo");
            updateCommandText.AppendLine(" AND DisposalChangeDate = @DisposalChangeDate");
            updateCommandText.AppendLine(" AND StoreCd = @StoreCd");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateCorrectionDivSARSlipHeaderParameterListNew(command, dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[0], workEntity));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }

        // 2013/09/17 アドオン対応：カバーシート更新情報収集追加 ISC:yshindo start
        /// <summary>
        /// カバーシート更新情報収集(DAOCSAR05022)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void FillSARCoverSheetMasterForUpdateNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            // 総枚数
            int totalPageQty = this.GetTotalPageQtyNew(dsEntity);

            // 2012.06.26 Start カバーシートの入力枚数を画面入力値へ変更
            //// 入力枚数
            //int inputQty = this.GetInputPageCount(dsEntity);
            // 2012.06.26 End   カバーシートの入力枚数を画面入力値へ変更

            // エラー枚数
            int errorQty = this.GetErrorNoNew(dsEntity);

            // 承認枚数
            int approvalQty = this.GetApprovalCountNew(dsEntity);

            // 取得結果をDsEntity.更新用カバーシートマスタデータセットへ格納する。
            if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
            {
                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetRow = dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0];
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

                // 2012.06.26 Start カバーシートのキー情報にヘッダー情報をセットしているので修正
                //coverSheetRow.CorporationCode = headerRow.CorporationCode;
                //coverSheetRow.InputBatchNo = headerRow.InputBatchNo;
                //coverSheetRow.StoreCd = headerRow.StoreCd;
                //coverSheetRow.SlipTypeCd = headerRow.SlipTypeCd;
                //coverSheetRow.SlipDiv = headerRow.SlipDiv;
                //coverSheetRow.InputBatchDate = headerRow.DisposalChangeDate.ToShortDateString();
                // 2012.06.26 End   カバーシートのキー情報にヘッダー情報をセットしているので修正
                coverSheetRow.TotalPageQty = totalPageQty;
                // 2012.06.26 Start カバーシートの入力枚数を画面入力値へ変更
                //coverSheetRow.InputPageCount = inputQty;
                // 2012.06.26 End   カバーシートの入力枚数を画面入力値へ変更
                coverSheetRow.ErrorNo = errorQty;
                coverSheetRow.ApprovalCount = approvalQty;
            }
        }
        // 2013/09/17 アドオン対応：カバーシート更新情報収集追加 ISC:yshindo end

        /// <summary>
        /// ＳＡＲ伝票ヘッダエラー登録更新(DAOCSAR05023)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpInsSARSlipHeaderErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            //  2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
            //// 2011.12.19 Start 伝票取消の不具合を修正。
            ////if ((workEntity.TranUpdateMode == "0") || (workEntity.InitMode == "0"))
            ////{
            ////    this.InsertSARSlipHeaderError(dsEntity.SARSlipHeaderErrorDataset);
            ////}
            ////else if ((workEntity.TranUpdateMode == "1") || (workEntity.InitMode != "0"))
            ////{
            ////    this.UpdateSARSlipHeaderError(dsEntity.SARSlipHeaderErrorDataset);
            ////}

            //// TranUpdateModeを優先するように修正
            //if (workEntity.TranUpdateMode == "0")
            //{
            //    this.InsertSARSlipHeaderError(dsEntity.SARSlipHeaderErrorDataset);
            //}
            //else if (workEntity.TranUpdateMode == "1")
            //{
            //    this.UpdateSARSlipHeaderError(dsEntity.SARSlipHeaderErrorDataset);
            //}
            //else if (workEntity.InitMode == "0")
            //{
            //    this.InsertSARSlipHeaderError(dsEntity.SARSlipHeaderErrorDataset);
            //}
            //else if (workEntity.InitMode != "0")
            //{
            //    this.UpdateSARSlipHeaderError(dsEntity.SARSlipHeaderErrorDataset);
            //}
            //// 2011.12.19 End   伝票取消の不具合を修正。

            // TranUpdateModeを優先するように修正
            if (workEntity.TranUpdateMode == "0")
            {
                this.InsertSARSlipHeaderErrorNew(dsEntity.SARSlipHeaderErrorDataset);
            }
            else if (workEntity.TranUpdateMode == "1")
            {
                this.UpdateSARSlipHeaderErrorNew(dsEntity.SARSlipHeaderErrorDataset);
            }
            // InitModeに関係なく全てInsertを行う様に修正
            else
            {
                this.InsertSARSlipHeaderErrorNew(dsEntity.SARSlipHeaderErrorDataset);
            }
            //  2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了
        }

        /// <summary>
        /// ＳＡＲ伝票明細エラー登録更新(DAOCSAR05024)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpInsSARSlipDetailErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            // 2012.08.24 Start SlipLineNoの採番不具合修正
            int newSlipLineNo = 0;
            // 2012.08.24 End   SlipLineNoの採番不具合修正
            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Where(
                r => r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified))
            {
                //   2012.01.16 関根 行削除分が更新されてしまう不具合対応 開始
                // 削除フラグチェック（１以外：更新、１：削除）
                if (detailRow.LogicalDelFlg != "1")
                //   2012.01.16 関根 行削除分が更新されてしまう不具合対応 終了
                {
                    // 2012.08.24 Start SlipLineNoの採番不具合修正
                    newSlipLineNo++;
                    // 2012.08.24 End   SlipLineNoの採番不具合修正

                    using (DbCommand command = this.Connection.CreateCommand())
                    {
                        command.CommandText = this.CreateUpInsSARSlipDetailErrorQueryTextNew();

                        // 2012.08.24 Start SlipLineNoの採番不具合修正
                        //command.Parameters.AddRange(this.CreateUpInsSARSlipDetailErrorParameterList(command, detailRow));
                        command.Parameters.AddRange(this.CreateUpInsSARSlipDetailErrorParameterListNew(command, detailRow, newSlipLineNo));
                        // 2012.08.24 End   SlipLineNoの採番不具合修正
                        this.SetCommonItemForInsertNew(command);

                        ConnectionState prevState = this.Connection.State;

                        // データベースアクセス処理。
                        // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
                }
            }
        }

        /// <summary>
        /// ＳＡＲ伝票ヘッダーの登録(DAOCSAR05025)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        /// <returns>更新伝票番号枝番</returns>
        public virtual int InsertSARSlipHeaderNew(WorkEntity workEntity, DsEntity dsEntity, InitialDisplayEntity initEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            int nextSlipBranchNo = this.GetNextSlipBranchNoNew(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0]);

            using (DbCommand command = this.Connection.CreateCommand())
            {
                //   2012.01.21 関根 伝票取消時の障害対応 開始
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].LogicalDelFlg != "1")
                //   2012.01.21 関根 伝票取消時の障害対応 終了
                {
                    command.CommandText = this.CreateInsertSARSlipHeaderQueryTextNew();

                    command.Parameters.AddRange(this.CreateInsertSARSlipHeaderParameterListNew(
                        command,
                        workEntity,
                        dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0],
                        initEntity,
                        nextSlipBranchNo));
                    this.SetCommonItemForInsertNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }

            return nextSlipBranchNo;
        }

        /// <summary>
        /// 伝票明細登録更新(DAOCSAR05026)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void InsertSARSlipDetailNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            // 2012.08.24 Start SlipLineNoの採番不具合修正
            int newSlipLineNo = 0;
            // 2012.08.24 End   SlipLineNoの採番不具合修正
#warning SKN修正　訂正更新で赤伝・黒伝の明細を登録するように修正 -->
            //foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Where(r => r.RowState == DataRowState.Added))
            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
#warning SKN修正　訂正更新で赤伝・黒伝の明細を登録するように修正 <--
            {
                //   2012.01.16 関根 行削除分が更新されてしまう不具合対応 開始
                // 削除フラグチェック（１以外：更新、１：削除）
                if (detailRow.LogicalDelFlg != "1")
                //   2012.01.16 関根 行削除分が更新されてしまう不具合対応 終了
                {
                    // 2012.08.24 Start SlipLineNoの採番不具合修正
                    newSlipLineNo++;
                    detailRow.SlipLineNo = newSlipLineNo.ToString();
                    // 2012.08.24 End   SlipLineNoの採番不具合修正

                    using (DbCommand command = this.Connection.CreateCommand())
                    {
                        command.CommandText = this.CreateInsertSARSlipDetailFillQueryNew();

                        command.Parameters.AddRange(this.CreateInsertSARSlipDetailParameterListNew(command, workEntity, detailRow));
                        this.SetCommonItemForInsertNew(command);

                        ConnectionState prevState = this.Connection.State;

                        // データベースアクセス処理。
                        // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
                }
            }
        }

        /// <summary>
        /// ＳＡＲ伝票ヘッダー赤伝更新(DAOCSAR05028)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>更新伝票番号枝番</returns>
        public virtual int InsertSARSlipHeaderForDenyNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            int nextSlipBranchNo = this.GetNextSlipBranchNoNew(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0]);

            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = this.CreateInsertSARSlipHeaderForDenyQueryTextNew();

                command.Parameters.AddRange(this.CreateInsertSARSlipHeaderForDenyParameterListNew(command, dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0], workEntity, nextSlipBranchNo));
                this.SetCommonItemForUpdateNew(command);

                ConnectionState prevState = this.Connection.State;

                // データベースアクセス処理。
                // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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

            return nextSlipBranchNo;
        }

        /// <summary>
        /// ＳＡＲ伝票明細赤伝更新(DAOCSAR05029)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void InsertSARSlipDetailForDenyNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

#warning SKN修正　訂正更新で赤伝・黒伝の明細を登録するように修正 -->
            //foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Where(r => r.RowState == DataRowState.Added))
            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
#warning SKN修正　訂正更新で赤伝・黒伝の明細を登録するように修正 <--
            {
                using (DbCommand command = this.Connection.CreateCommand())
                {
                    command.CommandText = this.CreateInsertSARSlipDetailForDenyFillQueryNew();

                    command.Parameters.AddRange(this.CreateInsertSARSlipDetailForDenyParameterListNew(
                                                        command,
                                                        detailRow,
                                                        dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0],
                                                        workEntity));
                    this.SetCommonItemForInsertNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }

        /// <summary>
        /// カバーシート検索(DAOCSAR05030)
        /// </summary>
        /// <param name="coverSheetMasterEntity">カバーシート情報Entity</param>
        /// <param name="coverSheetMasterDataset">カバーシート情報Ｄｓ</param>
        public virtual void FillSARCoverSheetMasterNew(CoverSheetMasterEntity coverSheetMasterEntity, CoverSheetMasterDataset coverSheetMasterDataset)
        {
            #region 引数チェック
            if (coverSheetMasterEntity == null)
            {
                throw new ArgumentNullException("coverSheetMasterEntity");
            }

            if (coverSheetMasterDataset == null)
            {
                throw new ArgumentNullException("coverSheetMasterDataset");
            }
            #endregion 引数チェック

            // コマンドの作成(Command)
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成(Connection)
                // ただし、今回は自動生成で作られたConnectionを再利用する。
                dbCommand.Connection = this.Connection;
                dbCommand.CommandText = this.CreateFillSARCoverSheetMasterFillQueryNew();

                // パラメータの設定
                dbCommand.Parameters.AddRange(this.CreateFillSARCoverSheetMasterParameterListNew(dbCommand, coverSheetMasterEntity));

                // アダプターの作成(Adapter)
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    // アダプターにコマンドをセットしてあげる。
                    adapter.SelectCommand = (SqlCommand)dbCommand;
                    coverSheetMasterDataset.CoverSheetMasterDataTable1.Clear();

                    // Fillメソッドを呼び出す。
                    // Fillメソッドは取得件数を戻り値で返してくれる
                    adapter.Fill(coverSheetMasterDataset.CoverSheetMasterDataTable1);
                }
            }
        }

#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 -->
        /// <summary>
        /// ＳＡＲ伝票ヘッダーの更新()
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        public virtual void UpdateSARSlipHeaderNew(WorkEntity workEntity, DsEntity dsEntity, InitialDisplayEntity initEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            using (DbCommand command = this.Connection.CreateCommand())
            {
                // 8/30 SUEYOSHI 最新 - 1 の伝票番号を取得しentityに格納
                workEntity.SlipSubNo = Convert.ToString(this.GetNextSlipBranchNoNew(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0]) - 1);
                // 8/30 SUEYOSHI END

                command.CommandText = this.CreateUpdateSARSlipHeaderQueryTextNew();

                command.Parameters.AddRange(this.CreateUpdateSARSlipHeaderParameterListNew(
                    command,
                    workEntity,
                    dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0],
                    initEntity));
                this.SetCommonItemForInsertNew(command);

                ConnectionState prevState = this.Connection.State;

                // データベースアクセス処理。
                // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
        }
#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 <--

        // 2014.01.07 Start 対象店舗の営業時間帯を取得出来ない場合は、９９９店舗の時間帯を取得する。
        /// <summary>
        /// 営業時間帯の取得(DAOCSAR05025)
        /// </summary>
        /// <param name= "corporationCode">法人コード</param>
        /// <param name= "storeCd">店舗コード</param>
        /// <param name= "workTime">対象時刻</param>
        /// <returns>営業時間帯</returns>
        public virtual string SelectBusinessPeriodNew(
           string corporationCode,
           string storeCd,
           string workTime)
        {
            string outBusinessPeriod = string.Empty;

            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = this.GetBusinessPeriodNew();

                command.Parameters.AddRange(this.SelectGetBusinessPeriodParameterListNew(
                    command,
                    corporationCode,
                    storeCd,
                    workTime));

                this.SetCommonItemForInsertNew(command);

                ConnectionState prevState = this.Connection.State;

                // データベースアクセス処理。
                // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
                try
                {
                    if (this.Connection.State != ConnectionState.Open)
                    {
                        this.Connection.Open();
                    }

                    // 取得件数を戻り値に設定する。
                    DbDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        outBusinessPeriod = (string)reader["BusinessPeriod"];
                    }
                }
                finally
                {
                    if (prevState == ConnectionState.Closed)
                    {
                        this.Connection.Close();
                    }
                }
            }

            return outBusinessPeriod;
        }
        // 2014.01.07 End   対象店舗の営業時間帯を取得出来ない場合は、９９９店舗の時間帯を取得する。

        // 2012.06.26 Start 伝票取消対応(削除フラグをセットする)
        #region ＳＡＲ伝票ヘッダエラーに削除フラグをセット
        /// <summary>
        /// ＳＡＲ伝票ヘッダエラーに削除フラグをセット
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpdateDelFlgSARSlipHeaderErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARSlipHeaderError ");
            updateCommandText.AppendLine("    SET LogicalDelFlg = '1',");
            updateCommandText.AppendLine("        LogicalDelDate = @LogicalDelDate, ");
            updateCommandText.AppendLine("        UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("        UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine(" WHERE");
            updateCommandText.AppendLine("        CorporationCode = @CorporationCode");
            updateCommandText.AppendLine("   AND  SlipDiv = @SlipDiv");
            updateCommandText.AppendLine("   AND  SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine("   AND  SlipNo = @SlipNo");
            updateCommandText.AppendLine("   AND  DisposalChangeDate = @DisposalChangeDate");
            updateCommandText.AppendLine("   AND  StoreCd = @StoreCd");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateDelFlgSARSlipHeaderErrorParameterListNew(command, workEntity, dsEntity));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }
        #endregion

        #region ＳＡＲ伝票明細エラーに削除フラグをセット
        /// <summary>
        /// ＳＡＲ伝票明細エラーに削除フラグをセット
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpdateDelFlgSARSlipDetailErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARSlipDetailError ");
            updateCommandText.AppendLine("    SET LogicalDelFlg = '1',");
            updateCommandText.AppendLine("        LogicalDelDate = @LogicalDelDate, ");
            updateCommandText.AppendLine("        UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("        UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine(" WHERE");
            updateCommandText.AppendLine("        CorporationCode = @CorporationCode");
            updateCommandText.AppendLine("   AND  SlipDiv = @SlipDiv");
            updateCommandText.AppendLine("   AND  SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine("   AND  SlipNo = @SlipNo");
            updateCommandText.AppendLine("   AND  DisposalChangeDate = @DisposalChangeDate");
            updateCommandText.AppendLine("   AND  StoreCd = @StoreCd");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateDelFlgSARSlipDetailErrorParameterListNew(command, workEntity, dsEntity));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }
        #endregion

        #region ＳＡＲ伝票ヘッダに削除フラグをセット
        /// <summary>
        /// ＳＡＲ伝票ヘッダに削除フラグをセット
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpdateDelFlgSARSlipHeaderNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

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
            updateCommandText.AppendLine("   AND  DisposalChangeDate = @DisposalChangeDate");
            updateCommandText.AppendLine("   AND  StoreCd = @StoreCd");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateDelFlgSARSlipHeaderParameterListNew(command, workEntity, dsEntity));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }
        #endregion

        #region ＳＡＲ伝票明細に削除フラグをセット
        /// <summary>
        /// ＳＡＲ伝票明細に削除フラグをセット
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpdateDelFlgSARSlipDetailNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック
        #region ＳＡＲ伝票ヘッダに訂正不可フラグをセット
        /// <summary>
        /// ＳＡＲ伝票ヘッダに訂正不可フラグをセット
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpdateSlipRevisionFlgSARSlipHeaderNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARSlipHeader ");
            updateCommandText.AppendLine("    SET SlipRevisionFlg = '1',");
            updateCommandText.AppendLine("        UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("        UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine(" WHERE");
            updateCommandText.AppendLine("        CorporationCode = @CorporationCode");
            updateCommandText.AppendLine("   AND  SlipDiv = @SlipDiv");
            updateCommandText.AppendLine("   AND  SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine("   AND  SlipNo = @SlipNo");
            updateCommandText.AppendLine("   AND  DisposalChangeDate = @DisposalChangeDate");
            updateCommandText.AppendLine("   AND  StoreCd = @StoreCd");
            updateCommandText.AppendLine("   AND  LogicalDelFlg = '0'");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateSlipRevisionFlgSARSlipHeaderParameterListNew(command, workEntity, dsEntity));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }
        #endregion

        #region ＳＡＲ伝票ヘッダエラーにFixDateをセット
        /// <summary>
        /// ＳＡＲ伝票ヘッダエラーにFixDateをセット
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void UpdateFixDateSARSlipHeaderErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARSlipHeaderError ");
            updateCommandText.AppendLine("    SET FixDate = '9999/12/31',");
            updateCommandText.AppendLine("        UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("        UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine(" WHERE");
            updateCommandText.AppendLine("        CorporationCode = @CorporationCode");
            updateCommandText.AppendLine("   AND  SlipDiv = @SlipDiv");
            updateCommandText.AppendLine("   AND  SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine("   AND  SlipNo = @SlipNo");
            updateCommandText.AppendLine("   AND  DisposalChangeDate = @DisposalChangeDate");
            updateCommandText.AppendLine("   AND  StoreCd = @StoreCd");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpdateFixDateSARSlipHeaderErrorParameterListNew(command, workEntity, dsEntity));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }
        #endregion
        // 2012.06.26 End   伝票取消対応(削除フラグをセットする)

        // 2013.11.02 AST Add Start
        /// <summary>
        /// 売変区分を取得する。
        /// </summary>
        /// <param name="corporationCode">法人コード</param>
        /// <param name="storeCd">店舗コード</param>
        /// <param name="posDiscountDiv">ＰＯＳ値引区分</param>
        /// <returns>売変区分</returns>
        public string GetSalesChangeDivNew(string corporationCode, string storeCd, string posDiscountDiv)
        {
            string salesChangeDiv = string.Empty;
            StringBuilder sqlQueryText = new StringBuilder();

            sqlQueryText.AppendLine(" SELECT A.SalesChangeDiv");
            sqlQueryText.AppendLine(" FROM SARSalesChangePosDiscountMapping AS A");
            sqlQueryText.AppendLine(" WHERE A.CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine(" AND A.StoreCd = @StoreCd");
            sqlQueryText.AppendLine(" AND A.POSDiscountDiv = @POSDiscountDiv");

            // コマンドの作成（Command）
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成（Connection）
                dbCommand.Connection = this.Connection;
                dbCommand.CommandText = sqlQueryText.ToString();

                // パラメータの設定
                List<DbParameter> dbParameterList = new List<DbParameter>();
                dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", corporationCode));
                dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", storeCd));
                dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@POSDiscountDiv", posDiscountDiv));
                dbCommand.Parameters.AddRange(dbParameterList.ToArray());

                ConnectionState prevState = this.Connection.State;
                try
                {
                    if (this.Connection.State != ConnectionState.Open)
                    {
                        this.Connection.Open();
                    }

                    // 取得件数を戻り値に設定する。
                    DbDataReader reader = dbCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        salesChangeDiv = reader["SalesChangeDiv"].ToString();
                    }
                }
                finally
                {
                    if (prevState == ConnectionState.Closed)
                    {
                        this.Connection.Close();
                    }
                }
            }

            return salesChangeDiv;
        }
        // 2013.11.02 AST Add End

        // 2014.03.18 Start 効率改善対応（商品名取得変更）
        /// <summary>
        /// 伝票明細検索(DAOCSAR05013)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        public virtual void FillSARSlipDetailError_GetProductsKanjiNew(
            WorkEntity workEntity,
            DsEntity dsEntity)
        {
            #region 引数チェック
            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion 引数チェック

            // コマンドの作成(Command)
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成(Connection)
                // ただし、今回は自動生成で作られたConnectionを再利用する。
                if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
                {
                    dbCommand.Connection = this.Connection;
                    dbCommand.CommandText = this.CreateFillSARSlipDetailErrorGetProductsKanjiFillQueryNew(workEntity);

                    // パラメータの設定
                    dbCommand.Parameters.AddRange(this.CreateFillSARSlipDetailErrorGetProductsKanjiParameterListNew(dbCommand, dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0], workEntity));

                	//2019/02/20 k.sugawara Start タイムアウト時間を設定
                	// SQLコマンドタイムアウト時間設定
                	dbCommand.CommandTimeout = GetSqlCommandTimeoutValue(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].CorporationCode);
                	//2019/02/20 k.sugawara End タイムアウト時間を設定
                	
                    // アダプターの作成(Adapter)
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        // アダプターにコマンドをセットしてあげる。
                        adapter.SelectCommand = (SqlCommand)dbCommand;
                        dsEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();

                        // Fillメソッドを呼び出す。
                        // Fillメソッドは取得件数を戻り値で返してくれる
                        adapter.Fill(dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError);
                    }
                }
            }
        }
        // 2014.03.18 End   効率改善対応（商品名取得変更）


        #region private

        /// <summary>
        /// ＳＡＲカバーシートマスタの登録
        /// </summary>
        /// <param name="coverSheetMasterDataset">カバーシート情報Ｄｓ</param>
        private void InsSARCoverSheetMasterNew(CoverSheetMasterDataset coverSheetMasterDataset)
        {
            StringBuilder insertCommandText = new StringBuilder();
            insertCommandText.AppendLine(" INSERT INTO SARCoverSheetMaster(");
            insertCommandText.AppendLine(" CorporationCode");
            insertCommandText.AppendLine(" ,BatchNo");
            insertCommandText.AppendLine(" ,StoreCd");
            insertCommandText.AppendLine(" ,SlipTypeCd");
            insertCommandText.AppendLine(" ,SlipDiv");
            insertCommandText.AppendLine(" ,BatchDate");
            insertCommandText.AppendLine(" ,TotalPageQty");
            insertCommandText.AppendLine(" ,InputPageCount");
            insertCommandText.AppendLine(" ,ErrorNo");
            insertCommandText.AppendLine(" ,ApprovalCount");
            insertCommandText.AppendLine(" ,PersonnelInput");
            // 2013/09/25 アドオン対応：承認項目を追加 ISC:yshindo start
            insertCommandText.AppendLine(" ,ApproverCd");
            insertCommandText.AppendLine(" ,ApprovalStatus");
            insertCommandText.AppendLine(" ,ApprovalDate");
            // 2013/09/25 アドオン対応：承認項目を追加 ISC:yshindo end
            insertCommandText.AppendLine(" ,LogicalDelFlg");
            insertCommandText.AppendLine(" ,LastUpdateDate");
            insertCommandText.AppendLine(" ,CreateBy");
            insertCommandText.AppendLine(" ,CreateDate");
            insertCommandText.AppendLine(" ,UpdateBy");
            insertCommandText.AppendLine(" ,UpdateDate)");
            insertCommandText.AppendLine("VALUES(");
            insertCommandText.AppendLine(" @CorporationCode");
            insertCommandText.AppendLine(" ,@BatchNo");
            insertCommandText.AppendLine(" ,@StoreCd");
            insertCommandText.AppendLine(" ,@SlipTypeCd");
            insertCommandText.AppendLine(" ,@SlipDiv");
            insertCommandText.AppendLine(" ,@BatchDate");
            insertCommandText.AppendLine(" ,@TotalPageQty");
            insertCommandText.AppendLine(" ,@InputPageCount");
            insertCommandText.AppendLine(" ,@ErrorNo");
            insertCommandText.AppendLine(" ,@ApprovalCount");
            insertCommandText.AppendLine(" ,@PersonnelInput");
            // 2013/09/25 アドオン対応：承認項目を追加 ISC:yshindo start
            insertCommandText.AppendLine(" ,@ApproverCd");
            insertCommandText.AppendLine(" ,@ApprovalStatus");
            insertCommandText.AppendLine(" ,@ApprovalDate");
            // 2013/09/25 アドオン対応：承認項目を追加 ISC:yshindo end
            insertCommandText.AppendLine(" ,@LogicalDelFlg");
            insertCommandText.AppendLine(" ,@LastUpdateDate");
            insertCommandText.AppendLine(" ,@CreateBy");
            insertCommandText.AppendLine(" ,@CreateDate");
            insertCommandText.AppendLine(" ,@UpdateBy");
            insertCommandText.AppendLine(" ,@UpdateDate)");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = insertCommandText.ToString();

                command.Parameters.AddRange(this.CreateInsSARCoverSheetMasterParameterListNew(command, coverSheetMasterDataset.CoverSheetMasterDataTable1[0]));
                this.SetCommonItemForInsertNew(command);

                ConnectionState prevState = this.Connection.State;

                // データベースアクセス処理。
                // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
        }

        /// <summary>
        /// ＳＡＲカバーシートマスタの更新
        /// </summary>
        /// <param name="coverSheetMasterDataset">カバーシート情報Ｄｓ</param>
        private void UpSARCoverSheetMaster1New(CoverSheetMasterDataset coverSheetMasterDataset)
        {
            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARCoverSheetMaster ");
            updateCommandText.AppendLine(" SET TotalPageQty = @TotalPageQty,");
            updateCommandText.AppendLine("  InputPageCount = @InputPageCount, ");
            updateCommandText.AppendLine("  LastUpdateDate = @LastUpdateDate, ");
            updateCommandText.AppendLine("  UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("  UpdateDate = @UpdateDate");
            updateCommandText.AppendLine("WHERE");
            updateCommandText.AppendLine(" CorporationCode = @CorporationCode");
            updateCommandText.AppendLine(" AND BatchNo = @BatchNo");
            updateCommandText.AppendLine(" AND StoreCd = @StoreCd");
            updateCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            updateCommandText.AppendLine(" AND BatchDate = @BatchDate");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (coverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpSARCoverSheetMasterParameterListNew(command, coverSheetMasterDataset.CoverSheetMasterDataTable1[0], 1));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }

        /// <summary>
        /// ＳＡＲカバーシートマスタの各入力枚数更新
        /// </summary>
        /// <param name="coverSheetMasterDataset">カバーシート情報Ｄｓ</param>
        private void UpSARCoverSheetMaster2New(CoverSheetMasterDataset coverSheetMasterDataset)
        {
            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARCoverSheetMaster ");
            updateCommandText.AppendLine(" SET TotalPageQty = @TotalPageQty,");
            updateCommandText.AppendLine("  InputPageCount = @InputPageCount, ");
            updateCommandText.AppendLine("  ErrorNo = @ErrorNo, ");
            updateCommandText.AppendLine("  ApprovalCount = @ApprovalCount, ");
            updateCommandText.AppendLine("  LastUpdateDate = @LastUpdateDate, ");
            updateCommandText.AppendLine("  UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("  UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine("WHERE");
            updateCommandText.AppendLine(" CorporationCode = @CorporationCode");
            updateCommandText.AppendLine(" AND BatchNo = @BatchNo");
            updateCommandText.AppendLine(" AND StoreCd = @StoreCd");
            updateCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            updateCommandText.AppendLine(" AND BatchDate = @BatchDate");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (coverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();
                    command.Parameters.AddRange(this.CreateUpSARCoverSheetMasterParameterListNew(command, coverSheetMasterDataset.CoverSheetMasterDataTable1[0], 2));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }

        /// <summary>
        /// ＳＡＲ伝票ヘッダエラーを登録する。
        /// </summary>
        /// <param name="slipHeaderDataSet">伝票ヘッダDataSet</param>
        private void InsertSARSlipHeaderErrorNew(SARSlipHeaderErrorInfoDataset slipHeaderDataSet)
        {
            StringBuilder insertCommandText = new StringBuilder();
            insertCommandText.AppendLine(" INSERT INTO SARSlipHeaderError(");
            insertCommandText.AppendLine(" CorporationCode");
            insertCommandText.AppendLine(" ,InputMediaDiv");
            insertCommandText.AppendLine(" ,InputBatchNo");
            insertCommandText.AppendLine(" ,InputSequenceNo");
            insertCommandText.AppendLine(" ,InputDate");
            insertCommandText.AppendLine(" ,InputTime");
            insertCommandText.AppendLine(" ,DataType");
            insertCommandText.AppendLine(" ,UptakeSequenceNo");
            insertCommandText.AppendLine(" ,SlipDiv");
            insertCommandText.AppendLine(" ,SlipTypeCd");
            insertCommandText.AppendLine(" ,SlipNo");
            insertCommandText.AppendLine(" ,DisposalChangeDate");
            insertCommandText.AppendLine(" ,StoreCd");
            insertCommandText.AppendLine(" ,Category1Cd");
            insertCommandText.AppendLine(" ,Category2Cd");
            insertCommandText.AppendLine(" ,TotalQty");
            insertCommandText.AppendLine(" ,SlipOldTotalPrice");
            insertCommandText.AppendLine(" ,SlipNewTotalPrice");
            insertCommandText.AppendLine(" ,AutoSalesChangeFlg");
            insertCommandText.AppendLine(" ,SlipTotalDisposalAmt");
            insertCommandText.AppendLine(" ,SlipDate");
            insertCommandText.AppendLine(" ,ApprovalRequester");
            insertCommandText.AppendLine(" ,RequestApprovalDate");
            insertCommandText.AppendLine(" ,ApproverCd");
            insertCommandText.AppendLine(" ,ApprovalStatus");
            insertCommandText.AppendLine(" ,ApprovalDate");
            insertCommandText.AppendLine(" ,ErrorDiv");
            insertCommandText.AppendLine(" ,ErrorMessage");
            insertCommandText.AppendLine(" ,CorrectionDiv");
            insertCommandText.AppendLine(" ,FixDate");
            insertCommandText.AppendLine(" ,Attribute01");
            insertCommandText.AppendLine(" ,Attribute02");
            insertCommandText.AppendLine(" ,Attribute03");
            insertCommandText.AppendLine(" ,Attribute04");
            insertCommandText.AppendLine(" ,Attribute05");
            insertCommandText.AppendLine(" ,Attribute06");
            insertCommandText.AppendLine(" ,Attribute07");
            insertCommandText.AppendLine(" ,Attribute08");
            insertCommandText.AppendLine(" ,Attribute09");
            insertCommandText.AppendLine(" ,Attribute10");
            insertCommandText.AppendLine(" ,LogicalDelFlg");
            insertCommandText.AppendLine(" ,CreateBy");
            insertCommandText.AppendLine(" ,CreateDate");
            insertCommandText.AppendLine(" ,UpdateBy");
            insertCommandText.AppendLine(" ,UpdateDate)");
            insertCommandText.AppendLine("VALUES(");
            insertCommandText.AppendLine(" @CorporationCode");
            insertCommandText.AppendLine(" ,@InputMediaDiv");
            insertCommandText.AppendLine(" ,@InputBatchNo");
            insertCommandText.AppendLine(" ,@InputSequenceNo");
            insertCommandText.AppendLine(" ,@InputDate");
            insertCommandText.AppendLine(" ,@InputTime");
            insertCommandText.AppendLine(" ,@DataType");
            insertCommandText.AppendLine(" ,@UptakeSequenceNo");
            insertCommandText.AppendLine(" ,@SlipDiv");
            insertCommandText.AppendLine(" ,@SlipTypeCd");
            insertCommandText.AppendLine(" ,@SlipNo");
            insertCommandText.AppendLine(" ,@DisposalChangeDate");
            insertCommandText.AppendLine(" ,@StoreCd");
            insertCommandText.AppendLine(" ,@Category1Cd");
            insertCommandText.AppendLine(" ,@Category2Cd");
            insertCommandText.AppendLine(" ,@TotalQty");
            insertCommandText.AppendLine(" ,@SlipOldTotalPrice");
            insertCommandText.AppendLine(" ,@SlipNewTotalPrice");
            insertCommandText.AppendLine(" ,@AutoSalesChangeFlg");
            insertCommandText.AppendLine(" ,@SlipTotalDisposalAmt");
            insertCommandText.AppendLine(" ,@SlipDate");
            insertCommandText.AppendLine(" ,@ApprovalRequester");
            insertCommandText.AppendLine(" ,@RequestApprovalDate");
            insertCommandText.AppendLine(" ,@ApproverCd");
            insertCommandText.AppendLine(" ,@ApprovalStatus");
            insertCommandText.AppendLine(" ,@ApprovalDate");
            insertCommandText.AppendLine(" ,@ErrorDiv");
            insertCommandText.AppendLine(" ,@ErrorMessage");
            insertCommandText.AppendLine(" ,@CorrectionDiv");
            insertCommandText.AppendLine(" ,@FixDate");
            insertCommandText.AppendLine(" ,@Attribute01");
            insertCommandText.AppendLine(" ,@Attribute02");
            insertCommandText.AppendLine(" ,@Attribute03");
            insertCommandText.AppendLine(" ,@Attribute04");
            insertCommandText.AppendLine(" ,@Attribute05");
            insertCommandText.AppendLine(" ,@Attribute06");
            insertCommandText.AppendLine(" ,@Attribute07");
            insertCommandText.AppendLine(" ,@Attribute08");
            insertCommandText.AppendLine(" ,@Attribute09");
            insertCommandText.AppendLine(" ,@Attribute10");
            insertCommandText.AppendLine(" ,'0'");
            insertCommandText.AppendLine(" ,@CreateBy");
            insertCommandText.AppendLine(" ,@CreateDate");
            insertCommandText.AppendLine(" ,@UpdateBy");
            insertCommandText.AppendLine(" ,@UpdateDate)");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = insertCommandText.ToString();

                command.Parameters.AddRange(this.CreateInsertSARSlipHeaderErrorParameterListNew(command, slipHeaderDataSet.SARSlipHeaderError[0]));
                this.SetCommonItemForInsertNew(command);

                ConnectionState prevState = this.Connection.State;

                // データベースアクセス処理。
                // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
        }

        /// <summary>
        /// ＳＡＲ伝票ヘッダエラーを更新する。
        /// </summary>
        /// <param name="slipHeaderDataSet">伝票ヘッダDataSet</param>
        private void UpdateSARSlipHeaderErrorNew(SARSlipHeaderErrorInfoDataset slipHeaderDataSet)
        {
            StringBuilder updateCommandText = new StringBuilder();
            updateCommandText.AppendLine(" UPDATE SARSlipHeaderError ");
            updateCommandText.AppendLine(" SET Category1Cd = @Category1Cd,");
            updateCommandText.AppendLine("   Category2Cd = @Category2Cd,");
            updateCommandText.AppendLine("   TotalQty = @TotalQty,");
            updateCommandText.AppendLine("   SlipOldTotalPrice = @SlipOldTotalPrice,");
            updateCommandText.AppendLine("   SlipNewTotalPrice = @SlipNewTotalPrice,");
            updateCommandText.AppendLine("   AutoSalesChangeFlg = @AutoSalesChangeFlg,");
            updateCommandText.AppendLine("   SlipTotalDisposalAmt = @SlipTotalDisposalAmt, ");
            updateCommandText.AppendLine("   SlipDate = @SlipDate,");
            updateCommandText.AppendLine("   ApprovalRequester = @ApprovalRequester,");
            updateCommandText.AppendLine("   RequestApprovalDate = @RequestApprovalDate,");
            updateCommandText.AppendLine("   ApproverCd = @ApproverCd,");
            updateCommandText.AppendLine("   ApprovalStatus = @ApprovalStatus,");
            updateCommandText.AppendLine("   ApprovalDate = @ApprovalDate,");
            updateCommandText.AppendLine("   ErrorDiv = @ErrorDiv,");
            updateCommandText.AppendLine("   ErrorMessage = @ErrorMessage,");
            updateCommandText.AppendLine("   CorrectionDiv = @CorrectionDiv,");
            updateCommandText.AppendLine("   FixDate = @FixDate,");
            updateCommandText.AppendLine("   Attribute01 = @Attribute01,");
            updateCommandText.AppendLine("   Attribute02 = @Attribute02,");
            updateCommandText.AppendLine("   Attribute03 = @Attribute03,");
            updateCommandText.AppendLine("   Attribute04 = @Attribute04,");
            updateCommandText.AppendLine("   Attribute05 = @Attribute05,");
            updateCommandText.AppendLine("   Attribute06 = @Attribute06,");
            updateCommandText.AppendLine("   Attribute07 = @Attribute07,");
            updateCommandText.AppendLine("   Attribute08 = @Attribute08,");
            updateCommandText.AppendLine("   Attribute09 = @Attribute09,");
            updateCommandText.AppendLine("   Attribute10 = @Attribute10,");
            updateCommandText.AppendLine("   UpdateBy = @UpdateBy, ");
            updateCommandText.AppendLine("   UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine("WHERE");
            updateCommandText.AppendLine(" CorporationCode = @CorporationCode");
            updateCommandText.AppendLine(" AND InputMediaDiv = @InputMediaDiv");
            updateCommandText.AppendLine(" AND InputBatchNo = @InputBatchNo");
            updateCommandText.AppendLine(" AND InputSequenceNo = @InputSequenceNo");
            updateCommandText.AppendLine(" AND InputDate = @InputDate");
            updateCommandText.AppendLine(" AND InputTime = @InputTime");
            updateCommandText.AppendLine(" AND DataType = @DataType");
            updateCommandText.AppendLine(" AND UptakeSequenceNo = @UptakeSequenceNo");
            updateCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            updateCommandText.AppendLine(" AND SlipNo = @SlipNo");
            updateCommandText.AppendLine(" AND DisposalChangeDate = @DisposalChangeDate");
            updateCommandText.AppendLine(" AND StoreCd = @StoreCd");

            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (slipHeaderDataSet.SARSlipHeaderError.Rows.Count > 0)
                {
                    command.CommandText = updateCommandText.ToString();

                    command.Parameters.AddRange(this.CreateUpdateSARSlipHeaderErrorParameterListNew(command, slipHeaderDataSet.SARSlipHeaderError[0]));
                    this.SetCommonItemForUpdateNew(command);

                    ConnectionState prevState = this.Connection.State;

                    // データベースアクセス処理。
                    // 必ず try-finally 句で囲い、コネクションの解放処理を行う。
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
            }
        }

        /// <summary>
        /// 伝票番号枝番以外のＫｅｙで最大値を取得する。
        /// </summary>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <returns>次の伝票番号枝番</returns>
        private int GetNextSlipBranchNoNew(SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow)
        {
            int nextSlipBranchNo = 0;
            StringBuilder sqlQueryText = new StringBuilder();

            sqlQueryText.AppendLine("   SELECT (ISNULL(MAX(SlipBranchNo), 0) + 1)  AS NextSlipBranchNo");
            sqlQueryText.AppendLine("   FROM SARSlipHeader ");
            sqlQueryText.AppendLine("   WHERE CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine("      AND SlipDiv = @SlipDiv");
            sqlQueryText.AppendLine("      AND SlipTypeCd = @SlipTypeCd");
            sqlQueryText.AppendLine("      AND SlipNo = @SlipNo");
            sqlQueryText.AppendLine("      AND DisposalChangeDate = @DisposalChangeDate");
            sqlQueryText.AppendLine("      AND StoreCd = @StoreCd");

            // コマンドの作成（Command）
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成（Connection）
                dbCommand.Connection = this.Connection;
                dbCommand.CommandText = sqlQueryText.ToString();
                dbCommand.Parameters.AddRange(this.CreateGetNextSlipBranchNoParameterListNew(dbCommand, slipHeaderErrorRow));

                ConnectionState prevState = this.Connection.State;
                try
                {
                    if (this.Connection.State != ConnectionState.Open)
                    {
                        this.Connection.Open();
                    }

                    // 取得件数を戻り値に設定する。
                    DbDataReader reader = dbCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        nextSlipBranchNo = (int)reader["NextSlipBranchNo"];
                    }
                }
                finally
                {
                    if (prevState == ConnectionState.Closed)
                    {
                        this.Connection.Close();
                    }
                }
            }

            return nextSlipBranchNo;
        }

        /// <summary>
        /// 総枚数を取得する。
        /// </summary>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>総枚数</returns>
        private int GetTotalPageQtyNew(DsEntity dsEntity)
        {
            int totalPageQty = 0;
            StringBuilder sqlQueryText = new StringBuilder();

            // 2012.06.26 Start カバーシートへの更新情報収集方法を修正
            //sqlQueryText.AppendLine("   SELECT COUNT(*) AS SheetQty, '総枚数' AS SheetQtyType,");
            //sqlQueryText.AppendLine("       CorporationCode, InputBatchNo, StoreCd, SlipTypeCd, SlipDiv, DisposalChangeDate");
            //sqlQueryText.AppendLine("   FROM SARSlipHeaderError ");
            //sqlQueryText.AppendLine("   WHERE CorporationCode = @CorporationCode");
            //sqlQueryText.AppendLine("      AND InputBatchNo = @InputBatchNo");
            //sqlQueryText.AppendLine("      AND StoreCd = @StoreCd");
            //sqlQueryText.AppendLine("      AND SlipTypeCd = @SlipTypeCd");
            //sqlQueryText.AppendLine("      AND SlipDiv = @SlipDiv");
            //sqlQueryText.AppendLine("      AND DisposalChangeDate = @DisposalChangeDate");
            //sqlQueryText.AppendLine("      AND LogicalDelFlg = '0'");
            //sqlQueryText.AppendLine("   GROUP BY SlipNo, CorporationCode, InputBatchNo, StoreCd, SlipTypeCd, SlipDiv, DisposalChangeDate");

            sqlQueryText.AppendLine(" SELECT   COUNT(*) AS SheetQty ");
            sqlQueryText.AppendLine("         ,CorporationCode ");
            sqlQueryText.AppendLine("         ,InputBatchNo ");
            sqlQueryText.AppendLine("         ,SlipTypeCd ");
            sqlQueryText.AppendLine("         ,SlipDiv ");
            sqlQueryText.AppendLine(" FROM     SARSlipHeaderError WITH(NOLOCK) ");
            sqlQueryText.AppendLine(" WHERE EXISTS");
            sqlQueryText.AppendLine(" ( SELECT SHEV.*");
            sqlQueryText.AppendLine("   FROM ( SELECT    CorporationCode");
            sqlQueryText.AppendLine("                   ,InputBatchNo");
            sqlQueryText.AppendLine("                   ,InputMediaDiv");
            sqlQueryText.AppendLine("                   ,SlipTypeCd");
            sqlQueryText.AppendLine("                   ,SlipDiv");
            sqlQueryText.AppendLine("                   ,SlipNo");
            // 2014.04.30 Start 入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            //sqlQueryText.AppendLine("                   ,MAX(InputSequenceNo) AS InputSequenceNo");
            sqlQueryText.AppendLine("                   ,MAX(CAST(InputSequenceNo As numeric)) AS InputSequenceNo");
            // 2014.04.30 End   入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            sqlQueryText.AppendLine("          FROM      SARSlipHeaderError WITH(NOLOCK)");
            sqlQueryText.AppendLine("          WHERE     CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine("          AND       InputBatchNo = @InputBatchNo");
            sqlQueryText.AppendLine("          AND       LogicalDelFlg = '0'");
            sqlQueryText.AppendLine("          GROUP BY  CorporationCode");
            sqlQueryText.AppendLine("                   ,InputBatchNo");
            sqlQueryText.AppendLine("                   ,InputMediaDiv");
            sqlQueryText.AppendLine("                   ,SlipTypeCd");
            sqlQueryText.AppendLine("                   ,SlipDiv");
            sqlQueryText.AppendLine("                   ,SlipNo");
            sqlQueryText.AppendLine("        ) AS SHEV");
            sqlQueryText.AppendLine("   WHERE  SARSlipHeaderError.CorporationCode = SHEV.CorporationCode");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.InputBatchNo = SHEV.InputBatchNo");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipTypeCd = SHEV.SlipTypeCd");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipDiv = SHEV.SlipDiv");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipNo = SHEV.SlipNo");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.InputSequenceNo = SHEV.InputSequenceNo");
            sqlQueryText.AppendLine(" )");
            sqlQueryText.AppendLine(" GROUP BY CorporationCode");
            sqlQueryText.AppendLine("         ,InputBatchNo ");
            sqlQueryText.AppendLine("         ,SlipTypeCd ");
            sqlQueryText.AppendLine("         ,SlipDiv ");
            // 2012.06.26 End   カバーシートへの更新情報収集方法を修正

            // コマンドの作成（Command）
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成（Connection）
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    dbCommand.Connection = this.Connection;
                    dbCommand.CommandText = sqlQueryText.ToString();
                    dbCommand.Parameters.AddRange(this.CreateFillSARCoverSheetMasterForUpdateParameterListNew(dbCommand, dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0]));

                    ConnectionState prevState = this.Connection.State;
                    try
                    {
                        if (this.Connection.State != ConnectionState.Open)
                        {
                            this.Connection.Open();
                        }

                        // 取得件数を戻り値に設定する。
                        DbDataReader reader = dbCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            totalPageQty = (int)reader["SheetQty"];
                        }
                    }
                    finally
                    {
                        if (prevState == ConnectionState.Closed)
                        {
                            this.Connection.Close();
                        }
                    }
                }
            }

            return totalPageQty;
        }

        /// <summary>
        /// 入力枚数を取得する。
        /// </summary>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>入力枚数</returns>
        private int GetInputPageCountNew(DsEntity dsEntity)
        {
            int inputPageCount = 0;
            StringBuilder sqlQueryText = new StringBuilder();

            // 2012.06.26 Start カバーシートへの更新情報収集方法を修正
            //sqlQueryText.AppendLine("   SELECT COUNT(*) AS SheetQty, '入力枚数' AS SheetQtyType,");
            //sqlQueryText.AppendLine("       CorporationCode, InputBatchNo, StoreCd, SlipTypeCd, SlipDiv, DisposalChangeDate");
            //sqlQueryText.AppendLine("   FROM SARSlipHeaderError ");
            //sqlQueryText.AppendLine("   WHERE CorporationCode = @CorporationCode");
            //sqlQueryText.AppendLine("      AND InputBatchNo = @InputBatchNo");
            //sqlQueryText.AppendLine("      AND StoreCd = @StoreCd");
            //sqlQueryText.AppendLine("      AND SlipTypeCd = @SlipTypeCd");
            //sqlQueryText.AppendLine("      AND SlipDiv = @SlipDiv");
            //sqlQueryText.AppendLine("      AND DisposalChangeDate = @DisposalChangeDate");
            //sqlQueryText.AppendLine("      AND LogicalDelFlg = '0'");
            //sqlQueryText.AppendLine("   GROUP BY SlipNo, CorporationCode, InputBatchNo, StoreCd, SlipTypeCd, SlipDiv, DisposalChangeDate");

            sqlQueryText.AppendLine(" SELECT   COUNT(*) AS SheetQty ");
            sqlQueryText.AppendLine("         ,CorporationCode ");
            sqlQueryText.AppendLine("         ,InputBatchNo ");
            sqlQueryText.AppendLine("         ,SlipTypeCd ");
            sqlQueryText.AppendLine("         ,SlipDiv ");
            sqlQueryText.AppendLine(" FROM     SARSlipHeaderError WITH(NOLOCK) ");
            sqlQueryText.AppendLine(" WHERE EXISTS");
            sqlQueryText.AppendLine(" ( SELECT SHEV.*");
            sqlQueryText.AppendLine("   FROM ( SELECT    CorporationCode");
            sqlQueryText.AppendLine("                   ,InputBatchNo");
            sqlQueryText.AppendLine("                   ,InputMediaDiv");
            sqlQueryText.AppendLine("                   ,SlipTypeCd");
            sqlQueryText.AppendLine("                   ,SlipDiv");
            sqlQueryText.AppendLine("                   ,SlipNo");
            sqlQueryText.AppendLine("                   ,MAX(InputSequenceNo) AS InputSequenceNo");
            sqlQueryText.AppendLine("          FROM      SARSlipHeaderError WITH(NOLOCK)");
            sqlQueryText.AppendLine("          WHERE     CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine("          AND       InputBatchNo = @InputBatchNo");
            sqlQueryText.AppendLine("          AND       LogicalDelFlg = '0'");
            sqlQueryText.AppendLine("          GROUP BY  CorporationCode");
            sqlQueryText.AppendLine("                   ,InputBatchNo");
            sqlQueryText.AppendLine("                   ,InputMediaDiv");
            sqlQueryText.AppendLine("                   ,SlipTypeCd");
            sqlQueryText.AppendLine("                   ,SlipDiv");
            sqlQueryText.AppendLine("                   ,SlipNo");
            sqlQueryText.AppendLine("        ) AS SHEV");
            sqlQueryText.AppendLine("   WHERE  SARSlipHeaderError.CorporationCode = SHEV.CorporationCode");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.InputBatchNo = SHEV.InputBatchNo");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipTypeCd = SHEV.SlipTypeCd");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipDiv = SHEV.SlipDiv");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipNo = SHEV.SlipNo");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.InputSequenceNo = SHEV.InputSequenceNo");
            sqlQueryText.AppendLine(" )");
            sqlQueryText.AppendLine(" GROUP BY CorporationCode");
            sqlQueryText.AppendLine("         ,InputBatchNo ");
            sqlQueryText.AppendLine("         ,SlipTypeCd ");
            sqlQueryText.AppendLine("         ,SlipDiv ");
            // 2012.06.26 End   カバーシートへの更新情報収集方法を修正

            // コマンドの作成（Command）
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成（Connection）
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    dbCommand.Connection = this.Connection;
                    dbCommand.CommandText = sqlQueryText.ToString();
                    dbCommand.Parameters.AddRange(this.CreateFillSARCoverSheetMasterForUpdateParameterListNew(dbCommand, dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0]));

                    ConnectionState prevState = this.Connection.State;
                    try
                    {
                        if (this.Connection.State != ConnectionState.Open)
                        {
                            this.Connection.Open();
                        }

                        // 取得件数を戻り値に設定する。
                        DbDataReader reader = dbCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            inputPageCount = (int)reader["SheetQty"];
                        }
                    }
                    finally
                    {
                        if (prevState == ConnectionState.Closed)
                        {
                            this.Connection.Close();
                        }
                    }
                }
            }

            return inputPageCount;
        }

        /// <summary>
        /// エラー枚数を取得する。
        /// </summary>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>エラー枚数</returns>
        private int GetErrorNoNew(DsEntity dsEntity)
        {
            int errorNo = 0;
            StringBuilder sqlQueryText = new StringBuilder();

            // 2012.06.26 Start カバーシートへの更新情報収集方法を修正
            //sqlQueryText.AppendLine("   SELECT COUNT(*) AS SheetQty, 'エラー枚数' AS SheetQtyType,");
            //sqlQueryText.AppendLine("       CorporationCode, InputBatchNo, StoreCd, SlipTypeCd, SlipDiv, DisposalChangeDate");
            //sqlQueryText.AppendLine("   FROM SARSlipHeaderError ");
            //sqlQueryText.AppendLine("   WHERE CorporationCode = @CorporationCode");
            //sqlQueryText.AppendLine("      AND InputBatchNo = @InputBatchNo");
            //sqlQueryText.AppendLine("      AND StoreCd = @StoreCd");
            //sqlQueryText.AppendLine("      AND SlipTypeCd = @SlipTypeCd");
            //sqlQueryText.AppendLine("      AND SlipDiv = @SlipDiv");
            //sqlQueryText.AppendLine("      AND DisposalChangeDate = @DisposalChangeDate");
            //sqlQueryText.AppendLine("      AND LogicalDelFlg = '0'");
            //sqlQueryText.AppendLine("      AND ErrorDiv <> '0'");
            //sqlQueryText.AppendLine("   GROUP BY SlipNo, CorporationCode, InputBatchNo, StoreCd, SlipTypeCd, SlipDiv, DisposalChangeDate");

            sqlQueryText.AppendLine(" SELECT   COUNT(*) AS SheetQty ");
            sqlQueryText.AppendLine("         ,CorporationCode ");
            sqlQueryText.AppendLine("         ,InputBatchNo ");
            sqlQueryText.AppendLine("         ,SlipTypeCd ");
            sqlQueryText.AppendLine("         ,SlipDiv ");
            sqlQueryText.AppendLine(" FROM     SARSlipHeaderError WITH(NOLOCK) ");
            sqlQueryText.AppendLine(" WHERE EXISTS");
            sqlQueryText.AppendLine(" ( SELECT SHEV.*");
            sqlQueryText.AppendLine("   FROM ( SELECT    CorporationCode");
            sqlQueryText.AppendLine("                   ,InputBatchNo");
            sqlQueryText.AppendLine("                   ,InputMediaDiv");
            sqlQueryText.AppendLine("                   ,SlipTypeCd");
            sqlQueryText.AppendLine("                   ,SlipDiv");
            sqlQueryText.AppendLine("                   ,SlipNo");
            // 2014.04.30 Start 入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            //sqlQueryText.AppendLine("                   ,MAX(InputSequenceNo) AS InputSequenceNo");
            sqlQueryText.AppendLine("                   ,MAX(CAST(InputSequenceNo As numeric)) AS InputSequenceNo");
            // 2014.04.30 End   入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            sqlQueryText.AppendLine("          FROM      SARSlipHeaderError WITH(NOLOCK)");
            sqlQueryText.AppendLine("          WHERE     CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine("          AND       InputBatchNo = @InputBatchNo");
            sqlQueryText.AppendLine("          AND       LogicalDelFlg = '0'");
            sqlQueryText.AppendLine("          AND       ErrorDiv <> '0'");
            sqlQueryText.AppendLine("          GROUP BY  CorporationCode");
            sqlQueryText.AppendLine("                   ,InputBatchNo");
            sqlQueryText.AppendLine("                   ,InputMediaDiv");
            sqlQueryText.AppendLine("                   ,SlipTypeCd");
            sqlQueryText.AppendLine("                   ,SlipDiv");
            sqlQueryText.AppendLine("                   ,SlipNo");
            sqlQueryText.AppendLine("        ) AS SHEV");
            sqlQueryText.AppendLine("   WHERE  SARSlipHeaderError.CorporationCode = SHEV.CorporationCode");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.InputBatchNo = SHEV.InputBatchNo");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipTypeCd = SHEV.SlipTypeCd");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipDiv = SHEV.SlipDiv");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipNo = SHEV.SlipNo");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.InputSequenceNo = SHEV.InputSequenceNo");
            sqlQueryText.AppendLine(" )");
            sqlQueryText.AppendLine(" GROUP BY CorporationCode");
            sqlQueryText.AppendLine("         ,InputBatchNo ");
            sqlQueryText.AppendLine("         ,SlipTypeCd ");
            sqlQueryText.AppendLine("         ,SlipDiv ");
            // 2012.06.26 End   カバーシートへの更新情報収集方法を修正

            // コマンドの作成（Command）
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成（Connection）
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    dbCommand.Connection = this.Connection;
                    dbCommand.CommandText = sqlQueryText.ToString();
                    dbCommand.Parameters.AddRange(this.CreateFillSARCoverSheetMasterForUpdateParameterListNew(dbCommand, dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0]));

                    ConnectionState prevState = this.Connection.State;
                    try
                    {
                        if (this.Connection.State != ConnectionState.Open)
                        {
                            this.Connection.Open();
                        }

                        // 取得件数を戻り値に設定する。
                        DbDataReader reader = dbCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            errorNo = (int)reader["SheetQty"];
                        }
                    }
                    finally
                    {
                        if (prevState == ConnectionState.Closed)
                        {
                            this.Connection.Close();
                        }
                    }
                }
            }

            return errorNo;
        }

        /// <summary>
        /// 承認枚数を取得する。
        /// </summary>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>承認枚数</returns>
        private int GetApprovalCountNew(DsEntity dsEntity)
        {
            int approvalCount = 0;
            StringBuilder sqlQueryText = new StringBuilder();

            // 2012.06.26 Start カバーシートへの更新情報収集方法を修正
            //sqlQueryText.AppendLine("   SELECT COUNT(*) AS SheetQty, '承認枚数' AS SheetQtyType,");
            //sqlQueryText.AppendLine("       CorporationCode, InputBatchNo, StoreCd, SlipTypeCd, SlipDiv, DisposalChangeDate");
            //sqlQueryText.AppendLine("   FROM SARSlipHeaderError ");
            //sqlQueryText.AppendLine("   WHERE CorporationCode = @CorporationCode");
            //sqlQueryText.AppendLine("      AND InputBatchNo = @InputBatchNo");
            //sqlQueryText.AppendLine("      AND StoreCd = @StoreCd");
            //sqlQueryText.AppendLine("      AND SlipTypeCd = @SlipTypeCd");
            //sqlQueryText.AppendLine("      AND SlipDiv = @SlipDiv");
            //sqlQueryText.AppendLine("      AND DisposalChangeDate = @DisposalChangeDate");
            //sqlQueryText.AppendLine("      AND LogicalDelFlg = '0'");
            //sqlQueryText.AppendLine("      AND ErrorDiv = '0'");
            //sqlQueryText.AppendLine("   GROUP BY SlipNo, CorporationCode, InputBatchNo, StoreCd, SlipTypeCd, SlipDiv, DisposalChangeDate");

            sqlQueryText.AppendLine(" SELECT   COUNT(*) AS SheetQty ");
            sqlQueryText.AppendLine("         ,CorporationCode ");
            sqlQueryText.AppendLine("         ,InputBatchNo ");
            sqlQueryText.AppendLine("         ,SlipTypeCd ");
            sqlQueryText.AppendLine("         ,SlipDiv ");
            sqlQueryText.AppendLine(" FROM     SARSlipHeaderError WITH(NOLOCK) ");
            sqlQueryText.AppendLine(" WHERE EXISTS");
            sqlQueryText.AppendLine(" ( SELECT SHEV.*");
            sqlQueryText.AppendLine("   FROM ( SELECT    CorporationCode");
            sqlQueryText.AppendLine("                   ,InputBatchNo");
            sqlQueryText.AppendLine("                   ,InputMediaDiv");
            sqlQueryText.AppendLine("                   ,SlipTypeCd");
            sqlQueryText.AppendLine("                   ,SlipDiv");
            sqlQueryText.AppendLine("                   ,SlipNo");
            // 2014.04.30 Start 入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            //sqlQueryText.AppendLine("                   ,MAX(InputSequenceNo) AS InputSequenceNo");
            sqlQueryText.AppendLine("                   ,MAX(CAST(InputSequenceNo As numeric)) AS InputSequenceNo");
            // 2014.04.30 End   入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            sqlQueryText.AppendLine("          FROM      SARSlipHeaderError WITH(NOLOCK)");
            sqlQueryText.AppendLine("          WHERE     CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine("          AND       InputBatchNo = @InputBatchNo");
            // 2013.12.02 承認取消を行っても、カバーシートの承認枚数には以前承認された枚数が更新される不具合修正 START
            //sqlQueryText.AppendLine("          AND       ApprovalStatus = '3'");
            // 2013.12.02 承認取消を行っても、カバーシートの承認枚数には以前承認された枚数が更新される不具合修正 END
            sqlQueryText.AppendLine("          AND       LogicalDelFlg = '0'");
            sqlQueryText.AppendLine("          AND       ErrorDiv = '0'");
            sqlQueryText.AppendLine("          GROUP BY  CorporationCode");
            sqlQueryText.AppendLine("                   ,InputBatchNo");
            sqlQueryText.AppendLine("                   ,InputMediaDiv");
            sqlQueryText.AppendLine("                   ,SlipTypeCd");
            sqlQueryText.AppendLine("                   ,SlipDiv");
            sqlQueryText.AppendLine("                   ,SlipNo");
            sqlQueryText.AppendLine("        ) AS SHEV");
            sqlQueryText.AppendLine("   WHERE  SARSlipHeaderError.CorporationCode = SHEV.CorporationCode");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.InputBatchNo = SHEV.InputBatchNo");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipTypeCd = SHEV.SlipTypeCd");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipDiv = SHEV.SlipDiv");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.SlipNo = SHEV.SlipNo");
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.InputSequenceNo = SHEV.InputSequenceNo");
            // 2013.12.02 承認取消を行っても、カバーシートの承認枚数には以前承認された枚数が更新される不具合修正 START
            sqlQueryText.AppendLine("   AND    SARSlipHeaderError.ApprovalStatus = '3'");
            // 2013.12.02 承認取消を行っても、カバーシートの承認枚数には以前承認された枚数が更新される不具合修正 END
            sqlQueryText.AppendLine(" )");
            sqlQueryText.AppendLine(" GROUP BY CorporationCode");
            sqlQueryText.AppendLine("         ,InputBatchNo ");
            sqlQueryText.AppendLine("         ,SlipTypeCd ");
            sqlQueryText.AppendLine("         ,SlipDiv ");
            // 2012.06.26 End   カバーシートへの更新情報収集方法を修正

            // コマンドの作成（Command）
            using (DbCommand dbCommand = this.Connection.CreateCommand())
            {
                // コネクションの作成（Connection）
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    dbCommand.Connection = this.Connection;
                    dbCommand.CommandText = sqlQueryText.ToString();
                    dbCommand.Parameters.AddRange(this.CreateFillSARCoverSheetMasterForUpdateParameterListNew(dbCommand, dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0]));

                    ConnectionState prevState = this.Connection.State;
                    try
                    {
                        if (this.Connection.State != ConnectionState.Open)
                        {
                            this.Connection.Open();
                        }

                        // 取得件数を戻り値に設定する。
                        DbDataReader reader = dbCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            approvalCount = (int)reader["SheetQty"];
                        }
                    }
                    finally
                    {
                        if (prevState == ConnectionState.Closed)
                        {
                            this.Connection.Close();
                        }
                    }
                }
            }

            return approvalCount;
        }

        /// <summary>
        /// バッチ番号チェックするためのクエリを作成します。
        /// </summary>
        /// <returns>Query string</returns>
        private string CreateChkBatchNoFillQueryNew()
        {
            // 引数の店舗検索条件EntityからSQLを動的に組み立てる。DbParameter が使える場合は極力使用する。
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("SELECT");
            sqlBuilder.AppendLine(" CorporationCode");
            sqlBuilder.AppendLine(" ,BatchNo");
            sqlBuilder.AppendLine(" ,StoreCd");
            sqlBuilder.AppendLine(" ,SlipTypeCd");
            sqlBuilder.AppendLine(" ,SlipDiv");
            sqlBuilder.AppendLine(" ,BatchDate");
            sqlBuilder.AppendLine(" ,TotalPageQty");
            sqlBuilder.AppendLine(" ,InputPageCount");
            sqlBuilder.AppendLine(" ,ErrorNo");
            sqlBuilder.AppendLine(" ,ApprovalCount");
            sqlBuilder.AppendLine(" ,PersonnelInput");
            sqlBuilder.AppendLine(" ,LastUpdateDate");
            sqlBuilder.AppendLine(" ,ApprovalRequester");
            sqlBuilder.AppendLine(" ,RequestApprovalDate");
            sqlBuilder.AppendLine(" ,ApproverCd");
            sqlBuilder.AppendLine(" ,ApprovalStatus");
            sqlBuilder.AppendLine(" ,ApprovalDate");
            sqlBuilder.AppendLine(" ,LogicalDelDate");
            sqlBuilder.AppendLine(" ,LogicalDelFlg");
            sqlBuilder.AppendLine(" ,CreateBy");
            sqlBuilder.AppendLine(" ,CreateDate");
            sqlBuilder.AppendLine(" ,UpdateBy");
            sqlBuilder.AppendLine(" ,UpdateDate");
            sqlBuilder.AppendLine(" ,SystemRowversion");
            sqlBuilder.AppendLine("FROM");
            sqlBuilder.AppendLine(" SARCoverSheetMaster");
            sqlBuilder.AppendLine("WHERE");
            sqlBuilder.AppendLine(" CorporationCode = @CorporationCode");
            sqlBuilder.AppendLine(" AND BatchNo = @BatchNo");
            // 2012.06.26 Start バッチ番号チェックからバッチ店舗を外し法人、伝票種別、伝票区分で一意とする
            //sqlBuilder.AppendLine(" AND StoreCd = @StoreCd");
            // 2012.06.26 End   バッチ番号チェックからバッチ店舗を外し法人、伝票種別、伝票区分で一意とする
            sqlBuilder.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            sqlBuilder.AppendLine(" AND SlipDiv = @SlipDiv");
#warning SKN臨時対応　抽出条件から一時的にコメント
            //sqlBuilder.AppendLine(" AND BatchDate = @BatchDate");

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 伝票ヘッダ検索するためのクエリを作成します。
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>Query string</returns>
        private string CreateFillSARSlipHeaderErrorFillQueryNew(WorkEntity workEntity)
        {
            // 引数の店舗検索条件EntityからSQLを動的に組み立てる。DbParameter が使える場合は極力使用する。
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("SELECT");
            sqlBuilder.AppendLine("    CorporationCode");
            sqlBuilder.AppendLine("    ,InputMediaDiv");
            sqlBuilder.AppendLine("    ,InputBatchNo");
            sqlBuilder.AppendLine("    ,InputSequenceNo");
            sqlBuilder.AppendLine("    ,InputDate");
            sqlBuilder.AppendLine("    ,InputTime");
            sqlBuilder.AppendLine("    ,DataType");
            sqlBuilder.AppendLine("    ,UptakeSequenceNo");
            sqlBuilder.AppendLine("    ,SlipDiv");
            sqlBuilder.AppendLine("    ,SlipTypeCd");
            sqlBuilder.AppendLine("    ,SlipNo");
            sqlBuilder.AppendLine("    ,DisposalChangeDate");
            sqlBuilder.AppendLine("    ,StoreCd");
            sqlBuilder.AppendLine("    ,Category1Cd");
            sqlBuilder.AppendLine("    ,Category2Cd");
            sqlBuilder.AppendLine("    ,TotalQty");
            sqlBuilder.AppendLine("    ,SlipOldTotalPrice");
            sqlBuilder.AppendLine("    ,SlipNewTotalPrice");
            sqlBuilder.AppendLine("    ,AutoSalesChangeFlg");
            sqlBuilder.AppendLine("    ,SlipTotalDisposalAmt");
            sqlBuilder.AppendLine("    ,SlipDate");
            sqlBuilder.AppendLine("    ,ApprovalRequester");
            sqlBuilder.AppendLine("    ,RequestApprovalDate");
            sqlBuilder.AppendLine("    ,ApproverCd");
            sqlBuilder.AppendLine("    ,ApprovalStatus");
            sqlBuilder.AppendLine("    ,ApprovalDate");
            sqlBuilder.AppendLine("    ,ErrorDiv");
            sqlBuilder.AppendLine("    ,ErrorMessage");
            sqlBuilder.AppendLine("    ,RedBlackDiv");
            sqlBuilder.AppendLine("    ,CorrectionDiv");
            sqlBuilder.AppendLine("    ,FixDate");
            sqlBuilder.AppendLine("    ,Attribute01");
            sqlBuilder.AppendLine("    ,Attribute02");
            sqlBuilder.AppendLine("    ,Attribute03");
            sqlBuilder.AppendLine("    ,Attribute04");
            sqlBuilder.AppendLine("    ,Attribute05");
            sqlBuilder.AppendLine("    ,Attribute06");
            sqlBuilder.AppendLine("    ,Attribute07");
            sqlBuilder.AppendLine("    ,Attribute08");
            sqlBuilder.AppendLine("    ,Attribute09");
            sqlBuilder.AppendLine("    ,Attribute10");
            sqlBuilder.AppendLine("    ,LogicalDelDate");
            sqlBuilder.AppendLine("    ,LogicalDelFlg");
            sqlBuilder.AppendLine("    ,CreateBy");
            sqlBuilder.AppendLine("    ,CreateDate");
            sqlBuilder.AppendLine("    ,UpdateBy");
            sqlBuilder.AppendLine("    ,UpdateDate");
            sqlBuilder.AppendLine("    ,SystemRowversion");
            sqlBuilder.AppendLine("FROM");
            // 2012.06.26 Start 複数伝票登録し修正した場合に最大のInputSequenceNoの伝票しか取得されないのを修正
            //sqlBuilder.AppendLine(" SARSlipHeaderError");
            //sqlBuilder.AppendLine("WHERE");
            //sqlBuilder.AppendLine("  CorporationCode = @CorporationCode");
            //sqlBuilder.AppendLine("  AND InputMediaDiv = '1'");
            //sqlBuilder.AppendLine("  AND InputBatchNo = @InputBatchNo");
            //sqlBuilder.AppendLine("  AND InputSequenceNo = ");
            //sqlBuilder.AppendLine("         (SELECT ");
            //sqlBuilder.AppendLine("             MAX(InputSequenceNo) AS InputSequenceNo");
            //sqlBuilder.AppendLine("             FROM SARSlipHeaderError");
            //sqlBuilder.AppendLine("             WHERE CorporationCode = @CorporationCode");
            //sqlBuilder.AppendLine("                 AND InputMediaDiv = '1'");
            //sqlBuilder.AppendLine("                 AND InputBatchNo = @InputBatchNo)");
            //sqlBuilder.AppendLine("  AND LogicalDelFlg = '0'");

            //if (!string.IsNullOrEmpty(workEntity.TargetSlipNo))
            //{
            //    sqlBuilder.AppendLine("  AND SlipNo = @SlipNo");
            //}
            //else
            //{
            //    sqlBuilder.AppendLine("ORDER BY SlipNo ASC");
            //}

            sqlBuilder.AppendLine(" SARSlipHeaderError WITH(NOLOCK) ");
            sqlBuilder.AppendLine(" WHERE EXISTS");
            sqlBuilder.AppendLine(" ( SELECT SHEV.*");
            sqlBuilder.AppendLine("   FROM ( SELECT    CorporationCode");
            sqlBuilder.AppendLine("                   ,InputBatchNo");
            sqlBuilder.AppendLine("                   ,InputMediaDiv");
            sqlBuilder.AppendLine("                   ,SlipTypeCd");
            sqlBuilder.AppendLine("                   ,SlipDiv");
            sqlBuilder.AppendLine("                   ,SlipNo");
            // 2014.04.30 Start 入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            //sqlBuilder.AppendLine("                   ,MAX(InputSequenceNo) AS InputSequenceNo");
            sqlBuilder.AppendLine("                   ,MAX(CAST(InputSequenceNo As Numeric)) AS InputSequenceNo");
            // 2014.04.30 End   入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            sqlBuilder.AppendLine("          FROM      SARSlipHeaderError WITH(NOLOCK)");
            sqlBuilder.AppendLine("          WHERE     CorporationCode = @CorporationCode");
            // 8/9 SUEYOSHI 入力媒体区分は検索条件として不要の為削除 DEL START
            //sqlBuilder.AppendLine("          AND       InputMediaDiv = '1'");
            // 8/9 SUEYOSHI END
            sqlBuilder.AppendLine("          AND       InputBatchNo = @InputBatchNo");
            sqlBuilder.AppendLine("          AND       LogicalDelFlg = '0'");

            // 7/25 SUEYOSHI ADD
            sqlBuilder.AppendLine("          AND       SlipTypeCd = @SlipTypeCd");
            sqlBuilder.AppendLine("          AND       SlipDiv = @SlipDiv");
            // 7/25 SUEYOSHI END
            if (!string.IsNullOrEmpty(workEntity.TargetSlipNo))
            {
                sqlBuilder.AppendLine("      AND       SlipNo = @SlipNo");
            }
            sqlBuilder.AppendLine("          GROUP BY  CorporationCode");
            sqlBuilder.AppendLine("                   ,InputBatchNo");
            sqlBuilder.AppendLine("                   ,InputMediaDiv");
            sqlBuilder.AppendLine("                   ,SlipTypeCd");
            sqlBuilder.AppendLine("                   ,SlipDiv");
            sqlBuilder.AppendLine("                   ,SlipNo");
            sqlBuilder.AppendLine("        ) AS SHEV");
            sqlBuilder.AppendLine("   WHERE  SARSlipHeaderError.CorporationCode = SHEV.CorporationCode");
            sqlBuilder.AppendLine("   AND    SARSlipHeaderError.InputBatchNo = SHEV.InputBatchNo");
            sqlBuilder.AppendLine("   AND    SARSlipHeaderError.InputMediaDiv = SHEV.InputMediaDiv");
            sqlBuilder.AppendLine("   AND    SARSlipHeaderError.SlipTypeCd = SHEV.SlipTypeCd");
            sqlBuilder.AppendLine("   AND    SARSlipHeaderError.SlipDiv = SHEV.SlipDiv");
            sqlBuilder.AppendLine("   AND    SARSlipHeaderError.SlipNo = SHEV.SlipNo");
            sqlBuilder.AppendLine("   AND    SARSlipHeaderError.InputSequenceNo = SHEV.InputSequenceNo");
            sqlBuilder.AppendLine(" )");
            if (string.IsNullOrEmpty(workEntity.TargetSlipNo))
            {
                sqlBuilder.AppendLine("ORDER BY SlipNo ASC");
            }
            // 2012.06.26 End   複数伝票登録し修正した場合に最大のInputSequenceNoの伝票しか取得されないのを修正

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 伝票明細検索するためのクエリを作成します。
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>Query string</returns>
        private string CreateFillSARSlipDetailErrorFillQueryNew(WorkEntity workEntity)
        {
            // 引数の店舗検索条件EntityからSQLを動的に組み立てる。DbParameter が使える場合は極力使用する。
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("SELECT");
            sqlBuilder.AppendLine("    CorporationCode");
            sqlBuilder.AppendLine("    ,InputMediaDiv");
            sqlBuilder.AppendLine("    ,InputBatchNo");
            sqlBuilder.AppendLine("    ,InputSequenceNo");
            sqlBuilder.AppendLine("    ,InputSequenceLineNo");
            sqlBuilder.AppendLine("    ,InputDate");
            sqlBuilder.AppendLine("    ,InputTime");
            sqlBuilder.AppendLine("    ,DataType");
            sqlBuilder.AppendLine("    ,UptakeSequenceNo");
            sqlBuilder.AppendLine("    ,UptakeSequenceLineNo");
            sqlBuilder.AppendLine("    ,SlipLineNo");
            sqlBuilder.AppendLine("    ,StoreCd");
            sqlBuilder.AppendLine("    ,SlipTypeCd");
            sqlBuilder.AppendLine("    ,SlipDiv");
            sqlBuilder.AppendLine("    ,SlipNo");
            sqlBuilder.AppendLine("    ,DisposalChangeDate");
            sqlBuilder.AppendLine("    ,Category1Cd");
            sqlBuilder.AppendLine("    ,Category2Cd");
            sqlBuilder.AppendLine("    ,Category3Cd");
            sqlBuilder.AppendLine("    ,Category4Cd");
            sqlBuilder.AppendLine("    ,SKUCd");
            sqlBuilder.AppendLine("    ,ScanningCd");
            sqlBuilder.AppendLine("    ,DisposalChangeQty");
            // --> 2013/11/01 マスタ売価対応
            //// 2013.10.21 AST Chg Start
            //#region 修正前
            ////sqlBuilder.AppendLine("    ,SlipOldPrice");
            ////sqlBuilder.AppendLine("    ,SlipNewPrice");
            ////sqlBuilder.AppendLine("    ,SlipDisposalUnitPrica");
            //#endregion
            //sqlBuilder.AppendLine("    ,CONVERT(DECIMAL, CASE WHEN ISNULL(Attribute02, '') = '' THEN '0' ELSE Attribute02 END) AS SlipOldPrice");
            //sqlBuilder.AppendLine("    ,CONVERT(DECIMAL, CASE WHEN ISNULL(Attribute03, '') = '' THEN '0' ELSE Attribute03 END) AS SlipNewPrice");
            //sqlBuilder.AppendLine("    ,CONVERT(DECIMAL, CASE WHEN ISNULL(Attribute04, '') = '' THEN '0' ELSE Attribute04 END) AS SlipDisposalUnitPrica");
            //// 2013.10.21 AST Chg End
            sqlBuilder.AppendLine("    ,SlipOldPrice");
            sqlBuilder.AppendLine("    ,SlipNewPrice");
            sqlBuilder.AppendLine("    ,SlipDisposalUnitPrica");
            // <-- 2013/11/01 マスタ売価対応
            sqlBuilder.AppendLine("    ,SalesChangeDiv");
            sqlBuilder.AppendLine("    ,Reason");
            sqlBuilder.AppendLine("    ,SalesChangePrice");
            sqlBuilder.AppendLine("    ,DisposalPriceAmt");
            sqlBuilder.AppendLine("    ,ErrorDiv");
            sqlBuilder.AppendLine("    ,ErrorMessage");
            sqlBuilder.AppendLine("    ,Attribute01");
            sqlBuilder.AppendLine("    ,Attribute02");
            sqlBuilder.AppendLine("    ,Attribute03");
            sqlBuilder.AppendLine("    ,Attribute04");
            sqlBuilder.AppendLine("    ,Attribute05");
            sqlBuilder.AppendLine("    ,Attribute06");
            sqlBuilder.AppendLine("    ,Attribute07");
            sqlBuilder.AppendLine("    ,Attribute08");
            sqlBuilder.AppendLine("    ,Attribute09");
            sqlBuilder.AppendLine("    ,Attribute10");
            sqlBuilder.AppendLine("    ,LogicalDelDate");
            sqlBuilder.AppendLine("    ,LogicalDelFlg");
            sqlBuilder.AppendLine("    ,CreateBy");
            sqlBuilder.AppendLine("    ,CreateDate");
            sqlBuilder.AppendLine("    ,UpdateBy");
            sqlBuilder.AppendLine("    ,UpdateDate");
            sqlBuilder.AppendLine("    ,SystemRowversion");
            sqlBuilder.AppendLine("FROM");
            // 2012.06.26 Start 複数伝票登録し修正した場合に最大のInputSequenceNoの伝票しか取得されないのを修正
            //sqlBuilder.AppendLine(" SARSlipDetailError");
            //sqlBuilder.AppendLine("WHERE");
            //sqlBuilder.AppendLine("  CorporationCode = @CorporationCode");
            //sqlBuilder.AppendLine("  AND InputMediaDiv = '1'");
            //sqlBuilder.AppendLine("  AND InputBatchNo = @InputBatchNo");
            //sqlBuilder.AppendLine("  AND InputSequenceNo = ");
            //sqlBuilder.AppendLine("         (SELECT ");
            //sqlBuilder.AppendLine("             MAX(InputSequenceNo) AS InputSequenceNo");
            //sqlBuilder.AppendLine("             FROM SARSlipDetailError");
            //sqlBuilder.AppendLine("             WHERE CorporationCode = @CorporationCode");
            //sqlBuilder.AppendLine("                 AND InputMediaDiv = '1'");
            //sqlBuilder.AppendLine("                 AND InputBatchNo = @InputBatchNo)");
            //sqlBuilder.AppendLine("  AND LogicalDelFlg = '0'");

            //if (!string.IsNullOrEmpty(workEntity.TargetSlipNo))
            //{
            //    sqlBuilder.AppendLine("  AND SlipNo = @SlipNo");
            //}

            sqlBuilder.AppendLine(" SARSlipDetailError WITH(NOLOCK) ");
            sqlBuilder.AppendLine(" WHERE EXISTS");
            sqlBuilder.AppendLine(" ( SELECT SDEV.*");
            sqlBuilder.AppendLine("   FROM ( SELECT    CorporationCode");
            sqlBuilder.AppendLine("                   ,InputBatchNo");
            sqlBuilder.AppendLine("                   ,InputMediaDiv");
            sqlBuilder.AppendLine("                   ,SlipTypeCd");
            sqlBuilder.AppendLine("                   ,SlipDiv");
            sqlBuilder.AppendLine("                   ,SlipNo");
            // 2014.04.30 Start 入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            //sqlBuilder.AppendLine("                   ,MAX(InputSequenceNo) AS InputSequenceNo");
            sqlBuilder.AppendLine("                   ,MAX(CAST(InputSequenceNo As numeric)) AS InputSequenceNo");
            // 2014.04.30 End   入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            sqlBuilder.AppendLine("          FROM      SARSlipDetailError WITH(NOLOCK)");
            sqlBuilder.AppendLine("          WHERE     CorporationCode = @CorporationCode");
            // 8/9 SUEYOSHI 入力媒体区分は検索条件として不要の為削除 DEL START
            // sqlBuilder.AppendLine("          AND       InputMediaDiv = '1'");
            // 8/9 SUEYOSHI END
            sqlBuilder.AppendLine("          AND       InputBatchNo = @InputBatchNo");
            sqlBuilder.AppendLine("          AND       LogicalDelFlg = '0'");

            // 7/25 SUEYOSHI ADD
            sqlBuilder.AppendLine("          AND       SlipTypeCd = @SlipTypeCd");
            sqlBuilder.AppendLine("          AND       SlipDiv = @SlipDiv");
            // 7/25 SUEYOSHI END

            if (!string.IsNullOrEmpty(workEntity.TargetSlipNo))
            {
                sqlBuilder.AppendLine("      AND       SlipNo = @SlipNo");
            }
            sqlBuilder.AppendLine("          GROUP BY  CorporationCode");
            sqlBuilder.AppendLine("                   ,InputBatchNo");
            sqlBuilder.AppendLine("                   ,InputMediaDiv");
            sqlBuilder.AppendLine("                   ,SlipTypeCd");
            sqlBuilder.AppendLine("                   ,SlipDiv");
            sqlBuilder.AppendLine("                   ,SlipNo");
            sqlBuilder.AppendLine("        ) AS SDEV");
            sqlBuilder.AppendLine("   WHERE  SARSlipDetailError.CorporationCode = SDEV.CorporationCode");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.InputBatchNo = SDEV.InputBatchNo");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.InputMediaDiv = SDEV.InputMediaDiv");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.SlipTypeCd = SDEV.SlipTypeCd");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.SlipDiv = SDEV.SlipDiv");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.SlipNo = SDEV.SlipNo");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.InputSequenceNo = SDEV.InputSequenceNo");
            sqlBuilder.AppendLine(" )");
            // 2012.06.26 End   複数伝票登録し修正した場合に最大のInputSequenceNoの伝票しか取得されないのを修正

            sqlBuilder.AppendLine("ORDER BY SlipNo ASC, SlipLineNo ASC");

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// ＳＡＲ伝票明細エラー登録更新するためのクエリを作成します。
        /// </summary>
        /// <returns>Query string</returns>
        private string CreateUpInsSARSlipDetailErrorQueryTextNew()
        {
            StringBuilder insertCommandText = new StringBuilder();

            insertCommandText.AppendLine("MERGE INTO SARSlipDetailError USING ");
            insertCommandText.AppendLine("(SELECT  ");
            insertCommandText.AppendLine("    @CorporationCode AS CorporationCode");
            insertCommandText.AppendLine("    ,@InputDate AS InputDate");
            insertCommandText.AppendLine("    ,@InputMediaDiv AS InputMediaDiv");
            insertCommandText.AppendLine("    ,@InputBatchNo AS InputBatchNo");
            insertCommandText.AppendLine("    ,@InputSequenceNo AS InputSequenceNo");
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("    ,@InputSequenceLineNo AS InputSequenceLineNo");
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("    ,@InputTime AS InputTime");
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("    ,@DataType AS DataType");
            insertCommandText.AppendLine("    ,@UptakeSequenceNo AS UptakeSequenceNo");
            insertCommandText.AppendLine("    ,@UptakeSequenceLineNo AS UptakeSequenceLineNo");
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("    ,@SlipLineNo AS SlipLineNo");
            insertCommandText.AppendLine("    ,@StoreCd AS StoreCd");
            insertCommandText.AppendLine("    ,@SlipTypeCd AS SlipTypeCd");
            insertCommandText.AppendLine("    ,@SlipDiv AS SlipDiv");
            insertCommandText.AppendLine("    ,@SlipNo AS SlipNo");
            insertCommandText.AppendLine("    ,@DisposalChangeDate AS DisposalChangeDate");
            insertCommandText.AppendLine("    ,@Category1Cd AS Category1Cd");
            insertCommandText.AppendLine("    ,@Category2Cd AS Category2Cd");
            insertCommandText.AppendLine("    ,@Category3Cd AS Category3Cd");
            insertCommandText.AppendLine("    ,@Category4Cd AS Category4Cd");
            insertCommandText.AppendLine("    ,@SKUCd AS SKUCd");
            insertCommandText.AppendLine("    ,@ScanningCd AS ScanningCd");
            insertCommandText.AppendLine("    ,@DisposalChangeQty AS DisposalChangeQty");
            insertCommandText.AppendLine("    ,@SlipOldPrice AS SlipOldPrice");
            insertCommandText.AppendLine("    ,@SlipNewPrice AS SlipNewPrice");
            insertCommandText.AppendLine("    ,@SlipDisposalUnitPrica AS SlipDisposalUnitPrica");
            insertCommandText.AppendLine("    ,@SalesChangeDiv AS SalesChangeDiv");
            insertCommandText.AppendLine("    ,@Reason AS Reason");
            insertCommandText.AppendLine("    ,@SalesChangePrice AS SalesChangePrice");
            insertCommandText.AppendLine("    ,@DisposalPriceAmt AS DisposalPriceAmt");
            insertCommandText.AppendLine("    ,@Attribute01 AS Attribute01");
            insertCommandText.AppendLine("    ,@Attribute02 AS Attribute02");
            insertCommandText.AppendLine("    ,@Attribute03 AS Attribute03");
            insertCommandText.AppendLine("    ,@Attribute04 AS Attribute04");
            insertCommandText.AppendLine("    ,@Attribute05 AS Attribute05");
            insertCommandText.AppendLine("    ,@Attribute06 AS Attribute06");
            insertCommandText.AppendLine("    ,@Attribute07 AS Attribute07");
            insertCommandText.AppendLine("    ,@Attribute08 AS Attribute08");
            insertCommandText.AppendLine("    ,@Attribute09 AS Attribute09");
            insertCommandText.AppendLine("    ,@Attribute10 AS Attribute10");
            insertCommandText.AppendLine("    ,@LogicalDelFlg AS LogicalDelFlg");
            insertCommandText.AppendLine("    ,@CreateBy AS CreateBy");
            insertCommandText.AppendLine("    ,@CreateDate AS CreateDate");
            insertCommandText.AppendLine("    ,@UpdateBy AS UpdateBy");
            insertCommandText.AppendLine("    ,@UpdateDate AS UpdateDate");
            insertCommandText.AppendLine(" ) DataForUpdate");
            insertCommandText.AppendLine("ON ");
            //   2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
            //insertCommandText.AppendLine("  SARSlipDetailError.CorporationCode = DataForUpdate.CorporationCode");
            //insertCommandText.AppendLine("  AND SARSlipDetailError.SlipLineNo = DataForUpdate.SlipLineNo");
            //insertCommandText.AppendLine("  AND SARSlipDetailError.StoreCd = DataForUpdate.StoreCd");
            //insertCommandText.AppendLine("  AND SARSlipDetailError.SlipTypeCd = DataForUpdate.SlipTypeCd");
            //insertCommandText.AppendLine("  AND SARSlipDetailError.SlipDiv = DataForUpdate.SlipDiv");
            //insertCommandText.AppendLine("  AND SARSlipDetailError.SlipNo = DataForUpdate.SlipNo");
            //insertCommandText.AppendLine("  AND SARSlipDetailError.DisposalChangeDate = DataForUpdate.DisposalChangeDate");

            insertCommandText.AppendLine("  SARSlipDetailError.CorporationCode = DataForUpdate.CorporationCode");
            insertCommandText.AppendLine("  AND SARSlipDetailError.InputMediaDiv = DataForUpdate.InputMediaDiv");
            insertCommandText.AppendLine("  AND SARSlipDetailError.InputBatchNo = DataForUpdate.InputBatchNo");
            insertCommandText.AppendLine("  AND SARSlipDetailError.InputSequenceNo = DataForUpdate.InputSequenceNo");
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("  AND SARSlipDetailError.InputSequenceLineNo = DataForUpdate.InputSequenceLineNo");
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("  AND SARSlipDetailError.InputDate = DataForUpdate.InputDate");
            insertCommandText.AppendLine("  AND SARSlipDetailError.InputTime = DataForUpdate.InputTime");
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("  AND SARSlipDetailError.DataType = DataForUpdate.DataType");
            insertCommandText.AppendLine("  AND SARSlipDetailError.UptakeSequenceNo = DataForUpdate.UptakeSequenceNo");
            insertCommandText.AppendLine("  AND SARSlipDetailError.UptakeSequenceLineNo = DataForUpdate.UptakeSequenceLineNo");
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            //   2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了
            insertCommandText.AppendLine("WHEN MATCHED THEN ");
            insertCommandText.AppendLine("    UPDATE SET ");
            //   2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 開始
            insertCommandText.AppendLine("        SARSlipDetailError.SlipLineNo = DataForUpdate.SlipLineNo");
            insertCommandText.AppendLine("        ,SARSlipDetailError.StoreCd = DataForUpdate.StoreCd");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SlipTypeCd = DataForUpdate.SlipTypeCd");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SlipDiv = DataForUpdate.SlipDiv");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SlipNo = DataForUpdate.SlipNo");
            insertCommandText.AppendLine("        ,SARSlipDetailError.DisposalChangeDate = DataForUpdate.DisposalChangeDate");
            //   2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 終了
            insertCommandText.AppendLine("        ,SARSlipDetailError.Category1Cd = DataForUpdate.Category1Cd");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Category2Cd = DataForUpdate.Category2Cd");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Category3Cd = DataForUpdate.Category3Cd");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Category4Cd = DataForUpdate.Category4Cd");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SKUCd = DataForUpdate.SKUCd");
            insertCommandText.AppendLine("        ,SARSlipDetailError.ScanningCd = DataForUpdate.ScanningCd");
            insertCommandText.AppendLine("        ,SARSlipDetailError.DisposalChangeQty = DataForUpdate.DisposalChangeQty");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SlipOldPrice = DataForUpdate.SlipOldPrice");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SlipNewPrice = DataForUpdate.SlipNewPrice");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SlipDisposalUnitPrica = DataForUpdate.SlipDisposalUnitPrica");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SalesChangeDiv = DataForUpdate.SalesChangeDiv");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Reason = DataForUpdate.Reason");
            insertCommandText.AppendLine("        ,SARSlipDetailError.SalesChangePrice = DataForUpdate.SalesChangePrice");
            insertCommandText.AppendLine("        ,SARSlipDetailError.DisposalPriceAmt = DataForUpdate.DisposalPriceAmt");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute01 = DataForUpdate.Attribute01");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute02 = DataForUpdate.Attribute02");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute03 = DataForUpdate.Attribute03");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute04 = DataForUpdate.Attribute04");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute05 = DataForUpdate.Attribute05");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute06 = DataForUpdate.Attribute06");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute07 = DataForUpdate.Attribute07");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute08 = DataForUpdate.Attribute08");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute09 = DataForUpdate.Attribute09");
            insertCommandText.AppendLine("        ,SARSlipDetailError.Attribute10 = DataForUpdate.Attribute10");
            insertCommandText.AppendLine("        ,SARSlipDetailError.LogicalDelFlg = DataForUpdate.LogicalDelFlg");
            insertCommandText.AppendLine("        ,SARSlipDetailError.UpdateBy = DataForUpdate.UpdateBy");
            insertCommandText.AppendLine("        ,SARSlipDetailError.UpdateDate = DataForUpdate.UpdateDate  ");
            insertCommandText.AppendLine("WHEN NOT MATCHED THEN");
            insertCommandText.AppendLine("  INSERT (");
            insertCommandText.AppendLine("        CorporationCode");
            insertCommandText.AppendLine("        ,InputDate");
            insertCommandText.AppendLine("        ,InputMediaDiv");
            insertCommandText.AppendLine("        ,InputBatchNo");
            insertCommandText.AppendLine("        ,InputSequenceNo");
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("        ,InputSequenceLineNo");
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("        ,InputTime");
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("        ,DataType");
            insertCommandText.AppendLine("        ,UptakeSequenceNo");
            insertCommandText.AppendLine("        ,UptakeSequenceLineNo");
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("        ,SlipLineNo");
            insertCommandText.AppendLine("        ,StoreCd");
            insertCommandText.AppendLine("        ,SlipTypeCd");
            insertCommandText.AppendLine("        ,SlipDiv");
            insertCommandText.AppendLine("        ,SlipNo");
            insertCommandText.AppendLine("        ,DisposalChangeDate");
            insertCommandText.AppendLine("        ,Category1Cd");
            insertCommandText.AppendLine("        ,Category2Cd");
            insertCommandText.AppendLine("        ,Category3Cd");
            insertCommandText.AppendLine("        ,Category4Cd");
            insertCommandText.AppendLine("        ,SKUCd");
            insertCommandText.AppendLine("        ,ScanningCd");
            insertCommandText.AppendLine("        ,DisposalChangeQty");
            insertCommandText.AppendLine("        ,SlipOldPrice");
            insertCommandText.AppendLine("        ,SlipNewPrice");
            insertCommandText.AppendLine("        ,SlipDisposalUnitPrica");
            insertCommandText.AppendLine("        ,SalesChangeDiv");
            insertCommandText.AppendLine("        ,Reason");
            insertCommandText.AppendLine("        ,SalesChangePrice");
            insertCommandText.AppendLine("        ,DisposalPriceAmt");
            insertCommandText.AppendLine("        ,Attribute01");
            insertCommandText.AppendLine("        ,Attribute02");
            insertCommandText.AppendLine("        ,Attribute03");
            insertCommandText.AppendLine("        ,Attribute04");
            insertCommandText.AppendLine("        ,Attribute05");
            insertCommandText.AppendLine("        ,Attribute06");
            insertCommandText.AppendLine("        ,Attribute07");
            insertCommandText.AppendLine("        ,Attribute08");
            insertCommandText.AppendLine("        ,Attribute09");
            insertCommandText.AppendLine("        ,Attribute10");
            insertCommandText.AppendLine("        ,LogicalDelFlg");
            insertCommandText.AppendLine("        ,CreateBy");
            insertCommandText.AppendLine("        ,CreateDate");
            insertCommandText.AppendLine("        ,UpdateBy");
            insertCommandText.AppendLine("        ,UpdateDate) ");
            insertCommandText.AppendLine("    VALUES (");
            insertCommandText.AppendLine("         DataForUpdate.CorporationCode");
            insertCommandText.AppendLine("        , DataForUpdate.InputDate");
            insertCommandText.AppendLine("        , DataForUpdate.InputMediaDiv");
            insertCommandText.AppendLine("        , DataForUpdate.InputBatchNo");
            insertCommandText.AppendLine("        , DataForUpdate.InputSequenceNo");
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("        , DataForUpdate.InputSequenceLineNo");
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("        , DataForUpdate.InputTime");
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("        , DataForUpdate.DataType");
            insertCommandText.AppendLine("        , DataForUpdate.UptakeSequenceNo");
            insertCommandText.AppendLine("        , DataForUpdate.UptakeSequenceLineNo");
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            insertCommandText.AppendLine("        , DataForUpdate.SlipLineNo");
            insertCommandText.AppendLine("        , DataForUpdate.StoreCd");
            insertCommandText.AppendLine("        , DataForUpdate.SlipTypeCd");
            insertCommandText.AppendLine("        , DataForUpdate.SlipDiv");
            insertCommandText.AppendLine("        , DataForUpdate.SlipNo");
            insertCommandText.AppendLine("        , DataForUpdate.DisposalChangeDate");
            insertCommandText.AppendLine("        , DataForUpdate.Category1Cd");
            insertCommandText.AppendLine("        , DataForUpdate.Category2Cd");
            insertCommandText.AppendLine("        , DataForUpdate.Category3Cd");
            insertCommandText.AppendLine("        , DataForUpdate.Category4Cd");
            insertCommandText.AppendLine("        , DataForUpdate.SKUCd");
            insertCommandText.AppendLine("        , DataForUpdate.ScanningCd");
            insertCommandText.AppendLine("        , DataForUpdate.DisposalChangeQty");
            insertCommandText.AppendLine("        , DataForUpdate.SlipOldPrice");
            insertCommandText.AppendLine("        , DataForUpdate.SlipNewPrice");
            insertCommandText.AppendLine("        , DataForUpdate.SlipDisposalUnitPrica");
            insertCommandText.AppendLine("        , DataForUpdate.SalesChangeDiv");
            insertCommandText.AppendLine("        , DataForUpdate.Reason");
            insertCommandText.AppendLine("        , DataForUpdate.SalesChangePrice");
            insertCommandText.AppendLine("        , DataForUpdate.DisposalPriceAmt");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute01");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute02");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute03");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute04");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute05");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute06");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute07");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute08");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute09");
            insertCommandText.AppendLine("        , DataForUpdate.Attribute10");
            insertCommandText.AppendLine("        , '0'");
            insertCommandText.AppendLine("        , DataForUpdate.CreateBy");
            insertCommandText.AppendLine("        , DataForUpdate.CreateDate");
            insertCommandText.AppendLine("        , DataForUpdate.UpdateBy");
            insertCommandText.AppendLine("        , DataForUpdate.UpdateDate);");

            return insertCommandText.ToString();
        }

        /// <summary>
        /// ＳＡＲ伝票ヘッダーの登録ためのクエリを作成します。
        /// </summary>
        /// <returns>Query string</returns>
        private string CreateInsertSARSlipHeaderQueryTextNew()
        {
            StringBuilder insertCommandText = new StringBuilder();

            insertCommandText.AppendLine("INSERT INTO SARSlipHeader(");
            insertCommandText.AppendLine("   CorporationCode");
            insertCommandText.AppendLine("   ,SlipDiv");
            insertCommandText.AppendLine("   ,SlipTypeCd");
            insertCommandText.AppendLine("   ,SlipNo");
            insertCommandText.AppendLine("   ,SlipBranchNo");
            insertCommandText.AppendLine("   ,DisposalChangeDate");
            insertCommandText.AppendLine("   ,StoreCd");
            insertCommandText.AppendLine("   ,Category1Cd");
            insertCommandText.AppendLine("   ,Category2Cd");
            insertCommandText.AppendLine("   ,TotalQty");
            insertCommandText.AppendLine("   ,SlipOldTotalPrice");
            insertCommandText.AppendLine("   ,SlipNewTotalPrice");
            insertCommandText.AppendLine("   ,AutoSalesChangeFlg");
            insertCommandText.AppendLine("   ,SlipTotalDisposalAmt");
            insertCommandText.AppendLine("   ,SlipDate");
            insertCommandText.AppendLine("   ,PostingDate");
            insertCommandText.AppendLine("   ,ApprovalRequester");
            insertCommandText.AppendLine("   ,RequestApprovalDate");
            insertCommandText.AppendLine("   ,ApproverCd");
            insertCommandText.AppendLine("   ,ApprovalStatus");
            insertCommandText.AppendLine("   ,ApprovalDate");
            insertCommandText.AppendLine("   ,SlipRevisionFlg");
            insertCommandText.AppendLine("   ,CorrectionDiv");
            insertCommandText.AppendLine("   ,RedBlackDiv");
            insertCommandText.AppendLine("   ,InputMediaDiv");
            insertCommandText.AppendLine("   ,InputBatchNo");
            insertCommandText.AppendLine("   ,InputSequenceNo");
            insertCommandText.AppendLine("   ,LogicalDelFlg");
            insertCommandText.AppendLine("   ,CreateBy");
            insertCommandText.AppendLine("   ,CreateDate");
            insertCommandText.AppendLine("   ,UpdateBy");
            insertCommandText.AppendLine("   ,UpdateDate)");
            insertCommandText.AppendLine("VALUES(");
            insertCommandText.AppendLine("   @CorporationCode");
            insertCommandText.AppendLine("   ,@SlipDiv");
            insertCommandText.AppendLine("   ,@SlipTypeCd");
            insertCommandText.AppendLine("   ,@SlipNo");
            insertCommandText.AppendLine("   ,@SlipBranchNo");
            insertCommandText.AppendLine("   ,@DisposalChangeDate");
            insertCommandText.AppendLine("   ,@StoreCd");
            insertCommandText.AppendLine("   ,@Category1Cd");
            insertCommandText.AppendLine("   ,@Category2Cd");
            insertCommandText.AppendLine("   ,@TotalQty");
            insertCommandText.AppendLine("   ,@SlipOldTotalPrice");
            insertCommandText.AppendLine("   ,@SlipNewTotalPrice");
            insertCommandText.AppendLine("   ,@AutoSalesChangeFlg");
            insertCommandText.AppendLine("   ,@SlipTotalDisposalAmt");
            insertCommandText.AppendLine("   ,@SlipDate");
            insertCommandText.AppendLine("   ,@PostingDate");
            insertCommandText.AppendLine("   ,@ApprovalRequester");
            insertCommandText.AppendLine("   ,@RequestApprovalDate");
            insertCommandText.AppendLine("   ,@ApproverCd");
            insertCommandText.AppendLine("   ,@ApprovalStatus");
            insertCommandText.AppendLine("   ,@ApprovalDate");
            insertCommandText.AppendLine("   ,'0'");
            insertCommandText.AppendLine("   ,'0'");
            insertCommandText.AppendLine("   ,'0'");
            insertCommandText.AppendLine("   ,@InputMediaDiv");
            insertCommandText.AppendLine("   ,@InputBatchNo");
            insertCommandText.AppendLine("   ,@InputSequenceNo");
            insertCommandText.AppendLine("   ,'0'");
            insertCommandText.AppendLine("   ,@CreateBy");
            insertCommandText.AppendLine("   ,@CreateDate");
            insertCommandText.AppendLine("   ,@UpdateBy");
            insertCommandText.AppendLine("   ,@UpdateDate)");

            return insertCommandText.ToString();
        }

        /// <summary>
        /// 伝票明細登録更新するためのクエリを作成します。
        /// </summary>
        /// <returns>Query string</returns>
        private string CreateInsertSARSlipDetailFillQueryNew()
        {
            StringBuilder insertCommandText = new StringBuilder();

            insertCommandText.AppendLine("INSERT INTO SARSlipDetail(");
            insertCommandText.AppendLine("   CorporationCode");
            insertCommandText.AppendLine("   ,SlipLineNo");
            insertCommandText.AppendLine("   ,StoreCd");
            insertCommandText.AppendLine("   ,SlipTypeCd");
            insertCommandText.AppendLine("   ,SlipBranchNo");
            insertCommandText.AppendLine("   ,SlipDiv");
            insertCommandText.AppendLine("   ,SlipNo");
            insertCommandText.AppendLine("   ,DisposalChangeDate");
            insertCommandText.AppendLine("   ,Category1Cd");
            insertCommandText.AppendLine("   ,Category2Cd");
            insertCommandText.AppendLine("   ,Category3Cd");
            insertCommandText.AppendLine("   ,Category4Cd");
            insertCommandText.AppendLine("   ,SKUCd");
            insertCommandText.AppendLine("   ,ScanningCd");
            insertCommandText.AppendLine("   ,DisposalChangeQty");
            insertCommandText.AppendLine("   ,SlipOldPrice");
            insertCommandText.AppendLine("   ,SlipNewPrice");
            insertCommandText.AppendLine("   ,SlipDisposalUnitPrica");
            insertCommandText.AppendLine("   ,SalesChangeDiv");
            insertCommandText.AppendLine("   ,Reason");
            insertCommandText.AppendLine("   ,SalesChangePrice");
            insertCommandText.AppendLine("   ,DisposalPriceAmt");
            insertCommandText.AppendLine("   ,Attribute01");
            insertCommandText.AppendLine("   ,Attribute02");
            insertCommandText.AppendLine("   ,Attribute03");
            insertCommandText.AppendLine("   ,Attribute04");
            insertCommandText.AppendLine("   ,Attribute05");
            insertCommandText.AppendLine("   ,Attribute06");
            insertCommandText.AppendLine("   ,Attribute07");
            insertCommandText.AppendLine("   ,Attribute08");
            insertCommandText.AppendLine("   ,Attribute09");
            insertCommandText.AppendLine("   ,Attribute10");
            insertCommandText.AppendLine("   ,LogicalDelFlg");
            insertCommandText.AppendLine("   ,CreateBy");
            insertCommandText.AppendLine("   ,CreateDate");
            insertCommandText.AppendLine("   ,UpdateBy");
            insertCommandText.AppendLine("   ,UpdateDate)");
            insertCommandText.AppendLine("VALUES");
            insertCommandText.AppendLine("   (@CorporationCode");
            insertCommandText.AppendLine("   ,@SlipLineNo");
            insertCommandText.AppendLine("   ,@StoreCd");
            insertCommandText.AppendLine("   ,@SlipTypeCd");
            insertCommandText.AppendLine("   ,@SlipBranchNo");
            insertCommandText.AppendLine("   ,@SlipDiv");
            insertCommandText.AppendLine("   ,@SlipNo");
            insertCommandText.AppendLine("   ,@DisposalChangeDate");
            insertCommandText.AppendLine("   ,@Category1Cd");
            insertCommandText.AppendLine("   ,@Category2Cd");
            insertCommandText.AppendLine("   ,@Category3Cd");
            insertCommandText.AppendLine("   ,@Category4Cd");
            insertCommandText.AppendLine("   ,@SKUCd");
            insertCommandText.AppendLine("   ,@ScanningCd");
            insertCommandText.AppendLine("   ,@DisposalChangeQty");
            insertCommandText.AppendLine("   ,@SlipOldPrice");
            insertCommandText.AppendLine("   ,@SlipNewPrice");
            insertCommandText.AppendLine("   ,@SlipDisposalUnitPrica");
            insertCommandText.AppendLine("   ,@SalesChangeDiv");
            insertCommandText.AppendLine("   ,@Reason");
            insertCommandText.AppendLine("   ,@SalesChangePrice");
            insertCommandText.AppendLine("   ,@DisposalPriceAmt");
            insertCommandText.AppendLine("   ,@Attribute01");
            insertCommandText.AppendLine("   ,@Attribute02");
            insertCommandText.AppendLine("   ,@Attribute03");
            insertCommandText.AppendLine("   ,@Attribute04");
            insertCommandText.AppendLine("   ,@Attribute05");
            insertCommandText.AppendLine("   ,@Attribute06");
            insertCommandText.AppendLine("   ,@Attribute07");
            insertCommandText.AppendLine("   ,@Attribute08");
            insertCommandText.AppendLine("   ,@Attribute09");
            insertCommandText.AppendLine("   ,@Attribute10");
            insertCommandText.AppendLine("   ,'0'");
            insertCommandText.AppendLine("   ,@CreateBy");
            insertCommandText.AppendLine("   ,@CreateDate");
            insertCommandText.AppendLine("   ,@UpdateBy");
            insertCommandText.AppendLine("   ,@UpdateDate)");

            return insertCommandText.ToString();
        }

        /// <summary>
        /// ＳＡＲ伝票ヘッダー赤伝更新するためのクエリを作成します。
        /// </summary>
        /// <returns>Query string</returns>
        private string CreateInsertSARSlipHeaderForDenyQueryTextNew()
        {
            StringBuilder insertCommandText = new StringBuilder();

            insertCommandText.AppendLine("INSERT INTO SARSlipHeader(");
            insertCommandText.AppendLine("   CorporationCode");
            insertCommandText.AppendLine("   ,SlipDiv");
            insertCommandText.AppendLine("   ,SlipTypeCd");
            insertCommandText.AppendLine("   ,SlipNo");
            insertCommandText.AppendLine("   ,SlipBranchNo");
            insertCommandText.AppendLine("   ,DisposalChangeDate");
            insertCommandText.AppendLine("   ,StoreCd");
            insertCommandText.AppendLine("   ,Category1Cd");
            insertCommandText.AppendLine("   ,Category2Cd");
            insertCommandText.AppendLine("   ,TotalQty");
            insertCommandText.AppendLine("   ,SlipOldTotalPrice");
            insertCommandText.AppendLine("   ,SlipNewTotalPrice");
            insertCommandText.AppendLine("   ,AutoSalesChangeFlg");
            insertCommandText.AppendLine("   ,SlipTotalDisposalAmt");
            insertCommandText.AppendLine("   ,SlipDate");
            insertCommandText.AppendLine("   ,PostingDate");
            insertCommandText.AppendLine("   ,ApprovalRequester");
            insertCommandText.AppendLine("   ,RequestApprovalDate");
            insertCommandText.AppendLine("   ,ApproverCd");
            insertCommandText.AppendLine("   ,ApprovalStatus");
            insertCommandText.AppendLine("   ,ApprovalDate");
            insertCommandText.AppendLine("   ,SlipRevisionFlg");
            insertCommandText.AppendLine("   ,RedBlackDiv");
            insertCommandText.AppendLine("   ,CorrectionDiv");
            insertCommandText.AppendLine("   ,InputMediaDiv");
            insertCommandText.AppendLine("   ,InputBatchNo");
            insertCommandText.AppendLine("   ,InputSequenceNo");
            insertCommandText.AppendLine("   ,LogicalDelFlg");
            insertCommandText.AppendLine("   ,CreateBy");
            insertCommandText.AppendLine("   ,CreateDate");
            insertCommandText.AppendLine("   ,UpdateBy");
            insertCommandText.AppendLine("   ,UpdateDate)");
            insertCommandText.AppendLine("SELECT");
            insertCommandText.AppendLine("   CorporationCode");
            insertCommandText.AppendLine("   ,SlipDiv");
            insertCommandText.AppendLine("   ,SlipTypeCd");
            insertCommandText.AppendLine("   ,SlipNo");
            insertCommandText.AppendLine("   ,@NextSlipBranchNo");
            insertCommandText.AppendLine("   ,DisposalChangeDate");
            insertCommandText.AppendLine("   ,StoreCd");
            insertCommandText.AppendLine("   ,Category1Cd");
            insertCommandText.AppendLine("   ,Category2Cd");
            insertCommandText.AppendLine("   ,-1 * TotalQty");
            insertCommandText.AppendLine("   ,SlipOldTotalPrice");
            insertCommandText.AppendLine("   ,SlipNewTotalPrice");
            insertCommandText.AppendLine("   ,AutoSalesChangeFlg");
            insertCommandText.AppendLine("   ,-1 * SlipTotalDisposalAmt");
            insertCommandText.AppendLine("   ,SlipDate");
            insertCommandText.AppendLine("   ,PostingDate");
            insertCommandText.AppendLine("   ,ApprovalRequester");
            insertCommandText.AppendLine("   ,RequestApprovalDate");
            insertCommandText.AppendLine("   ,ApproverCd");
            insertCommandText.AppendLine("   ,ApprovalStatus");
            insertCommandText.AppendLine("   ,ApprovalDate");
            insertCommandText.AppendLine("   ,'1'");
            insertCommandText.AppendLine("   ,'1'");
            insertCommandText.AppendLine("   ,CorrectionDiv");
            insertCommandText.AppendLine("   ,InputMediaDiv");
            insertCommandText.AppendLine("   ,InputBatchNo");
            insertCommandText.AppendLine("   ,InputSequenceNo");
            insertCommandText.AppendLine("   ,'0'");
            insertCommandText.AppendLine("   ,CreateBy");
            insertCommandText.AppendLine("   ,CreateDate");
            insertCommandText.AppendLine("   ,@UpdateBy");
            insertCommandText.AppendLine("   ,@UpdateDate");
            insertCommandText.AppendLine("FROM SARSlipHeader");
            insertCommandText.AppendLine("WHERE");
            insertCommandText.AppendLine(" CorporationCode = @CorporationCode");
            insertCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            insertCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            insertCommandText.AppendLine(" AND SlipNo = @SlipNo");
            insertCommandText.AppendLine(" AND SlipBranchNo = (@NextSlipBranchNo - 1)");
            insertCommandText.AppendLine(" AND StoreCd = @StoreCd");
            // 8/30 SUEYOSHI 検索条件を追加
            insertCommandText.AppendLine(" AND DisposalChangeDate = @DisposalChangeDate");
            // 8/30 SUEYOSHI END

            return insertCommandText.ToString();
        }

        /// <summary>
        /// ＳＡＲ伝票明細赤伝更新するためのクエリを作成します。
        /// </summary>
        /// <returns>Query string</returns>
        private string CreateInsertSARSlipDetailForDenyFillQueryNew()
        {
            StringBuilder insertCommandText = new StringBuilder();

            insertCommandText.AppendLine("INSERT INTO SARSlipDetail");
            insertCommandText.AppendLine("    (CorporationCode,");
            insertCommandText.AppendLine("    SlipLineNo,");
            insertCommandText.AppendLine("    StoreCd,");
            insertCommandText.AppendLine("    SlipTypeCd,");
            insertCommandText.AppendLine("    SlipBranchNo,");
            insertCommandText.AppendLine("    SlipDiv,");
            insertCommandText.AppendLine("    SlipNo,");
            insertCommandText.AppendLine("    DisposalChangeDate,");
            insertCommandText.AppendLine("    Category1Cd,");
            insertCommandText.AppendLine("    Category2Cd,");
            insertCommandText.AppendLine("    Category3Cd,");
            insertCommandText.AppendLine("    Category4Cd,");
            insertCommandText.AppendLine("    SKUCd,");
            insertCommandText.AppendLine("    ScanningCd,");
            insertCommandText.AppendLine("    DisposalChangeQty,");
            insertCommandText.AppendLine("    SlipOldPrice,");
            insertCommandText.AppendLine("    SlipNewPrice,");
            insertCommandText.AppendLine("    SlipDisposalUnitPrica,");
            insertCommandText.AppendLine("    SalesChangeDiv,");
            insertCommandText.AppendLine("    Reason,");
            insertCommandText.AppendLine("    SalesChangePrice,");
            insertCommandText.AppendLine("    DisposalPriceAmt,");
            insertCommandText.AppendLine("    Attribute01,");
            insertCommandText.AppendLine("    Attribute02,");
            insertCommandText.AppendLine("    Attribute03,");
            insertCommandText.AppendLine("    Attribute04,");
            insertCommandText.AppendLine("    Attribute05,");
            insertCommandText.AppendLine("    Attribute06,");
            insertCommandText.AppendLine("    Attribute07,");
            insertCommandText.AppendLine("    Attribute08,");
            insertCommandText.AppendLine("    Attribute09,");
            insertCommandText.AppendLine("    Attribute10,");
            insertCommandText.AppendLine("    LogicalDelFlg,");
            insertCommandText.AppendLine("    CreateBy,");
            insertCommandText.AppendLine("    CreateDate,");
            insertCommandText.AppendLine("    UpdateBy,");
            insertCommandText.AppendLine("    UpdateDate)");
            insertCommandText.AppendLine("SELECT");
            insertCommandText.AppendLine("    CorporationCode,");
            insertCommandText.AppendLine("    SlipLineNo,");
            insertCommandText.AppendLine("    StoreCd,");
            insertCommandText.AppendLine("    SlipTypeCd,");
            insertCommandText.AppendLine("    SlipBranchNo + 1,");
            insertCommandText.AppendLine("    SlipDiv,");
            insertCommandText.AppendLine("    SlipNo,");
            insertCommandText.AppendLine("    DisposalChangeDate,");
            insertCommandText.AppendLine("    Category1Cd,");
            insertCommandText.AppendLine("    Category2Cd,");
            insertCommandText.AppendLine("    Category3Cd,");
            insertCommandText.AppendLine("    Category4Cd,");
            insertCommandText.AppendLine("    SKUCd,");
            insertCommandText.AppendLine("    ScanningCd,");
            insertCommandText.AppendLine("    -1 * DisposalChangeQty,");
            insertCommandText.AppendLine("    SlipOldPrice,");
            insertCommandText.AppendLine("    SlipNewPrice,");
            insertCommandText.AppendLine("    SlipDisposalUnitPrica,");
            insertCommandText.AppendLine("    SalesChangeDiv,");
            insertCommandText.AppendLine("    Reason,");
            insertCommandText.AppendLine("    SalesChangePrice,");
            insertCommandText.AppendLine("    -1 * DisposalPriceAmt,");
            insertCommandText.AppendLine("    Attribute01,");
            insertCommandText.AppendLine("    Attribute02,");
            insertCommandText.AppendLine("    Attribute03,");
            insertCommandText.AppendLine("    Attribute04,");
            insertCommandText.AppendLine("    Attribute05,");
            insertCommandText.AppendLine("    Attribute06,");
            insertCommandText.AppendLine("    Attribute07,");
            insertCommandText.AppendLine("    Attribute08,");
            insertCommandText.AppendLine("    Attribute09,");
            insertCommandText.AppendLine("    Attribute10,");
            insertCommandText.AppendLine("    '0',");
            insertCommandText.AppendLine("    @CreateBy,");
            insertCommandText.AppendLine("    @CreateDate,");
            insertCommandText.AppendLine("    @UpdateBy,");
            insertCommandText.AppendLine("    @UpdateDate");
            insertCommandText.AppendLine("FROM SARSlipDetail");
            insertCommandText.AppendLine("WHERE");
            insertCommandText.AppendLine(" CorporationCode = @CorporationCode");
            insertCommandText.AppendLine(" AND SlipLineNo = @SlipLineNo");
            insertCommandText.AppendLine(" AND StoreCd = @StoreCd");
            insertCommandText.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            insertCommandText.AppendLine(" AND SlipBranchNo = @SlipBranchNo");
            insertCommandText.AppendLine(" AND SlipDiv = @SlipDiv");
            insertCommandText.AppendLine(" AND SlipNo = @SlipNo");
            insertCommandText.AppendLine(" AND DisposalChangeDate = @DisposalChangeDate");

            return insertCommandText.ToString();
        }

        /// <summary>
        /// カバーシート検索するためのクエリを作成します。
        /// </summary>
        /// <returns>Query string</returns>
        private string CreateFillSARCoverSheetMasterFillQueryNew()
        {
            // 引数の店舗検索条件EntityからSQLを動的に組み立てる。DbParameter が使える場合は極力使用する。
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("SELECT");
            sqlBuilder.AppendLine("   CorporationCode");
            // 2013/09/17 アドオン対応：アジルさん対応の物理名修正漏れを修正 ISC:yshindo start
            //sqlBuilder.AppendLine("   ,BatchNo AS InputBatchNo");
            sqlBuilder.AppendLine("   ,BatchNo");
            // 2013/09/17 アドオン対応：アジルさん対応の物理名修正漏れを修正 ISC:yshindo end
            sqlBuilder.AppendLine("   ,StoreCd");
            sqlBuilder.AppendLine("   ,SlipTypeCd");
            sqlBuilder.AppendLine("   ,SlipDiv");
            // 2013/09/17 アドオン対応：アジルさん対応の物理名修正漏れを修正 ISC:yshindo start
            //sqlBuilder.AppendLine("   ,BatchDate AS InputBatchDate");
            sqlBuilder.AppendLine("   ,BatchDate");
            // 2013/09/17 アドオン対応：アジルさん対応の物理名修正漏れを修正 ISC:yshindo end
            sqlBuilder.AppendLine("   ,TotalPageQty");
            sqlBuilder.AppendLine("   ,InputPageCount");
            sqlBuilder.AppendLine("   ,ErrorNo");
            sqlBuilder.AppendLine("   ,ApprovalCount");
            sqlBuilder.AppendLine("   ,PersonnelInput");
            sqlBuilder.AppendLine("   ,LastUpdateDate");
            sqlBuilder.AppendLine("   ,ApprovalRequester");
            sqlBuilder.AppendLine("   ,RequestApprovalDate");
            sqlBuilder.AppendLine("   ,ApproverCd");
            sqlBuilder.AppendLine("   ,ApprovalStatus");
            sqlBuilder.AppendLine("   ,ApprovalDate");
            sqlBuilder.AppendLine("   ,LogicalDelDate");
            sqlBuilder.AppendLine("   ,LogicalDelFlg");
            sqlBuilder.AppendLine("   ,CreateBy");
            sqlBuilder.AppendLine("   ,CreateDate");
            sqlBuilder.AppendLine("   ,UpdateBy");
            sqlBuilder.AppendLine("   ,UpdateDate");
            sqlBuilder.AppendLine("   ,SystemRowversion");
            sqlBuilder.AppendLine("FROM");
            sqlBuilder.AppendLine(" SARCoverSheetMaster");
            sqlBuilder.AppendLine("WHERE");
            sqlBuilder.AppendLine(" CorporationCode = @CorporationCode");
            sqlBuilder.AppendLine(" AND BatchNo = @BatchNo");
            // 8/16 SUEYOSHI 抽出条件から削除 Del
            // sqlBuilder.AppendLine(" AND StoreCd = @StoreCd");
            // 8/16 SUEYOSHI END
            sqlBuilder.AppendLine(" AND SlipTypeCd = @SlipTypeCd");
            sqlBuilder.AppendLine(" AND SlipDiv = @SlipDiv");
#warning SKN臨時対応　抽出条件から一時的にコメント
            //sqlBuilder.AppendLine(" AND BatchDate = @BatchDate");

            return sqlBuilder.ToString();
        }

        //   2011.12.06 関根 大福帳の時刻・時間帯に値を更新する 修正 開始
        /// <summary>
        /// ＳＡＲ営業時間帯より対象時刻の営業時間帯を取得する。
        /// </summary>
        /// <returns>Query string</returns>
        private string GetBusinessPeriodNew()
        {
            string outBusinessPeriod = string.Empty;
            StringBuilder sqlQueryText = new StringBuilder();

            // 2014.01.07 Start 対象店舗の営業時間帯を取得出来ない場合は、９９９店舗の時間帯を取得する。
            //sqlQueryText.AppendLine(" SELECT BusinessPeriod");
            //sqlQueryText.AppendLine(" FROM   SARBusinessPeriod ");
            //sqlQueryText.AppendLine(" WHERE  CorporationCode = @CorporationCode");
            //sqlQueryText.AppendLine("   AND  StoreCd = @StoreCd");
            //sqlQueryText.AppendLine("   AND  DealPeriodFrom <= SUBSTRING(@DealPeriod, 1, 4)");
            //sqlQueryText.AppendLine("   AND  DealPeriodTo >= SUBSTRING(@DealPeriod, 1, 4)");
            sqlQueryText.AppendLine(" SELECT");
            sqlQueryText.AppendLine("   CASE WHEN ISNULL(MAX(A.BusinessPeriod1),'') <> '' THEN");
            sqlQueryText.AppendLine("            MAX(A.BusinessPeriod1)");
            sqlQueryText.AppendLine("        ELSE");
            sqlQueryText.AppendLine("            MAX(A.BusinessPeriod2)");
            sqlQueryText.AppendLine("        END AS BusinessPeriod");
            sqlQueryText.AppendLine(" FROM");
            sqlQueryText.AppendLine("   (");
            sqlQueryText.AppendLine("       SELECT  BusinessPeriod AS BusinessPeriod1");
            sqlQueryText.AppendLine("               ,''            AS BusinessPeriod2");
            sqlQueryText.AppendLine("         FROM   SARBusinessPeriod WITH(NOLOCK)");
            sqlQueryText.AppendLine("        WHERE  CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine("          AND  StoreCd         = @StoreCd");
            sqlQueryText.AppendLine("          AND  DealPeriodFrom <= SUBSTRING(@DealPeriod, 1, 4)");
            sqlQueryText.AppendLine("          AND  DealPeriodTo   >= SUBSTRING(@DealPeriod, 1, 4)");
            sqlQueryText.AppendLine("   UNION ALL");
            sqlQueryText.AppendLine("        SELECT ''                AS BusinessPeriod1");
            sqlQueryText.AppendLine("               ,A.BusinessPeriod AS BusinessPeriod2");
            sqlQueryText.AppendLine("          FROM   SARBusinessPeriod A WITH(NOLOCK)");
            sqlQueryText.AppendLine("            INNER JOIN V_APVariable B WITH(NOLOCK)");
            sqlQueryText.AppendLine("                    ON B.CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine("                   AND APVariableCode    = 'APVCSAR0073'");
            sqlQueryText.AppendLine("        WHERE  A.CorporationCode = @CorporationCode");
            sqlQueryText.AppendLine("          AND  A.StoreCd         = B.APVariableValue");
            sqlQueryText.AppendLine("          AND  A.DealPeriodFrom <= SUBSTRING(@DealPeriod, 1, 4)");
            sqlQueryText.AppendLine("          AND  A.DealPeriodTo   >= SUBSTRING(@DealPeriod, 1, 4)");
            sqlQueryText.AppendLine("   ) A");
            // 2014.01.07 End   対象店舗の営業時間帯を取得出来ない場合は、９９９店舗の時間帯を取得する。

            return sqlQueryText.ToString();
        }
        //   2011.12.06 関根 大福帳の時刻・時間帯に値を更新する 修正 終了


        // 2014.03.18 Start 効率改善対応（商品名取得変更）
        /// <summary>
        /// 伝票明細検索するためのクエリを作成します。
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>Query string</returns>
        private string CreateFillSARSlipDetailErrorGetProductsKanjiFillQueryNew(WorkEntity workEntity)
        {
            // 引数の店舗検索条件EntityからSQLを動的に組み立てる。DbParameter が使える場合は極力使用する。
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("SELECT");
            sqlBuilder.AppendLine("    SARSlipDetailError.CorporationCode");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.InputMediaDiv");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.InputBatchNo");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.InputSequenceNo");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.InputSequenceLineNo");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.InputDate");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.InputTime");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.DataType");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.UptakeSequenceNo");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.UptakeSequenceLineNo");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SlipLineNo");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.StoreCd");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SlipTypeCd");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SlipDiv");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SlipNo");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.DisposalChangeDate");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Category1Cd");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Category2Cd");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Category3Cd");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Category4Cd");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SKUCd");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.ScanningCd");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.DisposalChangeQty");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SlipOldPrice");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SlipNewPrice");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SlipDisposalUnitPrica");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SalesChangeDiv");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Reason");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SalesChangePrice");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.DisposalPriceAmt");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.ErrorDiv");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.ErrorMessage");
            // 2014.07.14 Start 商品名から規格名を削除する（売変指示書に合わせる）
            //sqlBuilder.AppendLine("    ,ISNULL(LEFT(RTRIM(STRProductName.ProductsKanji) + '　' + RTRIM(STRProductName.ProductsStdsKanji), 50), '') AS Attribute01");
            sqlBuilder.AppendLine("    ,ISNULL(LEFT(RTRIM(STRProductName.ProductsKanji), 50), '') AS Attribute01");
            // 2014.07.14 End   商品名から規格名を削除する（売変指示書に合わせる）
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute02");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute03");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute04");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute05");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute06");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute07");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute08");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute09");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.Attribute10");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.LogicalDelDate");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.LogicalDelFlg");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.CreateBy");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.CreateDate");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.UpdateBy");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.UpdateDate");
            sqlBuilder.AppendLine("    ,SARSlipDetailError.SystemRowversion");
            sqlBuilder.AppendLine("FROM");
            sqlBuilder.AppendLine(" SARSlipDetailError WITH(NOLOCK) ");
            sqlBuilder.AppendLine(" LEFT JOIN STRProductName WITH(NOLOCK) ");
            sqlBuilder.AppendLine("        ON STRProductName.CorporationCode = SARSlipDetailError.CorporationCode ");
            sqlBuilder.AppendLine("       AND STRProductName.OwnProductsCd = SARSlipDetailError.SKUCd ");
            sqlBuilder.AppendLine("       AND STRProductName.EffectiveStartDate <= SARSlipDetailError.DisposalChangeDate ");
            sqlBuilder.AppendLine("       AND STRProductName.EffectiveEndDate >= SARSlipDetailError.DisposalChangeDate ");
            sqlBuilder.AppendLine(" WHERE EXISTS");
            sqlBuilder.AppendLine(" ( SELECT SDEV.*");
            sqlBuilder.AppendLine("   FROM ( SELECT    CorporationCode");
            sqlBuilder.AppendLine("                   ,InputBatchNo");
            sqlBuilder.AppendLine("                   ,InputMediaDiv");
            sqlBuilder.AppendLine("                   ,SlipTypeCd");
            sqlBuilder.AppendLine("                   ,SlipDiv");
            sqlBuilder.AppendLine("                   ,SlipNo");
            // 2014.04.30 Start 入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            //sqlBuilder.AppendLine("                   ,MAX(InputSequenceNo) AS InputSequenceNo");
            sqlBuilder.AppendLine("                   ,MAX(CAST(InputSequenceNo As numeric)) AS InputSequenceNo");
            // 2014.04.30 End   入力ＳＥＱ番号が２桁以上になった場合、最新の伝票情報が取得出来ない不具合修正
            sqlBuilder.AppendLine("          FROM      SARSlipDetailError WITH(NOLOCK)");
            sqlBuilder.AppendLine("          WHERE     CorporationCode = @CorporationCode");
            sqlBuilder.AppendLine("          AND       InputBatchNo = @InputBatchNo");
            sqlBuilder.AppendLine("          AND       LogicalDelFlg = '0'");
            sqlBuilder.AppendLine("          AND       SlipTypeCd = @SlipTypeCd");
            sqlBuilder.AppendLine("          AND       SlipDiv = @SlipDiv");
            if (!string.IsNullOrEmpty(workEntity.TargetSlipNo))
            {
                sqlBuilder.AppendLine("      AND       SlipNo = @SlipNo");
            }
            sqlBuilder.AppendLine("          GROUP BY  CorporationCode");
            sqlBuilder.AppendLine("                   ,InputBatchNo");
            sqlBuilder.AppendLine("                   ,InputMediaDiv");
            sqlBuilder.AppendLine("                   ,SlipTypeCd");
            sqlBuilder.AppendLine("                   ,SlipDiv");
            sqlBuilder.AppendLine("                   ,SlipNo");
            sqlBuilder.AppendLine("        ) AS SDEV");
            sqlBuilder.AppendLine("   WHERE  SARSlipDetailError.CorporationCode = SDEV.CorporationCode");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.InputBatchNo = SDEV.InputBatchNo");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.InputMediaDiv = SDEV.InputMediaDiv");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.SlipTypeCd = SDEV.SlipTypeCd");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.SlipDiv = SDEV.SlipDiv");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.SlipNo = SDEV.SlipNo");
            sqlBuilder.AppendLine("   AND    SARSlipDetailError.InputSequenceNo = SDEV.InputSequenceNo");
            sqlBuilder.AppendLine(" )");

            // 2014.04.30 Start 伝票明細のソート順対応（行番号が２桁以上の場合の対応）
            // 行番号がテーブル定義上、文字列になっている為、
            // ソートする時に数値型に変換する。
            sqlBuilder.AppendLine("ORDER BY SARSlipDetailError.SlipNo ASC, CAST(SARSlipDetailError.SlipLineNo AS INT) ASC");
            // 2014.04.30 End   伝票明細のソート順対応（行番号が２桁以上の場合の対応）

            return sqlBuilder.ToString();
        }
        // 2014.03.18 End   効率改善対応（商品名取得変更）



        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="coverSheetMasterDataRow">カバーシート情報DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateChkBatchNoDbParameterListNew(
                DbCommand dbCommand,
                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetMasterDataRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", coverSheetMasterDataRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchNo", coverSheetMasterDataRow.BatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", coverSheetMasterDataRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", coverSheetMasterDataRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", coverSheetMasterDataRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchDate", coverSheetMasterDataRow.BatchDate));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="coverSheetMasterDataRow">カバーシート情報DataRow</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateFillSARSlipHeaderErrorParameterListNew(
                DbCommand dbCommand,
                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetMasterDataRow,
                WorkEntity workEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", coverSheetMasterDataRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", coverSheetMasterDataRow.BatchNo));
            // 7/27 SUEYOSHI ADD
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", coverSheetMasterDataRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", coverSheetMasterDataRow.SlipTypeCd));
            // 7/27 SUEYOSHI END

            if (!string.IsNullOrEmpty(workEntity.TargetSlipNo))
            {
                dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", workEntity.TargetSlipNo));
            }

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="coverSheetMasterDataRow">カバーシート情報DataRow</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateFillSARSlipDetailErrorParameterListNew(
                DbCommand dbCommand,
                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetMasterDataRow,
                WorkEntity workEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", coverSheetMasterDataRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", coverSheetMasterDataRow.BatchNo));
            // 7/27 SUEYOSHI ADD
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", coverSheetMasterDataRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", coverSheetMasterDataRow.SlipTypeCd));
            // 7/27 SUEYOSHI END

            if (!string.IsNullOrEmpty(workEntity.TargetSlipNo))
            {
                dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", workEntity.TargetSlipNo));
            }

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="coverSheetMasterDataRow">カバーシート情報DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateInsSARCoverSheetMasterParameterListNew(
                DbCommand dbCommand,
                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetMasterDataRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", coverSheetMasterDataRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchNo", coverSheetMasterDataRow.BatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", coverSheetMasterDataRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", coverSheetMasterDataRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", coverSheetMasterDataRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchDate", coverSheetMasterDataRow.BatchDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@TotalPageQty", coverSheetMasterDataRow.TotalPageQty));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputPageCount", coverSheetMasterDataRow.InputPageCount));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ErrorNo", coverSheetMasterDataRow.ErrorNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalCount", coverSheetMasterDataRow.ApprovalCount));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@PersonnelInput", coverSheetMasterDataRow.PersonnelInput));
            // 2013/09/25 アドオン対応：承認項目を追加 ISC:yshindo start
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApproverCd", coverSheetMasterDataRow.ApproverCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalStatus", coverSheetMasterDataRow.ApprovalStatus));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalDate", coverSheetMasterDataRow.ApprovalDate));
            // 2013/09/25 アドオン対応：承認項目を追加 ISC:yshindo end
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LogicalDelFlg", "0"));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LastUpdateDate", coverSheetMasterDataRow.LastUpdateDate));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="coverSheetMasterDataRow">カバーシート情報DataRow</param>
        /// <param name="updateDiv">UPDATE区分</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpSARCoverSheetMasterParameterListNew(
                DbCommand dbCommand,
                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetMasterDataRow,
                byte updateDiv)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", coverSheetMasterDataRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchNo", coverSheetMasterDataRow.BatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", coverSheetMasterDataRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", coverSheetMasterDataRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", coverSheetMasterDataRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchDate", coverSheetMasterDataRow.BatchDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@TotalPageQty", coverSheetMasterDataRow.TotalPageQty));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputPageCount", coverSheetMasterDataRow.InputPageCount));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LastUpdateDate", DateTime.Now));

            if (updateDiv == 2)
            {
                dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ErrorNo", coverSheetMasterDataRow.ErrorNo));
                dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalCount", coverSheetMasterDataRow.ApprovalCount));
            }

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="initEntity">初期表示Entity</param>
        /// <param name="coverSheetMasterDataRow">カバーシート情報DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateApprovalSARCoverSheetMasterParameterListNew(
                DbCommand dbCommand,
                InitialDisplayEntity initEntity,
                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetMasterDataRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", coverSheetMasterDataRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchNo", coverSheetMasterDataRow.BatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", coverSheetMasterDataRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", coverSheetMasterDataRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", coverSheetMasterDataRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchDate", coverSheetMasterDataRow.BatchDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalRequester", UserProfile.UserId));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@RequestApprovalDate", DateTime.Now));

            dbParameterList.Add(this.CreateDbParameterNew(
                dbCommand,
                "@ApprovalStatus",
                initEntity.APAvaliableDataSet.UTFAPVariable.FindByAPCorporationCodeAPVariableCode(
                    coverSheetMasterDataRow.CorporationCode,
                    APVariableListKey.SAR_APPROVALSTATUS_WAITING).APVariableValue));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LastUpdateDate", coverSheetMasterDataRow.LastUpdateDate));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="initEntity">初期表示Entity</param>
        /// <param name="slipDetailErrorRow">伝票明細情報DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateApprovalSARSlipHeaderErrorParameterListNew(
                DbCommand dbCommand,
                InitialDisplayEntity initEntity,
                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow slipDetailErrorRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalRequester", UserProfile.UserId));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@RequestApprovalDate", DateTime.Now));

            dbParameterList.Add(this.CreateDbParameterNew(
                dbCommand,
                "@ApprovalStatus",
                initEntity.APAvaliableDataSet.UTFAPVariable.FindByAPCorporationCodeAPVariableCode(
                    slipDetailErrorRow.CorporationCode,
                    APVariableListKey.SAR_APPROVALSTATUS_WAITING).APVariableValue));

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipDetailErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputMediaDiv", slipDetailErrorRow.InputMediaDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", slipDetailErrorRow.InputBatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputSequenceNo", slipDetailErrorRow.InputSequenceNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputDate", slipDetailErrorRow.InputDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputTime", slipDetailErrorRow.InputTime));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DataType", slipDetailErrorRow.DataType));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UptakeSequenceNo", slipDetailErrorRow.UptakeSequenceNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipDetailErrorRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipDetailErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipDetailErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipDetailErrorRow["SlipNo"].ToString()));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipDetailErrorRow">伝票明細情報DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateCorrectionDivSARSlipHeaderErrorParameterListNew(
                DbCommand dbCommand,
                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow slipDetailErrorRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipDetailErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputMediaDiv", slipDetailErrorRow.InputMediaDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", slipDetailErrorRow.InputBatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputSequenceNo", slipDetailErrorRow.InputSequenceNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputDate", slipDetailErrorRow.InputDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputTime", slipDetailErrorRow.InputTime));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DataType", slipDetailErrorRow.DataType));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UptakeSequenceNo", slipDetailErrorRow.UptakeSequenceNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipDetailErrorRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipDetailErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipDetailErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipDetailErrorRow["SlipNo"].ToString()));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipDetailErrorRow">伝票明細情報DataRow</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateCorrectionDivSARSlipHeaderParameterListNew(
                DbCommand dbCommand,
                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow slipDetailErrorRow,
                WorkEntity workEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipDetailErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipDetailErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipDetailErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipDetailErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipDetailErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipDetailErrorRow.StoreCd));
            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="coverSheetMasterEntity">カバーシート情報Entity</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateFillSARCoverSheetMasterParameterListNew(
                DbCommand dbCommand,
                CoverSheetMasterEntity coverSheetMasterEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            // 法人コード
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", coverSheetMasterEntity.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchNo", coverSheetMasterEntity.BatchNo));
            // 8/16 SUEYOSHI 抽出条件から削除
            // dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", coverSheetMasterEntity.StoreCd));
            // 8/16 SUEYOSHI END
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", coverSheetMasterEntity.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", coverSheetMasterEntity.SlipDiv));
            // 8/16 SUEYOSHI 抽出条件から削除
            // dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@BatchDate", coverSheetMasterEntity.BatchDate));
            // 8/16 SUEYOSHI END
            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipDetailErrorRow">伝票明細情報DataRow</param>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateInsertSARSlipDetailForDenyParameterListNew(
                DbCommand dbCommand,
                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow slipDetailErrorRow,
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow,
                WorkEntity workEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipDetailErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipLineNo", slipDetailErrorRow.SlipLineNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipDetailErrorRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipDetailErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipDetailErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipDetailErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipDetailErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipBranchNo", int.Parse(workEntity.SlipSubNo) - 1));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="nextSlipBranchNo">次の伝票番号枝番</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateInsertSARSlipHeaderForDenyParameterListNew(
                DbCommand dbCommand,
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow,
                WorkEntity workEntity,
                int nextSlipBranchNo)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipHeaderErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipHeaderErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipHeaderErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipHeaderErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@NextSlipBranchNo", nextSlipBranchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipHeaderErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipHeaderErrorRow.StoreCd));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipDetailErrorRow">伝票明細情報DataRow</param>
        /// <param name="newSlipLineNo">新しい伝票行番号</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpInsSARSlipDetailErrorParameterListNew(
                DbCommand dbCommand,
                // 2012.08.24 Start SlipLineNoの採番不具合修正
                //SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow slipDetailErrorRow)
                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow slipDetailErrorRow,
                int newSlipLineNo)
                // 2012.08.24 End   SlipLineNoの採番不具合修正
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipDetailErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputDate", slipDetailErrorRow.InputDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputMediaDiv", slipDetailErrorRow.InputMediaDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", slipDetailErrorRow.InputBatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputSequenceNo", slipDetailErrorRow.InputSequenceNo));
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            // 2012.08.24 Start SlipLineNoの採番不具合修正
            //dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputSequenceLineNo", slipDetailErrorRow.InputSequenceLineNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputSequenceLineNo", newSlipLineNo));
            // 2012.08.24 End   SlipLineNoの採番不具合修正
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputTime", slipDetailErrorRow.InputTime));
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DataType", slipDetailErrorRow.DataType));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UptakeSequenceNo", slipDetailErrorRow.UptakeSequenceNo));
            // 2012.08.24 Start SlipLineNoの採番不具合修正
            //dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UptakeSequenceLineNo", slipDetailErrorRow.UptakeSequenceLineNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UptakeSequenceLineNo", newSlipLineNo));
            // 2012.08.24 End   SlipLineNoの採番不具合修正
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
            // 2012.08.24 Start SlipLineNoの採番不具合修正
            //dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipLineNo", slipDetailErrorRow["SlipLineNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipLineNo", newSlipLineNo));
            // 2012.08.24 End   SlipLineNoの採番不具合修正
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipDetailErrorRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipDetailErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipDetailErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipDetailErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipDetailErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category1Cd", slipDetailErrorRow.Category1Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category2Cd", slipDetailErrorRow.Category2Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category3Cd", slipDetailErrorRow.Category3Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category4Cd", slipDetailErrorRow.Category4Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SKUCd", slipDetailErrorRow["SKUCd"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ScanningCd", slipDetailErrorRow["ScanningCd"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeQty", slipDetailErrorRow.DisposalChangeQty));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipOldPrice", slipDetailErrorRow.SlipOldPrice));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNewPrice", slipDetailErrorRow.SlipNewPrice));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDisposalUnitPrica", slipDetailErrorRow.SlipDisposalUnitPrica));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SalesChangeDiv", slipDetailErrorRow.SalesChangeDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Reason", slipDetailErrorRow.Reason));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SalesChangePrice", slipDetailErrorRow["SalesChangePrice"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalPriceAmt", slipDetailErrorRow["DisposalPriceAmt"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute01", slipDetailErrorRow["Attribute01"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute02", slipDetailErrorRow["Attribute02"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute03", slipDetailErrorRow["Attribute03"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute04", slipDetailErrorRow["Attribute04"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute05", slipDetailErrorRow["Attribute05"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute06", slipDetailErrorRow["Attribute06"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute07", slipDetailErrorRow["Attribute07"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute08", slipDetailErrorRow["Attribute08"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute09", slipDetailErrorRow["Attribute09"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute10", slipDetailErrorRow["Attribute10"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LogicalDelFlg", slipDetailErrorRow.LogicalDelFlg));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <param name="initEntity">初期表示Entity</param>
        /// <param name="nextSlipBranchNo">次の伝票番号枝番</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateInsertSARSlipHeaderParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow,
                InitialDisplayEntity initEntity,
                int nextSlipBranchNo)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipHeaderErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipHeaderErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipHeaderErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipHeaderErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipBranchNo", nextSlipBranchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipHeaderErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipHeaderErrorRow.StoreCd));

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category1Cd", slipHeaderErrorRow.Category1Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category2Cd", slipHeaderErrorRow.Category2Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@TotalQty", slipHeaderErrorRow.TotalQty));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipOldTotalPrice", slipHeaderErrorRow["SlipOldTotalPrice"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNewTotalPrice", slipHeaderErrorRow["SlipNewTotalPrice"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@AutoSalesChangeFlg", slipHeaderErrorRow.AutoSalesChangeFlg));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTotalDisposalAmt", slipHeaderErrorRow["SlipTotalDisposalAmt"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDate", slipHeaderErrorRow["SlipDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@PostingDate", initEntity.SystemDateDataset.CMCSystemDate[0].SystemDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalRequester", slipHeaderErrorRow["ApprovalRequester"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@RequestApprovalDate", slipHeaderErrorRow["RequestApprovalDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApproverCd", slipHeaderErrorRow.ApproverCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalStatus", slipHeaderErrorRow.ApprovalStatus));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalDate", slipHeaderErrorRow["ApprovalDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputMediaDiv", slipHeaderErrorRow.InputMediaDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", slipHeaderErrorRow.InputBatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputSequenceNo", slipHeaderErrorRow.InputSequenceNo));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="slipDetailErrorRow">.伝票明細更新DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateInsertSARSlipDetailParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow slipDetailErrorRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipDetailErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipLineNo", slipDetailErrorRow["SlipLineNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipDetailErrorRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipDetailErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipBranchNo", workEntity.SlipSubNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipDetailErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipDetailErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipDetailErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category1Cd", slipDetailErrorRow.Category1Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category2Cd", slipDetailErrorRow.Category2Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category3Cd", slipDetailErrorRow.Category3Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category4Cd", slipDetailErrorRow.Category4Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SKUCd", slipDetailErrorRow["SKUCd"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ScanningCd", slipDetailErrorRow["ScanningCd"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeQty", slipDetailErrorRow.DisposalChangeQty));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipOldPrice", slipDetailErrorRow.SlipOldPrice));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNewPrice", slipDetailErrorRow.SlipNewPrice));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDisposalUnitPrica", slipDetailErrorRow.SlipDisposalUnitPrica));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SalesChangeDiv", slipDetailErrorRow.SalesChangeDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Reason", slipDetailErrorRow.Reason));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SalesChangePrice", slipDetailErrorRow["SalesChangePrice"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalPriceAmt", slipDetailErrorRow["DisposalPriceAmt"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute01", slipDetailErrorRow["Attribute01"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute02", slipDetailErrorRow["Attribute02"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute03", slipDetailErrorRow["Attribute03"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute04", slipDetailErrorRow["Attribute04"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute05", slipDetailErrorRow["Attribute05"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute06", slipDetailErrorRow["Attribute06"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute07", slipDetailErrorRow["Attribute07"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute08", slipDetailErrorRow["Attribute08"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute09", slipDetailErrorRow["Attribute09"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute10", slipDetailErrorRow["Attribute10"].ToString()));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateInsertSARSlipHeaderErrorParameterListNew(
                DbCommand dbCommand,
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipHeaderErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputMediaDiv", slipHeaderErrorRow.InputMediaDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", slipHeaderErrorRow.InputBatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputSequenceNo", slipHeaderErrorRow.InputSequenceNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputDate", slipHeaderErrorRow.InputDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputTime", slipHeaderErrorRow.InputTime));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DataType", slipHeaderErrorRow["DataType"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UptakeSequenceNo", slipHeaderErrorRow.UptakeSequenceNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipHeaderErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipHeaderErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipHeaderErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipHeaderErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipHeaderErrorRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category1Cd", slipHeaderErrorRow.Category1Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category2Cd", slipHeaderErrorRow.Category2Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@TotalQty", slipHeaderErrorRow.TotalQty));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipOldTotalPrice", slipHeaderErrorRow["SlipOldTotalPrice"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNewTotalPrice", slipHeaderErrorRow["SlipNewTotalPrice"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@AutoSalesChangeFlg", slipHeaderErrorRow.AutoSalesChangeFlg));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTotalDisposalAmt", slipHeaderErrorRow["SlipTotalDisposalAmt"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDate", slipHeaderErrorRow["SlipDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalRequester", slipHeaderErrorRow["ApprovalRequester"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@RequestApprovalDate", slipHeaderErrorRow["RequestApprovalDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApproverCd", slipHeaderErrorRow.ApproverCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalStatus", slipHeaderErrorRow.ApprovalStatus));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalDate", slipHeaderErrorRow["ApprovalDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ErrorDiv", slipHeaderErrorRow.ErrorDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ErrorMessage", slipHeaderErrorRow["ErrorMessage"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorrectionDiv", slipHeaderErrorRow.CorrectionDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@FixDate", slipHeaderErrorRow["FixDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute01", slipHeaderErrorRow["Attribute01"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute02", slipHeaderErrorRow["Attribute02"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute03", slipHeaderErrorRow["Attribute03"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute04", slipHeaderErrorRow["Attribute04"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute05", slipHeaderErrorRow["Attribute05"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute06", slipHeaderErrorRow["Attribute06"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute07", slipHeaderErrorRow["Attribute07"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute08", slipHeaderErrorRow["Attribute08"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute09", slipHeaderErrorRow["Attribute09"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute10", slipHeaderErrorRow["Attribute10"].ToString()));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateSARSlipHeaderErrorParameterListNew(
                DbCommand dbCommand,
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category1Cd", slipHeaderErrorRow.Category1Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Category2Cd", slipHeaderErrorRow.Category2Cd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@TotalQty", slipHeaderErrorRow.TotalQty));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipOldTotalPrice", slipHeaderErrorRow["SlipOldTotalPrice"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNewTotalPrice", slipHeaderErrorRow["SlipNewTotalPrice"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@AutoSalesChangeFlg", slipHeaderErrorRow.AutoSalesChangeFlg));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTotalDisposalAmt", slipHeaderErrorRow["SlipTotalDisposalAmt"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDate", slipHeaderErrorRow["SlipDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalRequester", slipHeaderErrorRow["ApprovalRequester"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@RequestApprovalDate", slipHeaderErrorRow["RequestApprovalDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApproverCd", slipHeaderErrorRow.ApproverCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalStatus", slipHeaderErrorRow.ApprovalStatus));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ApprovalDate", slipHeaderErrorRow["ApprovalDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ErrorDiv", slipHeaderErrorRow.ErrorDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@ErrorMessage", slipHeaderErrorRow["ErrorMessage"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorrectionDiv", slipHeaderErrorRow.CorrectionDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@FixDate", slipHeaderErrorRow.FixDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute01", slipHeaderErrorRow["Attribute01"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute02", slipHeaderErrorRow["Attribute02"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute03", slipHeaderErrorRow["Attribute03"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute04", slipHeaderErrorRow["Attribute04"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute05", slipHeaderErrorRow["Attribute05"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute06", slipHeaderErrorRow["Attribute06"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute07", slipHeaderErrorRow["Attribute07"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute08", slipHeaderErrorRow["Attribute08"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute09", slipHeaderErrorRow["Attribute09"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@Attribute10", slipHeaderErrorRow["Attribute10"].ToString()));

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipHeaderErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputMediaDiv", slipHeaderErrorRow.InputMediaDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", slipHeaderErrorRow.InputBatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputSequenceNo", slipHeaderErrorRow.InputSequenceNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputDate", slipHeaderErrorRow.InputDate));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputTime", slipHeaderErrorRow.InputTime));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DataType", slipHeaderErrorRow["DataType"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UptakeSequenceNo", slipHeaderErrorRow.UptakeSequenceNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipHeaderErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipHeaderErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipHeaderErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipHeaderErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipHeaderErrorRow.StoreCd));

            return dbParameterList.ToArray();
        }

#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 -->
        /// <summary>
        /// ＳＡＲ伝票ヘッダーの更新ためのクエリを作成します。
        /// </summary>
        /// <returns>Query string</returns>
        private string CreateUpdateSARSlipHeaderQueryTextNew()
        {
            StringBuilder updateCommandText = new StringBuilder();

            updateCommandText.AppendLine(" UPDATE SARSlipHeader ");
            updateCommandText.AppendLine("    SET SlipRevisionFlg = @SlipRevisionFlg ");
            //updateCommandText.AppendLine("        LogicalDelFlg = '1',");
            //updateCommandText.AppendLine("        LogicalDelDate = @LogicalDelDate, ");
            //updateCommandText.AppendLine("        UpdateBy = @UpdateBy, ");
            //updateCommandText.AppendLine("        UpdateDate = @UpdateDate ");
            updateCommandText.AppendLine(" WHERE");
            updateCommandText.AppendLine("        CorporationCode = @CorporationCode");
            updateCommandText.AppendLine("   AND  SlipDiv = @SlipDiv");
            updateCommandText.AppendLine("   AND  SlipTypeCd = @SlipTypeCd");
            updateCommandText.AppendLine("   AND  SlipNo = @SlipNo");
            updateCommandText.AppendLine("   AND  SlipBranchNo = @SlipBranchNo");
            updateCommandText.AppendLine("   AND  DisposalChangeDate = @DisposalChangeDate");
            updateCommandText.AppendLine("   AND  StoreCd = @StoreCd");

            return updateCommandText.ToString();
        }
#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 <--

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateGetNextSlipBranchNoParameterListNew(
                DbCommand dbCommand,
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipHeaderErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipHeaderErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipHeaderErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipHeaderErrorRow["SlipNo"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipHeaderErrorRow["DisposalChangeDate"].ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipHeaderErrorRow.StoreCd));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateFillSARCoverSheetMasterForUpdateParameterListNew(
                DbCommand dbCommand,
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipHeaderErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", slipHeaderErrorRow.InputBatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipHeaderErrorRow.StoreCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipHeaderErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipHeaderErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipHeaderErrorRow["DisposalChangeDate"].ToString()));

            return dbParameterList.ToArray();
        }

        /// <summary>
        /// DbParameterの集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="parameterName">パラメータ名</param>
        /// <param name="parameterValue">パラメータ値</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter CreateDbParameterNew(DbCommand dbCommand, string parameterName, object parameterValue)
        {
            DbParameter dbParameter = dbCommand.CreateParameter();

            dbParameter.ParameterName = parameterName;
            dbParameter.Value = parameterValue;

            return dbParameter;
        }

        /// <summary>
        /// Set common items (CreateBy, CreateDate, UpdateBy, UpdateDate) for insertion.
        /// </summary>
        /// <param name="command">DbCommandオブジェクト</param>
        private void SetCommonItemForInsertNew(DbCommand command)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(command, "@CreateBy", UserProfile.UserId));
            dbParameterList.Add(this.CreateDbParameterNew(command, "@CreateDate", DateTime.Now));
            dbParameterList.Add(this.CreateDbParameterNew(command, "@UpdateBy", UserProfile.UserId));
            dbParameterList.Add(this.CreateDbParameterNew(command, "@UpdateDate", DateTime.Now));

            // パラメータの設定
            command.Parameters.AddRange(dbParameterList.ToArray());
        }

        /// <summary>
        /// Set common items ( UpdateBy, UpdateDate) for update.
        /// </summary>
        /// <param name="command">DbCommandオブジェクト</param>
        private void SetCommonItemForUpdateNew(DbCommand command)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(command, "@UpdateBy", UserProfile.UserId));
            dbParameterList.Add(this.CreateDbParameterNew(command, "@UpdateDate", DateTime.Now));

            // パラメータの設定
            command.Parameters.AddRange(dbParameterList.ToArray());
        }

#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 -->
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="slipHeaderErrorRow">伝票ヘッダ更新DataRow</param>
        /// <param name="initEntity">初期表示Entity</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateSARSlipHeaderParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow slipHeaderErrorRow,
                InitialDisplayEntity initEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            if (workEntity.SlipRevisionFlg == null || workEntity.SlipRevisionFlg.Equals(""))
            {
                workEntity.SlipRevisionFlg = "0";
            }
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipRevisionFlg", workEntity.SlipRevisionFlg));
            //dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LogicalDelDate", DateTime.Now));
            //dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UpdateBy", UserProfile.UserId));
            //dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@UpdateDate", DateTime.Now));

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", slipHeaderErrorRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", slipHeaderErrorRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", slipHeaderErrorRow.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", slipHeaderErrorRow.SlipNo.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipBranchNo", workEntity.SlipSubNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", slipHeaderErrorRow.DisposalChangeDate.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", slipHeaderErrorRow.StoreCd));

            return dbParameterList.ToArray();
        }
#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 <--

        // 2014.01.07 Start 対象店舗の営業時間帯を取得出来ない場合は、９９９店舗の時間帯を取得する。
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="corporationCode">法人コード</param>
        /// <param name="storeCd">店舗コード</param>
        /// <param name="workTime">対象時刻</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] SelectGetBusinessPeriodParameterListNew(
                DbCommand dbCommand,
                string corporationCode,
                string storeCd,
                string workTime)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", corporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", storeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DealPeriod", workTime));

            return dbParameterList.ToArray();
        }
        // 2014.01.07 End   対象店舗の営業時間帯を取得出来ない場合は、９９９店舗の時間帯を取得する。

        #region ＳＡＲ伝票ヘッダに訂正不可フラグをセット
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// ＳＡＲ伝票ヘッダに訂正不可フラグをセット
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateSlipRevisionFlgSARSlipHeaderParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                DsEntity dsEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerError = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", headerError.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", headerError.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", headerError.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", headerError.SlipNo.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", headerError.DisposalChangeDate.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", headerError.StoreCd));

            return dbParameterList.ToArray();
        }
        #endregion

        #region ＳＡＲ伝票ヘッダエラーにFixDateをセット
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// ＳＡＲ伝票ヘッダエラーにFixDateをセット
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateFixDateSARSlipHeaderErrorParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                DsEntity dsEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerError = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", headerError.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", headerError.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", headerError.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", headerError.SlipNo.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", headerError.DisposalChangeDate.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", headerError.StoreCd));

            return dbParameterList.ToArray();
        }
        #endregion
        #region ＳＡＲ伝票ヘッダエラーに削除フラグをセット
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// ＳＡＲ伝票ヘッダエラーに削除フラグをセット
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateDelFlgSARSlipHeaderErrorParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                DsEntity dsEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerError = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];
            
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LogicalDelDate", DateTime.Now));

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", headerError.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", headerError.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", headerError.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", headerError.SlipNo.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", headerError.DisposalChangeDate.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", headerError.StoreCd));

            return dbParameterList.ToArray();
        }
        #endregion

        #region ＳＡＲ伝票明細エラーに削除フラグをセット
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// ＳＡＲ伝票明細エラーに削除フラグをセット
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateDelFlgSARSlipDetailErrorParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                DsEntity dsEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            // ヘッダー情報で明細を更新する
            SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerError = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LogicalDelDate", DateTime.Now));

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", headerError.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", headerError.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", headerError.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", headerError.SlipNo.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", headerError.DisposalChangeDate.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", headerError.StoreCd));

            return dbParameterList.ToArray();
        }
        #endregion

        #region ＳＡＲ伝票ヘッダに削除フラグをセット
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// ＳＡＲ伝票ヘッダに削除フラグをセット
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateDelFlgSARSlipHeaderParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                DsEntity dsEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerError = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LogicalDelDate", DateTime.Now));

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", headerError.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", headerError.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", headerError.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", headerError.SlipNo.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", headerError.DisposalChangeDate.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", headerError.StoreCd));

            return dbParameterList.ToArray();
        }
        #endregion

        #region ＳＡＲ伝票明細に削除フラグをセット
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// ＳＡＲ伝票明細に削除フラグをセット
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <param name="dsEntity">DsEntity</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateUpdateDelFlgSARSlipDetailParameterListNew(
                DbCommand dbCommand,
                WorkEntity workEntity,
                DsEntity dsEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            // ヘッダー情報で明細を更新する
            SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerError = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@LogicalDelDate", DateTime.Now));

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", headerError.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", headerError.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", headerError.SlipTypeCd));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", headerError.SlipNo.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@DisposalChangeDate", headerError.DisposalChangeDate.ToString()));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@StoreCd", headerError.StoreCd));

            return dbParameterList.ToArray();
        }
        #endregion

        // 2014.03.18 Start 効率改善対応（商品名取得変更）
        /// <summary>
        /// DbParameter一覧の集合を作成します。
        /// </summary>
        /// <param name="dbCommand">DBコマンド</param>
        /// <param name="coverSheetMasterDataRow">カバーシート情報DataRow</param>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>DBパラメータ</returns>
        private DbParameter[] CreateFillSARSlipDetailErrorGetProductsKanjiParameterListNew(
                DbCommand dbCommand,
                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetMasterDataRow,
                WorkEntity workEntity)
        {
            List<DbParameter> dbParameterList = new List<DbParameter>();

            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@CorporationCode", coverSheetMasterDataRow.CorporationCode));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@InputBatchNo", coverSheetMasterDataRow.BatchNo));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipDiv", coverSheetMasterDataRow.SlipDiv));
            dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipTypeCd", coverSheetMasterDataRow.SlipTypeCd));

            if (!string.IsNullOrEmpty(workEntity.TargetSlipNo))
            {
                dbParameterList.Add(this.CreateDbParameterNew(dbCommand, "@SlipNo", workEntity.TargetSlipNo));
            }

            return dbParameterList.ToArray();
        }
        // 2014.03.18 End   効率改善対応（商品名取得変更）

	
		//2019/02/20 k.sugawara Start タイムアウト値を設定
        #region SQLコマンドタイムアウト値を取得します（AP変数）
        /// <summary>
        /// SQLコマンドタイムアウト値を取得します（AP変数）
        /// </summary>
        /// <param name="corporationCode">会社コード</param>
        /// <returns>SQLコマンドタイムアウト値</returns>
        private int GetSqlCommandTimeoutValue(string corporationCode)
        {
            // AP変数値を取得
            string apVariableValue = APVariableUtil.GetAPVariableValue(
                APVariableListKey.STR_SQL_COMMAND_TIMEOUT_VALUE, corporationCode);

            return Convert.ToInt32(apVariableValue);
        }
        #endregion
		//2019/02/20 k.sugawara End タイムアウト値を設定
        #endregion
    }
}
