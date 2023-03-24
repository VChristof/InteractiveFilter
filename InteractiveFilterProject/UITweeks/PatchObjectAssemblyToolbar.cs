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
        private const string TRANSFORM_RENAME = "Interactive-Filters";
        private const string TOP_CONTROL_NAME = "TopControl";
        private const string BOTTOM_CONTROL_NAME = "BottomControl";
        private const string GRPINFOOVERLAYS = "GRP-Info-Overlays";
        private const string KSP2_TOGGLE_BUTTON_TOOLSBAR_CENTER_OF_DRAG = "KSP2ToggleButton_ToolsBar-CenterOfDrag";
        private const string KSP2_TOGGLE_BUTTON_TOOLSBAR_CENTER_OF_THRUST = "KSP2ToggleButton_ToolsBar-CenterOfThrust";
        private const string KSP2_TOGGLE_BUTTON_TOOLSBAR_CENTER_OF_MASS = "KSP2ToggleButton_ToolsBar-CenterOfMass";
        private const string TOP_NORMAL_IMAGE = $"{InteractiveFilterLoader.ModName}/images/topNormal.png";
        private const string TOP_HIGHLIGHTED_IMAGE = $"{InteractiveFilterLoader.ModName}/images/topHighlighted.png";
        private const string BOTTOM_NORMAL_IMAGE = $"{InteractiveFilterLoader.ModName}/images/bottomNormal.png";
        private const string BOTTOM_HIGHLIGHTED_IMAGE = $"{InteractiveFilterLoader.ModName}/images/bottomHighlighted.png";

        [HarmonyPostfix]
        public static void Postfix(ObjectAssemblyToolbar __instance)
        {
            try
            {
                InteractiveFilter interactiveFilter = __instance.gameObject.AddComponent<InteractiveFilter>();
                Transform body = __instance.transform;

                Transform GRPInfoOverlays = body.FindChildEx(GRPINFOOVERLAYS);

                GameObject myConfig = GameObject.Instantiate(GRPInfoOverlays.gameObject, body);
                ((RectTransform)myConfig.transform).anchoredPosition += new Vector2(100.0f, 0.0f);
                HorizontalLayoutGroup horizontalLayoutGroup = myConfig.GetComponent<HorizontalLayoutGroup>();
                GameObject.Destroy(horizontalLayoutGroup);
                _ = DelayLayoutAdd(myConfig);

                Transform KSP2ToggleButtonToolsBarCenterOfDrag = myConfig.transform.FindChildEx(KSP2_TOGGLE_BUTTON_TOOLSBAR_CENTER_OF_DRAG);
                GameObject.Destroy(KSP2ToggleButtonToolsBarCenterOfDrag.gameObject);

                myConfig.name = TRANSFORM_RENAME;
                Transform topControl = myConfig.transform.FindChildEx(KSP2_TOGGLE_BUTTON_TOOLSBAR_CENTER_OF_THRUST);

                topControl.name = TOP_CONTROL_NAME;
                ToggleExtended topControlToggle = topControl.gameObject.GetComponent<ToggleExtended>();
                topControlToggle.onValueChanged.RemoveAllListeners();
                topControlToggle.onValueChanged.AddListener(interactiveFilter.ToggleTop);
                interactiveFilter.SetTopToggle(topControlToggle);
                ObjectAssemblyBuilderTooltipDisplay topToolTip = topControl.gameObject.GetComponent<ObjectAssemblyBuilderTooltipDisplay>();
                Traverse.Create(topToolTip).Field("tooltipText").SetValue("Top filter");
                GameObject.Destroy(topControl.gameObject.GetComponent<UIValue_WriteBool_Toggle>());

                Texture2D topImageNormal = AssetManager.GetAsset<Texture2D>(TOP_NORMAL_IMAGE);
                Texture2D topImageHighlighted = AssetManager.GetAsset<Texture2D>(TOP_HIGHLIGHTED_IMAGE);
                Sprite topSpriteNormal = Sprite.Create(topImageNormal, new Rect(0, 0, topImageNormal.width, topImageNormal.height), new Vector2(0.5f, 0.5f));
                Sprite topSpriteHighlighted = Sprite.Create(topImageHighlighted, new Rect(0, 0, topImageHighlighted.width, topImageHighlighted.height), new Vector2(0.5f, 0.5f));
                topControl.GetComponent<Image>().sprite = topSpriteNormal;
                ToggleExtendedEventsVisualizer topToggleExtendedEventsVisualizer = topControl.gameObject.GetComponent<ToggleExtendedEventsVisualizer>();
                topToggleExtendedEventsVisualizer.HighlightedSprite = topSpriteHighlighted;
                topToggleExtendedEventsVisualizer.NormalSprite = topSpriteNormal;
                topToggleExtendedEventsVisualizer.PressedSprite = topSpriteHighlighted;
                topToggleExtendedEventsVisualizer.SelectedSprite = topSpriteNormal;
                topToggleExtendedEventsVisualizer.enabled = false;

                Transform bottomControl = myConfig.transform.FindChildEx(KSP2_TOGGLE_BUTTON_TOOLSBAR_CENTER_OF_MASS);
                bottomControl.name = BOTTOM_CONTROL_NAME;
                ToggleExtended bottomControlToggle = bottomControl.gameObject.GetComponent<ToggleExtended>();
                bottomControlToggle.onValueChanged.RemoveAllListeners();
                bottomControlToggle.onValueChanged.AddListener(interactiveFilter.ToggleBottom);
                interactiveFilter.SetBottomToggle(bottomControlToggle);
                ObjectAssemblyBuilderTooltipDisplay bottomToolTip = bottomControl.gameObject.GetComponent<ObjectAssemblyBuilderTooltipDisplay>();
                Traverse.Create(bottomToolTip).Field("tooltipText").SetValue("Bottom filter");
                GameObject.Destroy(bottomControl.gameObject.GetComponent<UIValue_WriteBool_Toggle>());

                Texture2D bottomImageNormal = AssetManager.GetAsset<Texture2D>(BOTTOM_NORMAL_IMAGE);
                Texture2D bottomImageHighlighted = AssetManager.GetAsset<Texture2D>(BOTTOM_HIGHLIGHTED_IMAGE);
                Sprite bottomSpriteNormal = Sprite.Create(bottomImageNormal, new Rect(0, 0, bottomImageNormal.width, bottomImageNormal.height), new Vector2(0.5f, 0.5f));
                Sprite bottomSpriteHighlighted = Sprite.Create(bottomImageHighlighted, new Rect(0, 0, bottomImageHighlighted.width, bottomImageHighlighted.height), new Vector2(0.5f, 0.5f));
                bottomControl.GetComponent<Image>().sprite = bottomSpriteNormal;
                ToggleExtendedEventsVisualizer bottomToggleExtendedEventsVisualizer = bottomControl.GetComponent<ToggleExtendedEventsVisualizer>();
                bottomToggleExtendedEventsVisualizer.HighlightedSprite = bottomSpriteHighlighted;
                bottomToggleExtendedEventsVisualizer.NormalSprite = bottomSpriteNormal;
                bottomToggleExtendedEventsVisualizer.PressedSprite = bottomSpriteHighlighted;
                bottomToggleExtendedEventsVisualizer.SelectedSprite = bottomSpriteNormal;
                bottomToggleExtendedEventsVisualizer.enabled = false;
            } catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private static async Task DelayLayoutAdd(GameObject myConfig)
        {
            await Task.Delay(500);
            try
            {
                VerticalLayoutGroup verticalLayoutGroup = myConfig.AddComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.childControlHeight = true;
                verticalLayoutGroup.childControlWidth = true;
                verticalLayoutGroup.childForceExpandHeight = false;
                verticalLayoutGroup.childForceExpandWidth = false;
                verticalLayoutGroup.childScaleHeight = false;
                verticalLayoutGroup.childScaleWidth = false;
                verticalLayoutGroup.spacing = 4.0f;

                Transform topControl = myConfig.transform.FindChildEx(TOP_CONTROL_NAME);
                topControl.gameObject.GetComponent<ToggleExtendedEventsVisualizer>().enabled = true;

                Transform bottomControl = myConfig.transform.FindChildEx(BOTTOM_CONTROL_NAME);
                bottomControl.gameObject.GetComponent<ToggleExtendedEventsVisualizer>().enabled = true;
            } catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
