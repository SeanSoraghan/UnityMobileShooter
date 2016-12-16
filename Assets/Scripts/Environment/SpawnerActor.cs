using UnityEngine;
using System.Collections;

public class SpawnerActor : ActorController
{
    public GameObject ObjectToSpawn;
    public Transform  SpawnLocation;
    public float      SpawnRateSeconds   = 5.0f;
    public int        MaxConcurrentSpaws = 7;

    private float SecondsBetweenSpawns = 1.0f;
    private float LastSpawnTime        = 0.0f;
    private int   NumSpawnedChildren   = 0;

	void Start ()
    {
	    Initialise();
        SecondsBetweenSpawns = 1.0f / SpawnRateSeconds;
	}
	
	void Update ()
    {
	    UpdateActor();
        if (Time.time - LastSpawnTime > SecondsBetweenSpawns)
        { 
            Spawn();
            LastSpawnTime = Time.time;
        }
	}

    public void DecrementNumLiveSpawns() { NumSpawnedChildren --; }

    public virtual void Spawn()
    {
        Debug.Log (NumSpawnedChildren);
        if (ObjectToSpawn != null && NumSpawnedChildren < MaxConcurrentSpaws)
        {
            Object spawned = Instantiate (ObjectToSpawn, SpawnLocation.position, transform.rotation);
            GameObject spawnedObject = (GameObject) spawned;
            NumSpawnedChildren ++;
            //spawnedObject.transform.parent        = GunEnd;
            //spawnedObject.transform.localRotation = Quaternion.identity;
            //spawnedObject.transform.localPosition = Vector3.zero;
            //spawnedObject.transform.localScale    = Vector3.one;
        }
    }
}
