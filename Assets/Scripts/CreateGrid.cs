using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour {
    // Street following grid layout
    public static int[,] Grid;
    public static int Columns = 100;
    public static int Rows = 200;
    public int Seed = 10;
    
    // Pre-made textures to be used
    public Material BuildingOuter;
    public Material StreetVertical;
    public Material BuildingInner;
    public Material StreetHorizontal;

    // Building Prefabs to be placed in
    public GameObject Building1;
    public GameObject Building2;
    public GameObject Building3;
    public GameObject Building4;
    public GameObject Building5;
    public GameObject Building6;

    // The vertices of the mesh
	private Vector3[] Verts;

	// The triangles of the mesh (triplets of integer references to vertices)
	private int[] Tris;
    
	// The number of triangles that have been created so far
	private int Ntris = 0;

    // Start is called before the first frame update
    void Start() {
        Random.InitState(Seed);

        Grid = new int[Columns, Rows];

        GameObject grid = new GameObject("Grid");

        // Rules for creating streets
        for (int i = 0; i < Columns; i++) {
            for (int j = 0; j < Rows; j++) {
                if (i % 4 == 0) {
                    Grid[i, j] = 1;
                } else if (j % 3 == 1) {
                    Grid[i, j] = 1;
                } else if (j % 2 == 1) {
                    Grid[i, j] = Random.Range(0, 2);
                } else if (j % 15 == 0) {
                    Grid[i, j] = Random.Range(0, 2);
                } else {
                    Grid[i, j] = 0;
                }

                MakeTile(i, j, Grid[i, j], grid);
            }
        }
    }
    
    // Generate a particular tile at a position
    void MakeTile(int x, int y, int value, GameObject grid) {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Plane);
        g.name = "(" + x + ", " + y + ")";
        g.transform.position = new Vector3(x, 0, y);
        g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Renderer r = g.GetComponent<Renderer>();

        // Make appropriate rotations
        if (x % 8 == 0) {
            g.transform.Rotate(0f, 180f, 0f);
        } else if (x % 4 != 0 && y % 2 == 0) {
            g.transform.Rotate(0f, 180f, 0f);
        }

        // Draw texture for tile based on whether it's street or not
        if (value == 1) {
            if (x % 4 == 0 && y % 3 == 1 && x > 0) {
                g.transform.position = new Vector3(x, -0.01f, y);
                r.material = StreetVertical;
                
                // Make 'Street Component' (includes turns, 4-way int, T-junct)
                GameObject m;
                m = new GameObject("Street Component");
                m.transform.position = new Vector3(x - 0.5f, 0, y - 0.5f);
                Mesh my_mesh = CreateMyMesh();
                Ntris = 0; // reset to 0, else will cause Index Out of Bounds
                m.AddComponent<MeshFilter>();
                m.AddComponent<MeshRenderer>();
                m.GetComponent<MeshFilter>().mesh = my_mesh;
                Renderer rend = m.GetComponent<Renderer>();
                rend.material.color = new Color(0f, Random.Range(0f, 1f), 0f);
                m.transform.parent = grid.transform;
            } else if (x % 4 == 0) {
                r.material = StreetVertical;
            } else if (x % 4 != 0) {
                r.material = StreetHorizontal;
            } 
        } else {
            if (x >= Columns / 4 && x <= Columns * 0.75 
                    && y >= Rows / 4 && y <= Rows * 0.75) {
                r.material = BuildingInner;
            } else {
                r.material = BuildingOuter; 
            }

            // Place buildings
            if (x <= 2 || x >= Columns - 2 || y <= 2 || y >= Rows - 2) {
                GameObject building;
                building = (GameObject)Instantiate(Building3, 
                    g.transform.position, g.transform.rotation);
                building.transform.position += new Vector3(0f, 0.25f, 0f);
                building.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                building.transform.parent = grid.transform;
            } else if (x == 38 || (x == 40 && y == 67)) {
                GameObject building;
                building = (GameObject)Instantiate(Building5, 
                    g.transform.position, g.transform.rotation);
                building.transform.position += new Vector3(0f, 0.34f, 0f);
                building.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                building.transform.parent = grid.transform;
            } else if (x == 42 && y % 6 == 0) {
                GameObject building;
                building = (GameObject)Instantiate(Building6, 
                    g.transform.position, g.transform.rotation);
                building.transform.position += new Vector3(0f, 0.77f, 0f);
                building.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                building.transform.parent = grid.transform;
            } else if (x == Columns / 2 || y == Rows / 2) {
                GameObject building;
                building = (GameObject)Instantiate(Building4, 
                    g.transform.position, g.transform.rotation);
                building.transform.position += new Vector3(0f, 0.6f, 0f);
                building.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                building.transform.parent = grid.transform;
            } else if (x == Columns / 4 || y == Rows / 4) {
                GameObject building;
                building = (GameObject)Instantiate(Building1,
                    g.transform.position, g.transform.rotation);
                building.transform.position += new Vector3(0f, 0.25f, 0f);
                building.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                building.transform.parent = grid.transform;
            } else if (x == Columns * 0.75 || y == Rows * 0.75) {
                GameObject building;
                building = (GameObject)Instantiate(Building2, 
                    g.transform.position, g.transform.rotation);
                building.transform.position += new Vector3(0f, 0.6f, 0f);
                building.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                building.transform.parent = grid.transform;
            }
        }

        g.transform.parent = grid.transform;
    }

    // Creates a mesh for a 4 way intersection
    Mesh CreateMyMesh() {
		// create a mesh object
		Mesh mesh = new Mesh();

		// vertices of the mesh
		int num_Verts = 32;
		Verts = new Vector3[num_Verts];

		// vertices 
		Verts[0] = new Vector3(0, 0, 0);
		Verts[1] = new Vector3(0, 0, 0.4f);
		Verts[2] = new Vector3(0.2f, 0, 0);
		Verts[3] = new Vector3(0.2f, 0, 0.4f);

		Verts[4] = new Vector3(0.2f, 0, 0);
		Verts[5] = new Vector3(0.2f, 0, 0.2f);
		Verts[6] = new Vector3(0.4f, 0, 0f);
		Verts[7] = new Vector3(0.4f, 0, 0.2f);

		Verts[8] = new Vector3(0, 0, 0.6f);
		Verts[9] = new Vector3(0, 0, 1);
		Verts[10] = new Vector3(0.2f, 0, 0.6f);
		Verts[11] = new Vector3(0.2f, 0, 1);

		Verts[12] = new Vector3(0.2f, 0, 0.8f);
		Verts[13] = new Vector3(0.2f, 0, 1);
		Verts[14] = new Vector3(0.4f, 0, 0.8f);
		Verts[15] = new Vector3(0.4f, 0, 1);

		Verts[16] = new Vector3(0.6f, 0, 0);
		Verts[17] = new Vector3(0.6f, 0, 0.2f);
		Verts[18] = new Vector3(1, 0, 0);
		Verts[19] = new Vector3(1, 0, 0.2f);

		Verts[20] = new Vector3(0.8f, 0, 0.2f);
		Verts[21] = new Vector3(0.8f, 0, 0.4f);
		Verts[22] = new Vector3(1, 0, 0.2f);
		Verts[23] = new Vector3(1, 0, 0.4f);

		Verts[24] = new Vector3(0.6f, 0, 0.8f);
		Verts[25] = new Vector3(0.6f, 0, 1);
		Verts[26] = new Vector3(0.8f, 0, 0.8f);
		Verts[27] = new Vector3(0.8f, 0, 1);

		Verts[28] = new Vector3(0.8f, 0, 0.6f);
		Verts[29] = new Vector3(0.8f, 0, 1);
		Verts[30] = new Vector3(1, 0, 0.6f);
		Verts[31] = new Vector3(1, 0, 1);

		int num_Tris = 16;
		Tris = new int[num_Tris * 3]; // need 3 vertices per triangle

		// make the rectangles from vertices
		MakeQuad(0, 1, 2, 3);
		MakeQuad(4, 5, 6, 7);
		MakeQuad(8, 9, 10, 11);
		MakeQuad(12, 13, 14, 15);
		MakeQuad(16, 17, 18, 19);
		MakeQuad(20, 21, 22, 23);
		MakeQuad(24, 25, 26, 27);
		MakeQuad(28, 29, 30, 31);

		// save the vertices and triangles in the mesh object
		mesh.vertices = Verts;
		mesh.triangles = Tris;

		// automatically calculate the vertex normals
		mesh.RecalculateNormals();

		return (mesh);
	}

	// Make a triangle from three vertex indices (clockwise order)
	void MakeTri(int i1, int i2, int i3) {
		// figure out the base index for storing triangle indices
		int index = Ntris * 3;
		Ntris++;

		Tris[index] = i1;
		Tris[index + 1] = i2;
		Tris[index + 2] = i3;
	}

	// Make a quadrilateral from four vertex indices (clockwise order)
	void MakeQuad(int i1, int i2, int i3, int i4) {
		MakeTri(i1, i2, i3);
		MakeTri(i3, i2, i4);
	}

    // Update is called once per frame
	void Update () {
		return;
	}
}
