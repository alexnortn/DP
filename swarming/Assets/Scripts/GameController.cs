using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject[] thoughts;
	public Vector3 spawnValues;
	public Vector2 spawnScaleRange;
	public List<GameObject> thoughtList;
	public AudioClip[] audioList;
	public int thoughtCount;
	public float spawnWait;
	public float startWait;
	private float waveWait;

	private int spawnScale;

	// Use this for initialization
	void Start () {
		waveWait = 6f;
		thoughtList = new List<GameObject>();
		Physics.gravity = new Vector3(0, -0.5f, 0);
		StartCoroutine (SpawnWaves ());	

		//Loading the audio items into the array
        audioList =  new AudioClip[]
        {
        	Resources.Load("Sounds/thoughtSound1") as AudioClip,
        	Resources.Load("Sounds/thoughtSound2") as AudioClip,
        	Resources.Load("Sounds/thoughtSound3") as AudioClip,
        	Resources.Load("Sounds/thoughtSound4") as AudioClip,
        	Resources.Load("Sounds/thoughtSound5") as AudioClip,
        	Resources.Load("Sounds/thoughtSound6") as AudioClip,
        	Resources.Load("Sounds/thoughtSound7") as AudioClip,
        	Resources.Load("Sounds/thoughtSound8") as AudioClip,
        	Resources.Load("Sounds/thoughtSound9") as AudioClip,
        	Resources.Load("Sounds/thoughtSound10") as AudioClip,
        	Resources.Load("Sounds/thoughtSound11") as AudioClip
        	
        };
	}
	
	// Update is called once per frame
	void Update () {
		float probPick = Mathf.RoundToInt(Random.Range(0, 10));
		if (probPick > 8)
		{
			// int ty = Mathf.RoundToInt(Random.Range(0, thoughtCount));
		}
	
	}

	IEnumerator SpawnWaves ()
	{
		GameObject thought;
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			for (int i = 0; i < thoughtCount; i++)
			{
				// thought = thoughts [Random.Range (0, thoughts.Length)];

				spawnScale = Mathf.RoundToInt(Random.Range(spawnScaleRange.x, spawnScaleRange.y));

				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, Random.Range (-spawnValues.z, spawnValues.z));
				Quaternion spawnRotation = Quaternion.identity;
				// Instantiate (thought, spawnPosition, spawnRotation);

				thought = (GameObject) GameObject.Instantiate(thoughts [Random.Range (0, thoughts.Length)], spawnPosition, spawnRotation);
				thought.transform.localScale *= spawnScale;
				// thought.AddComponent(AudioSource);
				thought.audio.clip = audioList[Random.Range(0,11)];
				thought.audio.loop = true;
				thought.audio.volume = Random.Range(0.25F, 0.5F);
				thought.audio.Play();

				thoughtList.Add(thought);

				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
			waveWait = Random.Range(4f, 20f);
		}
	}
}
