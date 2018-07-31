using API.Models;
using System;
using System.Collections.Generic;

namespace API.Helpers
{
    public static class ProductStorageHelper
    {
        public static List<CapitalPriceTrackingDto> UpdateCapitalPriceTracking(int updatedIndex, 
           CapitalPriceTrackingDto updatedCapitalPriceTrackingDto,
            List<CapitalPriceTrackingDto> capitalPriceTrackings)
        {
            capitalPriceTrackings[updatedIndex].Amount = updatedCapitalPriceTrackingDto.Amount;
            capitalPriceTrackings[updatedIndex].Inventory = updatedCapitalPriceTrackingDto.Inventory;
            capitalPriceTrackings[updatedIndex].CapitalPrice = updatedCapitalPriceTrackingDto.CapitalPrice;
            capitalPriceTrackings[updatedIndex].InputPrice = updatedCapitalPriceTrackingDto.InputPrice;

            for (int i = updatedIndex + 1; i < capitalPriceTrackings.Count; i++)
            {
                int previousIndex = i - 1;
                {
                    if (capitalPriceTrackings[i].WarehousingId != Guid.Empty) // warehousing record 
                    {
                        capitalPriceTrackings[i].Inventory = capitalPriceTrackings[previousIndex].Inventory + capitalPriceTrackings[i].Amount;
                        capitalPriceTrackings[i].CapitalPrice = CalculateCapitalPrice(
                            capitalPriceTrackings[previousIndex].Inventory,
                            capitalPriceTrackings[previousIndex].CapitalPrice,
                            capitalPriceTrackings[i].Amount,
                            capitalPriceTrackings[i].InputPrice
                            );
                    }
                    else if (capitalPriceTrackings[i].BillId != Guid.Empty)// Bill record 
                    {
                        capitalPriceTrackings[i].CapitalPrice = capitalPriceTrackings[previousIndex].CapitalPrice;
                        capitalPriceTrackings[i].Inventory = capitalPriceTrackings[previousIndex].Inventory - capitalPriceTrackings[i].Amount;
                    }
                }
            }

            return capitalPriceTrackings;
        }


        public static double CalculateCapitalPrice(double oldInventory, double oldCapitalPrice,
           double inputAmount, double inputPrice)
        {
            if(oldInventory <=0)
            {
                return inputPrice;
            } else
            {
                return Math.Round(
                             (oldInventory * oldCapitalPrice + inputAmount * inputPrice) /
                             (oldInventory + inputAmount), 0);
            }
        }
    }
}
