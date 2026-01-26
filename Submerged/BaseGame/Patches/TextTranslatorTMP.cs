using HarmonyLib;
using TMPro;

namespace Submerged.BaseGame.Patches;

[HarmonyPatch]
public static class TextTranslatorTMPPatches
{
    [HarmonyPatch(typeof(TextTranslatorTMP), nameof(TextTranslatorTMP.ResetText))]
    [HarmonyPrefix]
    [BaseGameCode(LastChecked.v17_1_0, "We are patching this with its own code to get rid of inlining that ruins our translation patches")]
    public static void ResetTextPatch(TextTranslatorTMP __instance, out bool __runOriginal)
    {
        __runOriginal = false;

        if (__instance == null || (__instance.ResetOnlyWhenNoDefault && string.IsNullOrEmpty(__instance.defaultStr))) return;

        if (TranslationController.Instance == null) return;

        TextMeshPro component = __instance.GetComponent<TextMeshPro>();
        string text = TranslationController.Instance.GetStringWithDefault(__instance.TargetText, __instance.defaultStr);

        if (text == null) return;

        if (__instance.ToUpper)
        {
            text = text.ToUpperInvariant();
        }

        if (component != null)
        {
            component.text = text;
            component.ForceMeshUpdate();
        }
        else
        {
            TextMeshProUGUI component2 = __instance.GetComponent<TextMeshProUGUI>();
            if (component2 != null)
            {
                component2.text = text;
                component2.ForceMeshUpdate();
            }
        }

        __instance.OnTranslate?.Invoke();
    }
}
