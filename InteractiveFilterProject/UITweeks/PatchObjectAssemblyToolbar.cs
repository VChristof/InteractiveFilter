using HarmonyLib;
using KSP.OAB;
using KSP.UI.Binding;
using UnityEngine.UI;
using UnityEngine;
using KSP.UI;
using SpaceWarp.API.Assets;

namespace UITweeks.UITweeks
{
    [HarmonyPatch(typeof(ObjectAssemblyToolbar), nameof(ObjectAssemblyToolbar.Initialize))]
    class PatchObjectAssemblyToolbar
    {
        [HarmonyPostfix]
        public static void Postfix(ObjectAssemblyToolbar __instance)
        {
            InteractiveFilter interactiveFilter = __instance.gameObject.AddComponent<InteractiveFilter>();

            Transform body = __instance.transform;

            Transform GRPInfoOverlays = body.FindChildEx("GRP-Info-Overlays");

            GameObject myConfig = GameObject.Instantiate(GRPInfoOverlays.gameObject, body);
            ((RectTransform)myConfig.transform).anchoredPosition += new Vector2(100.0f, 0.0f);
            Debug.Log("PatchObjectAssemblyToolbar - 0");
            HorizontalLayoutGroup horizontalLayoutGroup = myConfig.GetComponent<HorizontalLayoutGroup>();
            GameObject.Destroy(horizontalLayoutGroup);
            _ = DelayLayoutAdd(myConfig);

            Transform KSP2ToggleButtonToolsBarCenterOfDrag = myConfig.transform.FindChildEx("KSP2ToggleButton_ToolsBar-CenterOfDrag");
            GameObject.Destroy(KSP2ToggleButtonToolsBarCenterOfDrag.gameObject);

            myConfig.name = "Interactive-Filters";
            Debug.Log("PatchObjectAssemblyToolbar - 1");
            Transform topControl = myConfig.transform.FindChildEx("KSP2ToggleButton_ToolsBar-CenterOfThrust");

            Debug.Log("PatchObjectAssemblyToolbar - 2");
            if (topControl != null)
            {
                Debug.Log("PatchObjectAssemblyToolbar - 3");
                topControl.name = "TopControl";
                ToggleExtended topControlToggle = topControl.gameObject.GetComponent<ToggleExtended>();
                Debug.Log("PatchObjectAssemblyToolbar - 4");
                if (topControlToggle != null)
                {
                    Debug.Log("PatchObjectAssemblyToolbar - 5");
                    topControlToggle.onValueChanged.RemoveAllListeners();
                    topControlToggle.onValueChanged.AddListener(interactiveFilter.ToggleTop);
                    interactiveFilter.SetTopToggle(topControlToggle);
                }
                ObjectAssemblyBuilderTooltipDisplay toolTip = topControl.gameObject.GetComponent<ObjectAssemblyBuilderTooltipDisplay>();
                Traverse.Create(toolTip).Field("tooltipText").SetValue("Top filter");
                GameObject.Destroy(topControl.gameObject.GetComponent<UIValue_WriteBool_Toggle>());

                Texture2D topImageNormal = AssetManager.GetAsset<Texture2D>($"{InteractiveFilterLoader.ModName}/images/topNormal.png");
                Texture2D topImageHighlighted = AssetManager.GetAsset<Texture2D>($"{InteractiveFilterLoader.ModName}/images/topHighlighted.png");
                Sprite topSpriteNormal = Sprite.Create(topImageNormal, new Rect(0, 0, topImageNormal.width, topImageNormal.height), new Vector2(0.5f, 0.5f));
                Sprite topSpriteHighlighted = Sprite.Create(topImageHighlighted, new Rect(0, 0, topImageHighlighted.width, topImageHighlighted.height), new Vector2(0.5f, 0.5f));
                topControl.GetComponent<Image>().sprite = topSpriteNormal;
                ToggleExtendedEventsVisualizer toggleExtendedEventsVisualizer = topControl.gameObject.GetComponent<ToggleExtendedEventsVisualizer>();
                toggleExtendedEventsVisualizer.HighlightedSprite = topSpriteHighlighted;
                toggleExtendedEventsVisualizer.NormalSprite = topSpriteNormal;
                toggleExtendedEventsVisualizer.PressedSprite = topSpriteHighlighted;
                toggleExtendedEventsVisualizer.SelectedSprite = topSpriteNormal;
                toggleExtendedEventsVisualizer.enabled = false;
            }
            Debug.Log("PatchObjectAssemblyToolbar - 6");
            Transform bottomControl = myConfig.transform.FindChildEx("KSP2ToggleButton_ToolsBar-CenterOfMass");
            if (bottomControl != null)
            {
                Debug.Log("PatchObjectAssemblyToolbar - 7");
                bottomControl.name = "BottomControl";
                ToggleExtended bottomControlToggle = bottomControl.gameObject.GetComponent<ToggleExtended>();
                Debug.Log("PatchObjectAssemblyToolbar - 8");
                if (bottomControlToggle != null)
                {
                    Debug.Log("PatchObjectAssemblyToolbar - 9");
                    bottomControlToggle.onValueChanged.RemoveAllListeners();
                    bottomControlToggle.onValueChanged.AddListener(interactiveFilter.ToggleBottom);
                    interactiveFilter.SetBottomToggle(bottomControlToggle);
                }
                ObjectAssemblyBuilderTooltipDisplay toolTip = bottomControl.gameObject.GetComponent<ObjectAssemblyBuilderTooltipDisplay>();
                Traverse.Create(toolTip).Field("tooltipText").SetValue("Bottom filter");
                GameObject.Destroy(bottomControl.gameObject.GetComponent<UIValue_WriteBool_Toggle>());

                Texture2D bottomImageNormal = AssetManager.GetAsset<Texture2D>($"{InteractiveFilterLoader.ModName}/images/bottomNormal.png");
                Texture2D bottomImageHighlighted = AssetManager.GetAsset<Texture2D>($"{InteractiveFilterLoader.ModName}/images/bottomHighlighted.png");
                Sprite bottomSpriteNormal = Sprite.Create(bottomImageNormal, new Rect(0, 0, bottomImageNormal.width, bottomImageNormal.height), new Vector2(0.5f, 0.5f));
                Sprite bottomSpriteHighlighted = Sprite.Create(bottomImageHighlighted, new Rect(0, 0, bottomImageHighlighted.width, bottomImageHighlighted.height), new Vector2(0.5f, 0.5f));
                bottomControl.GetComponent<Image>().sprite = bottomSpriteNormal;
                ToggleExtendedEventsVisualizer toggleExtendedEventsVisualizer = bottomControl.GetComponent<ToggleExtendedEventsVisualizer>();
                toggleExtendedEventsVisualizer.HighlightedSprite = bottomSpriteHighlighted;
                toggleExtendedEventsVisualizer.NormalSprite = bottomSpriteNormal;
                toggleExtendedEventsVisualizer.PressedSprite = bottomSpriteHighlighted;
                toggleExtendedEventsVisualizer.SelectedSprite = bottomSpriteNormal;
                toggleExtendedEventsVisualizer.enabled = false;
            }
        }

        private static async Task DelayLayoutAdd(GameObject myConfig)
        {
            await Task.Delay(500);
            VerticalLayoutGroup verticalLayoutGroup = myConfig.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlHeight = true;
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childScaleHeight = false;
            verticalLayoutGroup.childScaleWidth = false;
            verticalLayoutGroup.spacing = 4.0f;

            Transform topControl = myConfig.transform.FindChildEx("TopControl");
            topControl.gameObject.GetComponent<ToggleExtendedEventsVisualizer>().enabled = true;

            Transform bottomControl = myConfig.transform.FindChildEx("BottomControl");
            bottomControl.gameObject.GetComponent<ToggleExtendedEventsVisualizer>().enabled = true;
        }
    }
}
