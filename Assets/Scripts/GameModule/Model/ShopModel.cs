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
        NetWorkHandler.GetDispatch().AddListener<P12_Response, ShopItemVo>(GameEvent.RECIEVE_P12_RESPONSE, OnRecvPrePay);
        NetWorkHandler.GetDispatch().AddListener<P13_Response, ShopItemVo>(GameEvent.RECIEVE_P13_RESPONSE, OnRecvBuyPay);
    }

    private void OnRecvShopItem(P10_Response response, P10_Request request)
    {
        var vos = ShopItemVo.FromList(response.ProductList);
        
        if (request.ProductType == ShopType.Pay)
        {
            PayItems = vos;
        }
        else
        {
            OtherItems = vos;
        }
        Dispatch(ShopEvent.ON_GETTED_SHOP_LIST, request.ProductType);
    }

    private void OnRecvPrePay(P12_Response response, ShopItemVo reqItem)
    {
        if (response.Result.Code != NetworkConst.CODE_OK || response.ResultCode != PurchasedResponseKind.ProcessedSuccessfully)
        {
            MsgBox.OpenTips("pre buy failed");
        }
        else
        {
#if UNITY_EDITOR
            NetWorkHandler.RequestBillingBuy("aaaa", "123", "1111", "$1111", Device.Apple, reqItem);
#else
            //TODO apple goole 商店氪金逻辑
            // NetWorkHandler.RequestBillingBuy(reqItem);
#endif            
        }
    }

    private void OnRecvBuyPay(P13_Response response, ShopItemVo reqItem)
    {
        if (response.Result.Code != NetworkConst.CODE_OK || response.ResultCode != PurchasedResponseKind.ProcessedSuccessfully)
        {
            MsgBox.OpenTips("buy failed : errorCode is " + response.ResultCode );
        }
        else
        {
            var payItem = PayItems.Find(item=> item.pbItems.Id == reqItem.ID);
            payItem.UpdateBuyNum(1);

            var rewardVO = RewardMapVo.From(response);

            //update player assets
            PlayerModel.Instance.UpdateAssets(reqItem, rewardVO);
            Dispatch(ShopEvent.ON_GETTED_ITEM, rewardVO);
        }
    }

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
            //TODO 氪金流程
            NetWorkHandler.RequestBillingPreBuy(vo);
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
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P12_RESPONSE);
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P13_RESPONSE);

        base.Dispose();
    }
}