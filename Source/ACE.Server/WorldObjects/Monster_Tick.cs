using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using System;

namespace ACE.Server.WorldObjects
{
    partial class Creature
    {
        protected const double monsterTickInterval = 0.2;

        public double NextMonsterTickTime;

        public double NextFailedMovementDecayTime;

        private bool firstUpdate = true;

        private int AttacksReceivedWithoutBeingAbleToCounter = 0;
        private double NextNoCounterResetTime = double.MaxValue;
        private static double NoCounterInterval = 60;
        private Position PreviousTickPosition = new Position();

        /// <summary>
        /// Primary dispatch for monster think
        /// </summary>
        public void Monster_Tick(double currentUnixTime)
        {
            if (IsChessPiece && this is GamePiece gamePiece)
            {
                // faster than vtable?
                gamePiece.Tick(currentUnixTime);
                return;
            }

            if (IsPassivePet && this is Pet pet)
            {
                pet.Tick(currentUnixTime);
                return;
            }

            if (StunnedUntilTimestamp != 0)
            {
                if (StunnedUntilTimestamp >= currentUnixTime)
                {
                    if (NextStunEffectTimestamp <= currentUnixTime)
                    {
                        EnqueueBroadcast(new GameMessageScript(Guid, PlayScript.SplatterUpLeftBack));
                        EnqueueBroadcast(new GameMessageScript(Guid, PlayScript.SplatterUpRightBack));
                        EnqueueBroadcast(new GameMessageScript(Guid, PlayScript.SplatterUpLeftFront));
                        EnqueueBroadcast(new GameMessageScript(Guid, PlayScript.SplatterUpRightFront));
                        NextStunEffectTimestamp = currentUnixTime + StunEffectFrequency;
                    }
                    return;
                }
                else
                    StunnedUntilTimestamp = 0;
            }

            NextMonsterTickTime = currentUnixTime + ThreadSafeRandom.Next((float)monsterTickInterval * 0.8f, (float)monsterTickInterval * 1.2f); // Add some randomization here to keep creatures from acting in perfect synch.

            if (!IsAwake)
            {
                if (MonsterState == State.Return)
                    MonsterState = State.Idle;

                if (IsFactionMob || HasFoeType)
                    FactionMob_CheckMonsters();

                return;
            }
            else if (!IsDead)
            {
                //if (PhysicsObj.MovementManager.MoveToManager.FailProgressCount > 0 && Timers.RunningTime > NextCancelTime)
                //{
                //    Console.WriteLine("CancelTime");
                //    CancelMoveTo(WeenieError.ActionCancelled);
                //    return;
                //}

                if (NextFailedMovementDecayTime < currentUnixTime && PhysicsObj?.MovementManager?.MoveToManager?.FailProgressCount > 0)
                {
                    PhysicsObj.MovementManager.MoveToManager.FailProgressCount--;
                    NextFailedMovementDecayTime = currentUnixTime + 3;
                }

                if (PhysicsObj?.MovementManager?.MoveToManager?.FailProgressCount >= 5)
                    CancelMoveTo(WeenieError.ActionCancelled);

                UpdatePosition(!PhysicsObj.IsSticky);
            }
            else
                return;

            if (EmoteManager.IsBusy)
                return;

            HandleFindTarget();

            if (AttackTarget == null && !IsMoveToHomePending && !IsMovingToHome)
                TryMoveToHome();

            CheckMissHome();    // tickrate?

            if (IsMoveToHomePending)
                MoveToHome();
            if (IsMovingToHome || IsMoveToHomePending)
            {
                if (PendingEndMoveToHome)
                    EndMoveToHome();
                return;
            }

            var combatPet = this as CombatPet;

            var creatureTarget = AttackTarget as Creature;
            var playerTarget = AttackTarget as Player;

            if (playerTarget != null && playerTarget.IsSneaking)
            {
                if (IsDirectVisible(playerTarget))
                    playerTarget.EndSneaking($"{Name} can still see you! You stop sneaking!");
            }

            if (creatureTarget != null && (creatureTarget.IsDead || (combatPet == null && !IsVisibleTarget(creatureTarget))) || (playerTarget != null && playerTarget.IsSneaking))
            {
                FindNextTarget();
                return;
            }

            if (firstUpdate)
            {
                if (CurrentMotionState.Stance == MotionStance.NonCombat)
                    DoAttackStance();

                if (IsAnimating)
                {
                    //PhysicsObj.ShowPendingMotions();
                    PhysicsObj.update_object();
                    return;
                }

                firstUpdate = false;
            }

            if (MonsterState == State.Awake && GetDistanceToTarget() >= MaxChaseRange)
            {
                if (HasPendingMovement)
                    CancelMoveTo(WeenieError.ObjectGone);
                FindNextTarget();
                return;
            }

            if (IsSwitchWeaponsPending)
                SwitchWeapons();
            if (IsSwitchingWeapons || IsSwitchWeaponsPending)
                return;

            // select a new weapon if missile launcher is out of ammo
            var weapon = GetEquippedWeapon();
            /*if (weapon != null && weapon.IsAmmoLauncher)
            {
                var ammo = GetEquippedAmmo();
                if (ammo == null)
                    SwitchToMeleeAttack();
            }*/

            if (weapon == null && CurrentAttackType != null && CurrentAttackType == CombatType.Missile)
            {
                EquipInventoryItems(true, false, true, false);
                DoAttackStance();
                CurrentAttackType = null;
            }

            // decide current type of attack
            if (CurrentAttackType == null)
            {
                CurrentAttackType = GetNextAttackType();
                if (CurrentAttackType != CombatType.Missile || !MissileCombatMeleeRangeMode)
                    MaxRange = GetMaxRange();
                else
                    MaxRange = MaxMeleeRange;

                //if (CurrentAttack == AttackType.Magic)
                //MaxRange = MaxMeleeRange;   // FIXME: server position sync
            }

            if (IsAttackPending)
                Attack();
            if (IsAttacking || IsAttackPending)
            {
                if (PendingEndAttack)
                {
                    EndAttack(false);
                    if (PendingEndAttack)
                        return;
                }
                else
                    return;
            }

            PathfindingEnabled = PropertyManager.GetBool("pathfinding").Item;

            if ((/*IsEmotePending ||*/ IsWanderingPending || IsRouteStartPending /*|| IsEmoting*/ || IsWandering || IsRouting) && ((CurrentAttackType == CombatType.Melee && IsMeleeVisible(AttackTarget)) || (CurrentAttackType != CombatType.Melee && IsDirectVisible(AttackTarget))))
            {
                // If we can see our target abort everything and go for it.

                //Console.WriteLine("Pathfinding: Target Sighted!");

                FailedMovementCount = 0;

                IsEmotePending = false;
                IsWanderingPending = false;
                IsRouteStartPending = false;

                // Figure out a way to cancel motions so they will actually stop mid-play client-side. MotionCommand.Dead does it but I haven't been able to figure out why.
                //if (IsEmoting)
                //    PendingEndEmoting = true;

                if (IsWandering)
                    PendingEndWandering = true;

                if (IsRouting)
                    PendingEndRoute = true;
            }
            else
            {
                if (IsEmotePending)
                    Emote();
                if (IsEmoting || IsEmotePending)
                {
                    if (PendingEndEmoting)
                    {
                        EndEmoting(false);
                        if (PendingEndEmoting)
                            return;
                    }
                    else
                        return;
                }

                if (IsWanderingPending)
                    Wander();
                if (IsWandering || IsWanderingPending)
                {
                    if (PendingEndWandering)
                    {
                        EndWandering(false);
                        if (PendingEndWandering)
                            return;
                    }
                    else
                        return;
                }

                if (IsRouteStartPending)
                {
                    StartRoute();
                    if (IsRouteStartPending)
                        return;
                }
                if (IsRouting)
                {
                    if (PendingEndRoute)
                        EndRoute(false);
                    else if (PendingRetryRoute)
                        RetryRoute();
                    else if (PendingContinueRoute)
                        ContinueRoute();

                    if (IsRouting)
                        return;
                }
            }

            if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
            {
                if (NextNoCounterResetTime <= currentUnixTime)
                {
                    AttacksReceivedWithoutBeingAbleToCounter = 0;
                    NextNoCounterResetTime = double.MaxValue;
                }

                var distanceCovered = PreviousTickPosition?.SquaredDistanceTo(Location);
                PreviousTickPosition = new Position(Location);

                if (IsMoving && distanceCovered > 0.2)
                    AttacksReceivedWithoutBeingAbleToCounter = 0;

                if (AttackTarget != null && !Location.Indoors)
                {
                    var heightDifference = Math.Abs(Location.PositionZ - AttackTarget.Location.PositionZ);

                    if (heightDifference > 2.0 && AttacksReceivedWithoutBeingAbleToCounter > 2)
                    {
                        AttacksReceivedWithoutBeingAbleToCounter = 0;

                        if (HasRangedWeapon && CurrentAttackType == CombatType.Melee && !IsSwitchWeaponsPending && LastWeaponSwitchTime + 5 < currentUnixTime)
                        {
                            TrySwitchToMissileAttack();
                            return;
                        }
                        else
                        {
                            FindNewHome(100, 260, 100);
                            TryMoveToHome();
                            return;
                        }
                    }
                }
            }

            var targetDist = GetDistanceToTarget();
            if (CurrentAttackType != CombatType.Missile || Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
            {
                if (!PhysicsObj.IsSticky && targetDist > MaxRange || (!IsFacing(AttackTarget) && !IsSelfCast()))
                {
                    // turn / move towards
                    if (!IsMoving)
                        StartMovement();
                    else
                    {
                        if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
                        {
                            if (FailedMovementCount >= FailedMovementThreshold)
                            {
                                if (HasRangedWeapon && CurrentAttackType == CombatType.Melee && LastWeaponSwitchTime + MaxSwitchWeaponFrequency < currentUnixTime && IsVisibleTarget(AttackTarget))
                                    TrySwitchToMissileAttack();
                                else
                                {
                                    if (LastEmoteTime + MaxEmoteFrequency < currentUnixTime && EmoteChance > ThreadSafeRandom.Next(0.0f, 1.0f))
                                        TryEmoting();

                                    if (LastWanderTime + MaxWanderFrequency < currentUnixTime && WanderChance > ThreadSafeRandom.Next(0.0f, 1.0f))
                                    {
                                        if (PathfindingEnabled && !LastRouteStartAttemptWasNullRoute)
                                            TryWandering(160, 200, 3);
                                        else
                                            TryWandering(100, 260, 5);
                                    }

                                    if(PathfindingEnabled)
                                        TryRoute();
                                }
                            }
                            else if (HasRangedWeapon && CurrentAttackType == CombatType.Melee && targetDist > 20 && LastWeaponSwitchTime + MaxSwitchWeaponFrequency < currentUnixTime && IsVisibleTarget(AttackTarget))
                                TrySwitchToMissileAttack();
                        }
                    }
                }
                else
                    TryAttack();
            }
            else
            {
                if (IsMoving)
                    return;

                if (!IsFacing(AttackTarget))
                    StartMovement();
                else if (targetDist <= MaxRange)
                    TryAttack();
                else
                {
                    // monster switches to melee combat immediately,
                    // if target is beyond max range?

                    // should ranged mobs only get CurrentTargets within MaxRange?
                    //Console.WriteLine($"{Name}.MissileAttack({AttackTarget.Name}): targetDist={targetDist}, MaxRange={MaxRange}, switching to melee");
                    TrySwitchToMeleeAttack();
                }
            }

            // pets drawing aggro
            if (combatPet != null)
                combatPet.PetCheckMonsters();
        }
    }
}
