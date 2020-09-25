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
        var response = GetResponseData();

        return response;
        
    }

    P10_Response GetResponseData()
    {
        var res = new P10_Response();
        for (int i = 0; i < 5; i++)
        {
            var shopItem = new ShopBillingProduct(){ Id = 1, PayType = PayType.Diamond, Price = 123 };
            shopItem.ProductContent.Add(new ProductContent(){ Amount = 1, ContentId = 1});
            res.ProductList.Add(shopItem);
        }
        for (int i = 0; i < 5; i++)
        {
            var shopItem = new ShopBillingProduct(){ Id = 1, PayType = PayType.Gold, Price = 234 };
            shopItem.ProductContent.Add(new ProductContent(){ Amount = 1, ContentId = 2});
            res.ProductList.Add(shopItem);
        }
        return res;
    }
}