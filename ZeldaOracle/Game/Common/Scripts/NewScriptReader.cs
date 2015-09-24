﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Scripts {

	public class ScriptCommand {
		private string name;
		private Action<CommandParam> action;

		public ScriptCommand(string name, Action<CommandParam> action) {
			this.name	= name;
			this.action	= action;
		}

		public string Name {
			get { return name; }
		}

		public Action<CommandParam> Action {
			get { return action; }
		}
	}

	
	/** <summary>
	 * A script reader is an abstract object that
	 * is meant to be implemented to be able to
	 * interpret text files written in a certain syntax.
	 * </summary> */
	public class NewScriptReader {

		private int lineIndex;
		private int charIndex;
		private string word;
		private string line;

		private List<ScriptCommand> commands;

		private CommandParam parameter;
		private CommandParam parameterParent;
		private CommandParam parameterRoot;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public NewScriptReader() {
			parameter		= null;
			parameterRoot	= null;
			commands		= new List<ScriptCommand>();
		}
		

		//-----------------------------------------------------------------------------
		// Commands
		//-----------------------------------------------------------------------------

		protected void AddCommand(string name, Action<CommandParam> action) {
			ScriptCommand command = new ScriptCommand(name, action);
			commands.Add(command);
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		// Begins reading the script.
		protected virtual void BeginReading() {}

		// Ends reading the script.
		protected virtual void EndReading() {}

		// Reads a line in the script as a command.
		protected virtual bool ReadCommand(string commandName, CommandParam parameters) {
			for (int i = 0; i < commands.Count; i++) {
				if (String.Compare(commands[i].Name, commandName,
					StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					commands[i].Action(parameters);
					return true;
				}
			}

			return false;
		}

		private void ThrowParseError(string message, bool showCarret = true) {
			
			Console.WriteLine("Error in 'animations.conscript' at Line " + (lineIndex + 1) + ", Column " + (charIndex + 1) + ":");
			Console.WriteLine(message);

			Console.WriteLine(line);
			if (showCarret) {
				for (int i = 0; i < charIndex; i++)
					Console.Write(' ');
				Console.WriteLine('^');
			}
		}


		//-----------------------------------------------------------------------------
		// Parsing methods
		//-----------------------------------------------------------------------------

		// Return true if the given character is allowed for keywords
		private bool IsValidKeywordCharacter(char c) {
			string validKeywordSymbols = "$_.-+";

			if (Char.IsLetterOrDigit(c))
				return true;
			if (validKeywordSymbols.IndexOf(c) >= 0)
				return true;
			return false;
		}

		// Attempt to add a completed word, if it is not empty.
		protected void CompleteWord(bool completeIfEmpty = false) {
			if (word.Length > 0 || completeIfEmpty)
				AddParam();
			word = "";
		}

		protected void PrintParementers(CommandParam param) {
			CommandParam p = param.Children;
			while (p != null) {
				if (p.Type == CommandParamType.Array) {
					Console.Write("(");
					PrintParementers(p);
					Console.Write(")");
				}
				else
					Console.Write(p.Str);

				p = p.NextParam;
				if (p != null)
					Console.Write(", ");
			}
		}

		protected void CompleteStatement() {

			if (parameterRoot.Children != null) {
				string commandName = parameterRoot.Children.Str;
				parameterRoot.Children = parameterRoot.Children.NextParam;
				parameterRoot.Count--;
				ReadCommand(commandName, parameterRoot);
			}

			//PrintParementers(parameterRoot);
			
			parameterParent	= new CommandParam("");
			parameterParent.Type = CommandParamType.Array;
			parameterRoot	= parameterParent;
			parameter		= null;
		}

		private CommandParam AddParam() {
			CommandParam newParam = new CommandParam(word);
			if (parameter == null)
				parameterParent.Children = newParam;
			else
				parameter.NextParam = newParam;
			parameterParent.Count++;
			newParam.Parent = parameterParent;
			parameter = newParam;
			return newParam;
		}

		// Read a single line of the script.
		protected void ReadLine(string line) {
			word = "";
			bool quotes = false;
			
			charIndex = 0;

			parameterParent	= new CommandParam("");
			parameterParent.Type = CommandParamType.Array;
			parameterRoot	= parameterParent;
			parameter		= null;

			// Parse line character by character.
			for (int i = 0; i < line.Length; i++) {
				char c = line[i];
				charIndex = i;

				// Parse quotes.
				if (quotes) {
					// Closing quotes.
					if (c == '\"') {
						quotes = false;
						CompleteWord(true);
					}
					else
						word += c;
				}

				// Whitespace.
				else if (c == ' ' || c == '\t')
					CompleteWord();

				// Commas.
				else if (c == ',')
					CompleteWord();

				// Semicolons.
				else if (c == ';') {
					CompleteWord();
					CompleteStatement();
				}

				// Single-line comment.
				else if (c == '#') {
					break; // Ignore the rest of the line.
				}

				// Opening quotes.
				else if (word.Length == 0 && c == '\"')
					quotes = true;
					
				// Opening parenthesis.
				else if (word.Length == 0 && c == '(') {
					parameterParent = AddParam();
					parameterParent.Type = CommandParamType.Array;
					parameter = null;
				}
					
				// Closing parenthesis.
				else if (c == ')') {
					CompleteWord();
					if (parameterParent == parameterRoot) {
						ThrowParseError("Unexpected symbol ')'");
					}
					else {
						parameter = parameterParent;
						parameterParent = parameterParent.Parent;
					}
				}

				// Valid keyword character.
				else if (IsValidKeywordCharacter(c))
					word += c;

				// Error: Unexpected character.
				else
					ThrowParseError("Unexpected symbol '" + c + "'");
			}

			charIndex++;

			// Make sure quotes are closed and statements are ended.
			if (quotes)
				ThrowParseError("Expected \"");
			//else if (words.Count > 0 || word.Length > 0)
			//	ThrowParseError("Expected ;");
		}
		
		// Parse and interpret the given text stream as a script, line by line.
		public void ReadScript(StreamReader reader) {
			BeginReading();
			lineIndex = 0;
			while (!reader.EndOfStream) {
				line = reader.ReadLine();
				ReadLine(line);
				lineIndex++;
			}
			EndReading();
		}

	}
}