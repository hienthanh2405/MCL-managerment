using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SettingDto : BaseDto
    {

        [Required(ErrorMessage = "You must provider MoneyToPoint")]
        public int MoneyToPoint { get; set; }

        [Required(ErrorMessage = "You must provider PointToMoney")]
        public int PointToMoney { get; set; }

        public string CompanyName { get; set; }

        public string CompanyDescription { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyPhone { get; set; }

        public string CompanyFax { get; set; }

        public string CompanyWebsite { get; set; }

        public string CompanyEmail { get; set; }

        public int OldProductWarmingDays { get; set; }

        public int OldTruckTireWarmingDays { get; set; }

        public int OldTravelTireWarmingDays { get; set; }

        public bool IsAllowNegativeInventoryBill { get; set; }

        public bool IsOnlyDestroyBillOnSameDay { get; set; }

        public bool IsAllowUsingPoint { get; set; }

        public bool CanSaleWithDebtForRetailBill { get; set; }

    }
}
