using System;

using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Models;
using ACE.Server.Factories;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    public class Cow : Creature
    {
        /// <summary>
        /// A new biota be created taking all of its values from weenie.
        /// </summary>
        public Cow(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
        {
            SetEphemeralValues();
        }

        /// <summary>
        /// Restore a WorldObject from the database.
        /// </summary>
        public Cow(Biota biota) : base(biota)
        {
            SetEphemeralValues();
        }

        private void SetEphemeralValues()
        {
        }

        /// <summary>
        /// This is raised by Player.HandleActionUseItem.<para />
        /// The item does not exist in the players possession.<para />
        /// If the item was outside of range, the player will have been commanded to move using DoMoveTo before ActOnUse is called.<para />
        /// When this is called, it should be assumed that the player is within range.
        /// </summary>
        public override void ActOnUse(WorldObject activator)
        {
            // handled in base.OnActivate -> EmoteManager.OnUse()

            if (activator is Player player && Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
            {
                if(player.GetNumInventoryItemsOfWCID((uint)Factories.Enum.WeenieClassName.flask, true) > 0 && player.TryConsumeFromInventoryWithNetworking((int)Factories.Enum.WeenieClassName.flask, 1))
                {
                    var wo = WorldObjectFactory.CreateNewWorldObject((int)Factories.Enum.WeenieClassName.milk);

                    if (wo != null)
                    {
                        if (!player.TryCreateInInventoryWithNetworking(wo, out _, true))
                            wo.Destroy();
                        else
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You milk the {Name} and fill a flask.", ChatMessageType.Broadcast));
                    }
                }
                else
                    player.Session.Network.EnqueueSend(new GameMessageSystemChat($"If you had an empty flask you could try milking the cow.", ChatMessageType.Broadcast));
            }
        }
    }
}
