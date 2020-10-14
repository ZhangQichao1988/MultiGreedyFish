namespace Jackpot.Billing
{
    public class ResumeProduct : Product
    {
        public ResumeProduct(string id, string type, string title, string description, string price, string formattedPrice, Currency currency, string token) 
            : base(id, type, title,description, price, formattedPrice, currency)
        {
            PurchaseToken = token;
        }

        public string PurchaseToken { get; set; }
    }
}