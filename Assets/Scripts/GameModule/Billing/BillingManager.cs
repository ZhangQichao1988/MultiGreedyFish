using System;
using System.Collections.Generic;
using System.Linq;
using Jackpot;
using Jackpot.Billing;

/// <summary>
/// 氪金处理
/// </summary>
public static class BillingManager
{
    public static bool IsBillingRequested = false;

    #region field

    /// <summary>
    /// Purchaseの場合false, Resumeの場合true
    /// </summary>
    static bool resumeFlag = false;

    /// <summary>resume時のレスポンスコード</summary>
    static PurchasedResponseKind? purchasedResponseKind;

    /// <summary>ストアから返ってきた有効なProductリスト</summary>
    static Dictionary<string, Product> productDict = new Dictionary<string, Product>();

    /// <summary>Resume完了時に時に呼ばれるコールバック</summary>
    static Action onResumeFinish;

    #endregion

    #region public methods

    /// <summary>
    /// Initialize the specified prePurchase, applePurchase, googlePurchase and refreshReceipt.
    /// </summary>
    public static void Initialize()
    {
        if (!BillingService.Instance.IsInitialized)
        {
            BillingService.Instance.Initialize(
                PrePurchaseRequest,
                PurchasedRequest,
                SuccessPurchase,
                FailurePurchase,
                RefreshReceipt,
                null);
            onResumeFinish = null;
        }
        IsBillingRequested = false;
    }

    public static string GenerateAssetStateLog(byte[] rand)
    {
        return Platform.GenerateAssetStateLog(CryptographyUtil.GetBase64String(rand));
    }

    public static void GetResemaraDetectionIdentifier(Action<string> callback)
    {
        Jackpot.Platform.LoadResemaraDetectionIdentifier(
                resemaraDetectionIdentifier =>
                {
                    callback(resemaraDetectionIdentifier);
                });
    }

    /// <summary>
    /// 課金可能な端末状態かどうか
    /// iOS: 端末の機能制限でアプリ内課金が許可されているか
    /// Android: In-app BillingがV3かどうか
    /// </summary>
    /// <returns><c>true</c>, if billing supported was ised, <c>false</c> otherwise.</returns>
    public static bool IsBillingSupported()
    {
        return BillingService.Instance.IsBillingSupported();
    }

    /// <summary>
    /// 課金のResume処理を実施します
    /// </summary>
    /// <param name="callback">Callback.</param>
    public static void Resume(Action callback = null)
    {
        if (!IsBillingSupported())
        {
            if (callback != null)
            {
                callback();
            }

            return;
        }

        resumeFlag = true;
        purchasedResponseKind = null;
        onResumeFinish = null;

        Action<bool> onCheckResumeFinish = (isResume)=>{
            if (callback == null)
            {
                return;
            }

            if (isResume)
            {
                // 未完了の商品がある場合。callbackは購入成功/失敗のどちらかで呼ばれる
                onResumeFinish = callback;
            }
            else
            {
                // 未完了の商品がない場合は即callback呼ぶ
                callback();
            }
        };

        BillingService.Instance.Resume((products, resumeExecute)=>{
            if (products != null && products.Count > 0)
            {
                onCheckResumeFinish(true);
                resumeExecute(products);
            }
        });
    }

