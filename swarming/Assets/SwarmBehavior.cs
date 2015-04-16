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

	public Vector3 swarmBounds = new Vector3(300f, 300f, 300f);

	public GameObject prefab;

    // Interaction
    public bool flockSize;

	// Use this for initialization
	protected virtual void Start () {
		if (prefab == null)
		{
			// end early
			Debug.Log("Please assign a drone prefab.");
			return;

			flockSize = false;
		}

		// instantiate the drones
		GameObject droneTemp;
		drones = new List<GameObject>();
		for (int i = 0; i < droneCount; i++)
		{
			droneTemp = (GameObject) GameObject.Instantiate(prefab);
			DroneBehavior db = droneTemp.GetComponent<DroneBehavior>();
			db.drones = this.drones;
			db.swarm = this;

			// spawn inside circle
			Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z) * spawnRadius;
			droneTemp.transform.position = new Vector3(pos.x, pos.y, pos.z);
			droneTemp.transform.parent = transform;
			
			drones.Add(droneTemp);
		}
	}
	
	// Update is called once per frame
	protected virtual void Update () {

		// Interaction
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
	
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(swarmBounds.x, swarmBounds.y, swarmBounds.z));
		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}
}
