using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    public GameObject mapPanel;
    public GameObject woodWorkerDotPrefab;
    public GameObject stoneWorkerDotPrefab;
    public GameObject playerDotPrefab;
    public Transform playerTransform;
    public List<Worker> workers;

    private GameObject playerDot;
    private Dictionary<Worker, GameObject> workerDots = new Dictionary<Worker, GameObject>();

    void Start()
    {
        mapPanel.SetActive(false); // Hide the map initially

        // Create player dot
        playerDot = Instantiate(playerDotPrefab, mapPanel.transform);
    }

    void Update()
    {
        // Toggle map visibility
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapPanel.SetActive(!mapPanel.activeSelf);
        }

        if (mapPanel.activeSelf)
        {
            UpdateMap();
        }
    }

    void UpdateMap()
    {
        // Update player position on the map
        Vector2 playerPos = new Vector2(playerTransform.position.x, playerTransform.position.z);
        playerDot.GetComponent<RectTransform>().anchoredPosition = playerPos;

        // Update worker positions on the map
        foreach (var worker in workers)
        {
            if (!workerDots.ContainsKey(worker))
            {
                GameObject dotPrefab = worker.resourceType == "Wood" ? woodWorkerDotPrefab : stoneWorkerDotPrefab;
                GameObject dot = Instantiate(dotPrefab, mapPanel.transform);
                workerDots[worker] = dot;
            }

            Vector2 workerPos = new Vector2(worker.transform.position.x, worker.transform.position.z);
            workerDots[worker].GetComponent<RectTransform>().anchoredPosition = workerPos;
        }
    }
}
