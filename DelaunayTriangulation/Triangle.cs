/// <copyright file="Triangle.cs" company="">
/// Pierre-Andre Gagnon, https://github.com/pag4k
/// </copyright>
/// <summary>
/// A class to hold the data of a 2D triangle including circumcircle data.
/// </summary>

namespace DelaunayTriangulation
{
	
	public class Triangle {

		// The 3 vectors constituting the triangle.
		public Vector2 A, B, C;
		// The 3 edges constituting the edges.
		public Edge AB, BC, CA;
		// The vector corresponding to the circumcenter center of the triangle.
		// The circumcenter is the center of the unique circle in which the triangle can be inscribed, the circumcircle.
		public Vector2 circumcenter;
		// The squared radius of the circumcercle.
		public float squaredCircumradius;

		public Triangle(Vector2 A, Vector2 B, Vector2 C) {

			this.A = A;
			this.B = B;
			this.C = C;

			this.AB = new Edge (this.A, this.B);
			this.BC = new Edge (this.B, this.C);
			this.CA = new Edge (this.C, this.A);

			this.circumcenter = GetCircumcenter ();
			this.squaredCircumradius = GetSquaredCircumradius ();
		}

		public Triangle(Edge AB, Vector2 C) : this(AB.A, AB.B, C) {
		}

		// A utility function to calculate the determinant of 3 2D vectors put together in a 3x3 matrix.
		// Note that the z-coordinate of each vector is set to 1.
		private float Determinant3by3(Vector2 A, Vector2 B, Vector2 C) {
			return A.X * B.Y * 1 + A.Y * 1 * C.X + 1 * B.X * C.Y - 1 * B.Y * C.X - A.Y * B.X * 1 - A.X * 1 * C.Y;
		}

		// Verify if a given vector is inside the circumcircle.
		public bool InCircumcircle(Vector2 D) {
			return (this.squaredCircumradius -(this.circumcenter - D).LengthSquared() > 0);
		}

		// Verify if a given vector is a point in the triangle.
		private bool IsVertex(Vector2 A) {
			return (this.A == A || this.B == A || this.C == A);
		}

		// Verify if this triangle shares a edge with another one.
		public bool HasSharedVertex(Triangle other) {
			return this.IsVertex (other.A) || this.IsVertex (other.B) || this.IsVertex (other.C);
		}

		// Return the shared adge between this triangle and another.
		// It is assumed that there is only 1 such edge because otherwise, there would be 3 when they are the same
		// triangles.
		public Edge SharedEdge (Triangle other) {
		
			Edge[] Edges1 = { this.AB, this.BC, this.CA };
			Edge[] Edges2 = { other.AB, other.BC, other.CA };
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					if (Edges1[i].Equals(Edges2[j])) {
						return Edges1 [i];
					}
				}
			}
			return null;
		}

		// Return the vector of the circumcenter.
		private Vector2 GetCircumcenter() {
			float Axy =     Determinant3by3 (new Vector2 (A.X, A.Y), new Vector2 (B.X, B.Y), new Vector2 (C.X, C.Y));
			float Bx = -1 * Determinant3by3 (new Vector2 (A.X*A.X+A.Y*A.Y, A.Y), new Vector2 (B.X*B.X+B.Y*B.Y, B.Y), new Vector2 (C.X*C.X+C.Y*C.Y, C.Y));
			float By = 		Determinant3by3 (new Vector2 (A.X*A.X+A.Y*A.Y, A.X), new Vector2 (B.X*B.X+B.Y*B.Y, B.X), new Vector2 (C.X*C.X+C.Y*C.Y, C.X));
			return new Vector2 (-Bx / (2 * Axy), -By / (2 * Axy));
		}

		// Return the squared circumradius.
		private float GetSquaredCircumradius() {
			float A = this.AB.distance;
			float B = this.BC.distance;
			float C = this.CA.distance;
			return (A * B * C)*(A * B * C) / ((A + B + C) * (B + C - A) * (A + C - B) * (A + B - C));
		}

		// Return the coordinates of the circumcenter has a float array.
		public float[] GetFloatCircumcenter() {
			return new float[] { circumcenter.X, circumcenter.Y };
		}

		// Return of the edges as an array.
		public Edge[] GetEdges() {
			return new Edge[] { this.AB, this.BC, this.CA };
		}

		// Override Equals and GetHashCode to make sure they behave correctly.
		public override bool Equals(object obj)
		{
			if (!(obj is Triangle))
				return false;
			return Equals((Triangle)obj);
		}

		public bool Equals(Triangle other)
		{
			return this.AB.Equals(other.AB) && BC.Equals(other.BC) && CA.Equals(other.CA);
		}

		public override int GetHashCode()
		{
			int hash = this.AB.GetHashCode();
			hash = (((hash << 5) + hash) ^ this.BC.GetHashCode ());
			hash = (((hash << 5) + hash) ^ this.CA.GetHashCode ());
			return hash;
		}



	}
}
