using System;

using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Models;
using ACE.Server.Entity;
using ACE.Server.Network.GameEvent.Events;

namespace ACE.Server.WorldObjects
{
    public class Hooker : WorldObject
    {
        /// <summary>
        /// A new biota be created taking all of its values from weenie.
        /// </summary>
        public Hooker(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
        {
            SetEphemeralValues();
        }

        /// <summary>
        /// Restore a WorldObject from the database.
        /// </summary>
        public Hooker(Biota biota) : base(biota)
        {
            SetEphemeralValues();
        }

        private void SetEphemeralValues()
        {
            ActivationResponse |= ActivationResponse.Emote;
        }

        public override void ActOnUse(WorldObject activator)
        {
            // handled in base.OnActivate -> EmoteManager.OnUse()
        }

        public override ActivationResult CheckUseRequirements(WorldObject activator, bool silent = false)
        {
            if (!(activator is Player player))
                return new ActivationResult(false);

            if (!IsHooked(player, out var hook))
                return silent ? new ActivationResult(false) : new ActivationResult(new GameEventWeenieErrorWithString(player.Session, WeenieErrorWithString.ItemOnlyUsableOnHook, Name));

            if (hook.HouseOwner == null || (!(hook.House?.RootHouse?.OpenStatus ?? false) && !(hook.House?.RootHouse?.HasPermission(player) ?? false)))
                return silent ? new ActivationResult(false) : new ActivationResult(new GameEventWeenieError(player.Session, WeenieError.YouAreNotPermittedToUseThatHook));

            var myHookGroup = HookGroup ?? HookGroupType.Undef;
            if ((myHookGroup == HookGroupType.PortalItems || myHookGroup == HookGroupType.SpellTeachingItems) && hook.House?.RootHouse?.HouseType != HouseType.Mansion && hook.House?.RootHouse?.HouseType != HouseType.CustomMansion)
                return silent ? new ActivationResult(false) : new ActivationResult(new GameEventWeenieError(player.Session, WeenieError.YouAreNotPermittedToUseThatHook));

            var baseRequirements = base.CheckUseRequirements(activator, silent);
            if (!baseRequirements.Success)
                return baseRequirements;

            return new ActivationResult(true);
        }

        public bool IsHooked (WorldObject checker, out Hook hook)
        {
            hook = null;

            if (!OwnerId.HasValue || OwnerId.Value == 0)
                return false;

            var wo = checker.CurrentLandblock.GetObject(OwnerId.Value);

            if (wo == null)
                return false;

            if (!(wo is Hook _hook))
                return false;

            hook = _hook;

            return true;
        }
    }
}
