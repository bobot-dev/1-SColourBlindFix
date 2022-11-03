using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UMM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneSColourBlindFix
{
    [HarmonyPatch]
    [BepInPlugin(GUID, "1-S Colour Blind Options", "1.0.0")]
    public class OneSColourBlindFix : BaseUnityPlugin
    {
        public const string GUID = "bot.uk.onescolourblindfix";

        private static Harmony harmony;

        private void Start()
        {
            harmony = new Harmony(GUID);
            harmony.PatchAll();
        }



        [HarmonyPatch(typeof(ColorBlindActivator), nameof(ColorBlindActivator.Start))]
        [HarmonyPrefix]
        public static void AddNewColourBlindOptions(ColorBlindActivator __instance)
        {
            var baseCBS = __instance.GetComponentsInChildren<ColorBlindSetter>(true).ToList().Where(x => x.gameObject.name == "Gold Variation").First();

            if (baseCBS)
            {
                Color[] colors = new Color[] { Color.red, Color.green, new Color(0f, 0.25f, 1f) };
                CustomHudColorType[] chcts = new CustomHudColorType[] { CustomHudColorType.s1Red, CustomHudColorType.s1Green, CustomHudColorType.s1Blue };
                for (int i = 1; i <= 3; i++)
                {
                    var CBS = Instantiate(baseCBS, baseCBS.gameObject.transform.parent);
                    CBS.gameObject.name = $"1-S COLOR {i}";
                    CBS.transform.localPosition -= new Vector3(0, 130 * i, 0);
                    CBS.transform.GetComponentInChildren<Text>().text = $"1-S COLOR {i}";
                    CBS.enemyColor = false;
                    CBS.variationColor = false;
                    CBS.hct = (HudColorType)(chcts[i - 1]);
                    CBS.name = $"1scb{i}";
                }
            }
        }

        public static Color scbfRed = Color.red;
        public static Color scbfGreen = Color.green;
        public static Color scbfBlue = new Color(0f, 0.25f, 1f);

        [HarmonyPatch(typeof(ColorBlindSettings), nameof(ColorBlindSettings.SetHudColor))]
        [HarmonyPostfix]
        public static void SetHudColorPatch(ColorBlindSettings __instance, HudColorType hct, Color color)
        {
            switch (hct)
            {
                case (HudColorType)CustomHudColorType.s1Green:
                    scbfGreen = color;
                    break;
                case (HudColorType)CustomHudColorType.s1Red:
                    scbfRed = color;
                    break;
                case (HudColorType)CustomHudColorType.s1Blue:
                    scbfBlue = color;
                    break;
            }
        }

        [HarmonyPatch(typeof(ColorBlindSettings), nameof(ColorBlindSettings.GetHudColor))]
        [HarmonyPostfix]
        public static void GetHudColorPatch(ColorBlindSettings __instance, HudColorType hct, ref Color __result)
        {
            switch (hct)
            {
                case (HudColorType)CustomHudColorType.s1Green:
                    __result = Color.green;
                    break;
                case (HudColorType)CustomHudColorType.s1Red:
                    __result = Color.red;
                    break;
                case (HudColorType)CustomHudColorType.s1Blue:
                    __result = new Color(0f, 0.25f, 1f);
                    break;
            }
        }

        [HarmonyPatch(typeof(PuzzleLine), nameof(PuzzleLine.TranslateColor))]
        [HarmonyPostfix]
        public static void TranslateColorPatch(TileColor color, ref Color __result)
        {
            switch (color)
            {
                case TileColor.None:
                    __result = Color.white;
                    break;
                case TileColor.Red:
                    __result = scbfRed;
                    break;
                case TileColor.Green:
                    __result = scbfGreen;
                    break;
                case TileColor.Blue:
                    __result = scbfBlue;
                    break;
            }
        }

        [HarmonyPatch(typeof(PuzzlePanel), nameof(PuzzlePanel.Start))]
        [HarmonyPostfix]
        public static void StartPatch(PuzzlePanel __instance)
        {
            if (__instance.currentPanel && __instance.currentPanel.GetComponent<Image>())
            {
                __instance.currentPanel.GetComponent<Image>().color = __instance.pl.TranslateColor(__instance.tileColor);
            }
           
        }

    }

    public enum CustomHudColorType
    {
        s1Red = 10,
        s1Green = 11,
        s1Blue = 12,
    }
}
