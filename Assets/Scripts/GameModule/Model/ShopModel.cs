using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;

public class ShopModel : BaseModel<ShopModel>
{
    public List<ShopItemVo> PayItems;
    public List<ShopItemVo> OtherItems;
    public ShopModel() : base()
    {
        NetWorkHandler.GetDispatch().AddListener<P10_Response, P10_Request>(GameEvent.RECIEVE_P10_RESPONSE, OnRecvShopItem);
        NetWorkHandler.GetDispatch().AddListener<P11_Response, P11_Request, ShopItemVo>(GameEvent.RECIEVE_P11_RESPONSE, OnRecvItemBuyNormal);
    }

    private void OnRecvShopItem(P10_Response response, P10_Request request)
    {
        var vos = ShopItemVo.FromList(response.ProductList);
        
        // 删除无法抽选的商品
        for (int i = vos.Count - 1; i >= 0; --i)
        {
            if (vos[i].pbItems.LimitDetail != null && vos[i].pbItems.LimitDetail.LimitAmount > 0 &&  vos[i].pbItems.LimitDetail.LimitedRemainingAmount == 0)
            {
                vos.RemoveAt(i);
            }
        }
        // 排序
        vos.Sort((a,b)=> { return a.Priority - b.Priority; });


        if (request.ProductType == ShopType.Pay)
        {
            LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
            PayItems = vos;
            BillingManager.GetProductList(vos, (dic)=>{
                foreach (var dicItem in dic)
                {
                    Debug.Log(dicItem.Key);
                    ShopItemVo vo = PayItems.Find(t => t.PlatformID == dicItem.Key);
                    string[] priceList = dicItem.Value.Split('|');
                    vo.BillingPrice = priceList[0];
                    vo.BillingFormatPrice = priceList[1];
                    vo.IsVaildItem = true;
                }
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                Dispatch(ShopEvent.ON_GETTED_SHOP_LIST, request.ProductType);
            }, (err)=>{
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                MsgBox.Open("错误", "商品列表读取错误");
            });
        }
        else
        {
            OtherItems = vos;
            Dispatch(ShopEvent.ON_GETTED_SHOP_LIST, request.ProductType);
        }
    }

    public void CacheGainedItem(IList<ProductContent> items, string platformId)
    {
        var rewardVO = RewardMapVo.From(items);
        dicReward[platformId] = rewardVO;
    }

    /// <summary>
    /// 显示获取的道具
    /// </summary>
    /// <param name="platformId"></param>
    public void ShowGainedItem(string platformId)
    {
        var payItem = PayItems.Find(item=> item.pbItems.PlatformProductId == platformId);
        var rewardVO = dicReward[platformId];
        if (payItem == null || rewardVO == null)
        {
            Debug.LogError("No gained item founded!!");
            return;
        }
        dicReward.Remove(platformId);

        payItem.UpdateBuyNum(1);
        
        //update player assets
        PlayerModel.Instance.UpdateAssets(payItem, rewardVO);
        Dispatch(ShopEvent.ON_GETTED_ITEM, rewardVO);
    }

    private Dictionary<string, RewardMapVo> dicReward = new Dictionary<string, RewardMapVo>();

    private void OnRecvItemBuyNormal(P11_Response response, P11_Request request, ShopItemVo reqItem)
    {
        if (response.Result.Code == NetworkConst.CODE_OK)
        {
            var clientItem = OtherItems.Find(item=> item.pbItems.Id == request.ShopItemId);
            clientItem.UpdateBuyNum(request.ShopItemNum);

            var rewardVO = RewardMapVo.From(response);

            //update player assets
            PlayerModel.Instance.UpdateAssets(reqItem, rewardVO);
            Dispatch(ShopEvent.ON_GETTED_ITEM, rewardVO);

            FirebaseAnalytics.LogEvent(
              FirebaseAnalytics.EventSelectContent,
              new Parameter( FirebaseAnalytics.ParameterContentType, 0),
              new Parameter(FirebaseAnalytics.ParameterItemId, request.ShopItemId));
        }
        else
        {
            MsgBox.OpenTips(response.Result.Desc);
        }
    }

    public void RequestShopItem(ShopType sType, bool forceRequest = false)
    {
        if (forceRequest)
        {
            NetWorkHandler.RequestGetShopItem(sType);
        }
        else
        {
            Dispatch(ShopEvent.ON_GETTED_SHOP_LIST, sType);
        }
    }

    public List<ShopItemVo> GetShopItemByType(ShopType type)
    {
        if (type == ShopType.Pay)
        {
            return PayItems.FindAll(t=>t.IsVaildItem);
        }
        else
        {
            return OtherItems;
        }
    }

    public void BuyItemNormal(ShopItemVo vo, int num = 1)
    {
        if (vo.Paytype == PayType.Gold && PlayerModel.Instance.player.Gold >= vo.Price ||
            vo.Paytype == PayType.Diamond && PlayerModel.Instance.player.Diamond >= vo.Price
        )
        {
            NetWorkHandler.RequestBuyNormal(vo, num);
        }
        else if (vo.Paytype == PayType.Money)
        {
            LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
            BillingManager.Purchase(vo.PlatformID, (id, purchaseType)=>{
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                if (purchaseType == Jackpot.Billing.PurchaseSuccessTypes.Normal)
                {
                    ShopModel.Instance.ShowGainedItem(id);
                }
                else if (purchaseType == Jackpot.Billing.PurchaseSuccessTypes.Pending)
                {
                    MsgBox.OpenTips("PAY_STATUE_WAITTNG");
                }
                else
                {
                    MsgBox.OpenTips("PAY_STATUE_FAILED");
                }
            }, (err)=>{
                MsgBox.OpenTips(BillingManager.GetErrorWord(err));
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
            });
        }
        else
        {
            MsgBox.OpenTips("NOT_ENOUGH_RESOURCE");
        }
    }

    public override void Dispose()
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P10_RESPONSE);
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P11_RESPONSE);
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P13_RESPONSE);

        base.Dispose();
    }
}