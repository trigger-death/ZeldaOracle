﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Monsters.States;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {

	public enum MonsterRespawnType {
		Never	= 0,
		Always	= 1,
		Normal	= 2,
	}

	public enum MonsterColor {
		Red			= 0,
		Blue		= 1,
		Green		= 2,
		Orange		= 3,
		Gold		= 4,
		DarkRed		= 5,
		DarkBlue	= 6,
		InverseBlue	= 7,

		Count
	}

	public partial class Monster : Unit, ZeldaAPI.Monster {
		
		/// <summary>The current monster state.</summary>
		private MonsterState state;
		/// <summary>The previous monster state.</summary>
		private MonsterState previousState;

		// Properties

		private MonsterColor color;
		private int contactDamage;
		protected bool softKill;
		protected bool isBurnable;
		protected bool isGaleable;
		protected bool isStunnable;
		/// <summary>True if the player will always check collisions
		/// as if the monsters z position was at zero.</summary>
		protected bool ignoreZPosition;

		// States
		/*private MonsterBurnState		stateBurn;
		private MonsterStunState		stateStun;
		private MonsterFallInHoleState	stateFallInHole;
		private MonsterGaleState		stateGale;*/



		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Monster() {
			// Entity
			Properties = new Properties();

			// Graphics
			Graphics.DepthLayer         = DepthLayer.Monsters;
			Graphics.DepthLayerInAir    = DepthLayer.InAirMonsters;
			Graphics.DrawOffset         = new Point2I(-8, -14);
			centerOffset                = new Point2I(0, -6);

			// Physics
			Physics.CollisionBox        = new Rectangle2I(-5, -9, 10, 10);
			Physics.SoftCollisionBox    = new Rectangle2I(-6, -11, 12, 11);
			Physics.CollideWithWorld    = true;
			Physics.CollideWithRoomEdge = true;
			Physics.AutoDodges          = true;
			Physics.HasGravity          = true;
			Physics.IsDestroyedInHoles  = true;

			// Interactions
			Interactions.Enable();
			SetDefaultReactions();

			// Unit settings
			knockbackSpeed          = GameSettings.MONSTER_KNOCKBACK_SPEED;
			hurtKnockbackDuration   = GameSettings.MONSTER_HURT_KNOCKBACK_DURATION;
			bumpKnockbackDuration   = GameSettings.MONSTER_BUMP_KNOCKBACK_DURATION;
			hurtInvincibleDuration  = GameSettings.MONSTER_HURT_INVINCIBLE_DURATION;
			hurtFlickerDuration     = GameSettings.MONSTER_HURT_FLICKER_DURATION;
			isKnockbackable         = true;
			
			// Monster
			contactDamage	= 1;
			color			= MonsterColor.Red;
			isGaleable		= true;
			isBurnable		= true;
			isStunnable		= true;
			softKill		= false;
			ignoreZPosition	= false;
		}

		protected void SetDefaultReactions() {
			Interactions.ClearReactions();

			// Weapon interations
			Interactions.SetReaction(InteractionType.Sword,
				SenderReactions.Intercept, Reactions.DamageByLevel(1, 2, 3));
			Interactions.SetReaction(InteractionType.SwordSpin,
				Reactions.Damage2);
			Interactions.SetReaction(InteractionType.BiggoronSword,
				Reactions.Damage3);
			Interactions.SetReaction(InteractionType.Shield,
				SenderReactions.Bump, Reactions.Bump);
			Interactions.SetReaction(InteractionType.Shovel,
				Reactions.Bump);
			Interactions.SetReaction(InteractionType.Parry,
				Reactions.Parry);
			Interactions.SetReaction(InteractionType.Bracelet,
				Reactions.None);

			// Seed interations
			Interactions.SetReaction(InteractionType.EmberSeed,
				SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.ScentSeed,
				SenderReactions.Intercept, Reactions.SilentDamage);
			Interactions.SetReaction(InteractionType.PegasusSeed,
				SenderReactions.Intercept, Reactions.Stun);
			Interactions.SetReaction(InteractionType.GaleSeed,
				SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.MysterySeed,
				Reactions.MysterySeed);

			// Projectile interations
			Interactions.SetReaction(InteractionType.Arrow,
				SenderReactions.Destroy, Reactions.Damage);
			Interactions.SetReaction(InteractionType.SwordBeam,
				SenderReactions.Destroy, Reactions.Damage);
			Interactions.SetReaction(InteractionType.RodFire,
				SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.Boomerang,
				SenderReactions.Intercept, Reactions.Stun);
			Interactions.SetReaction(InteractionType.SwitchHook,
				Reactions.SwitchHook);

			// Environment interations
			Interactions.SetReaction(InteractionType.Fire,
				Reactions.Burn);
			Interactions.SetReaction(InteractionType.Gale,
				Reactions.Gale);
			Interactions.SetReaction(InteractionType.BombExplosion,
				Reactions.Damage);
			Interactions.SetReaction(InteractionType.ThrownObject,
				Reactions.Damage);
			Interactions.SetReaction(InteractionType.MineCart,
				Reactions.SoftKill);
			Interactions.SetReaction(InteractionType.MagnetBall,
				Reactions.Kill); // TODO: Confirm  this
			Interactions.SetReaction(InteractionType.Block,
				Reactions.Damage2);

			// Player interations
			Interactions.SetReaction(InteractionType.PlayerContact,
				delegate(Entity sender, EventArgs args)
			{
				if (!IsStunned)
					OnTouchPlayer(sender, args);
			});
		}


		//-----------------------------------------------------------------------------
		// Monster States and Behavior
		//-----------------------------------------------------------------------------

		// Begin the given monster state.
		public void BeginState(MonsterState newState) {
			if (state != newState) {
				if (state != null) {
					state.End(newState);
				}
				previousState = state;
				state = newState;
				newState.Begin(this, previousState);
			}
		}

		public void BeginNormalState() {
			BeginState(new MonsterNormalState());
		}

		public virtual void UpdateAI() {}

		public void BeginSpawnState(int duration = GameSettings.MONSTER_SPAWN_STATE_DURATION) {
			BeginState(new MonsterSpawnState(duration));
		}


		//-----------------------------------------------------------------------------
		// Monster Reactions
		//-----------------------------------------------------------------------------

		public virtual void OnStun() {}

		public virtual void OnBurn() {}

		public virtual void OnBurnComplete() {}

		public virtual void OnElectrocute() {}

		public virtual void OnElectrocuteComplete() {}

		public void SoftKill() {
			softKill = true;
			Kill();
		}

		public bool Stun(int damage = 0) {
			if (isStunnable && ((state is MonsterNormalState) || (state is MonsterStunState))) {
				OnStun();
				AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
				BeginState(new MonsterStunState(GameSettings.MONSTER_STUN_DURATION));
				if (damage > 0) {
					DamageInfo damageInfo = new DamageInfo(damage);
					damageInfo.ApplyKnockback = false;
					Hurt(damageInfo);
				}
				return true;
			}
			return false;
		}

		public bool EnterGale(EffectGale gale) {
			if (isGaleable && ((state is MonsterNormalState) || (state is MonsterStunState))) {
				BeginState(new MonsterGaleState(gale));
				return true;
			}
			return false;
		}

		public virtual bool Burn(int damage) {
			if (!IsInvincible && isBurnable &&
				((state is MonsterNormalState) || (state is MonsterStunState))) {
				OnBurn();
				BeginState(new MonsterBurnState(damage));
				return true;
			}
			return false;
		}

		public virtual void OnTouchPlayer(Entity sender, EventArgs args) {
			Player player = (Player) sender;
			player.Hurt(contactDamage, Center);
		}

		public virtual void OnSeedHit(SeedEntity seed) {
			seed.TriggerMonsterReaction(this);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		protected virtual void FacePlayer() {
			Vector2F lookVector = RoomControl.Player.Center - Center;
			direction = Direction.FromVector(lookVector);
		}

		public virtual bool CanSpawnAtLocation(Point2I location) {
			Vector2F spawnPosition =
				(location * GameSettings.TILE_SIZE) +
				new Vector2F(8, 8) - centerOffset;

			// Check for solid tiles
			Tile tile = RoomControl.GetTopTile(location);
			if (tile != null && (tile.IsSolid || tile.IsHoleWaterOrLava))
				return false;

			// Check for touching the player
			if (Physics.IsPlaceMeetingEntity(spawnPosition,
				RoomControl.Player, CollisionBoxType.Soft))
				return false;

			return true;
		}

		public virtual Vector2F GetRandomSpawnLocation() {

			List<Vector2F> locations = new List<Vector2F>();

			for (int x = 0; x < RoomControl.Room.Width; x++) {
				for (int y = 0; y < RoomControl.Room.Height; y++) {
					Vector2F spawnPosition =
						(new Point2I(x, y) * GameSettings.TILE_SIZE) +
						new Vector2F(8, 8) - centerOffset;
					if (CanSpawnAtLocation(new Point2I(x, y)))
						locations.Add(spawnPosition);
				}
			}

			if (locations.Count == 0)
				return position;
			return GRandom.Choose(locations);
		}

		public virtual void CreateDeathEffect() {
			Effect explosion = new Effect(
				GameData.ANIM_EFFECT_MONSTER_EXPLOSION,
				DepthLayer.EffectMonsterExplosion);
			AudioSystem.PlaySound(GameData.SOUND_MONSTER_DIE);
			RoomControl.SpawnEntity(explosion, Center);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			color = (MonsterColor) Properties.GetInteger("color", (int) color);

			health = healthMax;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_OCTOROK);

			// Begin the default monster state
			BeginNormalState();
			previousState = state;
		}

		public override void Die() {
			CreateDeathEffect();
			if (!softKill)
				Properties.Set("dead", true);
			base.Die();
		}

		public override void OnHurt(DamageInfo damage) {
			if (damage.PlaySound)
				AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
		}

		public override void OnFallInHole() {
			// Begin the fall-in-hole state (slipping into a hole)
			if (state is MonsterNormalState) {
				BeginState(new MonsterFallInHoleState());
			}
		}

		private void CollideMonsterAndPlayer() {
			/*
			Player player = RoomControl.Player;
			
			float tempZPosition = ZPosition;
			if (ignoreZPosition)
				ZPosition = 0;

			// Check for minecart interactions
			// TODO: Move this trigger into PlayerMineCartState
			//if (!isPassable && player.IsInMinecart &&
			//	physics.IsCollidingWith(player, CollisionBoxType.Soft)) {
			//	TriggerInteraction(InteractionType.MineCart, player);
			//	if (IsDestroyed)
			//		return;
			//}

			if (isPassable || player.IsPassable || GMath.Abs(player.ZPosition - zPosition) > 10) // TODO: magic number
				return;

			IEnumerable<UnitTool> monsterTools = EquippedTools.Where(
				t => t.IsPhysicsEnabled && t.IsSwordOrShield);
			IEnumerable<UnitTool> playerTools = player.EquippedTools.Where(
				t => t.IsPhysicsEnabled && t.IsSwordOrShield);

			// 1. (M-M) MonsterTools to PlayerTools
			// 2. (M-1) MonsterTools to Player
			// 4. (M-1) PlayerTools to Monster
			// 2. (1-1) Monster to Player

			// Collide my tools with the player's tools
			if (!IsStunned) {
				foreach (UnitTool monsterTool in monsterTools) {
					foreach (UnitTool playerTool in playerTools) {
						if (monsterTool.PositionedCollisionBox.Intersects(playerTool.PositionedCollisionBox)) {
							Vector2F contactPoint = playerTool.PositionedCollisionBox.Center;

							TriggerInteraction(InteractionType.Parry, player, new ParryInteractionArgs() {
								ContactPoint    = contactPoint,
								MonsterTool     = monsterTool,
								SenderTool      = playerTool
							});

							playerTool.OnParry(this, contactPoint);

							RoomControl.SpawnEntity(new EffectCling(), contactPoint);

							return;
						}
					}
				}
			}

			// Collide my tools with the player
			bool parry = false;
			if (!player.IsInvincible && player.IsDamageable && !IsStunned) {
				foreach (UnitTool tool in monsterTools) {
					if (player.Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
						player.Hurt(contactDamage, Center);
						parry = true;
						break;
					}
				}
			}

			// Collide with the player's tools
			foreach (UnitTool tool in playerTools) {
				if (Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
					tool.OnHitMonster(this);
					parry = true;
					break;
				}
			}
			bool parry = false;

			// Check collisions with player
			if (!parry && !IsStunned &&
				physics.IsCollidingWith(player, CollisionBoxType.Soft)) {
				Interactions.Trigger(InteractionType.PlayerContact, player);
			}

			if (ignoreZPosition)
				ZPosition = tempZPosition;
			*/
		}

		public override void Update() {
			// Update the current monster state
			state.Update();

			// Collide player and monster and their tools
			CollideMonsterAndPlayer();

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			state.DrawUnder(g);
			base.Draw(g);
			state.DrawOver(g);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int ContactDamage {
			get { return contactDamage; }
			set { contactDamage = value; }
		}

		public bool IsStunned {
			get { return (state is MonsterStunState); }
		}

		public MonsterState CurrentState {
			get { return state; }
		}

		public MonsterColor Color {
			get { return color; }
			set {
				color = value;
				Graphics.ColorDefinitions.SetAll(
					GameData.MONSTER_COLOR_DEFINITION_MAP[(int) color]);
			}
		}

		/// <summary>True if this monster is not counted towards clearing a room.</summary>
		public bool IgnoreMonster {
			get { return Properties.Get("ignore_monster", false); }
			set { Properties.Set("ignore_monster", value); }
		}

		/// <summary>True if this monster needs to be killed in order to clear the room.</summary>
		public bool NeedsClearing {
			get { return !IgnoreMonster && IsAlive; }
		}
	}
}
