﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

using XnaColor = Microsoft.Xna.Framework.Color;
using Color = ZeldaOracle.Common.Graphics.Color;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	public struct Palette4C {
		Color[] colors;

		public Palette4C(Color[] colors) {
			this.colors = colors;
		}

		public Color this[int index] {
			get { return colors[index]; }
		}
	}

	/// <summary>The subtypes to use for looking up colors in color groups.</summary>
	public enum LookupSubtypes {
		/// <summary>Used for tile and entities.</summary>
		Light		= 0,
		/// <summary>Used for tiles only.</summary>
		Medium		= 1,
		/// <summary>Used for entities only.</summary>
		Transparent = 1,
		/// <summary>Used for tile and entities.</summary>
		Dark		= 2,
		/// <summary>Used for tile and entities.</summary>
		Black		= 3,
		/// <summary>Used to assign all tile and entity colors in the group.</summary>
		All
	}

	public enum LookupResult {
		Success,
		InfiniteLoop,
		MaxLookupDepth
	}

	public class Palette {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------
		
		private struct PaletteColor {
			public static readonly PaletteColor Undefined = new PaletteColor();

			private Color? color;

			public PaletteColor(Color color) {
				this.color      = color;
			}

			public bool IsUndefined {
				get { return !color.HasValue; }
			}
			public Color Color {
				get {
					if (color.HasValue)
						return color.Value;
					return Color.Black;
				}
				set { color = value; }
			}
		}

		private struct LookupPair {
			public static readonly LookupPair Undefined = new LookupPair();

			public string Name { get; set; }
			public LookupSubtypes Subtype { get; set; }

			public LookupPair(string name, LookupSubtypes subtype) {
				this.Name       = name;
				this.Subtype    = subtype;
			}

			public bool IsUndefined {
				get { return Name == null; }
			}
		}


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The total number of colors in a palette.</summary>
		public static readonly Point2I Dimensions = new Point2I(256, 256);

		/// <summary>The default color group for tiles.</summary>
		public static readonly Color[] DefaultTile = new Color[PaletteDictionary.ColorGroupSize] {
			Color.ToGBCColor(Color.White),
			Color.ToGBCColor(0.66f, 0.66f, 0.66f),
			Color.ToGBCColor(0.33f, 0.33f, 0.33f),
			Color.Black
		};

		/// <summary>The default color group for enitities.</summary>
		public static readonly Color[] DefaultEntity = new Color[PaletteDictionary.ColorGroupSize] {
			Color.ToGBCColor(0.66f, 0.66f, 0.66f),
			Color.Transparent,
			Color.ToGBCColor(0.33f, 0.33f, 0.33f),
			Color.Black
		};

		/// <summary>The maximum allowed lookups before reaching a defined color.</summary>
		public const int MaxLookupDepth = 10;

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private PaletteDictionary dictionary;
		private Texture2D paletteTexture;
		private Dictionary<string, PaletteColor[]> colorGroups;
		private Dictionary<string, LookupPair[]> lookupGroups;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Creates a new palette.</summary>
		public Palette(GraphicsDevice graphicsDevice, PaletteDictionary dictionary) {
			this.dictionary			= dictionary;

			this.paletteTexture		= new Texture2D(graphicsDevice, Dimensions.X, Dimensions.Y);
			this.colorGroups		= new Dictionary<string, PaletteColor[]>();
			this.lookupGroups       = new Dictionary<string, LookupPair[]>();
			
			if (dictionary.PaletteType == PaletteTypes.Tile)
				colorGroups.Add("default", ColorGroupToPaletteGroup(DefaultTile));
			else if (dictionary.PaletteType == PaletteTypes.Entity)
				colorGroups.Add("default", ColorGroupToPaletteGroup(DefaultEntity));
		}

		/// <summary>Constructs a copy of the palette.</summary>
		public Palette(Palette copy) {
			this.dictionary     = new PaletteDictionary(copy.dictionary);

			this.paletteTexture		= new Texture2D(copy.paletteTexture.GraphicsDevice, Dimensions.X, Dimensions.Y);
			this.colorGroups		= new Dictionary<string, PaletteColor[]>();
			this.lookupGroups       = new Dictionary<string, LookupPair[]>();
			foreach (var pair in copy.colorGroups)
				this.colorGroups.Add(pair.Key, (PaletteColor[]) pair.Value.Clone());
			foreach (var pair in copy.lookupGroups)
				this.lookupGroups.Add(pair.Key, (LookupPair[]) pair.Value.Clone());
		}
		

		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Updates the shader texture with the current palette.</summary>
		public void UpdatePalette() {
			XnaColor[] colorData = new XnaColor[Dimensions.X*Dimensions.Y];
			Color[] defaultGroup = PaletteGroupToColorGroup(colorGroups["default"]);
			for (int i = 0; i < PaletteDictionary.MaxColorGroups; i++) {
				int index = i * PaletteDictionary.ColorGroupSize;
				for (int j = 0; j < PaletteDictionary.ColorGroupSize; j++) {
					colorData[index + j] = defaultGroup[j];
				}
			}
			foreach (var pair in dictionary.GetDictionary()) {
				int index = pair.Value;
				for (int i = 0; i < PaletteDictionary.ColorGroupSize; i++) {
					colorData[index + i] = LookupColor(pair.Key, (LookupSubtypes) i);
				}
			}
			paletteTexture.SetData(colorData);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Looks up the color with the specified name and subtype.</summary>
		public Color LookupColor(string name, LookupSubtypes subtype) {
			while (lookupGroups.ContainsKey(name)) {
				LookupPair lookupPair = lookupGroups[name][(int) subtype];
				if (!lookupPair.IsUndefined) {
					name = lookupPair.Name;
					subtype = lookupPair.Subtype;
				}
				else {
					break;
				}
			}
			PaletteColor[] colorGroup;
			if (colorGroups.TryGetValue(name, out colorGroup)) {
				PaletteColor color = colorGroup[(int) subtype];
				if (!color.IsUndefined)
					return color.Color;
			}
			return colorGroups["default"][(int) subtype].Color;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the name and subtype to a lookup.</summary>
		public LookupResult SetLookup(string name, LookupSubtypes subtype, string lookupName,
			LookupSubtypes lookupSubtype)
		{
			if (subtype != LookupSubtypes.All && lookupSubtype == LookupSubtypes.All)
				throw new ArgumentException("Mismatch, cannot assign a single color to a lookup subtype of all.");
			if (name == lookupName && (subtype == lookupSubtype || subtype == LookupSubtypes.All))
				return LookupResult.InfiniteLoop;

			// Do a for loop only if lookupSubtype is All
			LookupSubtypes i = (lookupSubtype == LookupSubtypes.All ? LookupSubtypes.Light : lookupSubtype);
			for (i = 0; i < LookupSubtypes.All; i++) {
				string nextName = lookupName;
				LookupSubtypes nextSubtype = i;
				int depth = 0;
				while (lookupGroups.ContainsKey(nextName)) {
					LookupPair lookupPair = lookupGroups[nextName][(int) nextSubtype];
					if (!lookupPair.IsUndefined) {
						if (depth == 10) {
							return LookupResult.MaxLookupDepth;
						}
						if (lookupPair.Name == name) {
							if (subtype == LookupSubtypes.All) {
								if (lookupSubtype == LookupSubtypes.All)
									return LookupResult.InfiniteLoop;
							}
							else if (lookupPair.Subtype == subtype)
								return LookupResult.InfiniteLoop;
						}
						nextName = lookupPair.Name;
						nextSubtype = lookupPair.Subtype;
						depth++;
					}
					else {
						break;
					}
				}
				if (lookupSubtype != LookupSubtypes.All)
					break;
			}

			LookupPair[] lookupGroup;
			PaletteColor[] colorGroup;
			colorGroups.TryGetValue(name, out colorGroup);
			if (!lookupGroups.TryGetValue(name, out lookupGroup)) {
				lookupGroup = new LookupPair[PaletteDictionary.ColorGroupSize];
				lookupGroups[name] = lookupGroup;
			}
			if (subtype != LookupSubtypes.All) {
				lookupGroup[(int) subtype] = new LookupPair(lookupName, lookupSubtype);
				// Check if color group is unreferenced and we can remove it
				if (colorGroup != null) {
					colorGroup[(int) subtype] = PaletteColor.Undefined;

					for (int j = 0; j < PaletteDictionary.ColorGroupSize; j++) {
						if (lookupGroup[j].IsUndefined) // Nope, end the function here
							return LookupResult.Success;
					}

					colorGroups.Remove(name);
				}
			}
			else {
				for (LookupSubtypes j = 0; j < LookupSubtypes.All; j++) {
					if (lookupSubtype == LookupSubtypes.All)
						lookupGroup[(int) j] = new LookupPair(lookupName, j);
					else
						lookupGroup[(int) j] = new LookupPair(lookupName, lookupSubtype);
				}

				// Remove the unreferenced color group
				if (colorGroup != null) {
					colorGroups.Remove(name);
				}
			}

			return LookupResult.Success;
		}

		/// <summary>Sets the name and subtype to a color.</summary>
		public void SetColor(string name, LookupSubtypes subtype, Color color) {
			PaletteColor[] colorGroup;
			if (!colorGroups.TryGetValue(name, out colorGroup)) {
				colorGroup = new PaletteColor[PaletteDictionary.ColorGroupSize];
				colorGroups[name] = colorGroup;
			}
			LookupPair[] lookupGroup;
			lookupGroups.TryGetValue(name, out lookupGroup);

			if (subtype != LookupSubtypes.All) {
				colorGroup[(int) subtype].Color = color;
				if (lookupGroup != null)
					lookupGroup[(int) subtype] = LookupPair.Undefined;
			}
			else {
				for (int i = 0; i < PaletteDictionary.ColorGroupSize; i++) {
					colorGroup[i].Color = color;
				}
				if (lookupGroup != null)
					lookupGroups.Remove(name);
			}
		}

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Converts a color group to a palette color group.</summary>
		private PaletteColor[] ColorGroupToPaletteGroup(Color[] colorGroup) {
			PaletteColor[] paletteGroup = new PaletteColor[PaletteDictionary.ColorGroupSize];
			for (int i = 0; i < PaletteDictionary.ColorGroupSize; i++) {
				paletteGroup[i].Color = colorGroup[i];
			}
			return paletteGroup;
		}

		/// <summary>Converts a palette color group to a color group.</summary>
		private Color[] PaletteGroupToColorGroup(PaletteColor[] paletteGroup) {
			Color[] colorGroup = new Color[PaletteDictionary.ColorGroupSize];
			for (int i = 0; i < PaletteDictionary.ColorGroupSize; i++) {
				colorGroup[i] = paletteGroup[i].Color;
			}
			return colorGroup;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the palette texture used for the shader.</summary>
		public Texture2D PaletteTexture {
			get { return paletteTexture; }
		}

		/// <summary>Gets the palette's local index mapping dictionary.</summary>
		public PaletteDictionary PaletteDictionary {
			get { return dictionary; }
		}

		/// <summary>Gets the defined type of the palette dictionary.</summary>
		public PaletteTypes PaletteType {
			get { return dictionary.PaletteType; }
		}
	}
}
