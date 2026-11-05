using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NihonUnisys.CoreCenter.Core;
using NihonUnisys.CoreCenter.Core.Resources;
using NihonUnisys.CoreCenter.Core.User;
using NihonUnisys.CoreCenter.Retail.Buying.Buying.Business;
using NihonUnisys.CoreCenter.Retail.Buying.Common.Business;
using NihonUnisys.CoreCenter.Retail.Buying.PublicBusiness.Business;
using NihonUnisys.CoreCenter.Retail.Common.AccountBook.Business;
using NihonUnisys.CoreCenter.Retail.Common.PrivateBusiness.DataTransferObject;
using NihonUnisys.CoreCenter.Retail.Common.PublicBusiness.Business;
using NihonUnisys.CoreCenter.Retail.Common.Resources;
using NihonUnisys.CoreCenter.Retail.Common;
using NihonUnisys.CoreCenter.Retail.Inventory.PublicBusiness.Business;
using NihonUnisys.CoreCenter.Retail.SalesPromotion.PublicBusiness.Business;
using NihonUnisys.CoreCenter.Retail.SalesTalking.PublicBusiness.Business;
// 2012.06.21 Start 大福帳へセットする内容の訂正
using NihonUnisys.CoreCenter.DistributionCommon.Common.MasterBusiness.DataTransferObject;
// 2012.06.21 End   大福帳へセットする内容の訂正
// 2012.07.19 Start 特売情報の取得方法変更
using NihonUnisys.CoreCenter.Retail.Sales.InputSalesSlip.Business.SARBargainPlanningProductStorePublicInformationDataSetTableAdapters;
using NihonUnisys.CoreCenter.DistributionCommon.Common.MasterBusiness.Business;
// 2012.07.19 End   特売情報の取得方法変更

#region 【修正履歴】
// 【修正履歴】
//   2011.11.30 小林 大福帳の伝票が増幅していくのを修正
//
//   2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新
//
//   2011.12.06 関根 大福帳の時刻・時間帯に値を更新する
//
//   2012.01.08 関根 黒伝・赤伝時の訂正区分の誤りを修正
//
//   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応
//
//   2012.01.12 関根 大福帳チェックエラー対応
//
//   2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応
//
//   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応
//
//   2012.01.16 関根 大福帳抽出で店舗コード指定ありメソッドを使用する
//
//   2012.01.21 在庫管理区分の更新対応
//
//   2012.01.21 関根 伝票取消時の障害対応
//
//   2012.01.23 関根 大福帳に赤伝が２重に登録される不具合対応（全明細削除時に発生）
//
//   2012.06.13 小林 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
//
//   2012.06.13 小林 廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。
//
//   2012.06.21 小林 大福帳へセットする内容の訂正
//
//   2012.06.21 小林 売変用の事由を追加（値下げ、値上げの２パターン分）
//
//   2012.06.21 小林 大福帳登録時に仮想キーで一意違反となるので修正
//                   ※伝票番号が伝票区分、伝票種別ごとに発番され、大福帳の仮想キーに入っていないので
//                     レシート番号、レシート行番号にそれぞれ伝票区分、伝票種別をセットする
//
//   2012.06.21 小林 ＰＯＳ値引区分にＡＰ変数の値をセットしておく
//
//   2012.06.26 小林 入力状況情報のOK ERRORの表示がされないのを修正
//
//   2012.06.26 小林 伝票取消対応(削除フラグをセットする)
//
//   2012.07.19 小林 特売情報の取得方法変更
//
//   2012.07.24 小林 商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
//
//   2012.08.24 伝票の行削除で論理削除フラグは立てないのでコメントアウト
//
//   2012.10.17 商品コードが入力されていない場合は部門代表商品とするようにする
//
//   2013.04.02 Sueyoshi 廃棄伝票マイナス入力対応
//
//   2013.11.26 関根 大福帳登録時に「CreateDate」「UpdateDate」には、日時をセットするように修正（現在は日付がセットされている）
//
//   2013.11.26 関根 承認解除の場合は、「伝票は赤黒を作成」「大福帳は赤のみ作成」を行うように修正
//
//   2013.11.26 関根 「AP変数．承認モード＝１(必要)」の設定だと、黒伝のみしか作成されない不具合を修正
//
//   2013.11.26 関根 承認解除の場合は、「伝票は赤黒を作成」「大福帳は赤のみ作成」を行うように修正
//
//   2013.11.26 関根 大福帳登録時に「CreateDate」「UpdateDate」には、日時をセットするように修正（現在は日付がセットされている）
//
//   2013.11.29 関根 伝票ヘッダの承認ステータス更新追加
//
//   2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成）
//
//   2013.12.05 関根 売変時の商流区分の設定変更
//
//   2013.12.05 関根 廃棄評価差額の商流区分の設定内容変更
//
//   2013.12.06 関根 売変訂正時には、商流区分を反転させる
//
//   2014.01.06 関根 税込売価金額（PretaxSellPrice）には、税抜売価金額（BeforetaxSellPrice）と同様の値をセットする
//
//   2014.01.07 関根 対象店舗の営業時間帯を取得出来ない場合は、９９９店舗の時間帯を取得する。
//
//   2014.01.09 関根 カバーシート検索画面から遷移した時に、複数伝票の明細が表示される不具合修正
//
//   2014.05.29 小林 カバーシート検索より伝票起票画面を表示し、伝票が１件の場合に伝票削除を行うとシステムエラーが発生するので修正

#endregion

namespace NihonUnisys.CoreCenter.Retail.Sales.InputSalesSlip.Business
{
    /// <summary>
    /// 売変廃棄伝票起票画面で使用しているクラス
    /// </summary>
    public partial class SalesChangeDisposalSlipEntryBusiness : BusinessBase, ISalesChangeDisposalSlipEntryBusiness
    {
        //2015.12.14 start 標準Brachと統合するにより重複のため不要
        ///// <summary>
        ///// ErrIDEnum
        ///// </summary>
        //public enum ErrIDEnum : int
        //{
        //    /// <summary>
        //    /// ビジネスルール１のエラー
        //    /// </summary>
        //    ERRCSARB0194 = 1,

        //    /// <summary>
        //    /// ビジネスルール２のエラー
        //    /// </summary>
        //    ERRCSARB0195 = 2,

        //    /// <summary>
        //    /// ビジネスルール３のエラー
        //    /// </summary>
        //    ERRCSARB0196 = 3,

        //    /// <summary>
        //    /// ビジネスルール４のエラー
        //    /// </summary>
        //    ERRCSARB0197 = 4,

        //    /// <summary>
        //    /// ビジネスルール５のエラー
        //    /// </summary>
        //    ERRCSARB0198 = 5,

        //    /// <summary>
        //    /// ビジネスルール６のエラー
        //    /// </summary>
        //    MSGCSARI0016 = 6,

        //    /// <summary>
        //    /// ビジネスルール７のエラー
        //    /// </summary>
        //    ERRCSARB0199 = 7,

        //    /// <summary>
        //    /// ビジネスルール８のエラー
        //    /// </summary>
        //    ERRCSARB0200 = 8,

        //    /// <summary>
        //    /// ビジネスルール９のエラー
        //    /// </summary>
        //    ERRCSARB0201 = 9,

        //    /// <summary>
        //    /// ビジネスルール１０のエラー
        //    /// </summary>
        //    ERRCSARB0202 = 10,

        //    /// <summary>
        //    /// ビジネスルール１０２のエラー
        //    /// </summary>
        //    ERRCSARB0203 = 102,

        //    /// <summary>
        //    /// ビジネスルール１０３のエラー
        //    /// </summary>
        //    ERRCSARB0204 = 103,

        //    /// <summary>
        //    /// ビジネスルール１０４のエラー
        //    /// </summary>
        //    ERRCSARB0205_104 = 104,

        //    /// <summary>
        //    /// ビジネスルール１０５のエラー
        //    /// </summary>
        //    ERRCSARB0205_105 = 105,

        //    /// <summary>
        //    /// ビジネスルール１０６のエラー
        //    /// </summary>
        //    ERRCSARB0205_106 = 106,

        //    /// <summary>
        //    /// ビジネスルール１０７のエラー
        //    /// </summary>
        //    ERRCSARB0206 = 107,

        //    /// <summary>
        //    /// ビジネスルール１０８のエラー
        //    /// </summary>
        //    ERRCSARB0207 = 108
        //}

        ///// <summary>
        ///// 赤のみ作成
        ///// </summary>
        //private const string slipUpdateMode_Red = "1";

        ///// <summary>
        ///// 黒のみ作成
        ///// </summary>
        //private const string slipUpdateMode_Black = "0";

        ///// <summary>
        ///// 赤黒両方作成
        ///// </summary>
        //private const string slipUpdateMode_RedBlack = "2";

        ///// <summary>
        ///// 起動モード:新規
        ///// </summary>
        //private const string initMode_New = "0";

        ///// <summary>
        ///// 起動モード:伝票検索からの遷移
        ///// </summary>
        //private const string initMode_SlipSearch = "1";

        ///// <summary>
        ///// 起動モード:カバーシートマスタからの遷移
        ///// </summary>
        //private const string initMode_CoverSheetMaster = "2";
        //2015.12.14 end 標準Brachと統合するにより重複のため不要


        // 2013.11.02 AST Add Start
        private IDictionary<string, string> posDiscountDivDictionary = null;
        // 2013.11.02 AST Add End

        // 2012.06.13 Start 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
        ///// <summary>
        ///// 売変伝票:01
        ///// </summary>
        //private const string salesChange = "01";

        ///// <summary>
        ///// 廃棄伝票:02
        ///// </summary>
        //private const string disposal = "02";
        // 2012.06.13 End   売変伝票区分、廃棄伝票区分をAP変数より取得するように変更

        // 2012.06.13 Start 廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。
        ///// <summary>
        ///// 売変伝票:SAR002
        ///// </summary>
        //private const string salesChangeValue = "SAR002";

        ///// <summary>
        ///// 廃棄伝票:SAR004
        ///// </summary>
        //private const string disposalValue = "SAR004";
        // 2012.06.13 End   廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。

        //2015.12.14 start 標準Brachと統合するにより重複のため不要
        // 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
        /// <summary>
        /// 商品種類 : ダミー商品 (07)
        /// </summary>
        //private const string productCategory_DummyItem = "07";
        // 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする
        //2015.12.14 end 標準Brachと統合するにより重複のため不要

        // 2013.08.27 Start 売価取得区分の定義
        /// <summary>
        /// 売価取得区分：標準売価
        /// </summary>
        public const string STANDARD_SELLING_PRICE_KBN = "0";

        /// <summary>
        /// 売価取得区分：最安値
        /// </summary>
        public const string LOWEST_PRICE_KBN = "1";
        // 2013.08.27 End   売価取得区分の定義

        // --> 2013/09/26 店別売価対応
        /// <summary>
        /// 売価取得区分：店別売価
        /// </summary>
        public const string STORE_PRICE_KBN = "2";
        // <-- 2013/09/26 店別売価対応
        
        /// <summary>
        /// 売変廃棄伝票起票画面の初期処理として、AP変数、業務日付を取得処理を呼び出す。
        /// (BSCSAR05008000131)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        /// <param name= "coverSheetMasterEntity">カバーシート情報Entity</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name= "isHeadquarters">isHeadquarters</param>
        public virtual void InitStartNew(
           WorkEntity workEntity,
           InitialDisplayEntity initEntity,
           CoverSheetMasterEntity coverSheetMasterEntity,
           DsEntity dsEntity,
           out string isHeadquarters)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            if (coverSheetMasterEntity == null)
            {
                throw new ArgumentNullException("coverSheetMasterEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            #endregion

            // AP変数取得Bsを呼び出す。BSCSAR05008000132
            this.FillAPVariableNew(workEntity, initEntity);

            // 【共通】ＣＭＣ業務日付取得を呼び出す。
            ICmcOtherAPI otherAPI = this.GetCmcOtherAPIInstanceNew();
            otherAPI.CMCGetSystemDate(workEntity.Subsystem, workEntity.SystemType, workEntity.CorporationCode, initEntity.SystemDateDataset);

            workEntity.SalesMngDate = initEntity.SystemDateDataset.CMCSystemDate[0].SystemDate;

            // 【在庫公開部品】BSCIVC01901000001を呼び出す。
            IIVCStockControlAPI stockControlAPI = this.GetIVCStockControlAPIInstanceNew();
            workEntity.IVPeriodDate = stockControlAPI.GetFixedMonthly(workEntity.CorporationCode).StockRecordedDate;

            // ユーザー情報取得のオブジェクトを呼出す(BSCSAR05008000043)
            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            //this.FillUserProfile(workEntity, initEntity);
            this.FillUserProfileNew(workEntity, initEntity, out isHeadquarters);
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。

            // ドロップダウンリスト生成のオブジェクトを呼出す
            this.FillCodeDivNew(workEntity, initEntity);
            if (workEntity.InitMode == initMode_SlipSearch ||
                workEntity.InitMode == initMode_CoverSheetMaster ||
                !string.IsNullOrEmpty(coverSheetMasterEntity.BatchNo))
            {
                // カバーシート検索と伝票（トラン）の検索実施
                // カバー検索(DAOCSAR05030)を呼び出す。
                using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
                {
                    dataAccessComponent.FillSARCoverSheetMasterNew(coverSheetMasterEntity, dsEntity.CoverSheetMasterDataset);
                }

                // 伝票検索(BSCSAR05008000143)を呼び出す。
                this.FillSlipNew(workEntity, dsEntity);
            }
        }

        /// <summary>
        /// AP変数取得処理を行う。
        /// (BSCSAR05008000132)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void FillAPVariableNew(
           WorkEntity workEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            string apVariableValue = string.Empty;
            foreach (var item in initEntity.APVariableDispDataset.UTFAPVariable)
            {
                apVariableValue = APVariableUtil.GetAPVariableValue(item.APVariableCode, workEntity.CorporationCode);

                // 取得される結果を初期表示Entity.AP変数取得Dｓへ格納する。
                item.APVariableValue = apVariableValue;

                // 取得されるAP変数情報を初期表示Entity.AP変数DataSetへ格納する。
                initEntity.APAvaliableDataSet.UTFAPVariable.AddUTFAPVariableRow(
                    item.APVariableCode,
                    workEntity.CorporationCode,
                    item.APVariableValue);
            }
        }

        /// <summary>
        /// ユーザ情報を取得する。
        /// </summary>
        /// <remarks>
        /// 店舗ユーザーの場合は、店舗コードを変更不可にする。
        /// </remarks>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        /// <param name= "isHeadquarters">isHeadquarters</param>
        public virtual void FillUserProfileNew(
           WorkEntity workEntity,
           InitialDisplayEntity initEntity,
           out string isHeadquarters)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            // 【BSCCMR01011000012】ユーザ情報店舗コード取得
            ICmcOtherAPI cmcOtherAPI = this.GetCmcOtherAPIInstanceNew();
            string storeCd = cmcOtherAPI.GetUserProfileStoreCd(workEntity.Subsystem);
            StoreDisclosureSearchConditionEntity storeDisclosureSearchCondition = new StoreDisclosureSearchConditionEntity()
            {
                BaseDate = workEntity.SalesMngDate,
                CorporationCode = workEntity.CorporationCode,
                StoreCD = storeCd
            };

            // 2014.01.09 Start 店舗ユーザーの場合は、店舗コードを変更不可にする。
            if (String.IsNullOrEmpty(storeCd))
            {
                //本部ユーザの場合
                isHeadquarters = "1";
            }
            else
            {
                //店舗ユーザの場合
                isHeadquarters = string.Empty;
            }
            // 2014.01.09 End   店舗ユーザーの場合は、店舗コードを変更不可にする。
            
            using (StoreDisclosureDataSet storeDisclosureDataSet = new StoreDisclosureDataSet())
            {
                // 【BSCCMR01001000001】ＣＭＲ店舗公開情報取得
                ICmrStoreAPI cmrStoreAPI = this.GetCmrStoreAPIInstanceNew();
                cmrStoreAPI.GetStoreDisclosure(
                    workEntity.Subsystem,
                    storeDisclosureSearchCondition,
                    storeDisclosureDataSet);

                // 取得された店舗公開DataSetを初期表示Entity.ユーザー店舗情報Dsへセットする
                initEntity.UserStoreDataset.Clear();
                initEntity.UserStoreDataset.UserStoreDataTable.Merge(storeDisclosureDataSet.StoreDisclosure);
            }
        }

        /// <summary>
        /// ユーザ情報を取得する。
        /// (BSCSAR05008000133)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void FillUserProfileNew(
           WorkEntity workEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            // 【BSCCMR01011000012】ユーザ情報店舗コード取得
            ICmcOtherAPI cmcOtherAPI = this.GetCmcOtherAPIInstanceNew();
            string storeCd = cmcOtherAPI.GetUserProfileStoreCd(workEntity.Subsystem);

            StoreDisclosureSearchConditionEntity storeDisclosureSearchCondition = new StoreDisclosureSearchConditionEntity()
            {
                BaseDate = workEntity.SalesMngDate,
                CorporationCode = workEntity.CorporationCode,
                StoreCD = storeCd
            };

            using (StoreDisclosureDataSet storeDisclosureDataSet = new StoreDisclosureDataSet())
            {
                // 【BSCCMR01001000001】ＣＭＲ店舗公開情報取得
                ICmrStoreAPI cmrStoreAPI = this.GetCmrStoreAPIInstanceNew();
                cmrStoreAPI.GetStoreDisclosure(
                    workEntity.Subsystem,
                    storeDisclosureSearchCondition,
                    storeDisclosureDataSet);

                // 取得された店舗公開DataSetを初期表示Entity.ユーザー店舗情報Dsへセットする
                initEntity.UserStoreDataset.Clear();
                initEntity.UserStoreDataset.UserStoreDataTable.Merge(storeDisclosureDataSet.StoreDisclosure);
            }
        }

        /// <summary>
        /// 分類１コード情報を取得する。
        /// (BSCSAR05008000079)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        public virtual void GetCategory1NameNew(WorkEntity workEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            #endregion

            // 共通【BSCCMR01003000010】の実施
            Category1NameDisclosureSearchConditionEntity searchConditionEntity = new Category1NameDisclosureSearchConditionEntity()
            {
                BaseDate = workEntity.ChangeDate,
                CorporationCode = workEntity.CorporationCode,
                Category1CD = workEntity.Category1Cd
            };

            using (Category1NameDisclosureDataSet category1NameDisclosureDataSet = new Category1NameDisclosureDataSet())
            {
                ICmrCategoryAPI categoryAPI = this.GetCmrCategoryAPIInstanceNew();
                categoryAPI.GetCategory1NameDisclosure(workEntity.Subsystem, searchConditionEntity, category1NameDisclosureDataSet);

                // 取得される分類１名称公開DataSet.分類１名称（漢字）をWorkEntity.分類１名称へセットする。
                workEntity.Category1Name = category1NameDisclosureDataSet.Category1NameDisclosure.Select(row => row.Category1Kanji).FirstOrDefault()
                                            ?? string.Empty;
            }
        }

        /// <summary>
        /// 分類２コード情報を取得する。
        /// (BSCSAR05008000080)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        public virtual void GetCategory2NameNew(WorkEntity workEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            #endregion

            // 共通【BSCCMR01003000011】の実施
            Category2NameDisclosureSearchConditionEntity searchConditionEntity = new Category2NameDisclosureSearchConditionEntity()
            {
                BaseDate = workEntity.ChangeDate,
                CorporationCode = workEntity.CorporationCode,
                Category1CD = workEntity.Category1Cd,
                Category2CD = workEntity.Category2Cd
            };

            using (Category2NameDisclosureDataSet category2NameDisclosureDataSet = new Category2NameDisclosureDataSet())
            {
                ICmrCategoryAPI categoryAPI = this.GetCmrCategoryAPIInstanceNew();
                categoryAPI.GetCategory2NameDisclosure(workEntity.Subsystem, searchConditionEntity, category2NameDisclosureDataSet);

                // 取得される分類２名称公開DataSet.分類２名称（漢字）をWorkEntity.分類２名称へセットする。
                workEntity.Category2Name = category2NameDisclosureDataSet.Category2NameDisclosure.Select(row => row.Category2Kanji).FirstOrDefault()
                                            ?? string.Empty;
            }
        }

        /// <summary>
        /// ドロップダウンリストを生成する。
        /// (BSCSAR05008000134)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void FillCodeDivNew(
           WorkEntity workEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            // 伝票種別ドロップダウンリスト【BSCSAR05008000135】
            this.FillSlipTypeMasterNew(workEntity, ref initEntity);

            // 伝票区分売変ドロップダウンリスト【BSCSAR05008000136】
            this.FillSlipChangeDivNew(workEntity, ref initEntity);

            // 伝票区分廃棄ドロップダウンリスト【BSCSAR05008000137】
            this.FillSlipDisposalDivNew(workEntity, ref initEntity);

            // 事由ドロップダウンリスト【BSCSAR05008000138】
            this.FillSlipReasonDivNew(workEntity, ref initEntity);
        }

        /// <summary>
        /// 伝票種別コード区分を取得する。
        /// (BSCSAR05008000135)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void FillSlipTypeMasterNew(
           WorkEntity workEntity,
           ref InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }
            #endregion

            initEntity.SlipTypeListDictionary = CodeDivisionUtil.GetCodeDivisionValueElements(workEntity.SlipTypeCdForDiv, workEntity.CorporationCode);
        }

        /// <summary>
        /// 伝票区分売変コード区分を取得する。
        /// (BSCSAR05008000136)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void FillSlipChangeDivNew(
           WorkEntity workEntity,
           ref InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            initEntity.SalesChangeSlipDivListDictionary = CodeDivisionUtil.GetCodeDivisionValueElements(workEntity.SlipKbnSalesChangeCdForDiv, workEntity.CorporationCode);
        }

        /// <summary>
        /// 伝票区分廃棄コード区分を取得する。
        /// (BSCSAR05008000137)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void FillSlipDisposalDivNew(
           WorkEntity workEntity,
           ref InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            initEntity.DisposalSlipDivListDictionary = CodeDivisionUtil.GetCodeDivisionValueElements(workEntity.SlipDivDisposalCdForDiv, workEntity.CorporationCode);
        }

        /// <summary>
        /// 販売伝票事由コード（売変の値下げ・値上げ）を取得する。
        /// (BSCSAR05008000138)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void FillSlipReasonDivNew(
           WorkEntity workEntity,
           ref InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            initEntity.ReasonListDictionary = CodeDivisionUtil.GetCodeDivisionValueElements(workEntity.SaleSlipReasonCode, workEntity.CorporationCode);

            // 2012.06.21 Start 売変用の事由を追加（値下げ、値上げの２パターン分）
            initEntity.ReasonSalesChangePriceDownListDictionary = CodeDivisionUtil.GetCodeDivisionValueElements(workEntity.SaleSlipReasonCodeSalesChangePriceDown, workEntity.CorporationCode);

            initEntity.ReasonSalesChangePriceUpListDictionary = CodeDivisionUtil.GetCodeDivisionValueElements(workEntity.SaleSlipReasonCodeSalesChangePriceUp, workEntity.CorporationCode);
            // 2012.06.21 End   売変用の事由を追加（値下げ、値上げの２パターン分）
        }

        /// <summary>
        /// 店舗コードの存在チェックを行う。
        /// (BSCSAR05008000139)
        /// </summary>
        /// <param name="workEntity">ワークエンティティ</param>
        /// <returns>判断結果</returns>
        public virtual string ChkStoreCodeNew(WorkEntity workEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            #endregion

            // 【共通】ＣＭＣ店舗存在チェックを実行する
            StoreDisclosureSearchConditionEntity storeSearchConditionEntity = new StoreDisclosureSearchConditionEntity()
            {
                CorporationCode = workEntity.CorporationCode,
                BaseDate = workEntity.BatchDate,
                StoreCD = workEntity.StoreCd
            };

            using (StoreDisclosureDataSet storeDisclosureDs = new StoreDisclosureDataSet())
            {
                ICmrStoreAPI storeAPI = this.GetCmrStoreAPIInstanceNew();
                storeAPI.GetStoreDisclosure(workEntity.Subsystem, storeSearchConditionEntity, storeDisclosureDs);

                if (storeDisclosureDs.StoreDisclosure.Count > 0)
                {
                    workEntity.StoreName = storeDisclosureDs.StoreDisclosure[0].StoreKanji;
                    return "0";
                }
                else
                {
                    return "9";
                }
            }
        }

        /// <summary>
        /// カバーシート情報の保存を行う。
        /// (BSCSAR05008000140)
        /// </summary>
        /// <remarks>
        /// （１）起動モードが「新規」の場合
        /// ・バッチ番号入力チェックを行う。
        /// →既にDBに該当のバッチ番号が存在する場合は「既に使われているバッチ番号」である旨のメッセージを画面に表示する。
        /// →DBに存在しない場合は、カバーシートの登録更新処理を行う。
        /// （２）起動モードが「伝票検索」か「カバーシートマスタ」の場合
        ///・伝票検索処理を呼び出す。
        /// </remarks>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <returns>判定結果</returns>
        public virtual string UpInsSARCoverSheetMasterNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            if (workEntity.InitMode == initMode_New)
            {
                using (BYRBatchNumberMasterDataSet batchNumberMasterDataSet = new BYRBatchNumberMasterDataSet())
                {
                    int checkResult = this.ChkBatchNoNew(workEntity, dsEntity.CoverSheetMasterDataset, batchNumberMasterDataSet);
                    workEntity.CoverExecuteMode = checkResult.ToString();

                    if (checkResult == 1)
                    {
                        return Constants.OnFlag;
                    }
                    else if (checkResult == int.Parse(workEntity.CoverExecuteMode))
                    {
                        // 伝票検索メゾットの実施-【BSCSAR05008000142】を呼び出す
                        this.UpCoverSheetMasterNew(workEntity, dsEntity);
                    }
                }
            }
            else if (workEntity.InitMode == initMode_SlipSearch || workEntity.InitMode == initMode_CoverSheetMaster)
            {
                this.FillSlipNew(workEntity, dsEntity);
            }

            // 伝票番号取得を実施する。
            this.FillSlipNoNew(workEntity, dsEntity);

            return Constants.OffFlag;
        }

        /// <summary>
        /// バッチ番号入力チェックを行う。
        /// (BSCSAR05008000141)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "coverSheetMasterDataset">DsEntity.更新用カバーシートマスタデータセット</param>
        /// <param name="batchNumberMasterDataSet">バッチ番号Ds</param>
        /// <returns>判定結果</returns>
        public virtual int ChkBatchNoNew(
           WorkEntity workEntity,
           CoverSheetMasterDataset coverSheetMasterDataset,
           BYRBatchNumberMasterDataSet batchNumberMasterDataSet)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (coverSheetMasterDataset == null)
            {
                throw new ArgumentNullException("coverSheetMasterDataset");
            }

            if (batchNumberMasterDataSet == null)
            {
                throw new ArgumentNullException("batchNumberMasterDataSet");
            }

            #endregion

            // 【DAOCSAR05010】を実施
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                dataAccessComponent.ChkBatchNoNew(workEntity, coverSheetMasterDataset, batchNumberMasterDataSet);
            }

            return (batchNumberMasterDataSet.BYRBatchNumberMaster.Count == 0) ? 0 : 1;
        }

        /// <summary>
        /// カバーシートの登録更新処理を行う。
        /// (BSCSAR05008000142)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void UpCoverSheetMasterNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            // 【DAOCSAR05011】を実施
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                dataAccessComponent.UpInsSARCoverSheetMasterNew(workEntity, dsEntity);
            }
        }

        /// <summary>
        /// 伝票検索処理を行う。
        /// (BSCSAR05008000143)
        /// </summary>
        /// <remarks>
        /// 起動モードが「カバーシート検索画面からの遷移」の場合
        ///・伝票検索SQLを実行。
        ///→伝票番号が指定されていない場合は、取得した「伝票ヘッダーエラー」の伝票番号を検索条件の伝票番号に設定する。
        /// </remarks>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void FillSlipNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            // 2014.01.09 Start カバーシート検索画面から遷移した時に、複数伝票の明細が表示される不具合修正
            //// 伝票検索SQLの実施BSCSAR05008000090
            //this.FillSlipHeader(workEntity, dsEntity);

            //// 伝票明細検索メゾットの実施BSCSAR05008000091
            //this.FillSlipDetail(workEntity, dsEntity);

            //// 伝票番号取得のＳＱＬの実施BSCSAR05008000092
            //this.FillSlipNo(workEntity, dsEntity);

            if (workEntity.InitMode == "1")
            {
                // 「InitMode=1(カバーシート検索画面からの遷移」)」
                // 伝票検索SQLの実施BSCSAR05008000144
                this.FillSlipHeaderNew(workEntity, dsEntity);
                // 2014.05.29 Start カバーシート検索より伝票起票画面を表示し、伝票が１件の場合に伝票削除を行うとシステムエラーが発生するので修正
                //if (string.IsNullOrEmpty(workEntity.TargetSlipNo))
                // 伝票番号が空だがヘッダー情報が取得できた場合は先頭の伝票番号で明細を取得するようにする
                // 伝票番号が空でヘッダー情報も取得できない場合はカバーシートのみで伝票全てが削除された場合なので検索条件の伝票番号は空のまま処理を続行する
                if (string.IsNullOrEmpty(workEntity.TargetSlipNo) && dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Count != 0)
                // 2014.05.29 End   カバーシート検索より伝票起票画面を表示し、伝票が１件の場合に伝票削除を行うとシステムエラーが発生するので修正
                {
                    // カバーシートから遷移した時に、伝票番号が指定されていない場合は、
                    // 伝票ヘッダーエラーから１件目の伝票番号をセットする。
                    workEntity.TargetSlipNo = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipNo;
                }

                // 伝票明細検索メゾットの実施BSCSAR05008000145
                this.FillSlipDetailNew(workEntity, dsEntity);

                // 伝票番号取得のＳＱＬの実施BSCSAR05008000146
                this.FillSlipNoNew(workEntity, dsEntity);
            }
            else
            {
                // 伝票検索SQLの実施BSCSAR05008000144
                this.FillSlipHeaderNew(workEntity, dsEntity);

                // 伝票明細検索メゾットの実施BSCSAR05008000145
                this.FillSlipDetailNew(workEntity, dsEntity);

                // 伝票番号取得のＳＱＬの実施BSCSAR05008000146
                this.FillSlipNoNew(workEntity, dsEntity);
            }
            // 2014.01.09 End   カバーシート検索画面から遷移した時に、複数伝票の明細が表示される不具合修正
        }

        /// <summary>
        /// 伝票ヘッダの検索を行う。
        /// (BSCSAR05008000144)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void FillSlipHeaderNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            // 【DAOCSAR05012】を実施する
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                dataAccessComponent.FillSARSlipHeaderErrorNew(workEntity, dsEntity);
            }

            // ステータス名称を取得する
            IList<CodeDivisionValueElement> statusNameList = CodeDivisionUtil.GetCodeDivisionValueElements(workEntity.SlipRegisStatusCodeKbn, workEntity.CorporationCode);
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Count > 0)
            {
                // 2013/09/23 start 入力状況情報は、エラー区分（ErrorDiv）と付き合わせるのが正しいと思われる。
                #region 2013/09/23 DELETE
                //// 2012.06.26 Start 入力状況情報のOK ERRORの表示がされないのを修正
                ////var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];
                ////headerRow.Attribute01 = statusNameList.Where(row => row.CodeDivisionValue == headerRow.ApprovalStatus).Select(r => r.CodeDivisionValueName).FirstOrDefault();
                //foreach (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerRow in dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows)
                //{
                //    headerRow.Attribute01 = statusNameList.Where(row => row.CodeDivisionValue == headerRow.ApprovalStatus).Select(r => r.CodeDivisionValueName).FirstOrDefault();
                //}
                //// 2012.06.26 End   入力状況情報のOK ERRORの表示がされないのを修正 
                #endregion
                foreach (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow headerRow in dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows)
                {
                    headerRow.Attribute01 = statusNameList.Where(row => row.CodeDivisionValue == headerRow.ErrorDiv).Select(r => r.CodeDivisionValueName).FirstOrDefault();
                }
                // 2013/09/23 end 入力状況情報は、エラー区分（ErrorDiv）と付き合わせるのが正しいと思われる。
            }
        }

        /// <summary>
        /// 伝票明細の検索を行う。
        /// (BSCSAR05008000145)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void FillSlipDetailNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            // 【DAOCSAR05013】を実施する
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                // 2014.03.18 Start 効率改善対応（商品名取得変更）
                //dataAccessComponent.FillSARSlipDetailError(workEntity, dsEntity);
                dataAccessComponent.FillSARSlipDetailError_GetProductsKanjiNew(workEntity, dsEntity);
                // 2014.03.18 End   効率改善対応（商品名取得変更）
            }

            // 2014.03.18 Start 効率改善対応（商品名取得変更）
            ///////////////////////////////////////////////////////////////////////
            // 上記伝票明細エラーの取得時に商品名も一緒に取得する様にした為、
            // 以下の処理はコメント(2014.03.18 効率改善対応（商品名取得変更）)
            ///////////////////////////////////////////////////////////////////////
//            ISTRProductControlAPI productControlAPI = this.GetSTRProductControlAPIInstance();
//            STRProductCodeExistCheckAndGetDataSet strProductCodeExistCheckAndGetDataSet = new STRProductCodeExistCheckAndGetDataSet();

//            // 【BSCSTR10000000001】実施（DsEntity.伝票情報明細Dsのデータ数分実施）
//            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
//            {
//                ProductsSearchConditionEntity productsSearchConditionEntity = new ProductsSearchConditionEntity()
//                {
//                    SearchBasicDate = detailRow.DisposalChangeDate,
//                    CorporationCode = detailRow.CorporationCode,
//                    SKUCd = detailRow["SKUCd"].ToString()
//                };

