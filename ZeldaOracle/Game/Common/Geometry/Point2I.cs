﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ZeldaOracle.Common.Geometry {
/** <summary>
 * The 2D integer precision point with basic operations and functions.
 * </summary> */
public struct Point2I {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> Returns a point positioned at (0, 0). </summary> */
	public static Point2I Zero {
		get { return new Point2I(); }
	}
	/** <summary> Returns a point positioned at (1, 1). </summary> */
	public static Point2I One {
		get { return new Point2I(1, 1); }
	}

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The x coordinate of the point. </summary> */
	public int X;
	/** <summary> The y coordinate of the point. </summary> */
	public int Y;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs a point positioned at the specified coordinates. </summary> */
	public Point2I(int x, int y) {
		this.X	= x;
		this.Y	= y;
	}
	/** <summary> Constructs a point positioned at the specified coordinates. </summary> */
	public Point2I(int xy) {
		this.X	= xy;
		this.Y	= xy;
	}
	/** <summary> Constructs a copy of the specified point. </summary> */
	public Point2I(Point2I p) {
		this.X	= p.X;
		this.Y	= p.Y;
	}
	/** <summary> Constructs a copy of the specified point. </summary> */
	public Point2I(Vector2F v) {
		this.X	= (int)v.X;
		this.Y	= (int)v.Y;
	}

	#endregion
	//=========== GENERAL ============
	#region General

	/** <summary> Outputs a string representing this point as (x, y). </summary> */
	public override string ToString() {
		return "(" + X + ", " + Y + ")";
	}
	/** <summary> Outputs a string representing this point as (x, y). </summary> */
	public string ToString(IFormatProvider provider) {
		// TODO: Write formatting for Point2I.ToString(format).

		return "(" + X.ToString(provider) + ", " + Y.ToString(provider) + ")";
	}
	/** <summary> Outputs a string representing this point as (x, y). </summary> */
	public string ToString(string format, IFormatProvider provider) {
		return "(" + X.ToString(format, provider) + ", " + Y.ToString(format, provider) + ")";
	}
	/** <summary> Outputs a string representing this point as (x, y). </summary> */
	public string ToString(string format) {
		return "(" + X.ToString(format) + ", " + Y.ToString(format) + ")";
	}
	/** <summary> Returns true if the specified point has the same x and y coordinates. </summary> */
	public override bool Equals(object obj) {
		if (obj is Point2I)
			return (X == ((Point2I)obj).X && Y == ((Point2I)obj).Y);
		return false;
	}
	/** <summary> Returns the hash code for this point. </summary> */
	public override int GetHashCode() {
		return base.GetHashCode();
	}
	/** <summary> Parses the point. </summary> */
	public static Point2I Parse(string text) {
		Point2I value = Point2I.Zero;

		if (text.Length > 0) {
			if (text[0] == '(')
				text = text.Substring(1);
			if (text[text.Length - 1] == ')')
				text = text.Substring(0, text.Length - 1);

			int commaPos = text.IndexOf(',');
			if (commaPos == -1)
				commaPos = text.IndexOf(' ');
			if (commaPos != -1) {

				string strX = text.Substring(0, commaPos);
				string strY = text.Substring(commaPos + 1);

				try {
					value.X = Int32.Parse(strX);
					value.Y = Int32.Parse(strY);
				} catch (FormatException e) {
					throw e;
				} catch (ArgumentNullException e) {
					throw e;
				} catch (OverflowException e) {
					throw e;
				}
			}
			else {
				throw new FormatException();
			}
		}
		else {
			throw new ArgumentNullException();
		}

		return value;
	}

	#endregion
	//========== OPERATORS ===========
	#region Operators
	//--------------------------------
	#region Unary Arithmetic

	public static Point2I operator +(Point2I p) {
		return p;
	}
	public static Point2I operator -(Point2I p) {
		return new Point2I(-p.X, -p.Y);
	}
	public static Point2I operator ++(Point2I p) {
		return new Point2I(++p.X, ++p.Y);
	}
	public static Point2I operator --(Point2I p) {
		return new Point2I(--p.X, --p.Y);
	}

	#endregion
	//--------------------------------
	#region Binary Arithmetic

	public static Point2I operator +(Point2I p1, Point2I p2) {
		return new Point2I(p1.X + p2.X, p1.Y + p2.Y);
	}
	public static Point2I operator +(int i1, Point2I p2) {
		return new Point2I(i1 + p2.X, i1 + p2.Y);
	}
	public static Point2I operator +(Point2I p1, int i2) {
		return new Point2I(p1.X + i2, p1.Y + i2);
	}

	public static Point2I operator -(Point2I p1, Point2I p2) {
		return new Point2I(p1.X - p2.X, p1.Y - p2.Y);
	}
	public static Point2I operator -(int i1, Point2I p2) {
		return new Point2I(i1 - p2.X, i1 - p2.Y);
	}
	public static Point2I operator -(Point2I p1, int i2) {
		return new Point2I(p1.X - i2, p1.Y - i2);
	}

	public static Point2I operator *(Point2I p1, Point2I p2) {
		return new Point2I(p1.X * p2.X, p1.Y * p2.Y);
	}
	public static Point2I operator *(int i1, Point2I p2) {
		return new Point2I(i1 * p2.X, i1 * p2.Y);
	}
	public static Point2I operator *(Point2I p1, int i2) {
		return new Point2I(p1.X * i2, p1.Y * i2);
	}

	public static Point2I operator /(Point2I p1, Point2I p2) {
		return new Point2I(p1.X / p2.X, p1.Y / p2.Y);
	}
	public static Point2I operator /(int i1, Point2I p2) {
		return new Point2I(i1 / p2.X, i1 / p2.Y);
	}
	public static Point2I operator /(Point2I p1, int i2) {
		return new Point2I(p1.X / i2, p1.Y / i2);
	}

	public static Point2I operator %(Point2I p1, Point2I p2) {
		return new Point2I(p1.X % p2.X, p1.Y % p2.Y);
	}
	public static Point2I operator %(int i1, Point2I p2) {
		return new Point2I(i1 % p2.X, i1 % p2.Y);
	}
	public static Point2I operator %(Point2I p1, int i2) {
		return new Point2I(p1.X % i2, p1.Y % i2);
	}

	#endregion
	//--------------------------------
	#region Binary Logic

	public static bool operator ==(Point2I p1, Point2I p2) {
		return (p1.X == p2.X && p1.Y == p2.Y);
	}
	public static bool operator ==(int i1, Point2I p2) {
		return (i1 == p2.X && i1 == p2.Y);
	}
	public static bool operator ==(Point2I p1, int i2) {
		return (p1.X == i2 && p1.Y == i2);
	}

	public static bool operator !=(Point2I p1, Point2I p2) {
		return (p1.X != p2.X || p1.Y != p2.Y);
	}
	public static bool operator !=(int i1, Point2I p2) {
		return (i1 != p2.X || i1 != p2.Y);
	}
	public static bool operator !=(Point2I p1, int i2) {
		return (p1.X != i2 || p1.Y != i2);
	}

	public static bool operator <(Point2I p1, Point2I p2) {
		return (p1.X < p2.X && p1.Y < p2.Y);
	}
	public static bool operator <(int i1, Point2I p2) {
		return (i1 < p2.X && i1 < p2.Y);
	}
	public static bool operator <(Point2I p1, int i2) {
		return (p1.X < i2 && p1.Y < i2);
	}

	public static bool operator >(Point2I p1, Point2I p2) {
		return (p1.X > p2.X && p1.Y > p2.Y);
	}
	public static bool operator >(int i1, Point2I p2) {
		return (i1 > p2.X && i1 > p2.Y);
	}
	public static bool operator >(Point2I p1, int i2) {
		return (p1.X > i2 && p1.Y > i2);
	}

	public static bool operator <=(Point2I p1, Point2I p2) {
		return (p1.X <= p2.X && p1.Y <= p2.Y);
	}
	public static bool operator <=(int i1, Point2I p2) {
		return (i1 <= p2.X && i1 <= p2.Y);
	}
	public static bool operator <=(Point2I p1, int i2) {
		return (p1.X <= i2 && p1.Y <= i2);
	}

	public static bool operator >=(Point2I p1, Point2I p2) {
		return (p1.X >= p2.X && p1.Y >= p2.Y);
	}
	public static bool operator >=(int i1, Point2I p2) {
		return (i1 >= p2.X && i1 >= p2.Y);
	}
	public static bool operator >=(Point2I p1, int i2) {
		return (p1.X >= i2 && p1.Y >= i2);
	}

	#endregion
	//--------------------------------
	#region Conversion

	public static implicit operator Point2I(Point p) {
		return new Point2I(p.X, p.Y);
	}
	public static explicit operator Point2I(int i) {
		return new Point2I(i);
	}

	public static explicit operator Point(Point2I p) {
		return new Point(p.X, p.Y);
	}
	public static explicit operator Vector2(Point2I p) {
		return new Vector2((float)p.X, (float)p.Y);
	}

	#endregion
	//--------------------------------
	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets the direction of the point. </summary> */
	[ContentSerializerIgnore]
	public double Direction {
		get {
			if (X == 0 && Y == 0)
				return 0.0;
			return GMath.Atan2(Y, X);
		}
		set {
			double length = GMath.Sqrt((X * X) + (Y * Y));
			X = (int)(length * GMath.Cos(value));
			Y = (int)(length * GMath.Sin(value));
		}
	}
	/** <summary> Gets or sets the length of the point. </summary> */
	[ContentSerializerIgnore]
	public double Length {
		get {
			return GMath.Sqrt((X * X) + (Y * Y));
		}
		set {
			double oldLength = GMath.Sqrt((X * X) + (Y * Y));
			if (oldLength > 0) {
				X = (int)(X * (value / oldLength));
				Y = (int)(Y * (value / oldLength));
			}
			else {
				X = (int)value;
				Y = 0;
			}
		}
	}
	/** <summary> Gets or sets the x or y coordinate from the index. </summary> */
	[ContentSerializerIgnore]
	public int this[int coordinate] {
		get {
			if (coordinate < 0 || coordinate > 1)
				throw new System.IndexOutOfRangeException("Point2D[coordinateIndex] must be either 0 or 1.");
			else
				return (coordinate == 0 ? X : Y);
		}
		set {
			if (coordinate < 0 || coordinate > 1)
				throw new System.IndexOutOfRangeException("Point2D[coordinateIndex] must be either 0 or 1.");
			else if (coordinate == 0)
				X = value;
			else
				Y = value;
		}
	}
	/** <summary> Returns true if the point is positioned at (0, 0). </summary> */
	public bool IsZero {
		get { return (X == 0 && Y == 0); }
	}
	/** <summary> Returns the perpendicular point. </summary> */
	public Point2I Perpendicular {
		get { return new Point2I(-Y, X); }
	}

	#endregion
}
} // End namespace