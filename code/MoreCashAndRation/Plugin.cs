using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;


[BepInPlugin("m16a4666.PolyMods.MoreCash&Ration", "MoreCash&Ration", "1.0.0")]
public class CollectibleMod : BaseUnityPlugin
{
    public static ConfigEntry<int> MinCash;
    public static ConfigEntry<int> MaxCash;
    public static ConfigEntry<int> MinRations;
    public static ConfigEntry<int> MaxRations;


    private void Awake()
    {
        // 创建配置项
        MinCash = Config.Bind("General", 
            "Min Cash", 
            250, 
            "Minimum amount of cash to add");
            
        MaxCash = Config.Bind("General",
            "Max Cash",
            1000,
            "Maximum amount of cash to add");
            
        MinRations = Config.Bind("General",
            "Min Rations",
            100,
            "Minimum amount of rations to add");
            
        MaxRations = Config.Bind("General",
            "Max Rations",
            250,
            "Maximum amount of rations to add");


        // 应用Harmony补丁
        Harmony.CreateAndPatchAll(typeof(CollectiblePatches));
Logger.LogInfo("CashMod loaded!");
    }
}


[HarmonyPatch(typeof(CollectibleItem))]
public class CollectiblePatches
{
    [HarmonyPatch("OnTriggerEnter")]
    [HarmonyPrefix]
    static void ModifyCollectibleValues(CollectibleItem __instance, Collider other)
    {
        // 确保是玩家触发
        if (!other.GetComponent<PlayerMovement>()) return;


        // 使用Traverse访问私有字段
        var instanceTraverse = Traverse.Create(__instance);
        
        // 修改现金参数
        if (__instance.giveMoneyOnPickup)
        {
            instanceTraverse.Field("minMoney").SetValue(CollectibleMod.MinCash.Value);
            instanceTraverse.Field("maxMoney").SetValue(CollectibleMod.MaxCash.Value);
        }


        // 修改补给参数
        if ((bool)instanceTraverse.Field("giveRationsOnPickup").GetValue())
        {
            instanceTraverse.Field("minRations").SetValue(CollectibleMod.MinRations.Value);
            instanceTraverse.Field("maxRations").SetValue(CollectibleMod.MaxRations.Value);
        }
    }
}