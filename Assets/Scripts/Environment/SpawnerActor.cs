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
    private ArrayList SpawnedObjects = new ArrayList();

	void Start ()
    {
	    Initialise();
        SpawnedObjects.Clear();
        SecondsBetweenSpawns = 1.0f / SpawnRateSeconds;
	}
	
	void Update ()
    {
	    UpdateActor();
        RemoveDeadSpawns();
        if (Time.time - LastSpawnTime > SecondsBetweenSpawns)
        { 
            Spawn();
            LastSpawnTime = Time.time;
        }
	}

    public virtual void Spawn()
    {
        int numSpawnedChildren = SpawnedObjects.Count;
        if (ObjectToSpawn != null && numSpawnedChildren < MaxConcurrentSpaws)
        {
            Object spawned = Instantiate (ObjectToSpawn, SpawnLocation.position, transform.rotation);
            GameObject spawnedObject = (GameObject) spawned;
            SpawnedObjects.Add (spawnedObject);
            //spawnedObject.transform.parent        = GunEnd;
            //spawnedObject.transform.localRotation = Quaternion.identity;
            //spawnedObject.transform.localPosition = Vector3.zero;
            //spawnedObject.transform.localScale    = Vector3.one;
        }
    }

    private void RemoveDeadSpawns()
    {
        ArrayList indexesToRemove = new ArrayList();
        indexesToRemove.Clear();
        for (int i = 0; i < SpawnedObjects.Count; ++i)
            if ((GameObject) SpawnedObjects[i] == null)
                indexesToRemove.Add (i);
        for (int iIndex = 0; iIndex < indexesToRemove.Count; ++iIndex)
            SpawnedObjects.RemoveAt ((int) indexesToRemove[iIndex]);
    }
}
