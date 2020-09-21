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
        NetWorkHandler.GetDispatch().AddListener<P11_Response, P11_Request>(GameEvent.RECIEVE_P11_RESPONSE, OnRecvItemBuyNormal);
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

    private void OnRecvItemBuyNormal(P11_Response response, P11_Request request)
    {
        if (response.Result.Code == NetworkConst.CODE_OK)
        {
            var clientItem = OtherItems.Find(item=> item.pbItems.Id == request.ShopItemId);
            clientItem.UpdateBuyNum(request.ShopItemNum);


            Dispatch(ShopEvent.ON_GETTED_ITEM, RewardMapVo.From(response));
        }
        else
        {
            Debug.LogError(response.Result.Desc);
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
        NetWorkHandler.RequestBuyNormal(vo.masterDataItem.ID, num);
    }

    public override void Dispose()
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P10_RESPONSE);
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P11_RESPONSE);
        base.Dispose();
    }
}