    /// <summary>
    /// ストアに並ぶ商品一覧を取得
    /// マスタとPFを比較し両方で有効な商品のみを取得します
    /// </summary>
    /// <param name="identiferList">プロダクトIDs.</param>
    /// <param name="onSuccess">有効な商品の商品IDと価格をDictionaryで返すcallback</param>
    /// <param name="onFailure">エラー時のcallback</param>
    public static void GetProductList(
        List<ShopItemVo> identiferList,
        Action<Dictionary<string, string>> onSuccess,
        Action<QueryError> onFailure)
    {
        List<string> idList = identiferList.Select(t=>t.PlatformID).ToList();
        new QueryContext()
            .OnQueryRequest((startQuery, failureQueryRequest) => startQuery(idList))
            .OnSuccess(
                products =>
                {
                    var res = new Dictionary<string, string>();
                    UnityEngine.Debug.Log("GetProduct len" + products.Count);
                    foreach (var p in products)
                    {
                        UnityEngine.Debug.Log("GetProduct id is " + p.Id);
                        productDict[p.Id] = p;
#if UNITY_EDITOR || BILLING_DEBUG
                        ShopItemVo itemResult = identiferList.Find(t=>t.PlatformID == p.Id);
                        p.Price = itemResult.Price.ToString();
                        p.FormattedPrice = decimal.Parse(p.Price).ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("ja-JP"));
#endif
                        UnityEngine.Debug.Log("GetProduct price is " + p.Price);
                        UnityEngine.Debug.Log("GetProduct f_price is " + p.FormattedPrice);
                        res.Add(p.Id, p.Price + "|" + p.FormattedPrice);

                    }

                    onSuccess(res);
                })
            .OnFailure(
                error =>
                {
                    onFailure(error);
                })
            .Execute();
    }


    /// <summary>
    /// 商品を購入します
    /// </summary>
    /// <param name="billingProduct">購入する商品</param>
    /// <param name="onSuccess">成功時のcallback</param>
    /// <param name="onFailure">エラー時のcallback</param>
    public static void Purchase(
        string productId,
        Action<string> onSuccess,
        Action<PurchaseError> onFailure)
    {
        if (IsBillingRequested)
        {
            // 課金リクエストフラグが有効な時は購入できない
            var error = Jackpot.Billing.PurchaseError.FailedOnPurchasedRequest(
                productId,
                "Failed purchase. purchase duplicate items.");
            onFailure(error);
            return;
        }

        // 課金リクエストフラグをtrueへ
        IsBillingRequested = true;
        resumeFlag = false;
        purchasedResponseKind = null;
        onResumeFinish = null;


        try
        {
            new PurchaseContext(productDict[productId])
            .OnPurchasedRequest(PurchasedRequest)
            .OnSuccessPurchase(
                (successProduct, purchasedProduct, suceedType) =>
                {
                    onSuccess(purchasedProduct.Id);
                    SuccessPurchase(successProduct, purchasedProduct, suceedType);
                })
            .OnRefreshReceipt(RefreshReceipt)
            .OnFailurePurchase(
                error =>
                {
                    onFailure(error);
                    FailurePurchase(error);
                })
            .Execute();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogErrorFormat("ex msg is : {0} ,ex stack: {1}", ex.Message, ex.StackTrace);
        }
        
    }

    #endregion

    #region private methods

    /// <summary>
    /// 課金前のチェックAPI呼び出し
    /// Android用のpayload発行も兼ねています
    /// </summary>
    /// <param name="product">Product.</param>
    /// <param name="startPurchase">Start purchase.</param>
    /// <param name="failurePrePurchase">Failure pre purchase.</param>
    static void PrePurchaseRequest(
        PrePurchaseProduct product, Action<PrePurchaseProduct> startPurchase, Action<Jackpot.Billing.PurchaseError> failurePrePurchase)
    {

        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P12_RESPONSE);
        NetWorkHandler.GetDispatch().AddListener<P12_Response, string>(GameEvent.RECIEVE_P12_RESPONSE, (response, pid)=>{
            if (response.Result.Code != NetworkConst.CODE_OK || response.ResultCode != PurchasedResponseKind.ProcessedSuccessfully)
            {
                failurePrePurchase(Jackpot.Billing.PurchaseError.FailedOnPrePurchase(product.Id, response.Result.Desc));
            }
            else
            {
                product.AccountId = PlayerModel.Instance.player.PlayerId.ToString();
                startPurchase(product);
            }
        });
        NetWorkHandler.RequestBillingPreBuy(product.Id);
        
    }

    static bool IsLoggedIn()
    {
        return PlayerModel.Instance.player != null;
    }

    /// <summary>
    /// レシート検証処理
    /// </summary>
    /// <param name="purchasedProduct">Purchased product.</param>
    /// <param name="finishPurchase">Finish purchase.</param>
    /// <param name="failurePurchasedRequest">Failure purchased request.</param>
    static void PurchasedRequest(
        PurchasedProduct purchasedProduct,
        Action<PurchasedProduct> finishPurchase,
        Action<Jackpot.Billing.PurchaseError> failurePurchasedRequest)
    {
        UnityEngine.Debug.Log("On Finish Purchase");
        if (!IsLoggedIn())
        {
            // ユーザーがログイン前にResumeで購入処理が走ることがあるので、未ログインの場合はエラーとしておく
            var error = Jackpot.Billing.PurchaseError.FailedOnPurchasedRequest(
                purchasedProduct.Id, "Failed purchase product. not logged in still.");
            failurePurchasedRequest(error);
            return;
        }

        // 取得した商品情報があれば、価格情報を取得しておく
        string price = string.Empty;
        string formattedPrice = string.Empty;
        Product purchaseProduct = null;
        if (productDict.TryGetValue(purchasedProduct.Id, out purchaseProduct))
        {
            price = purchaseProduct.Price;
            formattedPrice = purchaseProduct.FormattedPrice;
        }

        UnityEngine.Debug.Log("PurchasedProduct Info :" + purchasedProduct.Id );
        UnityEngine.Debug.Log("PurchasedProduct Info :" + purchasedProduct.TransactionId );
        UnityEngine.Debug.Log("PurchasedProduct Info :" + purchasedProduct.TransactionDate.ToString() );
        UnityEngine.Debug.Log("PurchasedProduct Info :" + purchasedProduct.Receipt );
        UnityEngine.Debug.Log("PurchasedProduct Info :" + price );
        UnityEngine.Debug.Log("PurchasedProduct Info :" + formattedPrice );

#if UNITY_EDITOR || BILLING_DEBUG
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P15_RESPONSE);
        NetWorkHandler.GetDispatch().AddListener<P15_Response, string>(GameEvent.RECIEVE_P15_RESPONSE, (response, pid)=>{
            if (response.Result.Code != NetworkConst.CODE_OK || response.ResultCode != PurchasedResponseKind.ProcessedSuccessfully)
            {
                Jackpot.Billing.PurchaseError error =
                            Jackpot.Billing.PurchaseError.FailedOnPurchasedRequest(
                                purchasedProduct.Id,
                                response.Result.Desc
                            );

                failurePurchasedRequest(error);
            }
            else
            {
                ShopModel.Instance.CacheGainedItem(response.Content, purchasedProduct.Id);
                finishPurchase(purchasedProduct);
            }
        });
        NetWorkHandler.RequestDebugBilling(purchasedProduct.Id);
