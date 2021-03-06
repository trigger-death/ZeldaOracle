﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemFeather : ItemWeapon {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemFeather() {
			Flags =
				WeaponFlags.UsableWhileJumping |
				WeaponFlags.UsableWithSword;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Check if the player is allowed to jump.
		public override bool IsUsable() {
			return (!Player.StateParameters.ProhibitJumping &&
					!Player.IsInMinecart &&
					!Player.IsUnderwater);
		}

		// Jump when on ground.
		public override bool OnButtonPress() {
			if (Player.IsOnGround && !Player.Physics.IsInHole) {
				Player.Movement.Jump();
				return true;
			}
			return false;
		}

		// Deplay cape when in air.
		public override void OnButtonDown() {
			if (Player.IsInAir && Level == ItemWeapon.Level2) {
				Player.Movement.DeployCape();
			}
		}
	}
}
