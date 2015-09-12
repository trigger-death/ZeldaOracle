﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.GameStates {
	
	public class GameStateQueue : GameState {
	
        private List<GameState> states;
		private int stateIndex;
        

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

        public GameStateQueue(params GameState[] states) {
            this.states = new List<GameState>();
            this.states.AddRange(states);
        }


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

        public void Add(GameState state) {
            states.Add(state);
        }

		public void NextState() {
            stateIndex++;

            if (CurrentState != null) {
                CurrentState.Begin(gameManager);
                if (!CurrentState.IsActive)
                    NextState();
            }
            else
                End();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

        public override void OnBegin() {
            stateIndex = 0;

            if (CurrentState != null)
            {
                CurrentState.Begin(gameManager);
                if (!CurrentState.IsActive)
                    NextState();
            }
            else
                End();
        }

        public override void Update(float timeDelta) {
            if (CurrentState != null)
            {
                CurrentState.Update(timeDelta);
                if (!CurrentState.IsActive)
                    NextState();
            }
            else
                End();
        }

        public override void Draw(Graphics2D g) {
			if (CurrentState != null)
				CurrentState.Draw(g);
        }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

        public GameState CurrentState {
            get {
                if (stateIndex >= states.Count)
                    return null;
                return states[stateIndex];
            }
        }
	}
}