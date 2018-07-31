using API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace API.Helpers
{
    public static class ChangeCapitalPriceTrackingsResultHelper
    {
        public static ChangeCapitalPriceTrackingsResult ChangeCapitalPriceTrackings(int startedIndex, double deltaInventory, double inputPrice, List<CapitalPriceTrackingDto> capitalPriceTrackings, bool isRemove)
        {

            if (startedIndex == 0) // missing default record, add one (this is a hot fix)
            {
                CapitalPriceTrackingDto capitalPriceTrackingDto = new CapitalPriceTrackingDto()
                {
                    BillId = Guid.Empty,
                    WarehousingId = Guid.Empty,
                    InventoryId = Guid.Empty,
                    InputPrice = capitalPriceTrackings[0].InputPrice,
                    CapitalPrice = capitalPriceTrackings[0].CapitalPrice,
                    Inventory = capitalPriceTrackings[0].Inventory
                };
                capitalPriceTrackings.Insert(0, capitalPriceTrackingDto);
                startedIndex = 1;
            }
            if (isRemove)
            {
                capitalPriceTrackings.RemoveAt(startedIndex);
            }
            else
            {
                capitalPriceTrackings[startedIndex].Amount += deltaInventory;
                capitalPriceTrackings[startedIndex].InputPrice = inputPrice;
            }

            for (int i = startedIndex; i < capitalPriceTrackings.Count; i++)
            {
                int previousIndex = (i - 1 < 0) ? 0 : (i - 1); // this is a hot shit for fix bug: do not create default capital price tracking when create a product on other storage
                if (previousIndex == 0)  // shit here
                {
                    capitalPriceTrackings[i].Inventory = capitalPriceTrackings[i].Amount; // shit here
                }
                else
                {
                    if (capitalPriceTrackings[i].WarehousingId != Guid.Empty) // warehousing record 
                    {
                        capitalPriceTrackings[i].Inventory = capitalPriceTrackings[previousIndex].Inventory + capitalPriceTrackings[i].Amount;
                        capitalPriceTrackings[i].CapitalPrice =

                        capitalPriceTrackings[i].CapitalPrice =
                               ProductStorageHelper.CalculateCapitalPrice(
                               capitalPriceTrackings[previousIndex].Inventory,
                               capitalPriceTrackings[previousIndex].CapitalPrice,
                               capitalPriceTrackings[i].Amount,
                               capitalPriceTrackings[i].InputPrice
                               );

                    }
                    else if (capitalPriceTrackings[i].BillId != Guid.Empty)// Bill record record 
                    {
                        capitalPriceTrackings[i].Inventory = capitalPriceTrackings[previousIndex].Inventory - capitalPriceTrackings[i].Amount;
                        capitalPriceTrackings[i].CapitalPrice = capitalPriceTrackings[previousIndex].CapitalPrice;
                    }
                }
            }
            var inventory = capitalPriceTrackings[capitalPriceTrackings.Count - 1].Inventory;
            var capitalPrice = capitalPriceTrackings[capitalPriceTrackings.Count - 1].CapitalPrice;
            var capitalPriceTrackingsJson = JsonConvert.SerializeObject(capitalPriceTrackings);

            return new ChangeCapitalPriceTrackingsResult
            {
                Inventory = inventory,
                CapitalPrice = capitalPrice,
                CapitalPriceTrackingsJson = capitalPriceTrackingsJson
            };
        }
    }

    public class ChangeCapitalPriceTrackingsResult
    {
        public double Inventory { get; set; }
        public double CapitalPrice { get; set; }
        public string CapitalPriceTrackingsJson { get; set; }

    }



}
