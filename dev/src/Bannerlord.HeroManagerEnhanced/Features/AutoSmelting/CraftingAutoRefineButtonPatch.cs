using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Bannerlord.HeroManagerEnhanced.Features.AutoSmelting;

[PrefabExtension(
    "Crafting",
    "descendant::ListPanel[@Id='MainActionListPanel']/Children/Widget[@Id='MainActionParent']")]
internal sealed class CraftingAutoRefineButtonPatch : PrefabExtensionInsertPatch
{
    public override InsertType Type => InsertType.Append;

    [PrefabExtensionText]
    public string GetPrefabExtension =>
        "<Widget Id=\"HME_AutoRefineParent\" WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" SuggestedWidth=\"190\" SuggestedHeight=\"50\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Bottom\" MarginLeft=\"10\" IsVisible=\"@IsInRefinementMode\">"
      + "<Children>"
      + "<ButtonWidget Id=\"HME_AutoRefineButton\" DoNotPassEventsToChildren=\"true\" WidthSizePolicy=\"StretchToParent\" HeightSizePolicy=\"StretchToParent\" Brush=\"Crafting.MainAction.Button\" Command.Click=\"ExecuteAutoRefine\" IsEnabled=\"@IsMainActionEnabled\" UpdateChildrenStates=\"true\">"
      + "<Children>"
    + "<TextWidget WidthSizePolicy=\"CoverChildren\" HeightSizePolicy=\"CoverChildren\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\" MarginTop=\"15\" Brush=\"Crafting.MainAction.Text\" Text=\"@AutoRefineButtonText\" />"
      + "</Children>"
      + "</ButtonWidget>"
      + "</Children>"
      + "</Widget>";
}