//                productControlAPI.ProductsCodeExistCheckAndGet(
//                    productsSearchConditionEntity,
//                    ref strProductCodeExistCheckAndGetDataSet);

//#warning SKN臨時対応
//                //detailRow.Attribute01 = strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet[0]["ProductsKanji"].ToString();
//                if (strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet.Rows.Count > 0)
//                {
//                    detailRow.Attribute01 = strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet[0]["ProductsKanji"].ToString();
//                }
//            }
            // 2014.03.18 End   効率改善対応（商品名取得変更）
        }

        /// <summary>
        /// 伝票番号取得処理を行う。
        /// (BSCSAR05008000146)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void FillSlipNoNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            // 【BSCBYR07015000017】仕入公開APIを実施する => BSCBYR08001000001?!
            if (dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Rows.Count > 0)
            {
                IByrSlipNoPublicBusiness byrSlipNoPublicBusiness = this.GetByrSlipNoPublicBusinessNew();
                string slipNo = byrSlipNoPublicBusiness.GetSlipNo(
                    workEntity.CorporationCode,
                    dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].SlipDiv,
                    dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].SlipTypeCd,
                    null,
                    null,
                    dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].StoreCd);

                if (!string.IsNullOrEmpty(slipNo))
                {
                    workEntity.GetSlipNo = slipNo;
                }
            }
        }

        /// <summary>
        /// 承認依頼を実施する。
        /// (BSCSAR05008000147)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void UpApprovalNew(
           WorkEntity workEntity,
           DsEntity dsEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            // カバーシートとエラートランに承認ステータスを立てる。
            this.UpApprovalCoverSheetMasterNew(workEntity, dsEntity, initEntity);
            this.UpApprovalSlipHeaderErrorNew(workEntity, dsEntity, initEntity);
        }

        /// <summary>
        /// カバー承認依頼を実施する。
        /// (BSCSAR05008000148)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void UpApprovalCoverSheetMasterNew(
           WorkEntity workEntity,
           DsEntity dsEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            // 【DAOCSAR05015】を実施する
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                dataAccessComponent.UpdateApprovalSARCoverSheetMasterNew(workEntity, dsEntity, initEntity);
            }
        }

        /// <summary>
        /// エラートラン承認依頼を実施する。
        /// (BSCSAR05008000149)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name= "initEntity">初期表示Entity</param>
        public virtual void UpApprovalSlipHeaderErrorNew(
           WorkEntity workEntity,
           DsEntity dsEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            // 【DAOCSAR05016】を実施する
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                dataAccessComponent.UpdateApprovalSARSlipHeaderErrorNew(workEntity, dsEntity, initEntity);
            }
        }

        /// <summary>
        /// 商品情報を取得する。
        /// (BSCSAR05008000150)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "itemInfoDataset">商品情報Ds</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void FillItemInfoNew(
           WorkEntity workEntity,
           ItemInfoDataset itemInfoDataset,
           DsEntity dsEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (itemInfoDataset == null)
            {
                throw new ArgumentNullException("itemInfoDataset");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            #endregion

            // 商品情報取得のオブジェクトを実施する(BSCSAR05008000151)
            this.FillItemInfoAPINew(workEntity, itemInfoDataset);

            //2015.11.13  start 売変廃棄伝票画面金額設定しないため削除(STRDeliveryPatternのデータにより二行取得してしまうから)
            //if (itemInfoDataset.ItemInfo.Count == 1)
            //{
            //2015.11.13  end 売変廃棄伝票画面金額設定しないため削除

            //2015.11.30 start マスターラインに商品コード存在しないときシステムエラー修正
            if (itemInfoDataset.ItemInfo.Count > 0)
            {
                //2015.11.30 end マスターラインに商品コード存在しないときシステムエラー修正

                using (GetStockResultPublicDataSet stockResultPublicDataSet = new GetStockResultPublicDataSet())
                using (Category2DisclosureDataSet category2DisclosureDataSet = new Category2DisclosureDataSet())
                using (Category1DisclosureDataSet category1DisclosureDataSet = new Category1DisclosureDataSet())
                // 2012.07.19 Start 特売情報の取得方法変更
                //using (SPRBargainPlanningProductPublicInformationDataSet bargainPlanningProductPublicInformationDataSet = new SPRBargainPlanningProductPublicInformationDataSet())
                using (SARBargainPlanningProductStorePublicInformationSalesTableAdapter sarBargainPlanningProductStorePublicInformationSalesTableAdapter = new SARBargainPlanningProductStorePublicInformationSalesTableAdapter())
                using (SARBargainPlanningProductStorePublicInformationDataSet sarBargainPlanningProductStorePublicInformationDataSet = new SARBargainPlanningProductStorePublicInformationDataSet())
                // 2012.07.19 End   特売情報の取得方法変更
                {
#warning SKN臨時対応　商品情報取得 -->
                    //// 論理在庫取得(BSCIVR01901000007)
                    //StockSearchPublicEntity stockSearchPublicEntity = new StockSearchPublicEntity()
                    //{
                    //    SubSystemDiv = workEntity.IVGetSystemDiv,
                    //    GetMethodDiv = (StockSearchPublicEntity.GetMethod)int.Parse(workEntity.IVGetDiv),
                    //    BaseDate = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDate,
                    //    CorporationCode = workEntity.CorporationCode,
                    //    StoreCd = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].StoreCd,
                    //    SKUCd = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[0]["SKUCd"].ToString()
                    //};

                    //IIVRStockControlAPI stockControlAPI = this.GetIVRStockControlAPIInstance();

                    //// ①　(OUT)在庫取得結果公開DataSet
                    //stockControlAPI.GetStock(stockSearchPublicEntity, stockResultPublicDataSet);

                    //// ＣＭＲ分類２公開情報取得(BSCCMR01003000001)
                    //Category2DisclosureSearchConditionEntity category2DisclosureSearchEntity = new Category2DisclosureSearchConditionEntity()
                    //{
                    //    BaseDate = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDate,
                    //    CorporationCode = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].CorporationCode,
                    //    Category1CD = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].Category1Cd,
                    //    Category2CD = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].Category2Cd
                    //};

                    //ICmrCategoryAPI cmrCategoryAPI = this.GetCmrCategoryAPIInstance();
                    //cmrCategoryAPI.GetCategory2Disclosure(workEntity.Subsystem, category2DisclosureSearchEntity, category2DisclosureDataSet);
                    //if (category2DisclosureDataSet.Category2Disclosure.Count == 0)
                    //{
                    //    // ＣＭＲ分類１公開情報取得
                    //    Category1DisclosureSearchConditionEntity category1DisclosureSearchEntity = new Category1DisclosureSearchConditionEntity();
                    //    category1DisclosureSearchEntity.BaseDate = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDate;
                    //    category1DisclosureSearchEntity.CorporationCode = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].CorporationCode;
                    //    category1DisclosureSearchEntity.Category1CD = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].Category1Cd;

                    //    cmrCategoryAPI.GetCategory1Disclosure(workEntity.Subsystem, category1DisclosureSearchEntity, category1DisclosureDataSet);
                    //}

                    //// 取得されたＣＭＲ分類２（または１）公開情報Datasetから、自動売変フラグを分岐判断します。
                    //string autoSalesChangeFlg = (category2DisclosureDataSet.Category2Disclosure.Count > 0) ?
                    //    category2DisclosureDataSet.Category2Disclosure[0].AutoSalesChangeFlg :
                    //    category1DisclosureDataSet.Category1Disclosure[0].AutoSalesChangeFlg;

                    //// 店別商品情報取得(BSCSTR10000000005)
                    //StoreProductsSearchConditionEntity storeProductsSearchConditionEntity = new StoreProductsSearchConditionEntity()
                    //{
                    //    SearchBasicDate = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDate,
                    //    CorporationCode = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].CorporationCode,
                    //    SKUCd = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[0]["SKUCd"].ToString(),
                    //    StoreCd = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].StoreCd
                    //};

                    //ISTRProductControlAPI strProductControlAPI = this.GetSTRProductControlAPIInstance();
                    //STRStoreProductsDisclosureDataSet storeProductsDisclosureDataSet = new STRStoreProductsDisclosureDataSet();

                    //// ②　取得されたSTR店別商品公開用DataSet.商品名称（漢字）
                    //strProductControlAPI.GetStoreProductsLineup(string.Empty, storeProductsSearchConditionEntity, ref storeProductsDisclosureDataSet);

                    //STRSKUPricePatternDisclosureDataSet skuPricePatternDisclosureDataSet = new STRSKUPricePatternDisclosureDataSet();

                    //// 「定番売価を取得」(自動売変フラグ＝自動売変可能のとき)
                    //if (autoSalesChangeFlg == Constants.OnFlag)
                    //{
                    //    // 売価公開パターン取得(BSCSTR10000000006)
                    //    ProductsSearchConditionEntity productSearchConditionEntity = new ProductsSearchConditionEntity()
                    //    {
                    //        SearchBasicDate = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDate,
                    //        CorporationCode = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].CorporationCode,
                    //        SKUCd = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[0]["SKUCd"].ToString()
                    //    };

                    //    // ③　（OUT）STRSKU売価パターン公開用DataSet
                    //    strProductControlAPI.GetSKUPricePatternDisclosure(productSearchConditionEntity, ref skuPricePatternDisclosureDataSet);
                    //}
                    //else if (autoSalesChangeFlg == Constants.OffFlag)
                    //{
                    //    // 「特売売価を取得」(自動売変フラグ＝自動売変不可能のとき)
                    //    // 特売企画商品公開情報取得(BSCSPR09004000001)
                    //    IBargainPlanningProductPublicInformationBusiness bargainPlanningProductPublicInformationBusiness = this.GetBargainPlanningProductPublicInformationBusiness();

                    //    // ④　（OUT）SPR特売企画商品公開情報DataSet
                    //    bargainPlanningProductPublicInformationBusiness.BargainPlanningProductPublicInformationSearch(
                    //        dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].CorporationCode,
                    //        dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDate.ToString("yyyyMMdd"),
                    //        string.Empty,
                    //        dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].Category1Cd,
                    //        itemInfoDataset.ItemInfo[0]["SKUCd"].ToString(),
                    //        bargainPlanningProductPublicInformationDataSet);
                    //}

                    //// 最終的にの処理
                    //itemInfoDataset.ItemInfo[0].ProductsKanji = storeProductsDisclosureDataSet.STRStoreProductsDisclosure[0]["ProductsKanji"].ToString();
                    //workEntity.ProductInfo_LogicalStockQty = stockResultPublicDataSet.GetStockResult.Select(r => r.BookStockQty).FirstOrDefault().ToString();
                    //workEntity.ProductInfo_SalePrice =
                    //    (autoSalesChangeFlg == Constants.OnFlag) ?
                    //    skuPricePatternDisclosureDataSet.STRSKUPricePatternDisclosure.Select(r => r.SaleUnitPrice).FirstOrDefault().ToString() :
                    //    bargainPlanningProductPublicInformationDataSet.SPRBargainPlanningProductPublicInformation.Select(r => r.SaleUnitPrice).FirstOrDefault().ToString();


                    // 論理在庫取得(BSCIVR01901000007)
                    StockSearchPublicEntity stockSearchPublicEntity = new StockSearchPublicEntity()
                    {
                        SubSystemDiv = workEntity.IVGetSystemDiv,
                        GetMethodDiv = (StockSearchPublicEntity.GetMethod)int.Parse(workEntity.IVGetDiv),
                        // 2012.07.24 Start 商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                        //BaseDate = Convert.ToDateTime(workEntity.BatchDate),
                        BaseDate = Convert.ToDateTime(workEntity.ChangeDate),
                        // 2012.07.24 End   商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                        CorporationCode = workEntity.CorporationCode,
                        StoreCd = workEntity.StoreCd,
                        SKUCd = workEntity.ItemCd,
                    };

                    IIVRStockControlAPI stockControlAPI = this.GetIVRStockControlAPIInstanceNew();

                    // ①　(OUT)在庫取得結果公開DataSet
                    stockControlAPI.GetStock(stockSearchPublicEntity, stockResultPublicDataSet);

                    //// ＣＭＲ分類２公開情報取得(BSCCMR01003000001)
                    //Category2DisclosureSearchConditionEntity category2DisclosureSearchEntity = new Category2DisclosureSearchConditionEntity()
                    //{
                    //    BaseDate = Convert.ToDateTime(workEntity.BatchDate),
                    //    CorporationCode = workEntity.CorporationCode,
                    //    Category1CD = workEntity.Category1Cd,
                    //    Category2CD = workEntity.Category2Cd
                    //};

                    //ICmrCategoryAPI cmrCategoryAPI = this.GetCmrCategoryAPIInstance();
                    //cmrCategoryAPI.GetCategory2Disclosure(workEntity.Subsystem, category2DisclosureSearchEntity, category2DisclosureDataSet);
                    //if (category2DisclosureDataSet.Category2Disclosure.Count == 0)
                    //{
                    //    // ＣＭＲ分類１公開情報取得
                    //    Category1DisclosureSearchConditionEntity category1DisclosureSearchEntity = new Category1DisclosureSearchConditionEntity();
                    //    category1DisclosureSearchEntity.BaseDate = Convert.ToDateTime(workEntity.BatchDate);
                    //    category1DisclosureSearchEntity.CorporationCode = workEntity.CorporationCode;
                    //    category1DisclosureSearchEntity.Category1CD = workEntity.Category1Cd;

                    //    cmrCategoryAPI.GetCategory1Disclosure(workEntity.Subsystem, category1DisclosureSearchEntity, category1DisclosureDataSet);
                    //}

                    //// 取得されたＣＭＲ分類２（または１）公開情報Datasetから、自動売変フラグを分岐判断します。
                    //string autoSalesChangeFlg = (category2DisclosureDataSet.Category2Disclosure.Count > 0) ?
                    //    category2DisclosureDataSet.Category2Disclosure[0].AutoSalesChangeFlg :
                    //    category1DisclosureDataSet.Category1Disclosure[0].AutoSalesChangeFlg;

                    // 2012.07.19 Start 特売情報の取得方法変更
                    //string autoSalesChangeFlg = Constants.OnFlag;

                    // ※コメントアウトされていた箇所をコピーし解除
                    // ＣＭＲ分類２公開情報取得(BSCCMR01003000001)
                    Category2DisclosureSearchConditionEntity category2DisclosureSearchEntity = new Category2DisclosureSearchConditionEntity()
                    {
                        // 2012.07.24 Start 商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                        //BaseDate = Convert.ToDateTime(workEntity.BatchDate),
                        BaseDate = Convert.ToDateTime(workEntity.ChangeDate),
                        // 2012.07.24 End   商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                        CorporationCode = workEntity.CorporationCode,
                        Category1CD = workEntity.Category1Cd,
                        Category2CD = workEntity.Category2Cd
                    };

                    ICmrCategoryAPI cmrCategoryAPI = this.GetCmrCategoryAPIInstanceNew();
                    cmrCategoryAPI.GetCategory2Disclosure(workEntity.Subsystem, category2DisclosureSearchEntity, category2DisclosureDataSet);
                    if (category2DisclosureDataSet.Category2Disclosure.Count == 0)
                    {
                        // ＣＭＲ分類１公開情報取得
                        Category1DisclosureSearchConditionEntity category1DisclosureSearchEntity = new Category1DisclosureSearchConditionEntity();
                        // 2012.07.24 Start 商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                        //category1DisclosureSearchEntity.BaseDate = Convert.ToDateTime(workEntity.BatchDate);
                        category1DisclosureSearchEntity.BaseDate = Convert.ToDateTime(workEntity.ChangeDate);
                        // 2012.07.24 End   商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                        category1DisclosureSearchEntity.CorporationCode = workEntity.CorporationCode;
                        category1DisclosureSearchEntity.Category1CD = workEntity.Category1Cd;

                        cmrCategoryAPI.GetCategory1Disclosure(workEntity.Subsystem, category1DisclosureSearchEntity, category1DisclosureDataSet);
                    }

                    // 取得されたＣＭＲ分類２（または１）公開情報Datasetから、自動売変フラグを分岐判断します。
                    string autoSalesChangeFlg = (category2DisclosureDataSet.Category2Disclosure.Count > 0) ?
                        category2DisclosureDataSet.Category2Disclosure[0].AutoSalesChangeFlg :
                        category1DisclosureDataSet.Category1Disclosure[0].AutoSalesChangeFlg;
                    // 2012.07.19 End   特売情報の取得方法変更

                    //// 店別商品情報取得(BSCSTR10000000005)
                    //StoreProductsSearchConditionEntity storeProductsSearchConditionEntity = new StoreProductsSearchConditionEntity()
                    //{
                    //    SearchBasicDate = Convert.ToDateTime(workEntity.BatchDate),
                    //    CorporationCode = workEntity.CorporationCode,
                    //    SKUCd = workEntity.ItemCd,
                    //    StoreCd = workEntity.StoreCd
                    //};


                    // 2013.10.18 AST Chg Start
                    #region 修正前
                    //ISTRProductControlAPI strProductControlAPI = this.GetSTRProductControlAPIInstance();
                    #endregion
                    // 標準価格取得処理を使用する
                    ISTRProductControlAPI strProductControlAPI = (ISTRProductControlAPI)ClassObjectUtil.GetBusinessInstance("BSCSTR10000", typeof(ISTRProductControlAPI));
                    // 2013.10.18 AST Chg End
                    //STRStoreProductsDisclosureDataSet storeProductsDisclosureDataSet = new STRStoreProductsDisclosureDataSet();

                    //// ②　取得されたSTR店別商品公開用DataSet.商品名称（漢字）
                    //strProductControlAPI.GetStoreProductsLineup(string.Empty, storeProductsSearchConditionEntity, ref storeProductsDisclosureDataSet);

                    // 2013.10.18 AST Chg Start
                    #region 修正前
                    //STRSKUPricePatternDisclosureDataSet skuPricePatternDisclosureDataSet = new STRSKUPricePatternDisclosureDataSet();

                    //// 2013.08.27 Start 売価の取得方法変更 
                    //// 標準売価を取得する
                    //// 売価公開パターン取得(BSCSTR10000000006)
                    //ProductsSearchConditionEntity productSearchConditionEntity = new ProductsSearchConditionEntity()
                    //{
                    //    SearchBasicDate = Convert.ToDateTime(workEntity.ChangeDate),
                    //    CorporationCode = workEntity.CorporationCode,
                    //    SKUCd = workEntity.ItemCd
                    //};

                    //// STRSKU売価パターン公開用DataSet
                    //strProductControlAPI.GetSKUPricePatternDisclosure(productSearchConditionEntity, ref skuPricePatternDisclosureDataSet);

                    ////
                    //workEntity.StandardSellingPrice = skuPricePatternDisclosureDataSet.STRSKUPricePatternDisclosure.Select(r => r.SaleUnitPrice).FirstOrDefault();
                    #endregion
                    // 標準価格取得処理を使用する
                    // --> 2013/11/01 マスタ売価対応
                    #region DEL
                    //decimal taxExcludedPrice = 0m;
                    //decimal preTaxSaleUnitPrice = 0m;
                    //strProductControlAPI.CalcTaxIncludedPrice(
                    //            Convert.ToDateTime(workEntity.ChangeDate),
                    //            workEntity.ItemCd,
                    //            string.Empty,
                    //            ref taxExcludedPrice,
                    //            ref preTaxSaleUnitPrice);
                    //workEntity.StandardSellingPrice = preTaxSaleUnitPrice;
                    #endregion

                    STRProductWithPatternDisclosureDataSet strProductWithPatternDisclosureDs = null;
                    using (strProductWithPatternDisclosureDs = new STRProductWithPatternDisclosureDataSet())
                    {
                        ProductsSearchConditionEntity productsSearchConditionEnt = new ProductsSearchConditionEntity();
                        productsSearchConditionEnt.CorporationCode = workEntity.CorporationCode;
                        productsSearchConditionEnt.SKUCd = itemInfoDataset.ItemInfo[0]["SKUCd"].ToString();
                        productsSearchConditionEnt.SearchBasicDate = workEntity.ChangeDate.Value;

                        strProductControlAPI.GetProductBasicAttributeWithPattern(
                            "9",
                            productsSearchConditionEnt,
                            ref strProductWithPatternDisclosureDs);
                        //デバッグ用
                        //object[] parameter = { "テスト：2281行目：strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Rows.Count：" + strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Rows.Count };
                        //throw RetailBusinessException.CreateInstance(ErrorKey.ERRCCMRB1056, parameter);


                        if (strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Rows.Count > 0)
                        {

                            // 定番売価を取得
                            if (strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Any(r => r.SellPricePatternNo == "1")
                                && strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Where(r => r.SellPricePatternNo == "1").First().IsSaleUnitPriceNull() == false)
                            {
                                workEntity.StandardSellingPrice = strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Where(r => r.SellPricePatternNo == "1").First().SaleUnitPrice;
                            }
                            else
                            {
                                productsSearchConditionEnt = new ProductsSearchConditionEntity();
                                productsSearchConditionEnt.CorporationCode = workEntity.CorporationCode;
                                productsSearchConditionEnt.SKUCd = itemInfoDataset.ItemInfo[0]["SKUCd"].ToString();
                                productsSearchConditionEnt.SearchBaseDate = workEntity.ChangeDate.Value;

                                STRSKUPurchasePatternDisclosureDataSet strSKUPurchasePatternDisclosureDs = null;
                                using (strSKUPurchasePatternDisclosureDs = new STRSKUPurchasePatternDisclosureDataSet())
                                {
                                    strProductControlAPI.GetSKUPurchasePatternDisclosure(
                                        productsSearchConditionEnt,
                                        "",
                                        "",
                                        "",
                                        ref strSKUPurchasePatternDisclosureDs);

                                    if (strSKUPurchasePatternDisclosureDs.STRSKUPurchasePatternDisclosure.Count > 0
                                        && strSKUPurchasePatternDisclosureDs.STRSKUPurchasePatternDisclosure.Any(r => r.InvStock == "1")
                                        && strSKUPurchasePatternDisclosureDs.STRSKUPurchasePatternDisclosure.Where(r => r.InvStock == "1").First().IsPriceAtStockNull() == false)
                                    {
                                        workEntity.StandardSellingPrice =
                                            strSKUPurchasePatternDisclosureDs.STRSKUPurchasePatternDisclosure.Where(r => r.InvStock == "1").First().PriceAtStock;
                                    }
                                    else
                                    {
                                        workEntity.StandardSellingPrice = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            workEntity.StandardSellingPrice = 0;
                        }
                    }
                    // <-- 2013/11/01 マスタ売価対応
                    // 2013.10.18 AST Chg End

                    // 標準売価か最安値を設定する
                    if (STANDARD_SELLING_PRICE_KBN.Equals(workEntity.SalePriceGetKbn))
                    {
                        // 標準売価を設定する
                        workEntity.ProductInfo_SalePrice = workEntity.StandardSellingPrice.ToString();
                    }
                    else if (LOWEST_PRICE_KBN.Equals(workEntity.SalePriceGetKbn))
                    {
                        // ④　（OUT）SAR特売企画商品情報DataSet
                        sarBargainPlanningProductStorePublicInformationSalesTableAdapter.BargainPlanningProductStorePublicInformationSalesSearch(
                            workEntity.CorporationCode,
                            workEntity.ChangeDate.Value.ToString("yyyyMMdd"),
                            string.Empty,
                            workEntity.StoreCd,
                            workEntity.Category1Cd,
                            itemInfoDataset.ItemInfo[0]["SKUCd"].ToString(),
                            sarBargainPlanningProductStorePublicInformationDataSet);

                        if (sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Rows.Count != 0)
                        {
                            // 最安値を取得しセット
                            workEntity.ProductInfo_SalePrice = sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Select(r => r.SaleUnitPrice).Min().ToString();
                        }
                        else
                        {
                            // 特売売価が取得できない場合は定番売価を設定する
                            workEntity.ProductInfo_SalePrice = workEntity.StandardSellingPrice.ToString();
                        }
                    }
                    // --> 2013/09/26 店別売価対応
                    else if (STORE_PRICE_KBN.Equals(workEntity.SalePriceGetKbn))
                    {
                        // 店別売価設定のとき
                        #region (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                        // --> 2013/11/01 マスタ売価対応
                        #region DEL
                        //// 2013.10.18 AST Chg Start
                        //#region 修正前
                        ////// ＳＴＲ店別商品公開用DataSet
                        ////STRStoreProductsDisclosureDataSet strStoreProductsDisclosureDataSet = null;
                        ////using (strStoreProductsDisclosureDataSet = new STRStoreProductsDisclosureDataSet())
                        ////{
                        ////// 商品店検索条件Entity
                        ////StoreProductsSearchConditionEntity storeProductsSearchConditionEntity = new StoreProductsSearchConditionEntity()
                        ////{
                        ////CorporationCode = workEntity.CorporationCode,
                        ////StoreCd = workEntity.StoreCd,
                        ////SearchBasicDate = workEntity.ChangeDate,
                        ////SKUCd = itemInfoDataset.ItemInfo[0]["SKUCd"].ToString()
                        ////};
                        ////
                        ////// (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                        ////// ※区分="9"(取扱い終了も取得する)
                        ////strProductControlAPI.GetStoreProductsLineup(
                        ////"9",
                        ////storeProductsSearchConditionEntity,
                        ////ref strStoreProductsDisclosureDataSet);
                        ////
                        ////if (strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure.Count > 0)
                        ////{
                        ////// 取得した商品情報をセット(先頭の１件のみをセット)
                        ////// 最安値を取得しセット
                        ////workEntity.ProductInfo_SalePrice = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].SaleUnitPrice.ToString();
                        ////}
                        //#endregion
                        //taxExcludedPrice = 0m;
                        //preTaxSaleUnitPrice = 0m;
                        //strProductControlAPI.CalcTaxIncludedPrice(
                        //            Convert.ToDateTime(workEntity.ChangeDate),
                        //            itemInfoDataset.ItemInfo[0]["SKUCd"].ToString(),
                        //            workEntity.StoreCd,
                        //            ref taxExcludedPrice,
                        //            ref preTaxSaleUnitPrice);
                        //workEntity.StandardSellingPrice = preTaxSaleUnitPrice;
                        //if (workEntity.StandardSellingPrice > 0)
                        //{
                        //    workEntity.ProductInfo_SalePrice = preTaxSaleUnitPrice.ToString();
                        //}
                        //// 2013.10.18 AST Chg End
                        //else
                        //{
                        //    // 店別商品情報が取得できなかった場合
                        //    // 店別商品情報が取得できない場合は定番売価を設定する
                        //    workEntity.ProductInfo_SalePrice = workEntity.StandardSellingPrice.ToString();
                        //}
                        //// 2013.10.18 AST Del Start
                        //#region 削除
                        ////}
                        //#endregion
                        //// 2013.10.18 AST Del End
                        #endregion

                        // ＳＴＲ店別商品公開用DataSet
                        STRStoreProductsDisclosureDataSet strStoreProductsDisclosureDataSet = null;
                        using (strStoreProductsDisclosureDataSet = new STRStoreProductsDisclosureDataSet())
                        {
                            // 商品店検索条件Entity
                            StoreProductsSearchConditionEntity storeProductsSearchConditionEntity = new StoreProductsSearchConditionEntity()
                            {
                                CorporationCode = workEntity.CorporationCode,
                                StoreCd = workEntity.StoreCd,
                                SearchBasicDate = workEntity.ChangeDate,
                                SKUCd = itemInfoDataset.ItemInfo[0]["SKUCd"].ToString()
                            };

                            // (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                            // ※区分="9"(取扱い終了も取得する)
                            strProductControlAPI.GetStoreProductsLineup(
                            "9",
                            storeProductsSearchConditionEntity,
                            ref strStoreProductsDisclosureDataSet);

                            if (strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure.Count > 0)
                            {
                                // 取得した商品情報をセット(先頭の１件のみをセット)
                                // 最安値を取得しセット
                                workEntity.ProductInfo_SalePrice = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].SaleUnitPrice.ToString();
                            }
                            else
                            {
                                // 標準売価を設定する
                                workEntity.ProductInfo_SalePrice = workEntity.StandardSellingPrice.ToString();
                            }
                            // <-- 2013/11/01 マスタ売価対応

                        }
                        #endregion

                    }
                    // <-- 2013/09/26 店別売価対応

                    #region コメントアウト
                    //// 「定番売価を取得」(自動売変フラグ＝自動売変可能のとき)
                    //if (autoSalesChangeFlg == Constants.OnFlag)
                    //{
                    //    // 売価公開パターン取得(BSCSTR10000000006)
                    //    ProductsSearchConditionEntity productSearchConditionEntity = new ProductsSearchConditionEntity()
                    //    {
                    //        // 2012.07.24 Start 商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                    //        //SearchBasicDate = Convert.ToDateTime(workEntity.BatchDate),
                    //        SearchBasicDate = Convert.ToDateTime(workEntity.ChangeDate),
                    //        // 2012.07.24 End   商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                    //        CorporationCode = workEntity.CorporationCode,
                    //        SKUCd = workEntity.ItemCd
                    //    };

                    //    // ③　（OUT）STRSKU売価パターン公開用DataSet
                    //    strProductControlAPI.GetSKUPricePatternDisclosure(productSearchConditionEntity, ref skuPricePatternDisclosureDataSet);

                    //    // 2012.07.19 Start 特売情報の取得方法変更
                    //    workEntity.ProductInfo_SalePrice = skuPricePatternDisclosureDataSet.STRSKUPricePatternDisclosure.Select(r => r.SaleUnitPrice).FirstOrDefault().ToString();
                    //    // 2012.07.19 End   特売情報の取得方法変更
                    //}
                    //else if (autoSalesChangeFlg == Constants.OffFlag)
                    //{
                    //    // 「特売売価を取得」(自動売変フラグ＝自動売変不可能のとき)
                    //    // 2012.07.19 Start 特売情報の取得方法変更
                    //    //// 特売企画商品公開情報取得(BSCSPR09004000001)
                    //    //IBargainPlanningProductPublicInformationBusiness bargainPlanningProductPublicInformationBusiness = this.GetBargainPlanningProductPublicInformationBusiness();

                    //    //// ④　（OUT）SPR特売企画商品公開情報DataSet
                    //    //bargainPlanningProductPublicInformationBusiness.BargainPlanningProductPublicInformationSearch(
                    //    //    workEntity.CorporationCode,
                    //    //    // 2012.07.24 Start 商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                    //    //    //Convert.ToDateTime(workEntity.BatchDate).ToString("yyyyMMdd"),
                    //    //    Convert.ToDateTime(workEntity.ChangeDate).ToString("yyyyMMdd"),
                    //    //    // 2012.07.24 End   商品情報、特売情報の取得でバッチ日付を使用しているので伝票日付で取得するように修正
                    //    //    string.Empty,
                    //    //    workEntity.Category1Cd,
                    //    //    itemInfoDataset.ItemInfo[0]["SKUCd"].ToString(),
                    //    //    bargainPlanningProductPublicInformationDataSet);

                    //// ④　（OUT）SAR特売企画商品情報DataSet
                    //sarBargainPlanningProductStorePublicInformationSalesTableAdapter.BargainPlanningProductStorePublicInformationSalesSearch(
                    //    workEntity.CorporationCode,
                    //    workEntity.ChangeDate.Value.ToString("yyyyMMdd"),
                    //    string.Empty,
                    //    workEntity.StoreCd,
                    //    workEntity.Category1Cd,
                    //    itemInfoDataset.ItemInfo[0]["SKUCd"].ToString(),
                    //    sarBargainPlanningProductStorePublicInformationDataSet);

                    //if (sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Rows.Count != 0)
                    //{
                    //    // 特売売価を取得をセット
                    //    workEntity.ProductInfo_SalePrice = sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Select(r => r.SaleUnitPrice).FirstOrDefault().ToString();
                    //}
                    //else
                    //{
                    //    // 特売売価が取得できない場合は定番売価を取得する
                    //    // 売価公開パターン取得(BSCSTR10000000006)
                    //    ProductsSearchConditionEntity productSearchConditionEntity = new ProductsSearchConditionEntity()
                    //    {
                    //        SearchBasicDate = Convert.ToDateTime(workEntity.ChangeDate),
                    //        CorporationCode = workEntity.CorporationCode,
                    //        SKUCd = workEntity.ItemCd
                    //    };

                    //    // STRSKU売価パターン公開用DataSet
                    //    strProductControlAPI.GetSKUPricePatternDisclosure(productSearchConditionEntity, ref skuPricePatternDisclosureDataSet);

                    //    workEntity.ProductInfo_SalePrice = skuPricePatternDisclosureDataSet.STRSKUPricePatternDisclosure.Select(r => r.SaleUnitPrice).FirstOrDefault().ToString();
                    //}
                    //// 2012.07.19 End   特売情報の取得方法変更
                    //}
                    #endregion
                    // 2013.08.27 End 売価の取得方法変更

                    // 最終的にの処理
                    //itemInfoDataset.ItemInfo[0].ProductsKanji = storeProductsDisclosureDataSet.STRStoreProductsDisclosure[0]["ProductsKanji"].ToString();
                    workEntity.ProductInfo_LogicalStockQty = stockResultPublicDataSet.GetStockResult.Select(r => r.BookStockQty).FirstOrDefault().ToString();
                    // 2012.07.19 Start 特売情報の取得方法変更
                    //workEntity.ProductInfo_SalePrice =
                    //    (autoSalesChangeFlg == Constants.OnFlag) ?
                    //    skuPricePatternDisclosureDataSet.STRSKUPricePatternDisclosure.Select(r => r.SaleUnitPrice).FirstOrDefault().ToString() :
                    //    bargainPlanningProductPublicInformationDataSet.SPRBargainPlanningProductPublicInformation.Select(r => r.SaleUnitPrice).FirstOrDefault().ToString();
                    // 2012.07.19 End   特売情報の取得方法変更

                    // 2013.10.18 AST Del Start
                    // 標準価格取得処理を使用する
                    #region 削除
                    //skuPricePatternDisclosureDataSet.Dispose();
                    #endregion
                    // 2013.10.18 AST Del End
#warning SKN臨時対応　商品情報取得 <--
                }
            //2015.11.13  start 売変廃棄伝票画面金額設定しないため削除
            //}
            //2015.11.13  end 売変廃棄伝票画面金額設定しないため削除
          //2015.11.30 start マスターラインに商品コード存在しないときシステムエラー修正
            }
          //2015.11.30 end マスターラインに商品コード存在しないときシステムエラー修正

        }

        /// <summary>
        /// 商品情報を取得する。
        /// (BSCSAR05008000151)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "itemInfoDataset">商品情報Ds</param>
        public virtual void FillItemInfoAPINew(
           WorkEntity workEntity,
           ItemInfoDataset itemInfoDataset)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (itemInfoDataset == null)
            {
                throw new ArgumentNullException("itemInfoDataset");
            }

            #endregion

            // 公開API[BSCSTR10000000001]の実施 - BSCSTR10000000001?
            ISTRProductControlAPI productControlAPI = this.GetSTRProductControlAPIInstanceNew();

            StoreProductsSearchConditionEntity productsSearchConditionEntity = new StoreProductsSearchConditionEntity()
            {
                SearchBasicDate = (DateTime)workEntity.ChangeDate,
                CorporationCode = workEntity.CorporationCode,
                OwnProductsCd = workEntity.ItemCd,
                StoreCd = workEntity.StoreCd
            };

            STRStoreProductsDisclosureDataSet strStoreProductsDisclosureDataSet = new STRStoreProductsDisclosureDataSet();
            productControlAPI.GetStoreProductsLineup(
                string.Empty,
                productsSearchConditionEntity,
                ref strStoreProductsDisclosureDataSet);

            itemInfoDataset.ItemInfo.Merge(strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure, true, System.Data.MissingSchemaAction.Ignore);
        }
        
        // 2013/09/17 アドオン対応：カバー修正情報取得追加 ISC:yshindo start

        /// <summary>
        /// カバー修正情報を取得する。
        /// (BSCSAR05008000152)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void FillCoverSheetMasterNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            // 【DAOCSAR05022】を実施する
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                dataAccessComponent.FillSARCoverSheetMasterForUpdateNew(workEntity, dsEntity);
            }
        }

        // 2013/09/17 アドオン対応：カバー修正情報取得追加 ISC:yshindo end

        /// <summary>
        /// 伝票計上処理を行う。売変廃棄画面用
        /// (BSCSAR05008000153)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        /// <param name="coverSheetMasterEntity">カバーシートマスタワークEntity</param>
        public virtual void UpInsSlipNew(
           WorkEntity workEntity,
           DsEntity dsEntity,
           InitialDisplayEntity initEntity,
            // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
           CoverSheetMasterEntity coverSheetMasterEntity)
        // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            //START 20220722 orion.ri 伝票修正モードで操作する途中でカバーシートで承認否認する可能性があるので、最新のステータスを伝票情報に反映する
            DsEntity newestEntity = new DsEntity();

            newestEntity.CoverSheetMasterDataset = new CoverSheetMasterDataset();

            newestEntity.SARSlipHeaderErrorDataset = new SARSlipHeaderErrorInfoDataset();

            newestEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.Merge(dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1);

            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                dataAccessComponent.FillSARSlipHeaderErrorNew(workEntity, newestEntity);
            }

            if (newestEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Any()) {

                dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApprovalStatus = newestEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApprovalStatus;
                dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApproverCd = newestEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApproverCd;
                dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApprovalDate = newestEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApprovalDate;
                dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1.FirstOrDefault().ApprovalStatus = newestEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.FirstOrDefault().ApprovalStatus;
            }
            //End 20220722 orion.ri


            // 2013/09/23 k.haraikawa start 税抜き計算処理
            // --> 2013/11/01 マスタ売価対応
            //string cmcTaxRateAPIId = "BSCCMR01010000";
            //ICmcTaxRateAPI cmcTaxRateAPI = (ICmcTaxRateAPI)ClassObjectUtil.GetBusinessInstance(cmcTaxRateAPIId, typeof(ICmcTaxRateAPI));
            // <-- 2013/11/01 マスタ売価対応

            // 2013.11.02 AST Add Start
            string allStoreCd = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_ALL_STORE_CODE, workEntity.CorporationCode);
            // 2013.11.02 AST Add End

            // --> 2013/11/01 マスタ売価対応
            //// ヘッダのSlipDate取得
            //DateTime WorkSlipDate = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDate;

            //string workTaxCd;
            //int rowcount = 0;
            //string outTaxDiv, outTaxDiv2, outTaxDiv3;
            //decimal outTaxRate, outTaxRate2, outTaxRate3;
            //decimal outTax, outTax2, outTax3;
            //decimal outPrice, outPrice2, outPrice3;
            //decimal workNewPrice, workOldPrice, workSlipOldPrice;

            // <-- 2013/11/01 マスタ売価対応
            // --> 2013/11/01 マスタ売価対応
            #region DEL
            //foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
            //{
                ////税抜区分取得
                //ISTRProductControlAPI strProductControlAPI = this.GetSTRProductControlAPIInstance();
                //STRProductDisclosureDataSet strProductDisclosureDataSet = new STRProductDisclosureDataSet();
                //strProductControlAPI.GetProductInfoByScannedData(
                //    dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].CorporationCode,
                //    detailRow.ScanningCd,
                //    dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDate,
                //    ref strProductDisclosureDataSet);
                //STRProductDisclosureDataSet.STRProductBasicAttributeDisclosureRow strRow = null;
                //if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Rows.Count != 0)
                //{
                //    strRow = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0];
                //}
                //workTaxCd = strRow.TaxCd;

                //// 2013.10.21 AST Chg Start
                //#region 修正前
                ////workNewPrice = detailRow.IsAttribute02Null() || string.IsNullOrWhiteSpace(detailRow.Attribute02)
                ////? detailRow.SlipNewPrice : Convert.ToDecimal(detailRow.Attribute02);
                ////workOldPrice = detailRow.IsAttribute03Null() || string.IsNullOrWhiteSpace(detailRow.Attribute03)
                ////? detailRow.SlipOldPrice : Convert.ToDecimal(detailRow.Attribute03);
                //#endregion
                //workOldPrice = detailRow.IsAttribute02Null() || string.IsNullOrWhiteSpace(detailRow.Attribute02)
                //    ? detailRow.SlipOldPrice : Convert.ToDecimal(detailRow.Attribute02);
                //workNewPrice = detailRow.IsAttribute03Null() || string.IsNullOrWhiteSpace(detailRow.Attribute03)
                //    ? detailRow.SlipNewPrice : Convert.ToDecimal(detailRow.Attribute03);
                //// 2013.10.21 AST Chg End
                //workSlipOldPrice = detailRow.IsAttribute04Null() || string.IsNullOrWhiteSpace(detailRow.Attribute04)
                //    ? detailRow.SlipDisposalUnitPrica : Convert.ToDecimal(detailRow.Attribute04);

                //// 2013.10.18 AST Chg Start
                ////#region 修正前
                //////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", WorkSlipDate, workTaxCd, workNewPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                //////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", WorkSlipDate, workTaxCd, workOldPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv2, out outTaxRate2, out outTax2, out outPrice2);
                //////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", WorkSlipDate, workTaxCd, workSlipOldPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv3, out outTaxRate3, out outTax3, out outPrice3);
                ////#endregion
                //// 税区分が非課税以外は税はがしを行う
                //this.CalcConsumptionTaxExcluded("SA", WorkSlipDate, workTaxCd, workNewPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                //this.CalcConsumptionTaxExcluded("SA", WorkSlipDate, workTaxCd, workOldPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv2, out outTaxRate2, out outTax2, out outPrice2);
                //this.CalcConsumptionTaxExcluded("SA", WorkSlipDate, workTaxCd, workSlipOldPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv3, out outTaxRate3, out outTax3, out outPrice3);
                //// 2013.10.18 AST Chg End

                //// 2013/09/23 k.haraikawa SAR伝票明細は予備０１が商品名称なので合わせる。
                //// 2013.10.21 AST Chg Start
                //#region 修正前
                ////dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[rowcount].Attribute02 = Convert.ToString(workNewPrice);
                ////dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[rowcount].Attribute03 = Convert.ToString(workOldPrice);
                //#endregion
                //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[rowcount].Attribute02 = Convert.ToString(workOldPrice);
                //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[rowcount].Attribute03 = Convert.ToString(workNewPrice);
                //// 2013.10.21 AST Chg End
                //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[rowcount].Attribute04 = Convert.ToString(workSlipOldPrice);
                //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[rowcount].SlipNewPrice = outPrice;
                //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[rowcount].SlipOldPrice = outPrice2;
                //dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[rowcount].SlipDisposalUnitPrica = outPrice3;
                //rowcount++;

            //    // 2013.11.02 AST Add Start
            //    // ＰＯＳ値引区分から売変区分を取得する
            //    detailRow.SalesChangeDiv = this.GetSalesChangeDiv(detailRow.CorporationCode, allStoreCd, detailRow.SlipDiv);
            //    // 2013.11.02 AST Add End
            //}
            // 2013/09/23 k.haraikawa end 税抜き計算処理
            #endregion
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Count > 0)
            {
                var headTable = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError;
                var detailTable = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError;
                foreach (var headRow in headTable)
                {
                    // ＰＯＳ値引区分から売変区分を取得する
                    string salesChangeDiv = this.GetSalesChangeDivNew(headRow.CorporationCode, allStoreCd, headRow.SlipDiv);

                    var detailRows = detailTable.Where(r => 
                        r.CorporationCode == headRow.CorporationCode
                        && r.InputMediaDiv == headRow.InputMediaDiv
                        && r.InputBatchNo == headRow.InputBatchNo
                        && r.SlipTypeCd == headRow.SlipTypeCd
                        && r.SlipDiv == headRow.SlipDiv
                        && r.SlipNo == headRow.SlipNo
                        && r.InputSequenceNo == headRow.InputSequenceNo
                        );

                    foreach (var detailRow in detailRows)
                    {
                        detailRow.SalesChangeDiv = salesChangeDiv;
                    }
                }
            }
            // <-- 2013/11/01 マスタ売価対応

            // エラーチェック
            // 【BSCSAR05008000161】を呼び出す。
            this.ChkErrorNew(workEntity, dsEntity);

            // トラン登録更新
            // 【BSCSAR05008000154】を呼び出す。
            this.UpInsSlipErrorNew(workEntity, dsEntity);

            // 2013.08.28 Start 承認モードの設定
            //string approvalFeatureDiv = initEntity.APVariableDispDataset.UTFAPVariable.FindByAPCorporationCodeAPVariableCode(
            //        workEntity.CorporationCode,
            //        APVariableListKey.SAR_ApprovalFeatureDiv).APVariableValue;
            string approvalFeatureDiv = workEntity.ApprovalMode;
            // 2013/10/15 アドオン対応　大福帳の作成条件変更 ISC:yshindo start
            // 実行モード："100"　→　伝票＋大福帳作成
            // 実行モード："1" かつ 承認モード："0"　→　伝票＋大福帳作成
            // 実行モード："1" かつ 承認モード："1"　→　伝票作成
            // 2013.08.28 Start 承認モードの設定
            //if ((workEntity.ExecuteMode != Constants.OnFlag) ||
            //    (workEntity.ExecuteMode == Constants.OnFlag && approvalFeatureDiv == Constants.OffFlag))
            //{
            // ＜伝票登録更新＞計上処理
            // ※DsEntity.DsEntity.伝票ヘッダ更新Ds.エラー区分＝０：正常のみ下記処理へ、０以外はスキップする
            // ※伝票への登録更新があれば、大福帳とカバーシートへも登録更新を行う
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].ErrorDiv == Constants.OffFlag)
            {
                // 伝票ヘッダ・明細・大福帳へ登録更新を行う
                // ※DsEntity.伝票ヘッダ更新Ds.承認ステータスの値と承認モードによって伝票と大福帳の更新モードを分岐させる
                // 2013.11.26 「AP変数．承認モード＝１(必要)」の設定だと、黒伝のみしか作成されない不具合を修正 Start
                //if (approvalFeatureDiv == Constants.OffFlag)
                //{
                //    // 赤黒両方作成
                //    workEntity.SlipUpdateMode = slipUpdateMode_RedBlack;
                //}
                //else if ((approvalFeatureDiv == Constants.OnFlag) && (
                //    (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].ApprovalStatus == approvalStatus_Apply)
                //    || (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].ApprovalStatus == approvalStatus_Waiting)))
                //{
                //    // 黒のみ作成
                //    workEntity.SlipUpdateMode = slipUpdateMode_Black;
                //}
                //else if ((approvalFeatureDiv == Constants.OnFlag) &&
                //    (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].ApprovalStatus == approvalStatus_Incorrect))
                //{
                //    // 赤のみ作成
                //    workEntity.SlipUpdateMode = slipUpdateMode_Red;
                //}

                // 常に「赤黒作成モード」に設定する。
                // この設定により以下の処理の流れとなる。
                //     ・常に赤黒作成モードでも、新規の場合は黒伝のみ作成され、修正の場合は赤黒が作成されるので問題は無い
                //     ・処理モードにより、カバーシートからの遷移の場合は、大福帳に更新され、
                //       承認モード＝０（不要）である場合も、大福帳に更新される為、問題は無い。
                //     ・伝票取消の場合でも、「赤伝作成モード」となる為、問題無い。
                //     ・ＰＤＡからの場合は、伝票～大福帳まで更新される。
                // 赤黒両方作成
                workEntity.SlipUpdateMode = slipUpdateMode_RedBlack;
                // 2013.11.26 「AP変数．承認モード＝１(必要)」の設定だと、黒伝のみしか作成されない不具合を修正 End


                // 2012.06.26 Start 伝票取消対応(削除フラグをセットする)
                //// 伝票更新【BSCSAR05008000068】
                //this.UpInsSlipExecute(workEntity, dsEntity, initEntity);

                // 伝票取消かを確認
                if (workEntity.CancelMode == Constants.OnFlag)
                {
                    // 赤のみ作成
                    workEntity.SlipUpdateMode = slipUpdateMode_Red;

                    // 伝票データ（赤伝）の作成
                    this.UpInsSlipExecuteForDenyNew(workEntity, dsEntity);

                    using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
                    {
                        // 元伝票と赤伝を訂正不可にし、エラーヘッダのFixDateを更新する
                        dataAccessComponent.UpdateSlipRevisionFlgSARSlipHeaderNew(workEntity, dsEntity);
                        dataAccessComponent.UpdateFixDateSARSlipHeaderErrorNew(workEntity, dsEntity);
                    }
                }
                else
                {
                    // 伝票更新【BSCSAR05008000157】
                    this.UpInsSlipExecuteNew(workEntity, dsEntity, initEntity);
                }
                // 2012.06.26 End   伝票取消対応(削除フラグをセットする)
            }

            //すでに承認済みの場合は伝票修正は直接大福帳を登録でき、再度の承認は不要
            if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].ApprovalStatus == "3") {

                workEntity.ExecuteMode = "100";

            }

            if ((workEntity.ExecuteMode != Constants.OnFlag && dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].ApprovalStatus == "3") ||
                (workEntity.ExecuteMode == Constants.OnFlag && approvalFeatureDiv == Constants.OffFlag))
            {
                // ※DsEntity.DsEntity.伝票ヘッダ更新Ds.エラー区分＝０：正常のみ下記処理へ、０以外はスキップする
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].ErrorDiv == Constants.OffFlag)
                {
                    // 2013.11.26 承認解除の場合は、「伝票は赤黒を作成」「大福帳は赤のみ作成」を行うように修正 START
                    if (workEntity.ExecuteMode == "200")
                    {
                        // 赤のみ作成
                        workEntity.SlipUpdateMode = slipUpdateMode_Red;
                    }
                    // 2013.11.26 承認解除の場合は、「伝票は赤黒を作成」「大福帳は赤のみ作成」を行うように修正 END

                    // 大福帳更新【BSCSAR05008000160】
                    // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
                    //this.UpInsAccountBook(workEntity, dsEntity, initEntity);
                    this.UpInsAccountBookNew(workEntity, dsEntity, initEntity, coverSheetMasterEntity);
                    // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
                }
            }
            // 2013/10/15 アドオン対応　大福帳の作成条件変更 ISC:yshindo end

            // 2013/09/17 アドオン対応：エラー件数が更新されないため、エラー以外もカバーシートを更新する ISC:yshindo start

            // カバーシートの更新前に承認ステータスを更新
            if (workEntity.CoverExecuteMode == "1")
            {
                using (CoverSheetMasterDataAccessComponent coverSheetMasterDataAccessComponent = new CoverSheetMasterDataAccessComponent())
                {
                    foreach (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow sarSlipHeaderErrorRow in dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows)
                    {
                        // 伝票ヘッダ承認ステータス更新
                        coverSheetMasterDataAccessComponent.UpdateApprovalSARSlipHeaderErrorNew(
                            dsEntity,
                            sarSlipHeaderErrorRow);

                        // 2013.11.29 関根 伝票ヘッダの承認ステータス更新追加 START
                        coverSheetMasterDataAccessComponent.UpdateApprovalSARSlipHeaderNew(
                            dsEntity,
                            sarSlipHeaderErrorRow);
                        // 2013.11.29 関根 伝票ヘッダの承認ステータス更新追加 END
                    }
                }
            }

            // 伝票の登録更新を行う際はカバーシートの更新も行う。
            // 【BSCSAR05008000152】を呼び出す
            this.FillCoverSheetMasterNew(workEntity, dsEntity);

            // カバーシート登録更新を実行する(BSCSAR05008000142)
            workEntity.CoverExecuteMode = "2";
            this.UpCoverSheetMasterNew(workEntity, dsEntity);
            // 2013/09/17 アドオン対応：エラー件数が更新されないため、エラー以外もカバーシートを更新する ISC:yshindo end
            // 2013/10/15 アドオン対応　大福帳の作成条件変更 ISC:yshindo start
            //}
            // 2013/10/15 アドオン対応　大福帳の作成条件変更 ISC:yshindo end

            // 2012.08.24 Start 伝票の行削除で論理削除フラグは立てないのでコメントアウト
            //// 8/21 SUEYOSHI 行削除で論理削除フラグが立っている場合、ＤＢ上にも削除フラグをたてる
            //foreach (SARSlipDetailErrorInfoDataSet.SARSlipDetailErrorRow detialRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows)
            //{
            //    if (detialRow.LogicalDelFlg == "1")
            //    {
            //        using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            //        {
            //            // 指定の伝票に削除フラグをセットし論理削除状態にする
            //            dataAccessComponent.UpdateDelFlgSARSlipDetailError(workEntity, dsEntity);
            //        }
            //    }
            //}
            //// 8/21 SUEYOSHI END
            // 2012.08.24 End   伝票の行削除で論理削除フラグは立てないのでコメントアウト
        }


        /// <summary>
        /// 伝票計上処理を行う。売変廃棄画面用
        /// (BSCSAR05008000153)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        /// <param name="coverSheetMasterEntity">カバーシートマスタワークEntity</param>
        public virtual void UpInsSlipNewForCoverSheetMaster(
           WorkEntity workEntity,
           DsEntity dsEntity,
           InitialDisplayEntity initEntity,
           CoverSheetMasterEntity coverSheetMasterEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            string approvalFeatureDiv = workEntity.ApprovalMode;

                // 常に「赤黒作成モード」に設定する。
                // この設定により以下の処理の流れとなる。
                //     ・常に赤黒作成モードでも、新規の場合は黒伝のみ作成され、修正の場合は赤黒が作成されるので問題は無い
                //     ・処理モードにより、カバーシートからの遷移の場合は、大福帳に更新され、
                //       承認モード＝０（不要）である場合も、大福帳に更新される為、問題は無い。
                //     ・伝票取消の場合でも、「赤伝作成モード」となる為、問題無い。
                //     ・ＰＤＡからの場合は、伝票～大福帳まで更新される。
                // 赤黒両方作成
            workEntity.SlipUpdateMode = slipUpdateMode_RedBlack;
                // 2013.11.26 「AP変数．承認モード＝１(必要)」の設定だと、黒伝のみしか作成されない不具合を修正 End

            if ((workEntity.ExecuteMode != Constants.OnFlag) ||
                (workEntity.ExecuteMode == Constants.OnFlag && approvalFeatureDiv == Constants.OffFlag))
            {
                // ※DsEntity.DsEntity.伝票ヘッダ更新Ds.エラー区分＝０：正常のみ下記処理へ、０以外はスキップする
                if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].ErrorDiv == Constants.OffFlag)
                {

                    // 2013.11.26 承認解除の場合は、「伝票は赤黒を作成」「大福帳は赤のみ作成」を行うように修正 START
                    if (workEntity.ExecuteMode == "200")
                    {
                        // 赤のみ作成
                        workEntity.SlipUpdateMode = slipUpdateMode_Red;
                    }
                    // 2013.11.26 承認解除の場合は、「伝票は赤黒を作成」「大福帳は赤のみ作成」を行うように修正 END

                    // 大福帳更新【BSCSAR05008000160】
                    // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
                    //this.UpInsAccountBook(workEntity, dsEntity, initEntity);
                    this.UpInsAccountBookNew(workEntity, dsEntity, initEntity, coverSheetMasterEntity);
                    // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
                }
            }

            // カバーシートの更新前に承認ステータスを更新
            using (CoverSheetMasterDataAccessComponent coverSheetMasterDataAccessComponent = new CoverSheetMasterDataAccessComponent())
            {
                foreach (SARSlipHeaderErrorInfoDataset.SARSlipHeaderErrorRow sarSlipHeaderErrorRow in dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows)
                {
                    // 伝票ヘッダ承認ステータス更新
                    coverSheetMasterDataAccessComponent.UpdateApprovalSARSlipHeaderErrorNew(
                        dsEntity,
                        sarSlipHeaderErrorRow);

                    // 2013.11.29 関根 伝票ヘッダの承認ステータス更新追加 START
                    coverSheetMasterDataAccessComponent.UpdateApprovalSARSlipHeaderNew(
                        dsEntity,
                        sarSlipHeaderErrorRow);
                    // 2013.11.29 関根 伝票ヘッダの承認ステータス更新追加 END
                }
            }

            // 伝票の登録更新を行う際はカバーシートの更新も行う。
            // 【BSCSAR05008000152】を呼び出す
            this.FillCoverSheetMasterNew(workEntity, dsEntity);

            // カバーシート登録更新を実行する(BSCSAR05008000142)
            workEntity.CoverExecuteMode = "2";
            this.UpCoverSheetMasterNew(workEntity, dsEntity);
        }

        /// <summary>
        /// エラートラン（ヘッダ・明細）登録更新処理を呼び出す。
        /// (BSCSAR05008000154)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void UpInsSlipErrorNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            // エラートラン（ヘッダ・明細）登録更新【BSCSAR05008000156】
            this.UpInsSlipErrorExecuteNew(workEntity, dsEntity);
        }

        /// <summary>
        /// エラートラン（ヘッダ・明細）登録更新処理を行う。
        /// (BSCSAR05008000156)
        /// </summary>
        /// <remarks>
        /// （１）伝票取消モードの場合、指定の伝票に削除フラグをセットし論理削除状態にする。
        /// （２）伝票取消モードでない場合、エラートラン（ヘッダ・明細）登録更新処理を行う。
        /// </remarks>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void UpInsSlipErrorExecuteNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                // 2012.06.26 Start 伝票取消対応(削除フラグをセットする)
                //dataAccessComponent.UpInsSARSlipHeaderError(workEntity, dsEntity);
                //dataAccessComponent.UpInsSARSlipDetailError(workEntity, dsEntity);

                // 伝票取消かを確認
                if (workEntity.CancelMode == Constants.OnFlag)
                {
                    // 取消時はエラートランを論理削除しない
                    // FixDate更新は赤伝INSERT後にUpInsSlipNewから実行する
                }
                else
                {
                    dataAccessComponent.UpInsSARSlipHeaderErrorNew(workEntity, dsEntity);
                    dataAccessComponent.UpInsSARSlipDetailErrorNew(workEntity, dsEntity);
                }
                // 2012.06.26 End   伝票取消対応(削除フラグをセットする)
            }
        }

        /// <summary>
        /// 伝票登録更新処理を行う。
        /// (BSCSAR05008000157)
        /// </summary>
        /// <remarks>
        /// （１）黒伝更新モードの場合、黒伝を作成する。
        /// （２）赤伝更新モードの場合、赤伝を作成する。
        /// （３）赤黒更新モードの場合、赤伝黒伝両方を作成する。既に伝票エラーテーブルに存在するデータに対しての訂正分の場合、伝票訂正フラグ="1"を更新する。
        /// 　　　伝票訂正フラグが"1"の場合、赤伝登録更新の処理を呼び出し、伝票訂正フラグが"0"の場合、黒伝登録更新の処理を呼び出す。
        /// </remarks>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        public virtual void UpInsSlipExecuteNew(
           WorkEntity workEntity,
           DsEntity dsEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            switch (workEntity.SlipUpdateMode)
            {
                case slipUpdateMode_Black:      // 黒伝を作成する。BSCSAR05008000159
                    this.UpInsSlipExecuteForNewestNew(workEntity, dsEntity, initEntity);
                    break;

                case slipUpdateMode_Red:        // 赤伝を作成する。BSCSAR05008000158
                    this.UpInsSlipExecuteForDenyNew(workEntity, dsEntity);
                    break;
                
                case slipUpdateMode_RedBlack:   // 赤黒両方を作成する。
                    //   2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 開始
                    //if (!workEntity.InitMode.Equals("0"))
                    //{
                    //    workEntity.SlipRevisionFlg = "1";
                    //    this.UpdateSlipExecuteForNewest(workEntity, dsEntity, initEntity);
                    //}

                    //   2012.01.23 関根 大福帳に赤伝が２重に登録される不具合対応（全明細削除時に発生） 開始
                    if (dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Rows.Count > 0)
                    //   2012.01.23 関根 大福帳に赤伝が２重に登録される不具合対応（全明細削除時に発生） 終了
                    {
                        // InitModeでチェックすると、既に伝票エラーテーブルに存在するデータの訂正分なのか、新規分なのか判断できない為、
                        // １件目のInputSequenceNoでチェックする。
                        if (Convert.ToInt32(dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError[0].InputSequenceNo) > 1)
                        {
                            // 訂正分の場合は、伝票訂正フラグ="1"を更新する。
                            workEntity.SlipRevisionFlg = "1";
                            this.UpdateSlipExecuteForNewestNew(workEntity, dsEntity, initEntity);
                        }
                    }
                    //   2012.01.14 関根 伝票エラーテーブルの入力ＳＥＱ番号のインクリメント不整合対応 終了
                    
                    workEntity.SlipRevisionFlg = "1";
                    this.UpInsSlipExecuteForDenyNew(workEntity, dsEntity);

                    workEntity.SlipRevisionFlg = "0";
                    this.UpInsSlipExecuteForNewestNew(workEntity, dsEntity, initEntity);
                    break;
            }
        }

        /// <summary>
        /// 伝票ヘッダ明細赤伝登録更新処理を行う。
        /// (BSCSAR05008000158)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void UpInsSlipExecuteForDenyNew(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            // 伝票データ（赤伝）の作成
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                workEntity.SlipSubNo = dataAccessComponent.InsertSARSlipHeaderForDenyNew(workEntity, dsEntity).ToString();
                dataAccessComponent.InsertSARSlipDetailForDenyNew(workEntity, dsEntity);
            }
        }

        /// <summary>
        /// 伝票ヘッダ明細黒伝登録更新処理を行う。
        /// (BSCSAR05008000159)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        public virtual void UpInsSlipExecuteForNewestNew(
           WorkEntity workEntity,
           DsEntity dsEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            // 伝票データ（黒伝）の作成
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                workEntity.SlipSubNo = dataAccessComponent.InsertSARSlipHeaderNew(workEntity, dsEntity, initEntity).ToString();
                dataAccessComponent.InsertSARSlipDetailNew(workEntity, dsEntity);
            }
        }

        /// UNImplement
        /// <summary>
        /// 大福帳の登録更新処理を行う。
        /// (BSCSAR05008000160)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        /// <param name="coverSheetMasterEntity">カバーシートマスタワークEntity</param>
        // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
        //public virtual void UpInsAccountBook(WorkEntity workEntity, DsEntity dsEntity, InitialDisplayEntity initEntity)
        public virtual void UpInsAccountBookNew(WorkEntity workEntity, DsEntity dsEntity, InitialDisplayEntity initEntity, CoverSheetMasterEntity coverSheetMasterEntity)
        // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            ICmcAccountBookComponent cmcAccountBookComponent = this.GetCmcAccountBookComponentNew();
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            // 2012.06.13 Start 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
            // ※元々コメントアウトしてあった部分を使用しAP変数より取得するように変更
            //var slipTypeCd_SalesChange = initEntity.APAvaliableDataSet.UTFAPVariable.FindByAPCorporationCodeAPVariableCode(
            //                                headerRow.CorporationCode,
            //                                APVariableListKey.SAR_SALESCHANGE_SLIPTYPECD).APVariableValue;
            var slipTypeCd_SalesChange = APVariableUtil.GetAPVariableValue(
                APVariableListKey.SAR_SALESCHANGE_SLIPTYPECD, headerRow.CorporationCode);
            //var slipTypeCd_Disposal = initEntity.APAvaliableDataSet.UTFAPVariable.FindByAPCorporationCodeAPVariableCode(
            //                                headerRow.CorporationCode,
            //                                APVariableListKey.SAR_DISPOSAL_SLIPTYPECD).APVariableValue;
            var slipTypeCd_Disposal = APVariableUtil.GetAPVariableValue(
                APVariableListKey.SAR_DISPOSAL_SLIPTYPECD, headerRow.CorporationCode);
            // 2012.06.13 End   売変伝票区分、廃棄伝票区分をAP変数より取得するように変更

            string dataID = string.Empty;
            // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） START
            string dataID_SalesChange = string.Empty;
            // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） END

            // 2012.06.13 Start 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
            //if (headerRow.SlipTypeCd == salesChange)
            if (headerRow.SlipTypeCd == slipTypeCd_SalesChange)
            // 2012.06.13 End   売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
            {
                // 2012.06.13 Start 廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。
                //dataID = salesChangeValue;

                //ＳＡＲ売変（実績）伝票作成フラグ
                //自動売変対象外商品の売変情報について、大福帳(売変（実績）伝票)への作成要否を制御　0：作成しない　1：作成する
                if (APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SALESCHANGERESULT_INS_FLG, UserProfile.CorporationCode).ToString() == "1")
                {
                    dataID = CodeDivisionUtil.GetCodeDivisionValueName(CodeDivisionKey.DataId, "03", UserProfile.CorporationCode);
                }
                else
                {
                    dataID = CodeDivisionUtil.GetCodeDivisionValueName(CodeDivisionKey.DataId, "02", UserProfile.CorporationCode);
                }
                // 2012.06.13 End   廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。
            }
            // 2012.06.13 Start 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
            //else if (headerRow.SlipTypeCd == disposal)
            else if (headerRow.SlipTypeCd == slipTypeCd_Disposal)
            // 2012.06.13 End   売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
            {
                // 2012.06.13 Start 廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。
                //dataID = disposalValue;
                dataID = CodeDivisionUtil.GetCodeDivisionValueName(CodeDivisionKey.DataId, "04", UserProfile.CorporationCode);
                // 2012.06.13 End   廃棄のDataId、売変のDataIdをソースコードへ直接指定しているのでAP変数より取得するように修正。

                // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） START
                dataID_SalesChange = CodeDivisionUtil.GetCodeDivisionValueName(CodeDivisionKey.DataId, "03", UserProfile.CorporationCode);
                // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） END
            }
            // 2013/09/17 アドオン対応：売変伝票、廃棄伝票以外は処理しないように修正 ISC:yshindo start
            else
            {
                return;
            }
            // 2013/09/17 アドオン対応：売変伝票、廃棄伝票以外は処理しないように修正 ISC:yshindo end

            AccountBookDataSet redSlipDataset = new AccountBookDataSet();
            // 2012.01.21 関根 伝票取消時の障害対応 開始
            AccountBookDataSet accountBookDataSet = new AccountBookDataSet();
            // 2012.01.21 関根 伝票取消時の障害対応 終了

            //   2012.01.16 関根 大福帳抽出で店舗コード指定ありメソッドを使用する 開始
            //cmcAccountBookComponent.ExtractAccountBookBase(
            cmcAccountBookComponent.ExtractAccountBookBaseStore(
                //   2012.01.16 関根 大福帳抽出で店舗コード指定ありメソッドを使用する 終了
                out redSlipDataset,
                dataID,
                headerRow.CorporationCode,
                headerRow.StoreCd,
                headerRow.SlipDate,
                // 暫定修正 2011.11.29 Start 大福帳の伝票が増幅していくのを修正
                //string.Empty,
                //string.Empty,
                //string.Empty,
                headerRow.SlipDiv,
                headerRow.SlipTypeCd,
                headerRow.SlipNo,
                // 暫定修正 2011.11.29 End   大福帳の伝票が増幅していくのを修正
                string.Empty,
                null,
                null,
                // 8/21 SUEYOSHI 固定値で渡していた為、修正
                // Constants.OnFlag,
                string.Empty,
                // 8/21 SUEYOSHI END
                string.Empty,
                string.Empty,
                Constants.OnFlag);

            switch (workEntity.SlipUpdateMode)
            {
                case slipUpdateMode_Black:
                    // ■WorkEntity.伝票更新モード＝０：黒伝
                    // 大福帳データセットを作成して、BSCCMC02001000001を呼び出す
                    // 2012.01.12 関根 大福帳チェックエラー対応 変更 開始
                    //// 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
                    ////cmcAccountBookComponent.RegisterAccountBookBase(this.GenerateAccountBookDataSet(workEntity, dsEntity, dataID));
                    //cmcAccountBookComponent.RegisterAccountBookBase(this.GenerateAccountBookDataSet(workEntity, dsEntity, dataID, coverSheetMasterEntity));
                    //// 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了

                    // 2012.01.21 関根 伝票取消時の障害対応 開始
                    //cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateAccountBookDataSet(workEntity, dsEntity, dataID, coverSheetMasterEntity));

                    accountBookDataSet = this.GenerateAccountBookDataSetNew(workEntity, dsEntity, dataID, coverSheetMasterEntity);
                    if (accountBookDataSet.AccountBook.Rows.Count > 0)
                    {
                        // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                        cmcAccountBookComponent.RegisterAccountBookForJournal(accountBookDataSet);
                    }
                    // 2012.01.21 関根 伝票取消時の障害対応 終了
                    // 2012.01.12 関根 大福帳チェックエラー対応 変更 終了
                    break;

                case slipUpdateMode_Red:
                    // ■WorkEntity.伝票更新モード＝１：赤伝
                    // 大福帳赤伝データセットを作成して、BSCCMC02001000001を呼び出す
                    // 2012.01.12 関根 大福帳チェックエラー対応 変更 開始
                    //// 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
                    ////cmcAccountBookComponent.RegisterAccountBookBase(this.GenerateRedAccountBookDataSet(redSlipDataset));
                    //cmcAccountBookComponent.RegisterAccountBookBase(this.GenerateRedAccountBookDataSet(redSlipDataset, coverSheetMasterEntity));
                    //// 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了

                    // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                    cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateRedAccountBookDataSetNew(redSlipDataset, coverSheetMasterEntity));
                    // 2012.01.12 関根 大福帳チェックエラー対応 変更 終了

                    // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） START
                    // 廃棄伝票の場合は、大福帳売変（SAR003）に対しても赤伝を作成する
                    if (headerRow.SlipTypeCd == slipTypeCd_Disposal)
                    {
                        // 廃棄標準売変評価差額値上伝票区分
                        string disposalBalanceUp = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_UP, UserProfile.CorporationCode);
                        // 廃棄標準売変評価差額値下伝票区分
                        string disposalBalanceDown = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_DOWN, UserProfile.CorporationCode);
                        // 廃棄評価差額伝票種別
                        string disposalBalanceSlipTypeCd = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_SLIP_TYPE_CD, UserProfile.CorporationCode);

                        // 廃棄標準売変評価差額値上用に赤伝を作成
                        redSlipDataset = new AccountBookDataSet();

                        cmcAccountBookComponent.ExtractAccountBookBaseStore(
                            out redSlipDataset,
                            dataID_SalesChange,
                            headerRow.CorporationCode,
                            headerRow.StoreCd,
                            headerRow.SlipDate,
                            disposalBalanceUp,
                            disposalBalanceSlipTypeCd,
                            headerRow.SlipNo,
                            string.Empty,
                            null,
                            null,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            Constants.OnFlag);

                        if (redSlipDataset.AccountBook.Rows.Count > 0)
                        {
                            // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                            cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateRedAccountBookDataSetNew(redSlipDataset, coverSheetMasterEntity));
                        }


                        // 廃棄標準売変評価差額値下用に赤伝を作成
                        redSlipDataset = new AccountBookDataSet();

                        cmcAccountBookComponent.ExtractAccountBookBaseStore(
                            out redSlipDataset,
                            dataID_SalesChange,
                            headerRow.CorporationCode,
                            headerRow.StoreCd,
                            headerRow.SlipDate,
                            disposalBalanceDown,
                            disposalBalanceSlipTypeCd,
                            headerRow.SlipNo,
                            string.Empty,
                            null,
                            null,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            Constants.OnFlag);

                        if (redSlipDataset.AccountBook.Rows.Count > 0)
                        {
                            // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                            cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateRedAccountBookDataSetNew(redSlipDataset, coverSheetMasterEntity));
                        }
                    }
                    // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） END
                    break;

                case slipUpdateMode_RedBlack:
                    // ■WorkEntity.伝票更新モード＝２：赤黒両方作成
                    // 大福帳データセットと大福帳赤伝データセットを作成して、BSCCMC02001000001を呼び出す
                    // 大福帳データセットを作成して、BSCCMC02001000001を呼び出す

                    // 2012.01.12 関根 大福帳チェックエラー対応 変更 開始
                    //// 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
                    ////cmcAccountBookComponent.RegisterAccountBookBase(this.GenerateRedAccountBookDataSet(redSlipDataset));
                    ////cmcAccountBookComponent.RegisterAccountBookBase(this.GenerateAccountBookDataSet(workEntity, dsEntity, dataID));
                    //cmcAccountBookComponent.RegisterAccountBookBase(this.GenerateRedAccountBookDataSet(redSlipDataset, coverSheetMasterEntity));
                    //cmcAccountBookComponent.RegisterAccountBookBase(this.GenerateAccountBookDataSet(workEntity, dsEntity, dataID, coverSheetMasterEntity));
                    //// 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了

                    //   2012.01.23 関根 大福帳に赤伝が２重に登録される不具合対応（全明細削除時に発生） 開始
                    //// 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                    //if (redSlipDataset.AccountBook.Rows.Count > 0)
                    //{
                    //    cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateRedAccountBookDataSet(redSlipDataset, coverSheetMasterEntity));
                    //}

                    // 赤伝用に取得したデータセットにデータが存在するかチェック
                    if (redSlipDataset.AccountBook.Rows.Count > 0)
                    {
                        // 赤伝用に取得したデータセットが既に赤伝（訂正区分="-"）であるかチェック　※赤伝の場合は更新しない
                        if (!redSlipDataset.AccountBook[0].CorrectionDiv.Equals("-"))
                        {
                            // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                            cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateRedAccountBookDataSetNew(redSlipDataset, coverSheetMasterEntity));

                            // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） START
                            // 廃棄伝票の場合は、大福帳売変（SAR003）に対しても赤伝を作成する
                            if (headerRow.SlipTypeCd == slipTypeCd_Disposal)
                            {
                                // 廃棄標準売変評価差額値上伝票区分
                                string disposalBalanceUp = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_UP, UserProfile.CorporationCode);
                                // 廃棄標準売変評価差額値下伝票区分
                                string disposalBalanceDown = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_DOWN, UserProfile.CorporationCode);
                                // 廃棄評価差額伝票種別
                                string disposalBalanceSlipTypeCd = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_SLIP_TYPE_CD, UserProfile.CorporationCode);

                                // 廃棄標準売変評価差額値上用に赤伝を作成
                                redSlipDataset = new AccountBookDataSet();

                                cmcAccountBookComponent.ExtractAccountBookBaseStore(
                                    out redSlipDataset,
                                    dataID_SalesChange,
                                    headerRow.CorporationCode,
                                    headerRow.StoreCd,
                                    headerRow.SlipDate,
                                    disposalBalanceUp,
                                    disposalBalanceSlipTypeCd,
                                    headerRow.SlipNo,
                                    string.Empty,
                                    null,
                                    null,
                                    string.Empty,
                                    string.Empty,
                                    string.Empty,
                                    Constants.OnFlag);

                                if (redSlipDataset.AccountBook.Rows.Count > 0)
                                {
                                    // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                                    cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateRedAccountBookDataSetNew(redSlipDataset, coverSheetMasterEntity));
                                }


                                // 廃棄標準売変評価差額値下用に赤伝を作成
                                redSlipDataset = new AccountBookDataSet();

                                cmcAccountBookComponent.ExtractAccountBookBaseStore(
                                    out redSlipDataset,
                                    dataID_SalesChange,
                                    headerRow.CorporationCode,
                                    headerRow.StoreCd,
                                    headerRow.SlipDate,
                                    disposalBalanceDown,
                                    disposalBalanceSlipTypeCd,
                                    headerRow.SlipNo,
                                    string.Empty,
                                    null,
                                    null,
                                    string.Empty,
                                    string.Empty,
                                    string.Empty,
                                    Constants.OnFlag);

                                if (redSlipDataset.AccountBook.Rows.Count > 0)
                                {
                                    // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                                    cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateRedAccountBookDataSetNew(redSlipDataset, coverSheetMasterEntity));
                                }
                            }
                            // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） END
                        }
                    }
                    //   2012.01.23 関根 大福帳に赤伝が２重に登録される不具合対応（全明細削除時に発生） 終了


                    // 2012.01.21 関根 伝票取消時の障害対応 開始
                    //cmcAccountBookComponent.RegisterAccountBookForJournal(this.GenerateAccountBookDataSet(workEntity, dsEntity, dataID, coverSheetMasterEntity));

                    // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） START
                    //accountBookDataSet = this.GenerateAccountBookDataSet(workEntity, dsEntity, dataID, coverSheetMasterEntity);
                    //if (accountBookDataSet.AccountBook.Rows.Count > 0)
                    //{
                    //    // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                    //    cmcAccountBookComponent.RegisterAccountBookForJournal(accountBookDataSet);
                    //}

                    accountBookDataSet = this.GenerateAccountBookDataSetNew(workEntity, dsEntity, dataID, coverSheetMasterEntity);
                    if (headerRow.SlipTypeCd == slipTypeCd_SalesChange)
                    {
                        // 売変の場合は、そのまま大福帳へ登録する。
                        if (accountBookDataSet.AccountBook.Rows.Count > 0)
                        {
                            // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                            cmcAccountBookComponent.RegisterAccountBookForJournal(accountBookDataSet);
                        }
                    }
                    else
                    {
                        // 廃棄の場合、大福帳DataSet(accountBookDataSet)には、
                        // 「廃棄」と「評価差額」の両方がセットされている為、
                        // それぞれ分けて大福帳へ登録する。
                        using (AccountBookDataSet copyAccountBookDataSet = new AccountBookDataSet())
                        using (AccountBookDataSet copyAccountBookDataSet_salesChange = new AccountBookDataSet())
                        {
                            // 廃棄の大福帳登録
                            this.CopyAccountBookDataSetNew(accountBookDataSet, copyAccountBookDataSet, dataID);
                            if (copyAccountBookDataSet.AccountBook.Rows.Count > 0)
                            {
                                // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                                cmcAccountBookComponent.RegisterAccountBookForJournal(copyAccountBookDataSet);
                            }

                            // 廃棄評価差額の大福帳登録
                            this.CopyAccountBookDataSetNew(accountBookDataSet, copyAccountBookDataSet_salesChange, dataID_SalesChange);
                            if (copyAccountBookDataSet_salesChange.AccountBook.Rows.Count > 0)
                            {
                                // 大福帳チェックに不具合があり、赤伝作成時に必ずエラーとなる為、大福帳チェックを行わないメソッドで登録する。
                                cmcAccountBookComponent.RegisterAccountBookForJournal(copyAccountBookDataSet_salesChange);
                            }
                        }
                    }
                    // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） END
                    // 2012.01.21 関根 伝票取消時の障害対応 終了
                    // 2012.01.12 関根 大福帳チェックエラー対応 変更 終了
                    break;
            }
        }

        /// <summary>
        /// 商品コードの存在チェックを行う。基準日時点で店舗の扱いがあること。商品が有効であることを確認する。
        /// (BSCSAR05008000173)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkItemCodeNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール１０２
            // 基準日時点で店舗の扱いがあること。商品が有効であることを確認する。確認できない場合はエラーとする。
            // 店別商品情報取得【BSCSTR10000000005】の実施
            STRStoreProductsDisclosureDataSet storeProductsDisclosureDataSet = new STRStoreProductsDisclosureDataSet();
            ISTRProductControlAPI strProductControlAPI = this.GetSTRProductControlAPIInstanceNew();

            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
            {
                if (detailRow.ErrorDiv != Constants.ErrorDiv_Fatal)
                {
                    // 店別商品情報取得(BSCSTR10000000005)
                    StoreProductsSearchConditionEntity storeProductsSearchConditionEntity = new StoreProductsSearchConditionEntity()
                    {
                        StoreCd = workEntity.StoreCd,
                        SearchBasicDate = workEntity.ChangeDate.Value,
                        CorporationCode = workEntity.CorporationCode,
                        SKUCd = detailRow["SKUCd"].ToString()
                    };

                    // ②　取得されたSTR店別商品公開用DataSet.商品名称（漢字）
                    strProductControlAPI.GetStoreProductsLineup(string.Empty, storeProductsSearchConditionEntity, ref storeProductsDisclosureDataSet);

                    if (storeProductsDisclosureDataSet.STRStoreProductsDisclosure.Count == 0)
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                        detailRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0203.ToString());
                    }
                    else
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Normal;
                        detailRow.ErrorMessage = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 数量＝０のチェックを行う。
        /// (BSCSAR05008000174)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkQtyZeroNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール１０３
            // 数量の０（ゼロ）入力のエラーチェック
            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
            {
                if (detailRow.ErrorDiv != Constants.ErrorDiv_Fatal)
                {
                    if (detailRow.DisposalChangeQty == 0)
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                        detailRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0204.ToString());
                    }
                    else
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Normal;
                        detailRow.ErrorMessage = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 新売単価の０またはマイナスでないかどうかのチェックを行う。
        /// 新売単価の値が０またはマイナス値の場合、エラーとする。
        /// (BSCSAR05008000175)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkPriceZeroNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール１０４
            // 販売伝票明細更新Dsの新売単価の値がマイナス値また０の場合、エラー。
            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
            {
                if (detailRow.ErrorDiv != Constants.ErrorDiv_Fatal)
                {
                    if (detailRow.SlipNewPrice <= 0)
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                        detailRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0205_104.ToString().Remove(12, 4));
                    }
                    else
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Normal;
                        detailRow.ErrorMessage = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 値上げチェック(新売単価＞旧売単価)を行う。「旧売単価＞新売単価」の場合、エラーとする。
        /// (BSCSAR05008000176)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkPriceUpNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール１０５
            // 伝票明細更新Ds.旧売単価と新売単価の関大小関係が、「旧売単価＞新売単価」の場合、エラー。
            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
            {
                if (detailRow.ErrorDiv != Constants.ErrorDiv_Fatal)
                {
                    if (detailRow.SlipOldPrice > detailRow.SlipNewPrice)
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                        detailRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0205_105.ToString().Remove(12, 4));
                    }
                    else
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Normal;
                        detailRow.ErrorMessage = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 値下げチェック(新売単価＜旧売単価)を行う。「旧売単価＜新売単価」の場合、エラーとする。
        /// (BSCSAR05008000177)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkPriceDownNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール１０６
            // 伝票明細更新Ds.旧売単価と新売単価の関大小関係が、「旧売単価＜新売単価」の場合、エラー。
            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
            {
                if (detailRow.ErrorDiv != Constants.ErrorDiv_Fatal)
                {
                    if (detailRow.SlipOldPrice < detailRow.SlipNewPrice)
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                        detailRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0205_106.ToString().Remove(12, 4));
                    }
                    else
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Normal;
                        detailRow.ErrorMessage = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 必須入力チェックを行う。
        /// (BSCSAR05008000178)
        /// </summary>
        /// <remarks>
        /// （１）売変または廃棄数が"0"の場合、エラー区分にエラー（"2"）を返し、エラーコード「ERRCSARB0206」のメッセージを表示する（数量が入力されていません）。
        /// （２）伝票取消モードでない、かつ伝票新売価金額が"0"の場合、エラー区分にエラー（"2"）を返しエラーコード「ERRCSARB0206」のメッセージを表示する（新売単価が入力されていません）。
        /// （３）事由が"NULL"又は空の場合、エラー区分にエラー（"2"）を返しエラーコード「ERRCSARB0206」のメッセージを表示する（事由が入力されていません）。
        /// （４）（１）～（３）に該当しない場合、エラー区分に正常（"0"）を返す。
        /// </remarks>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkMustNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール１０７
            foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
            {
                if (detailRow.ErrorDiv != Constants.ErrorDiv_Fatal && !string.IsNullOrEmpty(detailRow["SKUCd"].ToString()))
                {
                    if (detailRow.DisposalChangeQty == 0)
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                        detailRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0206.ToString(), "数量");
                    }
                    else if ((workEntity.CancelMode != Constants.OnFlag) && (detailRow.SlipNewPrice == 0))
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                        detailRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0206.ToString(), "新売単価");
                    }
                    else if (string.IsNullOrEmpty(detailRow.Reason))
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                        detailRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0206.ToString(), "事由");
                    }
                    else
                    {
                        detailRow.ErrorDiv = Constants.ErrorDiv_Normal;
                        detailRow.ErrorMessage = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 明細行にエラーが発生している行が存在するかどうかのチェックを行う。
        /// (BSCSAR05008000163)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkDetailErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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

            // ビジネスルール１
            // 明細行にエラーが発生している行があるか、ないかのチェックを行う。
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];
            if (this.HasDetailErrorNew(dsEntity.SARSlipDetailErrorDataSet))
            {
                headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                headerRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0194.ToString());
            }
            else
            {
                headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                headerRow.ErrorMessage = string.Empty;
            }
        }

        /// <summary>
        /// 店舗コードの必須チェックを行う。
        /// (BSCSAR050080001164)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkMustStoreCodeNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール２
            // DsEntity.伝票ヘッダ更新Ds.店舗コード＝未設定(null）の場合、エラー。
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            if (headerRow.ErrorDiv != Constants.ErrorDiv_Fatal)
            {
                if (string.IsNullOrEmpty(headerRow.StoreCd))
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                    headerRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0195.ToString());
                }
                else
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                    headerRow.ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// 分類１コードの必須チェックを行う。
        /// (BSCSAR050080001165)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkMustCategory1CdNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール３
            // DsEntity.伝票ヘッダ更新Ds.分類1コード=未設定(null）の場合、エラー。
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            if (headerRow.ErrorDiv != Constants.ErrorDiv_Fatal)
            {
                if (string.IsNullOrEmpty(headerRow.Category1Cd))
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                    headerRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0196.ToString());
                }
                else
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                    headerRow.ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// 販売管理営業日時点における、店舗コードの存在チェックを行う。
        /// (BSCSAR05008000166)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkStoreCodeNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール４
            // 販売管理営業日時点での存在チェック (【ＣＭＣ店舗存在チェック】の呼出)
            ICmcStoreAPI storeAPI = this.GetCmcStoreAPIInstanceNew();
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            if (headerRow.ErrorDiv != Constants.ErrorDiv_Fatal)
            {
                if (!storeAPI.CheckStore(workEntity.Subsystem, workEntity.CorporationCode, workEntity.SalesMngDate.Value, workEntity.StoreCd))
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                    headerRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0197.ToString());
                }
                else
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                    headerRow.ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// 販売管理営業日時点における、分類１コードの存在チェックを行う。
        /// (BSCSAR05008000167)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkCategory1CdNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール５
            // 【ＣＭＲ店舗分類１存在チェック】の呼出
            ICmrStoreAPI storeAPI = this.GetCmrStoreAPIInstanceNew();
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            if (headerRow.ErrorDiv != Constants.ErrorDiv_Fatal)
            {
                if (!storeAPI.CheckStoreCategory1(
                        workEntity.Subsystem,
                        workEntity.CorporationCode,
                        workEntity.SalesMngDate.Value,
                        workEntity.StoreCd,
                        workEntity.Category1Cd,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty))
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                    headerRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0198.ToString());
                }
                else
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                    headerRow.ErrorMessage = string.Empty;
                }
            }
        }

        /// UNIMPLEMENT
        /// <summary>
        /// 計上日チェックを行う。在庫計上日を超過しての入力時は確認メッセージ【MSGCSARI0016】（計上日を過ぎた日付です）を表示する。
        /// (BSCSAR05008000168)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkSlipDateNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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

            // ビジネスルール６
            // 在庫計上日を超過しての入力時は確認メッセージ【MSGCSARI0016】を表示する。
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            if (headerRow.ErrorDiv != Constants.ErrorDiv_Fatal)
            {
                if (headerRow.DisposalChangeDate < workEntity.IVPeriodDate)
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                    headerRow.ErrorMessage = ResourceUtil.GetMessage(ErrIDEnum.MSGCSARI0016.ToString());
                }
                else
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                    headerRow.ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// 売変日＜＝販売管理営業日であることをチェックする。売変日＞販売管理営業日の場合、エラーとする。
        /// (BSCSAR05008000169)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkSlipDateContextNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール７
            // DsEntity.伝票ヘッダ更新Ds.売変日＞WorkEntity.販売管理営業日の場合、エラー。
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            if (headerRow.ErrorDiv != Constants.ErrorDiv_Fatal)
            {
                if (headerRow.DisposalChangeDate > workEntity.SalesMngDate)
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                    headerRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0199.ToString());
                }
                else
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                    headerRow.ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// 合計売価金額のチェックを行う。
        /// (BSCSAR05008000170)
        /// </summary>
        /// ①ヘッダ差額＝伝票旧売価金額合計－伝票新売価金額合計
        /// ②明細差額合計＝ＳＵＭ（伝票旧売価）－ＳＵＭ（伝票新売価金額）
        /// →①ヘッダ差額≠②明細差額合計の場合、エラーとする。
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkSellPriceNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール８
            // ①ヘッダ差額＝伝票旧売価金額合計－伝票新売価金額合計
            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];

            decimal headerDiff = headerRow.SlipOldTotalPrice - headerRow.SlipNewTotalPrice;

            // ②明細差額合計＝ＳＵＭ（伝票旧売価）－ＳＵＭ（伝票新売価金額）
            decimal totalDetailDiff = dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Sum(d => d.SlipOldPrice)
                                        - dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError.Sum(d => d.SlipNewPrice);

            // ①ヘッダ差額≠②明細差額合計の場合、エラー。
            if (headerRow.ErrorDiv != Constants.ErrorDiv_Fatal)
            {
                if (headerDiff != totalDetailDiff)
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                    headerRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0200.ToString());
                }
                else
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                    headerRow.ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// チェックディジット計算処理の呼出を行う。伝票番号とデジットコードが一致しない場合、エラーとする。
        /// (BSCSAR05008000171)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkCheckDigitNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            // ビジネスルール９
            // チェックディジット計算の呼出
            ICmcOtherAPI otherAPI = this.GetCmcOtherAPIInstanceNew();
            string digitCd = string.Empty;

            var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];
            if (headerRow.ErrorDiv != Constants.ErrorDiv_Fatal)
            {
                otherAPI.CalcCheckDigit(
                    workEntity.Subsystem,
                    headerRow.SlipNo.Remove(headerRow.SlipNo.Length - 1, 1),
                    int.Parse(workEntity.SalesSlipDigit),
                    workEntity.ChkDigitType,
                    int.Parse(workEntity.Weight),
                    out digitCd);

                if (digitCd != headerRow.SlipNo)
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Fatal;
                    headerRow.ErrorMessage = ResourceUtil.GetErrorMessage(ErrIDEnum.ERRCSARB0201.ToString());
                }
                else
                {
                    headerRow.ErrorDiv = Constants.ErrorDiv_Normal;
                    headerRow.ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// 伝票番号指定時における伝票番号範囲チェックを行う。
        /// (BSCSAR05008000172)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkSlipNoSopeNew(WorkEntity workEntity, DsEntity dsEntity)
        {
            ////QA１２８答え：
            ////このチェックは実装しません。
            ////エラーチェック条件Dsにも取得されません。
        }

        /// <summary>
        /// 伝票エラーチェックを行う。
        /// (BSCSAR05008000161)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void ChkErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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

            string treatmentDiv_Detail = "1";

            // ＜エラーチェック条件Dsの取得＞(【BSCSAR05008000162】)
            Buying.Common.Business.BYRIncludeCheckConditionDataSet includeCheckConditionDataSet = this.FillChkErrorNew(workEntity, dsEntity);

            var checkConditions = includeCheckConditionDataSet.BYRIncludeCheckCondition.
                                    OrderBy(r => r.TreatmentDiv != treatmentDiv_Detail).
                                       ThenBy(r => r.BusinessRuleNo);

            foreach (var checkConditionRow in checkConditions)
            {
                // ■エラーチェック
                this.SwitchErrorCheckNew(int.Parse(checkConditionRow.BusinessRuleNo), workEntity, dsEntity);
            }
        }

        /// <summary>
        /// 大福帳登録更新処理を行う。
        /// (BSCSAR05008000162)
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <returns>エラーチェック条件Ds</returns>
        public virtual Buying.Common.Business.BYRIncludeCheckConditionDataSet FillChkErrorNew(WorkEntity workEntity, DsEntity dsEntity)
        {
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
            Buying.Common.Business.IncludeCheckConditionSearchEntity includeCheckCondition = new Buying.Common.Business.IncludeCheckConditionSearchEntity();
            includeCheckCondition.CorporationCode = workEntity.CorporationCode;
            includeCheckCondition.SlipTypeCd = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipTypeCd;
            includeCheckCondition.SlipDiv = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0].SlipDiv;

            using (Buying.Common.Business.BYRIncludeCheckConditionDataSet includeCheckConditionSearchDataSet = new Buying.Common.Business.BYRIncludeCheckConditionDataSet())
            {
                IPurchaseAccountingBusiness purchaseAccountingBusiness = this.GetPurchaseAccountingBusinessInstanceNew();
                purchaseAccountingBusiness.CheckPurchaseAccountingAcquisitionConditions(includeCheckCondition, includeCheckConditionSearchDataSet);

                return includeCheckConditionSearchDataSet;
            }
        }


