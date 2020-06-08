using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBuildings : MonoBehaviour {
    // values that the user can change
    public int Seed = 10;
    public int numBuildings = 6;

    // textures to be placed on buildings
    public Material Window1;
    public Material Window2;
    public Material Door1;
    public Material Door2;
    public Material Wall1;
    public Material Wall2;
    public Material Wall3;

    // array for footprint of building
    private int[,] footprint;

    // the vertices of the mesh
	private Vector3[] verts;

	// the triangles of the mesh (triplets of integer references to vertices)
	private int[] tris;

	// the number of triangles that have been created so far
	private int ntris = 0;

    // Start is called before the first frame update
    void Start() {
        Random.InitState(Seed);

        for (int i = 1; i < numBuildings + 1; i++) {
            GenerateBuilding(i);
        }
    }

    // Function for making desired building type based on given integer
    public GameObject GenerateBuilding(int i) {
        int num = Random.Range(1, i + 10);
        float chance = Random.Range(0f, 1f);

        GameObject building = new GameObject("Building");
        building.transform.position = new Vector3(0f, 0f, 0f);

        // Random variation in buildings based on random seed
        if (chance >= 0.16f && chance < 0.32f) {
            BuildingOne(num, building);
        } else if (chance >= 0.32f && chance < 0.48f) {
            BuildingTwo(num, building);
        } else if (chance >= 0.48f && chance < 0.64f) {
            BuildingThree(num, building);
        } else if (chance >= 0.64f && chance < 0.8f) {
            BuildingFour(num, building);
        } else if (chance >= 0.8f && chance < 0.96f) {
            BuildingFive(num, building);
        } else {
            BuildingSix(num, building);
        }

        return (building);
    }

    // Make a cross-shaped house with a cross-gable roof
    public void BuildingOne(int num, GameObject building) {
        // Define footprint
        footprint = new int[3, 3];
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (i == 1 || j == 1) {
                    footprint[i, j] = 1;
                } else {
                    footprint[i, j] = 0;
                }
            }
        }

        // Make building with cubes
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (footprint[i, j] == 1) {
                    // Each section is divided into 6 blocks
                    GameObject block1
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block1.name = "Cross.1 " + i + "," + j;
                    GameObject block2 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block2.name = "Cross.2 " + i + "," + j;
                    GameObject block3 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block3.name = "Cross.3 " + i + "," + j;
                    GameObject block4 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block4.name = "Cross.4 " + i + "," + j;
                    GameObject block5
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block5.name = "Cross.5 " + i + "," + j;
                    GameObject block6 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block6.name = "Cross.6 " + i + "," + j;

                    // Put blocks into right position
                    block1.transform.position 
                        = new Vector3(2 * i, 0.375f, 2 * j - 20 * num);
                    block2.transform.position 
                        = new Vector3(2 * i + 2 / 3f, 0.375f, 2 * j - 20 * num);
                    block3.transform.position 
                        = new Vector3(2 * i - 2 / 3f, 0.375f, 2 * j - 20 * num);
                    block4.transform.position 
                        = new Vector3(2 * i, 1.125f, 2 * j - 20 * num);
                    block5.transform.position 
                        = new Vector3(2 * i + 2 / 3f, 1.125f, 2 * j - 20 * num);
                    block6.transform.position 
                        = new Vector3(2 * i - 2 / 3f, 1.125f, 2 * j - 20 * num);
                         
                    // Apply scaling
                    block1.transform.localScale     
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block2.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block3.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block4.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block5.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block6.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);        

                    // Attach facades
                    Renderer rend1 = block1.GetComponent<Renderer>();
                    Renderer rend2 = block2.GetComponent<Renderer>();
                    Renderer rend3 = block3.GetComponent<Renderer>();
                    Renderer rend4 = block4.GetComponent<Renderer>();
                    Renderer rend5 = block5.GetComponent<Renderer>();
                    Renderer rend6 = block6.GetComponent<Renderer>();

                    if (i == 1 && j == 0 || j == 2) {
                        rend1.material = Door2;
                        rend2.material = Wall1;
                        rend3.material = Wall1;
                        rend4.material = Wall1;
                        rend5.material = Window1;
                        rend6.material = Window1;
                    } else {
                        rend1.material = Wall1;
                        rend2.material = Wall1;
                        rend3.material = Wall1;
                        rend4.material = Window1;
                        rend5.material = Wall1;
                        rend6.material = Wall1;
                    }

                    block1.transform.parent = building.transform;
                    block2.transform.parent = building.transform;
                    block3.transform.parent = building.transform;
                    block4.transform.parent = building.transform;
                    block5.transform.parent = building.transform;
                    block6.transform.parent = building.transform;
                }
            }
        }

        // Attach a roof
        GameObject r = new GameObject("Cross Roof ");
        r.transform.position = new Vector3(-1, 1.5f, 1 - 20 * num);
        r.transform.localScale = new Vector3(2f, 1.5f, 2f);
        Mesh roof = CreateGableRoof();
        ntris = 0; // reset to 0, else will cause Index Out of Bounds
        r.AddComponent<MeshFilter>();
        r.AddComponent<MeshRenderer>();
        r.GetComponent<MeshFilter>().mesh = roof;
        Renderer renderer = r.GetComponent<Renderer>();
        renderer.material.color = new Color(0f, 0f, Random.Range(0f, 1f));
        Texture2D texture = RoofTextureMap(num);
        renderer.material.mainTexture = texture;
        r.transform.parent = building.transform;
    }

    // Make an 'L' shaped house with cross-hip roof
    public void BuildingTwo(int num, GameObject building) {
        // Define footprint
        footprint = new int[3, 3];
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (i == 0 || j == 0) {
                    footprint[i, j] = 1;
                } else {
                    footprint[i, j] = 0;
                }
            }
        }

        // Make building with cubes
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (footprint[i, j] == 1) {
                    // Each section is divided into 6 blocks
                    GameObject block1
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block1.name = "L.1 " + i + "," + j;
                    GameObject block2 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block2.name = "L.2 " + i + "," + j;
                    GameObject block3 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block3.name = "L.3 " + i + "," + j;
                    GameObject block4 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block4.name = "L.4 " + i + "," + j;
                    GameObject block5
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block5.name = "L.5 " + i + "," + j;
                    GameObject block6 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block6.name = "L.6 " + i + "," + j;

                    // Put blocks into right position
                    block1.transform.position = new Vector3(2 * i + 10 * 
                        num, 0.375f, 2 * j - 10 * num);
                    block2.transform.position = new Vector3(2 * i + 10 * 
                        num + 2 / 3f, 0.375f, 2 * j - 10 * num);
                    block3.transform.position = new Vector3(2 * i + 10 * 
                        num - 2 / 3f, 0.375f, 2 * j - 10 * num);
                    block4.transform.position = new Vector3(2 * i + 10 * 
                        num, 1.125f, 2 * j - 10 * num);
                    block5.transform.position = new Vector3(2 * i + 10 * 
                        num + 2 / 3f, 1.125f, 2 * j - 10 * num);
                    block6.transform.position = new Vector3(2 * i + 10 * 
                        num - 2 / 3f, 1.125f, 2 * j - 10 * num);
                         
                    // Apply scaling
                    block1.transform.localScale     
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block2.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block3.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block4.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block5.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);
                    block6.transform.localScale 
                        = new Vector3(2 / 3f, 0.75f, 2f);        

                    // Attach facades
                    Renderer rend1 = block1.GetComponent<Renderer>();
                    Renderer rend2 = block2.GetComponent<Renderer>();
                    Renderer rend3 = block3.GetComponent<Renderer>();
                    Renderer rend4 = block4.GetComponent<Renderer>();
                    Renderer rend5 = block5.GetComponent<Renderer>();
                    Renderer rend6 = block6.GetComponent<Renderer>();
                    
                    if (i == 0 && j == 0 || j == 2) {
                        rend1.material = Door2;
                        rend2.material = Wall1;
                        rend3.material = Wall1;
                        rend4.material = Wall1;
                        rend5.material = Window1;
                        rend6.material = Window1;
                    } else if (i == 2 && j == 0) {
                        rend1.material = Window1;
                        rend2.material = Wall1;
                        rend3.material = Wall1;
                        rend4.material = Wall1;
                        rend5.material = Window1;
                        rend6.material = Window1;
                    } else if (i == 1 && j == 0) {
                        rend1.material = Wall1;
                        rend2.material = Window1;
                        rend3.material = Window1;
                        rend4.material = Window1;
                        rend5.material = Wall1;
                        rend6.material = Wall1;
                    } else {
                        rend1.material = Wall1;
                        rend2.material = Wall1;
                        rend3.material = Wall1;
                        rend4.material = Wall1;
                        rend5.material = Wall1;
                        rend6.material = Wall1;
                    }

                    block1.transform.parent = building.transform;
                    block2.transform.parent = building.transform;
                    block3.transform.parent = building.transform;
                    block4.transform.parent = building.transform;
                    block5.transform.parent = building.transform;
                    block6.transform.parent = building.transform;
                }
            }
        }

        // Attach a roof
        GameObject r = new GameObject("L Roof");
        r.transform.position = new Vector3(10 * num - 1, 1.5f, -1 - 10 * num);
        r.transform.localScale = new Vector3(2f, 1.5f, 2f);
        Mesh roof = CreateHipRoof();
        ntris = 0; // reset to 0, else will cause Index Out of Bounds
        r.AddComponent<MeshFilter>();
        r.AddComponent<MeshRenderer>();
        r.GetComponent<MeshFilter>().mesh = roof;
        Renderer renderer = r.GetComponent<Renderer>();
        renderer.material.color = new Color(0f, Random.Range(0f, 1f), 0f);
        Texture2D texture = RoofTextureMap(num);
        renderer.material.mainTexture = texture;
        r.transform.parent = building.transform;
    }

    // Make Union College (a 'Z' shaped apartment block)
    public void BuildingThree(int num, GameObject building) {
        // Define footprint
        footprint = new int[5, 5];
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 5; j++) {
                if (i == 2) {
                    footprint[i, j] = 1;
                } else if (i >= 2 && j <= 1) {
                    footprint[i, j] = 1;
                } else if (i <= 2 && j >= 3) {
                    footprint[i, j] = 1;
                } else {
                    footprint[i, j] = 0;
                }
            }
        }

        // Make building with cubes
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 5; j++) {
                if (footprint[i, j] == 1) {
                    // Each section is divided into 8 blocks
                    GameObject block1
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block1.name = "UC.1 " + i + "," + j;
                    GameObject block2 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block2.name = "UC.2 " + i + "," + j;
                    GameObject block3 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block3.name = "UC.3 " + i + "," + j;
                    GameObject block4 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block4.name = "UC.4 " + i + "," + j;
                    GameObject block5
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block5.name = "UC.5 " + i + "," + j;
                    GameObject block6 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block6.name = "UC.6 " + i + "," + j;
                    GameObject block7 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block7.name = "UC.7 " + i + "," + j;
                    GameObject block8 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block8.name = "UC.8 " + i + "," + j;

                    // Different elevation profile
                    float yscale;
                    if (i == 2) {
                        yscale = 6f;
                    } else {
                        yscale = 3f;
                    }

                    // Put blocks into right position
                    block1.transform.position = new Vector3(5 * i, 
                        yscale / 4f, 5 * j + 30 * num);
                    block2.transform.position = new Vector3(5 * i + 2.5f, 
                        yscale / 4f, 5 * j + 30 * num);
                    block3.transform.position = new Vector3(5 * i, 
                        0.75f * yscale , 5 * j + 30 * num);
                    block4.transform.position = new Vector3(5 * i + 2.5f, 
                        0.75f * yscale, 5 * j + 30 * num);
                    block5.transform.position = new Vector3(5 * i, 
                        yscale / 4f, 5 * j + 30 * num + 2.5f);
                    block6.transform.position = new Vector3(5 * i + 2.5f, 
                        yscale / 4f, 5 * j + 30 * num + 2.5f);
                    block7.transform.position = new Vector3(5 * i, 
                        0.75f * yscale , 5 * j + 30 * num + 2.5f);
                    block8.transform.position = new Vector3(5 * i + 2.5f, 
                        0.75f * yscale, 5 * j + 30 * num + 2.5f);
                         
                    // Apply scaling
                    block1.transform.localScale 
                        = new Vector3(2.5f, yscale / 2f, 2.5f);
                    block2.transform.localScale 
                        = new Vector3(2.5f, yscale / 2f, 2.5f);
                    block3.transform.localScale 
                        = new Vector3(2.5f, yscale/ 2f , 2.5f);
                    block4.transform.localScale 
                        = new Vector3(2.5f, yscale/ 2f, 2.5f);
                    block5.transform.localScale 
                        = new Vector3(2.5f, yscale / 2f, 2.5f);
                    block6.transform.localScale 
                        = new Vector3(2.5f, yscale / 2f, 2.5f);
                    block7.transform.localScale 
                        = new Vector3(2.5f, yscale/ 2f , 2.5f);
                    block8.transform.localScale 
                        = new Vector3(2.5f, yscale/ 2f, 2.5f);


                    // Attach facades
                    Renderer rend1 = block1.GetComponent<Renderer>();
                    Renderer rend2 = block2.GetComponent<Renderer>();
                    Renderer rend3 = block3.GetComponent<Renderer>();
                    Renderer rend4 = block4.GetComponent<Renderer>();
                    Renderer rend5 = block5.GetComponent<Renderer>();
                    Renderer rend6 = block6.GetComponent<Renderer>();
                    Renderer rend7 = block7.GetComponent<Renderer>();
                    Renderer rend8 = block8.GetComponent<Renderer>();

                    if (i == 0 && j == 3) {
                        rend1.material = Window2;
                        rend2.material = Wall2;
                        rend3.material = Window2;
                        rend4.material = Wall2;
                        rend5.material = Wall2;
                        rend6.material = Wall2;
                        rend7.material = Wall2;
                        rend8.material = Wall2;
                    } else if (i == 0 && j == 4 || i == 3 && j == 1) {
                        rend1.material = Wall2;
                        rend2.material = Wall2;
                        rend3.material = Wall2;
                        rend4.material = Wall2;
                        rend5.material = Window2;
                        rend6.material = Wall2;
                        rend7.material = Window2;
                        rend8.material = Wall2;
                    } else if(i == 1 && j == 3 || i == 4 && j == 0) {
                        rend1.material = Wall2;
                        rend2.material = Window2;
                        rend3.material = Wall2;
                        rend4.material = Window2;
                        rend5.material = Wall2;
                        rend6.material = Wall2;
                        rend7.material = Wall2;
                        rend8.material = Wall2;
                    } else if (i == 1 && j == 4 || i == 4 && j == 1) {
                        rend1.material = Wall2;
                        rend2.material = Wall2;
                        rend3.material = Wall2;
                        rend4.material = Wall2;
                        rend5.material = Wall2;
                        rend6.material = Window2;
                        rend7.material = Wall2;
                        rend8.material = Window2;
                    } else if (i == 3 && j == 0) {
                        rend1.material = Window2;
                        rend2.material = Wall2;
                        rend3.material = Window2;
                        rend4.material = Wall2;
                        rend5.material = Wall2;
                        rend6.material =  Wall2;
                        rend7.material = Wall2;
                        rend8.material =  Wall2;
                    } else if (i == 2 && j == 4 || j == 0) {
                        rend1.material = Window2;
                        rend2.material = Wall2;
                        rend3.material = Wall2;
                        rend4.material = Window2;
                        rend5.material = Wall2;
                        rend6.material = Window2;
                        rend7.material = Window2;
                        rend8.material = Wall2;
                    } else if (i == 2 && j == 2) {
                        rend1.material = Wall2;
                        rend2.material = Wall2;
                        rend3.material = Wall2;
                        rend4.material = Window2;
                        rend5.material = Wall2;
                        rend6.material = Wall2;
                        rend7.material = Window2;
                        rend8.material = Wall2;
                    } else if (i == 2 && j == 1) {
                        rend1.material = Wall2;
                        rend2.material = Wall2;
                        rend3.material = Wall2;
                        rend4.material = Window2;
                        rend5.material = Door1;
                        rend6.material = Wall2;
                        rend7.material = Window2;
                        rend8.material = Wall2;
                    } else if (i == 2 && j ==3 ) {
                        rend1.material = Wall2;
                        rend2.material = Door1;
                        rend3.material = Wall2;
                        rend4.material = Window2;
                        rend5.material = Wall2;
                        rend6.material = Wall2;
                        rend7.material =  Window2;
                        rend8.material = Wall2;
                    }
                    else {
                        rend1.material = Wall2;
                        rend2.material = Wall2;
                        rend3.material = Wall2;
                        rend4.material = Wall2;
                        rend5.material = Wall2;
                        rend6.material = Wall2;
                        rend7.material = Wall2;
                        rend8.material = Wall2;
                    }

                    block1.transform.parent = building.transform;
                    block2.transform.parent = building.transform;
                    block3.transform.parent = building.transform;
                    block4.transform.parent = building.transform;
                    block5.transform.parent = building.transform;
                    block6.transform.parent = building.transform;
                    block7.transform.parent = building.transform;
                    block8.transform.parent = building.transform;
                }
            }
        }

        // Attach 3 roofs
        GameObject r1 = new GameObject("UC.1 Roof");
        r1.transform.position 
            = new Vector3(-1.25f, 3f, 5f + 30 * num + 8.75f);
        r1.transform.localScale = new Vector3(2f, 0.75f, 2f);
        Mesh roof1 = CreateTriangularRoof();
        ntris = 0; // reset to 0, else will cause Index Out of Bounds
        r1.AddComponent<MeshFilter>();
        r1.AddComponent<MeshRenderer>();
        r1.GetComponent<MeshFilter>().mesh = roof1;
        Renderer renderer1 = r1.GetComponent<Renderer>();
        renderer1.material.color = new Color(Random.Range(0f, 1f), 0f, 0f);
        Texture2D texture1 = RoofTextureMap(num);
        renderer1.material.mainTexture = texture1;
        r1.transform.parent = building.transform;

        GameObject r2 = new GameObject("UC.2 Roof");
        r2.transform.position 
            = new Vector3(13.75f, 3f, 5f + 30 * num - 6.25f);
        r2.transform.localScale = new Vector3(2f, 0.75f, 2f);
        Mesh roof2 = CreateTriangularRoof();
        ntris = 0; // reset to 0, else will cause Index Out of Bounds
        r2.AddComponent<MeshFilter>();
        r2.AddComponent<MeshRenderer>();
        r2.GetComponent<MeshFilter>().mesh = roof2;
        Renderer renderer2 = r2.GetComponent<Renderer>();
        renderer2.material.color = new Color(0f, Random.Range(0f, 1f), 0f);
        Texture2D texture2 = RoofTextureMap(num);
        renderer2.material.mainTexture = texture2;
        r2.transform.parent = building.transform;

        GameObject r3 = new GameObject("UC.3 Roof");
        r3.transform.position 
            = new Vector3(8.75f, 6f, 5f + 30 * num - 6.25f);
        r3.transform.localScale = new Vector3(1f, 1f, 5f);
        Mesh roof = CreateTriangularRoof();
        ntris = 0; // reset to 0, else will cause Index Out of Bounds
        r3.AddComponent<MeshFilter>();
        r3.AddComponent<MeshRenderer>();
        r3.GetComponent<MeshFilter>().mesh = roof;
        Renderer renderer3 = r3.GetComponent<Renderer>();
        renderer3.material.color = new Color(0f, 0f, Random.Range(0f, 1f));
        Texture2D texture3 = RoofTextureMap(num);
        renderer3.material.mainTexture = texture3;
        r3.transform.parent = building.transform;
    }

    // Make a regular bunker-style house with a triangular roof
    public void BuildingFour(int num, GameObject building) {
        // Define footprint
        footprint = new int[7, 7];
        for (int i = 0; i < 7; i++) {
            for (int j = 0; j < 7; j++) {
                if (i == 0 || j == 0 || i == 6 || j == 6) {
                    footprint[i, j] = 0;
                } else {
                    footprint[i, j] = 1;
                }
            }
        }

        // Make building with cubes
        for (int i = 0; i < 7; i++) {
            for (int j = 0; j < 7; j++) {
                if (footprint[i, j] == 1) {
                    // Each section is divided into 4 blocks
                    GameObject block1
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block1.name = "Bunker.1 " + i + "," + j;
                    GameObject block2 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block2.name = "Bunker.2 " + i + "," + j;
                    GameObject block3 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block3.name = "Bunker.3 " + i + "," + j;
                    GameObject block4 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block4.name = "Bunker.4 " + i + "," + j;

                    // Put blocks into right position
                    block1.transform.position 
                        = new Vector3(i + 10 * num, 0.375f, j + 20 * num);
                    block2.transform.position = new Vector3(i + 10 * num + 0.5f,
                        0.375f, j + 20 * num);
                    block3.transform.position 
                        = new Vector3(i + 10 * num, 1.125f, j + 20 * num);
                    block4.transform.position = new Vector3(i + 10 * num + 0.5f,
                        1.125f, j + 20 * num);
                         
                    // Apply scaling
                    block1.transform.localScale = new Vector3(0.5f, 0.75f, 1);
                    block2.transform.localScale = new Vector3(0.5f, 0.75f, 1);
                    block3.transform.localScale = new Vector3(0.5f, 0.75f, 1);
                    block4.transform.localScale = new Vector3(0.5f, 0.75f, 1);

                    // Attach facades
                    Renderer rend1 = block1.GetComponent<Renderer>();
                    Renderer rend2 = block2.GetComponent<Renderer>();
                    Renderer rend3 = block3.GetComponent<Renderer>();
                    Renderer rend4 = block4.GetComponent<Renderer>();

                    if (i == 3 && (j == 1 || j == 5)) {
                        rend1.material = Door2;
                        rend2.material = Door2;
                        rend3.material = Wall1;
                        rend4.material = Wall1;
                    } else if ((i == 1 || i == 2 || i == 4 || i == 5) 
                            && (j == 1 || j == 5 )) {
                        rend1.material = Wall1;
                        rend2.material = Wall1;
                        rend3.material = Window1;
                        rend4.material = Window1;
                    } else if ((i == 1 || i == 5) && j == 3) {
                        rend1.material = Wall1;
                        rend2.material = Wall1;
                        rend3.material = Window1;
                        rend4.material = Window1;
                    } else {
                        rend1.material = Wall1;
                        rend2.material = Wall1;
                        rend3.material = Wall1;
                        rend4.material = Wall1;
                    }   

                    block1.transform.parent = building.transform;
                    block2.transform.parent = building.transform;
                    block3.transform.parent = building.transform;
                    block4.transform.parent = building.transform;         
                }
            }
        }

        // Attach a roof
        GameObject r = new GameObject("Bunker Roof");
        r.transform.position 
            = new Vector3(10 * num + 0.75f, 1.5f, 0.5f + 20 * num);
        r.transform.localScale = new Vector3(1f, 0.5f, 1f);
        Mesh roof = CreateTriangularRoof();
        ntris = 0; // reset to 0, else will cause Index Out of Bounds
        r.AddComponent<MeshFilter>();
        r.AddComponent<MeshRenderer>();
        r.GetComponent<MeshFilter>().mesh = roof;
        Renderer renderer = r.GetComponent<Renderer>();
        renderer.material.color = new Color(Random.Range(0f, 1f), 0f, 0f);
        Texture2D texture = RoofTextureMap(num);
        renderer.material.mainTexture = texture;
        r.transform.parent = building.transform;
    }   

    // Make a skyscraper like similar to Burj Khalifa
    public void BuildingFive(int num, GameObject building) {
        // Define footprint
        footprint = new int[5, 3];
        footprint[2, 0] = 1;
        for (int i = 1; i < 4; i++) {
            footprint[i, 1] = 1;
        }
        for (int j = 0; j < 5; j++) {
            footprint[j, 2] = 1;
        }

        // Make building with cylinders
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 3; j++) {
                if (footprint[i, j] == 1) {
                    GameObject block 
                        = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    block.name = "Burj " + i + "," + j;
                    float yscale;
                    if (i == 2 && j == 0) {
                        yscale = 1.5f;
                    } else if (i == 2 && j == 1) {
                        yscale = 2.5f;
                    } else if ((i == 1 || i == 3) && j == 1) {
                        yscale = 2f;
                    } else if ((i == 0 || i == 4) && j == 2) {
                        yscale = 3f;
                    } else if ((i == 1 || i == 3) && j == 2) {
                        yscale = 5f;
                    } else {
                        yscale = 7f;
                    }
                    block.transform.localScale = new Vector3(1, yscale, 1);
                    block.transform.position 
                        = new Vector3(i + 30 * num, yscale, j);

                    // Attach facades
                    Renderer rend = block.GetComponent<Renderer>();
                    if ((i == 2 && j == 0) || j == 1) {
                        rend.material = Wall3;
                    } else if ((i == 0 || i == 4) && j == 2) {
                        rend.material = Wall1;
                    } else if ((i == 1 || i == 3) && j == 2) {
                        rend.material = Wall2;
                    } else {
                        rend.material.color = new Color(Random.Range(0f, 1f), 
                                Random.Range(0f, 1f), Random.Range(0f, 1f));
                        Texture2D texture = RoofTextureMap(num);
                        rend.material.mainTexture = texture;
                    } 

                    block.transform.parent = building.transform;
                }
            }
        }
    }

    // Make a hotel like buidling similar to Marina Bay Sands
    public void BuildingSix(int num, GameObject building) {
        // Define footprint
        footprint = new int[10, 3];
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 3; j++) {
                if (i == 0 || i == 1 || i == 4 || i == 5 || i == 8 || i == 9) {
                    footprint[i, j] = 1;
                } else {
                    footprint[i, j] = 0;
                }
            } 
        }

        // Make building with cubes
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 3; j++) {
                if (footprint[i, j] == 1) {
                    // Each section is divided into 5 blocks
                    GameObject block1
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block1.name = "Marina BS.1 " + i + "," + j;
                    GameObject block2 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block2.name = "Marina BS.2 " + i + "," + j;
                    GameObject block3 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block3.name = "Marina BS.3 " + i + "," + j;
                    GameObject block4 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block4.name = "Marina BS.4 " + i + "," + j;
                    GameObject block5 
                        = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block5.name = "Marina BS.5 " + i + "," + j;

                    // Put blocks into right position
                    block1.transform.position 
                        = new Vector3(i + 20 * num, 0.5f, j + 10);
                    block2.transform.position 
                        = new Vector3(i + 20 * num, 1.5f, j + 10);
                    block3.transform.position 
                        = new Vector3(i + 20 * num, 2.5f, j + 10);
                    block4.transform.position
                        = new Vector3(i + 20 * num, 3.5f, j + 10);
                    block5.transform.position
                        = new Vector3(i + 20 * num, 4.5f, j + 10);
                         
                    // Apply scaling
                    block1.transform.localScale = new Vector3(1, 1, 1);
                    block2.transform.localScale = new Vector3(1, 1, 1);
                    block3.transform.localScale = new Vector3(1, 1, 1);
                    block4.transform.localScale = new Vector3(1, 1, 1);

                    // Attach facades
                    Renderer rend1 = block1.GetComponent<Renderer>();
                    Renderer rend2 = block2.GetComponent<Renderer>();
                    Renderer rend3 = block3.GetComponent<Renderer>();
                    Renderer rend4 = block4.GetComponent<Renderer>();
                    Renderer rend5 = block5.GetComponent<Renderer>();

                    if (j == 1) {
                        rend1.material = Door1;
                    } else {
                        rend1.material = Wall3;
                    }
                    rend2.material = Wall3;
                    rend3.material = Wall3;
                    rend4.material = Wall3;
                    rend5.material = Wall3;

                    block1.transform.parent = building.transform;
                    block2.transform.parent = building.transform;
                    block3.transform.parent = building.transform;
                    block4.transform.parent = building.transform;
                    block5.transform.parent = building.transform;
                }
            }
        }

        // Attach a roof
        GameObject boat = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        boat.name = "Marina BS Roof";
        boat.transform.position = new Vector3(5 + 20 * num, 5.5f, 11);
        boat.transform.localScale = new Vector3(2, 6, 2);
        boat.transform.Rotate(90f, 0f, 90f);
        Renderer renderer = boat.GetComponent<Renderer>();
        renderer.material.color = new Color(Random.Range(0f, 1f), 
            Random.Range(0f, 1f), Random.Range(0f, 1f));
        Texture2D texture = RoofTextureMap(num);
        renderer.material.mainTexture = texture;
        boat.transform.parent = building.transform;
    }

    // Creates a cross-hip mesh
    public Mesh CreateTriangularRoof() {
        Mesh mesh = new Mesh();
        
        // vertices of the mesh
        int num_verts = 16;
		verts = new Vector3[num_verts];

		// vertices 
		verts[0] = new Vector3(0, 0, 5);
		verts[1] = new Vector3(2.5f, 2, 2.5f);
		verts[2] = new Vector3(0, 0, 0);

		verts[3] = new Vector3(0, 0, 0);
		verts[4] = new Vector3(2.5f, 2, 2.5f);
		verts[5] = new Vector3(5, 0, 0);

		verts[6] = new Vector3(2.5f, 2, 2.5f);
		verts[7] = new Vector3(5, 0, 5);
		verts[8] = new Vector3(5, 0, 0);

		verts[9] = new Vector3(2.5f, 2, 2.5f);
		verts[10] = new Vector3(0, 0, 5);
		verts[11] = new Vector3(5, 0, 5);

		verts[12] = new Vector3(0, 0, 5);
		verts[13] = new Vector3(0, 0, 0);
		verts[14] = new Vector3(5, 0, 5);
		verts[15] = new Vector3(5, 0, 0);

        // create uv coordinates
        Vector2[] uv = new Vector2[num_verts];

        uv[0] = new Vector2(0, 5);
		uv[1] = new Vector2(2.5f, 2.5f);
		uv[2] = new Vector2(0, 0);

		uv[3] = new Vector2(0, 0);
		uv[4] = new Vector2(2.5f, 2.5f);
		uv[5] = new Vector2(5, 0);

		uv[6] = new Vector2(2.5f, 2.5f);
		uv[7] = new Vector2(5, 5);
		uv[8] = new Vector2(5, 0);

		uv[9] = new Vector2(2.5f, 2.5f);
		uv[10] = new Vector2(0, 5);
		uv[11] = new Vector2(5, 5);

		uv[12] = new Vector2(0, 5);
		uv[13] = new Vector2(0, 0);
		uv[14] = new Vector2(5, 5);
		uv[15] = new Vector2(5, 0);

		int num_tris = 6;
		tris = new int[num_tris * 3]; // need 3 vertices per triangle

		// make the rectangles from vertices
        MakeTri(0, 1, 2);
        MakeTri(3, 4, 5);
        MakeTri(6, 7, 8);
        MakeTri(9,10, 11);
        MakeQuad(12, 13, 14, 15);

		// save the vertices and triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;
        mesh.uv = uv;

		// automatically calculate the vertex normals
		mesh.RecalculateNormals();

        return (mesh);
    }

    // Creates a cross-hip mesh
    public Mesh CreateHipRoof() {
        Mesh mesh = new Mesh();

        // vertices of the mesh
		int num_verts = 30;
		verts = new Vector3[num_verts];

		// vertices
		verts[0] = new Vector3(3, 0, 0);
		verts[1] = new Vector3(3, 0.8f, 0.5f);
		verts[2] = new Vector3(3, 0, 1);

		verts[3] = new Vector3(0.5f, 0.8f, 0.5f);
		verts[4] = new Vector3(1, 0, 1);
		verts[5] = new Vector3(3, 0.8f, 0.5f);
		verts[6] = new Vector3(3, 0, 1);

		verts[7] = new Vector3(0, 0, 0);
		verts[8] = new Vector3(0.5f, 0.8f, 0.5f);
		verts[9] = new Vector3(3, 0, 0);
		verts[10] = new Vector3(3, 0.8f, 0.5f);

		verts[11] = new Vector3(0, 0, 1);
		verts[12] = new Vector3(0, 0, 0);
		verts[13] = new Vector3(3, 0, 1);
		verts[14] = new Vector3(3, 0, 0);

        verts[15] = new Vector3(0.5f, 0.8f, 3);
		verts[16] = new Vector3(0, 0, 3);
		verts[17] = new Vector3(1, 0, 3);

		verts[18] = new Vector3(0, 0, 0);
		verts[19] = new Vector3(0, 0, 3);
		verts[20] = new Vector3(0.5f, 0.8f, 0.5f);
		verts[21] = new Vector3(0.5f, 0.8f, 3);

		verts[22] = new Vector3(0.5f, 0.8f, 0.5f);
		verts[23] = new Vector3(0.5f, 0.8f, 3);
		verts[24] = new Vector3(1, 0, 1);
		verts[25] = new Vector3(1, 0, 3);

		verts[26] = new Vector3(0, 0, 3);
		verts[27] = new Vector3(0, 0, 0);
		verts[28] = new Vector3(1, 0, 3);
		verts[29] = new Vector3(1, 0, 0);

        // create uv coordinates
        Vector2[] uv = new Vector2[num_verts];

        uv[0] = new Vector3(3, 0);
		uv[1] = new Vector3(3, 0.5f);
		uv[2] = new Vector3(3, 1);

		uv[3] = new Vector3(0.5f, 0.5f);
		uv[4] = new Vector3(1, 1);
		uv[5] = new Vector3(3, 0.5f);
		uv[6] = new Vector3(3, 1);

		uv[7] = new Vector3(0, 0);
		uv[8] = new Vector3(0.5f, 0.5f);
		uv[9] = new Vector3(3, 0);
		uv[10] = new Vector3(3, 0.5f);

		uv[11] = new Vector3(0, 1);
		uv[12] = new Vector3(0, 0);
		uv[13] = new Vector3(3, 1);
		uv[14] = new Vector3(3, 0);

        uv[15] = new Vector3(0.5f, 3);
		uv[16] = new Vector3(0, 3);
		uv[17] = new Vector3(1, 3);

		uv[18] = new Vector3(0, 0);
		uv[19] = new Vector3(0, 3);
		uv[20] = new Vector3(0.5f, 0.5f);
		uv[21] = new Vector3(0.5f, 3);

		uv[22] = new Vector3(0.5f, 0.5f);
		uv[23] = new Vector3(0.5f, 3);
		uv[24] = new Vector3(1, 1);
		uv[25] = new Vector3(1, 3);

		uv[26] = new Vector3(0, 3);
		uv[27] = new Vector3(0, 0);
		uv[28] = new Vector3(1, 3);
		uv[29] = new Vector3(1, 0);

		int num_tris = 14;
		tris = new int[num_tris * 3]; // need 3 vertices per triangle

		// make the rectangles from vertices
        MakeTri(0, 1, 2);
        MakeQuad(3, 4, 5, 6);
        MakeQuad(7, 8, 9, 10);
        MakeQuad(11, 12, 13, 14);
        MakeTri(15, 16, 17);
        MakeQuad(18, 19, 20, 21);
        MakeQuad(22, 23, 24, 25);
        MakeQuad(26, 27, 28, 29);

		// save the vertices and triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;
        mesh.uv = uv;

		// automatically calculate the vertex normals
		mesh.RecalculateNormals();

        return (mesh);
    }

    // Creates a cross-gable mesh
    public Mesh CreateGableRoof() {
        Mesh mesh = new Mesh();

        // vertices of the mesh
		int num_verts = 36;
		verts = new Vector3[num_verts];

		// vertices 
		verts[0] = new Vector3(0, 0, 0);
		verts[1] = new Vector3(0, 0, 1);
		verts[2] = new Vector3(0, 0.8f, 0.5f);

		verts[3] = new Vector3(3, 0, 0);
		verts[4] = new Vector3(3, 0.8f, 0.5f);
		verts[5] = new Vector3(3, 0, 1);

		verts[6] = new Vector3(0, 0.8f, 0.5f);
		verts[7] = new Vector3(0, 0, 1);
		verts[8] = new Vector3(3, 0.8f, 0.5f);
		verts[9] = new Vector3(3, 0, 1);

		verts[10] = new Vector3(0, 0, 0);
		verts[11] = new Vector3(0, 0.8f, 0.5f);
		verts[12] = new Vector3(3, 0, 0);
		verts[13] = new Vector3(3, 0.8f, 0.5f);

		verts[14] = new Vector3(0, 0, 1);
		verts[15] = new Vector3(0, 0, 0);
		verts[16] = new Vector3(3, 0, 1);
		verts[17] = new Vector3(3, 0, 0);

        verts[18] = new Vector3(1, 0, -1);
		verts[19] = new Vector3(1.5f, 0.8f, -1);
		verts[20] = new Vector3(2, 0, -1);

		verts[21] = new Vector3(1, 0, 2);
		verts[22] = new Vector3(2, 0, 2);
		verts[23] = new Vector3(1.5f, 0.8f, 2);

		verts[24] = new Vector3(1.5f, 0.8f, -1);
		verts[25] = new Vector3(1.5f, 0.8f, 2);
		verts[26] = new Vector3(2, 0, -1);
		verts[27] = new Vector3(2, 0, 2);

		verts[28] = new Vector3(1, 0, -1);
		verts[29] = new Vector3(1, 0, 2);
		verts[30] = new Vector3(1.5f, 0.8f, -1);
		verts[31] = new Vector3(1.5f, 0.8f, 2);

		verts[32] = new Vector3(1, 0, 2);
		verts[33] = new Vector3(1, 0, -1);
		verts[34] = new Vector3(2, 0, 2);
		verts[35] = new Vector3(2, 0, -1);

        // create uv coordinates
        Vector2[] uv = new Vector2[num_verts];

        uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(0, 1);
		uv[2] = new Vector2(0, 0.5f);

		uv[3] = new Vector2(3, 0);
		uv[4] = new Vector2(3, 0.5f);
		uv[5] = new Vector2(3, 1);

		uv[6] = new Vector2(0, 0.5f);
		uv[7] = new Vector2(0, 1);
		uv[8] = new Vector2(3, 0.5f);
		uv[9] = new Vector2(3, 1);

		uv[10] = new Vector2(0, 0);
		uv[11] = new Vector2(0, 0.5f);
		uv[12] = new Vector2(3, 0);
		uv[13] = new Vector2(3, 0.5f);

		uv[14] = new Vector2(0, 1);
		uv[15] = new Vector2(0, 0);
		uv[16] = new Vector2(3, 1);
		uv[17] = new Vector2(3, 0);

        uv[18] = new Vector2(1, -1);
		uv[19] = new Vector2(1.5f, -1);
		uv[20] = new Vector2(2, -1);

		uv[21] = new Vector2(1, 2);
		uv[22] = new Vector2(2, 2);
		uv[23] = new Vector2(1.5f, 2);

		uv[24] = new Vector2(1.5f, -1);
		uv[25] = new Vector2(1.5f, 2);
		uv[26] = new Vector2(2, -1);
		uv[27] = new Vector2(2, 2);

		uv[28] = new Vector2(1, -1);
		uv[29] = new Vector2(1, 2);
		uv[30] = new Vector2(1.5f, -1);
		uv[31] = new Vector2(1.5f, 2);

		uv[32] = new Vector2(1, 2);
		uv[33] = new Vector2(1, -1);
		uv[34] = new Vector2(2, 2);
		uv[35] = new Vector2(2, -1);

		int num_tris = 16;
		tris = new int[num_tris * 3]; // need 3 vertices per triangle

		// make the rectangles from vertices
        MakeTri(0, 1, 2);
        MakeTri(3, 4, 5);
        MakeQuad(6, 7, 8, 9);
        MakeQuad(10, 11, 12, 13);
        MakeQuad(14, 15, 16, 17);
        MakeTri(18, 19, 20);
        MakeTri(21, 22, 23);
        MakeQuad(24, 25, 26, 27);
        MakeQuad(28, 29, 30, 31);
        MakeQuad(32, 33, 34, 35);

		// save the vertices and triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;
        mesh.uv = uv;

		// automatically calculate the vertex normals
		mesh.RecalculateNormals();

        return (mesh);
    }

    // Make a triangle from three vertex indices (clockwise order)
	public void MakeTri(int i1, int i2, int i3) {
		// figure out the base index for storing triangle indices
		int index = ntris * 3;
		ntris++;

		tris[index] = i1;
		tris[index + 1] = i2;
		tris[index + 2] = i3;
	}

	// Make a quadrilateral from four vertex indices (clockwise order)
	public void MakeQuad(int i1, int i2, int i3, int i4) {
		MakeTri(i1, i2, i3);
		MakeTri(i3, i2, i4);
	}

    // Create a texture map to place on building roof
	public Texture2D RoofTextureMap(int num) {
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
