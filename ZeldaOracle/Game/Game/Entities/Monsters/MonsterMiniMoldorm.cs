﻿using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterMiniMoldorm : Monster {
		
		private const int NUM_BODY_PARTS = 2;
		private const int MAX_HISTORY = 20;
		private Vector2F[] bodyPositions;
		private ISprite[] bodySprites;
		private List<Vector2F> positionHistory;
		private int timer;
		private float rotationSpeed;
		private float moveSpeed;
		private Vector2F moveVector;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterMiniMoldorm() {
			positionHistory = new List<Vector2F>();
			bodyPositions = new Vector2F[NUM_BODY_PARTS];
			bodySprites = new ISprite[NUM_BODY_PARTS];
			bodySprites[0] = GameData.SPR_MONSTER_MINI_MOLDORM_BODY_SEGMENT_LARGE;
			bodySprites[1] = GameData.SPR_MONSTER_MINI_MOLDORM_BODY_SEGMENT_SMALL;

			// General
			MaxHealth		= 4;
			ContactDamage	= 2;
			Color			= MonsterColor.Green;
			
			// Movement
			moveSpeed = 1.0f;

			// Graphics
			Graphics.DrawOffset			= new Point2I(-8, -8);
			centerOffset				= new Point2I(0, 0);
			syncAnimationWithDirection	= false;

			// Physics
			Physics.HasGravity				= false;
			Physics.IsDestroyedInHoles		= false;
			Physics.CollisionBox			= new Rectangle2F(-6, -6, 12, 12);
			Physics.DisableSurfaceContact	= true;
			Physics.ReboundSolid			= true;
			Physics.ReboundRoomEdge			= true;

			// Interactions
			Interactions.InteractionBox = Physics.CollisionBox.Inflated(-2, -2);
			// Projectile Reactions
			Reactions[InteractionType.Gale].Set(SenderReactions.Intercept);
			Reactions[InteractionType.GaleSeed].Set(SenderReactions.Intercept);
			Reactions[InteractionType.PegasusSeed].Set(	SenderReactions.Intercept);
			Reactions[InteractionType.ScentSeed].Set(SenderReactions.Intercept);
			Reactions[InteractionType.Fire].Set(SenderReactions.Intercept);
			Reactions[InteractionType.RodFire].Set(SenderReactions.Intercept);
			Reactions[InteractionType.Boomerang].Set(SenderReactions.Intercept)
				.Add(MonsterReactions.ClingEffect);
			Reactions[InteractionType.SwitchHook].Set(SenderReactions.Intercept)
				.Add(MonsterReactions.Damage);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			Direction = Direction.Up;

			// Start moving in a random angle
			float randomAngle = GRandom.NextFloat(GMath.FullAngle);
			moveVector = Vector2F.FromPolar(randomAngle);
			rotationSpeed = 0.05f;
			if (GRandom.NextBool())
				rotationSpeed *= -1.0f;

			timer = 0;
			positionHistory.Clear();
			for (int i = 0; i < NUM_BODY_PARTS; i++)
				bodyPositions[i] = position;
			for (int i = 0; i < MAX_HISTORY; i++)
				positionHistory.Add(position);
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_MINI_MOLDORM_HEAD);
		}

		public override void UpdateAI() {
			// Check for rebound collisions to update movement vector.
			// We cannot rely on using Physics.Velocity because facing direction does 
			// not always sync with physics velocity.
			foreach (Direction dir in Direction.Range) {
				if (physics.IsCollidingInDirection(dir))
					moveVector[dir.Axis] = -moveVector[dir.Axis];
			}

			// Rotate velocity
			float angle = rotationSpeed;
			float cosAngle = GMath.Cos(angle);
			float sinAngle = GMath.Sin(angle);
			float x = (moveVector.X * cosAngle) - (moveVector.Y * sinAngle);
			float y = (moveVector.Y * cosAngle) + (moveVector.X * sinAngle);
			moveVector.X = x;
			moveVector.Y = y;
			moveVector = moveVector.Normalized * moveSpeed;

			physics.Velocity = moveVector;
			Graphics.SubStripIndex = Angle.FromVector(moveVector);

			// Reverse rotation direction regularly
			timer++;
			if (timer > 60 && GRandom.NextInt(60) == 0) {
				rotationSpeed *= -1.0f;
				timer = 0;
			}
		}

		public override void Update() {
			base.Update();
			
			for (int i = 0; i < NUM_BODY_PARTS; i++) {
				bodyPositions[i] = positionHistory[MAX_HISTORY - ((i + 1) * 7)];
			}

			positionHistory.Add(position);
			positionHistory.RemoveAt(0);
		}

		public override void Draw(RoomGraphics g) {
			// Draw body segments
			SpriteSettings drawSettings = new SpriteSettings() {
				Colors = Graphics.ModifiedColorDefinitions
			};
			for (int i = NUM_BODY_PARTS - 1; i >= 0; i--) {
				g.DrawSprite(bodySprites[i], drawSettings,
					bodyPositions[i], Graphics.DepthLayer);
			}

			// Draw head/eyes
			base.Draw(g);
		}
	}
}
