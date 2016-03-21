using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// An implementation of the flocking algorithm: http://www.red3d.com/cwr/boids/
// Additional resources:
// http://harry.me/2011/02/17/neat-algorithms---flocking/
public class SwarmBehavior : MonoBehaviour {
	/// <summary>
	/// the number of drones we want in this swarm
	/// </summary>
	public int droneCount = 150;
	public float spawnRadius = 100f;
	public List<GameObject> drones;

	public Vector3 swarmBounds = new Vector3(750f, 750f, 750f);

	public GameObject prefab;

    // Interaction
    public bool flockSize;
    public bool make;
    private int count;
    private float countdown;
    private float swarmTimer;


	//  VR update headset Transform, return offset
	private float lookOffset () {
		float headsetRotation,
			  negheadsetRotation,
			  normHeadsetRotation;

		// Get headset tracking from OVR
		headsetRotation = GameObject.Find("CenterEyeAnchor").transform.eulerAngles.x;

		negheadsetRotation = 360 - headsetRotation;
		normHeadsetRotation =  negheadsetRotation > headsetRotation ? (-1 * headsetRotation) : negheadsetRotation;

		// print(normHeadsetRotation);
		normHeadsetRotation = map(-90, 90, 100, 1000, normHeadsetRotation);
		
		return normHeadsetRotation;
	}

	// Map function, based off of Processing implementation
	public float map (float OldMin, float OldMax, float NewMin, float NewMax, float OldValue) {
     
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
     
        return NewValue;
    }

	// Use this for initialization
	protected virtual void Start () {

		drones = new List<GameObject>();
		make = true;
		swarmTimer = 10.0f;
		flockSize = false;
		countdown = 5.0f;

		if (prefab == null)
		{
			// end early
			Debug.Log("Please assign a drone prefab.");
			return;
		}
	}
	
	// Update is called once per frame
	protected virtual void Update () {

		// Instantiate all the drone prefabs
		if (make) makeDrones();

		// VR headset
		float thoughtSpace = lookOffset();

		// Procedurally set thoughtSpace
		swarmBounds.x = thoughtSpace; 
		swarmBounds.y = thoughtSpace;
		swarmBounds.z = thoughtSpace;

		// Auto-Swarm
		swarmFlock();

		// Interaction
		/*
		if (Input.GetMouseButtonDown(0)) 
		{
			flockSize = !flockSize;
            Debug.Log("Pressed left click.");
            if (flockSize) 
            {
            	swarmBounds.x = 25;
            	swarmBounds.y = 25;
            	swarmBounds.z = 25;
			} 
			else
			{
				swarmBounds.x = 300;
				swarmBounds.y = 300;
				swarmBounds.z = 300;
			}
		}
		*/	
	}

	protected virtual void OnDrawGizmosSelected() {
		Gizmos.DrawWireCube(transform.position, new Vector3(swarmBounds.x, swarmBounds.y, swarmBounds.z));
		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}

	protected virtual void makeDrones() {
		// instantiate the drones
		GameObject droneTemp;
		int droneScale;
		// int droneColliderScale;

		// drones = new List<GameObject>();
		for (int i = 0; i < droneCount; i++)
		{
			countdown -= Time.deltaTime;

			if(countdown <= 0) 
			{
				droneScale = Mathf.RoundToInt(Random.Range(1.0f, 6.0f));
				// droneColliderScale = Mathf.RoundToInt(Mathf.Ceil(Mathf.Sqrt(droneScale)));

				droneTemp = (GameObject) GameObject.Instantiate(prefab);
				DroneBehavior db = droneTemp.GetComponent<DroneBehavior>();
				db.drones = this.drones;
				db.swarm = this;

				// spawn inside circle
				Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z) * spawnRadius;
				droneTemp.transform.position = new Vector3(pos.x, pos.y, pos.z);
				droneTemp.transform.parent = transform;
				// Random scale
				droneTemp.transform.localScale = new Vector3(droneScale, droneScale, droneScale);
				droneTemp.GetComponent<TrailRenderer>().startWidth = droneScale;
				// droneTemp.GetComponent<BoxCollider>().size = new Vector3(droneColliderScale, droneColliderScale, droneColliderScale);
				
				drones.Add(droneTemp);

				count++;
				countdown = 5.0f;

				if (count >= droneCount) make = false;
			}

		}
	}

	protected virtual void swarmFlock() {
		swarmTimer -= Time.deltaTime;
		// Debug.Log(swarmTimer);

		if (swarmTimer <= 0)
		{
			flockSize = !flockSize;
            Debug.Log("Swarming...");
            if (flockSize) 
            {
            	swarmBounds.x = 25.0f;
            	swarmBounds.y = 25.0f;
            	swarmBounds.z = 25.0f;
            	swarmTimer = Random.Range(2.5f , 5.0f);
            	Debug.Log("Swarming for " + swarmTimer);
			} 
			else
			{
				swarmBounds.x = 750f;
				swarmBounds.y = 750f;
				swarmBounds.z = 750f;
				swarmTimer = Random.Range(30.0f , 60.0f);
				Debug.Log("Swarming for " + swarmTimer);
			}
		}
	}
}
