namespace API.Helpers
{
    public static class CONSTANT
    {
        public static int TIME_ZONE = 7; // vietnam time zone

        // Status for Product Storage filter 
        public static string PRODUCT_STORAGE_ALL ="ALL";
        public static string PRODUCT_STORAGE_HAS_INVENTORY = "HAS_INVENTORY";
        public static string PRODUCT_STORAGE_SOLD_OUT = "SOLD_OUT";

        // Status for InventoryEntity 
        public static string INVENTORY_INVENTORIED = "INVENTORIED";
        public static string INVENTORY_BANLANCED = "BANLANCED";
        public static string INVENTORY_DESTROY = "DESTROY";

        // Product Category Types

        public static string PRODUCT_CATEGORY_TIRE = "TIRE";
        public static string PRODUCT_CATEGORY_TRAVEL_TIRE = "TRAVEL_TIRE";
        public static string PRODUCT_CATEGORY_TRUCK_TIRE = "TRUCK_TIRE";

        public static string BILL_PREFIX = "DH";
        public static string WAREHOUSING_PREFIX = "PN";

        public static int GENERATED_NUMBER_LENGTH = 6; // ex: 0000001 on HD2018000001

        public static string ROLE_OWNER = "OWNER";
        public static string ROLE_MANAGER = "MANAGER";
        public static string ROLE_SALEMAN = "SALEMAN";
        public static string ROLE_WAREHOUSE = "WAREHOUSE";
      



    }
}
