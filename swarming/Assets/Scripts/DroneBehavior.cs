using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneBehavior : MonoBehaviour {

	// the overall speed of the simulation
	public float speed;
	// max speed any particular drone can move at
	public float maxSpeed;
	// maximum steering power
	public float maxSteer;

	// weights: used to modify the drone's movement
	public float separationWeight;
	public float alignmentWeight ;
	public float cohesionWeight  ;
	public float boundsWeight    ;
	public float homeWeight      ;

	private float neighborRadius = 50f;
	public float desiredSeparation;

	// velocity influences
	public Vector3 _separation;
	public Vector3 _alignment;
	public Vector3 _cohesion;
	public Vector3 _bounds;
	public Vector3 _home;

	// Wandering
	public bool fromHome = false;
	public float distanceHome;
	private bool log = true;

	// Perlin Noise Implementation
	public float separationScale = 50.0f;
	public float cohesionScale = 5.0f;

	// This is literally the distance from home that matters; it is swarmbounds
	private float cohesionOsc = 250f;
	public float xScale = 1.0f;

	// other members of my swarm
	public List<GameObject> drones;
	public SwarmBehavior swarm;

	// Color Lerp
	public Color colorStart = Color.red;
    public Color colorEnd = Color.blue;
    public float duration = 1.0F;
    public Renderer rend;
    private Material trail;

    // Interaction
    private bool scale;
    // private float droneScale;

	void FixedUpdate()
	{
		// we should always apply physics forces in FixedUpdate
		Flock();
	}

	protected virtual void Start()
	{
		rend = GetComponent<Renderer>();
		
		// Get the material list of the trail as per the scripting API.
		trail = GetComponent<TrailRenderer>().material;
		cohesionOsc = (swarm.swarmBounds.x)/2;
	}

	protected virtual void Update()
	{
		// Make the flock throb
		// Oscillate();

		if (Input.GetMouseButtonDown(0)) 
		{
			// desiredSeparation = swarm.flockSize ? 150f : 15f;
		}

	}

	public virtual void Flock()
	{

		Vector3 newVelocity = Vector3.zero;

		CalculateVelocities();

		//transform.forward = _alignment;
		newVelocity += _separation * separationWeight;
		newVelocity += _alignment * alignmentWeight;
		newVelocity += _cohesion * cohesionWeight;
		newVelocity += _bounds * boundsWeight;
		newVelocity += _home * homeWeight;
		newVelocity = newVelocity * speed;
		newVelocity = GetComponent<Rigidbody>().velocity + newVelocity;
		// newVelocity.y = 0f;

		GetComponent<Rigidbody>().velocity = Limit(newVelocity, maxSpeed);
	}

	/// <summary>
	/// Calculates the influence velocities for the drone. We do this in one big loop for efficiency.
	/// </summary>
	protected virtual void CalculateVelocities()
	{
		// the general procedure is that we add up velocities based on the neighbors in our radius for a particular influence (cohesion, separation, etc.) 
		// and divide the sum by the total number of drones in our neighbor radius
		// this produces an evened-out velocity that is aligned with its neighbors to apply to the target drone		
		Vector3 separationSum = Vector3.zero;
		Vector3 alignmentSum = Vector3.zero;
		Vector3 cohesionSum = Vector3.zero;
		Vector3 boundsSum = Vector3.zero;
		Vector3 homeSum = Vector3.zero;

		int separationCount = 0;
		int alignmentCount = 0;
		int cohesionCount = 0;
		int boundsCount = 0;

		for (int i = 0; i < this.drones.Count; i++)
		{
			if (drones[i] == null) continue;

			float distance = Vector3.Distance(transform.position, drones[i].transform.position);

			// Set the desired separation for each drone with respect to the size
			float droneScale = transform.localScale.x;
			// Now this actually works!!!
			// maxSpeed = 125f;
			// maxSpeed -=  (droneScale*2);

			// Scale Separation by Drone Size
			// desiredSeparation *= (droneScale/2);

			// separation
			// calculate separation influence velocity for this drone, based on its preference to keep distance between itself and neighboring drones
			if (distance > 0 && distance < desiredSeparation)
			{
				// calculate vector headed away from myself
				Vector3 direction = transform.position - drones[i].transform.position;	
				direction.Normalize();
				direction = direction / distance; // weight by distance
				separationSum += direction;
				separationCount++;
			}

			// alignment & cohesion
			// calculate alignment influence vector for this drone, based on its preference to be aligned with neighboring drones
			// calculate cohesion influence vector for this drone, based on its preference to be close to neighboring drones
			if (distance > 0 && distance < neighborRadius)
			{
				alignmentSum += drones[i].GetComponent<Rigidbody>().velocity;
				alignmentCount++;

				cohesionSum += drones[i].transform.position;
				cohesionCount++;
			}

			// return home
			if (distance > neighborRadius)
			{
				distanceHome = Vector3.Distance(transform.position, swarm.transform.position);
				if (distanceHome > cohesionOsc) 
				{
					fromHome = true;
					homeSum = transform.position - swarm.transform.position;
					if (log) 
					{
						// Debug.Log("Drifting Away");
						// Debug.Log(homeSum);
						log = false;
					}
				}
				else 
				{
					fromHome = false;
				}
			}

			// Gets a vector that points from the player's position to the target's.
			// Vector3 heading = transform.position - swarm.transform.position;
			// Vector3 distance = heading.magnitude;
			// Vector3 direction = heading / distance; // This is now the normalized direction.


			// bounds
			// calculate the bounds influence vector for this drone, based on whether or not neighboring drones are in bounds
			Bounds bounds = new Bounds(swarm.transform.position, new Vector3(swarm.swarmBounds.x, swarm.swarmBounds.y, swarm.swarmBounds.z));
			if (distance > 0 && distance < neighborRadius && !bounds.Contains(drones[i].transform.position))
			{
				Vector3 diff = transform.position - swarm.transform.position;
				if (diff.magnitude> 0)
				{
					boundsSum += swarm.transform.position;
					boundsCount++;
				}
			}

			// Reset your variables for the next cycle;
			// desiredSeparation = 25f;
		}

		// end
		_separation = separationCount > 0 ? separationSum / separationCount : separationSum;
		_alignment = alignmentCount > 0 ? Limit(alignmentSum / alignmentCount, maxSteer) : alignmentSum;
		_cohesion = cohesionCount > 0 ? Steer(cohesionSum / cohesionCount, true) : cohesionSum;
		_bounds = boundsCount > 0 ? Steer(boundsSum / boundsCount, true) : boundsSum;
		_home = fromHome ? Steer(swarm.transform.position, true) : homeSum;
	}

	/// <summary>
	/// Returns a steering vector to move the drone towards the target
	/// </summary>
	/// <param type="Vector3" name="target"></param>
	/// <param type="bool" name="slowDown"></param>
	protected virtual Vector3 Steer(Vector3 target, bool slowDown)
	{
		// the steering vector
		Vector3 steer = Vector3.zero;
		Vector3 targetDirection = target - transform.position;
		float targetDistance = targetDirection.magnitude;

		// transform.LookAt(Vector3.zero);
		// Quaternion rotation = Quaternion.LookRotation(_alignment);
  //       transform.rotation = rotation;

		// Lerp Color
		float lerp = Map(0 , 1, -150, 150, transform.position.y);
		rend.material.color = Color.Lerp(colorStart, colorEnd, lerp);
		trail.SetColor("_Color", Color.Lerp(colorStart, colorEnd, lerp));

		if (targetDistance > 0)
		{
			// move towards the target
			targetDirection.Normalize();
			
			// we have two options for speed
			if (slowDown && targetDistance < 100f * speed)
			{
				targetDirection *= (maxSpeed * targetDistance / (100f * speed));
				targetDirection *= speed;
			}
			else
			{
				targetDirection *= maxSpeed;
			}

			// set steering vector
			steer = targetDirection - GetComponent<Rigidbody>().velocity;
			steer = Limit(steer, maxSteer);
		}

		return steer;
     
	}

	/// <summary>
	/// Limit the magnitude of a vector to the specified max
	/// </summary>
	/// <param type="Vector3" name="v"></param>
	/// <param type="float" name="max"></param>
	protected virtual Vector3 Limit(Vector3 v, float max)
	{
		if (v.magnitude > max)
		{
			return v.normalized * max;
		}
		else
		{
			return v;
		}
	}

	/// <summary>
	/// Show some gizmos to provide a visual indication of what is happening: white => alignment, magenta => separation, blue => cohesion
	/// </summary>
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, neighborRadius);

		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, transform.position + _alignment.normalized * neighborRadius);

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, transform.position + _separation.normalized * neighborRadius);

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + _cohesion.normalized * neighborRadius);
	}

	void Oscillate()
	{
		// separationWeight = separationScale * Mathf.PerlinNoise(Time.time * 0.05f, 0.0f);
		// cohesionWeight   = cohesionScale * Mathf.PerlinNoise(Time.time * 0.25f, 0.0f);
		cohesionOsc = Mathf.Abs(Mathf.Tan(Time.time / 10) * (swarm.swarmBounds.x));
	}


	public float Map(float from, float to, float from2, float to2, float value)
	{
	        if(value <= from2){
	            return from;
	        }else if(value >= to2){
	            return to;
	        }else{
	            return (to - from) * ((value - from2) / (to2 - from2)) + from;
	        }
	}

}


