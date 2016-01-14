﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Tiles {

	public class TileButton : Tile, ZeldaAPI.Button {

		private bool isPressed;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileButton() {

		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			isPressed = Properties.Get("pressed", false);
			//Properties.Set("sprite_index", (isPressed ? 1 : 0));

			if (isPressed)
				CustomSprite = GameData.SPR_TILE_BUTTON_DOWN;
			else
				CustomSprite = GameData.SPR_TILE_BUTTON_UP;
		}

		public override void Update() {
			base.Update();

			bool isDown = false;

			// Check if the player is on top of this button.
			Rectangle2F pressRect = new Rectangle2F(0, 5, 16, 16);
			pressRect.Point += Position;
			Player player = RoomControl.Player;
			if (pressRect.Contains(player.Position))
				isDown = true;

			// Check if a tile is on top of this button.
			for (int i = Layer; i < RoomControl.Room.LayerCount; i++) {
				Tile tile = RoomControl.GetTile(Location, i);

				if (tile != null && tile.IsSolid) {
					isDown = true;
				}
			}

			// Check if the pressed state has changed.
			bool releasable = Properties.GetBoolean("releasable", true);
			if (isPressed != isDown && (isDown || releasable)) {
				isPressed = isDown;

				// Set the pressed state.
				if (Properties.Get("remember_state", false))
					Properties.SetBase("pressed", isPressed);
				else
					Properties.Set("pressed", isPressed);

				// Fire the event.
				if (isPressed) {
					CustomSprite = GameData.SPR_TILE_BUTTON_DOWN;
					GameControl.FireEvent(this, "on_press", this);
				}
				else {
					CustomSprite = GameData.SPR_TILE_BUTTON_UP;
					GameControl.FireEvent(this, "on_release", this);
				}
			}
		}
	}
}