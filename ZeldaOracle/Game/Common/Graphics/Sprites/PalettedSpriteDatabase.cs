﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace ZeldaOracle.Common.Graphics.Sprites {

	public struct ColorGroupSubtypePair {
		public string ColorGroup { get; set; }
		public LookupSubtypes Subtype { get; set; }
		public Color MappedColor { get; set; }

		public ColorGroupSubtypePair(string colorGroup, LookupSubtypes subtype, PaletteDictionary dictionary) {
			this.ColorGroup     = colorGroup;
			this.Subtype        = subtype;
			// Store the map color during creation.
			this.MappedColor	= dictionary.GetMappedColor(colorGroup, subtype);
		}
	}

	public struct SpritePaletteArgs {
		public Image Image { get; set; }
		public Rectangle2I SourceRect { get; set; }
		public Point2I DrawOffset { get; set; }
		public Dictionary<Color, Dictionary<int, ColorGroupSubtypePair>> ColorMapping { get; set; }
		public int[] IndexedPossibleColorGroups { get; set; }
		public string[] PossibleColorGroups { get; set; }
		public HashSet<Color> IgnoreColors { get; set; }
		public PaletteDictionary Dictionary { get; set; }
		public Point2I ChunkSize { get; set; }
		public Color[] DefaultMapping { get; set; }
	}

	public class UnspecifiedColorException : Exception {
		public Color Color { get; set; }

		public UnspecifiedColorException(Color color) {
			this.Color = color;
		}
	}

	public class NoMatchingColorGroupsException : Exception {
		public HashSet<Color> Colors { get; set; }

		public NoMatchingColorGroupsException(HashSet<Color> colors) {
			this.Colors = colors;
		}
	}

	public class PalettedSpriteDatabase {

		private class PalettedSpriteDatabaseImage {

			/// <summary>The maximum allowed size of an image. Except when a single cell is larger.</summary>
			private static readonly Point2I MaxImageSize = new Point2I(512, 512);

			/// <summary>The index for the next-defined sprite.</summary>
			private int index;
			/// <summary>The spritesheet dimensions of the image.</summary>
			private Point2I dimensions;
			/// <summary>The size of the sprites.</summary>
			private Point2I size;
			/// <summary>The collection of images.</summary>
			private List<Image> images;


			public PalettedSpriteDatabaseImage(Point2I size) {
				this.index          = 0;
				this.size           = size;
				this.images         = new List<Image>();
				this.dimensions     = GMath.Max(Point2I.One, MaxImageSize / size);
			}

			public BasicSprite AddSprite(SpritePaletteArgs args) {
				// Do we need to create a new image
				Image currentImage;
				if (ImageIndex == 0) {
					currentImage = new Image(Resources.GraphicsDevice, dimensions * size);
					images.Add(currentImage);
				}
				else {
					currentImage = CurrentImage;
				}

				// The list of already scanned colors
				HashSet<Color> scannedColors = new HashSet<Color>();

				Color defaultTransparent = Color.Transparent;
				Color defaultBlack = Color.Black;
				if (args.DefaultMapping != null) {
					defaultTransparent = args.DefaultMapping[(int) LookupSubtypes.Transparent];
					defaultBlack = args.DefaultMapping[(int) LookupSubtypes.Black];
				}

				// Modify the original sprite's colors based on the mapping
					XnaColor[] colorData = new XnaColor[args.SourceRect.Area];
				Point2I rectSize = args.SourceRect.Size;
				//XnaColor[] colorData = new XnaColor[currentImage.Width * currentImage.Height];
				XnaRectangle rect = (XnaRectangle) args.SourceRect;
				args.Image.Texture.GetData<XnaColor>(0, rect, colorData, 0, colorData.Length);
				Point2I chunkSize = args.ChunkSize;
				if (chunkSize.IsZero)
					chunkSize = rectSize;
				Point2I numChunks = (rectSize + chunkSize - 1) / chunkSize;
				for (int chunkX = 0; chunkX < numChunks.X; chunkX++) {
					for (int chunkY = 0; chunkY < numChunks.Y; chunkY++) {
						scannedColors.Clear();
						HashSet<int> possibleGroups = new HashSet<int>(args.IndexedPossibleColorGroups);

						for (int x = 0; x < chunkSize.X; x++) {
							int ix = chunkX * chunkSize.X + x;
							if (ix >= rectSize.X) break;
							for (int y = 0; y < chunkSize.Y; y++) {
								int iy = chunkY * chunkSize.Y + y;
								if (iy >= rectSize.Y) break;
								int index = ix + iy * rectSize.X;
								Color color = (Color) colorData[index];
								if (color.A == 0) {
									color = Color.Transparent;
									colorData[index] = color;
								}
								if (!scannedColors.Contains(color)) {
									scannedColors.Add(color);
									Dictionary<int, ColorGroupSubtypePair> dict;
									if (args.ColorMapping.TryGetValue(color, out dict)) {
										possibleGroups.RemoveWhere(s => !dict.ContainsKey(s));
										if (!possibleGroups.Any())
											throw new NoMatchingColorGroupsException(scannedColors);
									}
									else if (args.IgnoreColors.Contains(color)) {
										// Carry on
									}
									else if (color == Color.Black || (args.Dictionary.PaletteType == PaletteTypes.Entity && color.A == 0)) {
										// All groups are valid here
									}
									else {
										throw new UnspecifiedColorException(color);
									}
								}
							}
						}

						int indexedFinalColorGroup = -1;
						string finalColorGroup = null;
						for (int i = 0; i < args.IndexedPossibleColorGroups.Length; i++) {
							int index = args.IndexedPossibleColorGroups[i];
							if (possibleGroups.Contains(index)) {
								indexedFinalColorGroup = index;
								finalColorGroup = args.PossibleColorGroups[i];
								break;
							}
						}
						Dictionary<Color, Color> finalColorMapping = new Dictionary<Color, Color>();
						bool transparentDefined = args.Dictionary.PaletteType != PaletteTypes.Entity;
						bool blackDefined = false;
						foreach (var pair in args.ColorMapping) {
							if (pair.Value.ContainsKey(indexedFinalColorGroup)) {
								ColorGroupSubtypePair colorGroupPair = pair.Value[indexedFinalColorGroup];
								if (!transparentDefined && colorGroupPair.Subtype == LookupSubtypes.Transparent)
									transparentDefined = true;
								else if (!blackDefined && colorGroupPair.Subtype == LookupSubtypes.Black)
									blackDefined = true;
								if (args.DefaultMapping != null)
									finalColorMapping.Add(pair.Key, args.DefaultMapping[(int) colorGroupPair.Subtype]);
								else
									finalColorMapping.Add(pair.Key, colorGroupPair.MappedColor);
							}
						}
						if (!transparentDefined)
							finalColorMapping.Add(Color.Transparent, args.Dictionary.GetMappedColor(finalColorGroup, LookupSubtypes.Transparent));
						if (!blackDefined)
							finalColorMapping.Add(Color.Black, args.Dictionary.GetMappedColor(finalColorGroup, LookupSubtypes.Black));

						for (int x = 0; x < chunkSize.X; x++) {
							int ix = chunkX * chunkSize.X + x;
							if (ix >= rectSize.X) break;
							for (int y = 0; y < chunkSize.Y; y++) {
								int iy = chunkY * chunkSize.Y + y;
								if (iy >= rectSize.Y) break;
								int index = ix + iy * rectSize.X;
								colorData[index] = finalColorMapping[(Color) colorData[index]];
							}
						}
					}
				}
				/*for (int i = 0; i < colorData.Length; i++) {
					Color color = (Color)colorData[i];
					if (args.ColorMapping.ContainsKey(color)) {
						colorData[i] = args.ColorMapping[color].MappedColor;
					}
					else if (!args.IgnoreColors.Contains(color)) {
					}
				}*/

				// Save the mapping to the database image
				rect = (XnaRectangle) CurrentSourceRect;
				currentImage.Texture.SetData<XnaColor>(0, rect, colorData, 0, colorData.Length);

				// Create a new sprite from the database
				BasicSprite sprite = new BasicSprite(currentImage, CurrentSourceRect, args.DrawOffset);

				index++;
				return sprite;
			}

			private int IndeciesPerImage {
				get { return dimensions.X * dimensions.Y; }
			}
			private Image CurrentImage {
				get { return images.Last(); }
			}
			private int ImageIndex {
				get { return index % IndeciesPerImage; }
			}
			private Rectangle2I CurrentSourceRect {
				get { return new Rectangle2I((ImageIndex % dimensions.X) * size.X, (ImageIndex / dimensions.Y) * size.Y, size); }
			}
		}

		private Dictionary<Point2I, PalettedSpriteDatabaseImage> spriteImages;

		public PalettedSpriteDatabase() {
			this.spriteImages   = new Dictionary<Point2I, PalettedSpriteDatabaseImage>();
		}

		public BasicSprite AddSprite(SpritePaletteArgs args) {
			PalettedSpriteDatabaseImage databaseImage;
			if (spriteImages.ContainsKey(args.SourceRect.Size)) {
				databaseImage = spriteImages[args.SourceRect.Size];
			}
			else {
				databaseImage = new PalettedSpriteDatabaseImage(args.SourceRect.Size);
				spriteImages[args.SourceRect.Size] = databaseImage;
			}
			return databaseImage.AddSprite(args);
		}
	}
}
