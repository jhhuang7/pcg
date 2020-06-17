using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cg = CreateGrid;

public class Flocking : MonoBehaviour {
    // Booleans that allow user to toggle the four Forces individually
    public bool FlockCentering = true;
    public bool VelocityMatching = true;
    public bool CollisionAvoidance = true;
    public bool Wandering = true;
    public bool Trail = true;

    // User-editable number of Creatures
    public int NumCreatures = MinCreatues;
    private static int MinCreatues = 30;
    private static int MaxCreatues = cg.Columns + cg.Rows;

    // Flocking Objects and Materials
    public GameObject Bird;
    public GameObject Bird1;
    public Material Gold;
    public Material Transparent;
    public Material Ground;
    
    // Creature information
    private GameObject[] Creatures;
    private Vector3[] Velocities;
    private Vector3[] Forces;

    // Clamp the creature Velocities to some reasonable range
    private static float MinV = -1f;
    private static float MaxV = 1f;

    // Keep all the Creatures confined to World Box (defined in CreateGrid)
    private static float MinX = 5f;
    private static float MaxX = (float)cg.Columns - 5f;
    private static float MinY = 1.5f;
    private static float MaxY = 4.5f;
    private static float MinZ = 5f;
    private static float MaxZ = (float)cg.Rows - 5f;

    // Random Seed
    private int Seed = 4803;

    // Start is called before the first frame update
    void Start() {
        WorldBox();
        
        Init();
    }

    // Make a World Box, so city and flocking is "under the dome"
    void WorldBox() {
        GameObject dome = new GameObject("Dome");

        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = "World Box";
        box.transform.position 
            = new Vector3(cg.Columns / 2 - 0.5f, 2.5f, cg.Rows / 2 - 0.5f);
        box.transform.localScale = new Vector3(cg.Columns, 5f, cg.Rows);
        Renderer rnd = box.GetComponent<Renderer>();
        rnd.material = Transparent;
        box.transform.parent = dome.transform;

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position 
            = new Vector3(cg.Columns / 2 - 0.5f, 0, cg.Rows / 2 - 0.5f);
        ground.transform.Rotate(180f, 0f, 0f);
        ground.transform.localScale 
            = new Vector3(cg.Columns * 0.1f, 1f, cg.Rows * 0.1f);
        Renderer rend = ground.GetComponent<Renderer>();
        rend.material = Ground;
        ground.transform.parent = dome.transform;
    }

    // Initializes all variables needed for flocking simulation
    void Init() {
        Random.InitState(Seed);

        Creatures = new GameObject[MaxCreatues];
        float scale = 1f;

        for (int i = 0; i < MaxCreatues; i++) {
            float chance = Random.Range(0f, 1f);
            if (chance >= 0.5f) {
                Creatures[i] = (GameObject)Instantiate(Bird);
                scale = 0.2f;
            } else {
                Creatures[i] = (GameObject)Instantiate(Bird1);
                scale = 0.5f;
            }

            Creatures[i].transform.position = new Vector3(Random.Range(MinX, 
                MaxX), Random.Range(MinY, MaxY), Random.Range(MinZ, MaxZ));
            Creatures[i].transform.localScale 
                = new Vector3(scale, scale, scale);

            var tr = Creatures[i].AddComponent<TrailRenderer>();
            tr.startWidth = 0.1f;
            tr.endWidth = 0.05f;
            tr.time = 5f;
            tr.material = Gold;
            tr.enabled = false;

            if (i >= NumCreatures) {
                Creatures[i].SetActive(false);
            }
        }

        Velocities = new Vector3[MaxCreatues];
        for (int i = 0; i < MaxCreatues; i++) {
            Velocities[i] = new Vector3(Random.Range(MinV, MaxV), 
                Random.Range(MinV, MaxV), Random.Range(MinV, MaxV));
        }

        Forces = new Vector3[MaxCreatues];
        for (int i = 0; i < MaxCreatues; i++) {
            Forces[i] = new Vector3(0f, 0f, 0f);
        }
    }

    // Leaves a Gold line Trail for a given boid
    void LeaveTrail(int i) {
        if (Trail) {
            Creatures[i].GetComponent<TrailRenderer>().enabled = true;
        } else {
            Creatures[i].GetComponent<TrailRenderer>().enabled = false;
        }
    }

    // Creatures should never exceed high value and fall below low value
    void LimitCreatures() {
        if (NumCreatures < MinCreatues) {
            NumCreatures = MinCreatues;
        } else if (NumCreatures > MaxCreatues) {
            NumCreatures = MaxCreatues;
        }
    }

    // When the user presses the space bar, 
    // cause the Creatures to scatter to random locations in the World Box
    void SpacePress() {
        if (Input.GetKeyDown("space")) {
            for (int i = 0; i < NumCreatures; i++) {
                Creatures[i].transform.position = new Vector3(Random.
                    Range(MinX, MaxX), Random.Range(MinY, MaxY), 
                    Random.Range(MinZ, MaxZ));
            }
        }
    }

