using System;

namespace Tweaks.Patches
{
    public class ExampleShoppingCartPatch
    {
        internal static void Init()
        {
            /*
             *  Subscribe with 'On.Namespace.Type.Method += CustomMethod;' for each method you're patching.
             *  Or if you are writing an ILHook, use 'IL.' instead of 'On.'
             *  Note that not all types are in a namespace, especially in Unity games.
             */

            On.ShoppingCart.AddItemToCart += ShoppingCart_AddItemToCart;
        }

        private static void ShoppingCart_AddItemToCart(On.ShoppingCart.orig_AddItemToCart orig, ShoppingCart self, ShopItem item)
        {
            // Call the Trampoline for the Original method or another method in the Detour Chain if any exist
            orig(self, item);

            /*
             * Adding a random value to the visible price of the shopping cart is slightly complicated
             * due to the private setter of the CartValue property. So to change the value, we must get the setter
             * via reflection, and call it with the new value.
             */
            AccessTools.PropertySetter(typeof(ShoppingCart), "CartValue").Invoke(
                self, new object[] { self.CartValue + new Random().Next(0, 100) });
        }
    }
}
