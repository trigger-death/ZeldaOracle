﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemCane : ItemWeapon {
		
		private TileData somariaBlockTileData;
		private TileSomariaBlock somariaBlockTile;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemCane() {
			this.id				= "item_cane";
			this.name			= new string[] { "Cane of Somaria" };
			this.description	= new string[] { "Used to create blocks." };
			this.maxLevel		= Item.Level1;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWhileInHole;
			this.sprite			= new Sprite[] { GameData.SPR_ITEM_ICON_CANE };

			this.somariaBlockTile		= null;
			this.somariaBlockTileData	= null;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			Player.SwingCaneState.Weapon = this;
			Player.BeginState(Player.SwingCaneState);
		}

		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);

			somariaBlockTile = null;

			// Create the somaria block tile data.
			somariaBlockTileData = new TileData(
				typeof(TileSomariaBlock), TileFlags.Solid,
				TileSpecialFlags.Movable |
				TileSpecialFlags.Cuttable |
				TileSpecialFlags.Bombable |
				TileSpecialFlags.Boomerangable |
				TileSpecialFlags.Pickupable);
			somariaBlockTileData.CollisionModel	= GameData.MODEL_BLOCK;
			somariaBlockTileData.Sprite			= GameData.SPR_TILE_SOMARIA_BLOCK;
			somariaBlockTileData.BreakAnimation	= GameData.ANIM_EFFECT_SOMARIA_BLOCK_DESTROY;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileSomariaBlock SomariaBlockTile {
			get {
				if (somariaBlockTile != null && (somariaBlockTile.IsDestroyed || somariaBlockTile.RoomControl != Player.RoomControl))
					return null;
				return somariaBlockTile;
			}
			set { somariaBlockTile = value; }
		}

		public TileData SomariaBlockTileData {
			get { return somariaBlockTileData; }
		}
	}
}