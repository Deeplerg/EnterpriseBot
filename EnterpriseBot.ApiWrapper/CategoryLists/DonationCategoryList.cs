using EnterpriseBot.ApiWrapper.Categories.Donation;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class DonationCategoryList
    {
        internal DonationCategoryList() { }

        public DonationCategory Donation { get; internal set; }
        public DonationPurchaseCategory DonationPurchase { get; internal set; }
    }
}
