# CS-DelaunayTriangulation

A simple C# library that generates 2D Delaunay triangulations from generic types. It is based a sweepline algorithm were triangles are processed from left to right.

## Getting Started

These instructions will explain how to integrate this library to your project. The examples are from Unity.

### Installing

1. Download and add all the files in /DelaunayTriangulation to your projet.
2. Import the library to the .cs files where it will be used.
```
using DelaunayTriangulation;
```
3. Make sure the class that will provide the positions for the triangulation implements the IPosition interface.
```
public class Asteroid : MonoBehaviour, IPosition {
// [...]
   publit float[] GetPosition() {
       return new float[] {this.gameObject.transform.position.x, this.gameObject.transform.position.y};
   }
// [...]
}
```
4. Create and instantitate a Delauny object. Make sure to specify the class implementing IPosition as the generic type and to pass an  array of the that type as parameter.
```
Delaunay<Asteroid> triangulation;
triangulation = new Delaunay<Asteroid>(GameObject.FindObjectsOfType<Asteroid> ());
```
5. If you want the Delaunay triangulation. Get the object pairs forming the the Delaunay triangulation. Make sure to create the proper data structure to receive the data. The following example draws debug lines.
```
Asteroid[][] delaunayPairs = triangulation.GetDelaunayPairs();
for (int i = 0; i < delaunayPairs.GetLength(0); i++) {
    UnityEngine.Debug.DrawLine (delaunayPairs [i][0].transform.position, delaunayPairs [i][1].transform.position, Color.green, 60f);
}
```
6. If you want the Voronoi diagram. Get the pairs of coordinates formaing the Voronoi diagram. Again, make sure to create the proper data structure to receive the data. The following example draws debug lines.
```
float[][][] voronoiPairs = triangulation.GetVoronoiPairs();
for (int i = 0; i < pairs.Length; i++) {x
    UnityEngine.Debug.DrawLine (new Vector3(voronoiPairs[i][0][0],voronoiPairs[i][0][1]), new Vector3(voronoiPairs[i][1][0],voronoiPairs[i][1][1]), Color.red, 60f);
    }
```

## Example

![Delaunay triangulation with Voronoi diagram example](Delaunay-Voronoi.jpg?raw=true "Delaunay triangulation with Voronoi diagram example.")

## Authors

* **Pierre-André Gagnon** - *Initial work* - [pag4k](https://github.com/pag4k)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for xxxxxxxxdfsdsdsd

## Acknowledgmentsxxx

* Thanks to Sjaak Priester (https://www.codeguru.com/cpp/cpp/algorithms/general/article.php/c8901/Delaunay-Triangles.htm).
