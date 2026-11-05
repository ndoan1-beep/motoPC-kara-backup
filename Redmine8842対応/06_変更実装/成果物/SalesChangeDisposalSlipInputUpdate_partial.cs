using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Infragistics.Web.UI.GridControls;

using NihonUnisys.CoreCenter.Core;
using NihonUnisys.CoreCenter.Core.Resources;
using NihonUnisys.CoreCenter.Core.User;
using NihonUnisys.CoreCenter.DistributionCommon.Common.MasterBusiness.DataTransferObject;
using NihonUnisys.CoreCenter.Retail.Common;
using NihonUnisys.CoreCenter.Retail.Common.Resources;
using NihonUnisys.CoreCenter.Retail.Common.PublicBusiness.Business;
using NihonUnisys.CoreCenter.Retail.Sales.InputSalesSlip.Business;
using NihonUnisys.CoreCenter.Retail.Sales.InputSalesSlip.Facade;
using NihonUnisys.CoreCenter.Retail.SalesTalking.ItemsListing.Business;
using NihonUnisys.CoreCenter.Web.Controls;
using NihonUnisys.CoreCenter.Web.Controls.Containers;
using NihonUnisys.CoreCenter.Web.Controls.Utilities;
using System.Drawing;
// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
using NihonUnisys.CoreCenter.Retail.SalesTalking.PublicBusiness.Business;
// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
using NihonUnisys.CoreCenter.Core.Validation;
using System.Web.UI.WebControls;
using NihonUnisys.CoreCenter.Retail.Common.Category.Web;


#region 【修正履歴】
// 【修正履歴】
//   2011.11.28 小林 商品が未存在の場合はExceptionを発生させる
//
//   2011.11.28 小林 入力チェックの強化
//
//   2011.11.29 小林 子画面対応
//
//   2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新
//
//   2011.12.07 関根 伝票明細が０件の時のメッセージを変更
//
//   2011.12.10 関根 新規追加クリアボタン押下のクリア処理修正(text boxのみクリア)
//
//   2011.12.10 関根 クリアボタン押下のクリア処理修正
//
//   2011.12.10 関根 新規追加クリアボタン押下時の明細行が削除される不具合修正
//
//   2011.12.21 関根 伝票取消エラーの障害対応（新規登録時）
//
//   2012.01.04 関根 伝票検索画面から遷移時にカバー情報を編集不可にする
//
//   2012.01.04 関根 伝票検索から遷移時、店舗名称／部門名称を表示する
//
//   2012.01.05 関根 売単価のクリア
//
//   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応
//
//   2012.01.13 関根 明細の数量が変更出来ない不具合対応（伝票検索から遷移時）
//
//   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応
//
//   2012.01.16 関根 数量変更時の合計金額が計算されない不具合対応
//
//   2012.01.21 関根 伝票取消時の障害対応
//
//   2012.06.13 小林 廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。
//
//   2012.06.21 小林 売変選択時に画面の旧売価が０円で表示されているので修正
//
//   2012.06.21 小林 売変用の事由を追加（値下げ、値上げの２パターン分）
//
//   2012.06.26 小林 クリアされていない箇所がある
//
//   2012.06.26 小林 伝票明細情報の数値項目に小数点以下が表示されるので表示しない。
//
//   2012.06.26 小林 カバー保存後に伝票ヘッダー、明細が表示されることがある。(前回分を表示している)
//
//   2012.06.26 小林 伝票ヘッダー情報の入力制御を修正
//
//   2012.06.26 小林 セッションのDsTmpEntityを使用している箇所を少し変更
//
//   2012.06.26 小林 商品を選択した場合は商品コードの変更は不可とする。
//
//   2012.06.26 小林 入力エリアのクリア箇所を変更
//
//   2012.06.26 小林 新規ボタンを押してもカバーシートの保存状態は残す。
//
//   2012.06.26 小林 １カバーシートに複数伝票登録できないので登録できるように修正。
//
//   2012.06.26 小林 店舗コードをセットする内容の間違え
//
//   2012.06.26 小林 入力ＳＥＱ番号の対応漏れの対応
//
//   2012.06.26 小林 伝票取消対応(削除フラグをセットする)
//
//   2012.06.26 小林 選択し内容を変更した際にSEQNoが元に戻るので修正
//
//   2012.06.26 小林 カバーシートの入力枚数を画面入力値へ変更
//
//   2012.06.26 小林 カバーシートの入力者を10桁までとしてセットする
//
//   2012.08.02 MHBranchでの修正分を標準Branchへマージ
//
//   2012.08.27 新規入力時のＤＢへ更新前の行削除ではＤＢへ更新せず行のみを削除する
//
//   2012.08.27 行削除時に伝票状況を表示し直す
//
//   2012.08.27 店舗名称、部門名称取得時の日付はバッチ日付ではなく伝票日付で取得する
//
//   2012.08.27 行削除時の全行削除は業務エラーメッセージを表示する
//
//   2012.10.17 商品コードが入力されていない場合は部門代表商品とするようにする
//
//   2012.10.17 廃棄、売変の金額入力をＡＰ変数で制御するようにする
//
//   2012.10.26 廃棄入力中に選択ボタンを押すと一覧が売変用に変わってしまうので修正
//
//   2012.11.01 売変伝票の場合に旧売価項目を追加
//
//   2012.11.02 GridへBindしたあとの売変、廃棄のGrid調整を追加
//
//   2013.03.15 数量欄を入力可能へ変更
//
//   2013.11.26 カバシートを保存していない場合は、「保存」「伝票取消」ボタンを押せないようにする。（そのまま保存するとエラーとなる為）
//
//   2013.11.26 関根 １カバーシートに複数伝票登録した場合に、明細グリッドに全伝票分の明細が出力される不具合を修正
//
//   2013.12.06 関根 明細行削除時も保存処理と同様で、認証モードをチェックするように修正
//
//   2014.01.06 関根 伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。
//
//   2014.01.09 関根 店舗ユーザーの場合は、店舗コードを変更不可にする。
//
//   2014.03.18 関根 効率改善対応（Validatorコメント）
//
//   2014.03.18 関根 効率改善対応（伝票情報取得を複数回行っている処理を１回のみにする）
//
//   2014.04.17 関根 カバーシート検索画面へのリンク対応
//
//   2014.04.24 SERIO杉岡 カバーシート検索、売変廃棄伝票検索リンク表示制御
//
//   2014.04.30 関根@豊洲 行削除のエラー対応（行削除→クリア→行削除の順に行うと同一伝票が新規で登録されてしまう）
//
//   2014.04.30 関根@豊洲 行番号が消える現象対応
//
//   2014.04.30 関根@豊洲 店舗コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
//
//   2014.04.30 関根@豊洲 クリアボタン押下時は、検索前情報DataSet(OriginalDetailDataSet)も初期化する。
//
//   2014.04.30 関根@豊洲 １カバーシートに複数伝票登録した状態で、同一伝票を続けて保存すると全伝票分の明細が更新される不具合修正
//
//   2014.04.30 関根@豊洲 保存(F12)押下時は、ヘッダ情報は変更不可とする
//
//   2014.04.30 関根@豊洲 参照ボタンの使用可否設定追加
//
//   2014.04.30 関根@豊洲 クリアボタン押下時は、検索前情報DataSet(OriginalDetailDataSet)も初期化する。
//
//   2014.05.09 小林 再計算ボタン追加
//
//   2014.05.16 小林 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
//
//   2014.05.17 小林 ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
//
//   2014.05.27 小林 Gridの数量チェックを強化
//
//   2014.05.28 小林 クリアボタン押下で伝票番号が消えるので修正
//
//   2014.05.28 小林 カバー保存ボタンはカバーシート部分の入力の時のみ押下可能にする
//
//   2014.05.28 小林 明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
//
//   2014.05.28 小林 部門代表コードの部分を機能しないように修正(売価の変更をできないようにしているので部門代表を取得しても単価が変更できないため)
//
//   2014.05.28 小林 伝票取消を行うとカバーシートの伝票一覧より番号(No)が消えてしまうので修正
//
//   2014.05.28 小林 部門を空でフォーカスを移動すると先頭の部門名称が表示されてしまう
//
//   2014.08.04 関根 行削除時の未選択エラーメッセージ変更   
//
//   2015.10.28 李   明細にて直接編集へ変更             

#endregion

namespace NihonUnisys.CoreCenter.Retail.Sales.InputSalesSlip.Web
{
    /// <summary>
    /// 画面 ID : SCRCSAR050202
    /// 売変廃棄伝票起票
    /// </summary>
    public partial class SalesChangeDisposalSlipInputUpdate
    {
        #region DsEntitySession
        /// <summary>
        /// Gets or sets the get ds entity session.
        /// </summary>
        /// <value>
        /// The get ds entity session.
        /// </value>
        private DsEntity DsEntitySession
        {
            get { return (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity); }
            set { this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, value); }
        }
        #endregion

        #region OnInitComplete (イベントハンドラを追加する為に、OnInitCompleteイベントハンドラをオーバーライドする。)
        /// <summary>
        /// イベントハンドラを追加する為に、OnInitCompleteイベントハンドラをオーバーライドする。
        /// </summary>
        /// <param name="e">イベント引数。</param>
        protected override void OnInitComplete(EventArgs e)
        {
            // カバー保存ボタンにSaveCoverButton_Clickイベントハンドラを追加する。
            this.SaveCoverButton.Click += new EventHandler(this.SaveCoverButton_Click);

            // 承認依頼ボタンにApprovalRequestButton_Clickイベントハンドラを追加する。
            this.ApprovalRequestButton.Click += new EventHandler(this.ApprovalRequestButton_Click);

            // 次へボタンにNextCoverButton_Clickイベントハンドラを追加する。
            this.NextCoverButton.Click += new EventHandler(this.NextCoverButton_Click);

            // 新規追加ボタンにDetailAddButton_Clickイベントハンドラを追加する。
            this.DetailAddButton.Click += new EventHandler(this.DetailAddButton_Click);

            // 明細削除ボタンにDetailDeleateButton_Clickイベントハンドラを追加する。
            this.DetailDeleateButton.Click += new EventHandler(this.DetailDeleateButton_Click);

            // 選択ボタンにDetailChoiceButton_Clickイベントハンドラを追加する。
            //this.DetailChoiceButton.Click += new EventHandler(this.DetailChoiceButton_Click);

            // 2014.05.28 Start クリアボタン押下で伝票番号が消えるので修正
            // 明細のクリア処理の方法をクリア：F3へ移行
            //// 明細クリアボタンにDetailClearButton_Clickイベントハンドラを追加する。
            //this.DetailClearButton.Click += new EventHandler(this.DetailClearButton_Click);
            // 2014.05.28 End   クリアボタン押下で伝票番号が消えるので修正

            // 次へボタンにNextButton_Clickイベントハンドラを追加する。
            this.NextButton.Click += new EventHandler(this.NextButton_Click);

            // 戻るボタンにReturnButton_Clickイベントハンドラを追加する。
            this.ReturnButton.Click += new EventHandler(this.ReturnButton_Click);

            // クリアボタンにClearButton_Clickイベントハンドラを追加する。
            // 2014.05.28 Start クリアボタン押下で伝票番号が消えるので修正
            //this.ClearButton.Click += new EventHandler(this.ClearButton_Click);
            this.ClearButton.Click += new EventHandler(this.DetailClearButton_Click);
            // 2014.05.28 End   クリアボタン押下で伝票番号が消えるので修正

            // 伝票取消ボタンにSlipDeleateButton_Clickイベントハンドラを追加する。
            this.SlipDeleateButton.Click += new EventHandler(this.SlipDeleateButton_Click);

            // 入力状況一覧リスト項目にinputStatusListGrid_ItemCommandイベントハンドラを追加する。
            this.inputStatusListGrid.ItemCommand += new ItemCommandEventHandler(this.InputStatusListGrid_ItemCommand);

            // 新規ボタンにNewButton_Clickイベントハンドラを追加する。
            this.NewButton.Click += new EventHandler(this.NewButton_Click);

            // 全選択ボタンにSelectAllButton_Clickイベントハンドラを追加する。
            this.SelectAllButton.Click += new EventHandler(this.SelectAllButton_Click);

            // 全解除ボタンにCancelAllButton_Clickイベントハンドラを追加する。
            this.CancelAllButton.Click += new EventHandler(this.CancelAllButton_Click);

            // 店舗参照ボタンにStoreReferButton_Clickイベントハンドラを追加する。
            this.StoreReferButton.Click += new EventHandler(this.StoreReferButton_Click);

            // 分類１参照ボタンにCategory1ReferButton_Clickイベントハンドラを追加する。
            this.Category1ReferButton.Click += new EventHandler(this.Category1ReferButton_Click);

            // 分類２参照ボタンにCategory2ReferButton_Clickイベントハンドラを追加する。
            this.Category2ReferButton.Click += new EventHandler(this.Category2ReferButton_Click);

            this.SlipTypeCoverDropDownList.SelectedIndexChanged += new EventHandler(this.SlipTypeCoverDropDownList_SelectedIndexChanged);
            this.SlipDivisionCoverDropDownList.SelectedIndexChanged += new EventHandler(this.SlipDivisionCoverDropDownList_SelectedIndexChanged);

            // PageLinkButtonボタンにPageLinkButton_Clickイベントを登録する。
            this.PageDatePager.NavigatePage += new EventHandler<NavigatePageEventArgs>(this.PageDatePager_NavigatePage);

            this.SetConfirmMessage();

            // 商品検索ボタン
            this.SKUSerchButton.Click += new EventHandler(this.SKUSerchButton_Click);

            // 暫定修正 2011.11.28 Start 入力チェックの強化
            //デザインでイベントをあてているのでここのはコメントアウト
            //// SKUコード
            //this.DetailAddSKUCd.TextChanged += new EventHandler(this.DetailAddSKUCd_TextChanged);
            // 暫定修正 2011.11.28 End   入力チェックの強化

            // 売変廃棄伝票検索画面リンクボタンにDisposalSlipSearchLinkButton_Clickイベントハンドラを追加する。
            this.DisposalSlipSearchLinkButton.Click += new EventHandler(this.DisposalSlipSearchLinkButton_Click);


            // 2014.04.17 Start カバーシート検索画面へのリンク対応
            // カバーシート検索画面リンクボタンにDisposalSlipSearchLinkButton_Clickイベントハンドラを追加する。
            this.CoverSheetSearchLinkButton.Click += new EventHandler(this.CoverSheetSearchLinkButton_Click);
            // 2014.04.17 End   カバーシート検索画面へのリンク対応

            // 2014.05.09 Start 効率改善指摘事項の修正
            // 再計算ボタンにReCulculateButton_Clickイベントハンドラを追加する。
            this.ReCulculateButton.Click += new EventHandler(this.ReCulculateButton_Click);
            // 2014.05.09 End   効率改善指摘事項の修正

            // 2014.06.06 Start 破棄メッセージ統一対応
            // 戻る
            this.ReturnButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0027);

            // クリア
            this.ClearButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0027);
            // 2014.06.06 End   破棄メッセージ統一対応

            // ベースクラスのOnInitComplete(e)を呼び出す
            base.OnInitComplete(e);
        }
        #endregion

        #region SlipDivisionCoverDropDownList_SelectedIndexChanged
        /// <summary>
        /// SlipDivisionCoverDropDownList_SelectedIndexChanged
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベントデータ</param>
        protected void SlipDivisionCoverDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //20151006 start グリッドビューにて直接編集仕様へ変更によりコメントアウト　ri
            //// 2012.06.21 Start 売変用の事由を追加（値下げ、値上げの２パターン分）
            //string slipDivisionValue = this.SlipDivisionCoverDropDownList.SelectedValue;

            //InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);

            //if (this.SlipTypeCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChange)
            //{
            //    // 8/9 SUEYOSHI 売変区分をAP変数から取得するように修正
            //    // if (slipDivisionValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangePriceDown))
            //    if (slipDivisionValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice))
            //    // 8/9 SUEYOSHI END
            //    {
            //        // 値下げ
            //        // 事由ドロップダウンリスト
            //        this.DetailAddReason.DataSource = initialDisplayEntity.ReasonSalesChangePriceDownListDictionary;
            //        this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
            //        this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
            //        this.DetailAddReason.DataBind();
            //    }
            //    // 8/9 SUEYOSHI 売変区分をAP変数から取得するように修正 START
            //    // else if (slipDivisionValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangePriceUp))
            //    // 8/9 SUEYOSHI END
            //    else
            //    {
            //        // 値上げ
            //        // 事由ドロップダウンリスト
            //        this.DetailAddReason.DataSource = initialDisplayEntity.ReasonSalesChangePriceUpListDictionary;
            //        this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
            //        this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
            //        this.DetailAddReason.DataBind();
            //    }
            //}
            //else if (this.SlipTypeCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.Disposal)
            //{
            //    // 事由ドロップダウンリスト（廃棄用）
            //    this.DetailAddReason.DataSource = initialDisplayEntity.ReasonListDictionary;
            //    this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
            //    this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
            //    this.DetailAddReason.DataBind();
            //}
            //// 2012.06.21 End   売変用の事由を追加（値下げ、値上げの２パターン分）
            //20151006 end グリッドビューにて直接編集仕様へ変更によりコメントアウト　ri

            this.SetMainHeader();


            // 2014.06.14 Start 品質改善検証の指摘事項対応
            // 伝票区分指定後は、バッチ番号にフォーカス移動
            this.BatchNumberTextBox.Focus();
            // 2014.06.14 End   品質改善検証の指摘事項対応
        }
        #endregion

        #region DisposalSlipSearchLinkButton_Click (売変廃棄伝票検索画面リンク)
        /// <summary>
        /// 売変廃棄伝票検索画面リンク
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベントデータ</param>
        protected void DisposalSlipSearchLinkButton_Click(object sender, EventArgs e)
        {
            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);

            if (workEntity.InitMode == "1")
            {
                // 法人コード←WorkEntity.法人コード
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.CorporationCode, workEntity.CorporationCode);

                // バッチ番号←画面上(カバーシート情報)のバッチ番号
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.BatchNumber, this.BatchNumberTextBox.Text);

                // 店舗コード←画面上(カバーシート情報)の店舗コード
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.StoreCd, this.BatchStoreLabel.Text);

                // 伝票種別コード←画面上(カバーシート情報)の伝票種別コード
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeCd, this.SlipTypeCoverDropDownList.SelectedValue);

                // 伝票区分←画面上(カバーシート情報)の伝票区分
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SlipDiv, this.SlipDivisionCoverDropDownList.SelectedValue);

                // バッチ日付←画面上(カバーシート情報)のバッチ日付
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchDate, this.BatchDateDateTextBox.DateText);
            }

            if (workEntity.InitMode == "2")
            {
                // ＳＡ伝票訂正法人コード←WorkEntity.法人コード
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorporationCode, workEntity.CorporationCode);

                // ＳＡ伝票訂正伝票区分←画面上(伝票ヘッダ情報)の伝票区分
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDiv, this.SARPurchaseSlipHeader_SlipDivisionCd.Text);

                // ＳＡ伝票訂正伝票種別コード←画面上(伝票ヘッダ情報)の伝票種別
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionTypeCD, this.SARPurchaseSlipHeader_SlipTypeCd.Text);

                // ＳＡ伝票訂正伝票番号←画面上(伝票ヘッダ情報)の伝票番号
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionNo, this.TransferSlipNumberTextBox.Text);

                // ＳＡ伝票訂正バッチ日付←画面上(カバーシート情報)のバッチ日付
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionBatchDate, this.BatchDateDateTextBox.DateText);

                // ＳＡ伝票訂正店舗コード←画面上(伝票ヘッダ情報)のバッチ店舗
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionStoreCD, this.BatchStoreLabel.Text);

                // ＳＡ伝票訂正入力媒体区分←１：画面
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionInputMediaDiv, "1");

                // ＳＡ伝票訂正入力バッチ番号←画面上(伝票ヘッダ情報)のバッチ番号
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionInputBatchNo, this.SARPurchaseSlipHeader_BatchNumberLabel.Text);
            }

            // --> 2014/06/03 伝票起票より戻った際に一覧表示する対応
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.ListSearchFlg, "1");
            // <-- 2014/06/03 伝票起票より戻った際に一覧表示する対応
            //this.Redirect("SCRCSAR050305", null);
            this.Redirect("SCRCSAR050215", null);
        }
        #endregion


        // 2014.04.17 Start カバーシート検索画面へのリンク対応
        #region CoverSheetSearchLinkButton_Click (カバーシート検索画面リンク)
        /// <summary>
        /// カバーシート検索画面リンク
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベントデータ</param>
        protected void CoverSheetSearchLinkButton_Click(object sender, EventArgs e)
        {
            // --> 2014/06/03 伝票起票より戻った際に一覧表示する対応
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.ListSearchFlg, "1");
            // <-- 2014/06/03 伝票起票より戻った際に一覧表示する対応
            this.Redirect("SCRCSAR050205", null);
        }
        #endregion
        // 2014.04.17 End   カバーシート検索画面へのリンク対応

        
        #region SelectCheckedChanged
        /// <summary>
        /// SelectCheckedChanged
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベントデータ</param>
        protected void SelectCheckedChanged(object sender, EventArgs e)
        {
            CoreCenterCheckBox checkBox = (CoreCenterCheckBox)sender;
            this.SKUSerchButton.Enabled = checkBox.Checked;

            if (checkBox.Checked)
            {
                GridRecord gridRecord =
                  ((Infragistics.Web.UI.GridControls.GridRecordItem)((Infragistics.Web.UI.TemplateContainer)(((CoreCenterCheckBox)
                  sender).NamingContainer)).Item).Row;

                CoreCenterLabel skuCdTextBox = (CoreCenterLabel)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SKUCd_Control");
                this.SetSession("SKUCd", skuCdTextBox.Text);
                this.SetSession("RowIndex", gridRecord.Index);
                // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
                // 現在のdsEntity内容を退避する
                DsEntity DsTmpEntity = this.DsEntitySession;
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, DsTmpEntity);
                // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
            }
        }
        #endregion

        #region SKUSerchButton_Click (商品検索ボタン)
        /// <summary>
        /// 商品検索ボタン
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void SKUSerchButton_Click(object sender, EventArgs e)
        {
            // １．セッション情報の保存
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CallingDispID, this.ScreenId);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.ProductsSearchCorporationCode, UserProfile.CorporationCode);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.BasicDate, this.SalesChangeDate.Text);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.ProductsSearchSingleDoubleDiv, "1");
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.SKUCd, this.GetSession("SKUCd"));
            this.SetSession("IsPostBack", true);

            this.SKUSerchButton.Enabled = false;

            // ２．商品検索画面にリダイレクト
            this.Redirect("SCRCSTR070302", null);
        }
        #endregion

        #region PageDatePager_NavigatePage
        /// <summary>
        /// ページリンク ボタンクリックイベント。
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void PageDatePager_NavigatePage(object sender, NavigatePageEventArgs e)
        {
            this.TargetDataTable = this.GetSARSlipHeaderErrorDataset(e.TargetPageNumber).SARSlipHeaderError;
            this.DataBindToinputStatusListGrid();

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.PageNumber, e.TargetPageNumber);
            this.SetLineNoForGridView(this.inputStatusListGrid, "SARCoverSheetMaster_Number_Control", e.TargetPageNumber);
        }
        #endregion

        #region SlipTypeCoverDropDownList_SelectedIndexChanged
        /// <summary>
        /// Handles the SelectedIndexChanged event of the SlipTypeCoverDropDownList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SlipTypeCoverDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string value = this.SlipTypeCoverDropDownList.SelectedValue;

            Core.Diagnostics.LogUtil.WriteLog(
                "TraceLog",
                Core.Diagnostics.LogEntryCreator.CreateLogEntry<Core.Diagnostics.Entries.CoreCenterLogEntry>(
                    System.Diagnostics.TraceEventType.Information,
                    "TraceLog",
                    "【伝票種別プルーダウンリストイベント】" + this.SlipTypeCoverDropDownList.SelectedValue));

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue, this.SlipTypeCoverDropDownList.SelectedValue);
            this.SetHiddenGridViewBySlipType(value);

            //20151021 start 明細行編集へ変更　detele
            // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //this.ChangeInputControl();
            // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            //20151021 end 明細行編集へ変更　detele
    
            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);

            // 伝票区分ドロップダウンリスト
            if (value.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            {
                this.SlipDivisionCoverDropDownList.DataSource = initialDisplayEntity.SalesChangeSlipDivListDictionary;
                this.SlipDivisionCoverDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                this.SlipDivisionCoverDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                this.SlipDivisionCoverDropDownList.DataBind();
                this.SlipDivisionCoverDropDownList.Enabled = true;
                //デバッグ用
                //object[] parameter = { "テスト：554行目：initialDisplayEntity.SalesChangeSlipDivListDictionary.count：" + initialDisplayEntity.SalesChangeSlipDivListDictionary.Count() + "/" + initialDisplayEntity.SalesChangeSlipDivListDictionary.Select(c => c.CodeDivisionValueName) };
                //throw RetailBusinessException.CreateInstance(ErrorKey.ERRCCMRB1056, parameter);

                // 8/9 SUEYOSHI 売変区分をAP変数から取得するように修正 START
                // 2012.06.21 Start 売変用の事由を追加（値下げ、値上げの２パターン分）
                // 伝票種別ドロップダウンリスト
                //if (this.SlipDivisionCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChangePriceDown)
                //{

                //20151021 start 明細行編集へ変更　detele
                //if (this.SlipDivisionCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice)
                //// 8/9 SUEYOSHI END
                //{
                //    // 値下げ
                //    // 事由ドロップダウンリスト
                //    this.DetailAddReason.DataSource = initialDisplayEntity.ReasonSalesChangePriceDownListDictionary;
                //    this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                //    this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                //    this.DetailAddReason.DataBind();
                //}
                //// 8/9 SUEYOSHI 売変区分をAP変数から取得するように修正 START
                //// else if (this.SlipDivisionCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChangePriceUp)
                //else
                //// 8/9 SUEYOSHI END
                //{
                //    // 値上げ
                //    // 事由ドロップダウンリスト
                //    this.DetailAddReason.DataSource = initialDisplayEntity.ReasonSalesChangePriceUpListDictionary;
                //    this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                //    this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                //    this.DetailAddReason.DataBind();
                //}
                // 2012.06.21 End   売変用の事由を追加（値下げ、値上げの２パターン分）
                //20151021 end 明細行編集へ変更　detele

            }
            else if (value.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
            {
                this.SlipDivisionCoverDropDownList.DataSource = initialDisplayEntity.DisposalSlipDivListDictionary;
                this.SlipDivisionCoverDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                this.SlipDivisionCoverDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                this.SlipDivisionCoverDropDownList.DataBind();

                //20151021 start 明細行編集へ変更　detele
                // 2012.06.21 Start 売変用の事由を追加（値下げ、値上げの２パターン分）
                // 事由ドロップダウンリスト（廃棄用）
                //this.DetailAddReason.DataSource = initialDisplayEntity.ReasonListDictionary;
                //this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                //this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                //this.DetailAddReason.DataBind();
                // 2012.06.21 End   売変用の事由を追加（値下げ、値上げの２パターン分）
                //20151021 end 明細行編集へ変更　detele

            }

            this.SetMainHeader();
        }
        #endregion

        #region CancelAllButton_Click (選択解除ボタン押下)
        /// <summary>
        /// 選択解除ボタン押下
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void CancelAllButton_Click(object sender, EventArgs e)
        {
            foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
            {
                CoreCenterCheckBox selectCheckBox = (CoreCenterCheckBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Select_Control");

                // Unchecked
                selectCheckBox.Checked = false;
            }

            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            //20151015 start 明細行編集できるようになるため不要
            // 2014.05.09 Start 効率改善指摘事項の修正
            //this.DetailAddSKUCd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正
            //20151015 end 明細行編集できるようになるため不要
        }
        #endregion

        #region SelectAllButton_Click (全選択ボタン押下)
        /// <summary>
        /// 全選択ボタン押下
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void SelectAllButton_Click(object sender, EventArgs e)
        {

            foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
            {
                CoreCenterCheckBox selectCheckBox = (CoreCenterCheckBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Select_Control");

                // Checked
                selectCheckBox.Checked = true;
            }

            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            //20151015 start 明細行編集できるようになるため不要
            // 2014.05.09 Start 効率改善指摘事項の修正
            //this.DetailAddSKUCd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正
            //20151015 end 明細行編集できるようになるため不要

        }
        #endregion

        #region Category2Cd_TextChanged (分類２コード入力)
        /// <summary>
        /// 分類２コード入力
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void Category2Cd_TextChanged(object sender, EventArgs e)
        {
            // 分類１コードの入力が確認されない場合はエラーメッセージ[ERRCSARB0185]を表示する。
            if (string.IsNullOrEmpty(this.Category1Cd.Text))
            {
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0185)));
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.Category1Cd.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
                return;
            }

            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);

            WorkEntity workEntity = new WorkEntity();
            workEntity.CorporationCode = UserProfile.CorporationCode;
            workEntity.Category1Cd = this.Category1Cd.Text;
            workEntity.Category2Cd = this.Category2Cd.Text;

            //2016.02.25 start マスタ検索条件を業務日付から売変日へ変更する
            //if (initialDisplayEntity.SystemDateDataset.CMCSystemDate.Rows.Count > 0)
            //{
            //    workEntity.ChangeDate = initialDisplayEntity.SystemDateDataset.CMCSystemDate.FirstOrDefault().SystemDate;
            //}
            workEntity.ChangeDate = this.SalesChangeDate.DateValue;
            //2016.02.25 end
            // ファサード【BSFCSAR050800034】を実施する。
            this.GetFacadeInstance().GetCategory2NameNew(workEntity);

            // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
            //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
            //// ダミーコードを取得
            //this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
            //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
            // 2014.05.28 End   部門代表コードの部分を機能しないように修正

            // ファサード実施後、WorkEntity.分類２名称＝""の場合、エラーメッセージ[ERRCSARB0186]スローする
            if (string.IsNullOrEmpty(workEntity.Category2Name))
            {
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0186)));
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.Category2Cd.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
                return;
            }

            // エラーがスローされなければ、WorkEntity.分類２名称にされた値を分類２名称へ表示する。
            this.Category2Name.Text = workEntity.Category2Name;
        }
        #endregion

        //#region DetailAddSKUCd_TextChanged (商品コード入力)
        ///// <summary>
        ///// 商品コード入力
        ///// </summary>
        ///// <param name="sender">イベント送信元のオブジェクト。</param>
        ///// <param name="e">イベント引数。</param>
        //protected void DetailAddSKUCd_TextChanged(object sender, EventArgs e)
        //{
        //    #region 部門代表コードの部分を機能しないように修正
        //    // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
        //    //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
        //    //// 商品コードが未入力の場合は部門代表としてダミーコードをセットする
        //    //if (string.IsNullOrEmpty(this.DetailAddSKUCd.Text))
        //    //{
        //    //    string category1DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode);
        //    //    string category2DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode);
        //    //    if (string.IsNullOrEmpty(category1DummyCd) && string.IsNullOrEmpty(category2DummyCd))
        //    //    {
        //    //        // ダミーコードが取得できていない場合はダミーコードを取得する
        //    //        this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
        //    //        // ダミーコードの入れ直し
        //    //        category1DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode);
        //    //        category2DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode);
        //    //    }

        //    //    // 分類２がセットされていない場合は分類１のダミーコード。以外は分類２のダミーコードをセット。
        //    //    if (string.IsNullOrEmpty(this.Category2Cd.Text))
        //    //    {
        //    //        this.DetailAddSKUCd.Text = category1DummyCd;
        //    //    }
        //    //    else
        //    //    {
        //    //        this.DetailAddSKUCd.Text = category2DummyCd;
        //    //    }
        //    //}
        //    //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
        //    // 2014.05.28 End   部門代表コードの部分を機能しないように修正
        //    #endregion
        //    // 暫定修正 2011.11.28 Start 入力チェックの強化
        //    // 商品コードの必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
        //    if (string.IsNullOrEmpty(this.DetailAddSKUCd.Text))
        //    {
        //        // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
        //        // 商品コード未入力は入力エリアをクリアしエラーは表示しない。
        //        //this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "SKUコード")));
        //        this.ClearInputArea();
        //        // 2014.05.28 End   部門代表コードの部分を機能しないように修正
        //        this.DetailAddSKUCd.Focus();
        //        return;
        //    }

        //    // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
        //    // バッチ日付を入力すると伝票日付もセットされるのでバッチ日付のチェックのみとする
        //    //// 日付の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
        //    //if (string.IsNullOrEmpty(this.SalesChangeDate.DateText))
        //    //{
        //    //    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "日付")));
        //    //    this.SalesChangeDate.Focus();
        //    //    return;
        //    //}
        //    // 2014.05.28 End   部門代表コードの部分を機能しないように修正

        //    // バッチ日付の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
        //    if (string.IsNullOrEmpty(this.BatchDateDateTextBox.DateText))
        //    {
        //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "バッチ日付")));
        //        this.BatchDateDateTextBox.Focus();
        //        return;
        //    }
        //    // 暫定修正 2011.11.28 End   入力チェックの強化


        //    // 2014.04.30 Start 店舗コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
        //    // 店舗名称が空白の場合は、マスタ未存在と判断しエラーとする。
        //    if (string.IsNullOrEmpty(this.StoreName.Text))
        //    {
        //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0179)));
        //        this.StoreTextBox.Focus();
        //        return;
        //    }
        //    // 2014.04.30 End   店舗コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）

        //    // 2014.04.30 Start 部門コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
        //    // 店舗名称が空白の場合は、マスタ未存在と判断しエラーとする。
        //    if (string.IsNullOrEmpty(this.Category1Name.Text))
        //    {
        //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0184)));
        //        this.Category1Cd.Focus();
        //        return;
        //    }
        //    // 2014.04.30 End   部門コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）


        //    // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //    string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;
        //    // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする

        //    ItemInfoDataset itemInforDataSet = new ItemInfoDataset();
        //    WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);
        //    DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);

        //    workEntity.IVGetDiv = "0";

        //    string apVariableValue = APVariableUtil.GetAPVariableValue(APVariableListKey.STR_PRODUCTSCDDIGITS, workEntity.CorporationCode);
        //    // SKN臨時対応　商品情報取得 AP変数より商品コード入力桁数をチェックしない -->
        //    //if (this.DetailAddSKUCd.Text.Length == int.Parse(apVariableValue))
        //    //{
        //    // SKN臨時対応　商品情報取得 AP変数より商品コード入力桁数をチェックしない <--
        //    workEntity.ItemCd = this.DetailAddSKUCd.Text;
        //    workEntity.ChangeDate = this.SalesChangeDate.DateValue;

        //    // SKN臨時対応　商品情報取得 -->
        //    workEntity.StoreCd = this.StoreTextBox.Text;
        //    workEntity.IVGetSystemDiv = "IV";
        //    workEntity.IVGetDiv = "1";
        //    workEntity.Subsystem = "SA";
        //    workEntity.Category1Cd = this.Category1Cd.Text;
        //    workEntity.BatchDate = Convert.ToDateTime(this.BatchDateDateTextBox.Text);
        //    // SKN臨時対応　商品情報取得 <--

        //    //   2012.01.05 関根 売単価のクリア 追加 開始
        //    workEntity.ProductInfo_SalePrice = string.Empty;
        //    //   2012.01.05 関根 売単価のクリア 追加 終了

        //    // 2013.08.27 Start 売価取得区分設定
        //    InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
        //    if (SalesChangeDisposalSlipInputConstances.Disposal.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
        //    {
        //        // 現在入力中の伝票種別が廃棄伝票の場合
        //        workEntity.SalePriceGetKbn = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_UNIT_PRICE_FETCH_METHOD_DISPOSAL);
        //    }
        //    else if (SalesChangeDisposalSlipInputConstances.SalesChange.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
        //    {
        //        // 現在入力中の伝票種別が売変伝票の場合
        //        workEntity.SalePriceGetKbn = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_UNIT_PRICE_FETCH_METHOD_SALE);
        //    }
        //    // 2013.08.27 End   売価取得区分設定

        //    // 商品情報取得のファサード実行(BSFCSAR050800010)
        //    this.GetFacadeInstance().FillItemInfo(workEntity, itemInforDataSet, dsEntity);

        //    // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //    //this.DetailAddTransferQuantity.Text = workEntity.ProductInfo_LogicalStockQty;
        //    //this.DetailAddSlipNewPriceSalesChange.Text = workEntity.ProductInfo_SalePrice;
        //    //// 2012.06.21 Start 売変選択時に画面の旧売価が０円で表示されているので修正
        //    //// 取得した売単価を保持する
        //    //this.SetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice, workEntity.ProductInfo_SalePrice);
        //    //// 2012.06.21 End   売変選択時に画面の旧売価が０円で表示されているので修正

        //    //// SKN臨時対応　商品情報取得 -->
        //    //DetailAddSKUCd_TextChanged_ProductInfo_LogicalStockQty = workEntity.ProductInfo_LogicalStockQty;
        //    //DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice = workEntity.ProductInfo_SalePrice;
        //    //this.DetailAddSlipNewPriceDisposal.Text = workEntity.ProductInfo_SalePrice;
        //    // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //    if (itemInforDataSet.Tables[0].Rows.Count > 0)
        //    {
        //        // 2014.05.09 Start 効率改善指摘事項の修正
        //        if (itemInforDataSet.ItemInfo[0].Category1Cd != this.Category1Cd.Text)
        //        {
        //            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0275)));
        //            this.DetailAddSKUCd.Focus();
        //            return;
        //        }
        //        // 2014.05.09 End   効率改善指摘事項の修正

        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.SkuName, itemInforDataSet.ItemInfo[0].ProductsKanji);
        //        // 暫定修正 2011.11.28 Start 商品が未存在の場合はExceptionを発生させる
        //        //}
        //    }
        //    else
        //    {
        //        // SKUコード以外はクリア
        //        this.DetailAddTransferQuantity.Text = string.Empty;
        //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //        this.DetailAddSlipOldPriceSalesChange.Text = string.Empty;
        //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //        this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
        //        // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //        this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
        //        // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //        this.DetailAddReason.SelectedIndex = 0;
        //        // DataSetを空にする
        //        if (itemInforDataSet != null && itemInforDataSet.Tables.Count != 0 && itemInforDataSet.Tables[0].Rows.Count != 0)
        //        {
        //            itemInforDataSet.Tables[0].Clear();
        //        }
        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset, itemInforDataSet);
        //        // Exceptionを発生させる
        //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
        //        // 2014.05.09 Start 効率改善指摘事項の修正
        //        this.DetailAddSKUCd.Focus();
        //        // 2014.05.09 End   効率改善指摘事項の修正
        //        return;
        //    }
        //    // 暫定修正 2011.11.28 End   商品が未存在の場合はExceptionを発生させる
        //    // SKN臨時対応　商品情報取得 <--

        //    // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //    // AP変数の値を元に入力エリアの売価項目に対して入力可、不可の制御を行う
        //    // （0：変更不可、1：変更可能、3：警告あり商品のみ可能）
        // if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "3")
        //    {
        //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //        {
        //            // 売変の金額入力変更
        //            CheckAndChangeSalesChangePriceInput();
        //        }
        //        else
        //        {
        //            // 廃棄の金額入力変更
        //            CheckAndChangeDisposalPriceInput();
        //        }
        //    }

        //    // マスタの値をセットする
        //    // 2014.06.14 Start 品質改善検証の指摘事項対応
        //    //this.DetailAddTransferQuantity.Text = workEntity.ProductInfo_LogicalStockQty;
        //    // 数量には空白をデフォルトとする。
        //    this.DetailAddTransferQuantity.Text = string.Empty;
        //    // 2014.06.14 End   品質改善検証の指摘事項対応
        //    if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //    {
        //        // 売変
        //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //        this.DetailAddSlipOldPriceSalesChange.Text = workEntity.ProductInfo_SalePrice;
        //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //        this.DetailAddSlipNewPriceSalesChange.Text = workEntity.ProductInfo_SalePrice;
        //    }
        //    else
        //    {
        //        // 廃棄
        //        this.DetailAddSlipNewPriceDisposal.Text = workEntity.ProductInfo_SalePrice;
        //        // ※廃棄の場合は後処理で金額などを求めている箇所で DetailAddSlipNewPriceSalesChange の項目を使っているので同様の値を入れておく
        //        this.DetailAddSlipNewPriceSalesChange.Text = workEntity.ProductInfo_SalePrice;
        //    }
        //    // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //    // 画面上に旧売価項目を追加するためSessionへ入れる必要がなくなったのでコメントアウト
        //    //// 取得した売単価を保持する
        //    //this.SetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice, workEntity.ProductInfo_SalePrice);
        //    // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

        //    // 2014.06.14 Start 品質改善検証の指摘事項対応
        //    //DetailAddSKUCd_TextChanged_ProductInfo_LogicalStockQty = workEntity.ProductInfo_LogicalStockQty;
        //    // 2014.06.14 End   品質改善検証の指摘事項対応

        //    DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice = workEntity.ProductInfo_SalePrice;
        //    // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //    // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //    DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_OldSalePrice = workEntity.ProductInfo_SalePrice;
        //    // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

        //    // 2014.06.02 Start 画面操作統一対応（カーソル位置）
        //    // 商品マスタに存在する場合は、次項目へカーソル移動
        //    this.DetailAddTransferQuantity.Focus();                        // 数量
        //    // 2014.06.02 End   画面操作統一対応（カーソル位置）

        //    this.SetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset, itemInforDataSet);
        //    // SKN臨時対応　商品情報取得 AP変数より商品コード入力桁数をチェックしない -->
        //    //}
        //    // SKN臨時対応　商品情報取得 AP変数より商品コード入力桁数をチェックしない <--

        //    this.SetSession("SKUCd", this.DetailAddSKUCd.Text);
        //    this.SKUSerchButton.Enabled = true;
        //    this.RemoveSession("RowIndex");
        //}
        //#endregion

        //20151015 start 明細行直接編集　コメントアウト

        //string DetailAddSKUCd_TextChanged_ProductInfo_LogicalStockQty = null;
        //string DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice = null;
        //// 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //string DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_OldSalePrice = null;
        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //20151015 end 明細行直接編集に　コメントアウト

        #region Page_PreRender
        protected void Page_PreRender(object sender, EventArgs e)
        {
            //20151015 start 明細行直接編集により不要　コメントアウト
            //if (string.IsNullOrEmpty(DetailAddSKUCd_TextChanged_ProductInfo_LogicalStockQty) == false)
            //{
            //    this.DetailAddTransferQuantity.Text = DetailAddSKUCd_TextChanged_ProductInfo_LogicalStockQty;
            //}

            //if (string.IsNullOrEmpty(DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice) == false)
            //{
            //    this.DetailAddSlipNewPriceSalesChange.Text = DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice;
            //}

            //// 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //if (string.IsNullOrEmpty(DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_OldSalePrice) == false)
            //{
            //    this.DetailAddSlipOldPriceSalesChange.Text = DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_OldSalePrice;
            //}
            // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

            //20151015 end 明細行直接編集により不要　コメントアウト

            // 2014.04.30 Start 行番号が消える現象対応
            // ※Validatorエラーとなった場合、
            //   行番号が既に消えておりどの時点で消えているのか分からない為、
            //   チェック前に再度行番号を振り直す。
            //this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            // 2014.04.30 End   行番号が消える現象対応
            // 2014.05.09 Start 効率改善指摘事項の修正
            // 明細数に応じて部門コードの変更可、不可を制御する
            this.CheckSlipDeatilGridCategory1();
            // 2014.05.09 End   効率改善指摘事項の修正

            // AP変数の値を元に入力エリアの売価項目に対して入力可、不可の制御を行う
            // （0：変更不可、1：変更可能、3：警告あり商品のみ可能）
            if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "0")
            {
                // 変更可能へ
                // 金額項目の画面制御
                DisplayChangePriceControl(false);
            }
            else if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "1")
            {
                // 変更不可能へ(警告ありの場合も初期は変更不可としておく)
                // 金額項目の画面制御
                DisplayChangePriceControl(true);
            }

            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(false);

            this.ConvertToCurencyFormat();

            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);

        }
        #endregion

        #region Category1Cd_TextChanged (分類１コード入力)
        /// <summary>
        /// 分類１コード入力
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void Category1Cd_TextChanged(object sender, EventArgs e)
        {
            // 2014.05.28 Start 部門を空でフォーカスを移動すると先頭の部門名称が表示されてしまう
            if (string.IsNullOrEmpty(this.Category1Cd.Text))
            {
                // 部門が空の場合は名称をクリアする
                this.Category1Name.Text = string.Empty;
                return;
            }
            // 2014.05.28 End   部門を空でフォーカスを移動すると先頭の部門名称が表示されてしまう

            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);

            WorkEntity workEntity = new WorkEntity();
            workEntity.CorporationCode = UserProfile.CorporationCode;
            workEntity.Category1Cd = this.Category1Cd.Text;

            //2016.2.25 start マスタ取得条件を業務日付から売変日へ変更する
            //if (initialDisplayEntity.SystemDateDataset.CMCSystemDate.Rows.Count > 0)
            //{
            //    workEntity.ChangeDate = initialDisplayEntity.SystemDateDataset.CMCSystemDate.FirstOrDefault().SystemDate;
            //}
            workEntity.ChangeDate = this.SalesChangeDate.DateValue;

            //2016.2.25 end マスタ取得条件を業務日付から売変日へ変更する

            // 分類１コードの入力が確認されたら、ファサード【BSFCSAR050800033】を実施する。
            this.GetFacadeInstance().GetCategory1NameNew(workEntity);

            // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
            //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
            //// ダミーコードを取得
            //this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
            //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
            // 2014.05.28 End   部門代表コードの部分を機能しないように修正

            if (string.IsNullOrEmpty(workEntity.Category1Name))
            {
                // 2014.04.30 Start 部門コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
                this.Category1Name.Text = string.Empty;
                // 2014.04.30 End   部門コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）

                // ファサード実施後、WorkEntity.分類１名称＝""の場合、エラーメッセージ[ERRCSARB0184]スローする
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0184)));
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.Category1Cd.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
                return;
            }

            // 2014.06.14 Start 品質改善検証の指摘事項対応
            //// 2014.06.02 Start 画面操作統一対応（カーソル位置）
            //// 部門マスタに存在する場合は、次項目へカーソル移動
            //this.DetailAddSKUCd.Focus();                        // 商品コード
            //// 2014.06.02 End   画面操作統一対応（カーソル位置）

            // 明細グリッドが行が存在する場合は、
            // 修正モードであると判断し、
            // 商品コードへのフォーカス移動は行なわない。
            // ※他画面から遷移した場合に、
            //   明細グリッドにフォーカス移動させたいが、
            //   この処理で必ず商品コードへフォーカスが移動されてしまう。
            string orgScreenId = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.OrgScreenId);
            
            //20151014 start 画面仕様変更　ri
            //if (string.IsNullOrEmpty(orgScreenId))
            //{
            //    // 新規登録のみ、部門指定後に商品コードへフォーカス移動
            //    this.DetailAddSKUCd.Focus();                        // 商品コード
            //}
            // 2014.06.14 End   品質改善検証の指摘事項対応
            //20151014 end 画面仕様変更　ri
            
            // エラーがスローされなければ、WorkEntity.分類１名称にされた値を分類１名称へ表示する。
            this.Category1Name.Text = workEntity.Category1Name;


            //20151007 start 明細行デフォルト表示追加　ri
            this.AddBlankRowToDataGrid();

            if (this.DetailAddButton.Enabled == false)
            {
                this.DetailAddButton.Enabled = true;
            }

            // AP変数の値を元に入力エリアの売価項目に対して入力可、不可の制御を行う
            // （0：変更不可、1：変更可能、3：警告あり商品のみ可能）
            if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "1")
            {
                // 変更可能へ
                // 金額項目の画面制御
                DisplayChangePriceControl(true);
            }
            else
            {
                // 変更不可能へ(警告ありの場合も初期は変更不可としておく)
                // 金額項目の画面制御
                DisplayChangePriceControl(false);
            }

            // ダミーコードを取得する
            this.GetAndSaveSkuDummyCode(this.Category1Cd.Text);

            //明細表示後部門編集無効へ
            this.SetHeaderEnableControls(false);
            //20151007 end 明細行デフォルト表示追加　ri
            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);

            // 2015.11.25 　start 明細行表示後一行目の商品コードにフォーカス
            foreach (GridRecord record in this.SlipDetailGridView.Rows)
            {
                CoreCenterTextBox SKUCdTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SKUCd_Control");

                SKUCdTextBox.Focus();
            }
            // 2015.11.25 　end 明細行表示後一行目の商品コードにフォーカス

        }
        #endregion

        #region NewButton_Click (新規ボタン)
        /// <summary>
        /// 新規ボタン
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void NewButton_Click(object sender, EventArgs e)
        {
            // カバーシート情報入力のバッチ店舗・伝票種別・伝票区分・バッチ番号が入力不可（保存済みデータ）でない場合にエラーメッセージ：
            // 「ERRCSARB0211」をスローして終了する。
            if (this.BatchStoreLabel.Enabled &&
                this.SlipTypeCoverDropDownList.Enabled &&
                this.SlipDivisionCoverDropDownList.Enabled &&
                this.BatchNumberTextBox.Enabled)
            {
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0211)));
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.BatchStoreLabel.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
                return;
            }
			
			//START 20220726 orion.ri カバーシート承認済みの場合、新規伝票追加できないようにする
            CoverSheetMasterEntity coverSheetMasterEntity = (CoverSheetMasterEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity);

            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);

            DsEntity dsEntityforCoverSheet = new DsEntity();
            dsEntityforCoverSheet.CoverSheetMasterDataset = new CoverSheetMasterDataset();

            this.GetFacadeInstance().FillSARCoverSheetMasterNew(coverSheetMasterEntity, dsEntityforCoverSheet);


            if (dsEntityforCoverSheet.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Any() &&
                dsEntityforCoverSheet.CoverSheetMasterDataset.CoverSheetMasterDataTable1.FirstOrDefault().ApprovalStatus == "3" &&
                workEntity.ApprovalMode == "1") {

                this.SetMessage(new MessageContainer("カバーシートは承認済みのため、新規伝票を追加できません"));

                return;

            }
			//END 20220726 orion.ri
			
            // 2012.06.26 Start 新規ボタンを押してもカバーシートの保存状態は残す。
            //this.SetEnableControls(true);
            // 2012.06.26 End   新規ボタンを押してもカバーシートの保存状態は残す。
            // 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
            this.SetHeaderEnableControls(true);
            // 2012.06.26 End   伝票ヘッダー情報の入力制御を修正

            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            //WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);

            // 伝票検索のファサードを実行する。
            // 【BSFCSAR050800032】
            this.GetFacadeInstance().FillSARSlipNew(workEntity, dsEntity);

            this.ClearHeader();
            this.ClearDetail();
            //20151015 start 明細行で直接編集できるように修正によりコメントアウト
            // 2012.06.26 Start 入力エリアのクリア箇所を変更
            //this.ClearInputArea();
            // 2012.06.26 End   入力エリアのクリア箇所を変更
            //20151015 end 明細行で直接編集できるように修正によりコメントアウト


            // 伝票番号＝WorkEntity.採番伝票番号　をセットする。
            this.TransferSlipNumberTextBox.Text = workEntity.GetSlipNo;

            if (this.DetailAddButton.Enabled == true)
            {
                this.DetailAddButton.Enabled = false;
            }

            // 売変日＝WorkEntity.販売管理営業日をセットする。
            // 2012.06.26 Start クリアされていない箇所がある
            //this.SalesChangeDate.Text = workEntity.ChangeDate.ToString();
            // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            //this.SalesChangeDate.DateValue = workEntity.ChangeDate;
            // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            // 2012.06.26 End   クリアされていない箇所がある

            // 2014.05.09 Start 効率改善指摘事項の修正
            this.Category1Cd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正

            this.Master.HeaderMessageLabelString = RetailConstant.DisplayMode.Add;
            this.Master.HeaderBackColor = Color.FromName(RetailConstant.ColorDisplayMode.Add);
        }
        #endregion

        #region InputStatusListGrid_ItemCommand (入力状況情報 伝票番号)
        /// <summary>
        /// 入力状況情報 伝票番号
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void InputStatusListGrid_ItemCommand(object sender, HandleCommandEventArgs e)
        {
            // Dｓ（データセット）の生成
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);
            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);

            CoreCenterLinkButton slipNoLinkButton = (CoreCenterLinkButton)sender;

            // WorkEntity.対象伝票番号＝クリックされた伝票番号　をセットする
            workEntity.TargetSlipNo = slipNoLinkButton.Text;
            // 伝票ヘッダー、伝票明細をクリア
            dsEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
            dsEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();

            // 伝票検索のファサードを実行する。
            // 【BSFCSAR050800032】
            this.GetFacadeInstance().FillSARSlipNew(workEntity, dsEntity);

            //2015.11.16 start 既存伝票の新規追加行が削除できるように追加

            DsEntity OrgDsEntity = new DsEntity();

            //OrgDsEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();

            OrgDsEntity.SARSlipDetailErrorDataSet = (SARSlipDetailErrorInfoDataSet)dsEntity.SARSlipDetailErrorDataSet.Copy();

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.OrgDsEntity, OrgDsEntity);
            //2015.11.16 end 既存伝票の新規追加行が削除できるように追加

            // 8/21 SUEYOSHI 店舗コード、部門コード、バッチ日付をエンティティに格納
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count != 0)
            {
                // 2012.08.27 Start 店舗名称、部門名称取得時の日付はバッチ日付ではなく伝票日付で取得する
                //workEntity.BatchDate = (DateTime)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchDate);
                workEntity.BatchDate = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].DisposalChangeDate;
                // 2012.08.27 End   店舗名称、部門名称取得時の日付はバッチ日付ではなく伝票日付で取得する
                workEntity.StoreCd = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].StoreCd;
                workEntity.Category1Cd = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].Category1Cd;

                // 【BSFCSAR050800025】を実行し判定結果（０：正常、９：異常）から入力可否を判断する。
                string storeCode = this.GetFacadeInstance().ChkStoreCodeNew(workEntity);

                if (storeCode.Equals("9"))
                {
                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0179)));
                    this.StoreTextBox.Focus();

                    return;
                }

                this.StoreName.Text = workEntity.StoreName;

                // 2012.08.27 Start 店舗名称、部門名称取得時の日付はバッチ日付ではなく伝票日付で取得する
                //if (initialDisplayEntity.SystemDateDataset.CMCSystemDate.Rows.Count > 0)
                //{
                //    workEntity.ChangeDate = initialDisplayEntity.SystemDateDataset.CMCSystemDate.FirstOrDefault().SystemDate;
                //}
                workEntity.ChangeDate = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].DisposalChangeDate;
                // 2012.08.27 End   店舗名称、部門名称取得時の日付はバッチ日付ではなく伝票日付で取得する

                // 分類１コードの入力が確認されたら、ファサード【BSFCSAR050800033】を実施する。
                this.GetFacadeInstance().GetCategory1NameNew(workEntity);

                if (string.IsNullOrEmpty(workEntity.Category1Name))
                {
                    // ファサード実施後、WorkEntity.分類１名称＝""の場合、エラーメッセージ[ERRCSARB0184]スローする
                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0184)));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.Category1Cd.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return;
                }

                // エラーがスローされなければ、WorkEntity.分類１名称にされた値を分類１名称へ表示する。
                this.Category1Name.Text = workEntity.Category1Name;
            }

            // 8/21 SUEYOSHI END

            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            //this.ShowDataPager(initialDisplayEntity, dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count, this.GetPageNumber());
            //this.TargetDataTable = this.GetSARSlipHeaderErrorDataset(this.GetPageNumber()).SARSlipHeaderError;
            //this.DataBindToinputStatusListGrid();
            //this.SetLineNoForGridView(this.inputStatusListGrid, "SARCoverSheetMaster_Number_Control", this.GetPageNumber());
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

            // DsEntity.伝票ヘッダ更新Dsから取出して表示
            this.ShowSARSlipHeaderErrorData(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows[0]);

            // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
            //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
            //// ダミーコードを取得
            //this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
            //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
            // 2014.05.28 End   部門代表コードの部分を機能しないように修正

            // 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
            // Enabled header controls
            //this.TransferSlipNumberTextBox.Enabled = false;
            //this.StoreTextBox.Enabled = false;
            //this.Category1Cd.Enabled = false;
            //this.Category2Cd.Enabled = false;
            //this.SalesChangeDate.Enabled = false;
            this.SetHeaderEnableControls(false);
            // 2012.06.26 End   伝票ヘッダー情報の入力制御を修正

            //20151021 start 明細行直接編集により　機能不要
            // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //this.ClearInputArea();
            // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            //20151021 end 明細行直接編集により　機能不要

            // 対象（クリックされた）となる伝票の明細データを表示する。
            this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
            this.DataBindToSlipDetailGridView();

            // 2014.04.30 Start クリアボタン押下時は、検索前情報DataSet(OriginalDetailDataSet)も初期化する。
            // 伝票番号クリック時は、修正前の状態を保持する。
            this.OriginalDetailDataSet = dsEntity.SARSlipDetailErrorDataSet;
            // 2014.04.30 End   クリアボタン押下時は、検索前情報DataSet(OriginalDetailDataSet)も初期化する。

            //20151021 start 明細行直接編集　　ADD
            //ドロップダウンリストbind
            this.BindToDetailReasonDownList(dsEntity);

            //保存後明細行6行表示維持
            this.AddBlankRowToDataGrid();
            //20151021 end 明細行直接編集　　ADD

            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            // 2012.06.26 Start 伝票明細情報の数値項目に小数点以下が表示されるので表示しない。
            this.ConvertToCurencyFormat();
            // 2012.06.26 End   伝票明細情報の数値項目に小数点以下が表示されるので表示しない。
            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(false);
            this.SetHiddenGridViewBySlipType((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue));

            // 2014.05.09 Start 効率改善指摘事項の修正
            this.DetailAddSKUCd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正

            // 新規 一旦承認されたカバーシートには新し伝票を追加しないようにする
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Any() &&
                dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApprovalStatus == "3" &&
                workEntity.ApprovalMode == "1")
            {
                this.NewButton.Enabled = false;
            }
            else
            {
                this.NewButton.Enabled = true;

            }

            this.Master.HeaderMessageLabelString = RetailConstant.DisplayMode.Modify;
            this.Master.HeaderBackColor = Color.FromName(RetailConstant.ColorDisplayMode.Modify);
        }
        #endregion

        #region SlipDeleateButton_Click (伝票取消)
        /// <summary>
        /// 伝票取消
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void SlipDeleateButton_Click(object sender, EventArgs e)
        {
            // Dｓ（データセット）の生成
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);
            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            DsEntity dsInputStatusEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity);
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。
            // 2013.08.27 Start いなげや対応で使用するので取得位置を冒頭に変更
            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
            // 2013.08.27 End   いなげや対応で使用するので取得位置を冒頭に変更

            // 2011.12.21 関根 伝票取消エラーの障害対応（新規登録時） 追加 開始
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count <= 0)
            {
                // メッセージERRCSARB0273を表示する。「伝票が保存されていない為、伝票取消を行えません。」
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0273)));
                // 2014.05.09 Start 効率改善指摘事項の修正
                //this.DetailAddSKUCd.Focus();
                if (this.Category1Cd.Enabled == true)
                {
                    this.Category1Cd.Focus();
                }
                // 2014.05.09 End   効率改善指摘事項の修正
                return;
            }
            // 2011.12.21 関根 伝票取消エラーの障害対応（新規登録時） 追加 終了


            // 伝票ヘッダ更新Dsは論理削除フラグ＝１をセットする
            foreach (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerRow in dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows)
            {
                headerRow.LogicalDelFlg = "0";
            }

            // 伝票明細更新Dsは論理削除フラグ＝１をセットする
            foreach (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows)
            {
                detailRow.LogicalDelFlg = "0";
            }

            // 2012.06.26 Start カバーシートの入力枚数を画面入力値へ変更
            CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetRow = dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0];
            // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            //coverSheetRow.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            coverSheetRow.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 1 : int.Parse(this.InputPageCountTextBox.Text);
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            // 伝票情報用のDataSetへマージする
            dsInputStatusEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);
            // 2012.06.26 End   カバーシートの入力枚数を画面入力値へ変更

            // 生成したデータセットをDsEntityへ格納
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

            // 2013.08.20 Start 承認モードの設定
            // WorkEntity.登録モード＝SAR_ApprovalFeatureDivをセット
            // workEntity.InsertMode = APVariableListKey.SAR_ApprovalFeatureDiv;
            if (SalesChangeDisposalSlipInputConstances.Disposal.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
            {
                // 現在入力中の伝票種別が廃棄伝票の場合
                workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDiv);

                // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo start
                // 実行モード←"100"：計上処理モード
                workEntity.ExecuteMode = "100";
                // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo end
            }
            else if (SalesChangeDisposalSlipInputConstances.SalesChange.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
            {
                // 現在入力中の伝票種別が売変伝票の場合
                workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDivForSalesChange);

                // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo start
                // 実行モード←"1"
                workEntity.ExecuteMode = "100";
                // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo end
            }
            // 2013.08.20 End 登録モードの設定

            // WorkEntity.取消モード＝１
            // 2012.06.26 Start 伝票取消対応(削除フラグをセットする)
            //workEntity.CancelMode = "1";
            workEntity.CancelMode = Constants.OnFlag;
            // 2012.06.26 End   伝票取消対応(削除フラグをセットする)

            // 2013.08.27 Start いなげや対応で使用するので取得位置を冒頭に変更
            //InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
            // 2013.08.27 End   いなげや対応で使用するので取得位置を冒頭に変更

            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
            // カバーシートマスタワークエンティティ
            CoverSheetMasterEntity coverSheetMasterEntity = (CoverSheetMasterEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity);
            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了

            // 2012.06.26 Start カバーシートの入力枚数を画面入力値へ変更
            // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            //coverSheetMasterEntity.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            coverSheetMasterEntity.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 1 : int.Parse(this.InputPageCountTextBox.Text);
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity, coverSheetMasterEntity);
            // 2012.06.26 End   カバーシートの入力枚数を画面入力値へ変更

            bool isDisPlayOff = this.CheckSar(initialDisplayEntity, APVariableListKey.SAR_CheckDisplay_Off);

            if (isDisPlayOff)
            {
                // AP変数：SAR_CheckDisplay_Off＝０
                // WorkEntity.ボタンID＝５：伝票取り消しをセット
                workEntity.ButtonId = "5";

                // 生成したDsEntityとWorkEntityをセッションへ登録し確認画面へ遷移する
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity, workEntity);

                this.Redirect("SCRCSAR050208", null);
            }
            else
            {
                // SAR_CheckDisplay_Off＝１
                // 【BSFCSAR050800031】
                // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
                //this.GetFacadeInstance().DelSARSlip(workEntity, dsEntity, initialDisplayEntity);
                this.GetFacadeInstance().DelSARSlipNew(workEntity, dsEntity, initialDisplayEntity, coverSheetMasterEntity);
                // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了

                // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
                // カバーシートに保存されている伝票全てを取得するので対象の伝票番号項目は空にする。
                workEntity.TargetSlipNo = string.Empty;
                // 伝票ヘッダー、伝票明細をクリア
                dsInputStatusEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
                dsInputStatusEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();

                // 伝票検索のファサードを実行する。
                // 【BSFCSAR050800032】
                this.GetFacadeInstance().FillSARSlipNew(workEntity, dsInputStatusEntity);

                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity, dsInputStatusEntity);

                // カバーシート情報表示部分
                this.TargetDataTable = this.GetSARSlipHeaderErrorDataset(this.GetPageNumber()).SARSlipHeaderError;
                this.DataBindToinputStatusListGrid();
                // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

                this.ClearHeader();
                this.ClearDetail();
                // 2012.06.26 Start 入力エリアのクリア箇所を変更
                this.ClearInputArea();
                // 2012.06.26 End   入力エリアのクリア箇所を変更

                // 2014.05.28 Start 伝票取消を行うとカバーシートの伝票一覧より番号(No)が消えてしまうので修正
                // 伝票番号も消えてしまうので新規番号をセットする
                // 伝票番号＝WorkEntity.採番伝票番号　をセットする。
                this.TransferSlipNumberTextBox.Text = workEntity.GetSlipNo;
                // 2014.05.28 End   伝票取消を行うとカバーシートの伝票一覧より番号(No)が消えてしまうので修正
            }

            // 2014.01.06 Start 伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。
            //// 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
            //this.SetHeaderEnableControls(true);
            //// 2012.06.26 End   伝票ヘッダー情報の入力制御を修正
            if (workEntity.InitMode == "1" || workEntity.InitMode == "2")
            {
                this.SetHeaderEnableControls(false);
            }
            else
            {
                this.SetHeaderEnableControls(true);
            }
            // 2014.01.06 End 伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。

            // 2014.05.28 Start 伝票取消を行うとカバーシートの伝票一覧より番号(No)が消えてしまうので修正
            this.SetLineNoForGridView(this.inputStatusListGrid, "SARCoverSheetMaster_Number_Control", this.GetPageNumber());
            // 2014.05.28 End   伝票取消を行うとカバーシートの伝票一覧より番号(No)が消えてしまうので修正

            // 2012.06.26 Start 伝票取消対応(削除フラグをセットする)
            workEntity.CancelMode = Constants.OffFlag;
            workEntity.TranUpdateMode = "0";
            // 2012.06.26 End   伝票取消対応(削除フラグをセットする)
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity, workEntity);
            // メッセージMSGCSARI0028を表示する。{0}は「伝票取消」
            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCSARI0028, "伝票取消")));
            // 2014.05.09 Start 効率改善指摘事項の修正
            //this.DetailAddSKUCd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正
            if (this.DetailAddButton.Enabled == true)
            {
                this.DetailAddButton.Enabled = false;
            }

            this.Master.HeaderMessageLabelString = RetailConstant.DisplayMode.Add;
            this.Master.HeaderBackColor = Color.FromName(RetailConstant.ColorDisplayMode.Add);

            //START 20220726 orion.ri カバーシート承認済みの場合、新規伝票追加できないようにする
            CoverSheetMasterEntity coverSheetMasterEntitySlipDeleate = (CoverSheetMasterEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity);

            DsEntity dsEntityforCoverSheetSlipDeleate = new DsEntity();
            dsEntityforCoverSheetSlipDeleate.CoverSheetMasterDataset = new CoverSheetMasterDataset();

            this.GetFacadeInstance().FillSARCoverSheetMasterNew(coverSheetMasterEntitySlipDeleate, dsEntityforCoverSheetSlipDeleate);

            if (dsEntityforCoverSheetSlipDeleate.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Any() &&
                dsEntityforCoverSheetSlipDeleate.CoverSheetMasterDataset.CoverSheetMasterDataTable1.FirstOrDefault().ApprovalStatus == "3" &&
                workEntity.ApprovalMode == "1")
            {
                this.ClearSession();
                this.Redirect("SCRCSAR050202", null);
            }
            //END 20220726 orion.ri

        }
        #endregion

        #region ClearButton_Click (クリアボタン)
        /// <summary>
        /// クリアボタン
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            int initMode = (int)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitMode);

            //デバッグ用
            //object[] parameter = { "テスト：1499行目：initMode：" + initMode };
            //throw RetailBusinessException.CreateInstance(ErrorKey.ERRCCMRB1056, parameter);


            this.ClearItems(initMode);

            //// 2014.05.09 Start 効率改善指摘事項の修正
            //this.DetailAddSKUCd.Focus();
            //// 2014.05.09 End   効率改善指摘事項の修正

        }
        #endregion

        #region ReturnButton_Click (戻るボタン)
        /// <summary>
        /// 戻るボタン
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void ReturnButton_Click(object sender, EventArgs e)
        {
            // １．下記項目の参照を破棄する。
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);

            // ３．遷移元画面情報から判定し、以下の画面にリダイレクトする。
            int initmode = (int)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitMode);
            if (initmode == 0)
            {
                this.Redirect("SCRCPOF010101", null);
            }
            else if (initmode == 1)
            {
                this.Redirect("SCRCSAR050206", null);
            }
            else if (initmode == 2)
            {
                //this.Redirect("SCRCSAR050306", null);
                this.Redirect("SCRCSAR050225", null);
            }
        }
        #endregion

        #region NextButton_Click (次へ（保存）ボタン)
        /// <summary>
        /// 次へ（保存）ボタン
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void NextButton_Click(object sender, EventArgs e)
        {
            //20151014 start 明細行編集できるように仕様変更　ri
          
            this.SaveDropDownList();

            this.GetAndSaveSkuDummyCode();

            //2015.12.03 start ダミーコード取得失敗時処理を停止する ri
            bool codeStuts = (bool)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CodeStuts);
            if (!codeStuts)
            {
                return;
            }
            //2015.12.03 end ダミーコード取得失敗時処理を停止する ri

            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(true);

            this.ConvertToCurencyFormat();
            //20151014 end 明細行編集できるように仕様変更コメントアウト　ri

            //20151014 start 明細行編集できるように仕様変更コメントアウト　ri

            // --> 2014/06/24 保存時に新規分も登録されてしまう不具合を修正
            //if (!string.IsNullOrEmpty(this.DetailAddSKUCd.Text))
            //{
            //    if (this.DetailAddSKUCd.Enabled == false)
            //    {
            //        // 修正の時のみ

            //        // 商品追加（編集反映）処理
            //        if (this.DetailAdd() == false)
            //        {
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        // 商品追加が残っている時

            //        // 入力エリアをクリア
            //        this.ClearInputArea();
            //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.SkuName, string.Empty);
            //    }
            //}
            // <-- 2014/06/24 保存時に新規分も登録されてしまう不具合を修正
            //20151014 end 明細行編集できるように仕様変更コメントアウト　ri

            // 2014.03.18 Start 効率改善対応（Validatorコメント）
            // 2014.04.30 Start 行番号が消える現象対応
            // ※Validatorエラーとなった場合、
            //   行番号が既に消えておりどの時点で消えているのか分からない為、
            //   チェック前に再度行番号を振り直す。
            //this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            // 2014.04.30 End   行番号が消える現象対応

            // カバーシート情報チェック
            if (CheckCoverSheetInput().Equals(true))
            {
                return;
            }

            // 伝票ヘッダー情報チェック
            if (CheckSlipHeaderInfoInput().Equals(true))
            {
                return;
            }

            // 2014.05.27 Start Gridの数量チェックを強化(保存ボタンは入力途中の商品も保存対象とするのでチェックを追加)
            // 伝票明細情報チェック
            //if (CheckSlipDeatilInfoInput().Equals(true))
            //{
            //    return;
            //}
            // 2014.05.27 End   Gridの数量チェックを強化

            // 伝票明細グリッドチェック
            if (CheckSlipDeatilGridInput().Equals(true))
            {
                return;
            }
            // 2014.03.18 End   効率改善対応（Validatorコメント）

            //20151014 start 明細行編集できるように仕様変更コメントアウト　ri
            // 2013.08.21 Start 数量のグリッド保管
            //this.SaveDisplayControl();
            // 2013.08.21 End 数量のグリッド保管
            //20151014 end 明細行編集できるように仕様変更コメントアウト　ri

            // 8/16 SUEYOSHI 保存ボタン押下の際に、追加ボタン押下処理と同様の処理を追加
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);

            #region DEL
            // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //if (!string.IsNullOrEmpty(this.DetailAddSKUCd.Text) &&
            //    !string.IsNullOrEmpty(this.DetailAddTransferQuantity.Text) &&
            //    !string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) &&
            //    !string.IsNullOrEmpty(this.DetailAddReason.SelectedItem.Text))
            //{
            //    string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;

            //string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;
            // --> 2014/06/24 保存時に新規分も登録されてしまう不具合を修正
            //bool flgUpdate = false;

            //// 売変、廃棄別に項目の比較を行い、全て入力されている場合は追加と同様の処理後更新する
            //if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            //{
            //    // 売変
            //    if (!string.IsNullOrEmpty(this.DetailAddSKUCd.Text) &&
            //        !string.IsNullOrEmpty(this.DetailAddTransferQuantity.Text) &&
            //        !string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text) &&
            //        !string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) &&
            //        !string.IsNullOrEmpty(this.DetailAddReason.SelectedItem.Text))
            //    {
            //        flgUpdate = true;
            //    }
            //}
            //else
            //{
            //    // 廃棄
            //    if (!string.IsNullOrEmpty(this.DetailAddSKUCd.Text) &&
            //        !string.IsNullOrEmpty(this.DetailAddTransferQuantity.Text) &&
            //        !string.IsNullOrEmpty(this.DetailAddSlipNewPriceDisposal.Text) &&
            //        !string.IsNullOrEmpty(this.DetailAddReason.SelectedItem.Text))
            //    {
            //        flgUpdate = true;
            //    }
            //}

            // <-- 2014/06/24 保存時に新規分も登録されてしまう不具合を修正

            // --> 2014/06/24 保存時に新規分も登録されてしまう不具合を修正
            
            //if (flgUpdate == true)
            //{
            //    // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

            //    int inputSequenceNo = 0;

            //    // dsTmpEntityを使用すると、連続で訂正した時に最新のInputSequenceNoが取得出来ないため、dsEntityより取得する。
            //    if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
            //    {
            //        inputSequenceNo = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
            //    }

            //    DateTime systemDate = (DateTime)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SystemDate);

            //    string updateMode = "0";
            //    if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode) != null)
            //    {
            //        updateMode = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode);
            //    }

            //    ItemInfoDataset itemInfoDataset = (ItemInfoDataset)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset);

            //    // 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
            //    //if (itemInfoDataset == null && updateMode.Equals("0"))
            //    //{
            //    //    // ※上記のif文を含めると「インスタンスエラー」となる為、分割してチェックする
            //    //    if (itemInfoDataset.Tables.Count == 0 || itemInfoDataset.Tables[0].Rows.Count == 0)
            //    //    {
            //    //        // SKUコード以外はクリア
            //    //        this.DetailAddTransferQuantity.Text = string.Empty;
            //    //        this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
            //    //        // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
            //    //        this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
            //    //        // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
            //    //        this.DetailAddReason.SelectedIndex = 0;
            //    //        // Exceptionを発生させる
            //    //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
            //    //        return;
            //    //    }
            //    //}

            //    // システムエラーが発生するので下記に変更
            //    if (updateMode.Equals("0") && (itemInfoDataset == null || itemInfoDataset.Tables.Count == 0 || itemInfoDataset.Tables[0].Rows.Count == 0))
            //    {
            //        // SKUコード以外はクリア
            //        this.DetailAddTransferQuantity.Text = string.Empty;
            //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //        this.DetailAddSlipOldPriceSalesChange.Text = string.Empty;
            //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            //        this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
            //        // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
            //        this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
            //        // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
            //        this.DetailAddReason.SelectedIndex = 0;
            //        // Exceptionを発生させる
            //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
            //        // 2014.05.09 Start 効率改善指摘事項の修正
            //        this.DetailAddSKUCd.Focus();
            //        // 2014.05.09 End   効率改善指摘事項の修正
            //        return;
            //    }
            //    // 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする

            //    // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //    // 廃棄の場合の処理
            //    if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
            //    {
            //        // ※廃棄の場合は後処理で金額などを求めている箇所で DetailAddSlipNewPriceSalesChange の項目を使っているので同様の値を入れておく
            //        this.DetailAddSlipNewPriceSalesChange.Text = this.DetailAddSlipNewPriceDisposal.Text;
            //    }
            //    // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

            //    string skuName = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SkuName);

            //    if (updateMode.Equals("0"))
            //    {
            //        SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow row = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.NewSARSlipDetailErrorRow();

            //        // 法人コード
            //        row.CorporationCode = UserProfile.CorporationCode;

            //        // グリッド上のRowNumber
            //        row.SlipLineNo = (this.SlipDetailGridView.Rows.Count + 1).ToString();

            //        // 入力媒体区分
            //        row.InputMediaDiv = "1";

            //        // 入力バッチ番号
            //        row.InputBatchNo = this.BatchNumberTextBox.Text;

            //        // 「入力バッチ番号・伝票番号」毎にインクリメントする。
            //        row.InputSequenceNo = inputSequenceNo.ToString();
            //        //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了

            //        // 入力ＳＥＱ行番号
            //        row.InputSequenceLineNo = this.SlipDetailGridView.Rows.Count + 1;

            //        // 伝票番号
            //        row.SlipNo = this.TransferSlipNumberTextBox.Text;

            //        // 取込処理日
            //        row.InputDate = systemDate;

            //        // 取込処理時刻
            //        row.InputTime = DateTime.Now.ToString("HHmmss");

            //        // データ種別
            //        row.DataType = string.Empty;

            //        // 取込ＳＥＱ番号
            //        row.UptakeSequenceNo = decimal.Parse((inputSequenceNo).ToString());

            //        // 取込ＳＥＱ行番号
            //        row.UptakeSequenceLineNo = this.SlipDetailGridView.Rows.Count + 1;

            //        // バッチ店舗（カバー情報）
            //        row.StoreCd = this.StoreTextBox.Text;

            //        // 伝票種別（カバー情報）
            //        row.SlipTypeCd = slipTypeValue;

            //        // 伝票区分（カバー情報）
            //        row.SlipDiv = this.SlipDivisionCoverDropDownList.SelectedValue;

            //        // グリッド上のSKUコード
            //        row.SKUCd = this.DetailAddSKUCd.Text;

            //        // 2013/09/23 k.haraikawa 予備０１は商品名称なので、税込み金額は予備０２から設定する。
            //        // 2013.08.22 Start Attribute02 = グリッド上の新売単価（税込み）
            //        row.Attribute01 = skuName;
            //        // 2013.10.21 AST Chg Start
            //        #region 修正前
            //        //row.Attribute02 = string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? "0" : this.DetailAddSlipNewPriceSalesChange.Text;
            //        //// 2013.08.22 End Attribute02 = グリッド上の新売単価（税込み）
            //        //
            //        //// 2013.08.22 Start Attribute03 = グリッド上の旧売単価（税込み）
            //        //row.Attribute03 = string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text) ? "0" : this.DetailAddSlipOldPriceSalesChange.Text;
            //        //// 2013.08.22 End   Attribute03 = グリッド上の旧売単価（税込み）
            //        #endregion
            //        // グリッド上の旧売単価（税込み）
            //        row.Attribute02 = string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text) ? "0" : this.DetailAddSlipOldPriceSalesChange.Text;

            //        // グリッド上の新売単価（税込み）
            //        row.Attribute03 = string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? "0" : this.DetailAddSlipNewPriceSalesChange.Text;
            //        // 2013.10.21 AST Chg End

            //        // グリッド上の数量
            //        row.DisposalChangeQty = decimal.Parse(this.DetailAddTransferQuantity.Text);

            //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            //        {
            //            // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //            //// 売変の場合はセッションの商品マスタより取得した売価情報を取得し旧売価とする
            //            //row.SlipOldPrice =
            //            //    string.IsNullOrEmpty((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice)) ? 0 : decimal.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice));
            //            row.SlipOldPrice =
            //                string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text) ? 0 : decimal.Parse(this.DetailAddSlipOldPriceSalesChange.Text);
            //            // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            //        }
            //        else
            //        {   // 廃棄は０をセット
            //            row.SlipOldPrice = 0;
            //        }

            //        // グリッド上の新売単価　伝票種別＝廃棄の場合は０
            //        row.SlipNewPrice =
            //            string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? 0 : decimal.Parse(this.DetailAddSlipNewPriceSalesChange.Text);

            //        // グリッド上の売単価　伝票種別＝売変の場合は０
            //        row.SlipDisposalUnitPrica =
            //            string.IsNullOrEmpty(this.DetailAddSlipNewPriceDisposal.Text) ? 0 : decimal.Parse(this.DetailAddSlipNewPriceDisposal.Text);

            //        // 2013/09/23 k.haraikawa 予備０１は商品名称なので、税込み金額は予備０２から設定する。
            //        // 2013.08.22 Start Attribute04 = グリッド上の廃棄売単価（税込み）
            //        row.Attribute04 =
            //            string.IsNullOrEmpty(this.DetailAddSlipNewPriceDisposal.Text) ? "0" : this.DetailAddSlipNewPriceDisposal.Text;
            //        // 2013.08.22 End   Attribute04 = グリッド上の廃棄売単価（税込み）

            //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            //        {
            //            row.SlipDisposalUnitPrica = 0;
            //        }

            //        // グリッド上の事由
            //        row.Reason = this.DetailAddReason.Items[this.DetailAddReason.SelectedIndex].Text;

            //        // グリッド上の（数量×新売単価）
            //        row.SalesChangePrice = row.DisposalChangeQty * row.SlipNewPrice;

            //        // グリッド上の（数量×売単価）
            //        row.DisposalPriceAmt = row.DisposalChangeQty * row.SlipDisposalUnitPrica;

            //        // 商品情報Ds.Category1Cd
            //        row.Category1Cd = this.Category1Cd.Text;

            //        // 商品情報Ds.Category2Cd
            //        row.Category2Cd = this.Category2Cd.Text;

            //        // 商品情報Ds.Category3Cd
            //        row.Category3Cd = string.Empty;

            //        // 商品情報Ds.Category4Cd
            //        row.Category4Cd = string.Empty;

            //        // 商品情報Ds.ScanningCd
            //        row.ScanningCd = string.Empty;

            //        if (itemInfoDataset != null && itemInfoDataset.ItemInfo.Rows.Count > 0)
            //        {
            //            var item = (ItemInfoDataset.ItemInfoRow)itemInfoDataset.ItemInfo.Rows[0];

            //            //   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応 追加 開始
            //            // 商品情報Ds.Category1Cd
            //            row.Category1Cd = item.Category1Cd;

            //            // 商品情報Ds.Category2Cd
            //            row.Category2Cd = item.Category2Cd;
            //            //   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応 追加 終了

            //            // 商品情報Ds.Category3Cd
            //            row.Category3Cd = item.Category3Cd;

            //            // 商品情報Ds.Category4Cd
            //            row.Category4Cd = item.Category4Cd;

            //            // 商品情報Ds.ScanningCd
            //            row.ScanningCd = item.ScanningCd;
            //        }

            //        // 伝票区分（カバー情報）
            //        row.SalesChangeDiv = this.DetailAddReason.SelectedValue;

            //        // エラー区分
            //        row.ErrorDiv = string.Empty;

            //        // 論理削除フラグ
            //        row.LogicalDelFlg = "0";

            //        // 売変・廃棄日
            //        row.DisposalChangeDate = Convert.ToDateTime(this.SalesChangeDate.Text);

            //        dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AddSARSlipDetailErrorRow(row);

            //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            //    }
            //    else
            //    {
            //        // 2013.08.21 Start tmpからグリッドの情報を取得するので、tmpに画面項目のセッションに反映する
            //        this.SaveDisplayControlTmp(false);
            //        // 2013.08.21 End tmpからグリッドの情報を取得するので、tmpに画面項目のセッションに反映する

            //        DsEntity dsTmpEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity);

            //        int rowIndex = int.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.RowIndex));
            //        var detailRow =
            //            (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow)dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[rowIndex];

            //        detailRow.SKUCd = this.DetailAddSKUCd.Text;
            //        detailRow.Attribute01 = skuName;
            //        detailRow.DisposalChangeQty = decimal.Parse(this.DetailAddTransferQuantity.Text);
            //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            //        {
            //            detailRow.SlipOldPrice = decimal.Parse(this.DetailAddSlipOldPriceSalesChange.Text);
            //        }
            //        else
            //        {   // 廃棄は０をセット
            //            detailRow.SlipOldPrice = 0;
            //        }
            //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            //        detailRow.SlipNewPrice = decimal.Parse(this.DetailAddSlipNewPriceSalesChange.Text);

            //        // 2013/09/23 k.haraikawa 予備０１は商品名称なので、税込み金額は予備０２から設定する。
            //        // 2013.08.22 Start Attribute02 = グリッド上の新売単価（税込み）
            //        detailRow.Attribute01 = skuName;
            //        // --> 2013/11/01 マスタ売価対応
            //        //// 2013.10.21 AST Chg Start
            //        //#region 修正前
            //        ////detailRow.Attribute02 = string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? "0" : this.DetailAddSlipNewPriceSalesChange.Text;
            //        ////// 2013.08.22 End Attribute02 = グリッド上の新売単価（税込み）
            //        ////
            //        ////// 2013.08.22 Start Attribute03 = グリッド上の旧売単価（税込み）
            //        ////detailRow.Attribute03 = string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text) ? "0" : this.DetailAddSlipOldPriceSalesChange.Text;
            //        ////// 2013.08.22 End   Attribute03 = グリッド上の旧売単価（税込み）
            //        //#endregion
            //        //// グリッド上の旧売単価（税込み）
            //        //detailRow.Attribute02 = string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text) ? "0" : this.DetailAddSlipOldPriceSalesChange.Text;

            //        //// グリッド上の新売単価（税込み）
            //        //detailRow.Attribute03 = string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? "0" : this.DetailAddSlipNewPriceSalesChange.Text;
            //        //// 2013.10.21 AST Chg End
            //        // <-- 2013/11/01 マスタ売価対応

            //        // 8/21 SUEYOSHI 売変伝票の場合、新売単価をチェック
            //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            //        {
            //            if (this.SlipDivisionCoverDropDownList.SelectedValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice))
            //            {
            //                // 値下伝票で新単価が値上げされていた場合業務エラー
            //                // 2014.05.09 Start 効率改善指摘事項の修正
            //                // 同じ単価も警告とする
            //                //if (detailRow.SlipNewPrice > detailRow.SlipOldPrice)
            //                if (detailRow.SlipNewPrice >= detailRow.SlipOldPrice)
            //                // 2014.05.09 End   効率改善指摘事項の修正
            //                {
            //                    // Exceptionを発生させる
            //                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB248, "新売単価", "旧売単価")));
            //                    // 2014.05.09 Start 効率改善指摘事項の修正
            //                    this.DetailAddSlipNewPriceSalesChange.Focus();
            //                    // 2014.05.09 End   効率改善指摘事項の修正
            //                    return;
            //                }
            //            }
            //            else
            //            {
            //                // 値上伝票で新単価が旧単価より値下げされていた場合業務エラー
            //                // 2014.05.09 Start 効率改善指摘事項の修正
            //                // 同じ単価も警告とする
            //                //if (detailRow.SlipNewPrice < detailRow.SlipOldPrice)
            //                if (detailRow.SlipNewPrice <= detailRow.SlipOldPrice)
            //                // 2014.05.09 End   効率改善指摘事項の修正
            //                {
            //                    // Exceptionを発生させる
            //                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB249, "新売単価", "旧売単価")));
            //                    // 2014.05.09 Start 効率改善指摘事項の修正
            //                    this.DetailAddSlipNewPriceSalesChange.Focus();
            //                    // 2014.05.09 End   効率改善指摘事項の修正
            //                    return;
            //                }
            //            }
            //        }
            //        // 8/21 SUEYOSHI END

            //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
            //        {
            //            detailRow.SlipDisposalUnitPrica = decimal.Parse(this.DetailAddSlipNewPriceDisposal.Text);
            //            // 2013/09/23 k.haraikawa 予備０１は商品名称なので、税込み金額は予備０２から設定する。
            //            // --> 2013/11/01 マスタ売価対応
            //            //// 2013.08.22 Start Attribute04 = グリッド上の廃棄売単価（税込み）
            //            //detailRow.Attribute04 = this.DetailAddSlipNewPriceDisposal.Text;
            //            //// 2013.08.22 End   Attribute04 = グリッド上の廃棄売単価（税込み）
            //            // <-- 2013/11/01 マスタ売価対応
            //        }

            //        if (dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
            //        {
            //            inputSequenceNo = dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
            //        }

            //        // 「入力バッチ番号・伝票番号」毎にインクリメントする。
            //        detailRow.InputSequenceNo = inputSequenceNo.ToString();

            //        // グリッド上の（数量×新売単価）
            //        detailRow.SalesChangePrice = detailRow.DisposalChangeQty * detailRow.SlipNewPrice;

            //        // グリッド上の（数量×売単価）
            //        detailRow.DisposalPriceAmt = detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica;

            //        detailRow.Reason = this.DetailAddReason.SelectedItem.Text;

            //        dsTmpEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);
            //        dsTmpEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Merge(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError);
            //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsTmpEntity);
            //        dsEntity = dsTmpEntity;

            //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode, "0");
            //    }
            //}
            //// 追加処理終了後入力エリアをクリア
            //this.ClearInputArea();
            //// 8/16 SUEYOSHI END
            
            // <-- 2014/06/24 保存時に新規分も登録されてしまう不具合を修正

            // Dｓ（データセット）の生成
            // 8/16 SUEYOSHI 保存ボタン押下の際に、追加ボタン押下処理と同様の処理を追加 DEL
            // DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            // 8/16 SUEYOSHI END
            #endregion
            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);
            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
            // カバーシートマスタワークエンティティ
            CoverSheetMasterEntity coverSheetMasterEntity = (CoverSheetMasterEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity);
            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            DsEntity dsInputStatusEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity);
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。


            //20151014 start 明細行編集できるように仕様変更　登録前に空白行削除　ri
            foreach (var deleteRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Where(x => string.IsNullOrEmpty(x.Field<string>("SKUCd"))).ToArray())
            {
                deleteRow.Delete();
            }

            //20151014 end 明細行編集できるように仕様変更　登録前に空白行削除　ri

            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count == 0 && dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count == 0)
            {
                //   2011.12.07 関根 伝票明細が０件の時のメッセージを変更 修正 開始
                //// 保存がされていない場合はデータが反映されない確認メッセージ[MSGCSARI0017]を表示する。
                //this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCSARI0017)));

                // 保存がされていない場合はデータが反映されない確認メッセージ[MSGCSARI0017]を表示する。
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCBYRE0114, this.TransferSlipNumberTextBox.Text)));
                //   2011.12.07 関根 伝票明細が０件の時のメッセージを変更 修正 終了

                //20151014 start 明細行編集できるように仕様変更　ri

                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

                this.AddBlankRowToDataGrid();
                //20151014 end 明細行編集できるように仕様変更　　ri

                // 2014.05.09 Start 効率改善指摘事項の修正
                //this.DetailAddSKUCd.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
                return;
            }

            this.SetItemsForSarHeaderErrorDataSet(dsEntity, workEntity);

            //2015.11.16 start 既存伝票の新規追加行が削除できるように追加

            DsEntity OrgDsEntity = new DsEntity();

            OrgDsEntity.SARSlipDetailErrorDataSet = (SARSlipDetailErrorInfoDataSet)dsEntity.SARSlipDetailErrorDataSet.Copy();

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.OrgDsEntity, OrgDsEntity);
            //2015.11.16 end 既存伝票の新規追加行が削除できるように追加

            // 生成したDsをDsEntityへ格納
            // WorkEntity.伝票実行モード＝１をセット（１：伝票登録を実行、０：伝票登録をしない）
            workEntity.SlipExecuteMode = "1";

            // 2013/09/25 いなげやアドオン対応：認証モードによりカバー更新モードを設定 ISC:yshindo start
            // WorkEntity.カバー更新モード＝０をセット（１：カーバーシート登録を実行、０：カーバーシート登録をしない）
            workEntity.CoverExecuteMode = "0";

            // 2013.08.20 Start 
            // WorkEntity.承認モード＝SAR_ApprovalFeatureDivをセット
            //workEntity.ApprovalMode = APVariableListKey.SAR_ApprovalFeatureDiv;
            if (SalesChangeDisposalSlipInputConstances.Disposal.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
            {
                // 現在入力中の伝票種別が廃棄伝票の場合
                // 認証モードの設定
                workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDiv);

                // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo start
                // 実行モード←"100"：計上処理モード
                //start  20220126 orizon.ri 標準不具合051,052 自動承認しないように（伝票作成する時点で大福帳作成しない）
                //workEntity.ExecuteMode = "100";
                workEntity.ExecuteMode = "1";
                //start  20220126 orizon.ri 標準不具合051,052 自動承認しないように（伝票作成する時点で大福帳作成しない）
                // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo end
            }
            else if (SalesChangeDisposalSlipInputConstances.SalesChange.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
            {
                // 現在入力中の伝票種別が売変伝票の場合
                workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDivForSalesChange);

                // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo start
                // 実行モード←"1"
                workEntity.ExecuteMode = "1";
                // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo end
            }
            // 2013.08.20 End 認証モードの設定

            // WorkEntity.カバー更新モード＝０をセット（１：カーバーシート登録を実行、０：カーバーシート登録をしない）
            // 承認しない
            if (workEntity.ApprovalMode.Equals("0"))
            {
                workEntity.CoverExecuteMode = "1";
            }
            // 承認する
            else
            {
                workEntity.CoverExecuteMode = "0";
            }
            // 2013/09/25 いなげやアドオン対応：認証モードによりカバー更新モードを設定 ISC:yshindo end

            // 2012.06.26 Start カバーシートの入力枚数を画面入力値へ変更
            CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetRow = dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0];
            // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            //coverSheetRow.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            coverSheetRow.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 1 : int.Parse(this.InputPageCountTextBox.Text);
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            // 伝票情報用のDataSetへマージする
            dsInputStatusEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);

            // 2014.05.16 Start 伝票起票の入力枚数は使用しないので非表示とする
            //coverSheetMasterEntity.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            coverSheetMasterEntity.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 1 : int.Parse(this.InputPageCountTextBox.Text);
            // 2014.05.16 End   伝票起票の入力枚数は使用しないので非表示とする

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity, coverSheetMasterEntity);
            // 2012.06.26 End   カバーシートの入力枚数を画面入力値へ変更

            // 【BSFCSAR050800030】
            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
            //this.GetFacadeInstance().UpInsSARSlip(workEntity, dsEntity, initialDisplayEntity);
            this.GetFacadeInstance().UpInsSARSlipNew(workEntity, dsEntity, initialDisplayEntity, coverSheetMasterEntity);
            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了

            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            //if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Rows.Count > 0)
            //{
            //    this.ShowSARCoverSheetMasterData(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Rows[0]);
            //}

            if (dsInputStatusEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
            {
                this.ShowSARCoverSheetMasterData(dsInputStatusEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows[0]);
            }

            // カバーシートに保存されている伝票全てを取得するので対象の伝票番号項目は空にする。
            workEntity.TargetSlipNo = string.Empty;
            // 伝票ヘッダー、伝票明細をクリア
            dsInputStatusEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
            dsInputStatusEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();

            // 伝票検索のファサードを実行する。
            // 【BSFCSAR050800032】
            this.GetFacadeInstance().FillSARSlipNew(workEntity, dsInputStatusEntity);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity, dsInputStatusEntity);

            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

            this.TargetDataTable = this.GetSARSlipHeaderErrorDataset(this.GetPageNumber()).SARSlipHeaderError;
            this.DataBindToinputStatusListGrid();

            // 2013.10.21 AST Chg Start
            // 2013.11.26 関根 １カバーシートに複数伝票登録した場合に、明細グリッドに全伝票分の明細が出力される不具合を修正 START
            //#region 修正前
            ////this.OriginalDetailDataSet = dsEntity.SARSlipDetailErrorDataSet;
            ////this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
            //#endregion
            //this.OriginalDetailDataSet = dsInputStatusEntity.SARSlipDetailErrorDataSet;
            //this.TargetDataSet = dsInputStatusEntity.SARSlipDetailErrorDataSet;

            // 上記の伝票検索ファサード（FillSARSlip）から取得した伝票明細には、対象カバーシートの全伝票分の明細がセットされいる為、
            // 明細グリッドに全明細分が表示されてしまう。
            // その為、最後に登録した伝票の明細を表示する。（※修正前の状態に戻す）
           
            this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
            // 2013.11.26 関根 １カバーシートに複数伝票登録した場合に、明細グリッドに全伝票分の明細が出力される不具合を修正 END
            
            // 2014.04.30 Start １カバーシートに複数伝票登録した状態で、同一伝票を続けて保存すると全伝票分の明細が更新される不具合修正
            // 上記の伝票検索ファサード（FillSARSlip）から取得した伝票明細には、対象カバーシートの全伝票分の明細がセットされいる為、
            // その情報をセッションに保持してしまうと、２回以上続け「保存(F12)」を行うと、
            // 全明細が一緒に更新されてしまう。（※修正前の状態に戻す）
            //this.DsEntitySession = dsInputStatusEntity;
            // 2014.04.30 End   １カバーシートに複数伝票登録した状態で、同一伝票を続けて保存すると全伝票分の明細が更新される不具合修正

            // 2013.10.21 AST Chg End

            this.DataBindToSlipDetailGridView();
            this.BindToDetailReasonDownList();

            //20151020 start 明細行直接編集へ仕様変更　保存後明細行6行表示維持
            this.AddBlankRowToDataGrid();
            //20151020 end 明細行直接編集へ仕様変更　保存後明細行6行表示維持

            this.SetLineNoForGridView(this.inputStatusListGrid, "SARCoverSheetMaster_Number_Control", this.GetPageNumber());
            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            // 2012.06.26 Start 伝票明細情報の数値項目に小数点以下が表示されるので表示しない。
            this.ConvertToCurencyFormat();
            // 2012.06.26 End   伝票明細情報の数値項目に小数点以下が表示されるので表示しない。
            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(true);
            this.SetHiddenGridViewBySlipType((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue));

            // 2014.04.30 Start 保存(F12)押下時は、ヘッダ情報は変更不可とする
            //// 2014.01.06 Start 伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。
            ////// 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
            ////this.SetHeaderEnableControls(true);
            ////// 2012.06.26 End   伝票ヘッダー情報の入力制御を修正
            //
            //if (workEntity.InitMode == "1" || workEntity.InitMode == "2")
            //{
            //    this.SetHeaderEnableControls(false);
            //}
            //else
            //{
            //    this.SetHeaderEnableControls(true);
            //}
            //// 2014.01.06 End   伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。

            // メニュー画面起動時のみ、ヘッダ情報変更可としていたが、
            // 同一伝票番号で新規登録を行なえてしまう為、
            // 一度保存した場合は変更不可とする。
            this.SetHeaderEnableControls(false);
            // 2014.04.30 End   保存(F12)押下時は、ヘッダ情報は変更不可とする

            // メッセージMSGCSARI0028を表示する。{0}は「伝票保存」
            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCSARI0028, "伝票保存")));

            // 2014.05.09 Start 効率改善指摘事項の修正
            //this.DetailAddSKUCd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正
            
            // 新規 一旦承認されたカバーシートには新し伝票を追加しないようにする
            if (dsInputStatusEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Any() &&
                dsInputStatusEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApprovalStatus == "3"&&
                workEntity.ApprovalMode == "1")
            {
                this.NewButton.Enabled = false;
            }
            else
            {
                this.NewButton.Enabled = true;

            }
            
            this.Master.HeaderMessageLabelString = RetailConstant.DisplayMode.Modify;
            this.Master.HeaderBackColor = Color.FromName(RetailConstant.ColorDisplayMode.Modify);
        }
        #endregion

        #region DetailClearButton_Click (明細クリアボタン)
        /// <summary>
        /// 明細クリアボタン
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void DetailClearButton_Click(object sender, EventArgs e)
        {
            //20151020 start 明細行直接変更するようにより処理不要　コメントアウト
            // 8/9 SUEYOSHI 行選択後からクリアボタン押下時の処理 ADD START
            //if (this.TargetDataSet != this.OriginalDetailDataSet)
            //{
            //    DsEntity dsEntity = this.DsEntitySession;

            //    this.TargetDataSet = this.OriginalDetailDataSet;
            //    this.DataBindToSlipDetailGridView();

            //    // 2012.11.02 Start GridへBindしたあとの売変、廃棄のGrid調整を追加
            //    this.SetHiddenGridViewBySlipType((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue));
            //    // 2012.11.02 End   GridへBindしたあとの売変、廃棄のGrid調整を追加

            //    // 選択ボタン押下イベントで、セッションからdsEntityを取得する為、OriginalDetailDataSetをDsEntityに格納後セッションに登録
            //    dsEntity.SARSlipDetailErrorDataSet = (SARSlipDetailErrorInfoDataSet)this.OriginalDetailDataSet;

            //    this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            //}
            //20151020 end 明細行直接変更するようにより処理不要　コメントアウト

            // 8/9 SUEYOSHI END

            //   2011.12.10 関根 新規追加クリアボタン押下のクリア処理修正(text boxのみクリア) 削除 開始
            //this.TargetDataSet = this.OriginalDetailDataSet;
            //this.DataBindToSlipDetailGridView();

            //this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            //this.ConvertToCurencyFormat();
            //this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice();
            //   2011.12.10 関根 新規追加クリアボタン押下のクリア処理修正(text boxのみクリア) 削除 終了

            // 2014.01.06 Start 伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。
            //// 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
            //this.SetHeaderEnableControls(true);
            //// 2012.06.26 End   伝票ヘッダー情報の入力制御を修正

            //20151020 start 明細行直接変更するようにより処理不要　コメントアウト
            //WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);
            //if (workEntity.InitMode == "1" || workEntity.InitMode == "2")
            //{
            //    this.SetHeaderEnableControls(false);
            //}
            //else
            //{
            //    this.SetHeaderEnableControls(true);

            //}
            //20151020 end 明細行直接変更するようにより処理不要　コメントアウト

            // 2014.01.06 End   伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。

            //20151020 start 明細行直接変更するようにより処理不要　コメントアウト

            //   2011.12.10 関根 新規追加クリアボタン押下時の明細行が削除される不具合修正 追加 開始
            //this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            //this.ConvertToCurencyFormat();
            //this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice();
            //   2011.12.10 関根 新規追加クリアボタン押下時の明細行が削除される不具合修正 追加 終了
            //20151020 end 明細行直接変更するようにより処理不要　コメントアウト

            // 新規モードの際は新規追加オブジェクトをすべてクリアする。
            // 2012.06.26 Start 入力エリアのクリア箇所を変更
            //this.DetailAddSKUCd.Text = string.Empty;
            //this.DetailAddTransferQuantity.Text = string.Empty;
            //this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
            //this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
            //this.DetailAddReason.SelectedIndex = 0;

            //20151020  start 明細行にて編集できるように変更により不要　コメントアウト　ri
            //this.ClearInputArea();
            // 2012.06.26 End   入力エリアのクリア箇所を変更

            // 2014.05.09 Start 効率改善指摘事項の修正
            //this.DetailAddSKUCd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正
            //20151020  end 明細行にて編集できるように変更により不要　コメントアウト　ri

            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Clear();
            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AcceptChanges();

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

            this.Category1Cd_TextChanged(this.Category1Cd, new EventArgs());
        }
        #endregion

//        #region DetailChoiceButton_Click (選択ボタン)
//        /// <summary>
//        /// 選択ボタン
//        /// </summary>
//        /// <param name="sender">イベント送信元のオブジェクト。</param>
//        /// <param name="e">イベント引数。</param>
//        protected void DetailChoiceButton_Click(object sender, EventArgs e)
//        {
//            int countSelect = 0;
//            int rowIndex = 0;

//            // 2014.05.27 Start Gridの数量チェックを強化
//            // 伝票明細グリッドチェック
//            if (CheckSlipDeatilGridInput().Equals(true))
//            {
//                return;
//            }
//            // 2014.05.27 End   Gridの数量チェックを強化

//            // 2013.08.21 Start 数量のグリッド保管
//            this.SaveDisplayControl();
//            // 2013.08.21 End 数量のグリッド保管
//            foreach (GridRecord record in this.SlipDetailGridView.Rows)
//            {
//                CoreCenterCheckBox selectCheckBox = (CoreCenterCheckBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_Select_Control");
//                if (selectCheckBox != null && selectCheckBox.Checked)
//                {
//                    countSelect++;
//                    rowIndex = record.Index;
//                }
//            }

//            // 選択チェック
//            if (countSelect == 0)
//            {
//                // 明細行で１つもチェックボックスがＯＮになっていなければ、エラーメッセージ：「ERRCSARB0043」をスローする。
//                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0043)));
//                // 2014.05.09 Start 効率改善指摘事項の修正
//                this.DetailAddSKUCd.Focus();
//                // 2014.05.09 End   効率改善指摘事項の修正
//                return;
//            }
//            else if (countSelect > 1)
//            {
//                // 明細行に複数行のチェックONが確認される場合はエラーメッセージ：「ERRCSARB0134」をスローする。
//                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0134, "１件", "選択")));
//                // 2014.05.09 Start 効率改善指摘事項の修正
//                this.DetailAddSKUCd.Focus();
//                // 2014.05.09 End   効率改善指摘事項の修正
//                return;
//            }

//            DsEntity dsEntity = this.DsEntitySession;

//            // 選択された１行を新規追加オブジェクトへセットする。
//            var row = (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow)dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[rowIndex];
//            this.ShowSARSlipDetailErrorData(row);
//#warning SKN臨時対応　商品情報取得 -->
//            // 商品名保持
//            this.SetSession(SalesChangeDisposalSlipInputSessionKey.SkuName, row.Attribute01.ToString());
//#warning SKN臨時対応　商品情報取得 <--

//            this.SetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode, "1");
//            // 2012.06.26 Start 商品を選択した場合は商品コードの変更は不可とする。
//            this.DetailAddSKUCd.Enabled = false;
//            // 2012.06.26 End   商品を選択した場合は商品コードの変更は不可とする。
//            // 2014.05.28 Start 明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
//            // 選択中は選択ボタンを押せなくする
//            this.DetailChoiceButton.Enabled = false;
//            this.DetailDeleateButton.Enabled = false;
//            // 選択中の場合は保存ボタンのメッセージを変更する
//            this.NextButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0029);
//            // 2014.05.28 End   明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
//            // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
//            string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;

//            // AP変数の値を元に入力エリアの売価項目に対して入力可、不可の制御を行う
//            // （0：変更不可、1：変更可能、3：警告あり商品のみ可能）
//            if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "3")
//            {
//                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
//                {
//                    // 売変の金額入力変更
//                    CheckAndChangeSalesChangePriceInput();
//                }
//                else
//                {
//                    // 廃棄の金額入力変更
//                    CheckAndChangeDisposalPriceInput();
//                }
//            }
//            // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする

//            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.RemoveAt(rowIndex);
//            this.SetSession(SalesChangeDisposalSlipInputSessionKey.RowIndex, rowIndex.ToString());

//            this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
//            this.DataBindToSlipDetailGridView();

//            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
//            this.ConvertToCurencyFormat();
//            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice();
//            // 2012.10.26 Start 廃棄入力中に選択ボタンを押すと一覧が売変用に変わってしまうので修正
//            this.SetHiddenGridViewBySlipType((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue));
//            // 2012.10.26 End   廃棄入力中に選択ボタンを押すと一覧が売変用に変わってしまうので修正

//            // 2014.05.09 Start 効率改善指摘事項の修正
//            this.DetailAddTransferQuantity.Focus();
//            // 2014.05.09 End   効率改善指摘事項の修正
//        }
//        #endregion

        #region DetailDeleateButton_Click (明細削除ボタン)
        /// <summary>
        /// 明細削除ボタン
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void DetailDeleateButton_Click(object sender, EventArgs e)
        {
            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(false);

            this.SaveDropDownList();

            bool isChecked = false;
            foreach (GridRecord record in this.SlipDetailGridView.Rows)
            {
                CoreCenterCheckBox selectCheckBox = (CoreCenterCheckBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_Select_Control");

                if (selectCheckBox != null && selectCheckBox.Checked)
                {
                    isChecked = true;
                    break;
                }
            }

            if (!isChecked)
            {
                // 2014.08.04 Start 行削除時の未選択エラーメッセージ変更                
                //// 明細行で１つもチェックボックスがＯＮになっていなければ、エラーメッセージ：【ERRCSARB0216】をスローする。
                //this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0216, "伝票明細グリッド項目")));
                this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
                // ！選択欄にチェックをつけて選択して下さい。！
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0276)));
                // 2014.08.04 End   行削除時の未選択エラーメッセージ変更

                // 2014.05.09 Start 効率改善指摘事項の修正
                this.DetailAddSKUCd.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
                return;
            }

            // Dｓ（データセット）の生成
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            
            // 20151023 start 明細行直接編集により不要
            //InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
            //WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);
            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
            // カバーシートマスタワークエンティティ
            //CoverSheetMasterEntity coverSheetMasterEntity = (CoverSheetMasterEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity);
            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
            // 20151023 end 明細行直接編集により不要


            //if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count == 0)
            //{
            //    // データの登録が行われていない場合に、行削除を行った場合、エラーメッセージ：【ERRCSARB0】をスローする。
            //    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0216, "伝票明細グリッド項目")));
            //    return;
            //}

            // 8/16 SUEYOSHI ＤＢに登録されていないデータに対して行削除を行った場合の対応
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count == 0)
            {
                if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
                {
                    // 2012.08.27 Start 新規入力時のＤＢへ更新前の行削除ではＤＢへ更新せず行のみを削除する
                    //// 新規伝票の場合はＤＳをクリアして変更をコミットする
                    //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Clear();
                    //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AcceptChanges();

                    //// クリアしたＤＳをバインド
                    //SARSlipDetailErrorInfoDataSet deleteSarSlipDetailErrorInfoDataSet = new SARSlipDetailErrorInfoDataSet();
                    //deleteSarSlipDetailErrorInfoDataSet = dsEntity.SARSlipDetailErrorDataSet;
                    //this.TargetDataSet = deleteSarSlipDetailErrorInfoDataSet;

                    for (int recIndex = this.SlipDetailGridView.Rows.Count - 1; recIndex >= 0; recIndex--)
                    {
                        CoreCenterCheckBox selectCheckBox = (CoreCenterCheckBox)WebDataGridUtil.FindControl(this.SlipDetailGridView.Rows[recIndex], "SARPurchaseSlipDetail_Select_Control");
                        if (selectCheckBox.Checked)
                        {
                            //20151015 start 行削除不具合修正　
                            //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[recIndex].Delete();
                            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.RemoveAt(recIndex);
                            //20151015 end 行削除不具合修正
                        }
                    }

                    //20151009  start 仕様変更後明細行不具合修正　ri
                    this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
                    //20151009  end 仕様変更後明細行不具合修正　ri

                    this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
                    // 2012.08.27 End   新規入力時のＤＢへ更新前の行削除ではＤＢへ更新せず行のみを削除する
                    this.DataBindToSlipDetailGridView();

                    //20151009  start 仕様変更後明細行不具合修正　ri
                    this.BindToDetailReasonDownList();
                    //20151009  end 仕様変更後明細行不具合修正　ri

                    // 2012.11.02 Start GridへBindしたあとの売変、廃棄のGrid調整を追加
                    this.SetHiddenGridViewBySlipType((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue));
                    // 2012.11.02 End   GridへBindしたあとの売変、廃棄のGrid調整を追加

                    //グリッドライン番号再セット
                    this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);

                    // メッセージMSGCSARI0028を表示する。{0}は「明細削除」
                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCSARI0028, "明細削除")));

                    // 以下の処理でヘッダー情報を作成する為、処理を終了する（２回目以降の削除対応）
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    //this.DetailAddSKUCd.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                }
            }
            // 8/16 SUEYOSHI END
            // 2014.04.30 Start 行削除のエラー対応（行削除→クリア→行削除の順に行うと同一伝票が新規で登録されてしまう）
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            //
            //    行削除ボタン押下時に伝票テーブルまで更新を行っている為、
            //    クリアボタンで行削除前の状態に戻ってしまうと、
            //    「画面上は修正前のデータ」「伝票テーブルは行削除後のデータ」となり、
            //    この状態で登録処理を行うと同一伝票が複数作成されてしまう。
            //
            //    行削除の運用を以下の通りに変更する。
            //      ・行削除では伝票テーブルの更新は行わない。
            //      ・新規伝票のみ行削除を行うようにし、画面表示のみ行削除を行う。
            //      ・既存伝票の場合は、数量０訂正を行うようにメッセージを表示させるのみとする。
            //
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            else
            {
                DsEntity OrgdsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.OrgDsEntity);

                //2015.11.16 start 登録済み伝票に新規商品追加する場合、画面にて削除できるようにする
                for (int recIndex = this.SlipDetailGridView.Rows.Count - 1; recIndex >= 0; recIndex--)
                {
                    CoreCenterCheckBox selectCheckBox = (CoreCenterCheckBox)WebDataGridUtil.FindControl(this.SlipDetailGridView.Rows[recIndex], "SARPurchaseSlipDetail_Select_Control");
                    if (selectCheckBox.Checked)
                    {
                        int inputSequenceLineNo = recIndex + 1;
                        DataRow[] dataRows = OrgdsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Select("InputSequenceLineNo = " + inputSequenceLineNo);

                        ////デバッグ用
                        //object[] parameter = { "テスト：2698行目：dataRows.count:" + dataRows.Count() + "/" + recIndex + "/" + OrgdsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count };
                        //throw RetailBusinessException.CreateInstance(ErrorKey.ERRCCMRB1056, parameter);

                        if (dataRows.Count() <=0)
                        {
                            //20151015 start 行削除不具合修正　
                            //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[recIndex].Delete();
                            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.RemoveAt(recIndex);
                            //20151015 end 行削除不具合修正
                        }else {
                            
                            // 既存伝票の行削除は、数量０訂正のメッセージ表示される。
                            // ！{0}を入力してください。！  
                            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0277, "既存伝票の行削除は、数量０訂正", inputSequenceLineNo)));

                            // 2014.05.09 Start 効率改善指摘事項の修正
                            //this.DetailAddSKUCd.Focus();
                            // 2014.05.09 End   効率改善指摘事項の修正
                            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
                            
                            //return;
                        }
                    }
                }


                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

                this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;

                this.DataBindToSlipDetailGridView();

                this.BindToDetailReasonDownList();

                this.SetHiddenGridViewBySlipType((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue));
                //2015.11.16 end 登録済み伝票に新規商品追加する場合、画面にて削除できるようにする
            }
            // 2014.04.30 End   行削除のエラー対応（行削除→クリア→行削除の順に行うと同一伝票が新規で登録されてしまう）

            #region 削除
            // 2014.04.30 Start 行削除のエラー対応（行削除→クリア→行削除の順に行うと同一伝票が新規で登録されてしまう）
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 行削除時は、伝票テーブルを更新しない為、以下の処理をコメント
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 開始
            //// 処理タイミングを変更の為、削除
            ////this.SetItemsForSarHeaderErrorDataSet(dsEntity, workEntity);
            ////   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 終了
            //
            //bool isDisPlayOff = this.CheckSar(initialDisplayEntity, APVariableListKey.SAR_CheckDisplay_Off);
            //
            //// 2012.08.27 Start 行削除時の全行削除は業務エラーメッセージを表示する
            //// 全ての明細がチェックされているかを確認
            //int checkSlipDetailCount = 0;
            //foreach (GridRecord record in this.SlipDetailGridView.Rows)
            //{
            //    CoreCenterCheckBox selectCheckBox = (CoreCenterCheckBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_Select_Control");
            //    if (selectCheckBox != null && selectCheckBox.Checked)
            //    {
            //        checkSlipDetailCount++;
            //    }
            //}
            //// 全ての明細がチェックされている場合はメッセージを表示
            //if (this.SlipDetailGridView.Rows.Count == checkSlipDetailCount)
            //{
            //    // 業務エラーメッセージERRCSARB0140を表示する。{0}は「全ての行削除は伝票取消ボタン」
            //    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0140, "全ての行削除は伝票取消ボタン")));
            //    return;
            //}
            //// 2012.08.27 End   行削除時の全行削除は業務エラーメッセージを表示する
            //
            //foreach (GridRecord record in this.SlipDetailGridView.Rows)
            //{
            //    CoreCenterCheckBox selectCheckBox = (CoreCenterCheckBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_Select_Control");
            //    if (selectCheckBox.Checked)
            //    {
            //        dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[record.Index]["LogicalDelFlg"] = "1";
            //    }
            //}
            //
            ////   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 開始
            //// 削除フラグをセットした後に、ヘッダーを更新する
            //this.SetItemsForSarHeaderErrorDataSet(dsEntity, workEntity);
            ////   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 終了
            //
            //// 2012.06.26 Start カバーシートの入力枚数を画面入力値へ変更
            //CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetRow = dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0];
            //coverSheetRow.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            //// 伝票情報用のDataSetへマージする
            //DsEntity dsInputStatusEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity);
            //dsInputStatusEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);
            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity, dsInputStatusEntity);
            //
            //coverSheetMasterEntity.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity, coverSheetMasterEntity);
            //// 2012.06.26 End   カバーシートの入力枚数を画面入力値へ変更
            //
            //
            //// 2013.12.06 関根 明細行削除時も保存処理と同様で、認証モードをチェックするように修正 Start
            //// WorkEntity.承認モード＝SAR_ApprovalFeatureDivをセット
            //if (SalesChangeDisposalSlipInputConstances.Disposal.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
            //{
            //    // 現在入力中の伝票種別が廃棄伝票の場合
            //    // 認証モードの設定
            //    workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDiv);
            //
            //    // 実行モード←"100"：計上処理モード
            //    workEntity.ExecuteMode = "100";
            //}
            //else if (SalesChangeDisposalSlipInputConstances.SalesChange.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
            //{
            //    // 現在入力中の伝票種別が売変伝票の場合
            //    workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDivForSalesChange);
            //
            //    // 実行モード←"1"
            //    workEntity.ExecuteMode = "1";
            //}
            //
            //// WorkEntity.カバー更新モード＝０をセット（１：カーバーシート登録を実行、０：カーバーシート登録をしない）
            //// 承認しない
            //if (workEntity.ApprovalMode.Equals("0"))
            //{
            //    workEntity.CoverExecuteMode = "1";
            //}
            //// 承認する
            //else
            //{
            //    workEntity.CoverExecuteMode = "0";
            //}
            //// 2013.12.06 関根 明細行削除時も保存処理と同様で、認証モードをチェックするように修正 End
            //
            //
            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            //
            //if (isDisPlayOff)
            //{
            //    // AP変数：SAR_CheckDisplay_Off＝０
            //    // WorkEntity.ボタンID＝３：明細削除をセット
            //    workEntity.ButtonId = "3";
            //
            //    // 生成したDsEntityとWorkEntityをセッションへ登録し確認画面へ遷移する
            //    this.SetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity, workEntity);
            //
            //    // 生成したDsEntityとWorkEntityをセッションへ登録し確認画面へ遷移する
            //    this.Redirect("SCRCSAR050208", null);
            //}
            //else
            //{
            //    // 【BSFCSAR050800012】
            //    // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
            //    //this.GetFacadeInstance().DelSARSlipDetail(workEntity, dsEntity, initialDisplayEntity);
            //    this.GetFacadeInstance().DelSARSlipDetail(workEntity, dsEntity, initialDisplayEntity, coverSheetMasterEntity);
            //    // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
            //    this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            //}
            //
            ////   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 開始
            ////SARSlipDetailErrorInfoDataSet sarSlipDetailErrorInfoDataSet = new SARSlipDetailErrorInfoDataSet();
            ////sarSlipDetailErrorInfoDataSet = dsEntity.SARSlipDetailErrorDataSet;
            //
            ////foreach (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detailRow in sarSlipDetailErrorInfoDataSet.SARSlipDetailError.Rows)
            ////{
            ////    if (detailRow.LogicalDelFlg == "1")
            ////    {
            ////        detailRow.Delete();
            ////    }
            ////}
            //
            //SARSlipDetailErrorInfoDataSet sarSlipDetailErrorInfoDataSet = new SARSlipDetailErrorInfoDataSet();
            //sarSlipDetailErrorInfoDataSet = dsEntity.SARSlipDetailErrorDataSet;
            //
            //
            //int maxRowsCount = sarSlipDetailErrorInfoDataSet.SARSlipDetailError.Count;
            //for (int i = maxRowsCount - 1; i >= 0; i--)
            //{
            //    SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detailRow = sarSlipDetailErrorInfoDataSet.SARSlipDetailError[i];
            //    if (detailRow.LogicalDelFlg == "1")
            //    {
            //        detailRow.Delete();
            //    }
            //}
            //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AcceptChanges();
            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            //// 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
            ////this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, dsEntity);
            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode, "0");
            //// 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
            ////   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 終了
            //// 2012.06.26 Start 商品を選択した場合は商品コードの変更は不可とする。
            //this.DetailAddSKUCd.Enabled = true;
            //// 2012.06.26 End   商品を選択した場合は商品コードの変更は不可とする。
            //
            //this.TargetDataSet = sarSlipDetailErrorInfoDataSet;
            //this.DataBindToSlipDetailGridView();
            //
            //this.SetHiddenGridViewBySlipType(this.SlipTypeCoverDropDownList.SelectedValue);
            //
            //this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            //this.ConvertToCurencyFormat();
            //this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice();
            //
            //// 2012.08.27 Start 行削除時に伝票状況を表示し直す
            //// カバーシートに保存されている伝票全てを取得するので対象の伝票番号項目は空にする。
            //workEntity.TargetSlipNo = string.Empty;
            //
            //// 8/30 SUEYOSHI カバーシートに保存されている伝票番号項目を空にする　
            //workEntity.SlipSubNo = string.Empty;
            //// 8/30 SUEYOSHI END
            //
            //// 伝票ヘッダー、伝票明細をクリア
            //dsInputStatusEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
            //dsInputStatusEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();
            //
            //// 伝票検索のファサードを実行する。
            //// 【BSFCSAR050800019】
            //this.GetFacadeInstance().FillSARSlip(workEntity, dsInputStatusEntity);
            //
            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity, dsInputStatusEntity);
            //
            //this.TargetDataTable = this.GetSARSlipHeaderErrorDataset(this.GetPageNumber()).SARSlipHeaderError;
            //this.DataBindToinputStatusListGrid();
            //this.SetLineNoForGridView(this.inputStatusListGrid, "SARCoverSheetMaster_Number_Control", this.GetPageNumber());
            //// 2012.08.27 End   行削除時に伝票状況を表示し直す
            //
            //// メッセージMSGCSARI0028を表示する。{0}は「明細削除」
            //this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCSARI0028, "明細削除")));
            // 2014.04.30 End   行削除のエラー対応（行削除→クリア→行削除の順に行うと同一伝票が新規で登録されてしまう）

            // 2014.05.09 Start 効率改善指摘事項の修正
            //this.DetailAddSKUCd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正
            #endregion

            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(false);
        }
        #endregion

        //20151015 start  明細行直接編集により不要　コメントアウト　
        #region DetailAdd (商品追加（編集反映）処理)
        /// <summary>
        /// 商品追加（編集反映）処理
        /// </summary>
        /// <returns>true：正常／false：チェックNG</returns>
        //private bool DetailAdd()
        //{
        //    bool ret = false;

        //    // ※Validatorエラーとなった場合、
        //    //   行番号が既に消えておりどの時点で消えているのか分からない為、
        //    //   チェック前に再度行番号を振り直す。
        //    this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);

        //    // --> 2014/06/23 伝票明細数制限
        //    string corporationCode = UserProfile.CorporationCode;

        //    string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;

        //    if (!string.IsNullOrEmpty(this.DetailAddSKUCd.Text)
        //        && this.DetailAddSKUCd.Enabled == true)
        //    {
        //        // 新規追加のとき

        //        // 最大明細数をチェック
        //        // 売変と廃棄で設定が異なる為、判定して取得
        //        string valueMaxDetailCount = (!string.IsNullOrEmpty(slipTypeValue) && slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange)) ? APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SALES_CHANGE_SLIP_MAX_DETAIL_COUNT, corporationCode) : APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_SLIP_MAX_DETAIL_COUNT, corporationCode);
        //        int maxDetailCount;
        //        if (int.TryParse(valueMaxDetailCount, out maxDetailCount) == false)
        //        {
        //            maxDetailCount = 99;
        //        }
        //        if (this.SlipDetailGridView.Rows.Count >= maxDetailCount)
        //        {
        //            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB242, maxDetailCount.ToString())));
        //            this.DetailAddSKUCd.Focus();
        //            return ret;
        //        }
        //    }
        //    // --< 2014/06/23 伝票明細数制限

        //    // 伝票ヘッダー情報チェック
        //    if (CheckSlipHeaderInfoInput().Equals(true))
        //    {
        //        return ret;
        //    }

        //    // 伝票明細グリッドチェック
        //    if (CheckSlipDeatilGridInput().Equals(true))
        //    {
        //        return ret;
        //    }

        //    // 伝票明細情報チェック
        //    if (CheckSlipDeatilInfoInput().Equals(true))
        //    {
        //        return ret;
        //    }
        //    // 2014.03.18 End   効率改善対応（Validatorコメント）

        //    // --> 2014/06/23 伝票明細数制限
        //    //string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;
        //    // --< 2014/06/23 伝票明細数制限
        //    // 2013.08.21 Start 数量のグリッド保管
        //    this.SaveDisplayControl();
        //    // 2013.08.21 End 数量のグリッド保管

        //    // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
        //    #region DEL
        //    //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
        //    //// 商品コードが未入力の場合は部門代表としてダミーコードをセットする
        //    //if (string.IsNullOrEmpty(this.DetailAddSKUCd.Text))
        //    //{
        //    //    string category1DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode);
        //    //    string category2DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode);
        //    //    if (string.IsNullOrEmpty(category1DummyCd) && string.IsNullOrEmpty(category2DummyCd))
        //    //    {
        //    //        // ダミーコードが取得できていない場合はダミーコードを取得する
        //    //        this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
        //    //        // ダミーコードの入れ直し
        //    //        category1DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode);
        //    //        category2DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode);
        //    //    }

        //    //    // 分類２がセットされていない場合は分類１のダミーコード。以外は分類２のダミーコードをセット。
        //    //    if (string.IsNullOrEmpty(this.Category2Cd.Text))
        //    //    {
        //    //        this.DetailAddSKUCd.Text = category1DummyCd;
        //    //    }
        //    //    else
        //    //    {
        //    //        this.DetailAddSKUCd.Text = category2DummyCd;
        //    //    }
        //    //}
        //    //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
        //    #endregion
        //    // 2014.05.28 End   部門代表コードの部分を機能しないように修正

        //    // 商品コードの必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
        //    if (string.IsNullOrEmpty(this.DetailAddSKUCd.Text))
        //    {
        //        // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
        //        //this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "SKUコード")));
        //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "商品コード")));
        //        // 2014.05.28 End   部門代表コードの部分を機能しないように修正
        //        // 2014.05.09 Start 効率改善指摘事項の修正
        //        this.DetailAddSKUCd.Focus();
        //        // 2014.05.09 End   効率改善指摘事項の修正
        //        return ret;
        //    }

        //    // 数量の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
        //    if (string.IsNullOrEmpty(this.DetailAddTransferQuantity.Text))
        //    {
        //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "数量")));
        //        // 2014.05.09 Start 効率改善指摘事項の修正
        //        this.DetailAddTransferQuantity.Focus();
        //        // 2014.05.09 End   効率改善指摘事項の修正
        //        return ret;
        //    }

        //    if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //    {
        //        // 新売単価の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
        //        if (string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text))
        //        {
        //            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "新売単価")));
        //            // 2014.05.09 Start 効率改善指摘事項の修正
        //            this.DetailAddSlipNewPriceSalesChange.Focus();
        //            // 2014.05.09 End   効率改善指摘事項の修正
        //            return ret;
        //        }
        //    }

        //    // 事由の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
        //    if (string.IsNullOrEmpty(this.DetailAddReason.SelectedItem.Text))
        //    {
        //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "事由")));
        //        // 2014.05.09 Start 効率改善指摘事項の修正
        //        this.DetailAddReason.Focus();
        //        // 2014.05.09 End   効率改善指摘事項の修正
        //        return ret;
        //    }

        //    // dsTmpEntity dsTmpEntity = this.dsTmpEntitySession; // (dsTmpEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.dsTmpEntity);
        //    // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
        //    //DsEntity dsTmpEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity);
        //    // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
        //    //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
        //    DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
        //    //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 終了

        //    // 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
        //    WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);

        //    workEntity.ItemCd = this.DetailAddSKUCd.Text;
        //    workEntity.ChangeDate = this.SalesChangeDate.DateValue;
        //    workEntity.StoreCd = this.StoreTextBox.Text;
        //    workEntity.IVGetSystemDiv = "IV";
        //    workEntity.IVGetDiv = "1";
        //    workEntity.Subsystem = "SA";
        //    workEntity.Category1Cd = this.Category1Cd.Text;
        //    workEntity.BatchDate = Convert.ToDateTime(this.BatchDateDateTextBox.Text);
        //    workEntity.ProductInfo_SalePrice = string.Empty;

        //    using (ItemInfoDataset itemInforDataSet = new ItemInfoDataset())
        //    {
        //        // 商品情報の取得
        //        this.GetFacadeInstance().FillItemInfo(workEntity, itemInforDataSet, dsEntity);

        //        if (itemInforDataSet.Tables[0].Rows.Count > 0)
        //        {
        //            // 2014.05.09 Start 効率改善指摘事項の修正
        //            if (itemInforDataSet.ItemInfo[0].Category1Cd != this.Category1Cd.Text)
        //            {
        //                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0275)));
        //                this.DetailAddSKUCd.Focus();
        //                return ret;
        //            }
        //            // 2014.05.09 End   効率改善指摘事項の修正

        //            this.SetSession(SalesChangeDisposalSlipInputSessionKey.SkuName, itemInforDataSet.ItemInfo[0].ProductsKanji);
        //        }
        //        else
        //        {
        //            // DataSetを空にする
        //            if (itemInforDataSet != null && itemInforDataSet.Tables.Count != 0 && itemInforDataSet.Tables[0].Rows.Count != 0)
        //            {
        //                itemInforDataSet.Tables[0].Clear();
        //            }
        //            this.SetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset, itemInforDataSet);
        //            // Exceptionを発生させる
        //            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
        //            // 2014.05.09 Start 効率改善指摘事項の修正
        //            this.DetailAddSKUCd.Focus();
        //            // 2014.05.09 End   効率改善指摘事項の修正
        //            return ret;
        //        }

        //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //        // 画面上に旧売価項目を追加するためSessionへ入れる必要がなくなったのでコメントアウト
        //        //// 取得した売単価を保持する(マスタ売価なので保持しておく)(売変時に旧売価として使用するため)
        //        //this.SetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice, workEntity.ProductInfo_SalePrice);
        //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

        //        // 入力されていない場合にのみセットする
        //        if (string.IsNullOrEmpty(this.DetailAddTransferQuantity.Text))
        //        {
        //            // 2014.06.14 Start 品質改善検証の指摘事項対応
        //            //this.DetailAddTransferQuantity.Text = workEntity.ProductInfo_LogicalStockQty;
        //            // 数量には空白をデフォルトとする。
        //            this.DetailAddTransferQuantity.Text = string.Empty;
        //            // 2014.06.14 End   品質改善検証の指摘事項対応
        //        }
        //        //DetailAddSKUCd_TextChanged_ProductInfo_LogicalStockQty = this.DetailAddTransferQuantity.Text;

        //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //        {
        //            // 売変
        //            // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //            if (string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text))
        //            {
        //                this.DetailAddSlipOldPriceSalesChange.Text = workEntity.ProductInfo_SalePrice;
        //            }
        //            //DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_OldSalePrice = this.DetailAddSlipOldPriceSalesChange.Text;
        //            // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //            if (string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text))
        //            {
        //                this.DetailAddSlipNewPriceSalesChange.Text = workEntity.ProductInfo_SalePrice;
        //            }
        //            //DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice = this.DetailAddSlipNewPriceSalesChange.Text;
        //        }
        //        else
        //        {
        //            // 廃棄
        //            if (string.IsNullOrEmpty(this.DetailAddSlipNewPriceDisposal.Text))
        //            {
        //                this.DetailAddSlipNewPriceDisposal.Text = workEntity.ProductInfo_SalePrice;
        //            }
        //            //DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice = this.DetailAddSlipNewPriceDisposal.Text;
        //        }

        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset, itemInforDataSet);
        //        this.SetSession("SKUCd", this.DetailAddSKUCd.Text);
        //    }

        //    // 廃棄の場合の処理
        //    if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
        //    {
        //        // ※廃棄の場合は後処理で金額などを求めている箇所で DetailAddSlipNewPriceSalesChange の項目を使っているので同様の値を入れておく
        //        this.DetailAddSlipNewPriceSalesChange.Text = this.DetailAddSlipNewPriceDisposal.Text;
        //    }
        //    // 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする

        //    int inputSequenceNo = 0;
        //    //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
        //    //if (dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
        //    //{
        //    //    inputSequenceNo = dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
        //    //}

        //    // dsTmpEntityを使用すると、連続で訂正した時に最新のInputSequenceNoが取得出来ないため、dsEntityより取得する。
        //    if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
        //    {
        //        inputSequenceNo = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
        //    }
        //    //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了

        //    DateTime systemDate = (DateTime)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SystemDate);

        //    string updateMode = "0";
        //    if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode) != null)
        //    {
        //        updateMode = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode);
        //    }

        //    ItemInfoDataset itemInfoDataset = (ItemInfoDataset)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset);
        //    //   2012.01.13 関根 明細の数量が変更出来ない不具合対応（伝票検索から遷移時） 変更 開始
        //    #region DEL
        //    //// 暫定修正 2011.11.28 Start 商品が未存在の場合はExceptionを発生させる
        //    //if(itemInfoDataset == null || itemInfoDataset.Tables.Count == 0 || itemInfoDataset.Tables[0].Rows.Count == 0)
        //    //{
        //    //    // SKUコード以外はクリア
        //    //    this.DetailAddTransferQuantity.Text = string.Empty;
        //    //    this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
        //    //    this.DetailAddReason.SelectedIndex = 0;
        //    //    // Exceptionを発生させる
        //    //    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
        //    //    return;
        //    //}
        //    //// 暫定修正 2011.11.28 End   商品が未存在の場合はExceptionを発生させる

        //    // 伝票検索より遷移した場合、[itemInfoDataset]はNULLになっているので必ずエラーとなる。
        //    // その為、updateMode=0(新規)の場合はチェックする。
        //    // 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
        //    //if (itemInfoDataset == null && updateMode.Equals("0"))
        //    //{
        //    //    // ※上記のif文を含めると「インスタンスエラー」となる為、分割してチェックする
        //    //    if (itemInfoDataset.Tables.Count == 0 || itemInfoDataset.Tables[0].Rows.Count == 0)
        //    //    {
        //    //        // SKUコード以外はクリア
        //    //        this.DetailAddTransferQuantity.Text = string.Empty;
        //    //        this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
        //    //        this.DetailAddReason.SelectedIndex = 0;
        //    //        // Exceptionを発生させる
        //    //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
        //    //        return;
        //    //    }
        //    //}
        //    #endregion
        //    ////   2012.01.13 関根 明細の数量が変更出来ない不具合対応（伝票検索から遷移時） 変更 終了

        //    // システムエラーが発生するので下記に変更
        //    if (updateMode.Equals("0") && (itemInfoDataset == null || itemInfoDataset.Tables.Count == 0 || itemInfoDataset.Tables[0].Rows.Count == 0))
        //    {
        //        // SKUコード以外はクリア
        //        this.DetailAddTransferQuantity.Text = string.Empty;
        //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //        this.DetailAddSlipOldPriceSalesChange.Text = string.Empty;
        //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //        this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
        //        // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //        this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
        //        // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //        this.DetailAddReason.SelectedIndex = 0;
        //        // Exceptionを発生させる
        //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
        //        // 2014.05.09 Start 効率改善指摘事項の修正
        //        this.DetailAddSKUCd.Focus();
        //        // 2014.05.09 End   効率改善指摘事項の修正
        //        return ret;
        //    }
        //    // 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする

        //    string skuName = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SkuName);

        //    if (updateMode.Equals("0"))
        //    {
        //        // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
        //        //SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow row = dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.NewSARSlipDetailErrorRow();
        //        SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow row = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.NewSARSlipDetailErrorRow();
        //        // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更

        //        // 法人コード
        //        row.CorporationCode = UserProfile.CorporationCode;

        //        // グリッド上のRowNumber
        //        row.SlipLineNo = (this.SlipDetailGridView.Rows.Count + 1).ToString();

        //        // 入力媒体区分
        //        row.InputMediaDiv = "1";

        //        // 入力バッチ番号
        //        row.InputBatchNo = this.BatchNumberTextBox.Text;

        //        // 入力ＳＥＱ番号
        //        //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
        //        //row.InputSequenceNo = (inputSequenceNo + 1).ToString();

        //        // 「入力バッチ番号・伝票番号」毎にインクリメントする。
        //        row.InputSequenceNo = inputSequenceNo.ToString();
        //        //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了

        //        // 入力ＳＥＱ行番号
        //        row.InputSequenceLineNo = this.SlipDetailGridView.Rows.Count + 1;

        //        // 伝票番号
        //        row.SlipNo = this.TransferSlipNumberTextBox.Text;

        //        // 取込処理日
        //        row.InputDate = systemDate;

        //        // 取込処理時刻
        //        row.InputTime = DateTime.Now.ToString("HHmmss");

        //        // データ種別
        //        row.DataType = string.Empty;

        //        // 取込ＳＥＱ番号
        //        // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
        //        //row.UptakeSequenceNo = decimal.Parse((inputSequenceNo + 1).ToString());
        //        row.UptakeSequenceNo = decimal.Parse((inputSequenceNo).ToString());
        //        // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応

        //        // 取込ＳＥＱ行番号
        //        row.UptakeSequenceLineNo = this.SlipDetailGridView.Rows.Count + 1;

        //        // 2012.06.26 Start 店舗コードをセットする内容の間違え
        //        //// バッチ店舗（伝票ヘッダー情報）
        //        //row.StoreCd = this.BatchStoreLabel.Text;
        //        // バッチ店舗（カバー情報）
        //        row.StoreCd = this.StoreTextBox.Text;
        //        // 2012.06.26 End   店舗コードをセットする内容の間違え

        //        // 伝票種別（カバー情報）
        //        row.SlipTypeCd = slipTypeValue;

        //        // 伝票区分（カバー情報）
        //        row.SlipDiv = this.SlipDivisionCoverDropDownList.SelectedValue;

        //        // グリッド上のSKUコード
        //        row.SKUCd = this.DetailAddSKUCd.Text;

        //        row.Attribute01 = skuName;

        //        // グリッド上の数量
        //        row.DisposalChangeQty = decimal.Parse(this.DetailAddTransferQuantity.Text);

        //        // グリッド上の旧売単価　伝票種別＝廃棄の場合は０
        //        // 2012.06.21 Start 売変選択時に画面の旧売価が０円で表示されているので修正
        //        //row.SlipOldPrice = 0;
        //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //        {
        //            // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //            //// 売変の場合はセッションの商品マスタより取得した売価情報を取得し旧売価とする
        //            //row.SlipOldPrice =
        //            //    string.IsNullOrEmpty((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice)) ? 0 : decimal.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice));
        //            row.SlipOldPrice =
        //                string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text) ? 0 : decimal.Parse(this.DetailAddSlipOldPriceSalesChange.Text);
        //            // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

        //        }
        //        else
        //        {   // 廃棄は０をセット
        //            row.SlipOldPrice = 0;
        //        }
        //        // 2012.06.21 End   売変選択時に画面の旧売価が０円で表示されているので修正

        //        // SKN臨時対応 売価取得 -->
        //        //// グリッド上の新売単価　伝票種別＝廃棄の場合は０
        //        //row.SlipNewPrice =
        //        //    string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? 0 : decimal.Parse(this.DetailAddSlipNewPriceSalesChange.Text);
        //        //if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
        //        //{
        //        //    row.SlipNewPrice = 0;
        //        //}

        //        // グリッド上の新売単価　伝票種別＝廃棄の場合は０
        //        row.SlipNewPrice =
        //            string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? 0 : decimal.Parse(this.DetailAddSlipNewPriceSalesChange.Text);
        //        // SKN臨時対応 売価取得 <--

        //        // グリッド上の売単価　伝票種別＝売変の場合は０
        //        row.SlipDisposalUnitPrica =
        //            string.IsNullOrEmpty(this.DetailAddSlipNewPriceDisposal.Text) ? 0 : decimal.Parse(this.DetailAddSlipNewPriceDisposal.Text);

        //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //        {
        //            row.SlipDisposalUnitPrica = 0;
        //        }

        //        // グリッド上の事由
        //        row.Reason = this.DetailAddReason.Items[this.DetailAddReason.SelectedIndex].Text;

        //        // グリッド上の（数量×新売単価）
        //        row.SalesChangePrice = row.DisposalChangeQty * row.SlipNewPrice;

        //        // グリッド上の（数量×売単価）
        //        row.DisposalPriceAmt = row.DisposalChangeQty * row.SlipDisposalUnitPrica;

        //        // 商品情報Ds.Category1Cd
        //        row.Category1Cd = this.Category1Cd.Text;

        //        // 商品情報Ds.Category2Cd
        //        row.Category2Cd = this.Category2Cd.Text;

        //        // 商品情報Ds.Category3Cd
        //        row.Category3Cd = string.Empty;

        //        // 商品情報Ds.Category4Cd
        //        row.Category4Cd = string.Empty;

        //        // 商品情報Ds.ScanningCd
        //        row.ScanningCd = string.Empty;

        //        if (itemInfoDataset != null && itemInfoDataset.ItemInfo.Rows.Count > 0)
        //        {
        //            var item = (ItemInfoDataset.ItemInfoRow)itemInfoDataset.ItemInfo.Rows[0];

        //            //   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応 追加 開始
        //            // 商品情報Ds.Category1Cd
        //            row.Category1Cd = item.Category1Cd;

        //            // 商品情報Ds.Category2Cd
        //            row.Category2Cd = item.Category2Cd;
        //            //   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応 追加 終了

        //            // 商品情報Ds.Category3Cd
        //            row.Category3Cd = item.Category3Cd;

        //            // 商品情報Ds.Category4Cd
        //            row.Category4Cd = item.Category4Cd;

        //            // 商品情報Ds.ScanningCd
        //            row.ScanningCd = item.ScanningCd;
        //        }

        //        // 伝票区分（カバー情報）
        //        row.SalesChangeDiv = this.DetailAddReason.SelectedValue;

        //        // エラー区分
        //        row.ErrorDiv = string.Empty;

        //        // 論理削除フラグ
        //        row.LogicalDelFlg = "0";

        //        // 売変・廃棄日
        //        row.DisposalChangeDate = Convert.ToDateTime(this.SalesChangeDate.Text);

        //        // 8/21 SUEYOSHI 売変伝票の場合、新売単価をチェック
        //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //        {
        //            if (this.SlipDivisionCoverDropDownList.SelectedValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice))
        //            {
        //                // 値下伝票で新単価が値上げされていた場合業務エラー
        //                // 2014.05.09 Start 効率改善指摘事項の修正
        //                // 同じ単価も警告とする
        //                //if (row.SlipNewPrice > row.SlipOldPrice)
        //                if (row.SlipNewPrice >= row.SlipOldPrice)
        //                // 2014.05.09 End   効率改善指摘事項の修正
        //                {
        //                    // Exceptionを発生させる
        //                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB248, "新売単価", "旧売単価")));
        //                    // 2014.05.09 Start 効率改善指摘事項の修正
        //                    this.DetailAddSlipNewPriceSalesChange.Focus();
        //                    // 2014.05.09 End   効率改善指摘事項の修正
        //                    return ret;
        //                }
        //            }
        //            else
        //            {
        //                // 値上伝票で新単価が旧単価より値下げされていた場合業務エラー
        //                // 2014.05.09 Start 効率改善指摘事項の修正
        //                // 同じ単価も警告とする
        //                //if (row.SlipNewPrice < row.SlipOldPrice)
        //                if (row.SlipNewPrice <= row.SlipOldPrice)
        //                // 2014.05.09 End   効率改善指摘事項の修正
        //                {
        //                    // Exceptionを発生させる
        //                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB249, "新売単価", "旧売単価")));
        //                    // 2014.05.09 Start 効率改善指摘事項の修正
        //                    this.DetailAddSlipNewPriceSalesChange.Focus();
        //                    // 2014.05.09 End   効率改善指摘事項の修正
        //                    return ret;
        //                }
        //            }
        //        }
        //        // 8/21 SUEYOSHI END

        //        // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
        //        //dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AddSARSlipDetailErrorRow(row);

        //        //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, dsTmpEntity);

        //        dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AddSARSlipDetailErrorRow(row);

        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
        //        // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
        //    }
        //    else
        //    {
        //        // 2013.08.21 Start tmpからグリッドの情報を取得するので、tmpに画面項目のセッションに反映する
        //        this.SaveDisplayControlTmp(true);
        //        // 2013.08.21 End tmpからグリッドの情報を取得するので、tmpに画面項目のセッションに反映する
        //        // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
        //        DsEntity dsTmpEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity);
        //        // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更

        //        int rowIndex = int.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.RowIndex));
        //        var detailRow =
        //            (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow)dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[rowIndex];

        //        detailRow.SKUCd = this.DetailAddSKUCd.Text;
        //        detailRow.Attribute01 = skuName;
        //        detailRow.DisposalChangeQty = decimal.Parse(this.DetailAddTransferQuantity.Text);
        //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //        {
        //            detailRow.SlipOldPrice = decimal.Parse(this.DetailAddSlipOldPriceSalesChange.Text);
        //        }
        //        else
        //        {   // 廃棄は０をセット
        //            detailRow.SlipOldPrice = 0;
        //        }
        //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //        detailRow.SlipNewPrice = decimal.Parse(this.DetailAddSlipNewPriceSalesChange.Text);

        //        // 8/21 SUEYOSHI 売変伝票の場合、新売単価をチェック
        //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //        {
        //            if (this.SlipDivisionCoverDropDownList.SelectedValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice))
        //            {
        //                // 値下伝票で新単価が値上げされていた場合業務エラー
        //                // 2014.05.09 Start 効率改善指摘事項の修正
        //                // 同じ単価も警告とする
        //                //if (detailRow.SlipNewPrice > detailRow.SlipOldPrice)
        //                if (detailRow.SlipNewPrice >= detailRow.SlipOldPrice)
        //                // 2014.05.09 End   効率改善指摘事項の修正
        //                {
        //                    // Exceptionを発生させる
        //                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB248, "新売単価", "旧売単価")));
        //                    // 2014.05.09 Start 効率改善指摘事項の修正
        //                    this.DetailAddSlipNewPriceSalesChange.Focus();
        //                    // 2014.05.09 End   効率改善指摘事項の修正
        //                    return ret;
        //                }
        //            }
        //            else
        //            {
        //                // 値上伝票で新単価が旧単価より値下げされていた場合業務エラー
        //                // 2014.05.09 Start 効率改善指摘事項の修正
        //                // 同じ単価も警告とする
        //                //if (detailRow.SlipNewPrice < detailRow.SlipOldPrice)
        //                if (detailRow.SlipNewPrice <= detailRow.SlipOldPrice)
        //                // 2014.05.09 End   効率改善指摘事項の修正
        //                {
        //                    // Exceptionを発生させる
        //                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB249, "新売単価", "旧売単価")));
        //                    // 2014.05.09 Start 効率改善指摘事項の修正
        //                    this.DetailAddSlipNewPriceSalesChange.Focus();
        //                    // 2014.05.09 End   効率改善指摘事項の修正
        //                    return ret;
        //                }
        //            }
        //        }
        //        // 8/21 SUEYOSHI END

        //        if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
        //        {
        //            detailRow.SlipDisposalUnitPrica = decimal.Parse(this.DetailAddSlipNewPriceDisposal.Text);
        //        }
        //        // 2012.06.26 Start 選択し内容を変更した際にSEQNoが元に戻るので修正
        //        // 訂正時はdsTmpEntityよりSEQNoを取得する。※１明細中１明細を選択した場合にdsEntityの明細は空になってしまうのでSEQNoが0から始まってしまう
        //        if (dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
        //        {
        //            inputSequenceNo = dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
        //        }
        //        // 2012.06.26 End   選択し内容を変更した際にSEQNoが元に戻るので修正
        //        //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
        //        // 入力ＳＥＱ番号
        //        // 「入力バッチ番号・伝票番号」毎にインクリメントする。
        //        detailRow.InputSequenceNo = inputSequenceNo.ToString();
        //        //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了


        //        //   2012.01.16 関根 数量変更時の合計金額が計算されない不具合対応 開始
        //        // グリッド上の（数量×新売単価）
        //        detailRow.SalesChangePrice = detailRow.DisposalChangeQty * detailRow.SlipNewPrice;

        //        // グリッド上の（数量×売単価）
        //        detailRow.DisposalPriceAmt = detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica;
        //        //   2012.01.16 関根 数量変更時の合計金額が計算されない不具合対応 終了


        //        detailRow.Reason = this.DetailAddReason.SelectedItem.Text;

        //        // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
        //        //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, dsTmpEntity);
        //        dsTmpEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);
        //        dsTmpEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Merge(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError);
        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsTmpEntity);
        //        dsEntity = dsTmpEntity;
        //        // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更

        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode, "0");
        //    }

        //    #region DEL
        //    //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 削除 開始
        //    // 上部で既に定義した為、削除
        //    //DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
        //    //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 削除 終了
        //    // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
        //    //dsTmpEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable);
        //    //dsTmpEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Merge(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError);
        //    //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsTmpEntity);
        //    // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更

        //    // 2012.06.26 Start 入力エリアのクリア箇所を変更
        //    //this.DetailAddSKUCd.Text = string.Empty;
        //    //this.DetailAddTransferQuantity.Text = string.Empty;
        //    //this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
        //    //this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
        //    //this.DetailAddReason.SelectedIndex = 0;
        //    #endregion

        //    this.ClearInputArea();
        //    // 2012.06.26 End   入力エリアのクリア箇所を変更

        //    this.SetSession(SalesChangeDisposalSlipInputSessionKey.SkuName, string.Empty);

        //    // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
        //    //this.TargetDataSet = dsTmpEntity.SARSlipDetailErrorDataSet;
        //    this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
        //    // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
        //    this.DataBindToSlipDetailGridView();

        //    this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
        //    this.ConvertToCurencyFormat();
        //    this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice();

        //    // Set show/hide by slip type selected value
        //    this.SetHiddenGridViewBySlipType(this.SlipTypeCoverDropDownList.SelectedValue);


        //    ret = true;
        //    return ret;
        //}
        #endregion
        //20151015 end  明細行直接編集により不要　コメントアウト　

        #region DetailAddButton_Click (新規追加ボタン)
        /// <summary>
        /// 新規追加ボタン
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void DetailAddButton_Click(object sender, EventArgs e)
        {
            // --> 2014/06/24 保存時にチェックされていない為、共通処理として移動
            #region Move ⇒ DetailAdd()
//            // 2014.03.18 Start 効率改善対応（Validatorコメント）
//            // 2014.04.30 Start 行番号が消える現象対応
//            // ※Validatorエラーとなった場合、
//            //   行番号が既に消えておりどの時点で消えているのか分からない為、
//            //   チェック前に再度行番号を振り直す。
//            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
//            // 2014.04.30 End   行番号が消える現象対応

//            // --> 2014/06/23 伝票明細数制限
//            string corporationCode = UserProfile.CorporationCode;

//            string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;

//            if (!string.IsNullOrEmpty(this.DetailAddSKUCd.Text)
//                && this.DetailAddSKUCd.Enabled == true)
//            {
//                // 新規追加のとき

//                // 最大明細数をチェック
//                // 売変と廃棄で設定が異なる為、判定して取得
//                string valueMaxDetailCount = (!string.IsNullOrEmpty(slipTypeValue) && slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange)) ? APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SALES_CHANGE_SLIP_MAX_DETAIL_COUNT, corporationCode) : APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_SLIP_MAX_DETAIL_COUNT, corporationCode);
//                int maxDetailCount;
//                if (int.TryParse(valueMaxDetailCount, out maxDetailCount) == false)
//                {
//                    maxDetailCount = 99;
//                }
//                if (this.SlipDetailGridView.Rows.Count >= maxDetailCount)
//                {
//                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB242, maxDetailCount.ToString())));
//                    this.DetailAddSKUCd.Focus();
//                    return;
//                }
//            }
//            // --< 2014/06/23 伝票明細数制限

//            // 伝票ヘッダー情報チェック
//            if (CheckSlipHeaderInfoInput().Equals(true))
//            {
//                return;
//            }

//            // 伝票明細グリッドチェック
//            if (CheckSlipDeatilGridInput().Equals(true))
//            {
//                return;
//            }

//            // 伝票明細情報チェック
//            if (CheckSlipDeatilInfoInput().Equals(true))
//            {
//                return;
//            }
//            // 2014.03.18 End   効率改善対応（Validatorコメント）

//            // --> 2014/06/23 伝票明細数制限
//            //string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;
//            // --< 2014/06/23 伝票明細数制限
//            // 2013.08.21 Start 数量のグリッド保管
//            this.SaveDisplayControl();
//            // 2013.08.21 End 数量のグリッド保管

//            // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
//            //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
//            //// 商品コードが未入力の場合は部門代表としてダミーコードをセットする
//            //if (string.IsNullOrEmpty(this.DetailAddSKUCd.Text))
//            //{
//            //    string category1DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode);
//            //    string category2DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode);
//            //    if (string.IsNullOrEmpty(category1DummyCd) && string.IsNullOrEmpty(category2DummyCd))
//            //    {
//            //        // ダミーコードが取得できていない場合はダミーコードを取得する
//            //        this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
//            //        // ダミーコードの入れ直し
//            //        category1DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode);
//            //        category2DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode);
//            //    }

//            //    // 分類２がセットされていない場合は分類１のダミーコード。以外は分類２のダミーコードをセット。
//            //    if (string.IsNullOrEmpty(this.Category2Cd.Text))
//            //    {
//            //        this.DetailAddSKUCd.Text = category1DummyCd;
//            //    }
//            //    else
//            //    {
//            //        this.DetailAddSKUCd.Text = category2DummyCd;
//            //    }
//            //}
//            //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
//            // 2014.05.28 End   部門代表コードの部分を機能しないように修正

//            // 商品コードの必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
//            if (string.IsNullOrEmpty(this.DetailAddSKUCd.Text))
//            {
//                // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
//                //this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "SKUコード")));
//                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "商品コード")));
//                // 2014.05.28 End   部門代表コードの部分を機能しないように修正
//                // 2014.05.09 Start 効率改善指摘事項の修正
//                this.DetailAddSKUCd.Focus();
//                // 2014.05.09 End   効率改善指摘事項の修正
//                return;
//            }

//            // 数量の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
//            if (string.IsNullOrEmpty(this.DetailAddTransferQuantity.Text))
//            {
//                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "数量")));
//                // 2014.05.09 Start 効率改善指摘事項の修正
//                this.DetailAddTransferQuantity.Focus();
//                // 2014.05.09 End   効率改善指摘事項の修正
//                return;
//            }

//            if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
//            {
//                // 新売単価の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
//                if (string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text))
//                {
//                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "新売単価")));
//                    // 2014.05.09 Start 効率改善指摘事項の修正
//                    this.DetailAddSlipNewPriceSalesChange.Focus();
//                    // 2014.05.09 End   効率改善指摘事項の修正
//                    return;
//                }
//            }

//            // 事由の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
//            if (string.IsNullOrEmpty(this.DetailAddReason.SelectedItem.Text))
//            {
//                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "事由")));
//                // 2014.05.09 Start 効率改善指摘事項の修正
//                this.DetailAddReason.Focus();
//                // 2014.05.09 End   効率改善指摘事項の修正
//                return;
//            }

//            // dsTmpEntity dsTmpEntity = this.dsTmpEntitySession; // (dsTmpEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.dsTmpEntity);
//            // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
//            //DsEntity dsTmpEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity);
//            // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
//            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
//            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
//            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 終了

//            // 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
//            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);

//            workEntity.ItemCd = this.DetailAddSKUCd.Text;
//            workEntity.ChangeDate = this.SalesChangeDate.DateValue;
//            workEntity.StoreCd = this.StoreTextBox.Text;
//            workEntity.IVGetSystemDiv = "IV";
//            workEntity.IVGetDiv = "1";
//            workEntity.Subsystem = "SA";
//            workEntity.Category1Cd = this.Category1Cd.Text;
//            workEntity.BatchDate = Convert.ToDateTime(this.BatchDateDateTextBox.Text);
//            workEntity.ProductInfo_SalePrice = string.Empty;

//            using (ItemInfoDataset itemInforDataSet = new ItemInfoDataset())
//            {
//                // 商品情報の取得
//                this.GetFacadeInstance().FillItemInfo(workEntity, itemInforDataSet, dsEntity);

//                if (itemInforDataSet.Tables[0].Rows.Count > 0)
//                {
//                    // 2014.05.09 Start 効率改善指摘事項の修正
//                    if (itemInforDataSet.ItemInfo[0].Category1Cd != this.Category1Cd.Text)
//                    {
//                        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARBIN03)));
//                        this.DetailAddSKUCd.Focus();
//                        return;
//                    }
//                    // 2014.05.09 End   効率改善指摘事項の修正

//                    this.SetSession(SalesChangeDisposalSlipInputSessionKey.SkuName, itemInforDataSet.ItemInfo[0].ProductsKanji);
//                }
//                else
//                {
//                    // DataSetを空にする
//                    if (itemInforDataSet != null && itemInforDataSet.Tables.Count != 0 && itemInforDataSet.Tables[0].Rows.Count != 0)
//                    {
//                        itemInforDataSet.Tables[0].Clear();
//                    }
//                    this.SetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset, itemInforDataSet);
//                    // Exceptionを発生させる
//                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
//                    // 2014.05.09 Start 効率改善指摘事項の修正
//                    this.DetailAddSKUCd.Focus();
//                    // 2014.05.09 End   効率改善指摘事項の修正
//                    return;
//                }

//                // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
//                // 画面上に旧売価項目を追加するためSessionへ入れる必要がなくなったのでコメントアウト
//                //// 取得した売単価を保持する(マスタ売価なので保持しておく)(売変時に旧売価として使用するため)
//                //this.SetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice, workEntity.ProductInfo_SalePrice);
//                // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

//                // 入力されていない場合にのみセットする
//                if (string.IsNullOrEmpty(this.DetailAddTransferQuantity.Text))
//                {
//                    // 2014.06.14 Start 品質改善検証の指摘事項対応
//                    //this.DetailAddTransferQuantity.Text = workEntity.ProductInfo_LogicalStockQty;
//                    // 数量には空白をデフォルトとする。
//                    this.DetailAddTransferQuantity.Text = string.Empty;
//                    // 2014.06.14 End   品質改善検証の指摘事項対応
//                }
//                DetailAddSKUCd_TextChanged_ProductInfo_LogicalStockQty = this.DetailAddTransferQuantity.Text;

//                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
//                {
//                    // 売変
//                    // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
//                    if (string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text))
//                    {
//                        this.DetailAddSlipOldPriceSalesChange.Text = workEntity.ProductInfo_SalePrice;
//                    }
//                    DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_OldSalePrice = this.DetailAddSlipOldPriceSalesChange.Text;
//                    // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
//                    if (string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text))
//                    {
//                        this.DetailAddSlipNewPriceSalesChange.Text = workEntity.ProductInfo_SalePrice;
//                    }
//                    DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice = this.DetailAddSlipNewPriceSalesChange.Text;
//                }
//                else
//                {
//                    // 廃棄
//                    if (string.IsNullOrEmpty(this.DetailAddSlipNewPriceDisposal.Text))
//                    {
//                        this.DetailAddSlipNewPriceDisposal.Text = workEntity.ProductInfo_SalePrice;
//                    }
//                    DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice = this.DetailAddSlipNewPriceDisposal.Text;
//                }

//                this.SetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset, itemInforDataSet);
//                this.SetSession("SKUCd", this.DetailAddSKUCd.Text);
//            }

//            // 廃棄の場合の処理
//            if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
//            {
//                // ※廃棄の場合は後処理で金額などを求めている箇所で DetailAddSlipNewPriceSalesChange の項目を使っているので同様の値を入れておく
//                this.DetailAddSlipNewPriceSalesChange.Text = this.DetailAddSlipNewPriceDisposal.Text;
//            }
//            // 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする

//            int inputSequenceNo = 0;
//            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
//            //if (dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
//            //{
//            //    inputSequenceNo = dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
//            //}

//            // dsTmpEntityを使用すると、連続で訂正した時に最新のInputSequenceNoが取得出来ないため、dsEntityより取得する。
//            if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
//            {
//                inputSequenceNo = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
//            }
//            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了

//            DateTime systemDate = (DateTime)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SystemDate);

//            string updateMode = "0";
//            if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode) != null)
//            {
//                updateMode = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode);
//            }

//            ItemInfoDataset itemInfoDataset = (ItemInfoDataset)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset);
//            //   2012.01.13 関根 明細の数量が変更出来ない不具合対応（伝票検索から遷移時） 変更 開始
//            //// 暫定修正 2011.11.28 Start 商品が未存在の場合はExceptionを発生させる
//            //if(itemInfoDataset == null || itemInfoDataset.Tables.Count == 0 || itemInfoDataset.Tables[0].Rows.Count == 0)
//            //{
//            //    // SKUコード以外はクリア
//            //    this.DetailAddTransferQuantity.Text = string.Empty;
//            //    this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
//            //    this.DetailAddReason.SelectedIndex = 0;
//            //    // Exceptionを発生させる
//            //    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
//            //    return;
//            //}
//            //// 暫定修正 2011.11.28 End   商品が未存在の場合はExceptionを発生させる

//            // 伝票検索より遷移した場合、[itemInfoDataset]はNULLになっているので必ずエラーとなる。
//            // その為、updateMode=0(新規)の場合はチェックする。
//            // 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
//            //if (itemInfoDataset == null && updateMode.Equals("0"))
//            //{
//            //    // ※上記のif文を含めると「インスタンスエラー」となる為、分割してチェックする
//            //    if (itemInfoDataset.Tables.Count == 0 || itemInfoDataset.Tables[0].Rows.Count == 0)
//            //    {
//            //        // SKUコード以外はクリア
//            //        this.DetailAddTransferQuantity.Text = string.Empty;
//            //        this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
//            //        this.DetailAddReason.SelectedIndex = 0;
//            //        // Exceptionを発生させる
//            //        this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
//            //        return;
//            //    }
//            //}
//            ////   2012.01.13 関根 明細の数量が変更出来ない不具合対応（伝票検索から遷移時） 変更 終了

//            // システムエラーが発生するので下記に変更
//            if (updateMode.Equals("0") && (itemInfoDataset == null || itemInfoDataset.Tables.Count == 0 || itemInfoDataset.Tables[0].Rows.Count == 0))
//            {
//                // SKUコード以外はクリア
//                this.DetailAddTransferQuantity.Text = string.Empty;
//                // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
//                this.DetailAddSlipOldPriceSalesChange.Text = string.Empty;
//                // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
//                this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
//                // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
//                this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
//                // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
//                this.DetailAddReason.SelectedIndex = 0;
//                // Exceptionを発生させる
//                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
//                // 2014.05.09 Start 効率改善指摘事項の修正
//                this.DetailAddSKUCd.Focus();
//                // 2014.05.09 End   効率改善指摘事項の修正
//                return;
//            }
//            // 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする

//            string skuName = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SkuName);

//            if (updateMode.Equals("0"))
//            {
//                // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
//                //SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow row = dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.NewSARSlipDetailErrorRow();
//                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow row = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.NewSARSlipDetailErrorRow();
//                // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更

//                // 法人コード
//                row.CorporationCode = UserProfile.CorporationCode;

//                // グリッド上のRowNumber
//                row.SlipLineNo = (this.SlipDetailGridView.Rows.Count + 1).ToString();

//                // 入力媒体区分
//                row.InputMediaDiv = "1";

//                // 入力バッチ番号
//                row.InputBatchNo = this.BatchNumberTextBox.Text;

//                // 入力ＳＥＱ番号
//                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
//                //row.InputSequenceNo = (inputSequenceNo + 1).ToString();

//                // 「入力バッチ番号・伝票番号」毎にインクリメントする。
//                row.InputSequenceNo = inputSequenceNo.ToString();
//                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了

//                // 入力ＳＥＱ行番号
//                row.InputSequenceLineNo = this.SlipDetailGridView.Rows.Count + 1;

//                // 伝票番号
//                row.SlipNo = this.TransferSlipNumberTextBox.Text;

//                // 取込処理日
//                row.InputDate = systemDate;

//                // 取込処理時刻
//                row.InputTime = DateTime.Now.ToString("HHmmss");

//                // データ種別
//                row.DataType = string.Empty;

//                // 取込ＳＥＱ番号
//                // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
//                //row.UptakeSequenceNo = decimal.Parse((inputSequenceNo + 1).ToString());
//                row.UptakeSequenceNo = decimal.Parse((inputSequenceNo).ToString());
//                // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応

//                // 取込ＳＥＱ行番号
//                row.UptakeSequenceLineNo = this.SlipDetailGridView.Rows.Count + 1;

//                // 2012.06.26 Start 店舗コードをセットする内容の間違え
//                //// バッチ店舗（伝票ヘッダー情報）
//                //row.StoreCd = this.BatchStoreLabel.Text;
//                // バッチ店舗（カバー情報）
//                row.StoreCd = this.StoreTextBox.Text;
//                // 2012.06.26 End   店舗コードをセットする内容の間違え

//                // 伝票種別（カバー情報）
//                row.SlipTypeCd = slipTypeValue;

//                // 伝票区分（カバー情報）
//                row.SlipDiv = this.SlipDivisionCoverDropDownList.SelectedValue;

//                // グリッド上のSKUコード
//                row.SKUCd = this.DetailAddSKUCd.Text;

//                row.Attribute01 = skuName;

//                // グリッド上の数量
//                row.DisposalChangeQty = decimal.Parse(this.DetailAddTransferQuantity.Text);

//                // グリッド上の旧売単価　伝票種別＝廃棄の場合は０
//                // 2012.06.21 Start 売変選択時に画面の旧売価が０円で表示されているので修正
//                //row.SlipOldPrice = 0;
//                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
//                {
//                    // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
//                    //// 売変の場合はセッションの商品マスタより取得した売価情報を取得し旧売価とする
//                    //row.SlipOldPrice =
//                    //    string.IsNullOrEmpty((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice)) ? 0 : decimal.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.ProductSalesPrice));
//                    row.SlipOldPrice =
//                        string.IsNullOrEmpty(this.DetailAddSlipOldPriceSalesChange.Text) ? 0 : decimal.Parse(this.DetailAddSlipOldPriceSalesChange.Text);
//                    // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

//                }
//                else
//                {   // 廃棄は０をセット
//                    row.SlipOldPrice = 0;
//                }
//                // 2012.06.21 End   売変選択時に画面の旧売価が０円で表示されているので修正

//                // SKN臨時対応 売価取得 -->
//                //// グリッド上の新売単価　伝票種別＝廃棄の場合は０
//                //row.SlipNewPrice =
//                //    string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? 0 : decimal.Parse(this.DetailAddSlipNewPriceSalesChange.Text);
//                //if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
//                //{
//                //    row.SlipNewPrice = 0;
//                //}

//                // グリッド上の新売単価　伝票種別＝廃棄の場合は０
//                row.SlipNewPrice =
//                    string.IsNullOrEmpty(this.DetailAddSlipNewPriceSalesChange.Text) ? 0 : decimal.Parse(this.DetailAddSlipNewPriceSalesChange.Text);
//                // SKN臨時対応 売価取得 <--

//                // グリッド上の売単価　伝票種別＝売変の場合は０
//                row.SlipDisposalUnitPrica =
//                    string.IsNullOrEmpty(this.DetailAddSlipNewPriceDisposal.Text) ? 0 : decimal.Parse(this.DetailAddSlipNewPriceDisposal.Text);

//                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
//                {
//                    row.SlipDisposalUnitPrica = 0;
//                }

//                // グリッド上の事由
//                row.Reason = this.DetailAddReason.Items[this.DetailAddReason.SelectedIndex].Text;

//                // グリッド上の（数量×新売単価）
//                row.SalesChangePrice = row.DisposalChangeQty * row.SlipNewPrice;

//                // グリッド上の（数量×売単価）
//                row.DisposalPriceAmt = row.DisposalChangeQty * row.SlipDisposalUnitPrica;

//                // 商品情報Ds.Category1Cd
//                row.Category1Cd = this.Category1Cd.Text;

//                // 商品情報Ds.Category2Cd
//                row.Category2Cd = this.Category2Cd.Text;

//                // 商品情報Ds.Category3Cd
//                row.Category3Cd = string.Empty;

//                // 商品情報Ds.Category4Cd
//                row.Category4Cd = string.Empty;

//                // 商品情報Ds.ScanningCd
//                row.ScanningCd = string.Empty;

//                if (itemInfoDataset != null && itemInfoDataset.ItemInfo.Rows.Count > 0)
//                {
//                    var item = (ItemInfoDataset.ItemInfoRow)itemInfoDataset.ItemInfo.Rows[0];

//                    //   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応 追加 開始
//                    // 商品情報Ds.Category1Cd
//                    row.Category1Cd = item.Category1Cd;

//                    // 商品情報Ds.Category2Cd
//                    row.Category2Cd = item.Category2Cd;
//                    //   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応 追加 終了

//                    // 商品情報Ds.Category3Cd
//                    row.Category3Cd = item.Category3Cd;

//                    // 商品情報Ds.Category4Cd
//                    row.Category4Cd = item.Category4Cd;

//                    // 商品情報Ds.ScanningCd
//                    row.ScanningCd = item.ScanningCd;
//                }

//                // 伝票区分（カバー情報）
//                row.SalesChangeDiv = this.DetailAddReason.SelectedValue;

//                // エラー区分
//                row.ErrorDiv = string.Empty;

//                // 論理削除フラグ
//                row.LogicalDelFlg = "0";
//#warning SKN臨時対応
//                // 売変・廃棄日
//                row.DisposalChangeDate = Convert.ToDateTime(this.SalesChangeDate.Text);

//                // 8/21 SUEYOSHI 売変伝票の場合、新売単価をチェック
//                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
//                {
//                    if (this.SlipDivisionCoverDropDownList.SelectedValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice))
//                    {
//                        // 値下伝票で新単価が値上げされていた場合業務エラー
//                        // 2014.05.09 Start 効率改善指摘事項の修正
//                        // 同じ単価も警告とする
//                        //if (row.SlipNewPrice > row.SlipOldPrice)
//                        if (row.SlipNewPrice >= row.SlipOldPrice)
//                        // 2014.05.09 End   効率改善指摘事項の修正
//                        {
//                            // Exceptionを発生させる
//                            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB248, "新売単価", "旧売単価")));
//                            // 2014.05.09 Start 効率改善指摘事項の修正
//                            this.DetailAddSlipNewPriceSalesChange.Focus();
//                            // 2014.05.09 End   効率改善指摘事項の修正
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        // 値上伝票で新単価が旧単価より値下げされていた場合業務エラー
//                        // 2014.05.09 Start 効率改善指摘事項の修正
//                        // 同じ単価も警告とする
//                        //if (row.SlipNewPrice < row.SlipOldPrice)
//                        if (row.SlipNewPrice <= row.SlipOldPrice)
//                        // 2014.05.09 End   効率改善指摘事項の修正
//                        {
//                            // Exceptionを発生させる
//                            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB249, "新売単価", "旧売単価")));
//                            // 2014.05.09 Start 効率改善指摘事項の修正
//                            this.DetailAddSlipNewPriceSalesChange.Focus();
//                            // 2014.05.09 End   効率改善指摘事項の修正
//                            return;
//                        }
//                    }
//                }
//                // 8/21 SUEYOSHI END

//                // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
//                //dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AddSARSlipDetailErrorRow(row);

//                //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, dsTmpEntity);

//                dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AddSARSlipDetailErrorRow(row);

//                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
//                // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
//            }
//            else
//            {
//                // 2013.08.21 Start tmpからグリッドの情報を取得するので、tmpに画面項目のセッションに反映する
//                this.SaveDisplayControlTmp(true);
//                // 2013.08.21 End tmpからグリッドの情報を取得するので、tmpに画面項目のセッションに反映する
//                // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
//                DsEntity dsTmpEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity);
//                // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更

//                int rowIndex = int.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.RowIndex));
//                var detailRow =
//                    (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow)dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[rowIndex];

//                detailRow.SKUCd = this.DetailAddSKUCd.Text;
//                detailRow.Attribute01 = skuName;
//                detailRow.DisposalChangeQty = decimal.Parse(this.DetailAddTransferQuantity.Text);
//                // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
//                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
//                {
//                    detailRow.SlipOldPrice = decimal.Parse(this.DetailAddSlipOldPriceSalesChange.Text);
//                }
//                else
//                {   // 廃棄は０をセット
//                    detailRow.SlipOldPrice = 0;
//                }
//                // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
//                detailRow.SlipNewPrice = decimal.Parse(this.DetailAddSlipNewPriceSalesChange.Text);

//                // 8/21 SUEYOSHI 売変伝票の場合、新売単価をチェック
//                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
//                {
//                    if (this.SlipDivisionCoverDropDownList.SelectedValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice))
//                    {
//                        // 値下伝票で新単価が値上げされていた場合業務エラー
//                        // 2014.05.09 Start 効率改善指摘事項の修正
//                        // 同じ単価も警告とする
//                        //if (detailRow.SlipNewPrice > detailRow.SlipOldPrice)
//                        if (detailRow.SlipNewPrice >= detailRow.SlipOldPrice)
//                        // 2014.05.09 End   効率改善指摘事項の修正
//                        {
//                            // Exceptionを発生させる
//                            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB248, "新売単価", "旧売単価")));
//                            // 2014.05.09 Start 効率改善指摘事項の修正
//                            this.DetailAddSlipNewPriceSalesChange.Focus();
//                            // 2014.05.09 End   効率改善指摘事項の修正
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        // 値上伝票で新単価が旧単価より値下げされていた場合業務エラー
//                        // 2014.05.09 Start 効率改善指摘事項の修正
//                        // 同じ単価も警告とする
//                        //if (detailRow.SlipNewPrice < detailRow.SlipOldPrice)
//                        if (detailRow.SlipNewPrice <= detailRow.SlipOldPrice)
//                        // 2014.05.09 End   効率改善指摘事項の修正
//                        {
//                            // Exceptionを発生させる
//                            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB249, "新売単価", "旧売単価")));
//                            // 2014.05.09 Start 効率改善指摘事項の修正
//                            this.DetailAddSlipNewPriceSalesChange.Focus();
//                            // 2014.05.09 End   効率改善指摘事項の修正
//                            return;
//                        }
//                    }
//                }
//                // 8/21 SUEYOSHI END

//                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
//                {
//                    detailRow.SlipDisposalUnitPrica = decimal.Parse(this.DetailAddSlipNewPriceDisposal.Text);
//                }
//                // 2012.06.26 Start 選択し内容を変更した際にSEQNoが元に戻るので修正
//                // 訂正時はdsTmpEntityよりSEQNoを取得する。※１明細中１明細を選択した場合にdsEntityの明細は空になってしまうのでSEQNoが0から始まってしまう
//                if (dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
//                {
//                    inputSequenceNo = dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
//                }
//                // 2012.06.26 End   選択し内容を変更した際にSEQNoが元に戻るので修正
//                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
//                // 入力ＳＥＱ番号
//                // 「入力バッチ番号・伝票番号」毎にインクリメントする。
//                detailRow.InputSequenceNo = inputSequenceNo.ToString();
//                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了


//                //   2012.01.16 関根 数量変更時の合計金額が計算されない不具合対応 開始
//                // グリッド上の（数量×新売単価）
//                detailRow.SalesChangePrice = detailRow.DisposalChangeQty * detailRow.SlipNewPrice;

//                // グリッド上の（数量×売単価）
//                detailRow.DisposalPriceAmt = detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica;
//                //   2012.01.16 関根 数量変更時の合計金額が計算されない不具合対応 終了


//                detailRow.Reason = this.DetailAddReason.SelectedItem.Text;

//                // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
//                //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, dsTmpEntity);
//                dsTmpEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);
//                dsTmpEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Merge(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError);
//                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsTmpEntity);
//                dsEntity = dsTmpEntity;
//                // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更

//                this.SetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode, "0");
//            }

//            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 削除 開始
//            // 上部で既に定義した為、削除
//            //DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
//            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 削除 終了
//            // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
//            //dsTmpEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable);
//            //dsTmpEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Merge(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError);
//            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsTmpEntity);
//            // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更

//            // 2012.06.26 Start 入力エリアのクリア箇所を変更
//            //this.DetailAddSKUCd.Text = string.Empty;
//            //this.DetailAddTransferQuantity.Text = string.Empty;
//            //this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
//            //this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
//            //this.DetailAddReason.SelectedIndex = 0;

//            this.ClearInputArea();
//            // 2012.06.26 End   入力エリアのクリア箇所を変更

//            this.SetSession(SalesChangeDisposalSlipInputSessionKey.SkuName, string.Empty);

//            // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
//            //this.TargetDataSet = dsTmpEntity.SARSlipDetailErrorDataSet;
//            this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
//            // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
            //this.DataBindToSlipDetailGridView();

            //this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            //this.ConvertToCurencyFormat();
            //this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice();

            //// Set show/hide by slip type selected value
            //this.SetHiddenGridViewBySlipType(this.SlipTypeCoverDropDownList.SelectedValue);

            #endregion

            // 商品追加（編集反映）処理
            //if (this.DetailAdd() == false)
            //{
            //    return;
            //}
            // <-- 2014/06/24 保存時にチェックされていない為、共通処理として移動

            //20151014 start 明細にて編集できるように仕様変更する　ri
            //最大明細行をチェックする
            if (this.CheckMaxDetailCount() == false)
            {
                this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);

                return;
            }
            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(false);
            
            this.SaveDropDownList();

            this.AddNewBlankRowToDataGrid();

            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);

            //20151014 end 明細にて編集できるように仕様変更する　ri

            // 2014.05.09 Start 効率改善指摘事項の修正
            //this.DetailAddSKUCd.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正
        }
        #endregion

        #region NextCoverButton_Click (次へ（新カバー）)
        /// <summary>
        /// 次へ（新カバー）
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void NextCoverButton_Click(object sender, EventArgs e)
        {
            this.BatchNumberTextBox.Text = string.Empty;
            // 2014.05.16 Start 伝票起票の入力枚数は使用しないので非表示とする
            //this.InputPageCountTextBox.Text = string.Empty;
            this.InputPageCountTextBox.Text = "1";
            // 2014.05.16 End   伝票起票の入力枚数は使用しないので非表示とする

            this.SetMainHeader();

            this.SetEnableControls(true);
            // 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
            this.SetHeaderEnableControls(false);
            // 2012.06.26 End   伝票ヘッダー情報の入力制御を修正

            this.TargetDataTable = new SARSlipHeaderErrorInfoDataset().SARSlipHeaderError;
            this.DataBindToinputStatusListGrid();

            this.ClearHeader();
            this.ClearDetail();
            // 2012.06.26 Start 入力エリアのクリア箇所を変更
            this.ClearInputArea();
            // 2012.06.26 End   入力エリアのクリア箇所を変更

            // 2014.06.14 Start 品質改善検証の指摘事項対応
            // 伝票種別ドロップダウンリスト
            if (this.SlipTypeCoverDropDownList.Items.Count <= 1)
            {
                this.SlipTypeCoverDropDownList.Enabled = false;
            }
            else
            {
                this.SlipTypeCoverDropDownList.Enabled = true;
            }

            // 伝票区分ドロップダウンリスト
            if (this.SlipDivisionCoverDropDownList.Items.Count <= 1)
            {
                this.SlipDivisionCoverDropDownList.Enabled = false;
            }
            else
            {
                this.SlipDivisionCoverDropDownList.Enabled = true;
            }

            if(this.DetailAddButton.Enabled == true){
                this.DetailAddButton.Enabled = false;
            }
            // 2014.06.14 End   品質改善検証の指摘事項対応

            // 2014.05.09 Start 効率改善指摘事項の修正
            this.BatchStoreLabel.Focus();
            // 2014.05.09 End   効率改善指摘事項の修正
        }
        #endregion

        #region ApprovalRequestButton_Click (承認依頼)
        /// <summary>
        /// 承認依頼
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void ApprovalRequestButton_Click(object sender, EventArgs e)
        {
            if (this.inputStatusListGrid.Rows.Count == 0)
            {
                // 情報メッセージを表示する。
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "承認依頼するデータ")));
                return;
            }

            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);

            // カバーシート情報の入力枚数=入力状況情報の最終レコード番号
            int maxRecord = this.inputStatusListGrid.Rows.Count;
            // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            //int pageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            int pageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 1 : int.Parse(this.InputPageCountTextBox.Text);
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            if (pageCount != maxRecord)
            {
                // 異なる場合、「カバーシート入力枚数と入力枚数が合っていません。」警告メッセージを表示。
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0133, "カバーシート入力枚数と入力枚数")));
                return;
            }

            // 2012.06.26 Start カバーシートの入力枚数を画面入力値へ変更
            CoverSheetMasterEntity coverSheetMasterEntity = (CoverSheetMasterEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity);

            CoverSheetMasterDataset.CoverSheetMasterDataTable1Row coverSheetRow = dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0];
            // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            //coverSheetRow.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            coverSheetRow.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 1 : int.Parse(this.InputPageCountTextBox.Text);
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            // 伝票情報用のDataSetへマージする
            DsEntity dsInputStatusEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity);
            dsInputStatusEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity, dsInputStatusEntity);

            // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            //coverSheetMasterEntity.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text);
            coverSheetMasterEntity.InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 1 : int.Parse(this.InputPageCountTextBox.Text);
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity, coverSheetMasterEntity);
            // 2012.06.26 End   カバーシートの入力枚数を画面入力値へ変更

            bool isDisplayOff = this.CheckSar(initialDisplayEntity, APVariableListKey.SAR_CheckDisplay_Off);

            if (isDisplayOff)
            {
                workEntity.ButtonId = "2";

                // 生成したDsEntityとWorkEntityをセッションへ登録し確認画面へ遷移する
                this.Redirect("SCRCSAR050208", null);
            }
            else
            {
                // ４．メッセージ：【MSGCSARI0020】表示後、承認依頼ファサードを呼び出す。
                // 【BSFCSAR050800027】
                this.GetFacadeInstance().ApprovalSARCoverSheetMasterNew(workEntity, dsEntity, initialDisplayEntity);

                // 2014.01.06 Start 伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。
                //// 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
                //this.SetHeaderEnableControls(true);
                //// 2012.06.26 End   伝票ヘッダー情報の入力制御を修正
                if (workEntity.InitMode == "1" || workEntity.InitMode == "2")
                {
                    this.SetHeaderEnableControls(false);
                }
                else
                {
                    this.SetHeaderEnableControls(true);
                }
                // 2014.01.06 End   伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。

                this.ClearCoverSheet();
                this.ClearHeader();
                this.ClearDetail();
                // 2012.06.26 Start 入力エリアのクリア箇所を変更
                this.ClearInputArea();
                // 2012.06.26 End   入力エリアのクリア箇所を変更
            }

            // ６．メッセージの表示MSGCSARI0028を表示する。{0}は「承認依頼」
            this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCSARI0028, "承認依頼")));
        }
        #endregion

        #region SaveCoverButton_Click (カバー保存)
        /// <summary>
        /// カバー保存
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void SaveCoverButton_Click(object sender, EventArgs e)
        {
            int initMode = 0;
            if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitMode) != null)
            {
                initMode = (int)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitMode);
            }

            // 2014.05.16 Start 伝票起票の入力枚数は使用しないので非表示とする
            if (string.IsNullOrEmpty(this.InputPageCountTextBox.Text))
            {
                // 入力枚数は非表示なので空の場合は強制的に0をセットする
                this.InputPageCountTextBox.Text = "1";
            }
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする

            if (initMode != 1 || initMode != 2)
            {
                if (string.IsNullOrEmpty(this.BatchStoreLabel.Text.Trim()) ||
                    string.IsNullOrEmpty(this.SlipTypeCoverDropDownList.SelectedItem.Text) ||
                    string.IsNullOrEmpty(this.SlipDivisionCoverDropDownList.SelectedItem.Text) ||
                    string.IsNullOrEmpty(this.BatchNumberTextBox.Text.Trim()) ||
                    string.IsNullOrEmpty(this.InputPageCountTextBox.Text.Trim()) ||
                    this.BatchDateDateTextBox.DateValue == null)
                {
                    string[] placeHolder = { "バッチ店舗", "伝票種別", "伝票区分", "バッチ番号", "入力枚数", "バッチ日付" };
                    int index = 0;

                    if (string.IsNullOrEmpty(this.BatchStoreLabel.Text.Trim()))
                    {
                        index = 0;
                        // 2014.05.09 Start 効率改善指摘事項の修正
                        this.BatchStoreLabel.Focus();
                        // 2014.05.09 End   効率改善指摘事項の修正
                    }
                    else if (string.IsNullOrEmpty(this.SlipTypeCoverDropDownList.SelectedItem.Text))
                    {
                        index = 1;
                        // 2014.05.09 Start 効率改善指摘事項の修正
                        this.SlipTypeCoverDropDownList.Focus();
                        // 2014.05.09 End   効率改善指摘事項の修正
                    }
                    else if (string.IsNullOrEmpty(this.SlipDivisionCoverDropDownList.SelectedItem.Text))
                    {
                        index = 2;
                        // 2014.05.09 Start 効率改善指摘事項の修正
                        this.SlipDivisionCoverDropDownList.Focus();
                        // 2014.05.09 End   効率改善指摘事項の修正
                    }
                    else if (string.IsNullOrEmpty(this.BatchNumberTextBox.Text))
                    {
                        index = 3;
                        // 2014.05.09 Start 効率改善指摘事項の修正
                        this.BatchNumberTextBox.Focus();
                        // 2014.05.09 End   効率改善指摘事項の修正
                    }
                    else if (string.IsNullOrEmpty(this.InputPageCountTextBox.Text))
                    {
                        index = 4;
                        // 2014.05.09 Start 効率改善指摘事項の修正
                        this.InputPageCountTextBox.Focus();
                        // 2014.05.09 End   効率改善指摘事項の修正
                    }
                    else if (this.BatchDateDateTextBox.DateValue == null)
                    {
                        index = 5;
                        // 2014.05.09 Start 効率改善指摘事項の修正
                        this.BatchDateDateTextBox.Focus();
                        // 2014.05.09 End   効率改善指摘事項の修正
                    }

                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, placeHolder[index].ToString())));
                    return;
                }
            }

            // 2014.03.18 Start 効率改善対応（Validatorコメント）
            // 2014.04.30 Start 行番号が消える現象対応
            // ※Validatorエラーとなった場合、
            //   行番号が既に消えておりどの時点で消えているのか分からない為、
            //   チェック前に再度行番号を振り直す。
            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            // 2014.04.30 End   行番号が消える現象対応

            if (CheckCoverSheetInput().Equals(true))
            {
                return;
            }
            // 2014.03.18 End   効率改善対応（Validatorコメント）

            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);

            if (this.BatchDateDateTextBox.DateValue < workEntity.IVPeriodDate)
            {
                // バッチ日付＜WorkEntity.在庫計上日
                // エラーの場合、エラーメッセージ【ERRCSARB0209】をスローする
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0209)));
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.BatchDateDateTextBox.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
                return;
            }

            // 8/9 SUEYOSHI 新規、検索からでも複数伝票登録できるように修正 UPD
            // if (initMode == 0)
            if (initMode == 0 || initMode == 2)
            // 8/9 SUEYOSHI END
            {
                DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
                // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
                DsEntity dsInputStatusEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity);
                // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

                dsEntity.CoverSheetMasterDataset = new CoverSheetMasterDataset();

                CoverSheetMasterDataset.CoverSheetMasterDataTable1Row row =
                        dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.NewCoverSheetMasterDataTable1Row();
                row.CorporationCode = UserProfile.CorporationCode;
                // 2013.08.28 Start データテーブルの項目をDBに合わせる
                row.BatchDate = DateTime.Parse(this.BatchDateDateTextBox.Text);
                row.BatchNo = this.BatchNumberTextBox.Text;
                // 2013.08.28 Start データテーブルの項目をDBに合わせる
                row.InputPageCount = int.Parse(this.InputPageCountTextBox.Text);
                row.StoreCd = this.BatchStoreLabel.Text;
                row.SlipTypeCd = this.SlipTypeCoverDropDownList.SelectedValue;
                row.SlipDiv = this.SlipDivisionCoverDropDownList.SelectedValue;
                row.TotalPageQty = 0;
                row.ErrorNo = 0;
                row.LastUpdateDate = DateTime.Now;
                row.ApprovalCount = 0;
                // 2013/09/24 いなげやアドオン対応：承認しないモード対応 ISC:yshindo start
                InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
                if (SalesChangeDisposalSlipInputConstances.Disposal.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
                {
                    // 現在入力中の伝票種別が廃棄伝票の場合
                    workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDiv);

                    //start  20220609 orizon.ri 標準不具合051,052 自動承認しないように（伝票作成する時点で大福帳作成しない）
                    // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo start
                    // 実行モード←"100"：計上処理モード
                    //workEntity.ExecuteMode = "100";
                    workEntity.ExecuteMode = "1";
                    // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo end
                    //end  20220609 orizon.ri 標準不具合051,052 自動承認しないように（伝票作成する時点で大福帳作成しない）
                }
                else if (SalesChangeDisposalSlipInputConstances.SalesChange.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
                {
                    // 現在入力中の伝票種別が売変伝票の場合
                    workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDivForSalesChange);

                    // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo start
                    // 実行モード←"1"
                    workEntity.ExecuteMode = "1";
                    // 2013/10/15 いなげやアドオン対応　実行モードの設定を追加 ISC:yshindo end
                }
                if (workEntity.ApprovalMode == Constants.OnFlag)
                {
                    // 2013/09/23 k.haraikawa start 承認項目の初期値を設定 
                    row.ApproverCd = string.Empty;
                    row.ApprovalStatus = "1";
                    row.ApprovalDate = "9999/12/31";
                    // 2013/09/23 k.haraikawa end 承認項目の初期値を設定 
                }
                else
                {
                    row.ApproverCd = UserProfile.UserId;
                    row.ApprovalStatus = "3";
                    row.ApprovalDate = DateTime.Now.ToString("yyyy/MM/dd");
                }
                // 2013/09/24 いなげやアドオン対応：承認しないモード対応 ISC:yshindo end
                // 2012.06.26 Start カバーシートの入力者を10桁までとしてセットする
                //row.PersonnelInput = UserProfile.UserName;
                row.PersonnelInput = UserProfile.UserName.Length <= 10 ? UserProfile.UserName : UserProfile.UserName.Substring(0, 10);
                // 2012.06.26 End   カバーシートの入力者を10桁までとしてセットする

                dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.AddCoverSheetMasterDataTable1Row(row);

                // カバーシートマスタワークエンティティ
                CoverSheetMasterEntity coverSheetMasterEntity = new CoverSheetMasterEntity()
                {
                    // 法人コード
                    CorporationCode = UserProfile.CorporationCode,

                    // バッチ番号
                    BatchNo = this.BatchNumberTextBox.Text,

                    // 店舗コード
                    StoreCd = this.BatchStoreLabel.Text,

                    // 伝票種別コード
                    SlipTypeCd = this.SlipTypeCoverDropDownList.SelectedValue,

                    // 伝票区分
                    SlipDiv = this.SlipDivisionCoverDropDownList.SelectedValue,

                    // 入力枚数
                    // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
                    //InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 0 : int.Parse(this.InputPageCountTextBox.Text),
                    InputPageCount = string.IsNullOrEmpty(this.InputPageCountTextBox.Text) ? 1 : int.Parse(this.InputPageCountTextBox.Text),
                    // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする

                    // バッチ日付
                    BatchDate = DateTime.Parse(this.BatchDateDateTextBox.Text)
                };

                this.SetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity, coverSheetMasterEntity);

                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

                //InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);

                if (this.GetFacadeInstance().UpInsSARCoverSheetMasterNew(workEntity, dsEntity).Equals("1"))
                {
                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCSARI0018)));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.BatchNumberTextBox.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return;
                }

                // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
                // カバーシートで入力した情報を伝票ヘッダーへもセットする
                this.SalesChangeDate.DateValue = this.BatchDateDateTextBox.DateValue;
                this.StoreTextBox.Text = this.BatchStoreLabel.Text;
                WorkEntity StoreworkEntity = new WorkEntity()
                {
                    Subsystem = "SA",
                    CorporationCode = UserProfile.CorporationCode,
                    BatchDate = this.SalesChangeDate.DateValue,
                    StoreCd = this.StoreTextBox.Text
                };

                // 【BSFCSAR050800025】を実行し判定結果（０：正常、９：異常）から入力可否を判断する。
                string rtnvalue = this.GetFacadeInstance().ChkStoreCodeNew(StoreworkEntity);

                if (rtnvalue.Equals("0"))
                {
                    this.StoreName.Text = StoreworkEntity.StoreName;
                }
                else
                {
                    this.StoreName.Text = string.Empty;
                }
                // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする

                // ６．画面情報表示～制御
                // ６．１カバーシート情報入力
                this.SetEnableControls(false);

                // ６．２入力状況情報
                this.SetMainHeader();

                // 2012.06.26 Start カバー保存後に伝票ヘッダー、明細が表示されることがある。(前回分を表示している)
                //if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                //{
                //    this.ShowSARSlipHeaderErrorData(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows[0]);
                //}
                // 2012.06.26 End   カバー保存後に伝票ヘッダー、明細が表示されることがある。(前回分を表示している)

                // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
                dsInputStatusEntity.CoverSheetMasterDataset = new CoverSheetMasterDataset();
                dsInputStatusEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
                dsInputStatusEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();
                dsInputStatusEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity, dsInputStatusEntity);
                // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

                this.TargetDataTable = this.GetSARSlipHeaderErrorDataset(this.GetPageNumber()).SARSlipHeaderError;
                this.DataBindToinputStatusListGrid();

                this.TransferSlipNumberTextBox.Text = workEntity.GetSlipNo;

                // 2012.06.26 Start カバー保存後に伝票ヘッダー、明細が表示されることがある。(前回分を表示している)
                //this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
                //this.DataBindToSlipDetailGridView();
                // 2012.06.26 End   カバー保存後に伝票ヘッダー、明細が表示されることがある。(前回分を表示している)

                this.SetLineNoForGridView(this.inputStatusListGrid, "SARCoverSheetMaster_Number_Control", this.GetPageNumber());
                // 2012.06.26 Start カバー保存後に伝票ヘッダー、明細が表示されることがある。(前回分を表示している)
                //this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
                //this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice();
                // 2012.06.26 End   カバー保存後に伝票ヘッダー、明細が表示されることがある。(前回分を表示している)
                this.SetHiddenGridViewBySlipType((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue));

                // 2014.01.06 Start 伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。
                //// 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
                //this.SetHeaderEnableControls(true);
                //// 2012.06.26 End   伝票ヘッダー情報の入力制御を修正
                if (initMode == 1 || initMode == 2)
                {
                    this.SetHeaderEnableControls(false);
                }
                else
                {
                    this.SetHeaderEnableControls(true);
                }
                // 2014.01.06 End   伝票検索画面やカバーシート検索画面から遷移した場合、ヘッダー情報は変更不可とする。
				//2016.03.18 start 部門検索子画面の基準日が売変日へ変更する。
                if (this.BatchDateDateTextBox.DateValue != null)
                {
                    this.SetSession(CategorySearchSessionKey.BasicDate, this.BatchDateDateTextBox.DateValue.Value.ToShortDateString());
                }
				//2016.03.18 end
                // ６．４メッセージ表示
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(MessageKey.MSGCSARI0028, "カバー保存")));
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.Category1Cd.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
            }

        }
        #endregion

        #region StoreTextBox_TextChanged (店舗コード入力)
        /// <summary>
        /// 店舗コード入力
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void StoreTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.BatchDateDateTextBox.DateValue == null)
            {
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "バッチ日付")));
                this.BatchDateDateTextBox.Focus();
                return;
            }

            // 8/1 SUEYOSHI ADD START
            if (string.IsNullOrEmpty(this.StoreTextBox.Text))
            {
                this.StoreTextBox.Text = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.StoreCd);
            }
            // 8/1 SUEYOSHI END

            WorkEntity workEntity = new WorkEntity()
            {
                Subsystem = "SA",
                CorporationCode = UserProfile.CorporationCode,
                BatchDate = this.BatchDateDateTextBox.DateValue,
                StoreCd = this.StoreTextBox.Text
            };

            // 【BSFCSAR050800025】を実行し判定結果（０：正常、９：異常）から入力可否を判断する。
            string storeCode = this.GetFacadeInstance().ChkStoreCodeNew(workEntity);

            if (storeCode.Equals("9"))
            {
                // 2014.04.30 Start 店舗コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
                this.StoreName.Text = string.Empty;
                // 2014.04.30 End   店舗コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）

                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0179)));
                this.StoreTextBox.Focus();

                return;
            }

            this.StoreName.Text = workEntity.StoreName;

           // 2014.06.02 Start 画面操作統一対応（カーソル位置） 
            // 店舗マスタに存在する場合は、次項目へカーソル移動
            //this.SalesChangeDate.Focus();                        // 売変または廃棄日
            // 2014.06.02 End   画面操作統一対応（カーソル位置）
        }
        #endregion

        #region StoreReferButton_Click
        /// <summary>
        /// StoreReferButton's click event handler.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event argument object</param>
        protected void StoreReferButton_Click(object sender, EventArgs e)
        {
            if (this.StoreModalResultHiddenField.Value == "1")
            {
                StoreSubScreenDataSet storeSubScreenDataSet =
                    (StoreSubScreenDataSet)this.GetSession(SalesChangeDisposalSlipInputSessionKey.StoreSubScreenDataSet);

                if (storeSubScreenDataSet != null)
                {
                    var selectedRow = storeSubScreenDataSet.StoreSubScreen.FirstOrDefault(row => !row.IsSelectFlgNull() && row.SelectFlg == Constants.OnFlag);

                    if (selectedRow != null)
                    {
                        this.StoreTextBox.Text = selectedRow.StoreCd;
                        this.StoreName.Text = selectedRow.IsStoreKanjiNull() ? string.Empty : selectedRow.StoreKanji;
                    }
                }
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.StoreTextBox.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
            }
        }
        #endregion

        #region Category1ReferButton_Click
        /// <summary>
        /// Category1ReferButton's click event handler.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event argument object</param>
        protected void Category1ReferButton_Click(object sender, EventArgs e)
        {
            if (this.CategoryModalResultHiddenField1.Value == "1")
            {
                // 暫定修正 2011.11.29 Start 子画面対応
                //CategorySubScreenDataSet categorySubScreenDataSet =
                //    (CategorySubScreenDataSet)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CategorySearchDataSet);
                CategorySubScreenDataSet categorySubScreenDataSet =
                    (CategorySubScreenDataSet)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CategorySubScreenDataSet);
                // 暫定修正 2011.11.29 End   子画面対応

                if (categorySubScreenDataSet != null)
                {
                    var selectedRow =
                        categorySubScreenDataSet.CategorySubScreen.FirstOrDefault(row => !row.IsSelectFlgNull() && row.SelectFlg == Constants.OnFlag);

                    if (selectedRow != null)
                    {
                        this.Category1Cd.Text = selectedRow.Category1Cd;
                        this.Category1Name.Text = selectedRow.IsCategory1KanjiNull() ? string.Empty : selectedRow.Category1Kanji;
                        // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
                        //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
                        //// ダミーコードを取得
                        //this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
                        //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
                        // 2014.05.28 End   部門代表コードの部分を機能しないように修正
                    }
                }
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.Category1Cd.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
            }
        }
        #endregion

        #region Category2ReferButton_Click
        /// <summary>
        /// Category2ReferButton's click event handler.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event argument object</param>
        protected void Category2ReferButton_Click(object sender, EventArgs e)
        {
            if (this.CategoryModalResultHiddenField2.Value == "1")
            {
                // 暫定修正 2011.11.29 Start 子画面対応
                //CategorySubScreenDataSet categorySubScreenDataSet =
                //    (CategorySubScreenDataSet)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CategorySearchDataSet);
                CategorySubScreenDataSet categorySubScreenDataSet =
                    (CategorySubScreenDataSet)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CategorySubScreenDataSet);
                // 暫定修正 2011.11.29 End   子画面対応

                if (categorySubScreenDataSet != null)
                {
                    var selectedRow =
                        categorySubScreenDataSet.CategorySubScreen.FirstOrDefault(row => !row.IsSelectFlgNull() && row.SelectFlg == Constants.OnFlag);

                    if (selectedRow != null)
                    {
                        this.Category2Cd.Text = selectedRow.Category2Cd;
                        this.Category2Name.Text = selectedRow.IsCategory2KanjiNull() ? string.Empty : selectedRow.Category2Kanji;
                        // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
                        //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
                        //// ダミーコードを取得
                        //this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
                        //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
                        // 2014.05.28 End   部門代表コードの部分を機能しないように修正
                    }
                }
                // 2014.05.09 Start 効率改善指摘事項の修正
                this.Category2Cd.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正
            }
        }
        #endregion

        // 2014.05.09 Start 効率改善指摘事項の修正
        #region ReCulculateButton_Click
        /// <summary>
        /// 再計算
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event argument object</param>
        protected void ReCulculateButton_Click(object sender, EventArgs e)
        {
            // 2014.05.27 Start Gridの数量チェックを強化
            // 伝票明細グリッドチェック
            if (CheckSlipDeatilGridInput().Equals(true))
            {
                return;
            }
            // 2014.05.27 End   Gridの数量チェックを強化

            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(true);
           
            //20151013 start q
            //// 2014.05.09 Start 効率改善指摘事項の修正
            //// ※選択ボタンより、明細を下ろした時は商品コード入力不可になっているので、
            ////   その時は数量にカーソル移動する
            //if (this.DetailAddSKUCd.Enabled == true)
            //{
            //    this.DetailAddSKUCd.Focus();
            //}
            //else
            //{
            //    this.DetailAddTransferQuantity.Focus();
            //}
            //// 2014.05.09 End   効率改善指摘事項の修正
            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
        }
        #endregion
        // 2014.05.09 End   効率改善指摘事項の修正

        #region SetEnableControls
        /// <summary>
        /// Sets the enable controls.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        private void SetEnableControls(bool enabled)
        {
            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            if (!string.IsNullOrEmpty((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.IsHeadquarters)))
            {
                // 本部ユーザーの場合
                // ※店舗ユーザーの場合は、そのままとする。
                this.BatchStoreLabel.Enabled = enabled;
            }
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。
            this.SlipTypeCoverDropDownList.Enabled = enabled;
            this.SlipDivisionCoverDropDownList.Enabled = enabled;
            this.BatchNumberTextBox.Enabled = enabled;
            this.BatchDateDateTextBox.Enabled = enabled;
            // 2014.05.28 Start カバー保存ボタンはカバーシート部分の入力の時のみ押下可能にする
            this.SaveCoverButton.Enabled = enabled;
            // 2014.05.28 End   カバー保存ボタンはカバーシート部分の入力の時のみ押下可能にする

            // 2013.11.26 カバシートを保存していない場合は、「保存」「伝票取消」ボタンを押せないようにする。（そのまま保存するとエラーとなる為） Start
            if (enabled == false)
            {
                // カバーシート入力項目が入力不可の場合は、保存が完了したと判断し「新規」「保存」「伝票取消」ボタンを有効とする
                this.ReCulculateButton.Enabled = true;
                this.NewButton.Enabled = true;             // 新規
                this.NextButton.Enabled = true;            // 保存
                this.SlipDeleateButton.Enabled = true;     // 伝票取消
            }
            else
            {
                // カバーシート入力項目が入力可の場合は、未保存と判断し「新規」「保存」「伝票取消」ボタンを無効とする
                this.ReCulculateButton.Enabled = false;
                this.NewButton.Enabled = false;            // 新規
                this.NextButton.Enabled = false;           // 保存
                this.SlipDeleateButton.Enabled = false;    // 伝票取消
            }
            // 2013.11.26 カバシートを保存していない場合は、「保存」「伝票取消」ボタンを押せないようにする。（そのまま保存するとエラーとなる為） End
        }
        #endregion

        // 2012.06.26 Start 伝票ヘッダー情報の入力制御を修正
        #region SetHeaderEnableControls (伝票ヘッダー情報の入力制御)
        /// <summary>
        /// 伝票ヘッダー情報の入力制御
        /// </summary>
        /// <param name="enabled">true=入力可能 false=入力不可能</param>
        private void SetHeaderEnableControls(bool enabled)
        {
            // 2014.05.17 Start ヘッダー部の伝票番号を入力不可とする
            //this.TransferSlipNumberTextBox.Enabled = enabled;
            // 2014.05.17 End   ヘッダー部の伝票番号を入力不可とする
            // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            //if (!string.IsNullOrEmpty((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.IsHeadquarters)))
            //{
            //    // 本部ユーザーの場合
            //    // ※店舗ユーザーの場合は、そのままとする。
            //    this.StoreTextBox.Enabled = enabled;
            //}
            // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            this.Category1Cd.Enabled = enabled;
            this.Category2Cd.Enabled = enabled;
            // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            //this.SalesChangeDate.Enabled = enabled;
            // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
            // 更新フラグのクリア
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode, "0");
            // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
            // 2012.06.26 Start 商品を選択した場合は商品コードの変更は不可とする。
            //this.DetailAddSKUCd.Enabled = true;
            // 2012.06.26 End   商品を選択した場合は商品コードの変更は不可とする。
            // 2014.05.28 Start 明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
            // 選択中以外は選択ボタンを押せるようにする
            //this.DetailChoiceButton.Enabled = true;
            this.DetailDeleateButton.Enabled = enabled;
            // 選択中以外の場合は保存ボタンのメッセージを元に戻すする
            this.NextButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0022);
            // 2014.05.28 End   明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御

            // 2014.04.30 Start 参照ボタンの使用可否設定追加
            this.StoreReferButton.Enabled = enabled;            // 店舗参照ボタン
            this.Category1ReferButton.Enabled = enabled;        // 分類１参照ボタン
            // 2014.04.30 End   参照ボタンの使用可否設定追加
        }
        #endregion
        // 2012.06.26 End   伝票ヘッダー情報の入力制御を修正

        #region PageInitialized
        /// <summary>
        /// ページの初期化する。
        /// </summary>
        private void PageInitialized()
        {
            string corporationCode = UserProfile.CorporationCode;

            int initialMode = 0;
            string orgScreenId = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.OrgScreenId);

            if (orgScreenId != null && orgScreenId.Equals(SalesChangeDisposalSlipInputConstances.Scrcsar050106))
            {
                initialMode = 1;
            }
            else if (orgScreenId != null && orgScreenId.Equals(SalesChangeDisposalSlipInputConstances.Scrcsar050306))
            {
                initialMode = 2;
            }

            if (initialMode == 1 || initialMode == 2)
            {
                this.Master.HeaderMessageLabelString = RetailConstant.DisplayMode.Modify;
                this.Master.HeaderBackColor = Color.FromName(RetailConstant.ColorDisplayMode.Modify);
            }
            else
            {
                this.Master.HeaderMessageLabelString = RetailConstant.DisplayMode.Add;
                this.Master.HeaderBackColor = Color.FromName(RetailConstant.ColorDisplayMode.Add);
            }

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.InitMode, initialMode);

            // 以下をWorkEntityへセットする
            WorkEntity workEntity = new WorkEntity()
            {
                CorporationCode = UserProfile.CorporationCode,

                // 呼出元サブシステムＩＤ 
                Subsystem = "SA",

                // 在庫サブシステムＩＤ
                IVSystemId = "IV",

                // 承認モード
                ApprovalMode = "1",

                // 業務種別
                SystemType = "1",

                // 起動モード
                InitMode = initialMode.ToString(),

                // 伝票種別コード区分コード
                SlipTypeCdForDiv = "CDCSAR0012",

                // 伝票区分売変コード区分コード
                SlipKbnSalesChangeCdForDiv = "CDCSAR0013",

                // 伝票区分廃棄コード区分コード
                SlipDivDisposalCdForDiv = "CDCSAR0014",

                // 2012.06.13 Start 廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。
                // ※ここは未使用なのでコメントアウトしておく
                //// 売変伝票データID
                //SalesChangeSlipDataId = "SAR002",

                //// 廃棄伝票データID
                //DisposalSlipDataId = "SAR004",
                // 2012.06.13 End   廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。

                // 販売伝票事由コード
                SaleSlipReasonCode = "CDCSAR0041",

                // 2012.06.21 Start 売変用の事由を追加（値下げ、値上げの２パターン分）
                // 販売伝票事由コード（売変の値下げ）
                SaleSlipReasonCodeSalesChangePriceDown = "CDCSAR0043",

                // 販売伝票事由コード（売変の値上げ）
                SaleSlipReasonCodeSalesChangePriceUp = "CDCSAR0044",
                // 2012.06.21 End   売変用の事由を追加（値下げ、値上げの２パターン分）

                // 伝票登録状況区分コード
                SlipRegisStatusCodeKbn = "CDCSAR0042",

                // 8/9 SUEYOSHI 同一のバッチ番号が複数あった場合の修正 ADD
                // 押下されたslipNoをセッションから取得
                TargetSlipNo = (string)GetSession(SalesChangeDisposalSlipSearchSessionKey.SASlipCorrectionNo)
                // 8/9 SUEYOSHI END
            };

            // 2014/04/10 Add By ORZ sou 画面効率化対応
            this.RemoveSession(SalesChangeDisposalSlipSearchSessionKey.SASlipCorrectionNo);
            // 2014/04/10 Add End

#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 -->
            if (initialMode == 2)
            {
                // WorkEntity.伝票番号枝番＝SASlipCorrectionBranchNoをセット
                workEntity.SlipSubNo = this.GetSession(SalesChangeDisposalSlipSearchSessionKey.SASlipCorrectionBranchNo).ToString();
                
                // 2014/04/10 Add By ORZ sou 画面効率化対応
                this.RemoveSession(SalesChangeDisposalSlipSearchSessionKey.SASlipCorrectionBranchNo);
                // 2014/04/10 Add End
            }
#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 <--



            // ２．メンバ変数を生成する。
            InitialDisplayEntity initialDisplayEntity = new InitialDisplayEntity();
            initialDisplayEntity.APAvaliableDataSet = new APVariablesDataSet();
            initialDisplayEntity.APVariableDispDataset = new APVariablesDataSet();
            initialDisplayEntity.SystemDateDataset = new DistributionCommon.Common.MasterBusiness.DataTransferObject.CMCSystemDateDataSet();
            initialDisplayEntity.UserStoreDataset = new UserStoreDataSet();
            initialDisplayEntity.DisposalSlipDivListDictionary = new List<CodeDivisionValueElement>();
            initialDisplayEntity.ReasonListDictionary = new List<CodeDivisionValueElement>();
            // 2012.06.21 Start 売変用の事由を追加（値下げ、値上げの２パターン分）
            initialDisplayEntity.ReasonSalesChangePriceDownListDictionary = new List<CodeDivisionValueElement>();
            initialDisplayEntity.ReasonSalesChangePriceUpListDictionary = new List<CodeDivisionValueElement>();
            // 2012.06.21 End   売変用の事由を追加（値下げ、値上げの２パターン分）
            initialDisplayEntity.SalesChangeSlipDivListDictionary = new List<CodeDivisionValueElement>();
            initialDisplayEntity.SlipTypeListDictionary = new List<CodeDivisionValueElement>();

            // AP変数取得Dｓのメンバに下記のＡＰ変数取得Ｋｅｙを入れて設定値を取得する
            // BYR確認画面ＯＦＦ
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_CheckDisplay_Off);

            // 2013.08.20 Start 承認機能区分を売変用として追加。既存は破棄用として使用する。
            // 承認機能区分(売変用)
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_ApprovalFeatureDivForSalesChange);
            // 承認機能区分(廃棄用)
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_ApprovalFeatureDiv);
            // 2013.08.20 End 承認機能区分を売変用として追加。既存は破棄用として使用する。

            // ＳＴＲ商品コード桁数
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.STR_PRODUCTSCDDIGITS);

            // 未承認
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_APPROVALSTATUS_APPLY);

            // 承認待ち
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_APPROVALSTATUS_WAITING);

            // 承認済み
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_APPROVALSTATUS_CORRECT);

            // 否認
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_APPROVALSTATUS_INCORRECT);

            // ＳＡＲ売変伝票種別
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_SALESCHANGE_SLIPTYPECD);

            // ＳＡＲ廃棄伝票種別
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_DISPOSAL_SLIPTYPECD);

            // 伝票入力画面上伝票番号行数
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_SLIPNO_MAXLINE);

            // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
            // ＳＡＲ廃棄入力原価売価変更区分
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_DISPOSAL_COST_PRICE_CHANGE_DIV);
            // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする

            // 2013.08.20 Start 売単価取得方法の取得
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_UNIT_PRICE_FETCH_METHOD_SALE);
            this.RegisterAPVariable(initialDisplayEntity.APVariableDispDataset, APVariableListKey.SAR_UNIT_PRICE_FETCH_METHOD_DISPOSAL);
            // 2013.08.20 End 売単価取得方法の取得

            // バッチ日付
            DateTime? inputBatchDate = null;

            if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchDate) != null)
            {
                ////inputBatchDate = DateTime.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchDate));
                inputBatchDate = Convert.ToDateTime(this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchDate));
            }

            // カバーシートマスタワークエンティティ
            CoverSheetMasterEntity coverSheetMasterEntity = new CoverSheetMasterEntity()
            {
                // 法人コード
                CorporationCode = UserProfile.CorporationCode,

                // バッチ番号
                BatchNo = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchNo),

                // 店舗コード
                StoreCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.StoreCd),

                // 伝票種別コード
                SlipTypeCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeCd),

                // 伝票区分
                SlipDiv = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipDiv),

                // バッチ日付
                BatchDate = inputBatchDate
            };

            if (initialMode == 0 && this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity) != null)
            {
                coverSheetMasterEntity = (CoverSheetMasterEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity);
            }

            // 伝票情報Entity
            SlipInfoEntity slipInfoEntity = new SlipInfoEntity()
            {
                // 法人コード
                CorporationCode = UserProfile.CorporationCode,

                // 店舗コード
                StoreCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.StoreCd),

                // 伝票種別コード
                SlipTypeCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeCd),

                // 伝票区分
                SlipDiv = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipDiv),

                // バッチ日付
                BatchDate = inputBatchDate,

                // 伝票番号
                SlipNo = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipNo)
            };

            // 2014/04/10 Add By ORZ sou 画面効率化対応
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SlipNo);
            // 2014/04/10 Add End

            DsEntity dsEntity = new DsEntity();
            dsEntity.CoverSheetMasterDataset = new CoverSheetMasterDataset();
            dsEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
            dsEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();

            // ３．売変データ入力初期表示ファサードを呼び出す。
            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            //this.GetFacadeInstance().InitStart(workEntity, initialDisplayEntity, coverSheetMasterEntity, dsEntity);
            string isHeadquarters = string.Empty;
            this.GetFacadeInstance().InitStartNew(workEntity, initialDisplayEntity, coverSheetMasterEntity, dsEntity, out isHeadquarters);
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity, workEntity);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity, initialDisplayEntity);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity, coverSheetMasterEntity);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.SystemDate, initialDisplayEntity.SystemDateDataset.CMCSystemDate.FirstOrDefault().SystemDate);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
            string valuePriceChangeDiv = initialDisplayEntity.APVariableDispDataset.UTFAPVariable.AsEnumerable().Where(
                r => r.APVariableCode == APVariableListKey.SAR_DISPOSAL_COST_PRICE_CHANGE_DIV &&
                     r.APCorporationCode == UserProfile.CorporationCode).FirstOrDefault().APVariableValue;

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv, valuePriceChangeDiv);
            // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする

            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.IsHeadquarters, isHeadquarters);
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。

            DsEntity tempEntity = new DsEntity();
            tempEntity.CoverSheetMasterDataset = new CoverSheetMasterDataset();
            tempEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
            tempEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();
            tempEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Merge(dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError);
            tempEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Merge(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError);
            tempEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, tempEntity);

            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            DsEntity dsInputStatusEntity = new DsEntity();
            dsInputStatusEntity.CoverSheetMasterDataset = new CoverSheetMasterDataset();
            dsInputStatusEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
            dsInputStatusEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();
            dsInputStatusEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Merge(dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError);
            dsInputStatusEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Merge(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError);
            dsInputStatusEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity, dsInputStatusEntity);
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

            //2015.11.16 start 既存伝票の新規追加行が削除できるように追加

            DsEntity OrgDsEntity = new DsEntity();

            OrgDsEntity.SARSlipDetailErrorDataSet = (SARSlipDetailErrorInfoDataSet)dsEntity.SARSlipDetailErrorDataSet.Copy();

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.OrgDsEntity, OrgDsEntity);
            //2015.11.16 end 既存伝票の新規追加行が削除できるように追加

            // ４．画面項目を表示～制御する。
            // ドロップダウンリスト生成
            // 伝票種別ドロップダウンリスト
            this.SlipTypeCoverDropDownList.DataSource = initialDisplayEntity.SlipTypeListDictionary;
            this.SlipTypeCoverDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
            this.SlipTypeCoverDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
            this.SlipTypeCoverDropDownList.DataBind();


            if (this.SlipTypeCoverDropDownList.Items.Count > 0)
            {
                // 7/25 SUEYOSHI UPD START 新規モードの場合
                if (workEntity.InitMode == "0")
                // 7/25 SUEYOSHI END
                {
                    if (this.SlipTypeCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChange)
                    {
                        // 伝票区分ドロップダウンリスト
                        this.SlipDivisionCoverDropDownList.DataSource = initialDisplayEntity.SalesChangeSlipDivListDictionary;
                        this.SlipDivisionCoverDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        this.SlipDivisionCoverDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        this.SlipDivisionCoverDropDownList.DataBind();

                        // 2012.06.21 Start 売変用の事由を追加（値下げ、値上げの２パターン分）
                        if (this.SlipDivisionCoverDropDownList.Items.Count > 0)
                        {
                            // 8/9 SUEYOSHI 売変区分をAP変数から取得するように修正 START 
                            // if (this.SlipDivisionCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChangePriceDown)
                            if (this.SlipDivisionCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice)
                            // 8/9 SUEYOSHI END
                            {
                                // 値下げ
                                // 事由ドロップダウンリスト
                                this.DetailAddReason.DataSource = initialDisplayEntity.ReasonSalesChangePriceDownListDictionary;
                                this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                                this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                                this.DetailAddReason.DataBind();

                            }
                            else
                            {
                                // 値上げ
                                // 事由ドロップダウンリスト
                                this.DetailAddReason.DataSource = initialDisplayEntity.ReasonSalesChangePriceUpListDictionary;
                                this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                                this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                                this.DetailAddReason.DataBind();
                            }
                        }
                    }
                    else if (this.SlipTypeCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.Disposal)
                    {
                        // 伝票区分ドロップダウンリスト
                        this.SlipDivisionCoverDropDownList.DataSource = initialDisplayEntity.DisposalSlipDivListDictionary;
                        this.SlipDivisionCoverDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        this.SlipDivisionCoverDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        this.SlipDivisionCoverDropDownList.DataBind();

                        // 事由ドロップダウンリスト（廃棄用）
                        this.DetailAddReason.DataSource = initialDisplayEntity.ReasonListDictionary;
                        this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        this.DetailAddReason.DataBind();
                    }
                }
                // 7/25 SUEYOSHI UPD START
                // 2013/09/23 いなげやアドオン対応：カバーシート画面から遷移時も追加 ISC:yshindo start
                //else if (workEntity.InitMode == "2")
                else if (workEntity.InitMode == "2" || initialMode == 1)
                // 2013/09/23 いなげやアドオン対応：カバーシート画面から遷移時も追加 ISC:yshindo end
                {
                    if (coverSheetMasterEntity.SlipTypeCd == SalesChangeDisposalSlipInputConstances.SalesChange)
                    {
                        // 伝票区分ドロップダウンリスト
                        this.SlipDivisionCoverDropDownList.DataSource = initialDisplayEntity.SalesChangeSlipDivListDictionary;
                        this.SlipDivisionCoverDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        this.SlipDivisionCoverDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        this.SlipDivisionCoverDropDownList.DataBind();

                        if (this.SlipDivisionCoverDropDownList.Items.Count > 0)
                        {
                            // 8/9 SUEYOSHI 売変区分をAP変数から取得するように修正 START 
                            // if (coverSheetMasterEntity.SlipDiv == SalesChangeDisposalSlipInputConstances.SalesChangePriceDown)
                            if (coverSheetMasterEntity.SlipDiv == SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice)
                            // 8/9 SUEYOSHI END
                            {
                                // 値下げ
                                // 事由ドロップダウンリスト
                                this.DetailAddReason.DataSource = initialDisplayEntity.ReasonSalesChangePriceDownListDictionary;
                                this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                                this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                                this.DetailAddReason.DataBind();
                            }
                            else
                            {
                                // 値上げ
                                // 事由ドロップダウンリスト
                                this.DetailAddReason.DataSource = initialDisplayEntity.ReasonSalesChangePriceUpListDictionary;
                                this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                                this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                                this.DetailAddReason.DataBind();
                            }
                        }
                    }
                    else if (coverSheetMasterEntity.SlipTypeCd == SalesChangeDisposalSlipInputConstances.Disposal)
                    {
                        // 伝票区分ドロップダウンリスト
                        this.SlipDivisionCoverDropDownList.DataSource = initialDisplayEntity.DisposalSlipDivListDictionary;
                        this.SlipDivisionCoverDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        this.SlipDivisionCoverDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        this.SlipDivisionCoverDropDownList.DataBind();

                        // 事由ドロップダウンリスト（廃棄用）
                        this.DetailAddReason.DataSource = initialDisplayEntity.ReasonListDictionary;
                        this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        this.DetailAddReason.DataBind();
                    }
                }
                // 7/25 SUEYOSHI UPD END
            }

            // 2012.06.21 Start 売変用の事由を追加（値下げ、値上げの２パターン分）
            //// 事由ドロップダウンリスト
            //this.DetailAddReason.DataSource = initialDisplayEntity.ReasonListDictionary;
            //this.DetailAddReason.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
            //this.DetailAddReason.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
            //this.DetailAddReason.DataBind();
            // 2012.06.21 End   売変用の事由を追加（値下げ、値上げの２パターン分）

            // 2013.11.26 Start カバシートを保存していない場合は、「保存」「伝票取消」ボタンを押せないようにする。（そのまま保存するとエラーとなる為）
            // 初期設定として「新規」「保存」「伝票取消」ボタンを無効にする。
            // この後処理で「(メソッド)SetEnableControls」処理で、カバーシート情報入力不可となった場合は、
            // 処理内でボタンを有効にしている。
            this.ReCulculateButton.Enabled = false;     //再計算
            this.NewButton.Enabled = false;             // 新規
            this.NextButton.Enabled = false;            // 保存
            this.SlipDeleateButton.Enabled = false;     // 伝票取消
            this.DetailAddButton.Enabled = false;
            // 2013.11.26 End   カバシートを保存していない場合は、「保存」「伝票取消」ボタンを押せないようにする。（そのまま保存するとエラーとなる為）

            if (initialMode == 1)
            {
                this.BatchStoreLabel.Text = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.StoreCd);

                // 2014/04/10 Add By ORZ sou 画面効率化対応
                this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.StoreCd);
                // 2014/04/10 Add End
                
                if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeCd) != null)
                {
                    this.SlipTypeCoverDropDownList.SelectedValue = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeCd);
                    // 2014/04/10 Add By ORZ sou 画面効率化対応
                    this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeCd);
                    // 2014/04/10 Add End
                }

                if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipDiv) != null)
                {
                    this.SlipDivisionCoverDropDownList.SelectedValue = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipDiv);
                    // 2014/04/10 Add By ORZ sou 画面効率化対応
                    this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SlipDiv);
                    // 2014/04/10 Add End
                }

                this.BatchNumberTextBox.Text = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchNo);
                // 2014/04/10 Add By ORZ sou 画面効率化対応
                this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.InputBatchNo);
                // 2014/04/10 Add End

                this.InputPageCountTextBox.Text = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputPageCount);
                // 2014/04/10 Add By ORZ sou 画面効率化対応
                this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.InputPageCount);
                // 2014/04/10 Add End

                if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchDate) != null)
                {
                    this.BatchDateDateTextBox.DateValue = DateTime.Parse(Convert.ToString(this.GetSession(SalesChangeDisposalSlipInputSessionKey.InputBatchDate)));
                }
                // 2013/09/17 いなげやアドオン対応：カバーシートから遷移時もコントロールの制御を追加 ISC:yshindo start
                this.SetEnableControls(false);
                this.NextCoverButton.Enabled = false;
                // 2013/09/17 いなげやアドオン対応：カバーシートから遷移時もコントロールの制御を追加 ISC:yshindo end
            }
            else if (initialMode == 2)
            {
                this.BatchStoreLabel.Text = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionStoreCD);
                // 2014/04/10 Add By ORZ sou 画面効率化対応
                this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionStoreCD);
                // 2014/04/10 Add End

                if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDataType) != null)
                {
                    this.SlipTypeCoverDropDownList.SelectedValue = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDataType);
                    // 2014/04/10 Add By ORZ sou 画面効率化対応
                    this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDataType);
                    // 2014/04/10 Add End
                }

                if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDiv) != null)
                {
                    this.SlipDivisionCoverDropDownList.SelectedValue = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDiv);
                    // 2014/04/10 Add By ORZ sou 画面効率化対応
                    this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDiv);
                    // 2014/04/10 Add End
                }

                this.BatchNumberTextBox.Text = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionInputBatchNo);
                // 2014/04/10 Add By ORZ sou 画面効率化対応
                this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionInputBatchNo);
                // 2014/04/10 Add End

                if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionBatchDate) != null)
                {
                    ////this.BatchDateDateTextBox.DateValue =
                    ////    DateTime.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionBatchDate));
                    this.BatchDateDateTextBox.DateValue =
                        Convert.ToDateTime(this.GetSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionBatchDate));
                    // 2014/04/10 Add By ORZ sou 画面効率化対応
                    this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionBatchDate);
                    // 2014/04/10 Add End

                }

                // 入力状況情報
                if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Rows.Count > 0)
                {
                    this.InputPageCountTextBox.Text = dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.FirstOrDefault().InputPageCount.ToString();
                }

                // 伝票ヘッダー情報
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    this.Category1Cd.Text = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().Category1Cd;
                    this.Category2Cd.Text = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().Category2Cd;
                }

                // 2012.01.04 関根 伝票検索画面から遷移時にカバー情報を編集不可にする 追加 開始
                // カバーシート情報入力
                this.SetEnableControls(false);
                // 2012.01.04 関根 伝票検索画面から遷移時にカバー情報を編集不可にする 追加 終了

                // 8/9 SUEYOSHI 検索画面からの遷移の場合　ボタン非アクティブ　追加
                this.NextCoverButton.Enabled = false;
                // 8/9 SUEYOSHI END
            }
            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            else if (initialMode == 0)
            {
                // 新規登録時
                if (initialDisplayEntity.UserStoreDataset.UserStoreDataTable.Count > 0)
                {
                    var userStoreRow = initialDisplayEntity.UserStoreDataset.UserStoreDataTable[0];

                    if (string.IsNullOrEmpty(isHeadquarters))
                    {
                        // 店舗ユーザー
                        this.BatchStoreLabel.Text = userStoreRow.StoreCd;
                        this.BatchStoreLabel.Enabled = false;
                    }
                    else
                    {
                        // 本部ユーザー
                        this.BatchStoreLabel.Text = string.Empty;
                        this.BatchStoreLabel.Enabled = true;
                    }
                }
            }
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。

            if (initialDisplayEntity.SystemDateDataset.CMCSystemDate.Rows.Count > 0)
            {
                // WorkEntity.販売管理月度＝業務日付データセット.業務月度
                workEntity.SalesMngMontn = initialDisplayEntity.SystemDateDataset.CMCSystemDate.FirstOrDefault().SystemDate.Month.ToString();

                // WorkEntity.販売管理業務日付＝業務日付データセット.業務日付
                workEntity.SalesMngDate = initialDisplayEntity.SystemDateDataset.CMCSystemDate.FirstOrDefault().SystemDate;

                this.SetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity, workEntity);
            }

            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            //if (initialDisplayEntity.UserStoreDataset.UserStoreDataTable.Count > 0)
            //{
            //    var userStoreRow = initialDisplayEntity.UserStoreDataset.UserStoreDataTable[0];

            //    // 店舗ユーザー
            //    if (userStoreRow.HeadquartersStoreDiv == "1")
            //    {
            //        this.hdUserProfileStoreCd.Value = userStoreRow.StoreCd;
            //        this.hdStockHeadquatersStoreDiv.Value = "1";
            //        this.hdStoreSearchConditionDiv.Value = "2";

            //    }
            //    else
            //    {
            //        // 本部ユーザー
            //        this.hdUserProfileStoreCd.Value = string.Empty;
            //        this.hdStockHeadquatersStoreDiv.Value = string.Empty;
            //        this.hdStoreSearchConditionDiv.Value = "1";
            //    }
            //}


            if (initialDisplayEntity.UserStoreDataset.UserStoreDataTable.Count > 0)
            {
                var userStoreRow = initialDisplayEntity.UserStoreDataset.UserStoreDataTable[0];

                if (string.IsNullOrEmpty(isHeadquarters))
                {
                    // 店舗ユーザー
                    this.hdUserProfileStoreCd.Value = userStoreRow.StoreCd;
                    this.hdStockHeadquatersStoreDiv.Value = "1";
                    this.hdStoreSearchConditionDiv.Value = "2";

                    // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
                    //this.StoreTextBox.Text = userStoreRow.StoreCd;
                    //this.StoreName.Text = userStoreRow.StoreKanji;
                    //this.StoreTextBox.Enabled = false;
                    //this.StoreReferButton.Enabled = false;
                    // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
                }
                else
                {
                    // 本部ユーザー
                    this.hdUserProfileStoreCd.Value = string.Empty;
                    this.hdStockHeadquatersStoreDiv.Value = string.Empty;
                    this.hdStoreSearchConditionDiv.Value = "1";

                    // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
                    //this.StoreTextBox.Text = string.Empty;
                    //this.StoreName.Text = string.Empty;
                    //this.StoreTextBox.Enabled = true;
                    //this.StoreReferButton.Enabled = true;
                    // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
                }
            }
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードは変更不可にする。

            // 起動モード＝１または２の場合、ファサード【BSFCSAR050800001】を実行した結果を下記にセットします。
            if (initialMode == 1 || initialMode == 2 || initialMode == 0)
            {
                // 入力状況情報
                if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Rows.Count > 0)
                {
                    this.ShowSARCoverSheetMasterData(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Rows[0]);
                    // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
                    // 伝票ヘッダが全てセットされてから名称を取得するため下の場所へ移動
                    //// 2012.01.04 関根 伝票検索から遷移時、店舗名称／部門名称を表示する 追加 開始
                    //this.StoreTextBox_TextChanged(null, new EventArgs());
                    //this.Category1Cd_TextChanged(null, new EventArgs());
                    //// 2012.01.04 関根 伝票検索から遷移時、店舗名称／部門名称を表示する 追加 終了
                    // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
                }
                else if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity) != null)
                {
                    this.ShowSARCoverSheetMasterData((CoverSheetMasterEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity));
                }

                // 伝票ヘッダー情報
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    this.ShowSARSlipHeaderErrorData(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows[0]);
                    // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
                    // 伝票ヘッダが全てセットされてから名称を取得するため上の場所から移動
                    // 2012.01.04 関根 伝票検索から遷移時、店舗名称／部門名称を表示する 追加 開始
                    // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
                    //this.StoreTextBox_TextChanged(null, new EventArgs());
                    // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
                    this.Category1Cd_TextChanged(null, new EventArgs());
                    // 2012.01.04 関根 伝票検索から遷移時、店舗名称／部門名称を表示する 追加 終了
                    // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
                }

                if (initialMode == 0 && !string.IsNullOrEmpty(workEntity.GetSlipNo))
                {
                    this.TransferSlipNumberTextBox.Text = workEntity.GetSlipNo.ToString();
                }

                this.SetMainHeader();

                // 2013/09/17 いなげやアドオン対応：カバーシート画面から遷移時のヘッダ情報を非アクティブにする ISC:yshindo start
                // 8/21 SUEYOSHI 検索結果画面からの遷移の場合、ヘッダー情報を非アクティブにする
                //if (initialMode == 2)
                //if (initialMode == 2 || initialMode == 1)
                //{
                    this.SetHeaderEnableControls(false);
                //}
                // 8/21 SUEYOSHI END
                // 2013/09/17 いなげやアドオン対応：カバーシート画面から遷移時のヘッダ情報を非アクティブにする ISC:yshindo end

                // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
                // カバーシートに保存されている伝票全てを取得するので対象の伝票番号項目は空にする。
                workEntity.TargetSlipNo = string.Empty;
                // 伝票ヘッダー、伝票明細をクリア
                dsInputStatusEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
                dsInputStatusEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();

                // 2014.03.18 Start 効率改善対応（伝票情報取得を複数回行っている処理を１回のみにする）
                //// 伝票検索のファサードを実行する。
                //// 【BSFCSAR050800019】
                //this.GetFacadeInstance().FillSARSlip(workEntity, dsInputStatusEntity);

                ////////////////////////////////////////////////////////////////////////////////////
                // カバーシートのバッチ番号に紐付いている伝票番号取得の為、
                // 伝票ヘッダーや伝票明細の情報までもSELECTしている為、
                // カバーシート情報のみ取得する様に修正
                ////////////////////////////////////////////////////////////////////////////////////
                // 伝票ヘッダー情報を検索するファサードを実行する。
                // 【BSFCSAR050800036】
                this.GetFacadeInstance().FillSARHeaderErrorSlipNew(workEntity, dsInputStatusEntity);
                // 2014.03.18 End   効率改善対応（伝票情報取得を複数回行っている処理を１回のみにする）

                // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

                // カバーシート情報表示部分
                this.TargetDataTable = this.GetSARSlipHeaderErrorDataset(0).SARSlipHeaderError;
                this.DataBindToinputStatusListGrid();

                // 伝票明細情報
                this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
                this.DataBindToSlipDetailGridView();

                //20151112 start 明細行直接編集　　ADD
                //ドロップダウンリストbind
                this.BindToDetailReasonDownList(dsEntity);
                //20151112 end 明細行直接編集　　ADD

                if (SalesChangeDisposalSlipInputConstances.Disposal.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
                {
                    // 現在入力中の伝票種別が廃棄伝票の場合
                    workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDiv);

                }
                else if (SalesChangeDisposalSlipInputConstances.SalesChange.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
                {
                    // 現在入力中の伝票種別が売変伝票の場合
                    workEntity.ApprovalMode = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDivForSalesChange);
                }

                // 新規 一旦承認されたカバーシートには新し伝票を追加しないようにする
                if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Any() &&
                    dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.FirstOrDefault().ApprovalStatus == "3" &&
                    workEntity.ApprovalMode == "1")
                {
                    this.NewButton.Enabled = false;
                }
                else
                {
                    this.NewButton.Enabled = true;

                }

            }
            else
            {
                // カバーシート情報表示部分
                this.TargetDataTable = new SARSlipHeaderErrorInfoDataset().SARSlipHeaderError;
                this.DataBindToinputStatusListGrid();

                // 伝票明細情報
                this.TargetDataSet = new SARSlipDetailErrorInfoDataSet();
                this.DataBindToSlipDetailGridView();


                //20151112 start 明細行直接編集　　ADD
                //ドロップダウンリストbind
                this.BindToDetailReasonDownList(dsEntity);
                //20151112 end 明細行直接編集　　ADD

            }

            this.ShowDataPager(initialDisplayEntity, dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count, 0);

            // 売変伝票の場合
            this.SetHiddenGridViewBySlipType(this.SlipTypeCoverDropDownList.SelectedValue);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue, this.SlipTypeCoverDropDownList.SelectedValue);

            if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.PageNumber) == null)
            {
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.PageNumber, 0);
            }

            this.SetLineNoForGridView(this.inputStatusListGrid, "SARCoverSheetMaster_Number_Control", 0);
            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
            this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(false);
            this.ConvertToCurencyFormat();

            // AP変数SAR_ApprovalFeatureDiv=0の場合は承認依頼ボタンは非表示になります。
            this.ApprovalRequestButton.Enabled = !this.CheckSar(initialDisplayEntity, APVariableListKey.SAR_ApprovalFeatureDiv);

            // Set value for hdCorporatonCode hidden controll.
            this.hdCorporationCode.Value = UserProfile.CorporationCode;

            // Set common screen argument items for using common screen.
            this.SetCommonScreenArgumentItems();

            this.GetProductName();

            string updateCoverSheetMessage = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateCoverSheetMessage);
            if (!string.IsNullOrEmpty(updateCoverSheetMessage))
            {
                this.SetMessage(new MessageContainer(updateCoverSheetMessage));
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.UpdateCoverSheetMessage, string.Empty);
            }

            // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            //if (this.InputPageCountTextBox.Text == "0")
            //{
            //    this.InputPageCountTextBox.Text = string.Empty;
            //}
            if (string.IsNullOrEmpty(this.InputPageCountTextBox.Text) || this.InputPageCountTextBox.Text == "0")
            {
                this.InputPageCountTextBox.Text = "1";
            }
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする

            //2015.10.23　start  明細行にて直接編集により不要
            // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //this.ChangeInputControl();
            // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            //2015.10.23 end  明細行にて直接編集により不要


            // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
            //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
            //// ダミーコードを取得
            //this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
            //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
            // 2014.05.28 End   部門代表コードの部分を機能しないように修正

            //   2014.04.24 SERIO杉岡 カバーシート検索、売変廃棄伝票検索リンク表示制御 開始
            CoverSheetSearchLinkButton.Visible = initialMode == 1;
            DisposalSlipSearchLinkButton.Visible = initialMode == 2;
            //   2014.04.24 SERIO杉岡 カバーシート検索、売変廃棄伝票検索リンク表示制御 終了

            // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            // カバーシートで入力した情報を伝票ヘッダーへもセットする
            this.SalesChangeDate.DateValue = this.BatchDateDateTextBox.DateValue;
            this.StoreTextBox.Text = this.BatchStoreLabel.Text;

            if (!string.IsNullOrEmpty(this.SalesChangeDate.Text) 
                && !string.IsNullOrEmpty(this.StoreTextBox.Text))
            {
                WorkEntity StoreworkEntity = new WorkEntity()
                {
                    Subsystem = "SA",
                    CorporationCode = UserProfile.CorporationCode,
                    BatchDate = this.SalesChangeDate.DateValue,
                    StoreCd = this.StoreTextBox.Text
                };

                // 【BSFCSAR050800025】を実行し判定結果（０：正常、９：異常）から入力可否を判断する。
                string rtnvalue = this.GetFacadeInstance().ChkStoreCodeNew(StoreworkEntity);

                if (rtnvalue.Equals("0"))
                {
                    this.StoreName.Text = StoreworkEntity.StoreName;
                }
                else
                {
                    this.StoreName.Text = string.Empty;
                }
            }
            else 
            {
                this.StoreName.Text = string.Empty;
            }
            // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする

            // 2014.05.09 Start 効率改善指摘事項の修正
            if (initialMode == 2 || initialMode == 1)
            {
                // 2014.06.14 Start 品質改善検証の指摘事項対応
                //// 修正の場合は商品コードにフォーカスをセット
                //this.DetailAddSKUCd.Focus();

                // カバーシート検索や売変廃棄伝票検索画面から遷移した場合は、
                // 明細グリッドにフォーカスを移動する
                if (this.SlipDetailGridView.Rows.Count > 0)
                {
                    // 明細グリッドの最初の入力項目にフォーカス移動
                    foreach (GridRecord record in this.SlipDetailGridView.Rows)
                    {
                        // グリッドから数量取得
                        CoreCenterTextBox disposalChangeQty = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");
                        disposalChangeQty.Focus();
                        break;
                    }
                }
                else
                {
                    // 明細グリッドが０件の場合は、商品コードにフォーカス移動
                    this.DetailAddSKUCd.Focus();
                }
                // 2014.06.14 End   品質改善検証の指摘事項対応
            }
            else
            {
                // 2014.06.14 Start 品質改善検証の指摘事項対応
                //// 新規の場合はバッチ店舗にフォーカスをセット
                //this.BatchStoreLabel.Focus();

                // 店舗ユーザーにより、入力できる項目変わる為、
                // 各項目にフォーカスがセットできるかを判断する。
                if (BatchStoreLabel.Enabled == true)
                {
                    // 新規の場合はバッチ店舗にフォーカスをセット
                    this.BatchStoreLabel.Focus();
                }
                else if (SlipDivisionCoverDropDownList.Enabled == true)
                {
                    // 新規の場合は伝票区分ドロップダウンリストにフォーカスをセット
                    this.SlipDivisionCoverDropDownList.Focus();
                }
                else
                {
                    // 新規の場合はバッチ番号にフォーカスをセット
                    this.BatchNumberTextBox.Focus();
                }
                // 2014.06.14 End   品質改善検証の指摘事項対応
            }
            // 2014.05.09 End   効率改善指摘事項の修正


            // 2014.06.14 Start 品質改善検証の指摘事項対応
            if (initialMode == 0)
            {
                // 伝票種別ドロップダウンリスト
                if (this.SlipTypeCoverDropDownList.Items.Count <= 1)
                {
                    this.SlipTypeCoverDropDownList.Enabled = false;
                }
                else
                {
                    this.SlipTypeCoverDropDownList.Enabled = true;
                }

                // 伝票区分ドロップダウンリスト
                if (this.SlipDivisionCoverDropDownList.Items.Count <= 1)
                {
                    this.SlipDivisionCoverDropDownList.Enabled = false;
                }
                else
                {
                    this.SlipDivisionCoverDropDownList.Enabled = true;
                }
            }
            // 2014.06.14 End   品質改善検証の指摘事項対応

        }
        #endregion

        #region RegisterAPVariable
        /// <summary>
        /// Registers the AP variable.
        /// </summary>
        /// <param name="apVariablesDataSet">The ap variables data set.</param>
        /// <param name="apVariableCode">The ap variable code.</param>
        private void RegisterAPVariable(APVariablesDataSet apVariablesDataSet, string apVariableCode)
        {
            APVariablesDataSet.UTFAPVariableRow row = apVariablesDataSet.UTFAPVariable.NewUTFAPVariableRow();
            row.APCorporationCode = UserProfile.CorporationCode;
            row.APVariableCode = apVariableCode;
            row.APVariableValue = string.Empty;
            apVariablesDataSet.UTFAPVariable.AddUTFAPVariableRow(row);
        }
        #endregion

        #region CheckSar
        /// <summary>
        /// Checks the sar.
        /// </summary>
        /// <param name="initialDisplayEntity">The initial display entity.</param>
        /// <param name="apVariableCode">The ap variable code.</param>
        /// <returns>On/Off flag</returns>
        private bool CheckSar(InitialDisplayEntity initialDisplayEntity, string apVariableCode)
        {
            return initialDisplayEntity.APAvaliableDataSet.UTFAPVariable.Any(r => r.APVariableCode == apVariableCode && r.APVariableValue == Constants.OffFlag);
        }
        #endregion

        // 2013.08.27 Start AP変数から情報を取得
        #region CheckApprovalFeatureDiv
        /// <summary>
        /// Checks the ApprovalFeatureDiv.
        /// </summary>
        /// <param name="initialDisplayEntity">The initial display entity.</param>
        /// <param name="apVariableCode">The ap variable code.</param>
        /// <returns>APVariableValue</returns>
        private String CheckApprovalFeatureDiv(InitialDisplayEntity initialDisplayEntity, string apVariableCode)
        {
            var query =
                from row in initialDisplayEntity.APAvaliableDataSet.UTFAPVariable
                where row.APVariableCode == apVariableCode
                select row;

            return query.FirstOrDefault().APVariableValue;
        }
        #endregion
        // 2013.08.27 End AP変数から情報を取得

        #region CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice
        /// <summary>
        /// Calculators the detail for diiference unit price and difference price.
        /// </summary>
        private void CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(bool isSave)
        {
            decimal totaldifferencePrice = 0;

            DsEntity dsEntity = this.DsEntitySession;

            string value = this.SlipTypeCoverDropDownList.SelectedValue;

            //20151020 start 明細行にて直接編集仕様変更　金額チェック追加　ri
            //売変の場合
            if (value.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            {
                if (isSave)
                {

                if (this.SlipDivisionCoverDropDownList.SelectedValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice))
                {
                    foreach (GridRecord record in this.SlipDetailGridView.Rows)
                    {

                        CoreCenterTextBox disposalChangeQtyLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");

                        CoreCenterTextBox newPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipNewPrice_Control");

                        CoreCenterTextBox oldPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipOldPrice_Control");

                        decimal newPrice = (string.IsNullOrEmpty(newPriceLabel.Text) ? 0 : decimal.Parse(newPriceLabel.Text));
                        decimal oldPrice = (string.IsNullOrEmpty(oldPriceLabel.Text) ? 0 : decimal.Parse(oldPriceLabel.Text));

                        if ((oldPrice != 0 || newPrice != 0) && newPrice >= oldPrice)
                        {
                            // Exceptionを発生させる
                            //this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB248, "新売単価", "旧売単価", record.Index + 1)));
                            throw RetailBusinessException.CreateInstance(ErrorKey.ERRCSARB248, "新売単価", "旧売単価", record.Index + 1);
                            
                        }
                    }
                }
                else
                {
                    foreach (GridRecord record in this.SlipDetailGridView.Rows)
                    {

                        CoreCenterTextBox disposalChangeQtyLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");

                        CoreCenterTextBox newPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipNewPrice_Control");

                        CoreCenterTextBox oldPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipOldPrice_Control");

                        decimal newPrice = (string.IsNullOrEmpty(newPriceLabel.Text) ? 0 : decimal.Parse(newPriceLabel.Text));
                        decimal oldPrice = (string.IsNullOrEmpty(oldPriceLabel.Text) ? 0 : decimal.Parse(oldPriceLabel.Text));
                        
                        if ((oldPrice != 0 || newPrice != 0) && newPrice <= oldPrice)
                        {
                            // Exceptionを発生させる
                            //this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB249, "新売単価", "旧売単価", record.Index + 1)));
                            throw RetailBusinessException.CreateInstance(ErrorKey.ERRCSARB249, "新売単価", "旧売単価", record.Index + 1);                           
                        }
                    }
                }
              }
                //20151020 end 明細行にて直接編集仕様変更　金額チェック追加　ri
                 
                foreach (GridRecord record in this.SlipDetailGridView.Rows)
                {

                    CoreCenterTextBox newPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipNewPrice_Control");

                    CoreCenterTextBox oldPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipOldPrice_Control");
                    // 2013.03.15 Start 数量欄を入力可能へ変更
                    //CoreCenterLabel disposalChangeQtyLabel = (CoreCenterLabel)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");
                    CoreCenterTextBox disposalChangeQtyLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");
                    // 2013.03.15 End   数量欄を入力可能へ変更


                    decimal newPrice = (string.IsNullOrEmpty(newPriceLabel.Text) ? 0 : decimal.Parse(newPriceLabel.Text));
                    decimal oldPrice = (string.IsNullOrEmpty(oldPriceLabel.Text) ? 0 : decimal.Parse(oldPriceLabel.Text));
                    decimal disposalChangeQty = (string.IsNullOrEmpty(disposalChangeQtyLabel.Text) ? 0 : decimal.Parse(disposalChangeQtyLabel.Text));


                    CoreCenterLabel diiferenceUnitPriceLabel = (CoreCenterLabel)WebDataGridUtil.FindControl(record, "DiiferenceUnitPrice");
                    CoreCenterLabel differencePriceLabel = (CoreCenterLabel)WebDataGridUtil.FindControl(record, "DifferencePrice");

                    decimal diiferenceUnitPrice = oldPrice - newPrice;
                    diiferenceUnitPriceLabel.Text = this.ConvertToDecimalFormat(Math.Abs(diiferenceUnitPrice));
                    differencePriceLabel.Text = this.ConvertToDecimalFormat(Math.Abs(diiferenceUnitPrice * disposalChangeQty));
                   
                    totaldifferencePrice += Math.Abs(diiferenceUnitPrice) * Math.Abs(disposalChangeQty);

                    decimal differencePrice = Math.Abs(newPrice * disposalChangeQty);

                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[record.Index]["DisposalChangeQty"] = disposalChangeQty; 
                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[record.Index]["SlipOldPrice"] = oldPrice;
                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[record.Index]["SlipNewPrice"] = newPrice;
                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[record.Index]["SalesChangePrice"] = differencePrice;


                    this.DifferenceTotalPrice.Text = this.ConvertToDecimalFormat(totaldifferencePrice);
                }
            }
            else if (value.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
            {
                foreach (GridRecord record in this.SlipDetailGridView.Rows)
                {
                    //CoreCenterTextBox SKUCdTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SKUCd_Control");

                    CoreCenterTextBox disposalChangeQtyLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");

                    CoreCenterTextBox SlipPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipPrice_Control");

                    CoreCenterLabel differencePriceLabel = (CoreCenterLabel)WebDataGridUtil.FindControl(record, "DifferencePrice");

                    decimal disposalChangeQty = (string.IsNullOrEmpty(disposalChangeQtyLabel.Text) ? 0 : decimal.Parse(disposalChangeQtyLabel.Text));

                    //if (!string.IsNullOrEmpty(SKUCdTextBox.Text) && disposalChangeQty == 0)
                    //{
                    //    // Exceptionを発生させる
                    //    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSTRB0193,　"数量は1以上", record.Index + 1)));
                    //    return;
                    //}

                    decimal SlipPrice = (string.IsNullOrEmpty(SlipPriceLabel.Text) ? 0 : decimal.Parse(SlipPriceLabel.Text));

                    decimal differencePrice = Math.Abs(SlipPrice * disposalChangeQty);

                    differencePriceLabel.Text = this.ConvertToDecimalFormat(differencePrice);

                    totaldifferencePrice += differencePrice;

                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[record.Index]["DisposalChangeQty"] = disposalChangeQty;

                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[record.Index]["SlipDisposalUnitPrica"] = SlipPrice; 

                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[record.Index]["DisposalPriceAmt"] = differencePrice;


                }

                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

                this.DifferenceTotalPrice.Text = this.ConvertToDecimalFormat(totaldifferencePrice);
            }
        }
        #endregion

        #region ConvertToCurencyFormat
        /// <summary>
        /// Converts to curency format.
        /// </summary>
        private void ConvertToCurencyFormat()
        {
            foreach (GridRecord record in this.SlipDetailGridView.Rows)
            {
                // 2013.03.15 Start 数量欄を入力可能へ変更
                //CoreCenterLabel transferQuantityLabel = (CoreCenterLabel)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");
                CoreCenterTextBox transferQuantityLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");
                // 2013.03.15 End   数量欄を入力可能へ変更
                CoreCenterTextBox slipOldPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipOldPrice_Control");
                CoreCenterTextBox slipNewPriceChangeQtyLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipNewPrice_Control");
                CoreCenterTextBox slipPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_SlipPrice_Control");

                // 2013.03.15 Start 数量欄を入力可能へ変更
                //transferQuantityLabel.Text = this.SetFormatToCurrency(transferQuantityLabel);
                // 2014.05.09 Start 効率改善指摘事項の修正
                //transferQuantityLabel.Text = string.IsNullOrEmpty(transferQuantityLabel.Text) ? this.ConvertToDecimalFormat(0) : this.ConvertToDecimalFormat(decimal.Parse(transferQuantityLabel.Text));
                transferQuantityLabel.Text = string.IsNullOrEmpty(transferQuantityLabel.Text) ? "0" : string.Format("{0:0}", decimal.Parse(transferQuantityLabel.Text));
                // 2014.05.09 End   効率改善指摘事項の修正
                // 2013.03.15 End   数量欄を入力可能へ変更
                slipOldPriceLabel.Text = this.SetFormatToCurrency(slipOldPriceLabel);
                slipNewPriceChangeQtyLabel.Text = this.SetTextFormatToCurrency(slipNewPriceChangeQtyLabel);
                slipPriceLabel.Text = this.SetFormatToCurrency(slipPriceLabel);
            }
        }
        #endregion

        #region SetFormatToCurrency
        /// <summary>
        /// Sets the format to curency.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>Currency format</returns>
        private string SetFormatToCurrency(CoreCenterTextBox label)
        {
            if (!string.IsNullOrEmpty(label.Text))
            {
                return this.ConvertToDecimalFormat(decimal.Parse(label.Text));
            }

            return this.ConvertToDecimalFormat(0);
        }
        #endregion

        #region SetTextFormatToCurrency
        /// <summary>
        /// Sets the format to curency.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>Currency format</returns>
        private string SetTextFormatToCurrency(CoreCenterTextBox label)
        {
            if (!string.IsNullOrEmpty(label.Text))
            {
                return this.ConvertToDecimalFormat(decimal.Parse(label.Text));
            }

            return this.ConvertToDecimalFormat(0);
        }
        #endregion

        #region ConvertToDecimalFormat

        /// <summary>
        /// Converts to decimal format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>decimal format</returns>
        private string ConvertToDecimalFormat(decimal value)
        {
            if (value > 0)
            {
                return string.Format("{0:##,##}", value);
            }

            return "0";
        }
        #endregion

        #region SetLineNoForGridView
        /// <summary>
        /// Sets the line no for grid view.
        /// </summary>
        /// <param name="gridView">The grid view.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <param name="pageNumber">The page number.</param>
        private void SetLineNoForGridView(CoreCenterDataGrid gridView, string controlName, int pageNumber)
        {
            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
            int maxLine = this.GetMaxLine(initialDisplayEntity);
            int lineNo = 1;
            foreach (GridRecord record in gridView.Rows)
            {
                CoreCenterLabel label = (CoreCenterLabel)WebDataGridUtil.FindControl(record, controlName);
                label.Text = ((pageNumber * maxLine) + lineNo).ToString();
                lineNo++;
            }
        }
        #endregion

        #region SetConfirmMessage
        /// <summary>
        /// Sets the confirm message.
        /// </summary>
        private void SetConfirmMessage()
        {
            // SAR_CheckDisplay_Off＝１
            // メッセージ：【MSGCSARI0021】表示後、明細削除のファサードを実施する
            this.DetailDeleateButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0021);

            // 変更が反映されないことを確認するメッセージ：【MSGCSARI0017】を表示する。
            this.ClearButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0017);

            // 保存がされていない場合はデータが反映されない確認メッセージ[MSGCSARI0017]を表示する。
            this.DetailClearButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0017);

            this.NextButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0022);

            this.ApprovalRequestButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0020);

            // 2014.06.14 Start 品質改善検証の指摘事項対応
            //this.NextCoverButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0017);
            this.NextCoverButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0027);
            // 2014.06.14 End   品質改善検証の指摘事項対応

            // ２．変更が反映されないことを確認するメッセージ：「」を表示する。
            this.ReturnButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0017);

            // 確認メッセージ【MSGCSARI0023】表示後、伝票登録のファサードを実施する
            this.SlipDeleateButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0023);

            // 2014.08.01 Start 破棄メッセージの統一
            //this.NewButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0017);
            this.NewButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0027);
            // 2014.08.01 End   破棄メッセージの統一
        }
        #endregion

        #region SetHiddenGridViewBySlipType
        /// <summary>
        /// Sets the type of the hidden grid view by slip.
        /// </summary>
        /// <param name="slipType">Type of the slip.</param>
        private void SetHiddenGridViewBySlipType(string slipType)
        {
            if (!string.IsNullOrEmpty(slipType) && slipType.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            {
                // 売変伝票の場合
                this.SlipDetailGridView.Columns[5].Hidden = false;

                if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "1")
                {
                    this.SlipDetailGridView.Columns[5].Header.FindControl("DetailSlipOldPriceSalesChangeRequiredLabel").Visible = true;

                }
                else
                {
                    this.SlipDetailGridView.Columns[5].Header.FindControl("DetailSlipOldPriceSalesChangeRequiredLabel").Visible = false;

                }
                this.SlipDetailGridView.Columns[6].Hidden = false;
                this.SlipDetailGridView.Columns[7].Hidden = true;
                // 2014.05.09 Start 効率改善指摘事項の修正
                // 事由を後ろへ移動したので差額が１つ前にきたから９→８へ変更
                //this.SlipDetailGridView.Columns[9].Hidden = false;
                this.SlipDetailGridView.Columns[8].Hidden = false;
                // 2014.05.09 End   効率改善指摘事項の修正
                // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
                //this.DetailAddSlipNewPriceSalesChangeCaptionLabel.Visible = true;
                //this.DetailAddSlipNewPriceSalesChange.Visible = true;
                //this.DetailAddSlipNewPriceDisposalCaptionLabel.Visible = false;
                //this.DetailAddSlipNewPriceDisposal.Visible = false;
                //this.DetailAddSlipNewPriceSalesChangeRequiredLabel.Visible = true;
                // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            }
            else
            {
                // 廃棄伝票の場合
                this.SlipDetailGridView.Columns[5].Hidden = true;
                this.SlipDetailGridView.Columns[6].Hidden = true;
                this.SlipDetailGridView.Columns[7].Hidden = false;

                if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "1")
                {
                   this.SlipDetailGridView.Columns[7].Header.FindControl("DetailSlipNewPriceDisposalRequiredLabel").Visible = true;

                }
                else
                {
                    this.SlipDetailGridView.Columns[7].Header.FindControl("DetailSlipNewPriceDisposalRequiredLabel").Visible = false;


                }
                // 2014.05.09 Start 効率改善指摘事項の修正
                // 事由を後ろへ移動したので差額が１つ前にきたから９→８へ変更
                //this.SlipDetailGridView.Columns[9].Hidden = true;
                this.SlipDetailGridView.Columns[8].Hidden = true;
                // 2014.05.09 End   効率改善指摘事項の修正
                // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
                //this.DetailAddSlipNewPriceSalesChangeCaptionLabel.Visible = false;
                //this.DetailAddSlipNewPriceSalesChange.Visible = false;
                //this.DetailAddSlipNewPriceDisposalCaptionLabel.Visible = true;
                //this.DetailAddSlipNewPriceDisposal.Visible = true;
                //this.DetailAddSlipNewPriceSalesChangeRequiredLabel.Visible = false;
                // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            }
        }
        #endregion

        #region ShowDataPager
        /// <summary>
        /// Shows the data pager.
        /// </summary>
        /// <param name="initialDisplayEntity">The initial display entity.</param>
        /// <param name="totalCount">The total count.</param>
        /// <param name="currentPage">The current page.</param>
        private void ShowDataPager(InitialDisplayEntity initialDisplayEntity, int totalCount, int currentPage)
        {
            string value = initialDisplayEntity.APVariableDispDataset.UTFAPVariable.AsEnumerable().Where(
                r => r.APVariableCode == APVariableListKey.SAR_SLIPNO_MAXLINE &&
                    r.APCorporationCode == UserProfile.CorporationCode).FirstOrDefault().APVariableValue;

            if (totalCount <= this.GetMaxLine(initialDisplayEntity))
            {
                this.PageDatePager.Visible = false;
            }
            else
            {
                this.PageDatePager.Visible = true;
            }

            this.PageDatePager.TotalRowCount = totalCount;
            this.PageDatePager.MaximumRows = this.GetMaxLine(initialDisplayEntity);
            this.PageDatePager.CurrentPageIndex = currentPage;
        }
        #endregion

        #region GetMaxLine
        /// <summary>
        /// Gets the max line.
        /// </summary>
        /// <param name="initialDisplayEntity">The initial display entity.</param>
        /// <returns>Max line</returns>
        private int GetMaxLine(InitialDisplayEntity initialDisplayEntity)
        {
            string value = initialDisplayEntity.APVariableDispDataset.UTFAPVariable.AsEnumerable().Where(
                r => r.APVariableCode == APVariableListKey.SAR_SLIPNO_MAXLINE &&
                    r.APCorporationCode == UserProfile.CorporationCode).FirstOrDefault().APVariableValue;

            return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
        }
        #endregion

        #region ClearItems
        /// <summary>
        /// Clears the items.
        /// </summary>
        /// <param name="initMode">The init mode.</param>
        private void ClearItems(int initMode)
        {
            if (initMode == 0)
            {
                //   2011.12.10 関根 クリアボタン押下のクリア処理修正 修正 開始
                //this.ClearCoverSheet();
                //this.ClearHeader();
                //this.ClearDetail();

                this.ClearDetail();
                // 2012.06.26 Start 入力エリアのクリア箇所を変更
                //this.ClearInputArea();
                // 2012.06.26 End   入力エリアのクリア箇所を変更

                DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
                dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Clear();
                dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AcceptChanges();
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

                // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
                //DsEntity dsTmpEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity);
                //dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Clear();
                //dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AcceptChanges();
                //this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, dsTmpEntity);
                // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
                //   2011.12.10 関根 クリアボタン押下のクリア処理修正 修正 終了

               

            }
            else if (initMode == 1)
            {
                this.ClearHeader();
                this.ClearDetail();
                // 2012.06.26 Start 入力エリアのクリア箇所を変更
                //this.ClearInputArea();
                // 2012.06.26 End   入力エリアのクリア箇所を変更
            }
            else if (initMode == 2)
            {
                DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);

                // 入力状況情報
                if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable.Rows.Count > 0)
                {
                    this.ShowSARCoverSheetMasterData(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows[0]);
                }

                // 伝票ヘッダー情報
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
                {
                    this.ShowSARSlipHeaderErrorData(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows[0]);
                    // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
                    //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
                    //// ダミーコードを取得
                    //this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
                    //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
                    // 2014.05.28 End   部門代表コードの部分を機能しないように修正
                }

                this.TargetDataTable = this.GetSARSlipHeaderErrorDataset(this.GetPageNumber()).SARSlipHeaderError;
                this.DataBindToinputStatusListGrid();

                this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
                this.DataBindToSlipDetailGridView();

                this.SetHiddenGridViewBySlipType(this.SlipTypeCoverDropDownList.SelectedValue);
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue, this.SlipTypeCoverDropDownList.SelectedValue);

                this.SetLineNoForGridView(this.inputStatusListGrid, "SARCoverSheetMaster_Number_Control", this.GetPageNumber());
                this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);
                this.ConvertToCurencyFormat();
                this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(false);
            }
        }
        #endregion

        #region ClearCoverSheet
        /// <summary>
        /// Clears the cover sheet.
        /// </summary>
        private void ClearCoverSheet()
        {
            this.BatchStoreLabel.Text = string.Empty;
            if (this.SlipTypeCoverDropDownList.Items.Count > 0)
            {
                this.SlipTypeCoverDropDownList.SelectedIndex = 0;
            }

            if (this.SlipDivisionCoverDropDownList.Items.Count > 0)
            {
                this.SlipDivisionCoverDropDownList.SelectedIndex = 0;
            }

            // 2014.05.16 Start 伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            //this.InputPageCountTextBox.Text = string.Empty;
            this.InputPageCountTextBox.Text = "1";
            // 2014.05.16 End   伝票起票の入力枚数はいなげや様では使用しないので非表示とする
            this.BatchNumberTextBox.Text = string.Empty;
            this.BatchDateDateTextBox.DateValue = null;
            // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            this.SalesChangeDate.DateValue = null;
            this.StoreTextBox.Text = string.Empty;
            this.StoreName.Text = string.Empty;
            // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする

            this.TargetDataTable = new SARSlipHeaderErrorInfoDataset().SARSlipHeaderError;
            this.DataBindToinputStatusListGrid();
        }
        #endregion

        #region ClearHeader
        /// <summary>
        /// Clears the header.
        /// </summary>
        private void ClearHeader()
        {
            this.TransferSlipNumberTextBox.Text = string.Empty;
            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            //this.StoreTextBox.Text = string.Empty;
            //this.StoreName.Text = string.Empty;
            // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            //if (!string.IsNullOrEmpty((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.IsHeadquarters)))
            //{
            //    // 本部ユーザーの場合
            //    // ※店舗ユーザーの場合は、そのままとする。
            //    this.StoreTextBox.Text = string.Empty;
            //    this.StoreName.Text = string.Empty;
            //}
            // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。
            this.Category1Cd.Text = string.Empty;
            this.Category1Name.Text = string.Empty;
            this.Category2Cd.Text = string.Empty;
            this.Category2Name.Text = string.Empty;
            // 2014.05.17 Start ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            //this.SalesChangeDate.DateValue = null;
            // 2014.05.17 End   ヘッダー部の店舗と日付はカバーシートの値を使用するので入力不可とする
            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            dsEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
            dsEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。
            // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
            //// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
            //// ダミーコードを取得
            //this.GetAndSaveSkuDummyCode(this.Category1Cd.Text, this.Category2Cd.Text);
            //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
            // 2014.05.28 End   部門代表コードの部分を機能しないように修正
        }
        #endregion

        #region ClearDetail
        /// <summary>
        /// Clears the detail.
        /// </summary>
        private void ClearDetail()
        {
            this.TargetDataSet = new SARSlipDetailErrorInfoDataSet();
            this.DataBindToSlipDetailGridView();
            // 2012.11.02 Start GridへBindしたあとの売変、廃棄のGrid調整を追加
            this.SetHiddenGridViewBySlipType((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue));
            // 2012.11.02 End   GridへBindしたあとの売変、廃棄のGrid調整を追加
            // 2012.06.26 Start クリアされていない箇所がある
            this.DifferenceTotalPrice.Text = "0";
            // 2012.06.26 End   クリアされていない箇所がある
            // 2012.06.26 Start セッションのDsTmpEntityを使用している箇所を少し変更
            // 更新フラグのクリア
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode, "0");
            // 2012.06.26 End   セッションのDsTmpEntityを使用している箇所を少し変更
            // 2012.06.26 Start 商品を選択した場合は商品コードの変更は不可とする。
            this.DetailAddSKUCd.Enabled = true;
            // 2012.06.26 End   商品を選択した場合は商品コードの変更は不可とする。
            // 2014.05.28 Start 明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
            // 選択中以外は選択ボタンを押せるようにする
            //this.DetailChoiceButton.Enabled = true;
            this.DetailDeleateButton.Enabled = true;
            // 選択中以外の場合は保存ボタンのメッセージを元に戻すする
            this.NextButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0022);
            // 2014.05.28 End   明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            dsEntity.SARSlipDetailErrorDataSet = new SARSlipDetailErrorInfoDataSet();
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

            // 2014.04.30 Start クリアボタン押下時は、検索前情報DataSet(OriginalDetailDataSet)も初期化する。
            // 新規（F7）ボタン押下時の後で、クリアボタン押下すると前伝票明細の情報が表示されてしまう為、初期化する。
            this.OriginalDetailDataSet = new SARSlipDetailErrorInfoDataSet();
            // 2014.04.30 End   クリアボタン押下時は、検索前情報DataSet(OriginalDetailDataSet)も初期化する。
        }
        #endregion

        // 2012.06.26 Start 入力エリアのクリア箇所を変更
        #region ClearInputArea (入力エリアのクリア)
        /// <summary>
        /// 入力エリアのクリア
        /// </summary>
        private void ClearInputArea()
        {
            //20151015 start 明細行直接編集できるように修正によりコメントアウト
            //this.DetailAddSKUCd.Text = string.Empty;
            //this.DetailAddTransferQuantity.Text = string.Empty;
            //// 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //this.DetailAddSlipOldPriceSalesChange.Text = string.Empty;
            //// 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            //this.DetailAddSlipNewPriceSalesChange.Text = string.Empty;
            //this.DetailAddSlipNewPriceDisposal.Text = string.Empty;
            //this.DetailAddReason.SelectedIndex = 0;
            //20151015 end 明細行直接編集できるように修正によりコメントアウト
            this.DetailAddSKUCd.Enabled = true;
            // 2014.05.28 Start 明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
            // 選択中以外は選択ボタンを押せるようにする
            //this.DetailChoiceButton.Enabled = true;
            this.DetailDeleateButton.Enabled = true;
            // 選択中以外の場合は保存ボタンのメッセージを元に戻すする
            this.NextButton.ConfirmMessage = ResourceUtil.GetMessage(MessageKey.MSGCSARI0022);
            // 2014.05.28 End   明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御

            //20151015 start 明細行直接編集できるように修正によりコメントアウト
            // 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
            //DetailAddSKUCd_TextChanged_ProductInfo_LogicalStockQty = string.Empty;
            //DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_SalePrice = string.Empty;
            // 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
            // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
            //DetailAddSKUCd_TextChanged_ProductInfo_ProductInfo_OldSalePrice = string.Empty;
            // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
            //20151015 end 明細行直接編集できるように修正によりコメントアウト

            //20151023 start 明細行直接編集により不要
            // 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
            //if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "3")
            //{
            //    // 変更不可能へ
            //    // 金額項目の画面制御
            //    DisplayChangePriceControl(false);
            //}
            // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする
            //20151023 end 明細行直接編集により不要
            
        }
        #endregion
        // 2012.06.26 End   入力エリアのクリア箇所を変更

        #region SetMainHeader
        /// <summary>
        /// Sets the main header.
        /// </summary>
        private void SetMainHeader()
        {
            // 2012.08.02 Start MHBranchでの修正分を標準Branchへマージ
            //this.SARPurchaseSlipHeader_SlipTypeCd.Text = this.SlipTypeCoverDropDownList.SelectedValue;
            //this.SARPurchaseSlipHeader_SlipTypeName.Text = this.SlipTypeCoverDropDownList.SelectedItem.Text;

            //this.SARPurchaseSlipHeader_SlipDivisionCd.Text = this.SlipDivisionCoverDropDownList.SelectedValue;
            //this.SARPurchaseSlipHeader_SlipDivisionName.Text = this.SlipDivisionCoverDropDownList.SelectedItem.Text;

            if (String.IsNullOrEmpty(this.SlipTypeCoverDropDownList.SelectedValue) == false)
            {
                this.SARPurchaseSlipHeader_SlipTypeCd.Text = this.SlipTypeCoverDropDownList.SelectedValue;
                this.SARPurchaseSlipHeader_SlipTypeName.Text = this.SlipTypeCoverDropDownList.SelectedItem.Text;
            }

            if (String.IsNullOrEmpty(this.SlipDivisionCoverDropDownList.SelectedValue) == false)
            {
                this.SARPurchaseSlipHeader_SlipDivisionCd.Text = this.SlipDivisionCoverDropDownList.SelectedValue;
                this.SARPurchaseSlipHeader_SlipDivisionName.Text = this.SlipDivisionCoverDropDownList.SelectedItem.Text;
            }
            // 2012.08.02 End   MHBranchでの修正分を標準Branchへマージ

            this.SARPurchaseSlipHeader_BatchNumberLabel.Text = this.BatchNumberTextBox.Text.Trim();
        }
        #endregion

        #region GetFacadeInstance
        /// <summary>
        /// Gets the facade instance.
        /// </summary>
        /// <returns>Facade Instance</returns>
        // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
        //private ISalesChangeDisposalSlipEntryFacade GetFacadeInstance()
        private ISalesChangeDisposalSlipEntryFacade GetFacadeInstance()
        // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。
        {
            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            //return (ISalesChangeDisposalSlipEntryFacade)ClassObjectUtil.GetFacadeInstance(
            //    SalesChangeDisposalSlipInputConstances.FacadeId,
            //    typeof(ISalesChangeDisposalSlipEntryFacade));
            return (ISalesChangeDisposalSlipEntryFacade)ClassObjectUtil.GetFacadeInstance(
                SalesChangeDisposalSlipInputConstances.FacadeId,
                typeof(ISalesChangeDisposalSlipEntryFacade));
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。
        }
        #endregion

        #region GetProductName
        /// <summary>
        /// GetProductName
        /// </summary>
        private void GetProductName()
        {
            if (this.GetSession("IsPostBack") != null)
            {
                bool isHostBack = (bool)this.GetSession("IsPostBack");

                string skuCd = (string)this.GetSession("ProductsSearch_SKUCd");

                if (isHostBack)
                {
                    ProductSearchDataSet productSearchDataSet = (ProductSearchDataSet)this.GetSession("ProductsSearch_ProductSearchDataSet");

                    if (productSearchDataSet != null)
                    {
                        string skuCdSearch = productSearchDataSet.ProductSearchInfo[0].SKUCd;
                        string storeNameSearch = productSearchDataSet.ProductSearchInfo[0].ProductsKanji;

                        if (this.GetSession("RowIndex") != null)
                        {
                            var rowSelected = this.DsEntitySession.SARSlipDetailErrorDataSet.SARSlipDetailError[int.Parse((string)this.GetSession("RowIndex"))];
                            rowSelected.SKUCd = skuCdSearch;
                            rowSelected.Attribute01 = storeNameSearch;

                            this.RemoveSession("RowIndex");
                            this.DataBindToSlipDetailGridView();
                        }
                        else
                        {
                            this.DetailAddSKUCd.Text = skuCdSearch;
                            this.SetSession("SKUCd", skuCdSearch);
                            this.SetSession(SalesChangeDisposalSlipInputSessionKey.SkuName, storeNameSearch);
                        }
                    }
                }

                this.RemoveSession("IsPostBack");
            }
        }
        #endregion

        #region GetSARSlipHeaderErrorDataset
        /// <summary>
        /// Gets the SAR slip header error dataset.
        /// </summary>
        /// <param name="tagetPageNumber">The taget page number.</param>
        /// <returns>SARSlipHeaderErrorInfoDataset</returns>
        private SARSlipHeaderErrorInfoDataset GetSARSlipHeaderErrorDataset(int tagetPageNumber)
        {
            SARSlipHeaderErrorInfoDataset headerDataSet = new SARSlipHeaderErrorInfoDataset();
            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            //DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity);
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。
            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);

            int startRow = tagetPageNumber * this.GetMaxLine(initialDisplayEntity);
            int endRow = (tagetPageNumber + 1) * this.GetMaxLine(initialDisplayEntity);
            for (int i = startRow; i < endRow; i++)
            {
                if (i < dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count)
                {
                    headerDataSet.SARSlipHeaderError.ImportRow(dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows[i]);
                }
            }

            return headerDataSet;
        }
        #endregion

        #region GetPageNumber
        /// <summary>
        /// Gets the page number.
        /// </summary>
        /// <returns>Selected page</returns>
        private int GetPageNumber()
        {
            if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.PageNumber) == null)
            {
                return 0;
            }
            else
            {
                return (int)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PageNumber);
            }
        }
        #endregion

        #region SetCommonScreenArgumentItems
        /// <summary>
        /// Set common  screen's argument items
        /// </summary>
        private void SetCommonScreenArgumentItems()
        {
            // 暫定修正 2011.11.29 Start 子画面対応
            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.StoreScreenArgumentItem, "BasicDate,CorporationCode,StoreCd,HeadquatersStoreDiv,SingleDoubleDiv,SearchConditionDiv");
            //this.SetSession(SalesChangeDisposalSlipInputSessionKey.CategoryScreenArgumentItem, "CategoryBasicDate,CorporationCode,Category1Cd,Category2Cd,CategoryType,SingleDoubleDiv,SearchConditionDiv");
            // 店舗検索子画面の呼出
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.StoreBasicDate, DateTime.Today);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.StoreCorporationCode, UserProfile.CorporationCode);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.StoreSingleDoubleDiv, "1");
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.StoreSearchConditionDiv, "3");
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.StoreCallDiv, "1");

            // 分類検索子画面の呼出
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CategoryBasicDate, DateTime.Today.ToShortDateString());
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CategoryCorporationCode, UserProfile.CorporationCode);
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CategoryType, "1");
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CategorySingleDoubleDiv, "1");
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CategorySearchConditionDiv, "3");
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CategoryCallDiv, "1");
            // 暫定修正 2011.11.29 End   子画面対応
        }
        #endregion

        #region ShowSARCoverSheetMasterData
        /// <summary>
        ///  指定された SARCoverSheetMaster テーブルの <paramref name="row"/> のデータを
        ///  対応する画面コントロールに設定します。
        /// </summary>
        /// <param name="row">
        ///  画面コントロールに表示する SARCoverSheetMaster テーブルのレコード。
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///  <paramref name="row"/> を null に設定して呼び出すことはできません。
        /// </exception>
        /// <exception cref="ArgumentException">
        ///  指定された <paramref name="row"/> は SARCoverSheetMaster テーブルの <see cref="DataRow"/> ではありません。
        /// </exception>
        private void ShowSARCoverSheetMasterData(CoverSheetMasterEntity coverSheetMasterEntity)
        {
            if (coverSheetMasterEntity == null)
            {
                throw new ArgumentNullException("coverSheetMasterEntity");
            }

            if (coverSheetMasterEntity.StoreCd != null) { this.BatchStoreLabel.Text = coverSheetMasterEntity.StoreCd; }
            if (coverSheetMasterEntity.SlipTypeCd != null) { this.SlipTypeCoverDropDownList.SelectedValue = coverSheetMasterEntity.SlipTypeCd; }
            if (coverSheetMasterEntity.SlipDiv != null) { this.SlipDivisionCoverDropDownList.SelectedValue = coverSheetMasterEntity.SlipDiv; }
            if (coverSheetMasterEntity.BatchNo != null) { this.BatchNumberTextBox.Text = coverSheetMasterEntity.BatchNo; }
            if (coverSheetMasterEntity.InputPageCount != null) { this.InputPageCountTextBox.Text = coverSheetMasterEntity.InputPageCount.ToString(); }
            if (coverSheetMasterEntity.BatchDate != null) { this.BatchDateDateTextBox.DateValue = DateTime.Parse(coverSheetMasterEntity.BatchDate.ToString()); }
        }
        #endregion

        #region SetItemsForSarHeaderErrorDataSet
        /// <summary>
        /// Sets the items for sar header error data set.
        /// </summary>
        /// <param name="dsEntity">The ds entity.</param>
        /// <param name="workEntity">workEntity</param>
        private void SetItemsForSarHeaderErrorDataSet(DsEntity dsEntity, WorkEntity workEntity)
        {
            decimal totalQty = 0;
            decimal slipOldTotalPrice = 0;
            decimal slipNewTotalPrice = 0;
            decimal slipTotalDisposalAmt = 0;
            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 開始
            int inputSequenceNo = 0;
            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 終了
            //   2012.01.21 関根 伝票取消時の障害対応 開始
            int updateCnt = 0;
            //   2012.01.21 関根 伝票取消時の障害対応 終了

            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            // 新規伝票フラグ True=新規伝票 False=既存伝票
            bool IsNewSlip = true;
            // 現在画面入力されている既存伝票か新規伝票かを判断する
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
            {
                DataRow[] slipHeaderErrorRow;
                slipHeaderErrorRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Select("SlipNo = '" + this.TransferSlipNumberTextBox.Text + "'");
                if (slipHeaderErrorRow == null || slipHeaderErrorRow.Length == 0)
                {
                    // 画面に入力されている伝票番号と違う場合はヘッダー情報を初期化
                    dsEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();
                }
                else
                {
                    // 既存伝票にする
                    IsNewSlip = false;
                }
            }
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

            if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
            {
                totalQty = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().
                    Where(x => x.Field<string>("LogicalDelFlg") == "0").Sum(x => x.DisposalChangeQty);

                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 開始
                if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
                {
                    inputSequenceNo = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
                }
                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 終了

                // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
                if (IsNewSlip == true)
                {
                    // 新規伝票の場合は入力ＳＥＱ番号を初期化
                    inputSequenceNo = 0;
                    // 新規伝票の場合は変更をコミットしておく
                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AcceptChanges();
                }
                // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

                //   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 開始
                int lineNo = 0;
                //   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 終了

                foreach (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detialRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows)
                {
                    // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
                    if (IsNewSlip == true)
                    {
                        if (detialRow.LogicalDelFlg == "1")
                        {
                            // 新規登録扱いなので論理削除分は削除する
                            detialRow.Delete();
                            continue;
                        }
                    }
                    // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

                    //   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 開始
                    // 削除フラグチェック（１以外：更新、１：削除）
                    if (detialRow.LogicalDelFlg != "1")
                    //   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 終了
                    {
                        slipOldTotalPrice += detialRow.DisposalChangeQty * detialRow.SlipOldPrice;
                        slipNewTotalPrice += detialRow.DisposalChangeQty * detialRow.SlipNewPrice;
                        slipTotalDisposalAmt += detialRow.DisposalChangeQty * detialRow.SlipDisposalUnitPrica;
                        //   2012.01.21 関根 伝票取消時の障害対応 開始
                        updateCnt++;
                        //   2012.01.21 関根 伝票取消時の障害対応 終了
                    }

                    //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 開始
                    detialRow.InputSequenceNo = (inputSequenceNo + 1).ToString();
                    //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 終了
                    // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
                    detialRow.UptakeSequenceNo = (inputSequenceNo + 1);
                    // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応

                    // 7/25 SUEYOSHI ADD 一意制約エラー対応
                    detialRow.InputSequenceLineNo = Convert.ToDecimal(lineNo + 1);

                    //detialRow.UptakeSequenceLineNo = Convert.ToDecimal(inputSequenceNo + 1);
                    detialRow.UptakeSequenceLineNo = Convert.ToDecimal(lineNo + 1);
                    // 7/25 SUEYOSHI END

                    //   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 開始
                    // 行削除を行った後に、行追加を行うと「一意制約エラー」となる為、再度明細番号を振り直し
                    lineNo++;
                    detialRow.SlipLineNo = lineNo.ToString();
                    //   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 終了
                }
            }

            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count > 0)
            {
                var updateHeaderRow = (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow)dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows[0];
                updateHeaderRow.InputBatchNo = this.BatchNumberTextBox.Text;
                updateHeaderRow.SlipDiv = this.SlipDivisionCoverDropDownList.SelectedValue;
                updateHeaderRow.SlipTypeCd = this.SlipTypeCoverDropDownList.SelectedValue;
                updateHeaderRow.SlipNo = this.TransferSlipNumberTextBox.Text;
                updateHeaderRow.DisposalChangeDate = (DateTime)this.SalesChangeDate.DateValue;
                updateHeaderRow.StoreCd = this.StoreTextBox.Text;
                updateHeaderRow.Category1Cd = this.Category1Cd.Text;
                // 2013.11.01 AST Del Start
                // 更新時は何も設定しないようにする
                #region 削除
                //updateHeaderRow.Category2Cd = this.Category2Cd.Text;
                #endregion
                // 2013.11.01 AST Del End
                updateHeaderRow.TotalQty = totalQty;
                updateHeaderRow.SlipOldTotalPrice = slipOldTotalPrice;
                updateHeaderRow.SlipNewTotalPrice = slipNewTotalPrice;
                updateHeaderRow.SlipTotalDisposalAmt = slipTotalDisposalAmt;
                updateHeaderRow.InputTime = DateTime.Now.ToString("HHmmss");
                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 開始
                updateHeaderRow.InputSequenceNo = (inputSequenceNo + 1).ToString();
                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 追加 終了
                // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
                updateHeaderRow.UptakeSequenceNo = (inputSequenceNo + 1);
                // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応

                //   2012.01.21 関根 伝票取消時の障害対応 開始
                if (updateCnt == 0)
                {
                    // 更新件数が０件の場合は、論理削除フラグＯＮ
                    updateHeaderRow.LogicalDelFlg = "1";
                }
                //   2012.01.21 関根 伝票取消時の障害対応 終了
            }
            else
            {
                var insertHeaderRow =
                                    (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow)dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.NewSARSlipHeaderErrorRow();

                insertHeaderRow.CorporationCode = UserProfile.CorporationCode;
                insertHeaderRow.InputMediaDiv = "1";
                insertHeaderRow.InputBatchNo = this.BatchNumberTextBox.Text;
                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
                //insertHeaderRow.InputSequenceNo = "1";
                insertHeaderRow.InputSequenceNo = (inputSequenceNo + 1).ToString();
                //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了
                insertHeaderRow.InputDate = (DateTime)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SystemDate);
                insertHeaderRow.InputTime = DateTime.Now.ToString("HHmmss");
                insertHeaderRow.DataType = string.Empty;
                // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
                //insertHeaderRow.UptakeSequenceNo = 1;
                insertHeaderRow.UptakeSequenceNo = (inputSequenceNo + 1);
                // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応
                insertHeaderRow.SlipDiv = this.SlipDivisionCoverDropDownList.SelectedValue;
                insertHeaderRow.SlipTypeCd = this.SlipTypeCoverDropDownList.SelectedValue;
                insertHeaderRow.SlipNo = this.TransferSlipNumberTextBox.Text;
                insertHeaderRow.DisposalChangeDate = (DateTime)this.SalesChangeDate.DateValue;
                // 2012.06.26 Start 店舗コードをセットする内容の間違え
                //insertHeaderRow.StoreCd = this.BatchStoreLabel.Text;
                insertHeaderRow.StoreCd = this.StoreTextBox.Text;
                // 2012.06.26 End   店舗コードをセットする内容の間違え
                insertHeaderRow.Category1Cd = this.Category1Cd.Text;
                insertHeaderRow.Category2Cd = this.Category2Cd.Text;
                insertHeaderRow.TotalQty = totalQty;
                insertHeaderRow.SlipOldTotalPrice = slipOldTotalPrice;
                insertHeaderRow.SlipNewTotalPrice = slipNewTotalPrice;
                insertHeaderRow.AutoSalesChangeFlg = string.Empty;
                insertHeaderRow.SlipTotalDisposalAmt = slipTotalDisposalAmt;
                insertHeaderRow.ApprovalRequester = string.Empty;
                insertHeaderRow.ApproverCd = string.Empty;
                insertHeaderRow.ApprovalStatus = "1";
                insertHeaderRow.ErrorDiv = string.Empty;
                insertHeaderRow.ErrorMessage = string.Empty;
                insertHeaderRow.CorrectionDiv = string.Empty;
                insertHeaderRow.RedBlackDiv = "0";
                insertHeaderRow.CorrectionDiv = "0";
                insertHeaderRow.ErrorDiv = "0";
                insertHeaderRow.FixDate = (DateTime)this.SalesChangeDate.DateValue;
                //   2012.01.21 関根 伝票取消時の障害対応 開始
                //insertHeaderRow.LogicalDelFlg = "0";
                if (updateCnt == 0)
                {
                    // 更新件数が０件の場合は、論理削除フラグＯＮ
                    insertHeaderRow.LogicalDelFlg = "1";
                }
                else
                {
                    insertHeaderRow.LogicalDelFlg = "0";
                }
                //   2012.01.21 関根 伝票取消時の障害対応 終了

                dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.AddSARSlipHeaderErrorRow(insertHeaderRow);
            }

            // 伝票日付の判断
            string slipDate = string.Empty;
            string periodDate = string.Format("{0:yyyy/MM}", workEntity.IVPeriodDate);

            // 在庫計上月度＜＝画面上の売変日
            string saleChangeDate = string.Format("{0:yyyy/MM}", this.SalesChangeDate.DateValue);
            if (periodDate.CompareTo(saleChangeDate) <= 0)
            {
                // 伝票日付＝画面上の売変日
                slipDate = this.SalesChangeDate.Text;
            }
            else
            {
                // 伝票日付＝在庫計上月度/01
                slipDate = periodDate + "/01";
            }

            // DsEntity.伝票ヘッダ更新Ds.伝票日付＝伝票日付
            foreach (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerRow in dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows)
            {
                headerRow.SlipDate = DateTime.Parse(slipDate);
            }

            // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
            // 伝票明細にも値をセット
            this.SetItemsForSarDetailErrorDataSet(dsEntity, workEntity);
            // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。
        }
        #endregion

        // 2012.06.26 Start １カバーシートに複数伝票登録できないので登録できるように修正。
        #region SetItemsForSarDetailErrorDataSet
        /// <summary>
        /// 伝票明細に値をセット
        /// </summary>
        /// <param name="dsEntity">The ds entity.</param>
        /// <param name="workEntity">workEntity</param>
        private void SetItemsForSarDetailErrorDataSet(DsEntity dsEntity, WorkEntity workEntity)
        {
            // 伝票ヘッダー又は伝票明細にレコードが無い場合は処理を抜ける
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count == 0
                || dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count == 0)
            {
                return;
            }

            DateTime systemDate = (DateTime)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SystemDate);

            string systemTime = DateTime.Now.ToString("HHmmss");

            SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerRow = (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow)dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows[0];
            // ヘッダーの内容を明細へセットする
            // ※画面のヘッダー入力内容が変更された場合に一致させるため
            foreach (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detialRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows)
            {
                // 伝票番号
                detialRow.SlipNo = headerRow.SlipNo;

                // 店舗
                detialRow.StoreCd = headerRow.StoreCd;

                // 売変・廃棄日
                detialRow.DisposalChangeDate = headerRow.DisposalChangeDate;

                // 取込処理日
                detialRow.InputDate = systemDate;

                // 取込処理時刻
                detialRow.InputTime = systemTime;
            }
        }
        #endregion
        // 2012.06.26 End   １カバーシートに複数伝票登録できないので登録できるように修正。

        // 2014.05.28 Start 部門代表コードの部分を機能しないように修正
        #region 
        ///// 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
        //#region GetAndSaveSkuDummyCode
        ///// <summary>
        ///// ダミーコード取得
        ///// </summary>
        ///// <param name="category1Cd">分類１コード</param>
        ///// <param name="category2Cd">分類２コード</param>
        //private void GetAndSaveSkuDummyCode(string category1Cd, string category2Cd)
        //{
        //    if (string.IsNullOrEmpty(category1Cd) && string.IsNullOrEmpty(category2Cd))
        //    {
        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode, string.Empty);
        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode, string.Empty);
        //        return;
        //    }
        //    if (this.SalesChangeDate.DateValue == null)
        //    {
        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode, string.Empty);
        //        this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode, string.Empty);
        //        return;
        //    }

        //    CategoryCdProductsSearchConditionEntity categoryCdProductsSearchConditionEntity = new CategoryCdProductsSearchConditionEntity();

        //    categoryCdProductsSearchConditionEntity.CorporationCode = UserProfile.CorporationCode;
        //    categoryCdProductsSearchConditionEntity.SearchBasicDate = this.SalesChangeDate.DateValue;
        //    categoryCdProductsSearchConditionEntity.Category1Cd = category1Cd;
        //    if (!string.IsNullOrEmpty(category2Cd))
        //    {
        //        categoryCdProductsSearchConditionEntity.Category2Cd = category2Cd;
        //    }

        //    using (STRProductDisclosureDataSet strProductDisclosureDataSet = new STRProductDisclosureDataSet())
        //    {
        //        this.GetFacadeInstance().GetDummyVendorCode(
        //            categoryCdProductsSearchConditionEntity,
        //            strProductDisclosureDataSet);

        //        if (!string.IsNullOrEmpty(category2Cd))
        //        {
        //            if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Count > 0)
        //            {
        //                this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode, strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].SKUCd);
        //            }
        //            else
        //            {
        //                this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category2DummyCode, string.Empty);
        //            }
        //        }
        //        else
        //        {
        //            if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Count > 0)
        //            {
        //                this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode, strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].SKUCd);
        //            }
        //            else
        //            {
        //                this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode, string.Empty);
        //            }
        //        }
        //    }
        //}
        //#endregion
        ///// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
        #endregion
        // 2014.05.28 End   部門代表コードの部分を機能しないように修正

        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        #region ChangeInputControl
        /// <summary>
        /// 入力項目の画面制御
        /// </summary>
        private void ChangeInputControl()
        {
            string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;

            if (!string.IsNullOrEmpty(slipTypeValue) && slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            {
                // 売変伝票の場合
                this.DetailAddSlipNewPriceDisposalRequiredLabel.Visible = false;
                this.DetailAddSlipNewPriceDisposalCaptionLabel.Visible = false;
                this.DetailAddSlipNewPriceDisposal.Visible = false;
                // 2014.03.18 Start 効率改善対応（Validatorコメント）
                //this.DetailAddSlipNewPriceDisposalCoreCenterValidator.Visible = false;
                // 2014.03.18 End   効率改善対応（Validatorコメント）

                this.DetailAddSlipOldPriceSalesChangeRequiredLabel.Visible = true;
                this.DetailAddSlipOldPriceSalesChangeCaptionLabel.Visible = true;
                this.DetailAddSlipOldPriceSalesChange.Visible = true;
                // 2014.03.18 Start 効率改善対応（Validatorコメント）
                //this.DetailAddSlipOldPriceSalesChangeCoreCenterValidator.Visible = true;
                // 2014.03.18 End   効率改善対応（Validatorコメント）

                this.DetailAddSlipNewPriceSalesChangeRequiredLabel.Visible = true;
                this.DetailAddSlipNewPriceSalesChangeCaptionLabel.Visible = true;
                this.DetailAddSlipNewPriceSalesChange.Visible = true;
                // 2014.03.18 Start 効率改善対応（Validatorコメント）
                //this.DetailAddSlipNewPriceSalesChangeCoreCenterValidator.Visible = true;
                // 2014.03.18 End   効率改善対応（Validatorコメント）

                // AP変数の値を元に入力エリアの売価項目に対して入力可、不可の制御を行う
                // （0：変更不可、1：変更可能、3：警告あり商品のみ可能）
                if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "0")
                {
                    // 変更不可
                    DisplayChangePriceControl(false);
                }
                else if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "1")
                {
                    // 変更可能
                    DisplayChangePriceControl(true);
                }
                else if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "3")
                {
                    // 商品で判断(初期値として不可とする)
                    DisplayChangePriceControl(false);
                }
                else
                {
                    // 以外
                    DisplayChangePriceControl(false);
                }
            }
            else
            {
                // 廃棄伝票の場合
                this.DetailAddSlipNewPriceDisposalRequiredLabel.Visible = true;
                this.DetailAddSlipNewPriceDisposalCaptionLabel.Visible = true;
                this.DetailAddSlipNewPriceDisposal.Visible = true;
                // 2014.03.18 Start 効率改善対応（Validatorコメント）
                //this.DetailAddSlipNewPriceDisposalCoreCenterValidator.Visible = true;
                // 2014.03.18 End   効率改善対応（Validatorコメント）

                this.DetailAddSlipOldPriceSalesChangeRequiredLabel.Visible = false;
                this.DetailAddSlipOldPriceSalesChangeCaptionLabel.Visible = false;
                this.DetailAddSlipOldPriceSalesChange.Visible = false;
                // 2014.03.18 Start 効率改善対応（Validatorコメント）
                //this.DetailAddSlipOldPriceSalesChangeCoreCenterValidator.Visible = false;
                // 2014.03.18 End   効率改善対応（Validatorコメント）

                this.DetailAddSlipNewPriceSalesChangeRequiredLabel.Visible = false;
                this.DetailAddSlipNewPriceSalesChangeCaptionLabel.Visible = false;
                this.DetailAddSlipNewPriceSalesChange.Visible = false;
                // 2014.03.18 Start 効率改善対応（Validatorコメント）
                //this.DetailAddSlipNewPriceSalesChangeCoreCenterValidator.Visible = false;
                // 2014.03.18 End   効率改善対応（Validatorコメント）

                // AP変数の値を元に入力エリアの売価項目に対して入力可、不可の制御を行う
                // （0：変更不可、1：変更可能、3：警告あり商品のみ可能）
                if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "0")
                {
                    // 変更不可
                    DisplayChangePriceControl(false);
                }
                else if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "1")
                {
                    // 変更可能
                    DisplayChangePriceControl(true);
                }
                else if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "3")
                {
                    // 商品で判断(初期値として不可とする)
                    DisplayChangePriceControl(false);
                }
                else
                {
                    // 以外
                    DisplayChangePriceControl(false);
                }
            }
        }
        #endregion
        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加

        // 20151023 start 明細行直接編集により不要
        //// 2012.10.17 Start 廃棄、売変の金額入力をＡＰ変数で制御するようにする
        //#region CheckAndChangeSalesChangePriceInput
        ///// <summary>
        ///// 売変の金額入力変更
        ///// </summary>
        //private void CheckAndChangeSalesChangePriceInput()
        //{
        //    SalesChangeSlipInputProductsReadPDAEntity salesChangeSlipInputProductsReadPDAEntity = new SalesChangeSlipInputProductsReadPDAEntity();

        //    salesChangeSlipInputProductsReadPDAEntity.CorporationCode = UserProfile.CorporationCode;
        //    salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate = this.SalesChangeDate.DateValue;
        //    salesChangeSlipInputProductsReadPDAEntity.StoreCd = this.StoreTextBox.Text;
        //    salesChangeSlipInputProductsReadPDAEntity.ProductsCd = this.DetailAddSKUCd.Text;
        //    salesChangeSlipInputProductsReadPDAEntity.RetailReductionCheckDiv = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SALESCHANGE_PRICE_REDUCTION_CHECK_DIV, UserProfile.CorporationCode);
        //    salesChangeSlipInputProductsReadPDAEntity.Message = string.Empty;

        //    // Facade呼出し
        //    ISalesChangeSlipInputPDANewFacade salesChangeSlipInputPDAFacade =
        //        (ISalesChangeSlipInputPDANewFacade)ClassObjectUtil.GetFacadeInstance(SalesChangeDisposalSlipInputConstances.SalesChangeSlipInputPDANewFacadeClassId, typeof(ISalesChangeSlipInputPDANewFacade));

        //    // 売変入力ＰＤＡ商品検索
        //    int resultValue = SalesChangeDisposalSlipInputConstances.NormalStatus;
        //    resultValue = salesChangeSlipInputPDAFacade.SearchSalesChangeSlipProductPDA(salesChangeSlipInputProductsReadPDAEntity);

        //    // 戻り値の判定
        //    if (resultValue == SalesChangeDisposalSlipInputConstances.ErrorStatus)
        //    {
        //        // 戻り値がエラーの場合は変更不可能へ
        //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //        // 金額項目の画面制御
        //        DisplayChangePriceControl(false);
        //        this.SetMessage(new MessageContainer(salesChangeSlipInputProductsReadPDAEntity.Message));
        //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //        return;
        //    }
        //    // 戻りメッセージの判定
        //    if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.Message) == false)
        //    {
        //        // 戻りのメッセージがある場合は変更可能へ
        //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //        // 金額項目の画面制御
        //        DisplayChangePriceControl(true);
        //        this.SetMessage(new MessageContainer(salesChangeSlipInputProductsReadPDAEntity.Message));
        //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //        return;
        //    }

        //    // 以外は変更不可能へ
        //    // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //    // 金額項目の画面制御
        //    DisplayChangePriceControl(false);
        //    // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //}
        //#endregion

        //#region CheckAndChangeDisposalPriceInput
        ///// <summary>
        ///// 廃棄の金額入力変更
        ///// </summary>
        //private void CheckAndChangeDisposalPriceInput()
        //{
        //    DisposalSlipInputProductsReadPDAEntity disposalSlipInputProductsReadPDAEntity = new DisposalSlipInputProductsReadPDAEntity();

        //    disposalSlipInputProductsReadPDAEntity.CorporationCode = UserProfile.CorporationCode;
        //    disposalSlipInputProductsReadPDAEntity.SalesChangeDate = this.SalesChangeDate.DateValue;
        //    disposalSlipInputProductsReadPDAEntity.StoreCd = this.StoreTextBox.Text;
        //    disposalSlipInputProductsReadPDAEntity.ProductCd = this.DetailAddSKUCd.Text;
        //    disposalSlipInputProductsReadPDAEntity.SalePriceDisp = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_STOCKSALEPRICEDISPLAYFLAG, UserProfile.CorporationCode);
        //    disposalSlipInputProductsReadPDAEntity.SalePriceRoundOffMethod = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SELLPRICEROUNDINGMETHOD, UserProfile.CorporationCode);
        //    disposalSlipInputProductsReadPDAEntity.Message = string.Empty;

        //    // Facade呼出し
        //    IDisposalSlipInputPDANewFacade disposalSlipInputPDAFacade
        //        = (IDisposalSlipInputPDANewFacade)ClassObjectUtil.GetFacadeInstance(SalesChangeDisposalSlipInputConstances.DisposalSlipInputPDANewFacadeClassId, typeof(IDisposalSlipInputPDANewFacade));

        //    // 廃棄入力ＰＤＡ商品検索
        //    int resultValue = SalesChangeDisposalSlipInputConstances.NormalStatus;
        //    resultValue = disposalSlipInputPDAFacade.SearchDisposalSlipProductPDA(ref disposalSlipInputProductsReadPDAEntity);

        //    // 戻り値の判定
        //    if (resultValue == SalesChangeDisposalSlipInputConstances.ErrorStatus)
        //    {
        //        // 戻り値がエラーの場合は変更不可能へ
        //        // 金額項目の画面制御
        //        DisplayChangePriceControl(false);
        //        this.SetMessage(new MessageContainer(disposalSlipInputProductsReadPDAEntity.Message));
        //        return;
        //    }
        //    // 戻りメッセージの判定
        //    if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.Message) == false)
        //    {
        //        // 戻りのメッセージがある場合は変更可能へ
        //        // 金額項目の画面制御
        //        DisplayChangePriceControl(true);
        //        this.SetMessage(new MessageContainer(disposalSlipInputProductsReadPDAEntity.Message));
        //        return;
        //    }

        //    // 以外は変更不可能へ
        //    // 金額項目の画面制御
        //    DisplayChangePriceControl(false);
        //}
        //#endregion
        // 20151023 end 明細行直接編集により不要

        //2015.10.23 start 明細行直接編集によりコメントアウト
        //#region DisplayChangePriceControl
        ///// <summary>
        ///// 金額項目の画面制御
        ///// </summary>
        ///// <param name="display">表示(true=表示、false=非表示)</param>
        //private void DisplayChangePriceControl(bool display)
        //{
        //    string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;

        //    if (!string.IsNullOrEmpty(slipTypeValue) && slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
        //    {
        //        // 売変伝票の場合
        //        // 2012.11.01 Start 売変伝票の場合に旧売価項目を追加
        //        // 入力コントロール
        //        this.DetailAddSlipOldPriceSalesChange.Enabled = display;
        //        // 2014.03.18 Start 効率改善対応（Validatorコメント）
        //        //// 単項目チェック用？
        //        //this.DetailAddSlipOldPriceSalesChangeCoreCenterValidator.Visible = display;
        //        // 2014.03.18 End   効率改善対応（Validatorコメント）
        //        // 必須マーク
        //        this.DetailAddSlipOldPriceSalesChangeRequiredLabel.Visible = display;
        //        // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
        //    }
        //    else
        //    {
        //        // 廃棄伝票の場合
        //        // 入力コントロール
        //        this.DetailAddSlipNewPriceDisposal.Enabled = display;
        //        // 2014.03.18 Start 効率改善対応（Validatorコメント）
        //        //// 単項目チェック用？
        //        //this.DetailAddSlipNewPriceDisposalCoreCenterValidator.Visible = display;
        //        // 2014.03.18 End   効率改善対応（Validatorコメント）
        //        // 必須マーク
        //        this.DetailAddSlipNewPriceDisposalRequiredLabel.Visible = display;
        //    }
        //}
        //#endregion
        // 2012.10.17 End   廃棄、売変の金額入力をＡＰ変数で制御するようにする

        //2015.10.23 end 明細行直接編集によりコメントアウト

        // 2013.08.21 Start グリッドの入力値反映
        #region SaveDisplayControl
        /// <summary>
        /// 表示項目の保存
        /// </summary>
        private void SaveDisplayControl()
        {
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            int i = 0;
            // グリッド項目の保存
            foreach (GridRecord record in this.SlipDetailGridView.Rows)
            {
                // 数量の項目保存
                CoreCenterTextBox disposalChangeQtyLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");
                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detailRow = (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow)dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[i];
                detailRow.DisposalChangeQty = (string.IsNullOrEmpty(disposalChangeQtyLabel.Text) ? 0 : decimal.Parse(disposalChangeQtyLabel.Text));
                // 数量の変更に合わせて金額も再計算
                detailRow.SalesChangePrice = detailRow.DisposalChangeQty * detailRow.SlipNewPrice;
                detailRow.DisposalPriceAmt = detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica;
                i++;
            }
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
        }
        #endregion

        #region SaveDisplayControlTmp
        /// <summary>
        /// 表示項目の保存(tmpセッション)
        /// <param name="editFlg">編集行を修正するか否か(true=修正する、false=修正しない)</param>
        /// </summary>
        private void SaveDisplayControlTmp(bool editFlg)
        {
            int i = 0;
            DsEntity dsTmpEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity);
            int rowIndex = int.Parse((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.RowIndex));
            foreach (GridRecord record in this.SlipDetailGridView.Rows)
            {
                // 修正する場合(グリッドに編集行がない場合）
                if (editFlg)
                {
                    // 編集行をチェックして飛ばす
                    if (rowIndex == i)
                    {
                        i++;
                    }
                }
                // 画面上の数量保存
                CoreCenterTextBox disposalChangeQtyLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");
                SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detailRowTmp = (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow)dsTmpEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[i];
                detailRowTmp.DisposalChangeQty = (string.IsNullOrEmpty(disposalChangeQtyLabel.Text) ? 0 : decimal.Parse(disposalChangeQtyLabel.Text));
                // 数量の変更に合わせて金額も再計算
                detailRowTmp.SalesChangePrice = detailRowTmp.DisposalChangeQty * detailRowTmp.SlipNewPrice;
                detailRowTmp.DisposalPriceAmt = detailRowTmp.DisposalChangeQty * detailRowTmp.SlipDisposalUnitPrica;
                i++;
            }
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity, dsTmpEntity);
        }
        #endregion
        // 2013.08.21 End グリッドの入力値反映

        // 2014.03.18 Start 効率改善対応（Validatorコメント）
        // ※Validation処理をコメントした事により、入力値チェック処理を行う
        #region CheckCoverSheetInput(カバーシート情報チェック)
        /// <summary>
        /// カバーシート情報の入力チェックを行う
        /// </summary>
        private bool CheckCoverSheetInput()
        {
            // バッチ店舗
            if (this.BatchStoreLabel.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("StoreCd", this.BatchStoreLabel.Text, "バッチ店舗");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.BatchStoreLabel.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // 伝票種別
            if (this.SlipTypeCoverDropDownList.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("SlipTypeCd", this.SlipTypeCoverDropDownList.SelectedValue, "伝票種別");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.SlipTypeCoverDropDownList.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // 伝票区分
            if (this.SlipDivisionCoverDropDownList.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("SlipDiv", this.SlipDivisionCoverDropDownList.SelectedValue, "伝票区分");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.SlipDivisionCoverDropDownList.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // バッチ番号
            if (this.BatchNumberTextBox.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("BatchNo", this.BatchNumberTextBox.Text, "バッチ番号");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.BatchNumberTextBox.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // 入力枚数
            if (this.InputPageCountTextBox.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("InputPageCount", this.InputPageCountTextBox.Text, "入力枚数");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.InputPageCountTextBox.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // バッチ日付
            if (this.BatchDateDateTextBox.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("BatchDate", this.BatchDateDateTextBox.Text, "バッチ日付");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.BatchDateDateTextBox.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region CheckSlipHeaderInfoInput(伝票ヘッダ情報入力チェック)
        /// <summary>
        /// 伝票ヘッダ情報の入力チェックを行う
        /// </summary>
        private bool CheckSlipHeaderInfoInput()
        {
            // 伝票番号
            if (this.TransferSlipNumberTextBox.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("SlipNo", this.TransferSlipNumberTextBox.Text, "伝票番号");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.TransferSlipNumberTextBox.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }

                // 2014.05.17 Start ヘッダー部の伝票番号を入力不可とする
                if (string.IsNullOrEmpty(this.TransferSlipNumberTextBox.Text))
                {
                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0274, "伝票番号がセットされておりません。新規ボタンを押下")));
                    this.DetailAddSKUCd.Focus();
                    return true;
                }
            }
            
            // 2014.05.17 End   ヘッダー部の伝票番号を入力不可とする

            // 店舗
            if (this.StoreTextBox.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("StoreCd", this.StoreTextBox.Text, "店舗");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.StoreTextBox.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }

                // 2014.04.30 Start 店舗コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
                // 店舗名称が空白の場合は、マスタ未存在と判断しエラーとする。
                if (string.IsNullOrEmpty(this.StoreName.Text) || string.IsNullOrEmpty(this.StoreTextBox.Text))
                {
                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0179)));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.StoreTextBox.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
                // 2014.04.30 End   店舗コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
            }

            // 売変または廃棄日
            if (this.SalesChangeDate.Enabled == true)
            {

                ValidationResult result = ValidatorUtil.IsValid("DisposalChangeDate", this.SalesChangeDate.Text, "売変または廃棄日");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.SalesChangeDate.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }

            }

            // 部門
            if (this.Category1Cd.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("Category1Cd", this.Category1Cd.Text, "部門");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.Category1Cd.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
                // 2014.04.30 Start 部門コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
                // 部門名称が空白の場合は、マスタ未存在と判断しエラーとする。
                if (string.IsNullOrEmpty(this.Category1Name.Text) || string.IsNullOrEmpty(this.Category1Cd.Text))
                {
                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0184)));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.Category1Cd.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
                // 2014.04.30 End   部門コード未存在の場合は、画面上の名称もクリアする（保存ボタン押下時でチェックを行う為）
            }

            // 分類２
            if (this.Category2Cd.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("Category2Cd", this.Category2Cd.Text, "分類２");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.Category2Cd.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region CheckSlipDeatilInfoInput(伝票明細情報チェック)
        /// <summary>
        /// 伝票明細情報の入力チェックを行う
        /// </summary>
        private bool CheckSlipDeatilInfoInput()
        {
            // 商品コード
            if (this.DetailAddSKUCd.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("SKUCd", this.DetailAddSKUCd.Text, "商品コード");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.DetailAddSKUCd.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // 数量
            if (this.DetailAddTransferQuantity.Enabled == true)
            {
                // 2014.08.04 Start 数量チェックのValidator対応
                //ValidationResult result = ValidatorUtil.IsValid("DisposalChangeQty", this.DetailAddTransferQuantity.Text, "数量");
                // ＰＤＡ廃棄でエラーとなってしまう為、Integer用の単項目を使用する（画面では小数点入力エラーとする）
                ValidationResult result = ValidatorUtil.IsValid("DisposalChangeQty_Display", this.DetailAddTransferQuantity.Text, "数量");
                // 2014.08.04 End   数量チェックのValidator対応
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.DetailAddTransferQuantity.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // 旧売単価
            if (this.DetailAddSlipOldPriceSalesChange.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("SlipDisposalUnitPrica", this.DetailAddSlipOldPriceSalesChange.Text, "旧売単価");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.DetailAddSlipOldPriceSalesChange.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // 新売単価
            if (this.DetailAddSlipNewPriceSalesChange.Enabled == true)
            {
                ValidationResult result = ValidatorUtil.IsValid("SlipDisposalUnitPrica", this.DetailAddSlipNewPriceSalesChange.Text, "新売単価");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.DetailAddSlipNewPriceSalesChange.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            // 売単価
            if (this.DetailAddSlipNewPriceDisposal.Enabled == true)
            {
                // 2014.05.27 Start Gridの数量チェックを強化
                //ValidationResult result = ValidatorUtil.IsValid("SlipOldPrice", this.DetailAddSlipNewPriceDisposal.Text, "売単価");
                // チェックは新や旧の売単価に合わせる
                ValidationResult result = ValidatorUtil.IsValid("SlipDisposalUnitPrica", this.DetailAddSlipNewPriceDisposal.Text, "売単価");
                // 2014.05.27 End   Gridの数量チェックを強化
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
                    // 2014.05.09 Start 効率改善指摘事項の修正
                    this.DetailAddSlipNewPriceDisposal.Focus();
                    // 2014.05.09 End   効率改善指摘事項の修正
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region CheckSlipDeatilGridInput(伝票明細グリッドチェック)
        /// <summary>
        /// 伝票明細グリッドの入力チェックを行う
        /// </summary>
        private bool CheckSlipDeatilGridInput()
        {
            // グリッド項目の保存
            foreach (GridRecord record in this.SlipDetailGridView.Rows)
            {
                // グリッドから数量取得
                CoreCenterTextBox disposalChangeQtyLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferQuantity_Control");
                CoreCenterLabel lineNoLabel = (CoreCenterLabel)WebDataGridUtil.FindControl(record, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control");
                // 数量
                if (disposalChangeQtyLabel.Enabled == true)
                {
                    // 2014.08.04 Start 数量チェックのValidator対応
                    //ValidationResult result = ValidatorUtil.IsValid("DisposalChangeQty", disposalChangeQtyLabel.Text, "数量");
                    // ＰＤＡ廃棄でエラーとなってしまう為、Integer用の単項目を使用する（画面では小数点入力エラーとする）
                    ValidationResult result = ValidatorUtil.IsValid("DisposalChangeQty_Display", disposalChangeQtyLabel.Text, "数量");
                    // 2014.08.04 End   数量チェックのValidator対応
                    if (!result.IsSuccess)
                    {
                        // 2014.04.30 Start 行削除のエラー対応（行削除→クリア→行削除の順に行うと同一伝票が新規で登録されてしまう）
                        //this.SetMessage(new MessageContainer(lineNoLabel.Text + "行目：" + result.ErrorMessage));
                        //return true;

                        if (!disposalChangeQtyLabel.Text.Equals("0"))
                        {
                            // 数量０入力以外はエラーメッセージを表示
                            this.SetMessage(new MessageContainer(lineNoLabel.Text + "行目：" + result.ErrorMessage));
                            // 2014.05.09 Start 効率改善指摘事項の修正
                            //this.DetailAddSKUCd.Focus();
                            // 2014.05.09 End   効率改善指摘事項の修正
                            return true;
                        }
                        // 2014.04.30 End   行削除のエラー対応（行削除→クリア→行削除の順に行うと同一伝票が新規で登録されてしまう）
                    }
                }
            }

            return false;
        }
        #endregion
        // 2014.03.18 End   効率改善対応（Validatorコメント）

        // 2014.05.09 Start 効率改善指摘事項の修正
        #region CheckSlipDeatilGridCategory1(伝票明細グリッドのチェック)
        /// <summary>
        /// 伝票明細グリッドのチェックを行う
        /// </summary>
        private void CheckSlipDeatilGridCategory1()
        {
            // 2014.05.28 Start 明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
            string updateMode = "0";
            if (this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode) != null)
            {
                updateMode = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.UpdateMode);
            }

            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            // Gridに明細が１件以上存在する場合または選択し変更中の場合は部門コードは変更不可とする

                if (this.SlipDetailGridView.Rows.Count != 0 || updateMode == "1")
                // 2014.05.28 End   明細にチェックマークを付け、選択ボタンで選択を連続で行うと明細削除になってしまうので１回の選択までとするように制御
                {
                    // 明細が存在する場合は部門コードは変更不可とする
                    if (this.Category1Cd.Enabled == true)
                    {
                        this.Category1Cd.Enabled = false;
                        this.Category1ReferButton.Enabled = false;
                    }
                    //20151021 start 明細行直接編集へ仕様変更　
                    if (this.DetailDeleateButton.Enabled == false)
                    {
                        this.DetailDeleateButton.Enabled = true;
                        this.SelectAllButton.Enabled = true;
                        this.CancelAllButton.Enabled = true;
                        this.ReCulculateButton.Enabled = true;
                        this.NextButton.Enabled = true;
                    }
                    //20151021 end 明細行直接編集へ仕様変更　

                }
                else
                {
                    //20151021 start 明細行直接編集へ仕様変更　

                    // 明細が存在しない場合は部門コードは変更可とする
                    //if (this.Category1Cd.Enabled == false)
                    //{
                    //    this.Category1Cd.Enabled = true;
                    //    this.Category1ReferButton.Enabled = true;
                    //}
                    if (this.DetailDeleateButton.Enabled == true)
                    {
                        this.DetailDeleateButton.Enabled = false;
                        this.SelectAllButton.Enabled = false;
                        this.CancelAllButton.Enabled = false;
                        this.ReCulculateButton.Enabled = false;
                        this.NextButton.Enabled = false;
                    }
                    //20151021 end 明細行直接編集へ仕様変更　

                }

            }
        #endregion
        // 2014.05.09 End   効率改善指摘事項の修正

        #region 20151015 明細行で直接編集できるように新規追加処理

        #region DetailAddSKUCd_TextChanged (商品コード入力)
        /// <summary>
        /// 商品コード入力
        /// </summary>
        /// <param name="sender">イベント送信元のオブジェクト。</param>
        /// <param name="e">イベント引数。</param>
        protected void DetailAddSKUCd_TextChanged(object sender, EventArgs e)
        {
            this.SaveDropDownList();
            GridRecord gridRecord = ((Infragistics.Web.UI.GridControls.GridRecordItem)((Infragistics.Web.UI.TemplateContainer)(((CoreCenterTextBox)sender).NamingContainer)).Item).Row;

            CoreCenterTextBox SKUCdTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SKUCd_Control");

            CoreCenterTextBox TransferQuantityTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_TransferQuantity_Control");

            CoreCenterTextBox SlipPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SlipPrice_Control");

            CoreCenterTextBox SlipOldPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SlipOldPrice_Control");

            CoreCenterTextBox SlipNewPriceTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SlipNewPrice_Control");

            CoreCenterDropDownList ReasonDropDownList = (CoreCenterDropDownList)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Reason_Control");

            CoreCenterLabel SKUNameLabel = (CoreCenterLabel)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SKUName_Control");

            CoreCenterLabel differencePriceLabel = (CoreCenterLabel)WebDataGridUtil.FindControl(gridRecord, "DifferencePrice");

            decimal disposalChangeQty = (string.IsNullOrEmpty(TransferQuantityTextBox.Text) ? 0 : decimal.Parse(TransferQuantityTextBox.Text));

            //2015.12.03 ダミーコード取得時に入力した金額が削除されるため、コメントアウト ri
            //TransferQuantityTextBox.Text = string.Empty;

            SKUNameLabel.Text = string.Empty;

            SlipPriceLabel.Text = string.Empty;

            differencePriceLabel.Text = string.Empty;

            SlipOldPriceLabel.Text = string.Empty;

            SlipNewPriceTextBox.Text = string.Empty;

            // 商品コードが未入力の場合は部門代表としてダミーコードをセットする
            if (string.IsNullOrEmpty(SKUCdTextBox.Text) && disposalChangeQty > 0)
                {
                    string category1DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode);
                    if (string.IsNullOrEmpty(category1DummyCd))
                    {
                        // ダミーコードが取得できていない場合はダミーコードを取得する
                        this.GetAndSaveSkuDummyCode(this.Category1Cd.Text);
                        // ダミーコードの入れ直し
                        category1DummyCd = (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode);
                    }
                   
                        SKUCdTextBox.Text = category1DummyCd;

                }
                //// 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
   
             // 商品コードの必須チェックを行い、未入力が確認されたら処理をストップする。
            if (string.IsNullOrEmpty(SKUCdTextBox.Text))
                {
                    //2015.12.03 start ダミーコード取得失敗時処理を停止する
                    this.SetSession(SalesChangeDisposalSlipInputSessionKey.CodeStuts, false);
                    //2015.12.03 end ダミーコード取得失敗時処理を停止する
                    return;
                }

            // 商品コードをチェックする。
            if (!string.IsNullOrEmpty(SKUCdTextBox.Text))
            {
                ValidationResult result = ValidatorUtil.IsValid("SKUCd", this.DetailAddSKUCd.Text, "商品コード");
                if (!result.IsSuccess)
                {
                    this.SetMessage(new MessageContainer(result.ErrorMessage));
          
                    SKUCdTextBox.Focus();

                    //2015.12.03 start ダミーコード取得失敗時処理を停止する
                    this.SetSession(SalesChangeDisposalSlipInputSessionKey.CodeStuts, false);
                    //2015.12.03 end ダミーコード取得失敗時処理を停止する

                    return;
                }
                
            }

            // バッチ日付の必須チェックを行い、未入力が確認されたらエラーメッセージ：【ERRCSARB0206】をスローする。
            if (string.IsNullOrEmpty(this.BatchDateDateTextBox.DateText))
            {
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0206, "バッチ日付")));
                this.BatchDateDateTextBox.Focus();

                //2015.12.03 start ダミーコード取得失敗時処理を停止する
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.CodeStuts, false);
                //2015.12.03 end ダミーコード取得失敗時処理を停止する

                return;
            }

            // 伝票ヘッダー情報チェック
            if (CheckSlipHeaderInfoInput().Equals(true))
            {
                //2015.12.03 start ダミーコード取得失敗時処理を停止する
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.CodeStuts, false);
                //2015.12.03 end ダミーコード取得失敗時処理を停止する

                return;
            }

            string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;

            ItemInfoDataset itemInforDataSet = new ItemInfoDataset();
            WorkEntity workEntity = (WorkEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);

            workEntity.IVGetDiv = "0";
            workEntity.ItemCd = SKUCdTextBox.Text;
            workEntity.ChangeDate = this.SalesChangeDate.DateValue;

            // 商品情報取得 -->
            workEntity.StoreCd = this.StoreTextBox.Text;
            workEntity.IVGetSystemDiv = "IV";
            workEntity.IVGetDiv = "1";
            workEntity.Subsystem = "SA";
            workEntity.Category1Cd = this.Category1Cd.Text;
            workEntity.BatchDate = Convert.ToDateTime(this.BatchDateDateTextBox.Text);
            // 商品情報取得 <--

            // 売単価のクリア 追加 開始
            workEntity.ProductInfo_SalePrice = string.Empty;
            // 売単価のクリア 追加 終了

            // 2013.08.27 Start 売価取得区分設定
            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
            if (SalesChangeDisposalSlipInputConstances.Disposal.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
            {
                // 現在入力中の伝票種別が廃棄伝票の場合
                workEntity.SalePriceGetKbn = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_UNIT_PRICE_FETCH_METHOD_DISPOSAL);
            }
            else if (SalesChangeDisposalSlipInputConstances.SalesChange.Equals(this.SlipTypeCoverDropDownList.SelectedValue))
            {
                // 現在入力中の伝票種別が売変伝票の場合
                workEntity.SalePriceGetKbn = this.CheckApprovalFeatureDiv(initialDisplayEntity, APVariableListKey.SAR_UNIT_PRICE_FETCH_METHOD_SALE);
            }
            // 2013.08.27 End   売価取得区分設定

            // 商品情報取得のファサード実行(BSFCSAR050800028)
            this.GetFacadeInstance().FillItemInfoNew(workEntity, itemInforDataSet, dsEntity);

            if (itemInforDataSet.Tables[0].Rows.Count > 0)
            {
                if (itemInforDataSet.ItemInfo[0].Category1Cd != this.Category1Cd.Text)
                {
                    this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0275)));
                    SKUCdTextBox.Focus();

                    //2015.12.03 start ダミーコード取得失敗時処理を停止する
                    this.SetSession(SalesChangeDisposalSlipInputSessionKey.CodeStuts, false);
                    //2015.12.03 end ダミーコード取得失敗時処理を停止する

                    return;
                }
            }
            else
            {
                // SKUコード以外はクリア
                TransferQuantityTextBox.Text = string.Empty;
                // 売変伝票の場合に旧売価項目を追加
                SlipOldPriceLabel.Text = string.Empty;
                SlipNewPriceTextBox.Text = string.Empty;
                // 廃棄、売変の金額入力をＡＰ変数で制御するようにする
                SlipPriceLabel.Text = string.Empty;
                ReasonDropDownList.SelectedIndex = 0;
                // DataSetを空にする
                if (itemInforDataSet != null && itemInforDataSet.Tables.Count != 0 && itemInforDataSet.Tables[0].Rows.Count != 0)
                {
                    itemInforDataSet.Tables[0].Clear();
                }
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.ItemInfoDataset, itemInforDataSet);
                // Exceptionを発生させる
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB0132, "入力された商品コード")));
                // 2014.05.09 Start 効率改善指摘事項の修正
                SKUCdTextBox.Focus();
                // 2014.05.09 End   効率改善指摘事項の修正

                //2015.12.03 start ダミーコード取得失敗時処理を停止する
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.CodeStuts, false);
                //2015.12.03 end ダミーコード取得失敗時処理を停止する

                return;
            }
            // 暫定修正 2011.11.28 End   商品が未存在の場合はExceptionを発生させる
            // 商品情報取得 <--

             //    AP変数の値を元に入力エリアの売価項目に対して入力可、不可の制御を行う
             //（0：変更不可、1：変更可能、3：警告あり商品のみ可能）
             if ((string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv) == "3")
            {
                if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
                {
                    // 売変の金額入力変更
                    CheckAndChangeSalesChangePriceInput(gridRecord);
                }
                else
                {
                    // 廃棄の金額入力変更
                    CheckAndChangeDisposalPriceInput(gridRecord);
                }
            }

            string productName = itemInforDataSet.ItemInfo[0].ProductsKanji;

            SKUNameLabel.Text = productName;

            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["SKUCd"] = SKUCdTextBox.Text;

            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Attribute01"] = productName;

            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["DisposalChangeQty"] = disposalChangeQty; 
            if (slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            {
                // 売変
                // 売変伝票の場合に旧売価項目を追加
                SlipOldPriceLabel.Text = workEntity.ProductInfo_SalePrice;
                dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["SlipOldPrice"] = string.IsNullOrEmpty(SlipOldPriceLabel.Text) ? 0 : decimal.Parse(SlipOldPriceLabel.Text); 
                //  売変伝票の場合に旧売価項目を追加
                SlipNewPriceTextBox.Text = workEntity.ProductInfo_SalePrice;
                dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["SlipNewPrice"] = string.IsNullOrEmpty(SlipNewPriceTextBox.Text) ? 0 : decimal.Parse(SlipNewPriceTextBox.Text);

            }
            else
            {
                // 廃棄
                SlipPriceLabel.Text = workEntity.ProductInfo_SalePrice;
                dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["SlipDisposalUnitPrica"] = string.IsNullOrEmpty(SlipPriceLabel.Text) ? 0 : decimal.Parse(SlipPriceLabel.Text); 


            }

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

            this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;

            //this.DataBindToSlipDetailGridView();

            this.BindToDetailReasonDownList();

            this.SetHiddenGridViewBySlipType(this.SlipTypeCoverDropDownList.SelectedValue);

            this.SetLineNoForGridView(this.SlipDetailGridView, "SARPurchaseSlipDetail_TransferSlipLineNumber_Control", 0);

            TransferQuantityTextBox.Focus();       
　　　　  
        }
        #endregion

        #region SaveDropDownList (事由保存)
        /// <summary>
        /// 事由コンボボックスデータ保存
        /// 
        /// </summary>
        private void SaveDropDownList()
        {
            DsEntity dsEntity = (DsEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);

            foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
            {
                CoreCenterDropDownList ReasonDropDownList = (CoreCenterDropDownList)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Reason_Control");

                string reason = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Reason"].ToString().Trim();
                string selectedItem = ReasonDropDownList.SelectedItem.Text.ToString();
                if (!reason.Equals(selectedItem))
                {

                    dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Reason"] = selectedItem;
                }
            }

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

        }

        #endregion

        #region AddBlankRowToDataGrid (明細行のデフォルト表示)
        /// <summary>
        /// 明細行のデフォルト表示
        /// </summary>
        private void AddBlankRowToDataGrid()
        {

            DsEntity dsEntity = this.DsEntitySession;

            int lineNo = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Count;
            while (lineNo < 6)
            {
                this.AddBlankRowToTargetDataSet(dsEntity, lineNo);

                lineNo++;
            }

            //20220613 orz.ri 6行の場合上記while文のの処理がしないため、下記DataBindToSlipDetailGridViewのところでシステムエラーが発生する
            if (lineNo >= 6) {

                this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
            }

            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);
            this.DataBindToSlipDetailGridView();


            this.SetHiddenGridViewBySlipType(this.SlipTypeCoverDropDownList.SelectedValue);

            this.BindToDetailReasonDownList();

        }
        #endregion

        #region AddNewBlankRowToDataGrid(明細行に新規行追加)
        /// <summary>
        /// 明細行に新規行追加
        /// </summary>
        private void AddNewBlankRowToDataGrid()
        {

            DsEntity dsEntity = this.DsEntitySession;

            int lineNo = this.SlipDetailGridView.Rows.Count; 

            this.AddBlankRowToTargetDataSet(dsEntity, lineNo);


            this.SetSession(SalesChangeDisposalSlipInputSessionKey.DsEntity, dsEntity);

            this.DataBindToSlipDetailGridView();

            this.BindToDetailReasonDownList();

            this.SetHiddenGridViewBySlipType(this.SlipTypeCoverDropDownList.SelectedValue);


        }
        #endregion

        #region AddBlankRowToTargetDataSet(デフォルト表示用データセットに行を追加する)
        /// <summary>
        /// デフォルト表示用データセットに行を追加する
        /// </summary>
        private void AddBlankRowToTargetDataSet(DsEntity dsEntity, int lineNo)
        {
            int inputSequenceNo = 0;

            if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
            {
                inputSequenceNo = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AsEnumerable().Max(x => int.Parse(x.Field<string>("InputSequenceNo")));
            }

            DateTime systemDate = (DateTime)this.GetSession(SalesChangeDisposalSlipInputSessionKey.SystemDate);


            SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow newRow = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.NewSARSlipDetailErrorRow();

            
            newRow.CorporationCode = UserProfile.CorporationCode;

            // グリッド上のRowNumber
            newRow.SlipLineNo = (lineNo + 1).ToString();

            // 入力媒体区分
            newRow.InputMediaDiv = "1";

            // 入力バッチ番号
            newRow.InputBatchNo = this.BatchNumberTextBox.Text;

            // 入力ＳＥＱ番号
            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 開始
            //row.InputSequenceNo = (inputSequenceNo + 1).ToString();

            // 「入力バッチ番号・伝票番号」毎にインクリメントする。
            newRow.InputSequenceNo = inputSequenceNo.ToString();
            //   2012.01.13 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 変更 終了

            // 入力ＳＥＱ行番号
            newRow.InputSequenceLineNo = lineNo + 1;

            // 伝票番号
            newRow.SlipNo = this.TransferSlipNumberTextBox.Text;

            // 取込処理日
            newRow.InputDate = systemDate;

            // 取込処理時刻
            newRow.InputTime = DateTime.Now.ToString("HHmmss");

            // データ種別
            newRow.DataType = string.Empty;

            // 取込ＳＥＱ番号
            // 2012.06.26 Start 入力ＳＥＱ番号の対応漏れの対応
            //row.UptakeSequenceNo = decimal.Parse((inputSequenceNo + 1).ToString());
            newRow.UptakeSequenceNo = decimal.Parse((inputSequenceNo).ToString());
            // 2012.06.26 End   入力ＳＥＱ番号の対応漏れの対応

            // 取込ＳＥＱ行番号
            newRow.UptakeSequenceLineNo = lineNo + 1;

            newRow.SKUCd = string.Empty;

            newRow.DisposalChangeQty = 0;

            newRow.SlipDisposalUnitPrica = 0;

            newRow.StoreCd = this.StoreTextBox.Text;

            // 伝票種別（カバー情報）
            newRow.SlipTypeCd = this.SlipTypeCoverDropDownList.SelectedValue;

            // 伝票区分（カバー情報）
            newRow.SlipDiv = this.SlipDivisionCoverDropDownList.SelectedValue;

            newRow.Attribute01 = string.Empty;
            newRow.SlipOldPrice = 0;
            newRow.SlipNewPrice = 0;
            newRow.SlipDisposalUnitPrica = 0;
            // グリッド上の事由
            newRow.Reason = string.Empty;

            // グリッド上の（数量×新売単価）
            newRow.SalesChangePrice = 0;

            // グリッド上の（数量×売単価）
            newRow.DisposalPriceAmt = 0;

            // 商品情報Ds.Category1Cd
            newRow.Category1Cd = this.Category1Cd.Text;

            // 商品情報Ds.Category2Cd
            newRow.Category2Cd = this.Category2Cd.Text;

            // 商品情報Ds.Category3Cd
            newRow.Category3Cd = string.Empty;

            // 商品情報Ds.Category4Cd
            newRow.Category4Cd = string.Empty;

            // 商品情報Ds.ScanningCd
            newRow.ScanningCd = string.Empty;
            // 伝票区分（カバー情報）
            newRow.SalesChangeDiv = string.Empty;

            // エラー区分
            newRow.ErrorDiv = string.Empty;

            // 論理削除フラグ
            newRow.LogicalDelFlg = "0";

            // 売変・廃棄日
            newRow.DisposalChangeDate = Convert.ToDateTime(this.SalesChangeDate.Text);

            dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.AddSARSlipDetailErrorRow(newRow);

            this.TargetDataSet = dsEntity.SARSlipDetailErrorDataSet;
            
 
        }

        #endregion

        #region BindToDetailReasonDownList(明細行コンボボックスデフォルト表示)
        /// <summary>
        /// 明細行コンボボックスデフォルト表示
        /// </summary>
        private void BindToDetailReasonDownList()
        {

            string value = this.SlipTypeCoverDropDownList.SelectedValue;

            DsEntity dsEntity = this.DsEntitySession;

            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);


            // 伝票区分ドロップダウンリスト
            if (value.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            {
        
                if (this.SlipDivisionCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice)
                {
                    foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
                    {
                        CoreCenterDropDownList ReasonDropDownList = (CoreCenterDropDownList)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Reason_Control");
                        string Reason = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Reason"].ToString();
                        // 値下げ
                        // 事由ドロップダウンリスト

                            ReasonDropDownList.DataSource = initialDisplayEntity.ReasonSalesChangePriceDownListDictionary;
                            ReasonDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                            ReasonDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                            ReasonDropDownList.DataBind();
                            if (!string.IsNullOrEmpty(Reason))
                            {
                                ListItem li = ReasonDropDownList.Items.FindByText(Reason);
                                int i = ReasonDropDownList.Items.IndexOf(li);
                                ReasonDropDownList.SelectedIndex = i;

                            }
                    }

                }
                else
                {
                    foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
                    {
                        CoreCenterDropDownList ReasonDropDownList = (CoreCenterDropDownList)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Reason_Control");
                        string Reason = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Reason"].ToString();

                        // 値上げ
                        // 事由ドロップダウンリスト
                            ReasonDropDownList.DataSource = initialDisplayEntity.ReasonSalesChangePriceUpListDictionary;
                            ReasonDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                            ReasonDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                            ReasonDropDownList.DataBind();
                            if (!string.IsNullOrEmpty(Reason))
                            {
                                ListItem li = ReasonDropDownList.Items.FindByText(Reason);
                                int i = ReasonDropDownList.Items.IndexOf(li);
                                ReasonDropDownList.SelectedIndex = i;

                            }

                    }
                }
            }
            else if (value.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
            {
                foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
                {
                    CoreCenterDropDownList ReasonDropDownList = (CoreCenterDropDownList)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Reason_Control");
                    string Reason = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Reason"].ToString().Trim();
                    //string Reason = "期限切れ";

                        ReasonDropDownList.DataSource = initialDisplayEntity.ReasonListDictionary;
                        ReasonDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        ReasonDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        ReasonDropDownList.DataBind();
                        ReasonDropDownList.SelectedIndex = -1;
                        
                   if (!string.IsNullOrEmpty(Reason))
                    {
                        ListItem li = ReasonDropDownList.Items.FindByText(Reason);
                        int i = ReasonDropDownList.Items.IndexOf(li);
                        ReasonDropDownList.SelectedIndex = i;
  
                    }
                }

            }
        }

        #endregion

        #region BindToDetailReasonDownList(明細行コンボボックスデフォルト表示)
        /// <summary>
        /// 明細行コンボボックスデフォルト表示
        /// </summary>
        private void BindToDetailReasonDownList(DsEntity dsEntity)
        {

            string value = this.SlipTypeCoverDropDownList.SelectedValue;

            InitialDisplayEntity initialDisplayEntity = (InitialDisplayEntity)this.GetSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);


            // 伝票区分ドロップダウンリスト
            if (value.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            {

                if (this.SlipDivisionCoverDropDownList.SelectedValue == SalesChangeDisposalSlipInputConstances.SalesChangeDivPrice)
                {
                    foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
                    {
                        CoreCenterDropDownList ReasonDropDownList = (CoreCenterDropDownList)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Reason_Control");
                        string Reason = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Reason"].ToString();
                        // 値下げ
                        // 事由ドロップダウンリスト

                        ReasonDropDownList.DataSource = initialDisplayEntity.ReasonSalesChangePriceDownListDictionary;
                        ReasonDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        ReasonDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        ReasonDropDownList.DataBind();
                        if (!string.IsNullOrEmpty(Reason))
                        {
                            ListItem li = ReasonDropDownList.Items.FindByText(Reason);
                            int i = ReasonDropDownList.Items.IndexOf(li);
                            ReasonDropDownList.SelectedIndex = i;

                        }
                    }

                }
                else
                {
                    foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
                    {
                        CoreCenterDropDownList ReasonDropDownList = (CoreCenterDropDownList)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Reason_Control");
                        string Reason = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Reason"].ToString();

                        // 値上げ
                        // 事由ドロップダウンリスト
                        ReasonDropDownList.DataSource = initialDisplayEntity.ReasonSalesChangePriceUpListDictionary;
                        ReasonDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                        ReasonDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                        ReasonDropDownList.DataBind();
                        if (!string.IsNullOrEmpty(Reason))
                        {
                            ListItem li = ReasonDropDownList.Items.FindByText(Reason);
                            int i = ReasonDropDownList.Items.IndexOf(li);
                            ReasonDropDownList.SelectedIndex = i;

                        }

                    }
                }
            }
            else if (value.Equals(SalesChangeDisposalSlipInputConstances.Disposal))
            {
                foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
                {
                    CoreCenterDropDownList ReasonDropDownList = (CoreCenterDropDownList)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_Reason_Control");
                    string Reason = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows[gridRecord.Index]["Reason"].ToString().Trim();
                    //string Reason = "期限切れ";

                    ReasonDropDownList.DataSource = initialDisplayEntity.ReasonListDictionary;
                    ReasonDropDownList.DataTextField = CodeDivisionValueElement.BindingTextPropertyName;
                    ReasonDropDownList.DataValueField = CodeDivisionValueElement.BindingValuePropertyName;
                    ReasonDropDownList.DataBind();
                    ReasonDropDownList.SelectedIndex = -1;

                    if (!string.IsNullOrEmpty(Reason))
                    {
                        ListItem li = ReasonDropDownList.Items.FindByText(Reason);
                        int i = ReasonDropDownList.Items.IndexOf(li);
                        ReasonDropDownList.SelectedIndex = i;

                    }
                }

            }
        }

        #endregion

        #region CheckMaxDetailCount(最大明細をチェックする)
        /// <summary>
        /// 最大明細をチェックする
        /// </summary>
        private bool CheckMaxDetailCount()
        {
            bool ret = true;
            string corporationCode = UserProfile.CorporationCode;
            string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;
            // 最大明細数をチェック
            // 売変と廃棄で設定が異なる為、判定して取得
            string valueMaxDetailCount = (!string.IsNullOrEmpty(slipTypeValue) && slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange)) ? APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SALES_CHANGE_SLIP_MAX_DETAIL_COUNT, corporationCode) : APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_SLIP_MAX_DETAIL_COUNT, corporationCode);
            int maxDetailCount;
            if (int.TryParse(valueMaxDetailCount, out maxDetailCount) == false)
            {
                maxDetailCount = 99;
            }
            if (this.SlipDetailGridView.Rows.Count >= maxDetailCount)
            {
                this.SetMessage(new MessageContainer(ResourceUtil.GetMessage(ErrorKey.ERRCSARB242, maxDetailCount.ToString())));
                ret = false;
            }

            return ret;
        }

        #endregion

        #region GetAndSaveSkuDummyCode(ダミーコード取得)
        /// <summary>
        /// ダミーコード取得
        /// </summary>
        /// <param name="category1Cd">分類１コード</param>
        private void GetAndSaveSkuDummyCode(string category1Cd)
        {
            if (string.IsNullOrEmpty(category1Cd))
            {
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode, string.Empty);
                return;
            }
            if (this.SalesChangeDate.DateValue == null)
            {
                this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode, string.Empty);
                return;
            }

            CategoryCdProductsSearchConditionEntity categoryCdProductsSearchConditionEntity = new CategoryCdProductsSearchConditionEntity();

            categoryCdProductsSearchConditionEntity.CorporationCode = UserProfile.CorporationCode;
            categoryCdProductsSearchConditionEntity.SearchBasicDate = this.SalesChangeDate.DateValue;
            categoryCdProductsSearchConditionEntity.Category1Cd = category1Cd;

            using (STRProductDisclosureDataSet strProductDisclosureDataSet = new STRProductDisclosureDataSet())
            {
                this.GetFacadeInstance().GetDummyVendorCodeNew(
                    categoryCdProductsSearchConditionEntity,
                    strProductDisclosureDataSet);

                    if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Count > 0)
                    {
                        this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode, strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].SKUCd);
                    }
                    else
                    {
                        this.SetSession(SalesChangeDisposalSlipInputSessionKey.Category1DummyCode, string.Empty);
                    }
                    
                }
            }
       
        #endregion

        #region GetAndSaveSkuDummyCode(保存時ダミーコード取得)
        /// <summary>
        /// ダミーコード取得
        /// </summary>
        private void GetAndSaveSkuDummyCode()
        {
            //2015.12.03 start ダミーコード取得失敗時処理を停止する
            this.SetSession(SalesChangeDisposalSlipInputSessionKey.CodeStuts, true);
            //2015.12.03 end ダミーコード取得失敗時処理を停止する

            foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
            {
                CoreCenterTextBox SKUCdTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SKUCd_Control");

                CoreCenterTextBox TransferQuantityTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_TransferQuantity_Control");

                decimal disposalChangeQty = (string.IsNullOrEmpty(TransferQuantityTextBox.Text) ? 0 : decimal.Parse(TransferQuantityTextBox.Text));

                if (string.IsNullOrEmpty(SKUCdTextBox.Text) && disposalChangeQty > 0)
                {

                    this.DetailAddSKUCd_TextChanged(SKUCdTextBox, new EventArgs());

                    //2015.12.03　start 呼び出し元の計算処理と重複のため不要 ri
                    //    this.CalculatorDetailForDiiferenceUnitPriceAndDifferencePrice(false);
                    //2015.12.03 end 呼び出し元の計算処理と重複のため不要  ri
                }


            }

        }
        #endregion

        #region DisplayChangePriceControl
        /// <summary>
        /// 金額項目の画面制御
        /// </summary>
        /// <param name="display">表示(true=表示、false=非表示)</param>
        private void DisplayChangePriceControl(bool display)
        {
            ////デバッグ用
            //string PriceChangeDiv =  (string)this.GetSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv);
            //object[] parameter = { "テスト：8679行目：display：" + display + "/" + PriceChangeDiv };
            //throw RetailBusinessException.CreateInstance(ErrorKey.ERRCCMRB1056, parameter);
            string slipTypeValue = this.SlipTypeCoverDropDownList.SelectedValue;

            if (!string.IsNullOrEmpty(slipTypeValue) && slipTypeValue.Equals(SalesChangeDisposalSlipInputConstances.SalesChange))
            {

                foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
                {
                    CoreCenterTextBox oldPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SlipOldPrice_Control");

                    oldPriceLabel.Enabled = display;
                }
            }
            else
            {

                foreach (GridRecord gridRecord in this.SlipDetailGridView.Rows)
                {
                    CoreCenterTextBox SlipPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SlipPrice_Control");
                    
                    SlipPriceLabel.Enabled = display;
                }
            }
        }
        #endregion

        #region CheckAndChangeSalesChangePriceInput
        /// <summary>
        /// 売変の金額入力変更
        /// </summary>
        private void CheckAndChangeSalesChangePriceInput(GridRecord gridRecord)
        {
            CoreCenterTextBox oldPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SlipOldPrice_Control");
            CoreCenterTextBox SKUCdTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SKUCd_Control");
           
            
            SalesChangeSlipInputProductsReadPDAEntity salesChangeSlipInputProductsReadPDAEntity = new SalesChangeSlipInputProductsReadPDAEntity();

            salesChangeSlipInputProductsReadPDAEntity.CorporationCode = UserProfile.CorporationCode;
            salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate = this.SalesChangeDate.DateValue;
            salesChangeSlipInputProductsReadPDAEntity.StoreCd = this.StoreTextBox.Text;
            salesChangeSlipInputProductsReadPDAEntity.ProductsCd = SKUCdTextBox.Text;
            salesChangeSlipInputProductsReadPDAEntity.RetailReductionCheckDiv = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SALESCHANGE_PRICE_REDUCTION_CHECK_DIV, UserProfile.CorporationCode);
            salesChangeSlipInputProductsReadPDAEntity.Message = string.Empty;

            //// FacadKe呼出し
            //ISalesChangeSlipInputPDAFacade salesChangeSlipInputPDAFacade =
            //    (ISalesChangeSlipInputPDAFacade)ClassObjectUtil.GetFacadeInstance(SalesChangeDisposalSlipInputConstances.SalesChangeSlipInputPDAFacadeClassId, typeof(ISalesChangeSlipInputPDAFacade));

            // 売変入力ＰＤＡ商品検索
            int resultValue = SalesChangeDisposalSlipInputConstances.NormalStatus;
            //resultValue = salesChangeSlipInputPDAFacade.SearchSalesChangeSlipProductPDANew(salesChangeSlipInputProductsReadPDAEntity);
            resultValue = this.GetFacadeInstance().SearchSalesChangeSlipProductPDANew(salesChangeSlipInputProductsReadPDAEntity);

            // 戻り値の判定
            if (resultValue == SalesChangeDisposalSlipInputConstances.ErrorStatus)
            {
                this.SetMessage(new MessageContainer(salesChangeSlipInputProductsReadPDAEntity.Message));

                return;
            }
            // 戻りメッセージの判定
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.Message) == false)
            {
                // 戻りのメッセージがある場合は変更可能へ
                oldPriceLabel.Enabled = true;
                this.SetMessage(new MessageContainer(salesChangeSlipInputProductsReadPDAEntity.Message));
                // 2012.11.01 End   売変伝票の場合に旧売価項目を追加
                return;
            }
            else {
                oldPriceLabel.Enabled = false;
            }

        }
        #endregion

        #region CheckAndChangeDisposalPriceInput
        /// <summary>
        /// 廃棄の金額入力変更
        /// </summary>
        private void CheckAndChangeDisposalPriceInput(GridRecord gridRecord)
        {
            CoreCenterTextBox SlipPriceLabel = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SlipPrice_Control");
            CoreCenterTextBox SKUCdTextBox = (CoreCenterTextBox)WebDataGridUtil.FindControl(gridRecord, "SARPurchaseSlipDetail_SKUCd_Control");
            
            DisposalSlipInputProductsReadPDAEntity disposalSlipInputProductsReadPDAEntity = new DisposalSlipInputProductsReadPDAEntity();

            disposalSlipInputProductsReadPDAEntity.CorporationCode = UserProfile.CorporationCode;
            disposalSlipInputProductsReadPDAEntity.SalesChangeDate = this.SalesChangeDate.DateValue;
            disposalSlipInputProductsReadPDAEntity.StoreCd = this.StoreTextBox.Text;
            disposalSlipInputProductsReadPDAEntity.ProductCd = SKUCdTextBox.Text;
            disposalSlipInputProductsReadPDAEntity.SalePriceDisp = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_STOCKSALEPRICEDISPLAYFLAG, UserProfile.CorporationCode);
            disposalSlipInputProductsReadPDAEntity.SalePriceRoundOffMethod = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SELLPRICEROUNDINGMETHOD, UserProfile.CorporationCode);
            disposalSlipInputProductsReadPDAEntity.Message = string.Empty;

            //// Facade呼出し
            //IDisposalSlipInputPDAFacade disposalSlipInputPDAFacade
            //    = (IDisposalSlipInputPDAFacade)ClassObjectUtil.GetFacadeInstance(SalesChangeDisposalSlipInputConstances.DisposalSlipInputPDAFacadeClassId, typeof(IDisposalSlipInputPDAFacade));

            // 廃棄入力ＰＤＡ商品検索
            int resultValue = SalesChangeDisposalSlipInputConstances.NormalStatus;
            //resultValue = disposalSlipInputPDAFacade.SearchDisposalSlipProductPDANew(ref disposalSlipInputProductsReadPDAEntity);
            resultValue = this.GetFacadeInstance().SearchDisposalSlipProductPDANew(ref disposalSlipInputProductsReadPDAEntity);

            // 戻り値の判定
            if (resultValue == SalesChangeDisposalSlipInputConstances.ErrorStatus)
            {
                this.SetMessage(new MessageContainer(disposalSlipInputProductsReadPDAEntity.Message));
                return;
            }
            // 戻りメッセージの判定
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.Message) == false)
            {
                SlipPriceLabel.Enabled = true;
                this.SetMessage(new MessageContainer(disposalSlipInputProductsReadPDAEntity.Message));
                return;
            }
            else {
                SlipPriceLabel.Enabled = false;
            }

        }
        #endregion

        /// <summary>
        /// セッションを全部クリアする
        /// </summary>
        private void ClearSession()
        {
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.OrgScreenId);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.InitMode);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.WorkEntity);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.InitMode);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.InitialDisplayEntity);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.CoverSheetMasterEntity);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SystemDate);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.DsEntity);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.InputBatchNo);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.StoreCd);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeCd);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SlipDiv);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SlipNo);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.InputBatchDate);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.PriceChangeDiv);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.IsHeadquarters);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.DsTmpEntity);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.DsInputStatusEntity);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.OrgDsEntity);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionStoreCD);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDataType);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionDiv);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionInputBatchNo);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SASlipCorrectionBatchDate);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.SlipTypeSelectedValue);
            this.RemoveSession(SalesChangeDisposalSlipInputSessionKey.UpdateCoverSheetMessage);
        }
        #endregion

    }
}
