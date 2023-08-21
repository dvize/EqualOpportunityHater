using System.Reflection;
using Aki.Reflection.Patching;
using BepInEx;
using BepInEx.Configuration;
using EFT;

namespace EOH
{
    [BepInPlugin("com.dvize.EqualOpportunityHater", "dvize.EqualOpportunityHater", "1.0.0")]
    public class EOHPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> pmcsIncluded;
        public static ConfigEntry<bool> scavsIncluded;

        private void Awake()
        {

            pmcsIncluded = Config.Bind(
                "Main Settings",
                "PMCs Hate Each Other",
                true,
                "Same side PMCs will fight each other");

            scavsIncluded = Config.Bind(
                "Main Settings",
                "Scavs Hate Each Other",
                false,
                "Scavs will fight each other");

            new NewGamePatch().Enable();
        }

    }

    //re-initializes each new game
    internal class NewGamePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() => typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));

        [PatchPrefix]
        public static void PatchPrefix()
        {
            EOHComponent.Enable();
        }
    }
}
