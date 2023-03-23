using SpaceWarp.API.Mods;
using HarmonyLib;
using BepInEx;
using SpaceWarp;
using BepInEx.Configuration;

namespace UITweeks.UITweeks;

[BepInPlugin(ModGuid, ModName, ModVer)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class InteractiveFilterLoader : BaseSpaceWarpPlugin
{
    public const string ModGuid = "com.vchristof.interactive_filter";
    public const string ModName = "interactive_filter";
    public const string ModVer = "0.1";
    public static InteractiveFilterLoader Instance { get; set; }

    protected Dictionary<string,ConfigEntry<bool>> configUseMod = new();

    public override void OnInitialized()
    {
        base.OnInitialized();
        Instance = this;
        Harmony.CreateAndPatchAll(typeof(InteractiveFilterLoader).Assembly, ModGuid);
    }
}