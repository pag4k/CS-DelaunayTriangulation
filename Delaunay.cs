/// <copyright file="Delaunay.cs" company="">
/// Pierre-Andre Gagnon, https://github.com/pag4k
/// </copyright>
/// <summary>
/// This is the main class of the DelaunayTriangulation library.
/// It is a generic class with which a triangulation can be performed on any type implementing the IPosition interface.
/// It has two main public functions:
/// GetObjectPairs(): This fuctionc returns pairs of generic objects that are adjacent in the triangulation.
/// GetCircumcenterPairs(): This functions returns pairs of coordinates representing the edges constituting the Voronoi
/// diagram corresponding to the Delaunay triangulation.
/// </summary>
/// 
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DelaunayTriangulation
{
	public class Delaunay<T> where T: IPosition {

		// Map from the vertices that will be triangulated and their respective object of type T.
		// This is needed to return a triangulation with references to the original objets.
		private Dictionary<Vector2,T> vertexToObjectMap;
		// Map from the edges to the triangles they are part of.
		// This is needed to simplify the construction of the Voronoi diagram based on the triangulation.
		private Dictionary<Edge,Triangle[]> edgeToTriangleMap;
		// Triangles that have been processed. That means that they won't be modified and thus, are part of the
		// final triangulation.
		private List<Triangle> closedTriangles;
		// The perimeter triangle will be constructed based on the vertix positions so that its area covers them all.
		// This is needed for the triangulation algorithm.
		private Triangle perimeterTriangle;

		public Delaunay (T[] objectArray) {
			if (objectArray == null) {
				return;
			}
			if (objectArray.Length < 3) {
				return;
			}

			// The number of objects to be processed as vertices.
			int numberObjects = objectArray.Length;
			// List of all the vertices from which the triangulation has to be done.
			List<Vector2> vertices = new List<Vector2>();
			// Triangles to be processed. That means that could be further divided in smalled triangles.
			List<Triangle> openTriangles = new List<Triangle>();

			closedTriangles = new List<Triangle>();
			vertexToObjectMap = new Dictionary<Vector2,T> ();
			edgeToTriangleMap = new Dictionary<Edge, Triangle[]> ();


			// Declare floats to hold the min and max x and y.
			float minX = float.PositiveInfinity; 
			float maxX = float.NegativeInfinity;
			float minY = float.PositiveInfinity;
			float maxY = float.NegativeInfinity;

			// Loop over all the objects that were given as parameter.
			for (int i = 0; i < numberObjects; i++) {
				// Get the position of the ITriangulable object.
				float[] position = objectArray [i].getPosition ();
				if (position == null) {
					return;
				}
				if (position.Length != 2) {
					return;
				}

				// Create a Vector2 corresponsding to the object position.
				Vector2 vertex = new Vector2 (position [0], position [1]);
				// Adjust the min and max x and y.
				minX = (vertex.X < minX) ? vertex.X : minX;
				maxX = (vertex.X > maxX) ? vertex.X : maxX;
				minY = (vertex.Y < minY) ? vertex.Y : minY;
				maxY = (vertex.Y > maxY) ? vertex.Y : maxY;

				// Add the vertex to the vertices list.
				vertices.Add (vertex);
				// Add the vertex with its corresponding object to the vertexToObjectMap dictionnary.
				vertexToObjectMap.Add (vertex, objectArray [i]);
			}

			// Create a perimeter triangle that contains all the vertex based on the min and max x and y.
			// This algorithm is based on a circle inscribed into an equilateral triangle.
			// To overestimate the size of the triangle the biggest distance between the x's and the y's is used as the
			// radius of the circle.
			float distanceX = System.Math.Abs(maxX - minX);
			float distanceY = System.Math.Abs(maxY - minY);
			float averageX = (minX + maxX) / 2;
			float averageY = (minY + maxY) / 2;
			float triangleRadius = (distanceX > distanceY) ? distanceX : distanceY;
			float triangleSize = 2 * (float)System.Math.Sqrt(3) * triangleRadius;
			float triangleHeight = (float)System.Math.Sqrt(3) * triangleSize / 2;
			Vector2 perimeterVertexA = new Vector2 (averageX, averageY + triangleHeight - triangleRadius);
			Vector2 perimeterVertexB = new Vector2 (averageX - triangleSize / 2, averageY - triangleRadius);
			Vector2 perimeterVertexC = new Vector2 (averageX + triangleSize / 2, averageY - triangleRadius);
			perimeterTriangle = new Triangle (perimeterVertexA, perimeterVertexB, perimeterVertexC);
			numberObjects += 3;

			// Add the vertices of the perimeter triangle 
			openTriangles.Add(perimeterTriangle);

			// Sort the vertices based on their x position.
			vertices.Sort((a,b) => a.X.CompareTo(b.X));

			foreach (Vector2 currentVertex in vertices) {

				// Create a list of edges to hold the ones the edges of the triangles that will be modifed in this
				// iteration.
				List<Edge> edgeBuffer = new List<Edge>();

				// For each triangle in openTriangles, do two verification:
				// 1. If the triangle circumcircle is entirely to the left (along the x-axis) of current vertex, that
				// triangle is done and add it to the triangulation.
				// 2. If currentVertex is its circumcircles, add its edges to edgeBuffer and remove it from
				// openTriangles.
				for (int i = openTriangles.Count - 1; i > -1 ; i--) {
					if (openTriangles [i].squaredCircumradius - (currentVertex.X-openTriangles [i].circumcenter.X)*(currentVertex.X-openTriangles [i].circumcenter.X) < 0) {
						AddTriangle (openTriangles [i]);
						openTriangles.RemoveAt(i);
						continue;
					}
					if (openTriangles[i].InCircumcircle(currentVertex)) {
						edgeBuffer.Add (openTriangles[i].AB);
						edgeBuffer.Add (openTriangles[i].BC);
						edgeBuffer.Add (openTriangles[i].CA);
						openTriangles.RemoveAt(i);
					}
				}

				// Find the edges in edgeBuffer that only appear once in the list.
				List<Edge> uniqueEdgeBuffer = ReturnUniqueEdges (edgeBuffer);

				// For each edge in uniqueEdgeBuffer, add a new triangle in openTriangles based on the edge and
				// currentVertex.
				foreach (Edge uniqueEdge in uniqueEdgeBuffer) {
					openTriangles.Add (new Triangle (uniqueEdge, currentVertex));
				}
			}
		
			// Transfert the remaining triangles from openTriangles to the triangulation.
			// Because of the sweepline algorithm, the triangles whose circumcirle is not entirely left of the last
			// vertex were not transfered in closedTriangles.
			for (int i = 0; i < openTriangles.Count ; i++) {
				AddTriangle(openTriangles [i]);
//				UnityEngine.Debug.DrawLine (new Vector3(openTriangles[i].A.X,openTriangles[i].A.Y), new Vector3(openTriangles[i].B.X,openTriangles[i].B.Y), Color.red, 100f);
//				UnityEngine.Debug.DrawLine (new Vector3(openTriangles[i].B.X,openTriangles[i].B.Y), new Vector3(openTriangles[i].C.X,openTriangles[i].C.Y), Color.red, 100f);
//				UnityEngine.Debug.DrawLine (new Vector3(openTriangles[i].C.X,openTriangles[i].C.Y), new Vector3(openTriangles[i].A.X,openTriangles[i].A.Y), Color.red, 100f);
			}

			//DrawDebugLine(closedTriangles);

		}

		private void AddTriangle(Triangle triangle) {
			// Only add the triangle in the final list if it does not share an edge with the perimeter triangle.
			if (triangle.HasSharedVertex (perimeterTriangle) == true) {
				return;
			}
			// Add the triangle.
			closedTriangles.Add (triangle);
			// Add the triangle and all of its edges in the edgeToTriangleMap.
			Edge[] edges = triangle.GetEdges ();
			for (int i = 0; i < 3; i++) {
				if (edgeToTriangleMap.ContainsKey (edges [i])) {
					edgeToTriangleMap [edges [i]] [1] = triangle;
				} else {
					edgeToTriangleMap.Add(edges [i], new Triangle[2]);
					edgeToTriangleMap [edges [i]] [0] = triangle;
				}
			}

		}

		/// <summary>
		/// Gets pairs of objects corresponding to the triangulation.
		/// The objects are of the type given as generic parameter when the Delaunay object was created.
		/// </summary>
		/// <returns> It returns a 2D jagged where the first level represents the pairs that form edges in the
		/// triangulation and the second level represents the two objects of the each pair. </returns>
		public T[][] GetDelaunayPairs () {
			// Create a hashset and add to it all the edges in the triangulation.
			// The hashmap guarantee that there are no duplicates.
			HashSet<Edge> edgeList = new HashSet<Edge> ();
			foreach (Triangle triangle in closedTriangles) {
					edgeList.Add (triangle.AB);
					edgeList.Add (triangle.BC);
					edgeList.Add (triangle.CA);
			}

			// Create the object array that will be returned.
			T[][] objectPairsArray = new T[edgeList.Count][];

			// Convert all the edges to their respective coordinates and add them to the output array.
			int counter = 0;
			foreach (Edge edge in edgeList) {
				objectPairsArray [counter] = new T[2];
				objectPairsArray [counter][0] = vertexToObjectMap [edge.A];
				objectPairsArray [counter][1] = vertexToObjectMap [edge.B];
				counter++;
			}

			return objectPairsArray;
		}

		/// <summary>
		/// Gets pairs of objects corresponding to the triangulation.
		/// The objects are of the type given as generic parameter when the Delaunay object was created.
		/// </summary>
		/// <returns> It returns a 2D jagged where the first level represents the pairs that form edges in the
		/// triangulation and the second level represents the two objects of the each pair. </returns>
		public float[][][] GetVoronoiPairs () {

			// Create a list of 2D jagged array to hold the pairs of circumcenter.
			// Each element of the list corresponds to a pair.
			// The first level (length 2) corresponds to the two elements of each pair.
			// The second level (length 2) corresponds to the two coordinates, respectively x and y.
			List<float[][]> edgeList = new List<float[][]> ();
			foreach (Edge edge in edgeToTriangleMap.Keys) {
				if (edgeToTriangleMap [edge] [1] == null) {
					continue;
				}
				float[] position1 = edgeToTriangleMap [edge] [0].GetFloatCircumcenter ();
				float[] position2 = edgeToTriangleMap [edge] [1].GetFloatCircumcenter ();
				if (position1 [0] == position2 [0] && position1 [1] == position2 [1]) {
					continue;
				}
				edgeList.Add (new float[][] { position1, position2 });
			}

			// Create the float array that will be returned.
			float[][][] circumcenterPairsArray = new float[edgeList.Count][][];

			// Convert the list to a 3D float array.
			int counter = 0;
			foreach (float[][] edge in edgeList) {
				circumcenterPairsArray [counter] = edge;
				counter++;
			}

			return circumcenterPairsArray;
		}


		// A function that takes a list of edges and return a list of edge only containing the edges that were unique
		// in the original list.
		private List<Edge> ReturnUniqueEdges(List<Edge> edgesList) {
			List<Edge> uniqueEdges = new List<Edge> ();
			//HashSet<Edge> duplicatedEdges = new HashSet<Edge> ();

//			Dictionary<Edge,int> edgeOccurence = new Dictionary<Edge, int> ();
//			foreach (Edge edge in edgesList) {
//				if (edgeOccurence.ContainsKey (edge)) {
//					edgeOccurence[edge] = 2;
//				} else {
//					edgeOccurence.Add (edge, 1);
//				}
//			}
//
//			foreach (Edge edge in edgeOccurence.Keys) {
//				if (edgeOccurence [edge] == 1) {
//					uniqueEdges.Add (edge);
//				}
//			}



//			foreach (Edge currentEdge in edgesList) {
//				if (duplicatedEdges.Contains (currentEdge) == false) {
//					if (uniqueEdges.Contains (currentEdge) == true) {
//						uniqueEdges.Remove (currentEdge);
//						duplicatedEdges.Add (currentEdge);
//					} else {
//						uniqueEdges.Add (currentEdge);
//					}
//				}
//			}

			// Loop backward over the list and successively remove the edges as they are checked for uniqueness.
			List<int> indexEdge = new List<int>();
			for (int i = edgesList.Count - 1; i > -1; i--) {
				Edge currentEdge = edgesList [i];
				edgesList.RemoveAt (i);
				indexEdge.Clear();
				for (int j = 0; j < i; j++) {
					if (currentEdge.Equals(edgesList [j])) {
						indexEdge.Add (j);
					}
				}
				int found = indexEdge.Count;
				if (found > 0) {
					for (int j = found - 1; j > -1; j--) {
						edgesList.RemoveAt (indexEdge[j]);
					}
				i = i - found;
				} else  if (found == 0) {
					uniqueEdges.Add(currentEdge);
				} else {
					// This should never happen.
				}
			}
			return uniqueEdges;
		}

//		private Triangle[] GetNeighboursTriangles(Triangle t1) {
//			Triangle[] neighboursTriangles = new Triangle[3];
//			int i = 0;
//			foreach (Triangle t2 in closedTriangles) {
//				if (t1.Equals (t2)) {
//					continue;
//				}
//				if (t1.SharedEdge (t2) != null) {
//					neighboursTriangles[i] = t2;
//					i++;
//				}
//			}
//			return neighboursTriangles;
//		}
			
//		void DrawDebugLine(List<Triangle> triangles_data) {
//			foreach(Triangle triangle in triangles_data) {
//				UnityEngine.Debug.DrawLine (new Vector3(triangle.A.X,triangle.A.Y), new Vector3(triangle.B.X,triangle.B.Y), Color.green, 100f);
//				UnityEngine.Debug.DrawLine (new Vector3(triangle.B.X,triangle.B.Y), new Vector3(triangle.C.X,triangle.C.Y), Color.green, 100f);
//				UnityEngine.Debug.DrawLine (new Vector3(triangle.C.X,triangle.C.Y), new Vector3(triangle.A.X,triangle.A.Y), Color.green, 100f);
//			}
//		}

	}
}