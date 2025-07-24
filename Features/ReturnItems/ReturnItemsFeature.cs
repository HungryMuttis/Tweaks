namespace Tweaks.Features.ReturnItems
{
    [ModFeature]
    internal class ReturnItemsFeature : Feature<ReturnItemsFeature>
    {
        public override string FeatureName => "ReturnFallenItems";
        public override string FeatureDescription => "Returns any items that may have fallen out of the island";

        public override void Initialize()
        {
            ItemInstancePatch.Init();
        }
    }
}
