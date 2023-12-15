using BepInEx.Logging;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace EOH
{
    public class EOHComponent : MonoBehaviour
    {
        private static GameWorld gameWorld;

        private WildSpawnType sptUsec;
        private WildSpawnType sptBear;
        private Player player;

        private static BotsController botController;
        private static BotSpawner botSpawnerClass;
        protected static ManualLogSource Logger
        {
            get; private set;
        }

        public EOHComponent()
        {
            if (Logger == null)
            {
                Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(EOHComponent));
            }
        }

        private void Start()
        {
            sptUsec = (WildSpawnType)Aki.PrePatch.AkiBotsPrePatcher.sptUsecValue;
            sptBear = (WildSpawnType)Aki.PrePatch.AkiBotsPrePatcher.sptBearValue;

            botSpawnerClass.OnBotCreated += OnPlayerAdded;
            //botSpawnerClass.OnBotRemoved += OnPlayerRemoved;
        }
        public static void Enable()
        {
            if (Singleton<IBotGame>.Instantiated)
            {
                gameWorld = Singleton<GameWorld>.Instance;
                gameWorld.GetOrAddComponent<EOHComponent>();

                botController = (Singleton<IBotGame>.Instance).BotsController;
                botSpawnerClass = botController.BotSpawner;

                Logger.LogDebug("Equal Opportunity Hater Enabled");
            }
        }

        private void OnPlayerAdded(BotOwner botOwner)
        {
            player = botOwner.GetPlayer;
            if (!player.IsYourPlayer)
            {
                AddEnemyPMCs(botOwner);
                AddEnemyScavs(botOwner);
            }
        }

        private void AddEnemyPMCs(BotOwner botOwner)
        {
            if (!EOHPlugin.pmcsIncluded.Value)
                return;

            foreach (var bot in gameWorld.AllAlivePlayersList)
            {
                if (!botOwner.BotsGroup.Contains(bot.AIData.BotOwner))
                {
                    var botSettingsClass = new BotSettingsClass(gameWorld.GetAlivePlayerByProfileID(bot.ProfileId), bot.BotsGroup);
                    botOwner.Memory.AddEnemy(bot, botSettingsClass, true);
#if DEBUG
                        Logger.LogWarning($"For botOwner{botOwner.Id}({botOwner.Profile.Info.Settings.Role}): adding Enemy BotID:{bot.Id}({bot.Profile.Info.Settings.Role}) and Name:{bot.name}");
#endif
                }
            }
        }
        private void AddEnemyScavs(BotOwner botOwner)
        {
            if (!EOHPlugin.scavsIncluded.Value)
                return;

            foreach (var bot in gameWorld.AllAlivePlayersList)
            {
                if (!botOwner.BotsGroup.Contains(bot.AIData.BotOwner))
                {
                    var botSettingsClass = new BotSettingsClass(gameWorld.GetAlivePlayerByProfileID(bot.ProfileId), bot.BotsGroup);
                    botOwner.Memory.AddEnemy(bot, botSettingsClass, true);
#if DEBUG
                        Logger.LogWarning($"For botOwner{botOwner.Id}({botOwner.Profile.Info.Settings.Role}): adding Enemy BotID:{bot.Id}({bot.Profile.Info.Settings.Role}) and Name:{bot.name}");
#endif
                }
            }
        }

        private bool isPMC(Player player)
        {
            if (player.Profile.Info.Settings.Role == sptUsec || player.Profile.Info.Settings.Role == sptBear)
            {
                return true;
            }

            return false;
        }

        private bool isScav(Player player)
        {
            if (player.Profile.Info.Settings.Role == WildSpawnType.assault || player.Profile.Info.Settings.Role == WildSpawnType.cursedAssault)
            {
                return true;
            }

            return false;
        }


    }
}
