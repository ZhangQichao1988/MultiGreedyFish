using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
        
        if (request.ProductType == ShopType.Pay)
        {
            PayItems = vos;
            BillingManager.GetProductList(vos, (dic)=>{
                foreach (var dicItem in dic)
                {
                    Debug.Log(dicItem.Key);
                    ShopItemVo vo = PayItems.Find(t => t.PlatformID == dicItem.Key);
                    string[] priceList = dicItem.Value.Split('|');
                    vo.BillingPrice = priceList[0];
                    vo.BillingFormatPrice = priceList[1];
                }
                Dispatch(ShopEvent.ON_GETTED_SHOP_LIST, request.ProductType);
            }, (err)=>{
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
            return PayItems;
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
            BillingManager.Purchase(vo.PlatformID, (id)=>{
                ShopModel.Instance.ShowGainedItem(id);
            }, (err)=>{
                MsgBox.OpenTips(BillingManager.GetErrorWord(err));
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