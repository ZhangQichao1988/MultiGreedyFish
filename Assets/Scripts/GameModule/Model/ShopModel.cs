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

    public override void Dispose()
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P10_RESPONSE);
        base.Dispose();
    }
}