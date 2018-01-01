/// <copyright file="Edge.cs" company="">
/// Pierre-Andre Gagnon, https://github.com/pag4k
/// </copyright>
/// <summary>
/// A class to hold the data of a 2D edge.
/// </summary>

namespace DelaunayTriangulation
{
	
	public class Edge {

		// The 2 vectors constituting the edge.
		public Vector2 A, B;
		// The distance between the two vectors.
		public float distance;

		public Edge(Vector2 A, Vector2 B) {
			// Order the two vectors so that the one with the smallest x is A.
			// If their x's are equal, A is the one with the smallest y.
			if ((A.X < B.X) || (A.X == B.X && A.Y < B.Y)) {
				this.A = A;
				this.B = B;
			} else if ((A.X > B.X) || (A.X == B.X && A.Y > B.Y)) {
				this.A = B;
				this.B = A;
			} else {
				// This means that two points have the same location.
			}
			// Compute in advance the distance between the two vectors of the edge as it will later be used.
			distance = Vector2.Distance (this.A, this.B);

		}

		// Override Equals and GetHashCode to make sure they behave correctly.
		public override bool Equals(object obj)
		{
			if (!(obj is Edge))
				return false;
			return Equals((Edge)obj);
		}

		public bool Equals(Edge other)
		{
			return this.A == other.A && this.B == other.B;
		}

		public override int GetHashCode()
		{
			int hash = this.A.GetHashCode();
			hash = (((hash << 5) + hash) ^ this.B.GetHashCode ());
			return hash;
		}

	}
}
