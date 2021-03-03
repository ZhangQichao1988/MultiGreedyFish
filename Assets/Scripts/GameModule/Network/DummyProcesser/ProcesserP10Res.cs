using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 修改昵称
/// </summary>
public class ProcesserP10Res : BaseDummyProcesser<P10_Request, P10_Response>
{
    public override P10_Response ProcessRequest(int msgId, P10_Request pbData)
    {
        var response = GetResponseData(pbData.ProductType);

        return response;
        
    }

    P10_Response GetResponseData(ShopType type)
    {
        var res = new P10_Response();
        if (type == ShopType.Other)
        {  
            for (int i = 0; i < 10; i++)
            {
                var shopItem = new ShopBillingProduct(){ Id = 300, PayType = PayType.Diamond, Price = 1, PriceRate = 1f, Priority = 2 };
                shopItem.ProductContent.Add(new ProductContent(){ Amount = 1, ContentId = 4});
                res.ProductList.Add(shopItem);
            }
            for (int i = 0; i < 10; i++)
            {
                var shopItem = new ShopBillingProduct(){ Id = 1, PayType = PayType.Gold, Price = 2, PriceRate = 0.3f, Priority = 1 };
                shopItem.ProductContent.Add(new ProductContent(){ Amount = 1, ContentId = 2});
                res.ProductList.Add(shopItem);
            }

        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                var shopItem = new ShopBillingProduct(){ Id = 1, PayType = PayType.Money, Price = 11, PriceRate = 0.2f };
                shopItem.PlatformProductId = "jp.co.crazyfish.item00" + (i + 1);
                shopItem.ProductContent.Add(new ProductContent(){ Amount = 1, ContentId = 1});
                res.ProductList.Add(shopItem);
            }
        }
        
        return res;
    }

    public override void DispatchRes(int msgId, P10_Request request, P10_Response response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P10_Response, P10_Request>(NetWorkHandler.GetDispatchKey(msgId), response, request);
    }
}