#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 -->
        /// <summary>
        /// 伝票ヘッダ明細の伝票データ（黒伝）更新処理を行う。
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name="initEntity">初期表示Entity</param>
        public virtual void UpdateSlipExecuteForNewestNew(
           WorkEntity workEntity,
           DsEntity dsEntity,
           InitialDisplayEntity initEntity)
        {
            #region 必須入力チェック

            if (workEntity == null)
            {
                throw new ArgumentNullException("workEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }

            if (initEntity == null)
            {
                throw new ArgumentNullException("initEntity");
            }

            #endregion

            // 伝票データ（黒伝）の更新
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                dataAccessComponent.UpdateSARSlipHeaderNew(workEntity, dsEntity, initEntity);
            }
        }
#warning SKN臨時対応　「伝票訂正フラグ」の更新処理を追加 <--

        // 2012.10.17 Start 商品コードが入力されていない場合は部門代表商品とするようにする
        #region GetDummyVendorCodeNew
        /// <summary>
        /// ダミーコード取得ビジネス(BSCSAR05008000081)
        /// </summary>
        /// <param name="categoryCdProductsSearchConditionEntity">(IN) 分類コード商品検索条件Entity</param>
        /// <param name="strProductDisclosureDataSet">(OUT) STR商品公開用DataSet</param>
        public virtual void GetDummyVendorCodeNew(
           CategoryCdProductsSearchConditionEntity categoryCdProductsSearchConditionEntity,
           STRProductDisclosureDataSet strProductDisclosureDataSet)
        {
            #region 必須入力チェック

            if (categoryCdProductsSearchConditionEntity == null)
            {
                throw new ArgumentNullException("categoryCdProductsSearchConditionEntity");
            }

            if (strProductDisclosureDataSet == null)
            {
                throw new ArgumentNullException("strProductDisclosureDataSet");
            }

            #endregion

            // １．下記の引数で、商品種別情報取得ビジネスを呼び出す。
            ISTRProductControlAPI productControlAPI = this.GetSTRProductControlAPIInstanceNew();
            productControlAPI.GetDummyProductBasicAttributeByProductClassification(
                categoryCdProductsSearchConditionEntity,
                productCategory_DummyItem,
                strProductDisclosureDataSet);
        }
        #endregion
        // 2012.10.17 End   商品コードが入力されていない場合は部門代表商品とするようにする

        #region [BSCSAR05005000015] SearchSalesChangeSlipProductPDANew (売変入力ＰＤＡ商品検索BS)
        /// <summary>
        /// 売変入力ＰＤＡ商品検索BS
        /// ID：BSCSAR05005000015
        /// </summary>
        /// <param name="salesChangeSlipInputProductsReadPDAEntity">*売変入力ＰＤＡ商品検索Entity</param>
        /// <returns>結果ステータス(0：正常 1：異常)</returns>
        public virtual int SearchSalesChangeSlipProductPDANew(
            ref SalesChangeSlipInputProductsReadPDAEntity salesChangeSlipInputProductsReadPDAEntity)
        {
            #region 必須入力チェック
            if (salesChangeSlipInputProductsReadPDAEntity == null)
            {
                throw new ArgumentNullException("salesChangeSlipInputProductsReadPDAEntity");
            }
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.CorporationCode))
            {
                throw new ArgumentNullException("Entity.CorporationCode");
            }
            if (salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate == null)
            {
                throw new ArgumentNullException("Entity.SalesChangeDate");
            }
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.StoreCd))
            {
                throw new ArgumentNullException("Entity.StoreCd");
            }
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.ProductsCd))
            {
                throw new ArgumentNullException("Entity.ProductsCd");
            }
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.RetailReductionCheckDiv))
            {
                throw new ArgumentNullException("Entity.RetailReductionCheckDiv");
            }
            #endregion

            int resultValue = NormalStatus;

            // [BSCSAR05005000005]売変入力ＰＤＡ商品情報取得ビジネスを呼び出す。
            resultValue = this.GetSalesChangeSlipProductInformationPDANew(
                            ref salesChangeSlipInputProductsReadPDAEntity);

            // 正常以外は処理終了
            if (resultValue != NormalStatus)
            {
                return resultValue;
            }

            // [BSCSAR05005000006]在庫情報取得ビジネスを呼び出す。
            // --> 2012/07/17 在庫数取得不具合修正
            decimal logicalStokQty;
            // <-- 2012/07/17 在庫数取得不具合修正
            this.GetInventoryInformationPDANew(
                salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate,
                salesChangeSlipInputProductsReadPDAEntity.StoreCd,
                salesChangeSlipInputProductsReadPDAEntity.SKUCd,
                salesChangeSlipInputProductsReadPDAEntity.Category1Cd,
                salesChangeSlipInputProductsReadPDAEntity.Category2Cd,
                // --> 2012/07/17 在庫数取得不具合修正
                //salesChangeSlipInputProductsReadPDAEntity.LogicalStokQty);
                out logicalStokQty);
            salesChangeSlipInputProductsReadPDAEntity.LogicalStokQty = logicalStokQty;
            // <-- 2012/07/17 在庫数取得不具合修正

            return resultValue;
        }
        #endregion

        #region [BSCSAR05005000016] GetSalesChangeSlipProductInformationPDANew (売変入力ＰＤＡ商品情報取得BS)
        /// <summary>
        /// 売変入力ＰＤＡ商品情報取得BS
        /// ID：BSCSAR05005000016
        /// </summary>
        /// <param name="salesChangeSlipInputProductsReadPDAEntity">*売変入力ＰＤＡ商品検索Entity</param>
        /// <returns>結果ステータス(0：正常 1：異常)</returns>
        public virtual int GetSalesChangeSlipProductInformationPDANew(
            ref SalesChangeSlipInputProductsReadPDAEntity salesChangeSlipInputProductsReadPDAEntity)
        {
            #region 必須入力チェック
            if (salesChangeSlipInputProductsReadPDAEntity == null)
            {
                throw new ArgumentNullException("salesChangeSlipInputProductsReadPDAEntity");
            }
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.CorporationCode))
            {
                throw new ArgumentNullException("Entity.CorporationCode");
            }
            if (salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate == null)
            {
                throw new ArgumentNullException("Entity.SalesChangeDate");
            }
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.StoreCd))
            {
                throw new ArgumentNullException("Entity.StoreCd");
            }
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.ProductsCd))
            {
                throw new ArgumentNullException("Entity.ProductsCd");
            }
            if (string.IsNullOrEmpty(salesChangeSlipInputProductsReadPDAEntity.RetailReductionCheckDiv))
            {
                throw new ArgumentNullException("Entity.RetailReductionCheckDiv");
            }
            #endregion

            // STRProductControlAPIをインスタンス化
            ISTRProductControlAPI strProductControlAPI =
                (ISTRProductControlAPI)ClassObjectUtil.GetBusinessInstance(classIdSTRProductControlAPI, typeof(ISTRProductControlAPI));

            // BargainPlanningProductStorePublicInformationBusinessをインスタンス化
            IBargainPlanningProductStorePublicInformationBusiness strBargainPlanningProductStorePublicInformationBusiness =
                (IBargainPlanningProductStorePublicInformationBusiness)ClassObjectUtil.GetBusinessInstance(classIdBargainPlanningProductStorePublicInformationBusiness, typeof(IBargainPlanningProductStorePublicInformationBusiness));

            // CmrCategoryAPIをインスタンス化
            ICmrCategoryAPI cmrCategoryAPI =
                (ICmrCategoryAPI)ClassObjectUtil.GetBusinessInstance(classIdCmrCategoryAPI, typeof(ICmrCategoryAPI));

            #region (公開クラス)[BSCSTR10000000038]スキャニング商品情報取得ビジネスを呼び出す。
            // ＳＴＲ商品公開用DataSet
            STRProductDisclosureDataSet strProductDisclosureDataSet = null;
            using (strProductDisclosureDataSet = new STRProductDisclosureDataSet())
            {
                // (公開クラス)[BSCSTR10000000038]スキャニング商品情報取得ビジネスを呼び出す。
                strProductControlAPI.GetProductInfoByScannedData(
                    salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                    salesChangeSlipInputProductsReadPDAEntity.ProductsCd,
                    salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate,
                    ref strProductDisclosureDataSet);

                // 商品未存在の場合はエラーとして終了
                if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Rows.Count == 0)
                {
                    // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"商品情報"}）
                    // 「！{0}が取得できません。！」
                    salesChangeSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "商品情報" });

                    return ErrorStatus;
                }

                // 取得した商品情報をセットする
                salesChangeSlipInputProductsReadPDAEntity.ProductsCd = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].OwnProductsCd;
                salesChangeSlipInputProductsReadPDAEntity.ScanningCd = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].ScanningCd;
                salesChangeSlipInputProductsReadPDAEntity.SKUCd = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].SKUCd;
            }
            #endregion

            #region (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
            // ＳＴＲ店別商品公開用DataSet
            STRStoreProductsDisclosureDataSet strStoreProductsDisclosureDataSet = null;
            using (strStoreProductsDisclosureDataSet = new STRStoreProductsDisclosureDataSet())
            {
                // 商品店検索条件Entity
                StoreProductsSearchConditionEntity storeProductsSearchConditionEntity = new StoreProductsSearchConditionEntity()
                {
                    CorporationCode = salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                    StoreCd = salesChangeSlipInputProductsReadPDAEntity.StoreCd,
                    SearchBasicDate = salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate,
                    OwnProductsCd = salesChangeSlipInputProductsReadPDAEntity.ProductsCd,
                    ScanningCd = salesChangeSlipInputProductsReadPDAEntity.ScanningCd,
                    SKUCd = salesChangeSlipInputProductsReadPDAEntity.SKUCd
                };

                // (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                // ※区分="2"(販売終了（停止）は存在なし)
                strProductControlAPI.GetStoreProductsLineup(
                    "2",
                    storeProductsSearchConditionEntity,
                    ref strStoreProductsDisclosureDataSet);

                if (strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure.Count > 0)
                {
                    // 取得した商品情報をセット(先頭の１件のみをセット)
                    salesChangeSlipInputProductsReadPDAEntity.Category1Cd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].Category1Cd;
                    salesChangeSlipInputProductsReadPDAEntity.Category2Cd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].Category2Cd;
                    salesChangeSlipInputProductsReadPDAEntity.Category3Cd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].Category3Cd;
                    salesChangeSlipInputProductsReadPDAEntity.Category4Cd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].Category4Cd;
                    salesChangeSlipInputProductsReadPDAEntity.SKUCd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].SKUCd;
                    salesChangeSlipInputProductsReadPDAEntity.ProductsKanji = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].ProductsKanji;
                    salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].SaleUnitPrice;
                }
                else
                {
                    // 商品情報が取得できなかった場合
                    #region (公開クラス)[BSCSTR10000000001]商品コード存在チェック＆名称取得ビジネスを呼び出す。
                    // 商品コード存在チェック＆ゲットDataSet
                    STRProductCodeExistCheckAndGetDataSet strProductCodeExistCheckAndGetDataSet = null;
                    using (strProductCodeExistCheckAndGetDataSet = new STRProductCodeExistCheckAndGetDataSet())
                    {
                        // 商品検索条件Entity
                        ProductsSearchConditionEntity productsSearchConditionEntity = new ProductsSearchConditionEntity()
                        {
                            CorporationCode = salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                            SearchBasicDate = salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate,
                            OwnProductsCd = salesChangeSlipInputProductsReadPDAEntity.ProductsCd,
                            ScanningCd = salesChangeSlipInputProductsReadPDAEntity.ScanningCd,
                            SKUCd = salesChangeSlipInputProductsReadPDAEntity.SKUCd
                        };

                        // (公開クラス)[BSCSTR10000000001]商品コード存在チェック＆名称取得ビジネスを呼び出す。
                        strProductControlAPI.ProductsCodeExistCheckAndGet(
                            productsSearchConditionEntity,
                            ref strProductCodeExistCheckAndGetDataSet);

                        if (strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet.Rows.Count == 0)
                        {
                            // 商品情報が取得できなかった場合
                            #region (公開クラス)[BSCSTR10000000014]商品基本属性（標準パターン付き取得）ビジネスを呼び出す。
                            // ＳＴＲ商品（標準パターン付き）公開用DataSet
                            STRProductWithPatternDisclosureDataSet strProductWithPatternDisclosureDataSet = null;
                            using (strProductWithPatternDisclosureDataSet = new STRProductWithPatternDisclosureDataSet())
                            {
                                // 商品検索条件Entity
                                ProductsSearchConditionEntity productsSearchConditionWithPatternEntity = new ProductsSearchConditionEntity()
                                {
                                    CorporationCode = salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                                    SearchBasicDate = salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate,
                                    OwnProductsCd = salesChangeSlipInputProductsReadPDAEntity.ProductsCd,
                                    ScanningCd = salesChangeSlipInputProductsReadPDAEntity.ScanningCd,
                                    SKUCd = salesChangeSlipInputProductsReadPDAEntity.SKUCd
                                };

                                // (公開クラス)[BSCSTR10000000014]商品基本属性（標準パターン付き取得）ビジネスを呼び出す。
                                // ※区分="9"(取扱い終了も取得する)
                                strProductControlAPI.GetProductBasicAttributeWithPattern(
                                    "9",
                                    productsSearchConditionWithPatternEntity,
                                    ref strProductWithPatternDisclosureDataSet);

                                if (strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure.Rows.Count == 0)
                                {
                                    // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"商品情報"}）
                                    // 「！{0}が取得できません。！」
                                    salesChangeSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "商品情報" });

                                    return ErrorStatus;
                                }
                                else
                                {
                                    // 取得した商品情報をセット(先頭の１件のみをセット)
                                    salesChangeSlipInputProductsReadPDAEntity.Category1Cd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Category1Cd;
                                    salesChangeSlipInputProductsReadPDAEntity.Category2Cd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Category2Cd;
                                    salesChangeSlipInputProductsReadPDAEntity.Category3Cd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Category3Cd;
                                    salesChangeSlipInputProductsReadPDAEntity.Category4Cd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Category4Cd;
                                    salesChangeSlipInputProductsReadPDAEntity.SKUCd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].SKUCd;
                                    salesChangeSlipInputProductsReadPDAEntity.ProductsKanji = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].ProductsKanji;
                                    salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].SaleUnitPrice;
                                    if (string.IsNullOrEmpty(strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Status) == false)
                                    {
                                        // （基盤）エラーメッセージ取得（"ERRCSARB244", new string[]{STR商品（標準パターン付き）公開用DataSet．ステータス}）
                                        // 「！商品の状態が「{0}」です。継続しますか？！」
                                        salesChangeSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB244, new string[] { strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Status });
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            // 商品情報が取得できた場合
                            #region (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                            // ＳＴＲ店別商品公開用DataSet
                            STRStoreProductsDisclosureDataSet strStoreProductsDisclosureReDataSet = null;
                            using (strStoreProductsDisclosureReDataSet = new STRStoreProductsDisclosureDataSet())
                            {
                                // 商品店検索条件Entity
                                StoreProductsSearchConditionEntity storeProductsSearchConditionReEntity = new StoreProductsSearchConditionEntity()
                                {
                                    CorporationCode = salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                                    StoreCd = salesChangeSlipInputProductsReadPDAEntity.StoreCd,
                                    SearchBasicDate = salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate,
                                    OwnProductsCd = salesChangeSlipInputProductsReadPDAEntity.ProductsCd,
                                    ScanningCd = salesChangeSlipInputProductsReadPDAEntity.ScanningCd,
                                    SKUCd = salesChangeSlipInputProductsReadPDAEntity.SKUCd
                                };

                                // (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                                // ※区分="0"取扱い終了は存在なし)
                                strProductControlAPI.GetStoreProductsLineup(
                                    "0",
                                    storeProductsSearchConditionReEntity,
                                    ref strStoreProductsDisclosureReDataSet);

                                if (strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure.Count > 0)
                                {
                                    // 取得した商品情報をセット(先頭の１件のみをセット)
                                    salesChangeSlipInputProductsReadPDAEntity.Category1Cd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].Category1Cd;
                                    salesChangeSlipInputProductsReadPDAEntity.Category2Cd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].Category2Cd;
                                    salesChangeSlipInputProductsReadPDAEntity.Category3Cd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].Category3Cd;
                                    salesChangeSlipInputProductsReadPDAEntity.Category4Cd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].Category4Cd;
                                    salesChangeSlipInputProductsReadPDAEntity.SKUCd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].SKUCd;
                                    salesChangeSlipInputProductsReadPDAEntity.ProductsKanji = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].ProductsKanji;
                                    salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].SaleUnitPrice;
                                    // --> 2012/10/18 商品取得時の警告判定修正
                                    if (string.IsNullOrEmpty(strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet[0].StatusKanji) == false)
                                    {
                                        // （基盤）エラーメッセージ取得（"ERRCSARB244", new string[]{STR商品（標準パターン付き）公開用DataSet．ステータス}）
                                        // 「！商品の状態が「{0}」です。継続しますか？！」
                                        salesChangeSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB244, new string[] { strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet[0].StatusKanji });
                                    }
                                    // <-- 2012/10/18 商品取得時の警告判定修正
                                }
                                else
                                {
                                    // 商品情報が取得できなかった場合
                                    // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"商品情報"}）
                                    // 「！{0}が取得できません。！」
                                    salesChangeSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "商品情報" });

                                    return ErrorStatus;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region (公開クラス)[BSCCMR01003000002]ＣＭＲ分類２公開情報取得ビジネスを呼び出す。
            string mstStoreInvAppraisalDiv = string.Empty;
            salesChangeSlipInputProductsReadPDAEntity.AutoSalesChangeFlg = "0";

            // 分類２公開DataSet
            using (Category2DisclosureDataSet category2DisclosureDataSet = new Category2DisclosureDataSet())
            {
                // 分類２公開検索条件Entity
                Category2DisclosureSearchConditionEntity category2DisclosureSearchConditionEntity = new Category2DisclosureSearchConditionEntity()
                {
                    CorporationCode = salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                    BaseDate = salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate,
                    Category1CD = salesChangeSlipInputProductsReadPDAEntity.Category1Cd,
                    Category2CD = salesChangeSlipInputProductsReadPDAEntity.Category2Cd,
                    RegistrationDiv = string.Empty
                };

                // (公開クラス)[BSCCMR01003000002]ＣＭＲ分類２公開情報取得ビジネスを呼び出す。
                cmrCategoryAPI.GetCategory2Disclosure(
                    "SA",
                    category2DisclosureSearchConditionEntity,
                    category2DisclosureDataSet);

                // 分類２未存在の場合はエラーとして終了
                if (category2DisclosureDataSet.Category2Disclosure.Rows.Count == 0)
                {
                    // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"分類２情報"}）
                    // 「！{0}が取得できません。！」
                    salesChangeSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "分類２情報" });

                    return ErrorStatus;
                }

                // 取得した分類２情報をセットする
                mstStoreInvAppraisalDiv = category2DisclosureDataSet.Category2Disclosure[0].StoreInvAppraisalDiv;
                salesChangeSlipInputProductsReadPDAEntity.AutoSalesChangeFlg = category2DisclosureDataSet.Category2Disclosure[0].AutoSalesChangeFlg;
            }
            #endregion

            // 店舗棚卸評価区分が空の場合は分類１情報を取得する
            if (string.IsNullOrEmpty(mstStoreInvAppraisalDiv) == true)
            {
                #region (公開クラス)[BSCCMR01003000001]ＣＭＲ分類１公開情報取得ビジネスを呼び出す。
                // 分類１公開DataSet
                using (Category1DisclosureDataSet category1DisclosureDataSet = new Category1DisclosureDataSet())
                {
                    // 分類１公開検索条件Entity
                    Category1DisclosureSearchConditionEntity category1DisclosureSearchConditionEntity = new Category1DisclosureSearchConditionEntity()
                    {
                        CorporationCode = salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                        BaseDate = salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate,
                        Category1CD = salesChangeSlipInputProductsReadPDAEntity.Category1Cd,
                        RegistrationDiv = string.Empty
                    };

                    // (公開クラス)[BSCCMR01003000001]ＣＭＲ分類１公開情報取得ビジネスを呼び出す。
                    cmrCategoryAPI.GetCategory1Disclosure(
                        "SA",
                        category1DisclosureSearchConditionEntity,
                        category1DisclosureDataSet);

                    // 分類１未存在の場合はエラーとして終了
                    if (category1DisclosureDataSet.Category1Disclosure.Rows.Count == 0)
                    {
                        // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"分類１情報"}）
                        // 「！{0}が取得できません。！」
                        salesChangeSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "分類１情報" });

                        return ErrorStatus;
                    }

                    // 取得した分類１情報をセットする
                    mstStoreInvAppraisalDiv = category1DisclosureDataSet.Category1Disclosure[0].StoreInvAppraisalDiv;
                    salesChangeSlipInputProductsReadPDAEntity.AutoSalesChangeFlg = category1DisclosureDataSet.Category1Disclosure[0].AutoSalesChangeFlg;
                }
                #endregion
            }

            // 2013.10.20 AST Add Start
            #region 売単価の取得
            // 売単価取得方法(売変伝票)
            string salePriceGetKbn = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_UNIT_PRICE_FETCH_METHOD_SALE, salesChangeSlipInputProductsReadPDAEntity.CorporationCode);

            decimal taxExcludedPrice = 0m;
            decimal preTaxSaleUnitPrice = 0m;
            strProductControlAPI.CalcTaxIncludedPrice(
                        salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate.Value,
                        salesChangeSlipInputProductsReadPDAEntity.ProductsCd,
                        string.Empty,
                        ref taxExcludedPrice,
                        ref preTaxSaleUnitPrice);

            // 標準売価か最安値を設定する
            if (STANDARD_SELLING_PRICE_KBN.Equals(salePriceGetKbn))
            {
                // 標準売価を設定する
                salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = preTaxSaleUnitPrice;
            }
            else if (LOWEST_PRICE_KBN.Equals(salePriceGetKbn))
            {
                // ④　（OUT）SAR特売企画商品情報DataSet
                using (SARBargainPlanningProductStorePublicInformationSalesTableAdapter sarBargainPlanningProductStorePublicInformationSalesTableAdapter =
                    new SARBargainPlanningProductStorePublicInformationSalesTableAdapter())
                using (SARBargainPlanningProductStorePublicInformationDataSet sarBargainPlanningProductStorePublicInformationDataSet =
                    new SARBargainPlanningProductStorePublicInformationDataSet())
                {
                    sarBargainPlanningProductStorePublicInformationSalesTableAdapter.BargainPlanningProductStorePublicInformationSalesSearch(
                        salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                        salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate.Value.ToString("yyyyMMdd"),
                        string.Empty,
                        salesChangeSlipInputProductsReadPDAEntity.StoreCd,
                        salesChangeSlipInputProductsReadPDAEntity.Category1Cd,
                        salesChangeSlipInputProductsReadPDAEntity.SKUCd,
                        sarBargainPlanningProductStorePublicInformationDataSet);

                    if (sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Rows.Count != 0)
                    {
                        // 最安値を取得しセット
                        salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Select(r => r.SaleUnitPrice).Min();
                    }
                    else
                    {
                        // 特売売価が取得できない場合は定番売価を設定する
                        salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = preTaxSaleUnitPrice;
                    }
                }
            }
            else if (STORE_PRICE_KBN.Equals(salePriceGetKbn))
            {
                // 店別売価設定のとき
                #region (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                decimal storeTaxExcludedPrice = 0m;
                decimal storePreTaxSaleUnitPrice = 0m;
                strProductControlAPI.CalcTaxIncludedPrice(
                            salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate.Value,
                            salesChangeSlipInputProductsReadPDAEntity.ProductsCd,
                            salesChangeSlipInputProductsReadPDAEntity.StoreCd,
                            ref storeTaxExcludedPrice,
                            ref storePreTaxSaleUnitPrice);
                if (storePreTaxSaleUnitPrice > 0)
                {
                    salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = storePreTaxSaleUnitPrice;
                }
                else
                {
                    // 店別商品情報が取得できなかった場合
                    // 店別商品情報が取得できない場合は定番売価を設定する
                    salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = preTaxSaleUnitPrice;
                }
                #endregion
            }
            #endregion
            // 2013.10.20 AST Add End

            if (mstStoreInvAppraisalDiv != "4"
                && salesChangeSlipInputProductsReadPDAEntity.AutoSalesChangeFlg == "0")
            {
                // 店舗棚卸評価区分 <>「4：売価還元」And 自動売変フラグ = 「0：売価還元以外をエラーとする」の場合は正常終了
                // （基盤）エラーメッセージ取得（"ERRCSARB245", new string[]{""}）
                // 「！売価還元の商品ではありません。！」
                salesChangeSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB245, new string[] { "" });

                return ErrorStatus;
            }

            if (mstStoreInvAppraisalDiv == "4"
                && salesChangeSlipInputProductsReadPDAEntity.AutoSalesChangeFlg == "1")
            {
                // 店舗棚卸評価区分 =「4：売価還元」And 自動売変フラグ = 「1：自動売変可能」の場合は正常終了
                return NormalStatus;
            }
            else
            {
                // 2012.07.17 Start 特売売価の取得方法を変更
                #region (公開クラス)[BSCSPR09001000001]特売企画商品店公開情報取得ビジネスを呼び出す。
                //// ＳＰＲ特売企画商品店公開情報DataSet
                //using (SPRBargainPlanningProductStorePublicInformationDataSet sprBargainPlanningProductStorePublicInformationDataSet = new SPRBargainPlanningProductStorePublicInformationDataSet())
                //{
                //    // (公開クラス)[BSCSPR09001000001]特売企画商品店公開情報取得ビジネスを呼び出す。
                //    strBargainPlanningProductStorePublicInformationBusiness.BargainPlanningProductStorePublicInformationSearch(
                //        salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                //        salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate.Value.ToString("yyyyMMdd"),
                //        string.Empty,
                //        salesChangeSlipInputProductsReadPDAEntity.Category1Cd,
                //        salesChangeSlipInputProductsReadPDAEntity.SKUCd,
                //        salesChangeSlipInputProductsReadPDAEntity.StoreCd,
                //        "0",
                //        "0",
                //        "0",
                //        "1",
                //        sprBargainPlanningProductStorePublicInformationDataSet);

                //    if (sprBargainPlanningProductStorePublicInformationDataSet.SPRBargainPlanningProductStorePublicInformation.Count != 0)
                //    {
                //        // 取得した情報をセットする
                //        salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = sprBargainPlanningProductStorePublicInformationDataSet.SPRBargainPlanningProductStorePublicInformation[0].SaleUnitPrice;
                //    }
                //}
                #endregion

                #region 特売企画商品店情報取得ビジネスを呼び出す。
                // ＳＡＲ特売企画商品店情報DataSet
                using (SARBargainPlanningProductStorePublicInformationSalesTableAdapter sarBargainPlanningProductStorePublicInformationSalesTableAdapter = new SARBargainPlanningProductStorePublicInformationSalesTableAdapter())
                using (SARBargainPlanningProductStorePublicInformationDataSet sarBargainPlanningProductStorePublicInformationDataSet = new SARBargainPlanningProductStorePublicInformationDataSet())
                {
                    // (公開クラス)[BSCSPR09001000001]特売企画商品店公開情報取得ビジネスを呼び出す。
                    sarBargainPlanningProductStorePublicInformationSalesTableAdapter.BargainPlanningProductStorePublicInformationSalesSearch(
                        salesChangeSlipInputProductsReadPDAEntity.CorporationCode,
                        salesChangeSlipInputProductsReadPDAEntity.SalesChangeDate.Value.ToString("yyyyMMdd"),
                        string.Empty,
                        salesChangeSlipInputProductsReadPDAEntity.StoreCd,
                        salesChangeSlipInputProductsReadPDAEntity.Category1Cd,
                        salesChangeSlipInputProductsReadPDAEntity.SKUCd,
                        sarBargainPlanningProductStorePublicInformationDataSet);

                    if (sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Count != 0)
                    {
                        // 取得した情報をセットする
                        salesChangeSlipInputProductsReadPDAEntity.SaleUnitPrice = sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales[0].SaleUnitPrice;
                    }
                }
                #endregion
                // 2012.07.17 End   特売売価の取得方法を変更
            }

            return NormalStatus;
        }
        #endregion

        #region [BSCSAR05005000017] GetInventoryInformationPDANew (在庫情報取得BS)
        /// <summary>
        /// 在庫情報取得BS
        /// ID：BSCSAR05005000017
        /// </summary>
        /// <param name="corporationCode">*法人コード</param>
        /// <param name="salesChangeDate">*売変日</param>
        /// <param name="storeCd">*店舗コード</param>
        /// <param name="skuCd">*ＳＫＵコード</param>
        /// <param name="category1Cd">*分類１コード</param>
        /// <param name="category2Cd">*分類２コード</param>
        /// <param name="logicalStokQty">論理在庫数</param>
        public virtual void GetInventoryInformationPDANew(
            string corporationCode,
            DateTime? salesChangeDate,
            string storeCd,
            string skuCd,
            string category1Cd,
            string category2Cd,
            // --> 2012/07/17 在庫数取得不具合修正
            //decimal logicalStokQty)
            out decimal logicalStokQty)
        // <-- 2012/07/17 在庫数取得不具合修正
        {
            #region 必須入力チェック
            if (string.IsNullOrEmpty(corporationCode))
            {
                throw new ArgumentNullException("corporationCode");
            }
            if (salesChangeDate == null)
            {
                throw new ArgumentNullException("salesChangeDate");
            }
            if (string.IsNullOrEmpty(storeCd))
            {
                throw new ArgumentNullException("storeCd");
            }
            if (string.IsNullOrEmpty(skuCd))
            {
                throw new ArgumentNullException("skuCd");
            }
            if (string.IsNullOrEmpty(category1Cd))
            {
                throw new ArgumentNullException("category1Cd");
            }
            if (string.IsNullOrEmpty(category2Cd))
            {
                throw new ArgumentNullException("category2Cd");
            }
            #endregion

            // IVRStockControlAPIをインスタンス化
            IIVRStockControlAPI ivrStockControlAPI =
                (IIVRStockControlAPI)ClassObjectUtil.GetBusinessInstance(classIdIVRStockControlAPI, typeof(IIVRStockControlAPI));

            logicalStokQty = 0;

            #region (公開クラス)[BSCIVR01901000007]在庫情報取得を呼び出す
            // 在庫取得結果公開用DataSet
            using (GetStockResultPublicDataSet getStockResultPublicDataSet = new GetStockResultPublicDataSet())
            {
                // 在庫情報取得Entity
                StockSearchPublicEntity stockSearchPublicEntity = new StockSearchPublicEntity()
                {
                    CorporationCode = corporationCode,
                    StoreCd = storeCd,
                    BaseDate = Convert.ToDateTime(salesChangeDate),
                    SKUCd = skuCd,
                    Category1Cd = category1Cd,
                    Category2Cd = category2Cd,
                    QtyGroupDiv = StockSearchPublicEntity.QtyGroup.Single,          // 1：バラのまま
                    SubSystemDiv = "SA",
                    GetMethodDiv = StockSearchPublicEntity.GetMethod.RealStock      // 1：リアル在庫
                };

                // (公開クラス)[BSCIVR01901000007]在庫情報取得を呼び出す
                ivrStockControlAPI.GetStock(
                    stockSearchPublicEntity,
                    getStockResultPublicDataSet);

                // 取得できた場合は在庫数をセットする
                if (getStockResultPublicDataSet.GetStockResult.Rows.Count != 0)
                {
                    logicalStokQty = getStockResultPublicDataSet.GetStockResult[0].BookStockQty;
                }
            }
            #endregion
        }
        #endregion

        #region [BSCSAR05006000004] SearchDisposalSlipProductPDA (廃棄入力ＰＤＡ商品検索BS)
        /// <summary>
        /// 廃棄入力ＰＤＡ商品情報取得及び在庫情報取得処理を呼び出す。
        /// </summary>
        /// <param name="disposalSlipInputProductsReadPDAEntity">廃棄入力ＰＤＡ商品検索Entity</param>
        /// <returns>結果ステータス(0：正常 1：異常)</returns>
        public virtual int SearchDisposalSlipProductPDANew(
            ref DisposalSlipInputProductsReadPDAEntity disposalSlipInputProductsReadPDAEntity)
        {
            #region 必須入力チェック
            if (disposalSlipInputProductsReadPDAEntity == null)
            {
                throw new ArgumentNullException("disposalSlipInputProductsReadPDAEntity");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.CorporationCode))
            {
                throw new ArgumentNullException("Entity.CorporationCode");
            }
            if (disposalSlipInputProductsReadPDAEntity.SalesChangeDate == null)
            {
                throw new ArgumentNullException("Entity.SalesChangeDate");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.StoreCd))
            {
                throw new ArgumentNullException("Entity.StoreCd");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.ProductCd))
            {
                throw new ArgumentNullException("Entity.ProductCd");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.SalePriceDisp))
            {
                throw new ArgumentNullException("Entity.SalePriceDisp");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.SalePriceRoundOffMethod))
            {
                throw new ArgumentNullException("Entity.SalePriceRoundOffMethod");
            }
            #endregion

            int resultValue = NormalStatus;

            // [BSCSAR05006000002]廃棄入力ＰＤＡ商品情報取得ビジネスを呼び出す。
            resultValue = this.GetDisposalSlipProductInformationPDANew(
                            ref disposalSlipInputProductsReadPDAEntity);

            // 正常以外は処理終了
            if (resultValue != NormalStatus)
            {
                return resultValue;
            }

            // [BSCSAR05005000006]在庫情報取得ビジネスを呼び出す。
            decimal logicalStokQty;
            this.GetInventoryInformationPDANew(
                disposalSlipInputProductsReadPDAEntity.CorporationCode,
                disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                disposalSlipInputProductsReadPDAEntity.StoreCd,
                disposalSlipInputProductsReadPDAEntity.SKUCd,
                disposalSlipInputProductsReadPDAEntity.Category1Cd,
                disposalSlipInputProductsReadPDAEntity.Category2Cd,
                out logicalStokQty);
            disposalSlipInputProductsReadPDAEntity.LogicalStokQty = logicalStokQty;

            return resultValue;
        }
        #endregion

        #region [BSCSAR05006000005] GetDisposalSlipProductInformationPDANew (廃棄入力ＰＤＡ商品情報取得BS)
        /// <summary>
        /// 廃棄入力ＰＤＡ商品情報取得処理を行う。
        /// </summary>
        /// <param name="disposalSlipInputProductsReadPDAEntity">廃棄入力ＰＤＡ商品検索Entity</param>
        /// <returns>結果ステータス(0：正常 1：異常)</returns>
        public virtual int GetDisposalSlipProductInformationPDANew(
            ref DisposalSlipInputProductsReadPDAEntity disposalSlipInputProductsReadPDAEntity)
        {
            #region 必須入力チェック
            if (disposalSlipInputProductsReadPDAEntity == null)
            {
                throw new ArgumentNullException("disposalSlipInputProductsReadPDAEntity");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.CorporationCode))
            {
                throw new ArgumentNullException("Entity.CorporationCode");
            }
            if (disposalSlipInputProductsReadPDAEntity.SalesChangeDate == null)
            {
                throw new ArgumentNullException("Entity.SalesChangeDate");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.StoreCd))
            {
                throw new ArgumentNullException("Entity.StoreCd");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.ProductCd))
            {
                throw new ArgumentNullException("Entity.ProductCd");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.SalePriceDisp))
            {
                throw new ArgumentNullException("Entity.SalePriceDisp");
            }
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.SalePriceRoundOffMethod))
            {
                throw new ArgumentNullException("Entity.SalePriceRoundOffMethod");
            }
            #endregion

            // STRProductControlAPIをインスタンス化
            ISTRProductControlAPI strProductControlAPI =
                (ISTRProductControlAPI)ClassObjectUtil.GetBusinessInstance(classIdSTRProductControlAPI, typeof(ISTRProductControlAPI));

            // CmrCategoryAPIをインスタンス化
            ICmrCategoryAPI cmrCategoryAPI =
                (ICmrCategoryAPI)ClassObjectUtil.GetBusinessInstance(classIdCmrCategoryAPI, typeof(ICmrCategoryAPI));

            // ローカル変数の宣言
            decimal mstMarginRate = 0;
            decimal? codePrice = null;
            decimal? codePreTaxPrice = null;
            decimal? codeCost = null;
            decimal? codePreTaxCost = null;

            #region (公開クラス)[BSCSTR10000000038]スキャニング商品情報取得ビジネスを呼び出す。
            // ＳＴＲ商品公開用DataSet
            STRProductDisclosureDataSet strProductDisclosureDataSet = null;
            using (strProductDisclosureDataSet = new STRProductDisclosureDataSet())
            {
                // (公開クラス)[BSCSTR10000000038]スキャニング商品情報取得ビジネスを呼び出す。
                strProductControlAPI.GetProductInfoByScannedData(
                    disposalSlipInputProductsReadPDAEntity.CorporationCode,
                    disposalSlipInputProductsReadPDAEntity.ProductCd,
                    disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                    out codePrice,
                    out codePreTaxPrice,
                    out codeCost,
                    out codePreTaxCost,
                    ref strProductDisclosureDataSet);

                // 商品未存在の場合はエラーとして終了
                if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Rows.Count == 0)
                {
                    // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"商品情報"}）
                    // 「！{0}が取得できません。！」
                    disposalSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "商品情報" });

                    return ErrorStatus;
                }

                // 取得した商品情報をセットする
                disposalSlipInputProductsReadPDAEntity.ProductCd = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].OwnProductsCd;
                disposalSlipInputProductsReadPDAEntity.ScanningCd = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].ScanningCd;
                disposalSlipInputProductsReadPDAEntity.SKUCd = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].SKUCd;
            }
            #endregion

            #region (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
            // ＳＴＲ店別商品公開用DataSet
            STRStoreProductsDisclosureDataSet strStoreProductsDisclosureDataSet = null;
            using (strStoreProductsDisclosureDataSet = new STRStoreProductsDisclosureDataSet())
            {
                // 商品店検索条件Entity
                StoreProductsSearchConditionEntity storeProductsSearchConditionEntity = new StoreProductsSearchConditionEntity()
                {
                    CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode,
                    StoreCd = disposalSlipInputProductsReadPDAEntity.StoreCd,
                    SearchBasicDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                    OwnProductsCd = disposalSlipInputProductsReadPDAEntity.ProductCd,
                    ScanningCd = disposalSlipInputProductsReadPDAEntity.ScanningCd,
                    SKUCd = disposalSlipInputProductsReadPDAEntity.SKUCd
                };

                // (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                // ※区分="2"(販売終了（停止）は存在なし)
                strProductControlAPI.GetStoreProductsLineup(
                    "2",
                    storeProductsSearchConditionEntity,
                    ref strStoreProductsDisclosureDataSet);

                if (strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure.Count > 0)
                {
                    // 取得した商品情報をセット(先頭の１件のみをセット)
                    disposalSlipInputProductsReadPDAEntity.Category1Cd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].Category1Cd;
                    disposalSlipInputProductsReadPDAEntity.Category2Cd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].Category2Cd;
                    disposalSlipInputProductsReadPDAEntity.Category3Cd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].Category3Cd;
                    disposalSlipInputProductsReadPDAEntity.Category4Cd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].Category4Cd;
                    disposalSlipInputProductsReadPDAEntity.SKUCd = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].SKUCd;
                    disposalSlipInputProductsReadPDAEntity.ProductsKanji = strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].ProductsKanji;
                    disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = codePrice != null ? codePrice.Value : strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].SaleUnitPrice;
                    disposalSlipInputProductsReadPDAEntity.OriginUnitPrice = codeCost != null ? codeCost.Value : strStoreProductsDisclosureDataSet.STRStoreProductsDisclosure[0].CostUnitPrice;
                }
                else
                {
                    // 商品情報が取得できなかった場合
                    #region (公開クラス)[BSCSTR10000000001]商品コード存在チェック＆名称取得ビジネスを呼び出す。
                    // 商品コード存在チェック＆ゲットDataSet
                    STRProductCodeExistCheckAndGetDataSet strProductCodeExistCheckAndGetDataSet = null;
                    using (strProductCodeExistCheckAndGetDataSet = new STRProductCodeExistCheckAndGetDataSet())
                    {
                        // 商品検索条件Entity
                        ProductsSearchConditionEntity productsSearchConditionEntity = new ProductsSearchConditionEntity()
                        {
                            CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode,
                            SearchBasicDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                            OwnProductsCd = disposalSlipInputProductsReadPDAEntity.ProductCd,
                            ScanningCd = disposalSlipInputProductsReadPDAEntity.ScanningCd,
                            SKUCd = disposalSlipInputProductsReadPDAEntity.SKUCd
                        };

                        // (公開クラス)[BSCSTR10000000001]商品コード存在チェック＆名称取得ビジネスを呼び出す。
                        strProductControlAPI.ProductsCodeExistCheckAndGet(
                            productsSearchConditionEntity,
                            ref strProductCodeExistCheckAndGetDataSet);

                        if (strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet.Rows.Count == 0)
                        {
                            // 商品情報が取得できなかった場合
                            #region (公開クラス)[BSCSTR10000000014]商品基本属性（標準パターン付き取得）ビジネスを呼び出す。
                            // ＳＴＲ商品（標準パターン付き）公開用DataSet
                            STRProductWithPatternDisclosureDataSet strProductWithPatternDisclosureDataSet = null;
                            using (strProductWithPatternDisclosureDataSet = new STRProductWithPatternDisclosureDataSet())
                            {
                                // 商品検索条件Entity
                                ProductsSearchConditionEntity productsSearchConditionWithPatternEntity = new ProductsSearchConditionEntity()
                                {
                                    CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode,
                                    SearchBasicDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                                    OwnProductsCd = disposalSlipInputProductsReadPDAEntity.ProductCd,
                                    ScanningCd = disposalSlipInputProductsReadPDAEntity.ScanningCd,
                                    SKUCd = disposalSlipInputProductsReadPDAEntity.SKUCd
                                };

                                // (公開クラス)[BSCSTR10000000014]商品基本属性（標準パターン付き取得）ビジネスを呼び出す。
                                // ※区分="9"(取扱い終了も取得する)
                                strProductControlAPI.GetProductBasicAttributeWithPattern(
                                    "9",
                                    productsSearchConditionWithPatternEntity,
                                    ref strProductWithPatternDisclosureDataSet);

                                if (strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure.Rows.Count == 0)
                                {
                                    // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"商品情報"}）
                                    // 「！{0}が取得できません。！」
                                    disposalSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "商品情報" });

                                    return ErrorStatus;
                                }
                                else
                                {
                                    // 取得した商品情報をセット(先頭の１件のみをセット)
                                    disposalSlipInputProductsReadPDAEntity.Category1Cd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Category1Cd;
                                    disposalSlipInputProductsReadPDAEntity.Category2Cd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Category2Cd;
                                    disposalSlipInputProductsReadPDAEntity.Category3Cd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Category3Cd;
                                    disposalSlipInputProductsReadPDAEntity.Category4Cd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Category4Cd;
                                    disposalSlipInputProductsReadPDAEntity.SKUCd = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].SKUCd;
                                    disposalSlipInputProductsReadPDAEntity.ProductsKanji = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].ProductsKanji;
                                    disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = codePrice != null ? codePrice.Value : strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].SaleUnitPrice;
                                    disposalSlipInputProductsReadPDAEntity.OriginUnitPrice = codeCost != null ? codeCost.Value : strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].CostUnitPrice;
                                    mstMarginRate = strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].MarginRate;
                                    if (string.IsNullOrEmpty(strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Status) == false)
                                    {
                                        // （基盤）エラーメッセージ取得（"ERRCSARB244", new string[]{STR商品（標準パターン付き）公開用DataSet．ステータス}）
                                        // 「！商品の状態が「{0}」です。継続しますか？！」
                                        disposalSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB244, new string[] { strProductWithPatternDisclosureDataSet.STRProductWithPatternDisclosure[0].Status });
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            // 商品情報が取得できた場合
                            #region (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                            // ＳＴＲ店別商品公開用DataSet
                            STRStoreProductsDisclosureDataSet strStoreProductsDisclosureReDataSet = null;
                            using (strStoreProductsDisclosureReDataSet = new STRStoreProductsDisclosureDataSet())
                            {
                                // 商品店検索条件Entity
                                StoreProductsSearchConditionEntity storeProductsSearchConditionReEntity = new StoreProductsSearchConditionEntity()
                                {
                                    CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode,
                                    StoreCd = disposalSlipInputProductsReadPDAEntity.StoreCd,
                                    SearchBasicDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                                    OwnProductsCd = disposalSlipInputProductsReadPDAEntity.ProductCd,
                                    ScanningCd = disposalSlipInputProductsReadPDAEntity.ScanningCd,
                                    SKUCd = disposalSlipInputProductsReadPDAEntity.SKUCd
                                };

                                // (公開クラス)[BSCSTR10000000005]店別商品情報取得ビジネスを呼び出す。
                                // ※区分="0"取扱い終了は存在なし)
                                strProductControlAPI.GetStoreProductsLineup(
                                    "0",
                                    storeProductsSearchConditionReEntity,
                                    ref strStoreProductsDisclosureReDataSet);

                                if (strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure.Count > 0)
                                {
                                    // 取得した商品情報をセット(先頭の１件のみをセット)
                                    disposalSlipInputProductsReadPDAEntity.Category1Cd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].Category1Cd;
                                    disposalSlipInputProductsReadPDAEntity.Category2Cd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].Category2Cd;
                                    disposalSlipInputProductsReadPDAEntity.Category3Cd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].Category3Cd;
                                    disposalSlipInputProductsReadPDAEntity.Category4Cd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].Category4Cd;
                                    disposalSlipInputProductsReadPDAEntity.SKUCd = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].SKUCd;
                                    disposalSlipInputProductsReadPDAEntity.ProductsKanji = strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].ProductsKanji;
                                    disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = codePrice != null ? codePrice.Value : strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].SaleUnitPrice;
                                    disposalSlipInputProductsReadPDAEntity.OriginUnitPrice = codeCost != null ? codeCost.Value : strStoreProductsDisclosureReDataSet.STRStoreProductsDisclosure[0].CostUnitPrice;
                                    if (string.IsNullOrEmpty(strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet[0].StatusKanji) == false)
                                    {
                                        // （基盤）エラーメッセージ取得（"ERRCSARB244", new string[]{STR商品（標準パターン付き）公開用DataSet．ステータス}）
                                        // 「！商品の状態が「{0}」です。継続しますか？！」
                                        disposalSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB244, new string[] { strProductCodeExistCheckAndGetDataSet.STRProductCodeExistCheckAndGet[0].StatusKanji });
                                    }
                                }
                                else
                                {
                                    // 商品情報が取得できなかった場合
                                    // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"商品情報"}）
                                    // 「！{0}が取得できません。！」
                                    disposalSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "商品情報" });

                                    return ErrorStatus;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region (公開クラス)[BSCCMR01003000002]ＣＭＲ分類２公開情報取得ビジネスを呼び出す。
            disposalSlipInputProductsReadPDAEntity.StoreInvAppraisalDiv = string.Empty;
            disposalSlipInputProductsReadPDAEntity.AutoSalesChangeFlg = "0";

            // 分類２公開DataSet
            using (Category2DisclosureDataSet category2DisclosureDataSet = new Category2DisclosureDataSet())
            {
                // 分類２公開検索条件Entity
                Category2DisclosureSearchConditionEntity category2DisclosureSearchConditionEntity = new Category2DisclosureSearchConditionEntity()
                {
                    CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode,
                    BaseDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                    Category1CD = disposalSlipInputProductsReadPDAEntity.Category1Cd,
                    Category2CD = disposalSlipInputProductsReadPDAEntity.Category2Cd,
                    RegistrationDiv = string.Empty
                };

                // (公開クラス)[BSCCMR01003000002]ＣＭＲ分類２公開情報取得ビジネスを呼び出す。
                cmrCategoryAPI.GetCategory2Disclosure(
                    "SA",
                    category2DisclosureSearchConditionEntity,
                    category2DisclosureDataSet);

                // 分類２未存在の場合はエラーとして終了
                if (category2DisclosureDataSet.Category2Disclosure.Rows.Count == 0)
                {
                    // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"分類２情報"}）
                    // 「！{0}が取得できません。！」
                    disposalSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "分類２情報" });

                    return ErrorStatus;
                }

                // 取得した分類２情報をセットする
                disposalSlipInputProductsReadPDAEntity.StoreInvAppraisalDiv = category2DisclosureDataSet.Category2Disclosure[0].StoreInvAppraisalDiv;
                disposalSlipInputProductsReadPDAEntity.AutoSalesChangeFlg = category2DisclosureDataSet.Category2Disclosure[0].AutoSalesChangeFlg;
            }
            #endregion

            // 店舗棚卸評価区分が空の場合は分類１情報を取得する
            if (string.IsNullOrEmpty(disposalSlipInputProductsReadPDAEntity.StoreInvAppraisalDiv) == true)
            {
                #region (公開クラス)[BSCCMR01003000001]ＣＭＲ分類１公開情報取得ビジネスを呼び出す。
                // 分類１公開DataSet
                using (Category1DisclosureDataSet category1DisclosureDataSet = new Category1DisclosureDataSet())
                {
                    // 分類１公開検索条件Entity
                    Category1DisclosureSearchConditionEntity category1DisclosureSearchConditionEntity = new Category1DisclosureSearchConditionEntity()
                    {
                        CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode,
                        BaseDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                        Category1CD = disposalSlipInputProductsReadPDAEntity.Category1Cd,
                        RegistrationDiv = string.Empty
                    };

                    // (公開クラス)[BSCCMR01003000001]ＣＭＲ分類１公開情報取得ビジネスを呼び出す。
                    cmrCategoryAPI.GetCategory1Disclosure(
                        "SA",
                        category1DisclosureSearchConditionEntity,
                        category1DisclosureDataSet);

                    // 分類１未存在の場合はエラーとして終了
                    if (category1DisclosureDataSet.Category1Disclosure.Rows.Count == 0)
                    {
                        // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"分類１情報"}）
                        // 「！{0}が取得できません。！」
                        disposalSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "分類１情報" });

                        return ErrorStatus;
                    }

                    // 取得した分類１情報をセットする
                    disposalSlipInputProductsReadPDAEntity.StoreInvAppraisalDiv = category1DisclosureDataSet.Category1Disclosure[0].StoreInvAppraisalDiv;
                    disposalSlipInputProductsReadPDAEntity.AutoSalesChangeFlg = category1DisclosureDataSet.Category1Disclosure[0].AutoSalesChangeFlg;
                }
                #endregion
            }

            // 2013.10.20 AST Add Start
            #region 売単価の取得
            // 売単価取得方法(廃棄伝票)
            string salePriceGetKbn = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_UNIT_PRICE_FETCH_METHOD_DISPOSAL, disposalSlipInputProductsReadPDAEntity.CorporationCode);

            if (codePrice != null)
            {
                // Non-Pluで売価取得済の時
                return NormalStatus;
            }

            switch (salePriceGetKbn)
            {
                case STORE_PRICE_KBN:
                    // 店別売価設定のとき
                    break;
                case STANDARD_SELLING_PRICE_KBN:
                case LOWEST_PRICE_KBN:
                    // 標準売価か最安値を設定する
                    STRProductWithPatternDisclosureDataSet strProductWithPatternDisclosureDs = null;
                    using (strProductWithPatternDisclosureDs = new STRProductWithPatternDisclosureDataSet())
                    {
                        ProductsSearchConditionEntity productsSearchConditionEnt = new ProductsSearchConditionEntity();
                        productsSearchConditionEnt.CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode;
                        productsSearchConditionEnt.SKUCd = disposalSlipInputProductsReadPDAEntity.SKUCd;
                        productsSearchConditionEnt.SearchBasicDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate.Value;

                        strProductControlAPI.GetProductBasicAttributeWithPattern(
                            "9",
                            productsSearchConditionEnt,
                            ref strProductWithPatternDisclosureDs);


                        if (strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Rows.Count > 0)
                        {

                            // 定番売価を取得
                            if (strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Any(r => r.SellPricePatternNo == "1")
                                && strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Where(r => r.SellPricePatternNo == "1").First().IsSaleUnitPriceNull() == false)
                            {
                                disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = strProductWithPatternDisclosureDs.STRProductWithPatternDisclosure.Where(r => r.SellPricePatternNo == "1").First().SaleUnitPrice;
                            }
                            else
                            {
                                productsSearchConditionEnt = new ProductsSearchConditionEntity();
                                productsSearchConditionEnt.CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode;
                                productsSearchConditionEnt.SKUCd = disposalSlipInputProductsReadPDAEntity.SKUCd;
                                productsSearchConditionEnt.SearchBaseDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate.Value;

                                STRSKUPurchasePatternDisclosureDataSet strSKUPurchasePatternDisclosureDs = null;
                                using (strSKUPurchasePatternDisclosureDs = new STRSKUPurchasePatternDisclosureDataSet())
                                {
                                    strProductControlAPI.GetSKUPurchasePatternDisclosure(
                                        productsSearchConditionEnt,
                                        "",
                                        "",
                                        "",
                                        ref strSKUPurchasePatternDisclosureDs);

                                    if (strSKUPurchasePatternDisclosureDs.STRSKUPurchasePatternDisclosure.Count > 0
                                        && strSKUPurchasePatternDisclosureDs.STRSKUPurchasePatternDisclosure.Any(r => r.InvStock == "1")
                                        && strSKUPurchasePatternDisclosureDs.STRSKUPurchasePatternDisclosure.Where(r => r.InvStock == "1").First().IsPriceAtStockNull() == false)
                                    {
                                        disposalSlipInputProductsReadPDAEntity.SaleUnitPrice =
                                            strSKUPurchasePatternDisclosureDs.STRSKUPurchasePatternDisclosure.Where(r => r.InvStock == "1").First().PriceAtStock;
                                    }
                                    else
                                    {
                                        disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = 0;
                        }
                    }

                    if (LOWEST_PRICE_KBN.Equals(salePriceGetKbn))
                    {
                        // 最安値を設定する
                        // （OUT）SAR特売企画商品情報DataSet
                        using (SARBargainPlanningProductStorePublicInformationSalesTableAdapter sarBargainPlanningProductStorePublicInformationSalesTableAdapter =
                            new SARBargainPlanningProductStorePublicInformationSalesTableAdapter())
                        using (SARBargainPlanningProductStorePublicInformationDataSet sarBargainPlanningProductStorePublicInformationDataSet =
                            new SARBargainPlanningProductStorePublicInformationDataSet())
                        {
                            sarBargainPlanningProductStorePublicInformationSalesTableAdapter.BargainPlanningProductStorePublicInformationSalesSearch(
                                disposalSlipInputProductsReadPDAEntity.CorporationCode,
                                disposalSlipInputProductsReadPDAEntity.SalesChangeDate.Value.ToString("yyyyMMdd"),
                                string.Empty,
                                disposalSlipInputProductsReadPDAEntity.StoreCd,
                                disposalSlipInputProductsReadPDAEntity.Category1Cd,
                                disposalSlipInputProductsReadPDAEntity.SKUCd,
                                sarBargainPlanningProductStorePublicInformationDataSet);

                            if (sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Rows.Count != 0)
                            {
                                // 最安値を取得しセット
                                disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = sarBargainPlanningProductStorePublicInformationDataSet.SARBargainPlanningProductStorePublicInformationSales.Select(r => r.SaleUnitPrice).Min();
                            }
                        }
                    }
                    break;
                default:
                    break;

            }
            // <-- 2013/11/01 マスタ売価対応
            #endregion
            // 2013.10.20 AST Add End

            if (disposalSlipInputProductsReadPDAEntity.StoreInvAppraisalDiv == "4"
                && disposalSlipInputProductsReadPDAEntity.AutoSalesChangeFlg == "1")
            {
                // 店舗棚卸評価区分 =「4：売価還元」And 自動売変フラグ = 「1：自動売変可能」の場合は正常終了
                return NormalStatus;
            }
            else if (codePrice != null)
            {
                // Non-Pluで売価取得済の時
                return NormalStatus;
            }

            // 仕入売価表示 =「1」And 店舗棚卸評価区分 =「4：売価還元」And 売単価 = 0の場合
            if (disposalSlipInputProductsReadPDAEntity.SalePriceDisp == "1"
                && disposalSlipInputProductsReadPDAEntity.StoreInvAppraisalDiv == "4"
                && disposalSlipInputProductsReadPDAEntity.SaleUnitPrice == 0)
            {
                // 値入率 = 0 の場合
                if (mstMarginRate == 0)
                {
                    #region (公開クラス)[BSCSTR10000000011]商品基本属性取得を呼び出す
                    // ＳＴＲ商品公開用DataSet
                    STRProductDisclosureDataSet strProductDisclosureMarginRateDataSet = null;
                    using (strProductDisclosureMarginRateDataSet = new STRProductDisclosureDataSet())
                    {
                        // 商品検索条件Entity
                        ProductsSearchConditionEntity productsSearchConditionEntity = new ProductsSearchConditionEntity()
                        {
                            CorporationCode = disposalSlipInputProductsReadPDAEntity.CorporationCode,
                            SearchBasicDate = disposalSlipInputProductsReadPDAEntity.SalesChangeDate,
                            OwnProductsCd = disposalSlipInputProductsReadPDAEntity.ProductCd,
                            ScanningCd = disposalSlipInputProductsReadPDAEntity.ScanningCd,
                            SKUCd = disposalSlipInputProductsReadPDAEntity.SKUCd
                        };

                        // (公開クラス)[BSCSTR10000000011]商品基本属性取得を呼び出す
                        // ※区分="9"(取扱い終了も取得する)
                        strProductControlAPI.GetProductBasicAttribute(
                            "9",
                            productsSearchConditionEntity,
                            ref strProductDisclosureMarginRateDataSet);

                        if (strProductDisclosureMarginRateDataSet.STRProductBasicAttributeDisclosure.Rows.Count == 0)
                        {
                            // （基盤）エラーメッセージ取得（"ERRCSARB243", new string[]{"商品情報"}）
                            // 「！{0}が取得できません。！」
                            disposalSlipInputProductsReadPDAEntity.Message = ResourceUtil.GetErrorMessage(ErrorKey.ERRCSARB243, new string[] { "商品情報" });

                            return ErrorStatus;
                        }
                        else
                        {
                            // 取得した商品情報をセット(先頭の１件のみをセット)
                            mstMarginRate = strProductDisclosureMarginRateDataSet.STRProductBasicAttributeDisclosure[0].MarginRate;
                        }
                    }
                    #endregion
                }

                // 売単価の丸め
                switch (disposalSlipInputProductsReadPDAEntity.SalePriceRoundOffMethod)
                {
                    case "1":           // 四捨五入
                        // 売単価 = Math.Round(原単価 ÷ (1 － 値入率 ÷ 100), MidpointRounding.AwayFromZero)
                        disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = Math.Round(disposalSlipInputProductsReadPDAEntity.OriginUnitPrice / (1 - mstMarginRate / 100), MidpointRounding.AwayFromZero);
                        break;
                    case "2":           // 切上げ
                        // 売単価 = Math.Ceiling(原単価 ÷ (1 － 値入率 ÷ 100))
                        disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = Math.Ceiling(disposalSlipInputProductsReadPDAEntity.OriginUnitPrice / (1 - mstMarginRate / 100));
                        break;
                    case "3":           // 切捨て
                        // 売単価 = Math.Floor(原単価 ÷ (1 － 値入率 ÷ 100))
                        disposalSlipInputProductsReadPDAEntity.SaleUnitPrice = Math.Floor(disposalSlipInputProductsReadPDAEntity.OriginUnitPrice / (1 - mstMarginRate / 100));
                        break;
                    default:
                        goto case "1";
                }
            }

            return NormalStatus;
        }
        #endregion


        //2017/06/20 k.sugawara Start バッチ番号を自動採番できるようにする。
        /// <summary>
        /// カバーシート情報の保存を行う。
        /// (BSCSAR05008000179)
        /// </summary>
        /// <remarks>
        /// （１）起動モードが「新規」の場合
        /// ・バッチ番号の自動採番メソッドを呼び出す
        /// （２）起動モードが「伝票検索」か「カバーシートマスタ」の場合
        ///・伝票検索処理を呼び出す。
        /// </remarks>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <returns>判定結果</returns>
        public virtual string GetBatchNoNumberingBusiness(
           WorkEntity workEntity,
           DsEntity dsEntity)
        {
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

            if (workEntity.InitMode == initMode_New)
            {
                //バッチ番号の新規取得
                //販売バッチ番号採番ビジネスを下記の引数で呼び出す
                string byrCommonBusinessId = "BSCBYR07300";
                IByrCommonBusiness byrCommonBusiness = (IByrCommonBusiness)ClassObjectUtil.GetBusinessInstance(byrCommonBusinessId, typeof(IByrCommonBusiness));
                dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].BatchNo
                    = this.GetNewBatchNoNumberingBusiness(
                        dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].CorporationCode,
                        dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].SlipTypeCd,
                        dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].SlipDiv,
                        dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].BatchDate,
                        dsEntity.CoverSheetMasterDataset.CoverSheetMasterDataTable1[0].StoreCd,
                        null);
            }
            else if (workEntity.InitMode == initMode_SlipSearch || workEntity.InitMode == initMode_CoverSheetMaster)
            {
                this.FillSlipNew(workEntity, dsEntity);
            }

            // 伝票番号取得を実施する。
            this.FillSlipNoNew(workEntity, dsEntity);

            return Constants.OffFlag;
        }

        /// <summary>
        /// 登録済みのカバーシートを取得する。
        /// (BSCSAR05008000131)
        /// </summary>
        /// <param name= "coverSheetMasterEntity">カバーシート情報Entity</param>
        /// <param name= "dsEntity">DsEntity</param>
        public virtual void FillSARCoverSheetMasterNew(
           CoverSheetMasterEntity coverSheetMasterEntity,
           DsEntity dsEntity)
        {
            #region 必須入力チェック

            if (coverSheetMasterEntity == null)
            {
                throw new ArgumentNullException("coverSheetMasterEntity");
            }

            if (dsEntity == null)
            {
                throw new ArgumentNullException("dsEntity");
            }
            #endregion

            // カバーシート検索と伝票（トラン）の検索実施
            // カバー検索(DAOCSAR05030)を呼び出す。
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
                {
                    dataAccessComponent.FillSARCoverSheetMasterNew(coverSheetMasterEntity, dsEntity.CoverSheetMasterDataset);
                }
        }

        /// <summary>
        /// 販売バッチ番号採番ビジネス
        /// </summary>
        /// <param name= "corporationCode">(IN) 法人コード</param>
        /// <param name= "slipTypeCd">(IN) 伝票種別コード</param>
        /// <param name= "slipDiv">(IN) 伝票区分</param>
        /// <param name= "batchDate">(IN) バッチ日付</param>
        /// <param name= "batchStore">(IN) バッチ店舗</param>
        /// <param name= "manualBatchNo">(IN) 手入力バッチ番号</param>
        /// <returns>バッチ番号</returns>
        public virtual string GetNewBatchNoNumberingBusiness(
            string corporationCode,
            string slipTypeCd,
            string slipDiv,
            DateTime? batchDate = null,
            string batchStore = "",
            string manualBatchNo = null)
        {
            #region 必須チェック
            if (string.IsNullOrEmpty(corporationCode))
            {
                throw new ArgumentNullException("corporationCode");
            }
            if (string.IsNullOrEmpty(slipTypeCd))
            {
                throw new ArgumentNullException("slipTypeCd");
            }
            if (string.IsNullOrEmpty(slipDiv))
            {
                throw new ArgumentNullException("slipDiv");
            }
            #endregion 必須チェック

            //バッチサイクル値
            string inputBatchNoCycle = "9999999999";

            //バッチ番号自動発番最小値
            string batchNoAutoStartValue = "1";

            //仕入バッチ番号採番
            CoverSheetMasterEntity byBatchNumberMasterEntity = new CoverSheetMasterEntity();
            byBatchNumberMasterEntity.CorporationCode = corporationCode;
            byBatchNumberMasterEntity.StoreCd = batchStore;
            byBatchNumberMasterEntity.SlipTypeCd = slipTypeCd;
            byBatchNumberMasterEntity.SlipDiv = slipDiv;
            byBatchNumberMasterEntity.BatchDate = batchDate;

            string batchNo = "";
            using (SalesChangeDisposalSlipEntryDataAccessComponent salesChangeDataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                batchNo = salesChangeDataAccessComponent.UpdateByMaxBatchNo(byBatchNumberMasterEntity, Convert.ToDecimal(inputBatchNoCycle), Convert.ToDecimal(batchNoAutoStartValue));
            }

            //バッチ番号を戻す
            return batchNo;
        }

        //2017/06/20 k.sugawara End バッチ番号を自動採番できるようにする。

        #region private methods
        /// <summary>
        /// 伝票明細更新Ds内にエラーがある場合のチェック
        /// </summary>
        /// <param name="sARSlipDetailErrorInfoDataSet">伝票明細更新Ds</param>
        /// <returns>True:伝票明細更新Ds内にエラーがある場合</returns>
        private bool HasDetailErrorNew(SARSlipDetailErrorInfoDataSet sARSlipDetailErrorInfoDataSet)
        {
            foreach (var row in sARSlipDetailErrorInfoDataSet.SARSlipDetailError)
            {
                if (row.ErrorDiv == Constants.ErrorDiv_Fatal)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 大福帳データセットを作成する
        /// </summary>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        /// <param name="dataID">データID</param>
        /// <param name="coverSheetMasterEntity">カバーシートマスタワークEntity</param>
        /// <returns>AccountBookDataSet</returns>
        // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
        //private AccountBookDataSet GenerateAccountBookDataSet(WorkEntity workEntity, DsEntity dsEntity, string dataID)
        private AccountBookDataSet GenerateAccountBookDataSetNew(WorkEntity workEntity, DsEntity dsEntity, string dataID, CoverSheetMasterEntity coverSheetMasterEntity)
        // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
        {
            using (AccountBookDataSet accountBookDs = new AccountBookDataSet())
            {
                var headerRow = dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError[0];
                decimal tempSeq = 1;

                // 暫定修正 2011.11.29 Start 大福帳の伝票が増幅していくのを修正
                ICmcAccountBookComponent cmcAccountBookComponent = this.GetCmcAccountBookComponentNew();
                AccountBookDataSet accountBookHistory = new AccountBookDataSet();
                //   2012.01.16 関根 大福帳抽出で店舗コード指定ありメソッドを使用する 開始
                //cmcAccountBookComponent.ExtractAccountBookBase(
                cmcAccountBookComponent.ExtractAccountBookBaseStore(
                    //   2012.01.16 関根 大福帳抽出で店舗コード指定ありメソッドを使用する 終了
                    out accountBookHistory,
                    dataID,
                    headerRow.CorporationCode,
                    headerRow.StoreCd,
                    headerRow.SlipDate,
                    headerRow.SlipDiv,
                    headerRow.SlipTypeCd,
                    headerRow.SlipNo,
                    string.Empty,
                    null,
                    null,
                    // 8/21 SUEYOSHI 固定値で渡していた為、修正
                    // Constants.OnFlag,
                    string.Empty,
                    // 8/21 SUEYOSHI END
                    string.Empty,
                    string.Empty,
                    Constants.OnFlag);

                decimal newHistoryNo = 1;
                if (accountBookHistory.AccountBook != null && accountBookHistory.AccountBook.Rows.Count != 0)
                {
                    accountBookHistory.AccountBook.DefaultView.Sort = accountBookHistory.AccountBook.HistoryNoColumn.ColumnName + " desc";
                    if (accountBookHistory.AccountBook.DefaultView[0][accountBookHistory.AccountBook.HistoryNoColumn.ColumnName] != null
                        && accountBookHistory.AccountBook.DefaultView[0][accountBookHistory.AccountBook.HistoryNoColumn.ColumnName] != System.DBNull.Value
                        && String.IsNullOrEmpty(accountBookHistory.AccountBook.DefaultView[0][accountBookHistory.AccountBook.HistoryNoColumn.ColumnName].ToString()) == false)
                    {
                        newHistoryNo = (decimal)accountBookHistory.AccountBook.DefaultView[0][accountBookHistory.AccountBook.HistoryNoColumn.ColumnName];
                        // 1加算する    
                        newHistoryNo += 1;
                    }
                }


                // 2012.01.21 在庫管理区分の更新対応 開始
                string businessId = "BSCSTR10000";
                ISTRProductControlAPI strProductControlAPI = (ISTRProductControlAPI)ClassObjectUtil.GetBusinessInstance(businessId, typeof(ISTRProductControlAPI));
                // 2012.01.21 在庫管理区分の更新対応 終了

                // 2012.06.13 Start 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
                var slipTypeCd_Disposal = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_SLIPTYPECD, UserProfile.CorporationCode);
                // 2012.06.13 End   売変伝票区分、廃棄伝票区分をAP変数より取得するように変更

                // 2012.06.21 Start 大福帳へセットする内容の訂正
                string cmcTaxRateAPIId = "BSCCMR01010000";
                ICmcTaxRateAPI cmcTaxRateAPI = (ICmcTaxRateAPI)ClassObjectUtil.GetBusinessInstance(cmcTaxRateAPIId, typeof(ICmcTaxRateAPI));

                var typeOfTradeCd = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SALESSLIPTYPEOFTRADECD, UserProfile.CorporationCode);
                var typeOfTradeSubCd = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SALESSLIPTYPEOFTRADEDETAIL, UserProfile.CorporationCode);

                string taxDefaultRateCd = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_TAX_DEFAULT_RATECD, UserProfile.CorporationCode);
                // 2012.06.21 End   大福帳へセットする内容の訂正

                // 2013.11.02 AST Del Start
                #region 削除
                //// 2012.06.21 Start ＰＯＳ値引区分にＡＰ変数の値をセットしておく
                //var slipPosdiscountDiv = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SLIP_POSDISCOUNT_DIV, UserProfile.CorporationCode);
                //// 2012.06.21 End   ＰＯＳ値引区分にＡＰ変数の値をセットしておく
                #endregion
                // 2013.11.02 AST Del End

                // 2013.11.01 AST Add Start
                // 廃棄標準売変評価差額値上伝票区分
                string disposalBalanceUp = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_UP, UserProfile.CorporationCode);
                // 廃棄標準売変評価差額値上伝票区分
                string disposalBalanceDown = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_DOWN, UserProfile.CorporationCode);
                // 廃棄評価差額伝票種別
                string disposalBalanceSlipTypeCd = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_BALANCE_SLIP_TYPE_CD, UserProfile.CorporationCode);
                // 全店コード
                string allStoreCd = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_ALL_STORE_CODE, workEntity.CorporationCode);
                // 2013.11.01 AST Add End

                // --> 2013/11/01 マスタ売価対応
                // 消費税の端数処理区分は、商談の設定より取得
                string fractionDiv = APVariableUtil.GetAPVariableValue(APVariableListKey.STR_FRACTIONDIV, workEntity.CorporationCode);
                // <-- 2013/11/01 マスタ売価対応

                // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） START
                string dataID_Disposal = string.Empty;
                string dataID_SalesChange = string.Empty;
                dataID_Disposal = CodeDivisionUtil.GetCodeDivisionValueName(CodeDivisionKey.DataId, "04", UserProfile.CorporationCode);
                dataID_SalesChange = CodeDivisionUtil.GetCodeDivisionValueName(CodeDivisionKey.DataId, "03", UserProfile.CorporationCode);
                // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） END

                // 暫定修正 2011.11.29 End   大福帳の伝票が増幅していくのを修正
                foreach (var detailRow in dsEntity.SARSlipDetailErrorDataSet.SARSlipDetailError)
                {
                    //   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 開始
                    // 削除フラグチェック(1以外:更新、1:削除)
                    if (detailRow.LogicalDelFlg != "1")
                    //   2012.01.14 関根 行削除ボタンでエラーとなる不具合対応 終了
                    {
                        // 2012.01.21 在庫管理区分の更新対応 開始
                        // （商談交渉公開クラス）[BSCSTR10000000038]スキャニング商品情報取得
                        STRProductDisclosureDataSet strProductDisclosureDataSet = new STRProductDisclosureDataSet();
                        strProductControlAPI.GetProductInfoByScannedData(
                            headerRow.CorporationCode,
                            // --> 2014/06/16 伝票登録の効率化対応 
                            //detailRow.ScanningCd,
                            string.IsNullOrWhiteSpace(detailRow["SKUCd"].ToString()) ? detailRow.ScanningCd : detailRow.SKUCd,
                            // <-- 2014/06/16 伝票登録の効率化対応 
                            headerRow.SlipDate,
                            ref strProductDisclosureDataSet);
                        // 2012.01.21 在庫管理区分の更新対応 終了

                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        STRProductDisclosureDataSet.STRProductBasicAttributeDisclosureRow strRow = null;
                        if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Rows.Count != 0)
                        {
                            strRow = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0];
                        }
                        STRProductDisclosureDataSet.STRSKUPurchasePtternDisclosureRow strPurchaseRow = null;
                        if (strProductDisclosureDataSet.STRSKUPurchasePtternDisclosure.Rows.Count != 0)
                        {
                            strPurchaseRow = strProductDisclosureDataSet.STRSKUPurchasePtternDisclosure[0];
                        }
                        // 2012.06.21 End   大福帳へセットする内容の訂正

                        var newRow = accountBookDs.AccountBook.NewAccountBookRow();
                        newRow.AccountBookSequenceNo = tempSeq++;
                        newRow.AccountBookRegistrationDate = headerRow.SlipDate;
                        newRow.SubSystem = workEntity.Subsystem;
                        newRow.UserCd = UserProfile.UserId;
                        newRow.DataID = dataID;
                        newRow.SlipDate = headerRow.SlipDate;
                        newRow.SlipTypeCd = headerRow.SlipTypeCd;
                        newRow.SlipDiv = headerRow.SlipDiv;
                        newRow.SlipNo = headerRow.SlipNo;
                        newRow.SlipLineNo = detailRow.SlipLineNo;
                        // 暫定修正 2011.11.29 Start 大福帳の伝票が増幅していくのを修正
                        //newRow.HistoryNo = 0;
                        newRow.HistoryNo = newHistoryNo;
                        // 暫定修正 2011.11.29 End   大福帳の伝票が増幅していくのを修正

                        //   2012.01.08 関根 黒伝・赤伝時の訂正区分の誤りを修正 変更 開始
                        //newRow.CorrectionDiv = headerRow.CorrectionDiv;
                        newRow.CorrectionDiv = string.Empty;
                        //   2012.01.08 関根 黒伝・赤伝時の訂正区分の誤りを修正 変更 終了

                        //   2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 修正 開始
                        //newRow.DistributionDiv = string.Empty;
                        //newRow.CommercialDistributionDiv = headerRow.CorrectionDiv;
                        // 2012.06.13 Start 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
                        //if (coverSheetMasterEntity.SlipTypeCd.Equals(disposal))
                        if (coverSheetMasterEntity.SlipTypeCd.Equals(slipTypeCd_Disposal))
                        // 2012.06.13 End   売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
                        {
                            // 2013/09/24 アドオン対応：差額は別レコードを作成する ISC:yshindo start
                            // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                            //newRow.DistributionDiv = "-";
                            //newRow.CommercialDistributionDiv = "-";
                            // 廃棄の場合
                            if (detailRow.DisposalChangeQty >= 0)
                            {
                                newRow.DistributionDiv = "-";
                                newRow.CommercialDistributionDiv = "-";
                            }
                            else
                            {
                                newRow.DistributionDiv = "+";
                                newRow.CommercialDistributionDiv = "+";
                            }
                            // 20130402 end
                            // 2013/09/24 アドオン対応：差額は別レコードを作成する ISC:yshindo end
                        }
                        else
                        {
                            newRow.DistributionDiv = "-";
                            // 2013.12.05 関根 売変時の商流区分の設定変更 START
                            //// 2012.06.21 Start 大福帳へセットする内容の訂正
                            ////newRow.CommercialDistributionDiv = "+";
                            //if ((detailRow.SlipNewPrice - detailRow.SlipOldPrice) >= 0)
                            //{
                            //    newRow.CommercialDistributionDiv = "+";
                            //}
                            //else
                            //{
                            //    newRow.CommercialDistributionDiv = "-";
                            //}
                            //// 2012.06.21 End   大福帳へセットする内容の訂正

                            if ((detailRow.SlipNewPrice - detailRow.SlipOldPrice) <= 0)
                            {
                                newRow.CommercialDistributionDiv = "+";
                            }
                            else
                            {
                                newRow.CommercialDistributionDiv = "-";
                            }
                            // 2013.12.05 関根 売変時の商流区分の設定変更 END
                        }
                        //   2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 修正 終了
                        newRow.InputBatchNo = headerRow.InputBatchNo;
                        newRow.InputMediaDiv = headerRow.InputMediaDiv;
                        newRow.DataUnit = string.Empty;
                        newRow.WeekNo = string.Empty;
                        newRow.BasedOnMonthly = string.Empty;
                        //   2011.12.06 関根 大福帳の時刻・時間帯に値を更新する 修正 開始
                        //newRow.TimeZone = string.Empty;
                        string wkTime = DateTime.Now.ToString("HHmmss");
                        newRow.TimeOfDay = wkTime;
                        newRow.TimeZone = this.GetTimeZoneNew(headerRow.CorporationCode.ToString(), headerRow.StoreCd.ToString(), wkTime);
                        //   2011.12.06 関根 大福帳の時刻・時間帯に値を更新する 修正 終了
                        newRow.CorporationCode = headerRow.CorporationCode;
                        newRow.StoreCd = headerRow.StoreCd;
                        newRow.CenterCd = string.Empty;
                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        //newRow.SupplierCd = string.Empty;
                        newRow.SupplierCd = strPurchaseRow == null ? string.Empty : strPurchaseRow.SupplierCd;
                        // 2012.06.21 End   大福帳へセットする内容の訂正
                        newRow.OfficeCd = string.Empty;
                        //   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応 変更 開始
                        //newRow.Category1Cd = headerRow.Category1Cd;
                        //newRow.Category2Cd = headerRow.Category2Cd;
                        newRow.Category1Cd = detailRow.Category1Cd;
                        newRow.Category2Cd = detailRow.Category2Cd;
                        //   2012.01.08 関根 大福帳の分類２コードが空白になる不具合対応 変更 終了
                        newRow.Category3Cd = detailRow.Category3Cd;
                        newRow.Category4Cd = detailRow.Category4Cd;
                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        //newRow.SalesCategory1Cd = string.Empty;
                        //newRow.SalesCategory2Cd = string.Empty;
                        //newRow.SalesCategory3Cd = string.Empty;
                        //newRow.AlcoholSecondaryCategoryCd = string.Empty;
                        //newRow.AlcoholPrimaryCategoryCd = string.Empty;
                        newRow.SalesCategory1Cd = strRow == null ? string.Empty : strRow.SalesCategory1Cd;
                        newRow.SalesCategory2Cd = strRow == null ? string.Empty : strRow.SalesCategory2Cd;
                        newRow.SalesCategory3Cd = strRow == null ? string.Empty : strRow.SalesCategory3Cd;
                        newRow.AlcoholSecondaryCategoryCd = strRow == null ? string.Empty : strRow.AlcoholSecondaryCategoryCd;
                        newRow.AlcoholPrimaryCategoryCd = strRow == null ? string.Empty : strRow.AlcoholPrimaryCategoryCd;
                        // 2012.06.21 End   大福帳へセットする内容の訂正
                        newRow.LendAndBorrowDiv = string.Empty;
                        newRow.SlipIssueFlg = string.Empty;
                        newRow.SKUCd = detailRow["SKUCd"].ToString();
                        newRow.ScanningCd = detailRow.ScanningCd;
                        newRow.SizeCd = string.Empty;
                        newRow.RateDiv = string.Empty;
                        newRow.ProductsCategoryDiv = string.Empty;
                        newRow.RawMaterialsDiv = string.Empty;
                        newRow.SetProductFlg = string.Empty;
                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        //newRow.TaxRate = 0;
                        using (TaxRateDisclosureDataSet taxRateDisclosureDataSet = new TaxRateDisclosureDataSet())
                        {
                            TaxRateDisclosureSearchConditionEntity taxRateDisclosureSearchConditionEntity = new TaxRateDisclosureSearchConditionEntity()
                            {
                                TaxRateCd = strRow == null ? taxDefaultRateCd : strRow.TaxCd,
                                BaseDate = headerRow.SlipDate

                            };
                            cmcTaxRateAPI.GetTaxRateDisclosure("SA", taxRateDisclosureSearchConditionEntity, taxRateDisclosureDataSet);
                            if (taxRateDisclosureDataSet.TaxRateDisclosure.Rows.Count != 0)
                            {
                                newRow.TaxRate = taxRateDisclosureDataSet.TaxRateDisclosure[0].TaxRate;
                            }
                        }
                        // 2012.06.21 End   大福帳へセットする内容の訂正
                        newRow.StoreGuestNo = 0;
                        newRow.SectionGuestNo = 0;
                        newRow.PointPrice = 0;
                        newRow.InvValuationSellPrice = 0;
                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        //newRow.TaxRateCd = string.Empty;
                        newRow.TaxRateCd = strRow == null ? taxDefaultRateCd : strRow.TaxCd;
                        // 2012.06.21 End   大福帳へセットする内容の訂正
                        newRow.OrderDiv = string.Empty;
                        newRow.OrderTypeDiv = string.Empty;
                        newRow.OrderUnitCd = string.Empty;
                        newRow.OrderUnitQty = 0;
                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        //newRow.IncludeTaxOriginUnitPrice = 0;
                        // 2012.06.21 End   大福帳へセットする内容の訂正
                        newRow.OrderMeans = string.Empty;
                        newRow.ProductsHandleDiv = string.Empty;
                        newRow.PlanCd = string.Empty;
                        newRow.BargainDiv = string.Empty;
                        newRow.TerminalNo = string.Empty;
                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        //newRow.SalesChangeDiv = string.Empty;
                        //newRow.IncludeTaxSaleUnitPrice = headerRow.SlipTotalDisposalAmt;
                        //newRow.BeforeChangesIncludeTaxSaleUnitPrice = headerRow.SlipOldTotalPrice;
                        //newRow.AfterChangesIncludeTaxSaleUnitPrice = headerRow.SlipNewTotalPrice;
                        //newRow.IncludeTaxCostAmt = detailRow.SalesChangePrice;
                        //newRow.PretaxSellPrice = detailRow.DisposalPriceAmt;
                        //newRow.ExcludeTaxSaleUnitPrice = 0;
                        //newRow.BeforeChangesExcludeTaxSaleUnitPrice = detailRow.SlipOldPrice;
                        //newRow.AfterChangesExcludeTaxSaleUnitPrice = detailRow.SlipNewPrice;
                        //newRow.ExcludeTaxCostAmt = detailRow.SlipDisposalUnitPrica;
                        //newRow.ExcludeTaxOriginUnitPrice = 0;
                        //newRow.BeforetaxSellPrice = 0;
                        //newRow.TradePoints = detailRow.DisposalChangeQty;
                        //newRow.TargetQty = headerRow.TotalQty;

                        // 8/9 SUEYOSHI START 大福帳へセットする内容の追加
                        string logicalDelDate = "9999-12-31 00:00:00.000";
                        newRow.CreateBy = UserProfile.UserId;
                        newRow.UpdateBy = UserProfile.UserId;
                        // 2013.11.26 関根 大福帳登録時に「CreateDate」「UpdateDate」には、日時をセットするように修正（現在は日付がセットされている） START
                        //newRow.CreateDate = DateTime.Now.Date;
                        //newRow.UpdateDate = DateTime.Now.Date;
                        newRow.CreateDate = DateTime.Now;
                        newRow.UpdateDate = DateTime.Now;
                        // 2013.11.26 関根 大福帳登録時に「CreateDate」「UpdateDate」には、日時をセットするように修正（現在は日付がセットされている） END
                        newRow.LogicalDelDate = Convert.ToDateTime(logicalDelDate);
                        // 8/9 SUEYOSHI END

                        // 税込計算用(端数は切上げで行う)
                        string outTaxDiv;
                        decimal outTaxRate;
                        decimal outTax;
                        decimal outPrice;
                        //decimal outPrice3;

                        if (headerRow.SlipTypeCd.Equals(slipTypeCd_Disposal))
                        {
                            // 2013/09/24 アドオン対応：差額は別レコードを作成する ISC:yshindo start
                            // 廃棄
                            // 2013.11.02 AST Chg Start
                            #region 修正前
                            //newRow.SalesChangeDiv = string.Empty;
                            #endregion
                            // 廃棄の売変区分は明細エラーの売変区分をセットする
                            newRow.SalesChangeDiv = detailRow.SalesChangeDiv;
                            // 2013.11.02 AST Chg End
                            newRow.IncludeTaxOriginUnitPrice = 0;
                            // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                            //newRow.IncludeTaxSaleUnitPrice = detailRow.SlipDisposalUnitPrica;
                            //newRow.IncludeTaxSaleUnitPrice = Math.Abs(detailRow.SlipDisposalUnitPrica);
                            // --> 2013/11/01 マスタ売価対応
                            //decimal includeTaxSaleUnitPrice = decimal.Parse(detailRow.Attribute04);
                            decimal includeTaxSaleUnitPrice = Math.Abs(detailRow.SlipDisposalUnitPrica);
                            cmcTaxRateAPI.CalcConsumptionTaxIncluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, includeTaxSaleUnitPrice, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            includeTaxSaleUnitPrice = outPrice;
                            // <-- 2013/11/01 マスタ売価対応
                            newRow.IncludeTaxSaleUnitPrice = Math.Abs(includeTaxSaleUnitPrice);
                            // 20130402 end
                            newRow.BeforeChangesIncludeTaxSaleUnitPrice = 0;
                            newRow.AfterChangesIncludeTaxSaleUnitPrice = 0;
                            // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                            //newRow.IncludeTaxCostAmt = detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice;
                            //newRow.PretaxSellPrice = detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica;
                            newRow.IncludeTaxCostAmt = Math.Abs(detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice);
                            //newRow.PretaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica);
                            newRow.PretaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * includeTaxSaleUnitPrice);
                            // 20130402 end
                            // --> 2013/11/01 マスタ売価対応
                            //// 2013.10.18 AST Chg Start
                            //#region 修正前
                            ////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipDisposalUnitPrica, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //#endregion
                            //// 税区分が非課税以外は税はがしを行う
                            //this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipDisposalUnitPrica, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //// 2013.10.18 AST Chg End

                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipDisposalUnitPrica, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            // <-- 2013/11/01 マスタ売価対応
                            newRow.ExcludeTaxOriginUnitPrice = 0;
                            // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                            //newRow.ExcludeTaxSaleUnitPrice = outPrice;
                            //newRow.ExcludeTaxSaleUnitPrice = Math.Abs(outPrice);
                            // --> 2013/11/01 マスタ売価対応
                            //newRow.ExcludeTaxSaleUnitPrice = Math.Abs(detailRow.SlipDisposalUnitPrica);
                            newRow.ExcludeTaxSaleUnitPrice = Math.Abs(outPrice);
                            // <-- 2013/11/01 マスタ売価対応
                            // 20130402 end
                            newRow.BeforeChangesExcludeTaxSaleUnitPrice = 0;
                            newRow.AfterChangesExcludeTaxSaleUnitPrice = 0;
                            // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                            //cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //newRow.ExcludeTaxCostAmt = outPrice;
                            //cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //newRow.BeforetaxSellPrice = outPrice;
                            // --> 2013/11/01 マスタ売価対応
                            //// 2013.10.18 AST Chg Start
                            //#region 修正前
                            ////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice), Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //#endregion
                            //// 税区分が非課税以外は税はがしを行う
                            //this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice), Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //// 2013.10.18 AST Chg End

                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice), fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            // <-- 2013/11/01 マスタ売価対応
                            newRow.ExcludeTaxCostAmt = Math.Abs(outPrice);
                            //cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica), Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //newRow.BeforetaxSellPrice = Math.Abs(outPrice);
                            newRow.BeforetaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica);
                            // 20130402 end
                            // 2013/09/24 アドオン対応：差額は別レコードを作成する ISC:yshindo end
                        }
                        else
                        {
                            // 売変
                            // 明細部のSalesChangeDivに画面で選択された事由のコードがセットされているのでセットする
                            newRow.SalesChangeDiv = detailRow.SalesChangeDiv;
                            newRow.IncludeTaxOriginUnitPrice = 0;
                            // --> 2013/11/01 マスタ売価対応
                            //newRow.IncludeTaxSaleUnitPrice = detailRow.SlipNewPrice;
                            //newRow.BeforeChangesIncludeTaxSaleUnitPrice = detailRow.SlipOldPrice;
                            //newRow.AfterChangesIncludeTaxSaleUnitPrice = detailRow.SlipNewPrice;
                            cmcTaxRateAPI.CalcConsumptionTaxIncluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipNewPrice, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            newRow.IncludeTaxSaleUnitPrice = outPrice;
                            newRow.AfterChangesIncludeTaxSaleUnitPrice = outPrice;
                            cmcTaxRateAPI.CalcConsumptionTaxIncluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipOldPrice, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            newRow.BeforeChangesIncludeTaxSaleUnitPrice = outPrice;
                            // <-- 2013/11/01 マスタ売価対応
                            newRow.IncludeTaxCostAmt = 0;

                            // 2014.01.06 Start 税込売価金額（PretaxSellPrice）には、税抜売価金額（BeforetaxSellPrice）と同様の値をセットする
                            //// --> 2013/11/01 マスタ売価対応
                            ////newRow.PretaxSellPrice = System.Math.Abs((detailRow.DisposalChangeQty * detailRow.SlipNewPrice) - (detailRow.DisposalChangeQty * detailRow.SlipOldPrice));
                            //newRow.PretaxSellPrice = System.Math.Abs((detailRow.DisposalChangeQty * newRow.IncludeTaxSaleUnitPrice) - (detailRow.DisposalChangeQty * newRow.BeforeChangesIncludeTaxSaleUnitPrice));
                            //// <-- 2013/11/01 マスタ売価対応
                            // 2014.01.06 End   税込売価金額（PretaxSellPrice）には、税抜売価金額（BeforetaxSellPrice）と同様の値をセットする

                            newRow.ExcludeTaxOriginUnitPrice = 0;
                            // --> 2013/11/01 マスタ売価対応
                            //// 2013.10.18 AST Chg Start
                            //#region 修正前
                            ////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipNewPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //#endregion
                            //// 税区分が非課税以外は税はがしを行う
                            //this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipNewPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //// 2013.10.18 AST Chg End
                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipNewPrice, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            // <-- 2013/11/01 マスタ売価対応
                            newRow.ExcludeTaxSaleUnitPrice = outPrice;

                            // --> 2013/11/01 マスタ売価対応
                            //// 2013.10.18 AST Chg Start
                            //#region 修正前
                            ////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipOldPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //#endregion
                            //// 税区分が非課税以外は税はがしを行う
                            //this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipOldPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //// 2013.10.18 AST Chg End
                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipOldPrice, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            // <-- 2013/11/01 マスタ売価対応

                            newRow.BeforeChangesExcludeTaxSaleUnitPrice = outPrice;
                            // --> 2013/11/01 マスタ売価対応
                            //// 2013.10.18 AST Chg Start
                            //#region 修正前
                            ////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipNewPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //#endregion
                            //// 税区分が非課税以外は税はがしを行う
                            //this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipNewPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //// 2013.10.18 AST Chg End
                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.SlipNewPrice, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            // <-- 2013/11/01 マスタ売価対応

                            newRow.AfterChangesExcludeTaxSaleUnitPrice = outPrice;
                            newRow.ExcludeTaxCostAmt = 0;
                            decimal calcOldPrice;
                            decimal calcNewPrice;
                            // --> 2013/11/01 マスタ売価対応
                            //// 2013.10.18 AST Chg Start
                            //#region 修正前
                            ////// 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                            ////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty) * detailRow.SlipOldPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out calcOldPrice);
                            ////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty) * detailRow.SlipNewPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out calcNewPrice);
                            ////// 20130402 end
                            //#endregion
                            //// 税区分が非課税以外は税はがしを行う
                            //this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty) * detailRow.SlipOldPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out calcOldPrice);
                            //this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty) * detailRow.SlipNewPrice, Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out calcNewPrice);
                            //// 2013.10.18 AST Chg End
                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty) * detailRow.SlipOldPrice, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out calcOldPrice);
                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty) * detailRow.SlipNewPrice, fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out calcNewPrice);
                            // <-- 2013/11/01 マスタ売価対応

                            newRow.BeforetaxSellPrice = System.Math.Abs(calcNewPrice - calcOldPrice);

                            // 2014.01.06 Start 税込売価金額（PretaxSellPrice）には、税抜売価金額（BeforetaxSellPrice）と同様の値をセットする
                            newRow.PretaxSellPrice = System.Math.Abs(calcNewPrice - calcOldPrice);
                            // 2014.01.06 End   税込売価金額（PretaxSellPrice）には、税抜売価金額（BeforetaxSellPrice）と同様の値をセットする
                        }
                        // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                        //newRow.TradePoints = detailRow.DisposalChangeQty;
                        //newRow.TargetQty = detailRow.DisposalChangeQty;
                        newRow.TradePoints = Math.Abs(detailRow.DisposalChangeQty);
                        newRow.TargetQty = Math.Abs(detailRow.DisposalChangeQty);
                        // 20130402 end

                        // 2012.06.21 End   大福帳へセットする内容の訂正
                        // 2012.06.21 Start 大福帳登録時に仮想キーで一意違反となるので修正
                        //newRow.DealSeqNo = string.Empty;
                        newRow.ReceiptNo = detailRow.SlipTypeCd;
                        newRow.DealSeqNo = detailRow.SlipDiv;
                        // 2012.06.21 End   大福帳登録時に仮想キーで一意違反となるので修正
                        newRow.ExecTiming = string.Empty;
                        newRow.ProductMasterUnRegisteredFlg = string.Empty;
                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        //newRow.ReturnsFlg = string.Empty;
                        //newRow.ConsignmentBuyingFlg = string.Empty;
                        newRow.ReturnsFlg = "0";
                        newRow.ConsignmentBuyingFlg = "0";
                        // 2012.06.21 End   大福帳へセットする内容の訂正
                        newRow.TenantSellFlg = string.Empty;
                        newRow.AutoSalesChangeFlg = headerRow.AutoSalesChangeFlg;
                        // 2013.11.02 AST Chg Start
                        #region 修正前
                        //// 2012.06.21 Start ＰＯＳ値引区分にＡＰ変数の値をセットしておく
                        ////newRow.POSDiscountDiv = string.Empty;
                        //newRow.POSDiscountDiv = slipPosdiscountDiv;
                        //// 2012.06.21 End   ＰＯＳ値引区分にＡＰ変数の値をセットしておく
                        #endregion
                        // 伝票区分からＰＯＳ値引区分を設定する。
                        newRow.POSDiscountDiv = this.GetPosDiscountDivNew(newRow.CorporationCode, newRow.SlipDiv);
                        // 2013.11.02 AST Chg End
                        newRow.SlipDetailDiv = string.Empty;
                        newRow.Reason = detailRow.Reason;
                        newRow.StorePlanCd = string.Empty;
                        newRow.ViewsOfTransaction = string.Empty;
                        // 2012.01.21 在庫管理区分の更新対応 開始
                        //newRow.InvControlDiv = string.Empty;
                        if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Rows.Count >= 1)
                        {
                            newRow.InvControlDiv = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].InvControlDiv;
                        }
                        else
                        {
                            newRow.InvControlDiv = string.Empty;
                        }
                        // 2012.01.21 在庫管理区分の更新対応 終了
                        newRow.LogicalDelFlg = Constants.OffFlag;

                        // 2012.06.21 Start 大福帳へセットする内容の訂正
                        newRow.AccountBookRemarks1 = typeOfTradeCd;                 // 取引種別コード
                        newRow.AccountBookRemarks2 = typeOfTradeSubCd;              // 取引種別明細コード
                        newRow.AccountBookRemarks3 = "0";                           // レジマイナスモード
                        if (headerRow.SlipTypeCd.Equals(slipTypeCd_Disposal))       // 消費税額(廃棄、売変の為)
                        {
                            // 2013/09/24 アドオン対応：差額は別レコードを作成する ISC:yshindo start
                            // 廃棄
                            // 税額計算用(端数は切捨てで行う)
                            string taxTaxDiv;
                            decimal taxTaxRate;
                            decimal taxTax;
                            decimal taxPrice;
                            // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                            //cmcTaxRateAPI.CalcConsumptionTax("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica, Constants.APRFractionDiv_Truncation, out taxTaxDiv, out taxTaxRate, out taxTax, out taxPrice);
                            // --> 2013/11/01 マスタ売価対応
                            //// 2013.10.18 AST Chg Start
                            //#region 修正前
                            ////cmcTaxRateAPI.CalcConsumptionTax("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica), Constants.APRFractionDiv_Truncation, out taxTaxDiv, out taxTaxRate, out taxTax, out taxPrice);
                            //#endregion
                            //// 税区分が非課税以外は税計算を行う
                            //this.CalcConsumptionTax("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica), Constants.APRFractionDiv_Truncation, out taxTaxDiv, out taxTaxRate, out taxTax, out taxPrice);
                            //// 2013.10.18 AST Chg End
                            cmcTaxRateAPI.CalcConsumptionTax("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * detailRow.SlipDisposalUnitPrica), Constants.APRFractionDiv_Truncation, out taxTaxDiv, out taxTaxRate, out taxTax, out taxPrice);
                            // <-- 2013/11/01 マスタ売価対応

                            // 20130402 end
                            newRow.AccountBookRemarks4 = taxTax.ToString();
                            // 2013/09/24 アドオン対応：差額は別レコードを作成する ISC:yshindo end
                        }
                        else
                        {
                            // 売変
                            newRow.AccountBookRemarks4 = "0";
                        }
                        // 2012.06.21 End   大福帳へセットする内容の訂正

                        accountBookDs.AccountBook.AddAccountBookRow(newRow);

                        // 2013/09/24 アドオン対応：廃棄伝票の場合は差額分の大福帳を作成する ISC:yshindo start

                        // 廃棄
                        if (headerRow.SlipTypeCd.Equals(slipTypeCd_Disposal))
                        {
                            // 標準売価の税抜
                            decimal taxExcludedPrice = 0m;
                            // 標準売価の税込
                            decimal preTaxSaleUnitPrice = 0m;

                            // 2013/10/03 アドオン対応：受入指摘　店舗コード指定をはずし標準売価を取得する ISC:yshindo start
                            // 商談公開クラスの税込売価算出（商品、店舗コード指定）を呼び出す(BSCSTR10001000013)
                            //new STRProductControlINAPI().CalcTaxIncludedPrice(
                            ////strProductControlAPI.CalcTaxIncludedPrice(
                            //    headerRow.SlipDate,
                            //    detailRow.SKUCd,
                            //    detailRow.StoreCd,
                            //    ref taxExcludedPrice,
                            //    ref preTaxSaleUnitPrice);
                            strProductControlAPI.CalcTaxIncludedPrice(
                                headerRow.SlipDate,
                                detailRow.SKUCd,
                                string.Empty,
                                ref taxExcludedPrice,
                                ref preTaxSaleUnitPrice);
                            // 2013/10/03 アドオン対応：受入指摘　店舗コード指定をはずし標準売価を取得する ISC:yshindo end

                            // --> 2013/11/01 マスタ売価対応
                            // taxExcludedPriceは、税抜き売価
                            #region DEL
                            //// 2013/10/03 アドオン対応：受入指摘　内税の判定方法を修正 ISC:yshindo start
                            //// 内税(20)の場合は税抜きを算出する
                            //using (TaxRateDisclosureDataSet taxRateDisclosureDataSet = new TaxRateDisclosureDataSet())
                            //{
                            //    TaxRateDisclosureSearchConditionEntity taxRateDisclosureSearchConditionEntity = new TaxRateDisclosureSearchConditionEntity()
                            //    {
                            //        TaxRateCd = strRow == null ? taxDefaultRateCd : strRow.TaxCd,
                            //        BaseDate = headerRow.SlipDate

                            //    };
                            //    cmcTaxRateAPI.GetTaxRateDisclosure("SA", taxRateDisclosureSearchConditionEntity, taxRateDisclosureDataSet);
                            //    if (taxRateDisclosureDataSet.TaxRateDisclosure.Rows.Count != 0)
                            //    {
                            //        string taxDiv = taxRateDisclosureDataSet.TaxRateDisclosure[0].TaxDiv;
                            //        if (taxDiv.Equals("20"))
                            //        {
                            //            // 2013.10.18 AST Chg Start
                            //            #region 修正前
                            //            //cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(taxExcludedPrice), Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //            #endregion
                            //            // 税区分が非課税以外は税はがしを行う
                            //            this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(taxExcludedPrice), Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            //            // 2013.10.18 AST Chg End
                            //            // 税抜き売価を設定
                            //            taxExcludedPrice = outPrice;
                            //        }
                            //    }
                            //}
                            //// 2013/10/03 アドオン対応：受入指摘　内税の判定方法を修正 ISC:yshindo end
                            #endregion
                            // <-- 2013/11/01 マスタ売価対応


                            // 差額がある場合のみ作成
                            // --> 2013/11/01 マスタ売価対応
                            //if (detailRow.SlipDisposalUnitPrica - taxExcludedPrice != 0)

                            // 標準売価税抜き
                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(preTaxSaleUnitPrice), fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            if (outTaxDiv == "20")
                            {
                                // 内税(20)の場合は税抜きを算出する
                                taxExcludedPrice = outPrice;
                            }

                            // 店別売価税抜き
                            cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.SlipDisposalUnitPrica), fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                            decimal slipDisposalUnitPricaExcluded = outPrice;
                            if (slipDisposalUnitPricaExcluded - taxExcludedPrice != 0)
                            // <-- 2013/11/01 マスタ売価対応
                            {
                                var newDiffRow = accountBookDs.AccountBook.NewAccountBookRow();
                                newDiffRow.AccountBookSequenceNo = tempSeq++;
                                newDiffRow.AccountBookRegistrationDate = headerRow.SlipDate;
                                newDiffRow.SubSystem = workEntity.Subsystem;
                                newDiffRow.UserCd = UserProfile.UserId;
                                // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） START
                                //newDiffRow.DataID = dataID;
                                newDiffRow.DataID = dataID_SalesChange;
                                // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） END
                                newDiffRow.SlipDate = headerRow.SlipDate;
                                // 2013.11.01 AST Chg Start
                                #region 修正前
                                //newDiffRow.SlipTypeCd = headerRow.SlipTypeCd;
                                //newDiffRow.SlipDiv = headerRow.SlipDiv;
                                #endregion
                                // 廃棄評価差額伝票種別
                                newDiffRow.SlipTypeCd = disposalBalanceSlipTypeCd;
                                // 2013.11.01 AST Chg End
                                newDiffRow.SlipNo = headerRow.SlipNo;
                                newDiffRow.SlipLineNo = detailRow.SlipLineNo;
                                newDiffRow.HistoryNo = newHistoryNo;
                                newDiffRow.CorrectionDiv = string.Empty;
                                newDiffRow.DistributionDiv = "-";
                                // 2013.12.05 関根 廃棄評価差額の商流区分の設定内容変更 START
                                //// 商流区分…（廃棄入力数量×廃棄入力売単価（税抜き））－（廃棄入力数量×標準売価（税抜き））でプラスなら+マイナスなら-
                                //// --> 2013/11/01 マスタ売価対応
                                ////if ((detailRow.SlipDisposalUnitPrica - taxExcludedPrice) >= 0)
                                //if ((slipDisposalUnitPricaExcluded - taxExcludedPrice) >= 0)
                                //// <-- 2013/11/01 マスタ売価対応
                                //{
                                //    newDiffRow.CommercialDistributionDiv = "+";
                                //    // 2013.11.01 AST Add Start
                                //    // 廃棄標準売変評価差額値上伝票区分
                                //    newDiffRow.SlipDiv = disposalBalanceUp;
                                //    // 2013.11.01 AST Add End
                                //}
                                //else
                                //{
                                //    newDiffRow.CommercialDistributionDiv = "-";
                                //    // 2013.11.01 AST Add Start
                                //    // 廃棄標準売変評価差額値下伝票区分
                                //    newDiffRow.SlipDiv = disposalBalanceDown;
                                //    // 2013.11.01 AST Add End
                                //}

                                // 商流区分…（廃棄入力数量×廃棄入力売単価（税抜き））－（廃棄入力数量×標準売価（税抜き））でプラスなら-マイナスなら+
                                // ※標準売価を基準に、「標準売価>=店別売価＝値下」「標準売価<店別売価＝値上」となる。
                                if ((slipDisposalUnitPricaExcluded  - taxExcludedPrice) <= 0)
                                {
                                    newDiffRow.CommercialDistributionDiv = "+";
                                    // 廃棄標準売変評価差額値下伝票区分
                                    newDiffRow.SlipDiv = disposalBalanceDown;
                                }
                                else
                                {
                                    newDiffRow.CommercialDistributionDiv = "-";
                                    // 廃棄標準売変評価差額値上伝票区分
                                    newDiffRow.SlipDiv = disposalBalanceUp;
                                }
                                // 2013.12.05 関根 廃棄評価差額の商流区分の設定内容変更 END
                                newDiffRow.InputBatchNo = headerRow.InputBatchNo;
                                newDiffRow.InputMediaDiv = headerRow.InputMediaDiv;
                                newDiffRow.DataUnit = string.Empty;
                                newDiffRow.WeekNo = string.Empty;
                                newDiffRow.BasedOnMonthly = string.Empty;
                                newDiffRow.TimeOfDay = wkTime;
                                newDiffRow.TimeZone = this.GetTimeZoneNew(headerRow.CorporationCode.ToString(), headerRow.StoreCd.ToString(), wkTime);
                                newDiffRow.CorporationCode = headerRow.CorporationCode;
                                newDiffRow.StoreCd = headerRow.StoreCd;
                                newDiffRow.CenterCd = string.Empty;
                                newDiffRow.SupplierCd = strPurchaseRow == null ? string.Empty : strPurchaseRow.SupplierCd;
                                newDiffRow.OfficeCd = string.Empty;
                                newDiffRow.Category1Cd = detailRow.Category1Cd;
                                newDiffRow.Category2Cd = detailRow.Category2Cd;
                                newDiffRow.Category3Cd = detailRow.Category3Cd;
                                newDiffRow.Category4Cd = detailRow.Category4Cd;
                                newDiffRow.SalesCategory1Cd = strRow == null ? string.Empty : strRow.SalesCategory1Cd;
                                newDiffRow.SalesCategory2Cd = strRow == null ? string.Empty : strRow.SalesCategory2Cd;
                                newDiffRow.SalesCategory3Cd = strRow == null ? string.Empty : strRow.SalesCategory3Cd;
                                newDiffRow.AlcoholSecondaryCategoryCd = strRow == null ? string.Empty : strRow.AlcoholSecondaryCategoryCd;
                                newDiffRow.AlcoholPrimaryCategoryCd = strRow == null ? string.Empty : strRow.AlcoholPrimaryCategoryCd;
                                newDiffRow.LendAndBorrowDiv = string.Empty;
                                newDiffRow.SlipIssueFlg = string.Empty;
                                newDiffRow.SKUCd = detailRow["SKUCd"].ToString();
                                newDiffRow.ScanningCd = detailRow.ScanningCd;
                                newDiffRow.SizeCd = string.Empty;
                                newDiffRow.RateDiv = string.Empty;
                                newDiffRow.ProductsCategoryDiv = string.Empty;
                                newDiffRow.RawMaterialsDiv = string.Empty;
                                newDiffRow.SetProductFlg = string.Empty;
                                using (TaxRateDisclosureDataSet taxRateDisclosureDataSet = new TaxRateDisclosureDataSet())
                                {
                                    TaxRateDisclosureSearchConditionEntity taxRateDisclosureSearchConditionEntity = new TaxRateDisclosureSearchConditionEntity()
                                    {
                                        TaxRateCd = strRow == null ? taxDefaultRateCd : strRow.TaxCd,
                                        BaseDate = headerRow.SlipDate

                                    };
                                    cmcTaxRateAPI.GetTaxRateDisclosure("SA", taxRateDisclosureSearchConditionEntity, taxRateDisclosureDataSet);
                                    if (taxRateDisclosureDataSet.TaxRateDisclosure.Rows.Count != 0)
                                    {
                                        newDiffRow.TaxRate = taxRateDisclosureDataSet.TaxRateDisclosure[0].TaxRate;
                                    }
                                }
                                newDiffRow.StoreGuestNo = 0;
                                newDiffRow.SectionGuestNo = 0;
                                newDiffRow.PointPrice = 0;
                                newDiffRow.InvValuationSellPrice = 0;
                                newDiffRow.TaxRateCd = strRow == null ? taxDefaultRateCd : strRow.TaxCd;
                                newDiffRow.OrderDiv = string.Empty;
                                newDiffRow.OrderTypeDiv = string.Empty;
                                newDiffRow.OrderUnitCd = string.Empty;
                                newDiffRow.OrderUnitQty = 0;
                                newDiffRow.OrderMeans = string.Empty;
                                newDiffRow.ProductsHandleDiv = string.Empty;
                                newDiffRow.PlanCd = string.Empty;
                                newDiffRow.BargainDiv = string.Empty;
                                newDiffRow.TerminalNo = string.Empty;
                                newDiffRow.CreateBy = UserProfile.UserId;
                                newDiffRow.UpdateBy = UserProfile.UserId;
                                // 2013.11.26 関根 大福帳登録時に「CreateDate」「UpdateDate」には、日時をセットするように修正（現在は日付がセットされている） START
                                //newDiffRow.CreateDate = DateTime.Now.Date;
                                //newDiffRow.UpdateDate = DateTime.Now.Date;
                                newDiffRow.CreateDate = DateTime.Now;
                                newDiffRow.UpdateDate = DateTime.Now;
                                // 2013.11.26 関根 大福帳登録時に「CreateDate」「UpdateDate」には、日時をセットするように修正（現在は日付がセットされている） END
                                newDiffRow.LogicalDelDate = Convert.ToDateTime(logicalDelDate);
                                // 2013.11.02 AST Chg Start
                                #region 修正前
                                //// 2013/10/03 アドオン対応：受入指摘　売変区分は空文字 ISC:yshindo start
                                //// 2013.08.22 Start 売変区分…廃棄の標準売価差額用の値下げ、値上げ区分をセット
                                ////newDiffRow.SalesChangeDiv = newDiffRow.CommercialDistributionDiv;
                                //// 2013.08.22 End 売変区分…廃棄の標準売価差額用の値下げ、値上げ区分をセット
                                //newDiffRow.SalesChangeDiv = string.Empty;
                                //// 2013/10/03 アドオン対応：受入指摘　売変区分は空文字 ISC:yshindo end
                                #endregion
                                // ＰＯＳ値引区分から売変区分を取得する
                                newDiffRow.SalesChangeDiv = this.GetSalesChangeDivNew(newDiffRow.CorporationCode, allStoreCd, newDiffRow.SlipDiv);
                                // 2013.11.02 AST Chg End
                                newDiffRow.IncludeTaxOriginUnitPrice = 0;
                                // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                                // 廃棄売単価の税込み
                                // --> 2013/11/01 マスタ売価対応
                                //decimal preTaxSlipDisposalUnitPrica = decimal.Parse(detailRow.Attribute04.Trim());
                                //newDiffRow.IncludeTaxSaleUnitPrice = Math.Abs(preTaxSlipDisposalUnitPrica);
                                cmcTaxRateAPI.CalcConsumptionTaxIncluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.SlipDisposalUnitPrica), fractionDiv, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                                decimal preTaxSlipDisposalUnitPrica = outPrice;
                                newDiffRow.IncludeTaxSaleUnitPrice = Math.Abs(outPrice);
                                // <-- 2013/11/01 マスタ売価対応
                                // 20130402 end
                                // 2013.08.22 Start 変更前税込売単価
                                // 標準売価を設定
                                newDiffRow.BeforeChangesIncludeTaxSaleUnitPrice = preTaxSaleUnitPrice;
                                // 2013.08.22 End 変更前税込売単価
                                // 2013.08.22 Start 変更後税込売単価
                                newDiffRow.AfterChangesIncludeTaxSaleUnitPrice = Math.Abs(preTaxSlipDisposalUnitPrica);
                                // 2013.08.22 End 変更後税込売単価
                                newDiffRow.IncludeTaxCostAmt = Math.Abs(detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice);
                                // 2013/10/03 アドオン対応：受入指摘　差額は税抜金額を設定する ISC:yshindo start

                                // 2014.01.06 Start 税込売価金額（PretaxSellPrice）には、税抜売価金額（BeforetaxSellPrice）と同様の値をセットする
                                //// --> 2013/11/01 マスタ売価対応
                                ////// 2013.08.28 Start 税込売価金額
                                //////newDiffRow.PretaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * (preTaxSlipDisposalUnitPrica - preTaxSaleUnitPrice));
                                ////newDiffRow.PretaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * (detailRow.SlipDisposalUnitPrica - taxExcludedPrice));
                                ////// 2013.08.28 End 税込売価金額
                                //newDiffRow.PretaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * (newDiffRow.IncludeTaxSaleUnitPrice - newDiffRow.BeforeChangesIncludeTaxSaleUnitPrice));
                                //// <-- 2013/11/01 マスタ売価対応
                                newDiffRow.PretaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * (slipDisposalUnitPricaExcluded - taxExcludedPrice));
                                // 2014.01.06 End   税込売価金額（PretaxSellPrice）には、税抜売価金額（BeforetaxSellPrice）と同様の値をセットする
                                
                                // 2013/10/03 アドオン対応：受入指摘　差額は税抜金額を設定する ISC:yshindo end
                                newDiffRow.ExcludeTaxOriginUnitPrice = 0;
                                // --> 2013/11/01 マスタ売価対応
                                //newDiffRow.ExcludeTaxSaleUnitPrice = Math.Abs(detailRow.SlipDisposalUnitPrica);
                                newDiffRow.ExcludeTaxSaleUnitPrice = Math.Abs(slipDisposalUnitPricaExcluded);
                                // <-- 2013/11/01 マスタ売価対応
                                // 2013.08.22 Start 変更前、変更後の税抜売単価を修正
                                // 標準価格は特に税抜かないので税抜き処理は入れておく
                                newDiffRow.BeforeChangesExcludeTaxSaleUnitPrice = taxExcludedPrice;
                                // --> 2013/11/01 マスタ売価対応
                                //newDiffRow.AfterChangesExcludeTaxSaleUnitPrice = Math.Abs(detailRow.SlipDisposalUnitPrica);
                                newDiffRow.AfterChangesExcludeTaxSaleUnitPrice = newDiffRow.ExcludeTaxSaleUnitPrice;
                                // <-- 2013/11/01 マスタ売価対応
                                // 2013.08.22 End 変更前、変更後の税抜売単価を修正
                                // --> 2013/11/01 マスタ売価対応
                                //// 2013.10.18 AST Chg Start
                                //#region 修正前
                                ////cmcTaxRateAPI.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice), Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                                //#endregion
                                //// 税区分が非課税以外は税はがしを行う
                                //this.CalcConsumptionTaxExcluded("SA", headerRow.SlipDate, strRow == null ? taxDefaultRateCd : strRow.TaxCd, Math.Abs(detailRow.DisposalChangeQty * newRow.IncludeTaxOriginUnitPrice), Constants.APRFractionDiv_Revaluation, out outTaxDiv, out outTaxRate, out outTax, out outPrice);
                                //// 2013.10.18 AST Chg End
                                //newDiffRow.ExcludeTaxCostAmt = Math.Abs(outPrice);
                                newDiffRow.ExcludeTaxCostAmt = Math.Abs(detailRow.DisposalChangeQty * slipDisposalUnitPricaExcluded);
                                // <-- 2013/11/01 マスタ売価対応
                                // 2013.08.22 Start 税抜売価金額を修正
                                // --> 2013/11/01 マスタ売価対応
                                //newDiffRow.BeforetaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * (detailRow.SlipDisposalUnitPrica - taxExcludedPrice));
                                newDiffRow.BeforetaxSellPrice = Math.Abs(detailRow.DisposalChangeQty * (slipDisposalUnitPricaExcluded - taxExcludedPrice));
                                // <-- 2013/11/01 マスタ売価対応
                                // 2013.08.22 Start 税抜売価金額を修正
                                newDiffRow.TradePoints = Math.Abs(detailRow.DisposalChangeQty);
                                newDiffRow.TargetQty = Math.Abs(detailRow.DisposalChangeQty);

                                // 2012.06.21 End   大福帳へセットする内容の訂正
                                // 2012.06.21 Start 大福帳登録時に仮想キーで一意違反となるので修正
                                newDiffRow.ReceiptNo = detailRow.SlipTypeCd;
                                newDiffRow.DealSeqNo = detailRow.SlipDiv;
                                // 2012.06.21 End   大福帳登録時に仮想キーで一意違反となるので修正
                                newDiffRow.ExecTiming = string.Empty;
                                newDiffRow.ProductMasterUnRegisteredFlg = string.Empty;
                                // 2012.06.21 Start 大福帳へセットする内容の訂正
                                newDiffRow.ReturnsFlg = "0";
                                newDiffRow.ConsignmentBuyingFlg = "0";
                                // 2012.06.21 End   大福帳へセットする内容の訂正
                                newDiffRow.TenantSellFlg = string.Empty;
                                newDiffRow.AutoSalesChangeFlg = headerRow.AutoSalesChangeFlg;
                                // 2013.11.02 AST Chg Start
                                #region 修正前
                                //// 2012.06.21 Start ＰＯＳ値引区分にＡＰ変数の値をセットしておく
                                //newDiffRow.POSDiscountDiv = slipPosdiscountDiv;
                                //// 2012.06.21 End   ＰＯＳ値引区分にＡＰ変数の値をセットしておく
                                #endregion
                                // 伝票区分からＰＯＳ値引区分を設定する。
                                newDiffRow.POSDiscountDiv = this.GetPosDiscountDivNew(newDiffRow.CorporationCode, newDiffRow.SlipDiv);
                                // 2013.11.02 AST Chg End
                                newDiffRow.SlipDetailDiv = string.Empty;
                                newDiffRow.Reason = detailRow.Reason;
                                newDiffRow.StorePlanCd = string.Empty;
                                newDiffRow.ViewsOfTransaction = string.Empty;
                                // 2012.01.21 在庫管理区分の更新対応 開始
                                if (strProductDisclosureDataSet.STRProductBasicAttributeDisclosure.Rows.Count >= 1)
                                {
                                    newDiffRow.InvControlDiv = strProductDisclosureDataSet.STRProductBasicAttributeDisclosure[0].InvControlDiv;
                                }
                                else
                                {
                                    newDiffRow.InvControlDiv = string.Empty;
                                }
                                // 2012.01.21 在庫管理区分の更新対応 終了
                                newDiffRow.LogicalDelFlg = Constants.OffFlag;

                                // 2012.06.21 Start 大福帳へセットする内容の訂正
                                newDiffRow.AccountBookRemarks1 = typeOfTradeCd;                 // 取引種別コード
                                newDiffRow.AccountBookRemarks2 = typeOfTradeSubCd;              // 取引種別明細コード
                                newDiffRow.AccountBookRemarks3 = "0";                           // レジマイナスモード
                                // 2013.08.22 Start 消費税を0固定に修正
                                newDiffRow.AccountBookRemarks4 = "0";
                                // 2013.08.22 End 消費税を0固定に修正

                                accountBookDs.AccountBook.AddAccountBookRow(newDiffRow);
                            }
                        }

                        // 2013/09/24 アドオン対応：廃棄伝票の場合は差額分の大福帳を作成する ISC:yshindo end

                        // 2012.01.21 在庫管理区分の更新対応 開始
                        strProductDisclosureDataSet.Dispose();
                        // 2012.01.21 在庫管理区分の更新対応 終了
                    }
                }
                accountBookDs.AcceptChanges();
                return accountBookDs;
            }
        }

        /// <summary>
        /// 大福帳赤伝データセットを作成する
        /// </summary>
        /// <param name= "redSlipDataset">redSlipDataset</param>
        /// <param name="coverSheetMasterEntity">カバーシートマスタワークEntity</param>
        /// <returns>AccountBookDataSet</returns>
        // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 開始
        //private AccountBookDataSet GenerateRedAccountBookDataSet(AccountBookDataSet redSlipDataset)
        private AccountBookDataSet GenerateRedAccountBookDataSetNew(AccountBookDataSet redSlipDataset, CoverSheetMasterEntity coverSheetMasterEntity)
        // 2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 追加 終了
        {
            // 暫定修正 2011.11.29 Start 大福帳の伝票が増幅していくのを修正
            decimal newHistoryNo = 1;
            if (redSlipDataset.AccountBook != null && redSlipDataset.AccountBook.Rows.Count != 0)
            {
                redSlipDataset.AccountBook.DefaultView.Sort = redSlipDataset.AccountBook.HistoryNoColumn.ColumnName + " desc";
                if (redSlipDataset.AccountBook.DefaultView[0][redSlipDataset.AccountBook.HistoryNoColumn.ColumnName] != null
                    && redSlipDataset.AccountBook.DefaultView[0][redSlipDataset.AccountBook.HistoryNoColumn.ColumnName] != System.DBNull.Value
                    && String.IsNullOrEmpty(redSlipDataset.AccountBook.DefaultView[0][redSlipDataset.AccountBook.HistoryNoColumn.ColumnName].ToString()) == false)
                {
                    newHistoryNo = (decimal)redSlipDataset.AccountBook.DefaultView[0][redSlipDataset.AccountBook.HistoryNoColumn.ColumnName];
                    // 1加算する    
                    newHistoryNo += 1;
                }
            }
            // 暫定修正 2011.11.29 End   大福帳の伝票が増幅していくのを修正

            // 2012.06.13 Start 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
            var slipTypeCd_Disposal = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_DISPOSAL_SLIPTYPECD, UserProfile.CorporationCode);
            // 2012.06.13 End   売変伝票区分、廃棄伝票区分をAP変数より取得するように変更

            foreach (var row in redSlipDataset.AccountBook)
            {
                //   2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 修正 開始
                //// 暫定修正 2011.11.29 Start 大福帳の伝票が増幅していくのを修正
                ////row.CommercialDistributionDiv = "－";
                //row.CommercialDistributionDiv = "-";
                //row.HistoryNo = newHistoryNo;
                //// 暫定修正 2011.11.29 End   大福帳の伝票が増幅していくのを修正

                //   2012.01.08 関根 黒伝・赤伝時の訂正区分の誤りを修正 変更 開始
                row.CorrectionDiv = "-";
                //   2012.01.08 関根 黒伝・赤伝時の訂正区分の誤りを修正 変更 終了

                // 2012.06.13 Start 売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
                //if (coverSheetMasterEntity.SlipTypeCd.Equals(disposal))
                if (coverSheetMasterEntity.SlipTypeCd.Equals(slipTypeCd_Disposal))
                // 2012.06.13 End   売変伝票区分、廃棄伝票区分をAP変数より取得するように変更
                {
                    // 20130402 Sueyoshi 廃棄伝票マイナス入力対応
                    // 廃棄の場合
                    //row.DistributionDiv = "+";
                    //row.CommercialDistributionDiv = "+";
                    // 商流区分がマイナスで赤伝を起こす場合は+をセット
                    // DistributionDivの型がnchar(2)の為、空白を取り除く
                    if (row.DistributionDiv.Trim() == "-")
                    {
                        row.DistributionDiv = "+";
                    }
                    else
                    {
                        row.DistributionDiv = "-";
                    }
                    // 物流区分がマイナスで赤伝を起こす場合は+をセット
                    if (row.CommercialDistributionDiv == "-")
                    {
                        row.CommercialDistributionDiv = "+";
                    }
                    else
                    {
                        row.CommercialDistributionDiv = "-";
                    }
                    // 20130402 end
                    row.HistoryNo = newHistoryNo;
                }
                else
                {
                    // 2013.12.06 関根 売変訂正時には、商流区分を反転させる START
                    //row.DistributionDiv = "+";
                    //row.CommercialDistributionDiv = "-";

                    // 商流区分がマイナスで赤伝を起こす場合は+をセット
                    // DistributionDivの型がnchar(2)の為、空白を取り除く
                    if (row.DistributionDiv.Trim() == "-")
                    {
                        row.DistributionDiv = "+";
                    }
                    else
                    {
                        row.DistributionDiv = "-";
                    }
                    // 物流区分がマイナスで赤伝を起こす場合は+をセット
                    if (row.CommercialDistributionDiv == "-")
                    {
                        row.CommercialDistributionDiv = "+";
                    }
                    else
                    {
                        row.CommercialDistributionDiv = "-";
                    }
                    // 2013.12.06 関根 売変訂正時には、商流区分を反転させる END
                    row.HistoryNo = newHistoryNo;
                }
                //   2011.12.06 関根 大福帳の物流区分・商流区分に'+'or'-'を更新 修正 終了
            }

            redSlipDataset.AcceptChanges();

            return redSlipDataset;
        }

        /// <summary>
        /// Switch Error Check methods
        /// </summary>
        /// <param name="businessRuleNo">ビジネスルール番号</param>
        /// <param name= "workEntity">ワークエンティティ</param>
        /// <param name= "dsEntity">DsEntity</param>
        private void SwitchErrorCheckNew(int businessRuleNo, WorkEntity workEntity, DsEntity dsEntity)
        {
            switch (businessRuleNo)
            {
                case (int)ErrIDEnum.ERRCSARB0203:
                    this.ChkItemCodeNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0204:
                    this.ChkQtyZeroNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0205_104:
                    this.ChkPriceZeroNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0205_105:
                    this.ChkPriceUpNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0205_106:
                    this.ChkPriceDownNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0206:
                    this.ChkMustNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0194:
                    this.ChkDetailErrorNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0195:
                    this.ChkMustStoreCodeNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0196:
                    this.ChkMustCategory1CdNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0197:
                    this.ChkStoreCodeNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0198:
                    this.ChkCategory1CdNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.MSGCSARI0016:
                    this.ChkSlipDateNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0199:
                    this.ChkSlipDateContextNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0200:
                    this.ChkSellPriceNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0201:
                    this.ChkCheckDigitNew(workEntity, dsEntity);
                    break;
                case (int)ErrIDEnum.ERRCSARB0202:
                    this.ChkSlipNoSopeNew(workEntity, dsEntity);
                    break;
            }
        }

        /// <summary>
        /// ＣＭＣその他公開APIインスタンスの取得
        /// </summary>
        /// <returns>ＣＭＣその他公開APIインスタンス</returns>
        private ICmcOtherAPI GetCmcOtherAPIInstanceNew()
        {
            return (ICmcOtherAPI)ClassObjectUtil.GetBusinessInstance(Constants.CmcOtherAPIBusinessClassId, typeof(ICmcOtherAPI));
        }

        /// <summary>
        /// ＣＭＲ店舗公開APIインスタンスの取得
        /// </summary>
        /// <returns>ＣＭＲ店舗公開APIインスタンス</returns>
        private ICmrStoreAPI GetCmrStoreAPIInstanceNew()
        {
            return (ICmrStoreAPI)ClassObjectUtil.GetBusinessInstance(Constants.CmrStoreAPIBusinessClassId, typeof(ICmrStoreAPI));
        }

        /// <summary>
        /// ＣＭＣ店舗公開APIインスタンスの取得
        /// </summary>
        /// <returns>ＣＭＣ店舗公開APIインスタンス</returns>
        private ICmcStoreAPI GetCmcStoreAPIInstanceNew()
        {
            return (ICmcStoreAPI)ClassObjectUtil.GetBusinessInstance(Constants.CmcStoreAPIBusinessClassId, typeof(ICmcStoreAPI));
        }

        /// <summary>
        /// ByrSlipNoPublicBusinessインスタンスの取得
        /// </summary>
        /// <returns>ByrSlipNoPublicBusinessインスタンス</returns>
        private IByrSlipNoPublicBusiness GetByrSlipNoPublicBusinessNew()
        {
            string byrSlipNoPubilcBusinessClassId = "BSCBYR08001000";
            return (IByrSlipNoPublicBusiness)ClassObjectUtil.GetBusinessInstance(byrSlipNoPubilcBusinessClassId, typeof(IByrSlipNoPublicBusiness));
        }

        /// <summary>
        /// 共通API(商談交渉)公開APIインスタンスの取得
        /// </summary>
        /// <returns>共通API(商談交渉)公開APIインスタンス</returns>
        private ISTRProductControlAPI GetSTRProductControlAPIInstanceNew()
        {
            string strProductControlAPIClassId = "BSCSTR10000000";
            return (ISTRProductControlAPI)ClassObjectUtil.GetBusinessInstance(strProductControlAPIClassId, typeof(ISTRProductControlAPI));
        }

        /// <summary>
        /// 物理公開API(在庫)インスタンスの取得
        /// </summary>
        /// <returns>物理公開API(在庫)インスタンス</returns>
        private IIVCStockControlAPI GetIVCStockControlAPIInstanceNew()
        {
            string ivcStockControlAPIClassId = "BSCIVC01901";
            return (IIVCStockControlAPI)ClassObjectUtil.GetBusinessInstance(ivcStockControlAPIClassId, typeof(IIVCStockControlAPI));
        }

        /// <summary>
        /// 物理公開API(在庫)インスタンスの取得
        /// </summary>
        /// <returns>物理公開API(在庫)インスタンス</returns>
        private IIVRStockControlAPI GetIVRStockControlAPIInstanceNew()
        {
            string ivrStockControlAPIClassId = "BSCIVR01901";
            return (IIVRStockControlAPI)ClassObjectUtil.GetBusinessInstance(ivrStockControlAPIClassId, typeof(IIVRStockControlAPI));
        }

        /// <summary>
        /// ＣＭＲ分類公開APIインスタンスの取得
        /// </summary>
        /// <returns>ＣＭＲ分類公開APIインスタンス</returns>
        private ICmrCategoryAPI GetCmrCategoryAPIInstanceNew()
        {
            string cmrCategoryAPIClassId = "BSCCMR01003000";
            return (ICmrCategoryAPI)ClassObjectUtil.GetBusinessInstance(cmrCategoryAPIClassId, typeof(ICmrCategoryAPI));
        }

        /// <summary>
        /// 特売企画商品公開情報を取得するインターフェースインスタンスの取得
        /// </summary>
        /// <returns>特売企画商品公開情報を取得するインスタンス</returns>
        private IBargainPlanningProductPublicInformationBusiness GetBargainPlanningProductPublicInformationBusinessNew()
        {
            string bargainPlanningProductPublicInformationBusinessId = "BSCSPR09004000";
            return (IBargainPlanningProductPublicInformationBusiness)ClassObjectUtil.GetBusinessInstance(bargainPlanningProductPublicInformationBusinessId, typeof(IBargainPlanningProductPublicInformationBusiness));
        }

        /// <summary>
        /// オンライン仕入計上クラスインスタンスの取得
        /// </summary>
        /// <returns>オンライン仕入計上クラス</returns>
        private IPurchaseAccountingBusiness GetPurchaseAccountingBusinessInstanceNew()
        {
            string purchaseAccountingBusinessClassId = "BSCBYR07022";
            return (IPurchaseAccountingBusiness)ClassObjectUtil.GetBusinessInstance(purchaseAccountingBusinessClassId, typeof(IPurchaseAccountingBusiness));
        }

        /// <summary>
        /// ByrPurchaseSlipPublicBusinessインスタンスの取得
        /// </summary>
        /// <returns>ByrPurchaseSlipPublicBusinessインスタンス</returns>
        private IByrPurchaseSlipPublicBusiness GetByrPurchaseSlipPublicBusinessNew()
        {
            string byrPurchaseSlipPublicBusinessClassId = "BSCBYR08003";
            return (IByrPurchaseSlipPublicBusiness)ClassObjectUtil.GetBusinessInstance(byrPurchaseSlipPublicBusinessClassId, typeof(IByrPurchaseSlipPublicBusiness));
        }

        /// <summary>
        /// 大福帳コンポーネントインスタンスの取得
        /// </summary>
        /// <returns>大福帳コンポーネントインスタンス</returns>
        private ICmcAccountBookComponent GetCmcAccountBookComponentNew()
        {
            string cmcAccountBookClassID = "BSCCMC02001000";
            return (ICmcAccountBookComponent)ClassObjectUtil.GetBusinessInstance(cmcAccountBookClassID, typeof(ICmcAccountBookComponent));
        }

        //   2011.12.06 関根 大福帳の時刻・時間帯に値を更新する 修正 開始
        /// <summary>
        /// 営業時間帯の取得
        /// </summary>
        /// <param name="corporationCode">法人コード</param>
        /// <param name="storeCd">店舗コード</param>
        /// <param name= "workTime">対象時刻</param>
        /// <returns>営業時間帯</returns>
        private string GetTimeZoneNew(
           string corporationCode,
           string storeCd,
           string workTime)
        {
            string outBusinessPeriod = string.Empty;

            if (workTime.Length <= 0)
            {
                // 対象時刻が未設定の場合は、現在時刻を返す。
                return DateTime.Now.ToString("hh00");
            }

            // 伝票データ（黒伝）の更新
            using (SalesChangeDisposalSlipEntryDataAccessComponent dataAccessComponent = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                outBusinessPeriod = dataAccessComponent.SelectBusinessPeriodNew(corporationCode, storeCd, workTime);
            }

            return outBusinessPeriod;
        }
        //   2011.12.06 関根 大福帳の時刻・時間帯に値を更新する 修正 終了

        // --> 2013/11/01 マスタ売価対応
        #region DEL
        //// 2013.10.18 AST Add Start
        //// 税加算、税はがし処理を追加
        ///// <summary>
        ///// 消費税計算処理
        ///// </summary>
        ///// <param name="orgSubSystemId">サブシステムID</param>
        ///// <param name="baseDate">基準日</param>
        ///// <param name="taxRateCd">税率コード</param>
        ///// <param name="price">税抜金額</param>
        ///// <param name="fractionDiv">端数区分(1:四捨五入、2:切捨、3:切上)</param>
        ///// <param name="taxDiv">税区分</param>
        ///// <param name="taxRate">税率</param>
        ///// <param name="tax">税額</param>
        ///// <param name="calcTaxPrice">税込金額</param>
        //private void CalcConsumptionTax(string orgSubSystemId, DateTime baseDate, string taxRateCd, decimal price, string fractionDiv, out string taxDiv, out decimal taxRate, out decimal tax, out decimal calcTaxPrice)
        //{
        //    // ビジネスインターフェース取得
        //    ICmcTaxRateBusiness business = (ICmcTaxRateBusiness)ClassObjectUtil.GetBusinessInstance("BSCCMC01017000", typeof(ICmcTaxRateBusiness));

        //    using (var dataSet = new TaxRateDisclosureDataSet())
        //    {
        //        // 税率の取得ビジネスを呼び出す。
        //        business.GetTaxRateData(
        //            new TaxRateDisclosureSearchConditionEntity
        //            {
        //                BaseDate = baseDate,
        //                TaxRateCd = taxRateCd,
        //            },
        //            dataSet);

        //        var taxRateDisclosureRow = dataSet.TaxRateDisclosure.First();

        //        taxDiv = taxRateDisclosureRow.TaxDiv;
        //        taxRate = taxRateDisclosureRow.TaxRate;
        //    }

        //    // データベース上の税区分は NCHAR(2)型 で空白が含まれており、そのまま渡される可能性があるのでトリミング
        //    taxDiv = taxDiv != null ? taxDiv.Trim() : null;

        //    calcTaxPrice = price;
        //    tax = 0;

        //    // 計算を実施し戻り値をセット
        //    if (taxDiv == "10" || taxDiv == "20")
        //    {
        //        // 引数．税区分＝「１：外税」、「２：内税」の場合
        //        tax = price * taxRate / 100;

        //        if (fractionDiv == "1")
        //        {
        //            tax = Math.Round(tax, MidpointRounding.AwayFromZero);
        //        }
        //        else if (fractionDiv == "2")
        //        {
        //            tax = Math.Floor(tax);
        //        }
        //        else if (fractionDiv == "3")
        //        {
        //            tax = Math.Ceiling(tax);
        //        }
        //        else
        //        {
        //            throw new Exception(string.Format("端数区分の指定が不正です({0})", fractionDiv));
        //        }

        //        calcTaxPrice = price + tax;
        //    }
        //    else if (taxDiv == "30")
        //    {
        //        // ③税区分＝「３：非課税」の場合、
        //        // 戻り値．税額＝０
        //        //・金額計算
        //        // 戻り値．金額＝引数．対象金額
        //        tax = 0;
        //        calcTaxPrice = price;
        //    }
        //}

        ///// <summary>
        ///// 税はがし処理
        ///// </summary>
        ///// <param name="orgSubSystemId">サブシステムID</param>
        ///// <param name="baseDate">基準日</param>
        ///// <param name="taxRateCd">税率コード</param>
        ///// <param name="price">税込金額</param>
        ///// <param name="fractionDiv">端数区分(1:四捨五入、2:切捨、3:切上)</param>
        ///// <param name="taxDiv">税区分</param>
        ///// <param name="taxRate">税率</param>
        ///// <param name="tax">税額</param>
        ///// <param name="preTaxPrice">税抜金額</param>
        //private void CalcConsumptionTaxExcluded(string orgSubSystemId, DateTime baseDate, string taxRateCd, decimal price, string fractionDiv, out string taxDiv, out decimal taxRate, out decimal tax, out decimal preTaxPrice)
        //{
        //    // ビジネスインターフェース取得
        //    ICmcTaxRateBusiness business = (ICmcTaxRateBusiness)ClassObjectUtil.GetBusinessInstance("BSCCMC01017000", typeof(ICmcTaxRateBusiness));

        //    using (var dataSet = new TaxRateDisclosureDataSet())
        //    {
        //        // 税率の取得ビジネスを呼び出す。
        //        business.GetTaxRateData(
        //            new TaxRateDisclosureSearchConditionEntity
        //            {
        //                BaseDate = baseDate,
        //                TaxRateCd = taxRateCd,
        //            },
        //            dataSet);

        //        var taxRateDisclosureRow = dataSet.TaxRateDisclosure.First();

        //        taxDiv = taxRateDisclosureRow.TaxDiv;
        //        taxRate = taxRateDisclosureRow.TaxRate;
        //    }

        //    // データベース上の税区分は NCHAR(2)型 で空白が含まれており、そのまま渡される可能性があるのでトリミング
        //    taxDiv = taxDiv != null ? taxDiv.Trim() : null;

        //    preTaxPrice = price;
        //    tax = 0;

        //    // 計算を実施し戻り値をセット
        //    if (taxDiv == "10" || taxDiv == "20")
        //    {
        //        // 引数．税区分＝「１：外税」、「２：内税」の場合
        //        tax = price * taxRate / (taxRate + 100);

        //        if (fractionDiv == "1")
        //        {
        //            tax = Math.Round(tax, MidpointRounding.AwayFromZero);
        //        }
        //        else if (fractionDiv == "2")
        //        {
        //            tax = Math.Floor(tax);
        //        }
        //        else if (fractionDiv == "3")
        //        {
        //            tax = Math.Ceiling(tax);
        //        }
        //        else
        //        {
        //            throw new Exception(string.Format("端数区分の指定が不正です({0})", fractionDiv));
        //        }

        //        preTaxPrice = price - tax;
        //    }
        //    else if (taxDiv == "30")
        //    {
        //        // ③税区分＝「３：非課税」の場合、
        //        // 戻り値．税額＝０
        //        //・金額計算
        //        // 戻り値．金額＝引数．対象金額
        //        tax = 0;
        //        preTaxPrice = price;
        //    }
        //}
        //// 2013.10.18 AST Add End
        #endregion
        // <-- 2013/11/01 マスタ売価対応

        // 2013.11.02 AST Add Start
        /// <summary>
        /// 売変区分を取得する
        /// </summary>
        /// <param name="corporationCode">法人コード</param>
        /// <param name="storeCd">店舗コード</param>
        /// <param name="slipDiv">伝票区分</param>
        /// <returns>売変区分</returns>
        private string GetSalesChangeDivNew(string corporationCode, string storeCd, string slipDiv)
        {
            string salesChangeDiv = string.Empty;
            string posDiscountDiv = string.Empty;

            // ＰＯＳ値引区分の取得
            posDiscountDiv = this.GetPosDiscountDivNew(corporationCode, slipDiv);

            if (string.IsNullOrEmpty(posDiscountDiv))
            {
                return string.Empty;
            }

            // ＰＯＳ値引区分から売変区分の取得
            using (SalesChangeDisposalSlipEntryDataAccessComponent component = new SalesChangeDisposalSlipEntryDataAccessComponent())
            {
                //20151002 start バグ対応
                //salesChangeDiv = component.GetSalesChangeDiv(corporationCode, storeCd, this.posDiscountDivDictionary[slipDiv]);
                salesChangeDiv = component.GetSalesChangeDivNew(corporationCode, storeCd, posDiscountDiv);
                //20151002 end バグ対応

            }

            return salesChangeDiv;
        }

        /// <summary>
        /// ＰＯＳ値引区分を取得する
        /// </summary>
        /// <param name="corporationCode">法人コード</param>
        /// <param name="slipDiv">伝票区分</param>
        /// <returns>ＰＯＳ値引区分</returns>
        private string GetPosDiscountDivNew(string corporationCode, string slipDiv)
        {
            if (this.posDiscountDivDictionary == null)
            {
                // ＰＯＳ値引区分ディクショナリを取得
                this.posDiscountDivDictionary = APVariableUtil.GetAPVariableDetailedList(APVariableDetailedListKey.SAR_POS_DISCOUNT_DIV, corporationCode);
            }

            // 伝票種別が存在しなければ処理を抜ける
            if (!this.posDiscountDivDictionary.ContainsKey(slipDiv))
            {
                var slipPosdiscountDiv = APVariableUtil.GetAPVariableValue(APVariableListKey.SAR_SLIP_POSDISCOUNT_DIV, UserProfile.CorporationCode);
                return slipPosdiscountDiv;
            }

            return this.posDiscountDivDictionary[slipDiv];
        }
        // 2013.11.02 AST Add End


        // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） START
        /// <summary>
        /// DsEntityコピー処理
        /// </summary>
        /// <param name="accountBookDataSet">マスターAccountBookDataSet</param>
        /// <param name="copyAccountBookDataSet">コピーAccountBookDataSet</param>
        /// <param name="dataID">データＩＤ</param>
        private void CopyAccountBookDataSetNew(AccountBookDataSet accountBookDataSet, AccountBookDataSet copyAccountBookDataSet, string dataID)
        {
            // 明細のコピー
            foreach (var masteraccountbookRow in accountBookDataSet.AccountBook.Where(r => r.DataID == dataID))
            {
                var accountbookRow = copyAccountBookDataSet.AccountBook.NewAccountBookRow();

                accountbookRow.ItemArray = masteraccountbookRow.ItemArray;

                copyAccountBookDataSet.AccountBook.AddAccountBookRow(accountbookRow);
            }
        }
        // 2013.12.04 関根 廃棄伝票時の「標準売価＜＞店別売価」の売変伝票作成の考慮不足対応（訂正時赤伝を作成） END

        #endregion
    }
}
