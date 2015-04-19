using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject[] thoughts;
	public Vector3 spawnValues;
	public int thoughtCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	// Use this for initialization
	void Start () {
		StartCoroutine (SpawnWaves ());	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			for (int i = 0; i < thoughtCount; i++)
			{
				GameObject thought = thoughts [Random.Range (0, thoughts.Length)];
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (thought, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);

		}
	}
}
