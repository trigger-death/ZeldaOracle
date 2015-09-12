﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game
{
	class GameSettings
	{

		public const int		TILE_SIZE		= 16;	// Tile size in texels.
		public const int		SCREEN_WIDTH	= 160;
		public const int		SCREEN_HEIGHT	= 144;
		public const int		VIEW_WIDTH		= 160;
		public const int		VIEW_HEIGHT		= 128;
		public static readonly Point2I	SCREEN_SIZE		= new Point2I(SCREEN_WIDTH, SCREEN_HEIGHT);
		public static readonly Point2I	VIEW_SIZE		= new Point2I(VIEW_WIDTH, VIEW_HEIGHT);
		public static readonly Rectangle2I	SCREEN_BOUNDS	= new Rectangle2I(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

		public const float		PLAYER_MOVE_SPEED		= 64;	// Pixels per second.
		public const float		PLAYER_JUMP_SPEED		= 108;
		public const float		GRAVITY_ACCELERATION	= 450;
		
		public static readonly Point2I	ROOM_SIZE_SMALL	= new Point2I(10, 8);
		public static readonly Point2I	ROOM_SIZE_LARGE	= new Point2I(15, 11);

		public const int		DEFAULT_TILE_LAYER_COUNT = 2;
	}
}
