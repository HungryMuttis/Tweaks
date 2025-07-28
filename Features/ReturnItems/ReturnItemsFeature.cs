using CWAPI;

namespace Tweaks.Features.ReturnItems
{
    [Feature]
    internal class ReturnItemsFeature : Feature<ReturnItemsFeature>
    {
        public override BepInEx.Logging.ManualLogSource LogSource => Tweaks.Logger;
        public override string FeatureName => "ReturnFallenItems";
        public override string FeatureDescription => "Returns any items that may have fallen out of the island";

        public override void Initialize()
        {
            ItemInstancePatch.Init();
        }
    }
}
