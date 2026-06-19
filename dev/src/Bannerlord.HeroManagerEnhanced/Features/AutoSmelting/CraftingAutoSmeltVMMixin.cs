using System.Linq;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.HeroManagerEnhanced.Features.AutoSmelting;

[ViewModelMixin(nameof(CraftingVM.RefreshValues))]
internal sealed class CraftingAutoSmeltVMMixin : BaseViewModelMixin<CraftingVM>
{
    private readonly string _autoButtonText;

    public CraftingAutoSmeltVMMixin(CraftingVM vm)
        : base(vm)
    {
        _autoButtonText = new TextObject("{=HME_AUTO_BUTTON}Auto").ToString();
    }

    [DataSourceProperty]
    public string AutoSmeltButtonText => _autoButtonText;

    [DataSourceProperty]
    public string AutoRefineButtonText => _autoButtonText;

    [DataSourceProperty]
    public bool IsAutoSmeltEnabled => ViewModel?.IsInSmeltingMode == true && ViewModel.IsMainActionEnabled;

    [DataSourceProperty]
    public bool IsAutoRefineEnabled => ViewModel?.IsInRefinementMode == true && ViewModel.IsMainActionEnabled;

    public override void OnRefresh()
    {
        base.OnRefresh();
        OnPropertyChangedWithValue(IsAutoSmeltEnabled, nameof(IsAutoSmeltEnabled));
        OnPropertyChangedWithValue(IsAutoRefineEnabled, nameof(IsAutoRefineEnabled));
    }

    [DataSourceMethod]
    public void ExecuteAutoSmelt()
    {
        if (ViewModel is null || !ViewModel.IsInSmeltingMode)
            return;

        var smelting = ViewModel.Smelting;
        if (smelting is null)
            return;

        const int maxIterations = 5000;
        var iterations = 0;

        while (iterations++ < maxIterations)
        {
            if (!TrySelectExecutableSmeltItem(smelting))
                break;

            var beforeUnlockedCount = GetUnlockedItemCount(smelting);
            var staminaBefore = GetCurrentStamina();
            ViewModel.ExecuteMainAction();
            var staminaAfter = GetCurrentStamina();
            var afterUnlockedCount = GetUnlockedItemCount(smelting);

            var itemProcessed = afterUnlockedCount < beforeUnlockedCount;
            if (!itemProcessed)
                break;

            // A valid smelt action should consume stamina once; if not, stop to avoid runaway loops.
            if (staminaBefore >= 0f && staminaAfter >= staminaBefore)
                break;
        }

        OnPropertyChangedWithValue(IsAutoSmeltEnabled, nameof(IsAutoSmeltEnabled));
        OnPropertyChangedWithValue(IsAutoRefineEnabled, nameof(IsAutoRefineEnabled));
    }

    [DataSourceMethod]
    public void ExecuteAutoRefine()
    {
        if (ViewModel is null || !ViewModel.IsInRefinementMode)
            return;

        const int maxIterations = 5000;
        var iterations = 0;

        while (iterations++ < maxIterations)
        {
            // Always check current availability before processing the next recipe.
            if (!ViewModel.IsMainActionEnabled)
                break;

            var staminaBefore = GetCurrentStamina();
            ViewModel.ExecuteMainAction();
            var staminaAfter = GetCurrentStamina();

            // A valid refine action should consume stamina once; if not, stop to avoid runaway loops.
            if (staminaBefore >= 0f && staminaAfter >= staminaBefore)
                break;
        }

        OnPropertyChangedWithValue(IsAutoSmeltEnabled, nameof(IsAutoSmeltEnabled));
        OnPropertyChangedWithValue(IsAutoRefineEnabled, nameof(IsAutoRefineEnabled));
    }

    private bool TrySelectExecutableSmeltItem(SmeltingVM smelting)
    {
        if (smelting.SmeltableItemList is not { } list)
            return false;

        foreach (var item in list.Where(static candidate => candidate.NumOfItems > 0))
        {
            item.ExecuteSelection();
            if (ViewModel?.IsMainActionEnabled == true)
                return true;
        }

        return false;
    }

    private static int GetUnlockedItemCount(SmeltingVM smelting)
    {
        if (smelting.SmeltableItemList is not { } list)
            return 0;

        return list.Where(static item => !item.IsLocked)
            .Sum(static item => item.NumOfItems);
    }

    private float GetCurrentStamina()
    {
        return ViewModel?.CurrentCraftingHero?.CurrentStamina ?? -1f;
    }

}
