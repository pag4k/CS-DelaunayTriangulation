/// <copyright file="IPosition.cs" company="">
/// Pierre-Andre Gagnon, https://github.com/pag4k
/// </copyright>
/// <summary>
/// This interfaced is needed to do the triangulation based on objects implenting it and to return the result with
/// references to them.
/// It only contains a function that must return the 2D position of the object as a float array containing respectively
// /the x and y coordinates.
/// </summary>

namespace DelaunayTriangulation
{
	public interface IPosition {
		float[] getPosition ();

	}
}