    // Finds distance between 2 boids
    float Distance(Vector3 p, Vector3 pj) {
        float px = p[0];
        float py = p[1];
        float pz = p[2];

        float pjx = pj[0];
        float pjy = pj[1];
        float pjz = pj[2];

        float d =  Mathf.Sqrt(Mathf.Pow(px - pjx, 2) + Mathf.Pow(py - pjy, 2) + 
            Mathf.Pow(pz - pjz, 2));

        return d;
    }

    // Calculates the weight of a boid based on its neighbours in given radius
    float Weight(Vector3 p, Vector3 pj, float dmax) {
        float E = 0.01f; // small value epsilon to valid division by 0
        float d = Distance(p, pj);

        // Using an inverse square weight or linear weight based on chance
        float w;
        float chance = Random.Range(0f, 1f);
        if (chance < 0.5) {
            w = 1f / (Mathf.Pow(d, 2) + E);
        } else {
            w = Mathf.Max(dmax - d, 0);
        }

        return w;
    }

    // Calculates the flock centering force of given boid
    Vector3 FlockCenteringForce(int i) {
        Vector3 ffc = new Vector3(0f, 0f, 0f);

        if (!FlockCentering) {
            return ffc;
        }

        float rfc = 5.5f;
        Vector3 p = Creatures[i].transform.position;

        for (int j = 0; j < NumCreatures; j++) {
            Vector3 pj = Creatures[j].transform.position;

            if (j != i && Distance(p, pj) <= rfc) {
                ffc += Weight(p, pj, rfc) * (pj - p) / Weight(p, pj, rfc);
            }
        }

        return ffc;
    }
    
    // Calculates the collision avoidance force of given boid
    Vector3 CollisionAvoidanceForce(int i) {
        Vector3 fca = new Vector3(0f, 0f, 0f);

        if (!CollisionAvoidance) {
            return fca;
        }

        float rca = 5f;
        Vector3 p = Creatures[i].transform.position;

        for (int j = 0; j < NumCreatures; j++) {
            Vector3 pj = Creatures[j].transform.position;
            if (j != i && Distance(p, pj) <= rca) {
                fca += Weight(p, pj, rca) * (p - pj);
            }
        }

        return fca;
    }

    // Calculates the velocity matching force of given boid
    Vector3 VelocityMatchingForce(int i) {
        Vector3 fvm = new Vector3(0f, 0f, 0f);

        if (!VelocityMatching) {
            return fvm;
        }

        float rvm = 5.4f;
        Vector3 p = Creatures[i].transform.position;
        Vector3 v = Velocities[i];

        for (int j = 0; j < NumCreatures; j++) {
            Vector3 pj = Creatures[j].transform.position;
            Vector3 vj = Velocities[j];
            
            if (j != i && Distance(p, pj) <= rvm) {
                fvm += Weight(p, pj, rvm) * (vj - v);
            }
        }

        return fvm;
    }

    // Calculates the flock centering force of given boid
    Vector3 WanderingForce(int i) {
        Vector3 fw = new Vector3(0f, 0f, 0f);

        if (!Wandering) {
            return fw;
        }

        fw += new Vector3(Random.Range(MinY, MaxY), 
            Random.Range(MinY, MaxY), Random.Range(MinY, MaxY));

        return fw;
    }

    // Calculates sum of all Forces and store with boid
    void CalculateForces(int i) {
        // Weights for each force are scalars of choice
        float wfc = 0.3f;
        float wvm = 0.3f;
        float wca = 0.25f;
        float ww = 0.15f;

        // fall
        Forces[i] = wfc * FlockCenteringForce(i) + 
            wvm * VelocityMatchingForce(i) + 
            wca * CollisionAvoidanceForce(i) +
            ww * WanderingForce(i);
    }

    // Updated positions of boids based on new velocity
    void Simulation() {
        for (int i = 0; i < MaxCreatues; i++) {
            if (i < NumCreatures) {
                Creatures[i].SetActive(true);
                CalculateForces(i);
                LeaveTrail(i);
            } else {
                Creatures[i].SetActive(false);
            }
        }

        for (int j = 0; j < NumCreatures; j++) {
            Vector3 v = Velocities[j];
            Velocities[j] += Time.deltaTime * Forces[j];
            Velocities[j] = Vector3.ClampMagnitude(Velocities[j], MaxV);
            Creatures[j].transform.position += Time.deltaTime * 
                ((v + Velocities[j]) / 2f);
            
            // Make sure creatues don't go out of bounds
            float x = Creatures[j].transform.position[0];
            float y = Creatures[j].transform.position[1];
            float z = Creatures[j].transform.position[2];
            if (x <= MinX || x >= MaxX) {
                Velocities[j][0] *= -1;
            } 
            if (y <= MinY || y >= MaxY) {
                Velocities[j][1] *= -1;
            } 
            if (z <= MinX || z >= MaxZ) {
                Velocities[j][2] *= -1;
            }
        }
    }
    
    // Update is called once per frame
    void Update() {
        LimitCreatures();
        Simulation();
        SpacePress();
    }
}
