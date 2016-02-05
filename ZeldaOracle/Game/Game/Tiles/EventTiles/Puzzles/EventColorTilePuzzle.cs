﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Tiles.EventTiles.Puzzles {

	public class EventColorTilePuzzle : EventTile {

		private PuzzleColor[,] solution;
		private Rectangle2I solutionReferenceArea;
		private Rectangle2I solutionArea;
		private bool isSolved;
		private bool hasBeenSolvedAlready;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EventColorTilePuzzle() {

		}


		//-----------------------------------------------------------------------------
		// Puzzle Methods
		//-----------------------------------------------------------------------------

		private PuzzleColor GetTileColorAtLocation(Point2I location) {
			foreach (Tile tile in RoomControl.TileManager.GetTilesAtLocation(location, TileLayerOrder.HighestToLowest)) {
				if (!tile.IsMoving && (tile is IColoredTile))
					return ((IColoredTile) tile).Color;
			}
			return PuzzleColor.None;
		}

		private bool CheckSolved() {
			for (int y = 0; y < solutionArea.Height; y++) {
				for (int x = 0; x < solutionArea.Width; x++) {
					Point2I tileLocation = solutionArea.Point + new Point2I(x, y);
					if (GetTileColorAtLocation(tileLocation) != solution[x, y])
						return false;
				}
			}
			return true;
		}

		private bool CreateSolutionReference() {
			solution = new PuzzleColor[solutionReferenceArea.Width, solutionReferenceArea.Height];
			for (int y = 0; y < solutionReferenceArea.Height; y++) {
				for (int x = 0; x < solutionReferenceArea.Width; x++) {
					Point2I tileLocation = solutionReferenceArea.Point + new Point2I(x, y);
					solution[x, y] = GetTileColorAtLocation(tileLocation);
				}
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			base.Initialize();

			solutionReferenceArea	= new Rectangle2I(3, 4, 3, 3);
			solutionArea			= new Rectangle2I(9, 4, 3, 3);
			CreateSolutionReference();
			
			/*solution = new PuzzleColor[3, 3] {
				{ PuzzleColor.Red, PuzzleColor.Red, PuzzleColor.Red },
				{ PuzzleColor.Yellow, PuzzleColor.Red, PuzzleColor.Red },
				{ PuzzleColor.Blue, PuzzleColor.Red, PuzzleColor.Red },
			};*/


			hasBeenSolvedAlready = false;
			isSolved = false;
		}

		public override void Update() {

			// Check if solved.
			bool checkSolve = CheckSolved();
			bool solveOnce = Properties.GetBoolean("solve_once", false);

			if (checkSolve != isSolved && (!solveOnce || !hasBeenSolvedAlready)) {
				isSolved = checkSolve;

				if (isSolved) {
					hasBeenSolvedAlready = true;
					AudioSystem.PlaySound(GameData.SOUND_SECRET);
					RoomControl.GameControl.FireEvent(this, "solved");
				}
				else {
					RoomControl.GameControl.FireEvent(this, "unsolve");
				}
			}

			base.Update();
		}
	}
}