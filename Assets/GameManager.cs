using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject workerPrefab;
    public GameObject woodPrefab;
    public GameObject stonePrefab;
    public GameObject workerNumberCanvasPrefab; // Reference to the worker number canvas prefab
    public Text woodText;
    public Text stoneText;
    public int woodSpawnCount = 10;
    public int stoneSpawnCount = 10;
    public Vector2 terrainSize = new Vector2(500, 500);
    public Transform playerTransform;
    public float spawnDistanceInFrontOfPlayer = 2.0f;
    public MapController mapController; // Reference to the MapController

    private int woodCount = 0;
    private int stoneCount = 0;
    private int workerCounter = 0; // Counter to keep track of worker numbers
    public List<GameObject> targetedResources = new List<GameObject>();

    void Start()
    {
        SpawnResources(woodPrefab, woodSpawnCount, "Wood");
        SpawnResources(stonePrefab, stoneSpawnCount, "Stone");
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnWorkerToCollect("Wood");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnWorkerToCollect("Stone");
        }
    }

    void SpawnResources(GameObject resourcePrefab, int count, string tag)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(0, terrainSize.x),
                0,
                Random.Range(0, terrainSize.y)
            );

            // Adjust Y position based on terrain height if needed
            if (Terrain.activeTerrain != null)
            {
                position.y = Terrain.activeTerrain.SampleHeight(position) + 1.0f; // Add Y offset
            }

            GameObject resource = Instantiate(resourcePrefab, position, Quaternion.identity);
            resource.tag = tag;
        }
    }

    void SpawnWorkerToCollect(string resourceType)
    {
        if (workerPrefab == null)
        {
            Debug.LogError("Worker Prefab is not assigned!");
            return;
        }

        // Calculate spawn position in front of the player
        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * spawnDistanceInFrontOfPlayer;

        // Ensure the worker spawns slightly above the ground to avoid collision issues
        spawnPosition.y = 1.0f; // Fix Y position

        GameObject worker = Instantiate(workerPrefab, spawnPosition, Quaternion.identity);
        Worker workerScript = worker.GetComponent<Worker>();
        workerScript.resourceType = resourceType;
        workerScript.OnResourceCollected += OnResourceCollected;
        workerScript.gameManager = this; // Automatically assign the gameManager

        // Set worker color based on resource type
        if (resourceType == "Wood")
        {
            worker.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (resourceType == "Stone")
        {
            worker.GetComponent<Renderer>().material.color = Color.grey;
        }

        // Increment worker counter and assign number
        workerCounter++;
        GameObject workerNumberCanvas = Instantiate(workerNumberCanvasPrefab, worker.transform);
        Text workerNumberText = workerNumberCanvas.GetComponentInChildren<Text>();
        workerNumberText.text = workerCounter.ToString();
        workerNumberCanvas.transform.localPosition = new Vector3(0, 2.0f, 0); // Position canvas above worker

        GameObject target = FindNearestResource(resourceType);
        if (target == null)
        {
            Debug.LogError("No target resource found for type: " + resourceType);
            Destroy(worker);
            return;
        }

        targetedResources.Add(target);
        workerScript.target = target.transform;

        // Add worker to map controller
        mapController.workers.Add(workerScript);
    }

    public GameObject FindNearestResource(string resourceType)
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag(resourceType);
        GameObject nearestResource = null;
        float minDistance = float.MaxValue;

        foreach (GameObject resource in resources)
        {
            if (resource != null && !targetedResources.Contains(resource)) // Ensure resource is not targeted
            {
                float distance = Vector3.Distance(resource.transform.position, playerTransform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestResource = resource;
                }
            }
        }

        return nearestResource;
    }

    void OnResourceCollected(string resourceType)
    {
        if (resourceType == "Wood")
        {
            woodCount++;
        }
        else if (resourceType == "Stone")
        {
            stoneCount++;
        }

        UpdateUI();
    }

    public void RemoveTargetedResource(GameObject resource)
    {
        targetedResources.Remove(resource);
    }

    void UpdateUI()
    {
        woodText.text = "Wood: " + woodCount;
        stoneText.text = "Stone: " + stoneCount;
    }
}
