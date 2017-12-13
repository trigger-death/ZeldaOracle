﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using GdiColor		= System.Drawing.Color;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A structure for storing color data.</summary>
	public struct Color {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>R:255 G:255 B:255 A:0.</summary>
		public static readonly Color TransparentWhite	= new Color(255, 255, 255, 0);
		/// <summary>R:0 G:0 B:0 A:0.</summary>
		public static readonly Color Transparent		= new Color(0, 0, 0, 0);

		/// <summary>R:255 G:255 B:255 A:255.</summary>
		public static readonly Color White      = new Color(255, 255, 255, 255);
		/// <summary>R:0 G:0 B:0 A:255.</summary>
		public static readonly Color Black      = new Color(0, 0, 0, 255);

		/// <summary>R:255 G:0 B:0 A:255.</summary>
		public static readonly Color Red		= new Color(255, 0, 0);
		/// <summary>R:0 G:255 B:0 A:255.</summary>
		public static readonly Color Green		= new Color(0, 255, 0);
		/// <summary>R:0 G:0 B:255 A:255.</summary>
		public static readonly Color Blue		= new Color(0, 0, 255);

		/// <summary>R:255 G:255 B:0 A:255.</summary>
		public static readonly Color Yellow		= new Color(255, 255, 0);
		/// <summary>R:128 G:128 B:0 A:255.</summary>
		public static readonly Color Olive		= new Color(128, 128, 0);
		/// <summary>R:0 G:255 B:255 A:255.</summary>
		public static readonly Color Cyan		= new Color(0, 255, 255);
		/// <summary>R:128 G:0 B:0 A:255.</summary>
		public static readonly Color Maroon		= new Color(128, 0, 0);
		/// <summary>R:255 G:0 B:255 A:255.</summary>
		public static readonly Color Magenta	= new Color(255, 0, 255);


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The alpha value of the color.</summary>
		public byte A;
		/// <summary>The red value of the color.</summary>
		public byte R;
		/// <summary>The green value of the color.</summary>
		public byte G;
		/// <summary>The blue value of the color.</summary>
		public byte B;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a color with the specified values.</summary>
		public Color(int red, int green, int blue, bool asHsv = false) {
			if (asHsv) {
				this.R = 0;
				this.G = 0;
				this.B = 0;
				this.A = 255;
				this.SetHSV(red, green, blue);
			}
			else {
				this.R	= (byte)GMath.Clamp(red, 0, 255);
				this.G	= (byte)GMath.Clamp(green, 0, 255);
				this.B	= (byte)GMath.Clamp(blue, 0, 255);
				this.A	= 255;
			}
		}

		/// <summary>Constructs a color with the specified values.</summary>
		public Color(int red, int green, int blue, int alpha, bool asHsv = false) {
			if (asHsv) {
				this.R = 0;
				this.G = 0;
				this.B = 0;
				this.A = (byte)GMath.Clamp(alpha, 0, 255);
				this.SetHSV(red, green, blue);
			}
			else {
				this.R	= (byte)GMath.Clamp(red, 0, 255);
				this.G	= (byte)GMath.Clamp(green, 0, 255);
				this.B	= (byte)GMath.Clamp(blue, 0, 255);
				this.A	= (byte)GMath.Clamp(alpha, 0, 255);
			}
		}

		/// <summary>Constructs a color with the specified values.</summary>
		public Color(float red, float green, float blue, bool asHsv = false) {
			if (asHsv) {
				this.R = 0;
				this.G = 0;
				this.B = 0;
				this.A = 255;
				this.SetHSV(red, green, blue);
			}
			else {
				this.R	= (byte)(GMath.Clamp(red, 0f, 1f) * 255f);
				this.G	= (byte)(GMath.Clamp(green, 0f, 1f) * 255f);
				this.B	= (byte)(GMath.Clamp(blue, 0f, 1f) * 255f);
				this.A	= 255;
			}
		}

		/// <summary>Constructs a color with the specified values.</summary>
		public Color(float red, float green, float blue, float alpha, bool asHsv = false) {
			if (asHsv) {
				this.R	= 0;
				this.G	= 0;
				this.B	= 0;
				this.A	= (byte)(GMath.Clamp(alpha, 0f, 1f) * 255f);
				this.SetHSV(red, green, blue);
			}
			else {
				this.R	= (byte)(GMath.Clamp(red, 0f, 1f) * 255f);
				this.G	= (byte)(GMath.Clamp(green, 0f, 1f) * 255f);
				this.B	= (byte)(GMath.Clamp(blue, 0f, 1f) * 255f);
				this.A	= (byte)(GMath.Clamp(alpha, 0f, 1f) * 255f);
			}
		}

		/// <summary>Constructs a color with the specified values.</summary>
		public Color(double red, double green, double blue, bool asHsv = false) {
			if (asHsv) {
				this.R = 0;
				this.G = 0;
				this.B = 0;
				this.A = 255;
				this.SetHSV(red, green, blue);
			}
			else {
				this.R	= (byte)(GMath.Clamp(red, 0.0, 1.0) * 255.0);
				this.G	= (byte)(GMath.Clamp(green, 0.0, 1.0) * 255.0);
				this.B	= (byte)(GMath.Clamp(blue, 0.0, 1.0) * 255.0);
				this.A	= 255;
			}
		}

		/// <summary>Constructs a color with the specified values.</summary>
		public Color(double red, double green, double blue, double alpha, bool asHsv = false) {
			if (asHsv) {
				this.R	= 0;
				this.G	= 0;
				this.B	= 0;
				this.A	= (byte)(GMath.Clamp(alpha, 0.0, 1.0) * 255.0);
				this.SetHSV(red, green, blue);
			}
			else {
				this.R	= (byte)(GMath.Clamp(red, 0.0, 1.0) * 255.0);
				this.G	= (byte)(GMath.Clamp(green, 0.0, 1.0) * 255.0);
				this.B	= (byte)(GMath.Clamp(blue, 0.0, 1.0) * 255.0);
				this.A	= (byte)(GMath.Clamp(alpha, 0.0, 1.0) * 255.0);
			}
		}

		/// <summary>Constructs a copy of the specified color.</summary>
		public Color(Color color) {
			this.R	= color.R;
			this.G	= color.G;
			this.B	= color.B;
			this.A	= color.A;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Outputs a string representing this color.</summary>
		public override string ToString() {
			return "(R:" + R + " G:" + G + " B:" + B + " A:" + A + ")";
		}

		/// <summary>Outputs a string representing this color.</summary>
		public string ToString(IFormatProvider provider) {
			// TODO: Write formatting for Color.ToString(format).

			return "(R:" + R + " G:" + G + " B:" + B + " A:" + A + ")";
		}

		/// <summary>Outputs a string representing this color.</summary>
		public string ToString(string format, IFormatProvider provider) {
			return "(R:" + R + " G:" + G + " B:" + B + " A:" + A + ")";
		}

		/// <summary>Outputs a string representing this color.</summary>
		public string ToString(string format) {
			return "(R:" + R + " G:" + G + " B:" + B + " A:" + A + ")";
		}

		/// <summary>Returns true if the specified color has the same rgba values.</summary>
		public override bool Equals(object obj) {
			if (obj is Color)
				return (R == ((Color)obj).R && G == ((Color)obj).G && B == ((Color)obj).B && A == ((Color)obj).A);
			return false;
		}

		/// <summary>Returns the hash code for this color.</summary>
		public override int GetHashCode() {
			unchecked { return (int) PackedColor; }
		}

		/// <summary>Parses the color.</summary>
		public static Color Parse(string text) {
			Color value = Color.White;

			if (text.Length > 0) {
				if (text[0] == '(')
					text = text.Substring(1);
				if (text[text.Length - 1] == ')')
					text = text.Substring(0, text.Length - 1);

				int lastCommaPos	= -1;
				int commaPos		= -1;

				for (int i = 0; i < 4; i++) {
					lastCommaPos	= commaPos;
					commaPos		= text.IndexOf(',', lastCommaPos + 1);
					//if (commaPos == -1)
					//	commaPos	= text.IndexOf(' ', lastCommaPos + 1);
					if (commaPos == -1 && i >= 2) {
						commaPos = text.Length;
					}

					if (commaPos != -1) {
						string str = text.Substring(lastCommaPos + 1, commaPos - (lastCommaPos + 1));

						try {
							byte v = Byte.Parse(str);
							switch (i) {
							case 0: value.R = v; break;
							case 1: value.G = v; break;
							case 2: value.B = v; break;
							case 3: value.A = v; break;
							}
						}
						catch (FormatException e) {
							throw e;
						}
						catch (ArgumentNullException e) {
							throw e;
						}
						catch (OverflowException e) {
							throw e;
						}
					}
					else if (i < 2) {
						throw new FormatException();
					}

					if (commaPos == text.Length) {
						return value;
					}
				}
			}
			else {
				throw new ArgumentNullException();
			}

			return value;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(Color a, Color b) {
			return (a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A);
		}

		public static bool operator !=(Color a, Color b) {
			return (a.R != b.R || a.G != b.G || a.B != b.B || a.A != b.A);
		}

		public static Color operator *(Color a, float scalar) {
			return new Color(
				(byte) (GMath.Clamp((a.R / 255.0f) * scalar, 0.0f, 1.0f) * 255),
				(byte) (GMath.Clamp((a.G / 255.0f) * scalar, 0.0f, 1.0f) * 255),
				(byte) (GMath.Clamp((a.B / 255.0f) * scalar, 0.0f, 1.0f) * 255),
				(byte) (GMath.Clamp((a.A / 255.0f) * scalar, 0.0f, 1.0f) * 255));
		}


		//-----------------------------------------------------------------------------
		// Conversions
		//-----------------------------------------------------------------------------

		public static explicit operator Color(XnaColor c) {
			return new Color(c.R, c.G, c.B, c.A);
		}

		public static implicit operator XnaColor(Color c) {
			return new XnaColor(c.R, c.G, c.B, c.A);
		}

		public static explicit operator Color(GdiColor c) {
			return new Color(c.R, c.G, c.B, c.A);
		}

		public static implicit operator GdiColor(Color c) {
			return GdiColor.FromArgb(c.A, c.R, c.G, c.B);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the packed color with all the values inside an unsigned int.</summary>
		public uint PackedColor {
			get { return (((uint)A) << 12) + (((uint)R) << 8) + (((uint)G) << 8) + ((uint)B); }
		}
		
		/// <summary>Gets or sets hue of the color.</summary>
		public int Hue {
			get {
				byte max = GMath.Max(R, GMath.Max(G, B));

				if (max == R)
					return   0 + ((int)(G - B) * 60 / 255);
				else if (max == G)
					return 120 + ((int)(B - R) * 60 / 255);
				else
					return 240 + ((int)(R - G) * 60 / 255);
			}
			set { SetHSV(value, Sat, Val); }
		}

		/// <summary>Gets or sets the saturation of the color.</summary>
		public int Sat {
			get {
				byte max = (byte)((int)GMath.Max(R, GMath.Max(G, B)) * 100 / 255);
				byte min = (byte)((int)GMath.Min(R, GMath.Min(G, B)) * 100 / 255);
				byte c = (byte)(max - min);
				return (c != 0 ? c * 100 / max : 0);
			}
			set { SetHSV(Hue, value, Val); }
		}

		/// <summary>Gets or sets the value of the color.</summary>
		public int Val {
			get { return ((int)GMath.Max(R, GMath.Max(G, B)) * 100 / 255); }
			set { SetHSV(Hue, Sat, value); }
		}

		/// <summary>Gets the total of the RGB channels.</summary>
		public int Total {
			get { return R + G + B; }
		}

		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Sets the rgb values of the color.</summary>
		public void SetRGB(int red, int green, int blue) {
			R = (byte)GMath.Clamp(red, 0, 255);
			G = (byte)GMath.Clamp(green, 0, 255);
			B = (byte)GMath.Clamp(blue, 0, 255);
		}

		/// <summary>Sets the rgb values of the color.</summary>
		public void SetRGB(float red, float green, float blue) {
			R = (byte)(GMath.Clamp(red, 0f, 1f) * 255f);
			G = (byte)(GMath.Clamp(green, 0f, 1f) * 255f);
			B = (byte)(GMath.Clamp(blue, 0f, 1f) * 255f);
		}

		/// <summary>Sets the rgb values of the color.</summary>
		public void SetRGB(double red, double green, double blue) {
			R = (byte)(GMath.Clamp(red, 0.0, 1.0) * 255.0);
			G = (byte)(GMath.Clamp(green, 0.0, 1.0) * 255.0);
			B = (byte)(GMath.Clamp(blue, 0.0, 1.0) * 255.0);
		}
		
		/// <summary>Sets the hsv values of the color.</summary>
		public void SetHSV(int hue, int sat, int val) {
			SetHSV((float)hue / 360f, (float)sat / 100f, (float)val / 100f);
		}

		/// <summary>Sets the hsv values of the color.</summary>
		public void SetHSV(float hue, float sat, float val) {
			hue = GMath.Clamp(hue, 0f, 1f);
			sat = GMath.Clamp(sat, 0f, 1f);
			val = GMath.Clamp(val, 0f, 1f);
			float hue2 = hue * 6f;
			float hueI = (float)GMath.Floor(hue2);

			byte b0 = (byte)(255f * val);
			byte b1 = (byte)(255f * val * (1f - sat));
			byte b2 = (byte)(255f * val * (1f - sat * (hue2 - hueI)));
			byte b3 = (byte)(255f * val * (1f - sat * (1f - (hue2 - hueI))));

			switch ((int)hueI) {
			case 0: R = b0; G = b3; B = b1; break;
			case 1: R = b2; G = b0; B = b1; break;
			case 2: R = b1; G = b0; B = b3; break;
			case 3: R = b1; G = b2; B = b0; break;
			case 4: R = b3; G = b1; B = b0; break;
			case 5: R = b0; G = b1; B = b2; break;
			}
		}

		/// <summary>Sets the hsv values of the color.</summary>
		public void SetHSV(double hue, double sat, double val) {
			SetHSV((float)hue, (float)sat, (float)val);
		}


		//-----------------------------------------------------------------------------
		// Static
		//-----------------------------------------------------------------------------

		/// <summary>Converts the color to a 15-bit GameBoy Color color.</summary>
		public static Color ToGBCColor(int r, int g, int b) {
			return ToGBCColor(new Color(r, g, b));
		}

		/// <summary>Converts the color to a 15-bit GameBoy Color color.</summary>
		public static Color ToGBCColor(float r, float g, float b) {
			return ToGBCColor(new Color(r, g, b));
		}

		/// <summary>Converts the color to a 15-bit GameBoy Color color.</summary>
		public static Color ToGBCColor(Color color) {
			return new Color(color.R & 0xF8, color.G & 0xF8, color.B & 0xF8);
		}
	}
} // End namespace
