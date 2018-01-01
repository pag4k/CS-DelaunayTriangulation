/// <copyright file="Vector2.cs" company="">
/// Pierre-Andre Gagnon, https://github.com/pag4k
/// </copyright>
/// <summary>
/// A 2D vector class with only the functions that are required for the Delaunay triangulation.
/// </summary>

namespace DelaunayTriangulation
{

	public struct Vector2
	{
		public float X;
		public float Y;

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public static Vector2 operator +(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X + right.X, left.Y + right.Y);
		}

		public static Vector2 operator -(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X - right.X, left.Y - right.Y);
		}

		public static Vector2 operator *(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X * right.X, left.Y * right.Y);
		}
			
		public static Vector2 operator *(float left, Vector2 right)
		{
			return new Vector2(left, left) * right;
		}

		public static Vector2 operator *(Vector2 left, float right)
		{
			return left * new Vector2(right, right);
		}

		public static Vector2 operator /(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X / right.X, left.Y / right.Y);
		}
			
		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !(left == right);
		}

		public float Length()
		{
			float ls = X * X + Y * Y;
			return (float)System.Math.Sqrt((double)ls);
		}

		public float LengthSquared()
		{
			return X * X + Y * Y;
		}
			
		public static float Distance(Vector2 value1, Vector2 value2)
		{
			float dx = value1.X - value2.X;
			float dy = value1.Y - value2.Y;

			float ls = dx * dx + dy * dy;

			return (float)System.Math.Sqrt((double)ls);
		}

		public static float DistanceSquared(Vector2 value1, Vector2 value2)
		{
			float dx = value1.X - value2.X;
			float dy = value1.Y - value2.Y;

			return dx * dx + dy * dy;
		}

		public override int GetHashCode()
		{
			int hash = this.X.GetHashCode();
			hash = (((hash << 5) + hash) ^ this.Y.GetHashCode ());
			return hash;
		}

		public bool Equals(Vector2 other)
		{
			return this.X == other.X && this.Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Vector2))
				return false;
			return Equals((Vector2)obj);
		}
	}
}

