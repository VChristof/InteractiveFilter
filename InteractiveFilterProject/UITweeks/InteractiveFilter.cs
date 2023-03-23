using HarmonyLib;
using KSP.Game;
using KSP.OAB;
using UnityEngine;
using UnityEngine.UI;

namespace UITweeks.UITweeks
{
    class InteractiveFilter : MonoBehaviour
    {
        private const bool DEBUG_ENABLED = false;

        private static bool bottomFilterOn = false;
        private static bool topFilterOn = false;

        private bool partEventsRegisted = false;
        private static IObjectAssembly mainAssembly;
        private static readonly List<AssemblySizeFilterType> sizes = new();
        private static int partCount = -1;
        private float registerTimer = 0;

        private ToggleExtended topToggle, bottomToggle;

        private static readonly List<AssemblyPartsButton> assemblyPartsButtons = new();

        enum NODE_POSITION
        {
            TOP,
            BOTTOM
        }

        class AssemblySizeFilterTypeToPosition
        {
            public AssemblySizeFilterType size;
            public NODE_POSITION position;
            public AssemblySizeFilterTypeToPosition(AssemblySizeFilterType size, NODE_POSITION position)
            {
                this.size = size;
                this.position = position;
            }
        }

        private static readonly Dictionary<string, AssemblySizeFilterTypeToPosition> CATEGORY_OVERRIDE = new()
        {
            {"adapter_1v_bullet_methalox_0v-1v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.XS, NODE_POSITION.TOP) },
            {"adapter_1v_conical_0v-1v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.XS, NODE_POSITION.TOP) },
            {"adapter_1v_flat_0v-1v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.XS, NODE_POSITION.TOP) },

            {"adapter_2v_m2_1x1_methalox_1v-m2", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },
            {"adapter_2v_m2_1x2_methalox_1v-m2", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },
            {"adapter_2v_conical_methalox_1v-2v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },
            {"adapter_2v_rightcone_methalox_1v-2v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },
            {"adapter_2v_conical_1v-2v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },
            {"adapter_2v_flat_1v-2v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },

            {"adapter_3v_methalox_2v-3v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.M, NODE_POSITION.TOP) },
            {"adapter_3v_m3_methalox_m2-m3", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.M, NODE_POSITION.TOP) },
            {"adapter_3v_conical_2v-3v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.M, NODE_POSITION.TOP) },
            {"pod_3v_lander_crew", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.M, NODE_POSITION.TOP) },

            {"adapter_4v_methalox_3v-4v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.L, NODE_POSITION.TOP) },
            {"adapter_4v_3v-4v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.L, NODE_POSITION.TOP) },

            {"pod_1v_conical_crew", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.XS, NODE_POSITION.TOP) },

            {"pod_2v_conical_crew", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },

            {"pod_3v_conical_crew", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },
            {"cockpit_3v_m3_crew", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },


            {"resizer_1v_square_1v-0v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.XS, NODE_POSITION.TOP) },

            {"resizer_2v_circular_2v-1v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },
            {"resizer_2v_square_2v-1v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.S, NODE_POSITION.TOP) },

            {"resizer_3v_circular_3v-2v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.M, NODE_POSITION.TOP) },
            {"resizer_3v_square_3v-2v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.M, NODE_POSITION.TOP) },

            {"resizer_4v_circular_4v-2v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.M, NODE_POSITION.TOP) },
            {"resizer_4v_square_4v-2v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.M, NODE_POSITION.TOP) },

            {"resizer_4v_circular_4v-3v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.L, NODE_POSITION.TOP) },
            {"resizer_4v_square_4v-3v", new AssemblySizeFilterTypeToPosition(AssemblySizeFilterType.L, NODE_POSITION.TOP) }

        };
        void Update()
        {
            if (IsOABLoaded())
            {
                if (registerTimer <= 0)
                {
                    RegisterForEvents();
                }
                else
                {
                    registerTimer -= Time.unscaledDeltaTime;
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    bottomFilterOn = !bottomFilterOn;
                    if (bottomToggle != null)
                    {
                        bottomToggle.isOn = bottomFilterOn;
                    }
                    RefreshSizes();
                    RefreshUIButtons();

                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    topFilterOn = !topFilterOn;
                    if (topToggle != null)
                    {
                        topToggle.isOn = topFilterOn;
                    }
                    RefreshSizes();
                    RefreshUIButtons();

                }

                if (mainAssembly != null && mainAssembly.Parts.Count != partCount)
                {
                    RefreshSizes();
                }
            }
        }

        private Rect windowRect;
        private int windowWidth = 300;
        private int windowHeight = 300;

        void Awake()
        {
            windowRect = new Rect((Screen.width * 0.7f) - (windowWidth / 2), (Screen.height / 2) - (windowHeight / 2), 0, 0);
        }

        void OnGUI()
        {
            if (DEBUG_ENABLED && IsOABLoaded())
            {
                GUI.skin = SpaceWarp.API.UI.Skins.ConsoleSkin;
                windowRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                windowRect,
                    FillWindow,
                     "<color=#696DFF>//Interactive Filter</color>",
                    GUILayout.Height(0),
                    GUILayout.Width(300));
            }
        }

        public void ToggleTop(bool value)
        {
            topFilterOn = value;
            RefreshSizes();
            RefreshUIButtons();
        }

        public void SetTopToggle(ToggleExtended topToggle)
        {
            this.topToggle = topToggle;
        }

        public void ToggleBottom(bool value)
        {
            bottomFilterOn = value;
            RefreshSizes();
            RefreshUIButtons();
        }

        public void SetBottomToggle(ToggleExtended bottomToggle)
        {
            this.bottomToggle = bottomToggle;
        }

        private void FillWindow(int windowID)
        {
            GUILayout.BeginVertical();
            topFilterOn = GUILayout.Toggle(topFilterOn, "Top");
            bottomFilterOn = GUILayout.Toggle(bottomFilterOn, "Bottom");
            GUILayout.EndHorizontal();

            if (mainAssembly != null) {
                if (mainAssembly.Anchor != null)
                {
                    GUILayout.Label("mainAssembly: " + mainAssembly.Anchor.Name);
                }
                else
                {
                    GUILayout.Label("mainAssembly.Anchor is null");
                }
            }
            else
            {
                GUILayout.Label("mainAssembly is null");
            }

            if(GUI.changed)
            {
                RefreshSizes();
            }
        }

        private static void RefreshSizes()
        {
            sizes.Clear();
            partCount = 0;
            if (mainAssembly != null)
            {
                foreach (IObjectAssemblyPart oap in mainAssembly.Parts)
                {
                    CheckSize(oap);
                }
                partCount = mainAssembly.Parts.Count;
            }
            RefreshUIButtons();
        }

        private static void CheckSize(IObjectAssemblyPart oap)
        {
            foreach (ObjectAssemblyPartNode oapn in oap.Nodes)
            {
                Vector3 up = mainAssembly.Anchor.PartTransform.up;
                if((bottomFilterOn && topFilterOn)
                    || (bottomFilterOn && IsWithin5Percent(oapn.transform, -up))
                    || (topFilterOn && IsWithin5Percent(oapn.transform, up)))
                {
                    if (oapn.IsAvailable && !oapn.NodeTag.ToLower().Equals("srfattach"))
                    {
                        AssemblySizeFilterType size = GetSize(oapn.NodeTag, oap);
                        if (!sizes.Contains(size))
                        {
                            sizes.Add(size);
                        }
                    }
                }
            }
        }

        private static void RefreshUIButtons()
        {
            foreach (AssemblyPartsButton assemblyPartsButton in assemblyPartsButtons)
            {
                CheckToEnableButton(assemblyPartsButton);
            }
        }

        private static void CheckToEnableButton(AssemblyPartsButton assemblyPartsButton)
        {
            if (sizes.Count > 0 && (bottomFilterOn || topFilterOn))
            {
                bool shouldBeActive = sizes.Contains(assemblyPartsButton.part.Size) || IsOverridenContains(assemblyPartsButton.part, sizes);
                assemblyPartsButton.gameObject.SetActive(shouldBeActive);
            }
            else
            {
                assemblyPartsButton.gameObject.SetActive(true);
            }
        }
        public static void MyAddButton(AssemblyPartsButton assemblyPartsButton)
        {
            if (!assemblyPartsButtons.Contains(assemblyPartsButton))
            {
                assemblyPartsButtons.Add(assemblyPartsButton);
            }
            CheckToEnableButton(assemblyPartsButton);
        }
        public static void ClearButtons()
        {
            assemblyPartsButtons.Clear();
        }

        private static bool IsOABLoaded()
        {
            return GameManager.Instance.Game != null &&
                GameManager.Instance.Game.OAB != null &&
                GameManager.Instance.Game.OAB.Current != null &&
                GameManager.Instance.Game.OAB.Current.IsLoaded;
        }

        private void RegisterForEvents()
        {
            registerTimer = 3.0f;
            RegisterSetAssebly(ref GameManager.Instance.Game.OAB.Current.eventsBuilder.OnMainAssemblySet);
            RegisterSetAssebly(ref GameManager.Instance.Game.OAB.Current.eventsBuilder.OnMainAssemblyReset);
        }

        private void RegisterSetAssebly(ref Action<IObjectAssembly> actionToRegister)
        {
            foreach (Delegate myDelegate in actionToRegister.GetInvocationList())
            {
                if (myDelegate.Method.Name == nameof(SetAssembly))
                {
                    return;
                }
            }
            actionToRegister += SetAssembly;
        }

        void OnDestroy()
        {
            try
            {
                GameManager.Instance.Game.OAB.Current.eventsBuilder.OnMainAssemblySet -= SetAssembly;
                GameManager.Instance.Game.OAB.Current.eventsBuilder.OnMainAssemblyReset -= SetAssembly;

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void SetAssembly(IObjectAssembly assembly)
        {
            mainAssembly = assembly;
            RefreshSizes();
        }

        static AssemblySizeFilterType GetSize(string nodePosition, IObjectAssemblyPart oap)
        {
            if (CATEGORY_OVERRIDE.ContainsKey(oap.Name))
            {
                AssemblySizeFilterTypeToPosition assemblySizeFilterTypeToPosition = CATEGORY_OVERRIDE[oap.Name];
                if (assemblySizeFilterTypeToPosition.position.ToString().ToLower().Equals(nodePosition.ToLower()))
                {
                    return assemblySizeFilterTypeToPosition.size;
                }
            }
            return oap.PartSize;
        }

        static bool IsOverridenContains(IObjectAssemblyAvailablePart matchingPart, List<AssemblySizeFilterType> sizes)
        {
            if (CATEGORY_OVERRIDE.ContainsKey(matchingPart.Name))
            {
                AssemblySizeFilterTypeToPosition assemblySizeFilterTypeToPosition = CATEGORY_OVERRIDE[matchingPart.Name];
                if (bottomFilterOn || topFilterOn)
                {
                    return sizes.Contains(assemblySizeFilterTypeToPosition.size);
                }
            }
            return false;
        }

        private static bool IsWithin5Percent(Transform node, Vector3 direction)
        {
            Vector3 transform1Forward = node.forward;
            Vector3 transform2Forward = direction;
            float dotProduct = (float)Math.Round(Vector3.Dot(transform1Forward, transform2Forward), 4);
            float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
            return dotProduct == 1 || angle <= 5.0f;
        }

        [HarmonyPatch(typeof(AssemblyPartsPicker), "UpdateButtonsOnFilters")]
        class PatchAssemblyPartsPicker
        {
            [HarmonyPostfix]
            static void UpdateButtonsOnFilters(ref List<IObjectAssemblyAvailablePart> matchingParts)
            {
                ClearButtons();
            }
        }

        [HarmonyPatch(typeof(AssemblyFilterContainer), nameof(AssemblyFilterContainer.AddButton))]
        class PatchAssemblyFilterContainer
        {
            [HarmonyPostfix]
            static void AddButton(ref GameObject btn)
            {
                MyAddButton(btn.GetComponent<AssemblyPartsButton>());
            }
        }
    }
}