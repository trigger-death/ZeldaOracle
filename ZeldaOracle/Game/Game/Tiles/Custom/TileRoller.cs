﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileRoller : Tile {

		private int returnTimer;

		private int startPosition;

		// The leader roller that commands all the other tiles.
		private TileRoller firstRoller;
		// The next roller away from the leader.
		private TileRoller nextRoller;

		private int pushTimer;
		private bool pushed;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileRoller() {
			// TODO: Rollers can't be sword-stabbed, aren't pushable diagonally, and are only solid to the player.
		}

		//-----------------------------------------------------------------------------
		// Rolling
		//-----------------------------------------------------------------------------

		// Pushes the roller.
		public void PushRoller(int direction) {
			if (((IsVertical && Directions.IsVertical(direction)) || (!IsVertical && Directions.IsHorizontal(direction))) && nextRoller != null)
				nextRoller.PushRoller(direction);

			Graphics.PlayAnimation(TileData.SpriteList[1].Animation);
			Graphics.SubStripIndex = (direction <= 1 ? 0 : 1);

			// Only the main roller should start the pushback.
			if (firstRoller == this)
				returnTimer = 60;
			
			if (base.Move(direction, 1, GameSettings.TILE_ROLLER_MOVE_SPEED)) {
				if (firstRoller == this)
					AudioSystem.PlaySound(GameData.SOUND_BLUE_ROLLER);
			}
		}

		// Makes sure all rollers in the group can be pushed in the same direction.
		private bool CanPushRoller(int direction) {
			if (IsMoving)
				return false;

			// Make sure were not pushing out of bounds.
			Point2I newLocation = Location + Directions.ToPoint(direction);
			if (!RoomControl.IsTileInBounds(newLocation))
				return false;

			// Make sure there are no obstructions.
			int newLayer;
			if (IsMoveObstructed(direction, out newLayer))
				return false;

			if ((IsVertical && Directions.IsVertical(direction)) || (!IsVertical && Directions.IsHorizontal(direction)))
				return (nextRoller != null ? nextRoller.CanPushRoller(direction) : true);
			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			startPosition = (IsVertical ? Location.Y : Location.X);
			returnTimer = 0;
			TileRoller roller = this;
			do {
				firstRoller = roller;
				// Don't look any further, this is automatically the first roller.
				if (roller.Properties.GetBoolean("first_roller"))
					break;
				roller = RoomControl.GetTopTile(roller.Location + Directions.ToPoint(IsVertical ? Directions.Left : Directions.Up)) as TileRoller;
			} while (roller != null);

			nextRoller = RoomControl.GetTopTile(Location + Directions.ToPoint(IsVertical ? Directions.Right : Directions.Down)) as TileRoller;
			// Don't include the next roller if it's the start of a new group.
			if (nextRoller != null && nextRoller.Properties.GetBoolean("first_roller"))
				nextRoller = null;

			pushed = false;
			pushTimer = 0;
			
			Graphics.PlayAnimation(TileData.SpriteList[1].Animation);
			Graphics.AnimationPlayer.SkipToEnd();
		}

		public override bool OnPush(int direction, float movementSpeed) {
			return false;
		}

		public override void OnPushing(int direction) {
			int currentPosition = (IsVertical ? Location.Y : Location.X);
			if (!IsMoving && RoomControl.GameControl.Inventory.IsWeaponButtonDown(RoomControl.GameControl.Inventory.GetItem("item_bracelet"))) {
				bool pushableDirection = false;

				switch (direction) {
				case Directions.Right:	pushableDirection = !IsVertical && currentPosition >= startPosition; break;
				case Directions.Up:		pushableDirection =  IsVertical && currentPosition <= startPosition; break;
				case Directions.Left:	pushableDirection = !IsVertical && currentPosition <= startPosition; break;
				case Directions.Down:	pushableDirection =  IsVertical && currentPosition >= startPosition; break;
				}

				if (pushableDirection) {
					firstRoller.returnTimer = 60;
					pushed = true;
					if (pushTimer == PushDelay) {
						if (firstRoller.CanPushRoller(direction))
							firstRoller.PushRoller(direction);
					}
					else {
						pushTimer++;
					}
				}
			}
		}

		// Called when the tile is pushed into a hole.
		public override void OnFallInHole() {
		}

		// Called when the tile is pushed into water.
		public override void OnFallInWater() {
		}

		// Called when the tile is pushed into lava.
		public override void OnFallInLava() {
		}

		public override void Update() {
			base.Update();

			if (!IsMoving) {
				if (!pushed)
					pushTimer = 0;
				int currentPosition = (IsVertical ? Location.Y : Location.X);
				if (startPosition != currentPosition && firstRoller == this && returnTimer > 0) {
					int direction;
					if (currentPosition < startPosition)
						direction = (IsVertical ? Directions.Down : Directions.Right);
					else
						direction = (IsVertical ? Directions.Up : Directions.Left);

					if (!CanPushRoller(direction)) {
						// Reset the return timer while it can't roll back.
						returnTimer = 60;
					}
					else {
						returnTimer--;
						if (returnTimer == 0)
							PushRoller(direction);
					}
				}
			}
			pushed = false;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private bool IsVertical {
			get { return Properties.GetBoolean("vertical", false); }
		}
	}
}
