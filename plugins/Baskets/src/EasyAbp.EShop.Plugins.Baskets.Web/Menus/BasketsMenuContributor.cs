﻿using System.Collections.Generic;
using System.Threading.Tasks;
using EasyAbp.EShop.Plugins.Baskets.Localization;
using Volo.Abp.UI.Navigation;

namespace EasyAbp.EShop.Plugins.Baskets.Web.Menus
{
    public class BasketsMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenu(context);
            }
        }

        private async Task ConfigureMainMenu(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<BasketsResource>(); //Add main menu items.

            // var basketManagementMenuItem = new ApplicationMenuItem(BasketsMenus.Prefix, l["Menu:BasketManagement"]);

            context.Menu.AddItem(
                new ApplicationMenuItem(BasketsMenus.BasketItem, l["Menu:BasketItem"],
                    $"/EShop/Plugins/Baskets/BasketItems/BasketItem?basketName={BasketsConsts.DefaultBasketName}&userId=",
                    icon: "fa fa-shopping-cart")
            );

            // if (!basketManagementMenuItem.Items.IsNullOrEmpty())
            // {
            //     var eShopMenuItem = context.Menu.GetAdministration().Items.GetOrAdd(i => i.Name == BasketsMenus.ModuleGroupPrefix,
            //         () => new ApplicationMenuItem(BasketsMenus.ModuleGroupPrefix, l["Menu:EasyAbpEShop"], icon: "fa fa-shopping-bag"));
            //     
            //     eShopMenuItem.Items.Add(basketManagementMenuItem);
            // }
        }
    }
}