#else
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P13_RESPONSE);
        NetWorkHandler.GetDispatch().AddListener<P13_Response, string>(GameEvent.RECIEVE_P13_RESPONSE, (response, pid)=>{
            if (response.Result.Code != NetworkConst.CODE_OK || response.ResultCode != PurchasedResponseKind.ProcessedSuccessfully)
            {
                Jackpot.Billing.PurchaseError error =
                            Jackpot.Billing.PurchaseError.FailedOnPurchasedRequest(
                                purchasedProduct.Id,
                                response.Result.Desc
                            );
                failurePurchasedRequest(error);
            }
            else
            {
                ShopModel.Instance.CacheGainedItem(response.Content, purchasedProduct.Id);
                finishPurchase(purchasedProduct);
            }
        });

        if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
        {
            NetWorkHandler.RequestBillingBuy(purchasedProduct.Receipt, purchasedProduct.TransactionId, price, 
            formattedPrice, Device.Google, purchasedProduct.Id);
        }
        else
        {
            NetWorkHandler.RequestBillingBuy(purchasedProduct.SignedData, purchasedProduct.Signature, price, 
            formattedPrice, Device.Apple, purchasedProduct.Id);
        }
#endif
                
    }

    /// <summary>
    /// 購入成功時
    /// </summary>
    /// <param name="product">Product.</param>
    static void SuccessPurchase(Product product, PurchasedProduct purchasedProduct, PurchaseSuccessTypes successType)
    {
        // 課金リクエストフラグをfalseへ
        IsBillingRequested = false;

        // Resumeの購入成功時のみダイアログを出すDelegateを呼ぶ
        if (resumeFlag)
        {

            if (onResumeFinish != null)
            {
                onResumeFinish();
                onResumeFinish = null;
            }
            
            resumeFlag = false;
        }
    }

    /// <summary>
    /// 購入失敗時
    /// </summary>
    /// <param name="error">Error.</param>
    static void FailurePurchase(Jackpot.Billing.PurchaseError error)
    {
        // 課金リクエストフラグをfalseへ
        IsBillingRequested = false;
        if (resumeFlag)
        {
            if (onResumeFinish != null)
            {
                onResumeFinish();
                onResumeFinish = null;
            }

            resumeFlag = false;
        }
    }

    /// <summary>
    /// レシート更新処理
    /// </summary>
    /// <param name="purchasedProduct">Purchased product.</param>
    /// <param name="refreshReceipt">レシート更新処理の実行</param>
    /// <param name="cancelRefreshReceipt">レシート更新処理をキャンセル</param>
    static void RefreshReceipt(PurchasedProduct purchasedProduct, Action refreshReceipt, Action cancelRefreshReceipt)
    {
#if UNITY_EDITOR
        cancelRefreshReceipt();
#else
        // レシート更新回数を判定して、最大リトライ回数を超えているとレシート更新キャンセル処理を実行
        if (purchasedProduct.RetryRefreshReceiptCount < Jackpot.Billing.Config.MaxRetryRefreshReceiptCount)
        {
            refreshReceipt();
        }
        else
        {
            cancelRefreshReceipt();
        }
#endif
    }

    public static string GetErrorWord(PurchaseError err)
    {
        string errMsg = "";
        if (err.IsBillingNotSupported)
        {
            errMsg = LanguageDataTableProxy.GetText(50001);
            //_("PageShop_not_billing_supported")
        }

        if (err.IsCancelled)
        {

            errMsg = LanguageDataTableProxy.GetText(50002);
            // _("PageShop_billing_purchase_cancel")
        }
        
        if (err.IsFailedOnPurchasedRequest)
        {   
            if (err.PurchasedResponseKind == Jackpot.Billing.PurchasedResponseKind.ReceiptOutOfDate)
            {
                errMsg =  LanguageDataTableProxy.GetText(50003);
                //errMsg = _("PageShop_billing_expired_product")
            }
            else
            {
                errMsg =  LanguageDataTableProxy.GetText(50004);
                // errMsg = _("PageShop_billing_failure_platform_connect_shop")
            }
        }

        if (errMsg == "")
        {
            errMsg = LanguageDataTableProxy.GetText(50007);

            //  取得できないときの汎用エラー
            // errMsg = _("PageShop_billing_failure_purchase_shop")
        }
        
        return errMsg;
    }


    #endregion

    public static void FinishPendingTransactions()
    {
        
#if UNITY_IOS
        var transactions = AppleStoreKit.GetPendingTransactions();
        foreach (var transaction in transactions)
        {
            UnityEngine.Debug.LogFormat("[FINSH IOS Transactions] id: {0}, product: {1}", transaction.TransactionIdentifier, transaction.ProductIdentifier);
            AppleStoreKit.FinishTransaction(transaction.TransactionIdentifier);
        }
#elif UNITY_ANDROID
        BillingService.Instance.CloseAndroidProudct();
#endif
    }
}