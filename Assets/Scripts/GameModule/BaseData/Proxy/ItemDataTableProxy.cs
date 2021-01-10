public class ItemDataTableProxy : BaseDataTableProxy<ItemDataTable, ItemData, ItemDataTableProxy>
{
    static readonly int SHOP_LANG_ST_ID = 40000;

    public ItemDataTableProxy() : base("JsonData/ItemData") {}
    public string GetItemName(int id)
    {
        return LanguageDataTableProxy.GetText(SHOP_LANG_ST_ID + id);
    }
}