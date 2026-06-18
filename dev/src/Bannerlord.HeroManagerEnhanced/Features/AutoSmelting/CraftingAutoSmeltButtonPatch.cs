using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Bannerlord.HeroManagerEnhanced.Features.AutoSmelting;

[PrefabExtension(
    "Crafting",
    "descendant::ListPanel[@Id='MainActionListPanel']/Children/Widget[@Id='MainActionParent']")]
internal sealed class CraftingAutoSmeltButtonPatch : PrefabExtensionInsertPatch
{
    public override InsertType Type => InsertType.Append;

    [PrefabExtensionText]
    public string GetPrefabExtension =>
        "<Widget Id=\"HME_AutoSmeltParent\" WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" SuggestedWidth=\"190\" SuggestedHeight=\"50\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Bottom\" MarginLeft=\"10\" IsVisible=\"@IsInSmeltingMode\">"
      + "<Children>"
      + "<ButtonWidget Id=\"HME_AutoSmeltButton\" DoNotPassEventsToChildren=\"true\" WidthSizePolicy=\"StretchToParent\" HeightSizePolicy=\"StretchToParent\" Brush=\"Crafting.MainAction.Button\" Command.Click=\"ExecuteAutoSmelt\" IsEnabled=\"@IsAutoSmeltEnabled\" UpdateChildrenStates=\"true\">"
      + "<Children>"
        + "<TextWidget WidthSizePolicy=\"CoverChildren\" HeightSizePolicy=\"CoverChildren\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\" MarginTop=\"15\" Brush=\"Crafting.MainAction.Text\" Text=\"@AutoSmeltButtonText\" />"
      + "</Children>"
      + "</ButtonWidget>"
      + "</Children>"
        + "</Widget>";
}
