using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCreatures : MonoBehaviour {
    // master random seed so that a user can create different creatures.
    public int Seed = 1;

    // number of creatures to be created
    public int numCreatures = 5;

    // the vertices of the mesh
	private Vector3[] verts;
	// the triangles of the mesh (triplets of integer references to vertices)
	private int[] tris;
	// the number of triangles that have been created so far
	private int ntris = 0;

    // Start is called before the first frame update
    void Start() {
        for (int i = 1; i < numCreatures + 1; i++) {
            BuildCreature(i);
        }
    }

    // Puts all body parts for a creature together
    void BuildCreature(int i) {
        int rand = Random.Range(i + 1, i + 10); // used for position and color
        float chance = Random.Range(0f, 1f); // Bless the RNG!
        float size = chance * rand; // for size variation

        MakeHead(rand, size); // Beaks swappable
        MakeBody(rand, size);
        // Swappable arms, legs and tail
        for (int j = 1; j < 3; j++) {
            if (rand % 2 == 0) {
                MakeArm1(rand, j, size);
                MakeLeg1(rand, j, size);
            } else if (rand % 3 == 0) {
                MakeArm2(rand, j, size);
                MakeLeg2(rand, j, size);
            } else if (rand % 5 == 0) {
                MakeArm3(rand, j, size);
                MakeLeg3(rand, j, size);
            } else if (rand % 7 == 0) {
                MakeArm1(rand, j, size);
                MakeLeg2(rand, j, size);
            } else if (rand % 11 == 0) {
                MakeArm1(rand, j, size);
                MakeLeg3(rand, j, size);
            } else if (rand % 13 == 0) {
                MakeArm2(rand, j, size);
                MakeLeg1(rand, j, size);
            } else if (rand % 17 == 0) {
                MakeArm2(rand, j, size);
                MakeLeg3(rand, j, size);
            } else if (rand % 19 == 0) {
                MakeArm3(rand, j, size);
                MakeLeg1(rand, j, size);
            } else {
                MakeArm3(rand, j, size);
                MakeLeg2(rand, j, size);
            }   
        }
        if (chance < 0.3f) {
            MakeTail1(rand, size);
        } else {
            MakeTail2(rand, size);
        }
    }
    
    // Generates a head for the creature (includes skull, eyes, beak)
    void MakeHead(int rand, float size) {
        // Skull
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.position = size * new Vector3(0 + 2 * rand, 2, 0);                
        head.transform.localScale = size * new Vector3(0.75f, 1, 0.75f);
        
        Renderer renderer = head.GetComponent<Renderer>();
        renderer.material.color = new Color(0f, 0f, Random.Range(0f, 1f));
        Texture2D texture = TextureMap(rand);
        renderer.material.mainTexture = texture;

        // Beak
        GameObject beak = new GameObject("Beak");
        Mesh bk; // 2 different styles (swappable)
        if (rand % 2 == 0) {
            beak.transform.position = size * 
                new Vector3(0 + 2 * rand - 0.1f, 1.8f, -0.3f);
            beak.transform.localScale = size * new Vector3(0.2f, 0.2f, 0.2f);
            beak.transform.Rotate(-90f, 0, Random.Range(20f, 45f));
            bk = CreateBeak1();
        } else {
            beak.transform.position = size * 
                new Vector3(0 + 2 * rand - 0.1f, 2f, -0.45f);
            beak.transform.localScale = size * new Vector3(0.1f, 0.1f, 0.1f);
            beak.transform.Rotate(-90f, 0, 150f);
            bk = CreateBeak2();
        }
        ntris = 0; // reset to 0, else will cause Index Out of Bounds
        // Apply subdivision surfaces to mesh several times
        for (int i = 0; i < 2; i++) {
            bk = SubDivision(bk);
            ntris = 0; // reset to 0, else will cause Index Out of Bounds
        }

        beak.AddComponent<MeshFilter>();
        beak.AddComponent<MeshRenderer>();
        beak.GetComponent<MeshFilter>().mesh = bk;
        Renderer rend = beak.GetComponent<Renderer>();
        rend.material.color = new Color(0f, 1f, Random.Range(0f, 1f));
        Texture2D text = TextureMap(rand);
        rend.material.mainTexture = text;

        // Eyes (left and right)
        CreateEye(rand, 1, size);
        CreateEye(rand, 2, size);
    }

    // Creates an eye in the shape of two merged spheres
    void CreateEye(int rand, int side, float size) {
        float shift; // determines right of left eye
        if (side == 1) {
            shift = 0.2f;
        } else {
            shift = -0.2f;
        }

        // Eyeball
        GameObject eyeball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eyeball.name = "Eyeball";
        eyeball.transform.position 
            = size * new Vector3(0 + 2 * rand + shift, 2.3f, -0.15f);                
        eyeball.transform.localScale = size * new Vector3(0.2f, 0.2f, 0.2f);
        
        Renderer r1 = eyeball.GetComponent<Renderer>();
        r1.material.color = new Color(1, 0.92f, 0.016f);
        Texture2D t1 = TextureMap(rand);
        r1.material.mainTexture = t1;

        // Iris
        GameObject iris = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        iris.name = "Iris";
        iris.transform.position 
            = size * new Vector3(0 + 2 * rand + shift, 2.35f, -0.15f - 0.05f);                
        iris.transform.localScale = size * new Vector3(0.1f, 0.1f, 0.1f);
        
        Renderer r2 = iris.GetComponent<Renderer>();
        r2.material.color = new Color(0f, 0f, 0f);
        Texture2D t2 = TextureMap(rand);
        r2.material.mainTexture = t2;
    }

    // Creates beak variation 1 in the shape of a square based pyramid
    Mesh CreateBeak1() {
        Mesh mesh = new Mesh();
        
        // vertices of the mesh
        int num_verts = 18;
		verts = new Vector3[num_verts];

		// vertices 
		verts[0] = new Vector3(0, 0, 1);
		verts[1] = new Vector3(0.5f, 1, 0.5f);
		verts[2] = new Vector3(0, 0, 0);

		verts[3] = new Vector3(0, 0, 0);
		verts[4] = new Vector3(0.5f, 1, 0.5f);
		verts[5] = new Vector3(1, 0, 0);

		verts[6] = new Vector3(0.5f, 1, 0.5f);
		verts[7] = new Vector3(1, 0, 1);
		verts[8] = new Vector3(1, 0, 0);

		verts[9] = new Vector3(0.5f, 1, 0.5f);
		verts[10] = new Vector3(0, 0, 1);
		verts[11] = new Vector3(1, 0, 1);

		verts[12] = new Vector3(0, 0, 1);
		verts[13] = new Vector3(0, 0, 0);
		verts[14] = new Vector3(1, 0, 1);

		verts[15] = new Vector3(1, 0, 1);
        verts[16] = new Vector3(0, 0, 0);
        verts[17] = new Vector3(1, 0, 0);

		int num_tris = 6;
		tris = new int[num_tris * 3]; // need 3 vertices per triangle

		// make the rectangles from vertices
        MakeTri(0, 1, 2);
        MakeTri(3, 4, 5);
        MakeTri(6, 7, 8);
        MakeTri(9, 10, 11);
        MakeTri(12, 13, 14);
		MakeTri(15, 16, 17);

		// save the vertices and triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;

		// automatically calculate the vertex normals
		mesh.RecalculateNormals();

        return (mesh);
    }

    // Create a tetrahedron style for beak variation 2.
	Mesh CreateBeak2() {
		// create a mesh object
		Mesh mesh = new Mesh();

		// vertices of a tetrahedron
		int num_verts = 12;
		verts = new Vector3[num_verts];

		// vertices for faces of the tetrahedron
		verts[0] = new Vector3(-1, 1, -1);
		verts[1] = new Vector3(-1, -1, 1);
		verts[2] = new Vector3(1, 1, 1);

		verts[3] = new Vector3(1, -1, -1);
		verts[4] = new Vector3(-1, 1, -1);
		verts[5] = new Vector3(1, 1, 1);

		verts[6] = new Vector3(-1, -1, 1);
		verts[7] = new Vector3(-1, 1, -1);
		verts[8] = new Vector3(1, -1, -1);

		verts[9] = new Vector3(-1, -1, 1);
		verts[10] = new Vector3(1, -1, -1);
		verts[11] = new Vector3(1, 1, 1);

		int num_tris = 4; // need 4 triangles for 4 faces of tetrahedron
		tris = new int[num_tris * 3]; // need 3 vertices per triangle

		// make the triangles from vertices
		MakeTri(0, 1, 2);
		MakeTri(3, 4, 5);
		MakeTri(6, 7, 8);
		MakeTri(9, 10, 11);

		// save the vertices and triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;

		// automatically calculate the vertex normals
		mesh.RecalculateNormals();

		return (mesh);
	}

    // Generates the main body/ fuslage for the creature (Capsule)
    void MakeBody(int rand, float size) {
        GameObject bod = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        bod.name = "Body";
        bod.transform.position = size * 
            new Vector3(0f + 2 * rand, 1, 0f);
        bod.transform.localScale = size * new Vector3(0.5f, 1, 0.5f);

        Renderer rend = bod.GetComponent<Renderer>();
        rend.material.color = new Color(0f, Random.Range(0f, 1f), 0f);
        Texture2D text = TextureMap(rand);
        rend.material.mainTexture = text;
    }

    // Generates variation one of creature's arm/ wing (Rectangular prism)
    void MakeArm1(int rand, int side, float size) {
        GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        arm.name = "Arm " + side;
        if (side == 1) {
            arm.transform.position = size * 
                new Vector3(0.5f + 2 * rand, 1.2f, 0);                
            arm.transform.localScale = size * new Vector3(0.1f, 1.5f, 0.1f);
            arm.transform.Rotate(0f, 0, -45f);
        } else {
            arm.transform.position = size * 
                new Vector3(-0.5f + 2 * rand, 1.2f, 0);                
            arm.transform.localScale = size * new Vector3(0.1f, 1.5f, 0.1f);
            arm.transform.Rotate(0f, 0f, 45f);
        }

        Renderer renderer = arm.GetComponent<Renderer>();
        renderer.material.color = new Color(2f / rand, 0f, 0f);
        Texture2D texture = TextureMap(rand);
        renderer.material.mainTexture = texture;
    }

    // Generates variation two of creature's arm/ wing (Planes)
    void MakeArm2(int rand, int side, float size) {
        GameObject armt = GameObject.CreatePrimitive(PrimitiveType.Plane);
        armt.name = "Arm Top " + side;
        GameObject armb = GameObject.CreatePrimitive(PrimitiveType.Plane);
        armb.name = "Arm Bottom " + side;

        if (side == 1) {
            // Top
            armt.transform.position = size * 
                new Vector3(0.4f + 2 * rand, 1.1f, 0);                
            armt.transform.localScale = size * new Vector3(0.1f, 1.5f, 0.02f);
            armt.transform.Rotate(0f, 0, -45f);
            // Bottom
            armb.transform.position = size * 
                new Vector3(0.4f + 2 * rand, 1.1f, 0);                
            armb.transform.localScale = size * new Vector3(0.1f, 1.5f, 0.02f);
            armb.transform.Rotate(0f, 0, -225f);
        } else {
            // Top
            armt.transform.position = size * 
                new Vector3(-0.4f + 2 * rand, 1.1f, 0);
            armt.transform.localScale = size * new Vector3(0.1f, 1.5f, 0.02f);
            armt.transform.Rotate(0f, 0f, 45f);
            // Bottom
            armb.transform.position = size * 
                new Vector3(-0.4f + 2 * rand, 1.1f, 0);
            armb.transform.localScale = size * new Vector3(0.1f, 1.5f, 0.02f);
            armb.transform.Rotate(0f, 0, 225f);
        }

        Renderer renderert = armt.GetComponent<Renderer>();
        renderert.material.color = new Color(2f / rand, 0f, 0f);
        Texture2D texturet = TextureMap(rand);
        renderert.material.mainTexture = texturet;
        Renderer rendererb = armb.GetComponent<Renderer>();
        rendererb.material.color = new Color(2f / rand, 0f, 0f);
        Texture2D textureb = TextureMap(rand);
        rendererb.material.mainTexture = textureb;
    }

    // Generates variation three of creature's arm/ wing (Capsule)
    void MakeArm3(int rand, int side, float size) {
        GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        arm.name = "Arm " + side;
        if (side == 1) {
            arm.transform.position = size * 
                new Vector3(0.5f + 2 * rand, 1.2f, 0);                
            arm.transform.localScale = size * new Vector3(0.1f, 0.6f, 0.1f);
            arm.transform.Rotate(0f, 0, -45f);
        } else {
            arm.transform.position = size * 
                new Vector3(-0.5f + 2 * rand, 1.2f, 0);                
            arm.transform.localScale = size * new Vector3(0.1f, 0.6f, 0.1f);
            arm.transform.Rotate(0f, 0f, 45f);
        }

        Renderer renderer = arm.GetComponent<Renderer>();
        renderer.material.color = new Color(2f / rand, 0f, 0f);
        Texture2D texture = TextureMap(rand);
        renderer.material.mainTexture = texture;
    }

    // Generates variation one of creature's leg (Capsule)
    void MakeLeg1(int rand, int side, float size) {
        GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        leg.name = "Leg " + side;
        leg.name = "Leg " + side;
        if (side == 1) {
            leg.transform.position = size * 
                new Vector3(0.2f + 2 * rand, 0.03f, -0.12f);                
            leg.transform.localScale = size * new Vector3(0.1f, 0.3f, 0.1f);
            leg.transform.Rotate(45f, 0f, 45f);
        } else {
            leg.transform.position = size * 
                new Vector3(-0.2f + 2 * rand, 0.03f, -0.12f);                
            leg.transform.localScale = size * new Vector3(0.1f, 0.3f, 0.1f);
            leg.transform.Rotate(45f, 0f, -45f);
        }

        Renderer renderer = leg.GetComponent<Renderer>();
        renderer.material.color = new Color(1f, 1f, 2f / rand);
        Texture2D texture = TextureMap(rand);
        renderer.material.mainTexture = texture;
    }

    // Generates variation two of creature's leg (Rectangular Prism)
    void MakeLeg2(int rand, int side, float size) {
        GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leg.name = "Leg " + side;
        leg.name = "Leg " + side;
        if (side == 1) {
            leg.transform.position = size * 
                new Vector3(0.2f + 2 * rand, 0.1f, 0.1f);                
            leg.transform.localScale = size * new Vector3(0.1f, 0.5f, 0.1f);
            leg.transform.Rotate(-45f, 0f, 45f);
        } else {
            leg.transform.position = size * 
                new Vector3(-0.2f + 2 * rand, 0.1f, 0.1f);     
            leg.transform.localScale = size * new Vector3(0.1f, 0.5f, 0.1f);
            leg.transform.Rotate(-45f, 0f, -45f);
        }

        Renderer renderer = leg.GetComponent<Renderer>();
        renderer.material.color = new Color(1f, 1f, 2f / rand);
        Texture2D texture = TextureMap(rand);
        renderer.material.mainTexture = texture;
    }

    // Generates variation three of creature's leg (Cylinder)
    void MakeLeg3(int rand, int side, float size) {
        GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leg.name = "Leg " + side;
        if (side == 1) {
            leg.transform.position = size * 
                new Vector3(0.2f + 2 * rand, 0.09f, -0.13f);                
            leg.transform.localScale = size * new Vector3(0.1f, 0.3f, 0.1f);
            leg.transform.Rotate(45f, 0f, 45f);
        } else {
            leg.transform.position = size * 
                new Vector3(-0.2f + 2 * rand, 0.09f, -0.13f);                
            leg.transform.localScale = size * new Vector3(0.1f, 0.3f, 0.1f);
            leg.transform.Rotate(45f, 0f, -45f);
        }

        Renderer renderer = leg.GetComponent<Renderer>();
        renderer.material.color = new Color(1f, 1f, 2f / rand);
        Texture2D texture = TextureMap(rand);
        renderer.material.mainTexture = texture;
    }

    // Generates the variation 1 of creature's tail made of planes and quads
    void MakeTail1(int rand, float size) {
        // Top (main + tip)
        GameObject tailmt = GameObject.CreatePrimitive(PrimitiveType.Plane);
        tailmt.name = "Tail Main Top";
        tailmt.transform.position = size * 
            new Vector3(0 + 2 * rand, 0.5f, 0.25f);                
        tailmt.transform.localScale = size * new Vector3(0.05f, 1, 0.01f);
        tailmt.transform.Rotate(0f, 90f, 45f);
        Renderer renderermt = tailmt.GetComponent<Renderer>();
        renderermt.material.color = new Color(2f / rand, 0f, 1f);
        Texture2D texturemt = TextureMap(rand);
        renderermt.material.mainTexture = texturemt;

        GameObject tailtt = GameObject.CreatePrimitive(PrimitiveType.Quad);
        tailtt.name = "Tail Tip Top";
        tailtt.transform.position = size * 
            new Vector3(0 + 2 * rand, 0.4f, 0.5f);                
        tailtt.transform.localScale = size * new Vector3(0.05f, 0.2f, 0.01f);
        tailtt.transform.Rotate(45f, 0f, 0f);
        Renderer renderertt = tailtt.GetComponent<Renderer>();
        renderertt.material.color = new Color(2f / rand, 1f, 0f);
        Texture2D texturett = TextureMap(rand);
        renderertt.material.mainTexture = texturett;

        // Bottom (main + tip)
        GameObject tailmb = GameObject.CreatePrimitive(PrimitiveType.Plane);
        tailmb.name = "Tail Main Bottom";
        tailmb.transform.position = size * 
            new Vector3(0 + 2 * rand, 0.5f, 0.25f);                
        tailmb.transform.localScale = size * new Vector3(0.05f, 1, 0.01f);
        tailmb.transform.Rotate(0f, 90f, 225f);
        Renderer renderermb = tailmb.GetComponent<Renderer>();
        renderermb.material.color = new Color(2f / rand, 0f, 1f);
        Texture2D texturemb = TextureMap(rand);
        renderermb.material.mainTexture = texturemb;

        GameObject tailtb = GameObject.CreatePrimitive(PrimitiveType.Quad);
        tailtb.name = "Tail Tip Bottom";
        tailtb.transform.position = size * 
            new Vector3(0 + 2 * rand, 0.4f, 0.5f);                
        tailtb.transform.localScale = size * new Vector3(0.05f, 0.2f, 0.01f);
        tailtb.transform.Rotate(225f, 0f, 0f);
        Renderer renderertb = tailtb.GetComponent<Renderer>();
        renderertb.material.color = new Color(2f / rand, 1f, 0f);
        Texture2D texturetb = TextureMap(rand);
        renderertb.material.mainTexture = texturetb;
    }

    // Generates the variation 2 of creature's tail made of a BEST attempted
    // subdivision mesh
    void MakeTail2(int rand, float size) {
        GameObject tl = new GameObject("Tail");
        tl.transform.position = size * 
            new Vector3(-0.2f + 2 * rand, 0.75f, 0.4f);                
        tl.transform.localScale = size * new Vector3(0.5f, 0.5f, 0.5f);
        tl.transform.Rotate(45f, 90f, 45f);
        Mesh tail = CreateTail2Mesh();
        ntris = 0; // reset to 0, else will cause Index Out of Bounds
        // Apply subdivision surfaces to mesh several times
        for (int i = 0; i < 3; i++) {
            tail = SubDivision(tail);
            ntris = 0; // reset to 0, else will cause Index Out of Bounds
        }

        tl.AddComponent<MeshFilter>();
        tl.AddComponent<MeshRenderer>();
        tl.GetComponent<MeshFilter>().mesh = tail;
        Renderer rend = tl.GetComponent<Renderer>();
        rend.material.color = new Color(Random.Range(0f, 1f), 0f, 1f);
        Texture2D text = TextureMap(rand);
        rend.material.mainTexture = text;
    }

    // Creates the basis tail mesh that's two squared based pyramids 
    // on top of each other
    Mesh CreateTail2Mesh() {
        Mesh mesh = new Mesh();

        // vertices of the mesh
        int num_verts = 24;
		verts = new Vector3[num_verts];

		// vertices 
		verts[0] = new Vector3(0, 0, 1);
		verts[1] = new Vector3(0.5f, 1, 0.5f);
		verts[2] = new Vector3(0, 0, 0);

		verts[3] = new Vector3(0, 0, 0);
		verts[4] = new Vector3(0.5f, 1, 0.5f);
		verts[5] = new Vector3(1, 0, 0);

		verts[6] = new Vector3(0.5f, 1, 0.5f);
		verts[7] = new Vector3(1, 0, 1);
		verts[8] = new Vector3(1, 0, 0);

		verts[9] = new Vector3(0.5f, 1, 0.5f);
		verts[10] = new Vector3(0, 0, 1);
		verts[11] = new Vector3(1, 0, 1);

        verts[12] = new Vector3(0.5f, -1, 0.5f);
		verts[13] = new Vector3(0, 0, 1);
		verts[14] = new Vector3(0, 0, 0);

		verts[15] = new Vector3(0.5f, -1, 0.5f);
		verts[16] = new Vector3(0, 0, 0);
		verts[17] = new Vector3(1, 0, 0);

		verts[18] = new Vector3(0.5f, -1, 0.5f);
		verts[19] = new Vector3(1, 0, 0);
		verts[20] = new Vector3(1, 0, 1);

		verts[21] = new Vector3(0.5f, -1, 0.5f);
		verts[22] = new Vector3(1, 0, 1);
		verts[23] = new Vector3(0, 0, 1);

		int num_tris = 8;
		tris = new int[num_tris * 3]; // need 3 vertices per triangle

		// make the rectangles from vertices
        MakeTri(0, 1, 2);
        MakeTri(3, 4, 5);
        MakeTri(6, 7, 8);
        MakeTri(9, 10, 11);
        MakeTri(12, 13, 14);
        MakeTri(15, 16, 17);
        MakeTri(18, 19, 20);
        MakeTri(21, 22, 23);

		// save the vertices and triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;

		// automatically calculate the vertex normals
		mesh.RecalculateNormals();

        return (mesh);
    }

    // Make a triangle from three vertex indices (clockwise order)
	void MakeTri(int i1, int i2, int i3) {
		// figure out the base index for storing triangle indices
		int index = ntris * 3;
		ntris++;

		tris[index] = i1;
		tris[index + 1] = i2;
		tris[index + 2] = i3;
	}

	// Make a quadrilateral from four vertex indices (clockwise order)
	void MakeQuad(int i1, int i2, int i3, int i4) {
		MakeTri(i1, i2, i3);
		MakeTri(i3, i2, i4);
	}

    // Gets the triangle number of a given index in Vertex Table
    int Triangle_Number(int c) {
        return (int)c / 3;
    }

    // Gets the next corner of a given index in Vertex Table
    int Next_Corner(int c) {
        return 3 * Triangle_Number(c) + (c + 1) % 3;
    }

    // Gets the previous corner of a given index in Vertex Table
    int Previous_Corner(int c) {
        return Next_Corner(Next_Corner(c));
    }

    // The Swing operator for going around to adjacent vertices
    int Swing(int c, int[] O) {
        return Next_Corner(O[Next_Corner(c)]);
    }

    // Geometry table G with implied index of (x, y, z) for vertices
    int[] Geometry_Table(Mesh mesh) {
        int num_verts = mesh.vertices.Length;

        int[] G = new int[num_verts];

        for (int i = 0; i < num_verts; i++) {
            G[i] = i;
        }

        return G;
    }

    // Vertex table V with implied indecies to G table
    int[] Vertex_Table(Mesh mesh) {
        int tris = mesh.triangles.Length;

        int[] V = new int[tris];

        for (int i = 0; i < tris; i++) {
            V[i] = i;
        }

        return V;
    }

    // Opposites table O from table V
    int[] Opposite_Table(Mesh mesh) {
        int[] G = Geometry_Table(mesh);
        int[] V = Vertex_Table(mesh);
        int[] O = new int[V.Length];

        for (int a = 0; a < V.Length; a++) {
            for (int b = 0; b < V.Length; b++) {
                if (mesh.vertices[G[Next_Corner(a)]] == 
                        mesh.vertices[G[Previous_Corner(a)]] &&
                        mesh.vertices[G[Previous_Corner(a)]] == 
                        mesh.vertices[G[Next_Corner(b)]]) {
                    O[a] = b;
                    O[b] = a;
                }
            }
        }

        return O;
    }

    // Calculates a new even vertex during subdivision
    Vector3 NewEvenVert(Vector3 p, int[] V, Mesh mesh) {
        int k = 0;
        float B;
        Vector3 sum = new Vector3(0, 0, 0);

        for (int i = 0; i < V.Length; i++) {
            if (p == mesh.vertices[V[i]]) {
                sum += mesh.vertices[Next_Corner(i)];
                k += 1;
            }
        }

        if (k == 3) {
            B = 3 / 16f;
        } else {
            B = 3 / 8f * k;
        }

        return (1 - k * B) * p + B * sum;
    }

    // Applies loop subdivision to given mesh
    // Couldn't fully fix bugs to achieve proper subdivision :(
    Mesh SubDivision(Mesh mesh) {
        // Get adjacencies
        int[] G = Geometry_Table(mesh);
        int[] V = Vertex_Table(mesh);
        int[] O = Opposite_Table(mesh);

        Mesh new_mesh = new Mesh();
        
        // vertices of the mesh
        int num_verts = mesh.vertices.Length * 4; // 4 times vertices
		verts = new Vector3[num_verts];
        int i = 0;
        for (int c = 0; c < V.Length; c += 3) {
            int cn = Next_Corner(c);
            int cnn = Next_Corner(cn); // split into 4 tris

            // Create new vertices
            Vector3 p1 = mesh.vertices[G[c]];
            Vector3 p2 = mesh.vertices[G[cn]];
            Vector3 p3 = mesh.vertices[G[cnn]];
            Vector3 p1o = mesh.vertices[G[O[c]]];
            Vector3 p2o = mesh.vertices[G[O[cn]]];
            Vector3 p3o = mesh.vertices[G[O[cnn]]];
            Vector3 e1 = 3 / 8f * (p1 + p2) + 1 / 8f * (p1o + p2o);
            Vector3 e2 = 3 / 8f * (p2 + p3) + 1 / 8f * (p2o + p3o);
            Vector3 e3 = 3 / 8f * (p3 + p1) + 1 / 8f * (p3o + p1o);

            // Put new vertices into array
            verts[i++] = p1; // NewEvenVert(p1, V, mesh); 
            verts[i++] = e1;
            verts[i++] = e3;
            verts[i++] = p2; // NewEvenVert(p2, V, mesh);
            verts[i++] = e2;
            verts[i++] = e1;
            verts[i++] = p3; // NewEvenVert(p3, V, mesh);
            verts[i++] = e3;
            verts[i++] = e2;
            verts[i++] = e1;
            verts[i++] = e2;
            verts[i++] = e3;
        }

		// make the rectangles from vertices
        int num_tris = mesh.triangles.Length * 4; // split into 4 tris
        tris = new int[num_tris];
        for (int j = 0; j < num_verts; j += 3) {
            MakeTri(j, j + 1, j + 2);
        }

		// save the vertices and triangles in the mesh object
		new_mesh.vertices = verts;
		new_mesh.triangles = tris;

		// automatically calculate the vertex normals
		new_mesh.RecalculateNormals();

        return (new_mesh);
    }

    // Create a texture map to place on body parts
	Texture2D TextureMap(int num) {
        // dimenstions of texture
        int width = 100 * num;
        int height = 100 * num;
        int scale = 20 * num;

		// create the texture and an array of colors
		Texture2D texture = new Texture2D(width, height);
		Color[] colors = new Color[width * height];

		// create the Perlin noise pattern in "colors"
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				float x = scale * Random.Range(0, i) / (float)width;
				float y = scale * Random.Range(0, j) / (float)height;
				float t = Mathf.PerlinNoise(x, y);
				colors[j * height + i] = new Color(t, t / 2, t / 3, 0.4f);
			}
        }

		// copy the colors into the texture
		texture.SetPixels(colors);

		// do texture-y stuff, probably including making the mipmap levels
		texture.Apply();

		// return the texture
		return (texture);
	}

    // Update is called once per frame
    void Update() {
        
    }
}
