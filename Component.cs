﻿using BepInEx.Logging;
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

        private static BotControllerClass botController;
        private static BotSpawnerClass botSpawnerClass;
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
            //make sure its not the Mainplayer being added
            if (!botOwner.GetPlayer.IsYourPlayer)
            {
                player = botOwner.GetPlayer;
                Logger.LogDebug("In OnPlayerAdded Method: " + player.gameObject.name);

                //if botOwner is a pmc than add to all other pmcs alive
                if (EOHPlugin.pmcsIncluded.Value && isPMC(botOwner.GetPlayer))
                {
                    foreach (var bot in gameWorld.AllAlivePlayersList)
                    {
                        if (isPMC(bot) && bot.AIData.BotOwner != botOwner)
                        {
                            //remove allies if they exist of current bot
                            botOwner.BotsGroup.RemoveAlly(bot.AIData.BotOwner);
                            var botSettingsClass = new BotSettingsClass(Singleton<GameWorld>.Instance.GetAlivePlayerByProfileID(bot.ProfileId), bot.BotsGroup);
                            botOwner.Memory.AddEnemy(bot, botSettingsClass, true);
                            Logger.LogWarning($"For botOwner{botOwner.Id}({botOwner.Profile.Info.Settings.Role}): adding Enemy BotID:{bot.Id}({bot.Profile.Info.Settings.Role}) and Name:{bot.name}");
                        }
                    }
                }

                if (EOHPlugin.scavsIncluded.Value && isScav(botOwner.GetPlayer))
                {
                    foreach (var bot in gameWorld.AllAlivePlayersList)
                    {
                        if (isScav(bot) && bot.AIData.BotOwner != botOwner)
                        {
                            botOwner.BotsGroup.RemoveAlly(bot.AIData.BotOwner);
                            var botSettingsClass = new BotSettingsClass(Singleton<GameWorld>.Instance.GetAlivePlayerByProfileID(bot.ProfileId), bot.BotsGroup);
                            botOwner.Memory.AddEnemy(bot, botSettingsClass, true);
                            Logger.LogWarning($"For botOwner{botOwner.Id}({botOwner.Profile.Info.Settings.Role}): adding Enemy BotID:{bot.Id}({bot.Profile.Info.Settings.Role}) and Name:{bot.name}");
                        }
                    }
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
        private void OnPlayerRemoved(BotOwner botOwner)
        {

        }






    }
}