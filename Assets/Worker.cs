using UnityEngine;
using System.Collections;

public class Worker : MonoBehaviour
{
    public Transform target;
    public string resourceType;
    public delegate void ResourceCollectedHandler(string resourceType);
    public event ResourceCollectedHandler OnResourceCollected;
    public GameManager gameManager;

    private float speed = 5f;

    void Start()
    {
        // Ensure the worker starts slightly above the ground
        Vector3 startPosition = transform.position;
        startPosition.y = Terrain.activeTerrain.SampleHeight(startPosition) + 1.0f;
        transform.position = startPosition;
    }

    void Update()
    {
        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // Adjust Y position based on terrain height
            Vector3 position = transform.position;
            position.y = Terrain.activeTerrain.SampleHeight(position) + 1.0f;
            transform.position = position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(resourceType))
        {
            gameManager.RemoveTargetedResource(other.gameObject);
            Destroy(other.gameObject);
            OnResourceCollected?.Invoke(resourceType);

            // Add delay before finding the next target
            StartCoroutine(FindNextTargetWithDelay(0.1f));
        }
    }

    IEnumerator FindNextTargetWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject nextTarget = gameManager.FindNearestResource(resourceType);
        if (nextTarget != null)
        {
            target = nextTarget.transform;
            gameManager.targetedResources.Add(nextTarget);
        }
        else
        {
            Destroy(gameObject); // Destroy worker if no more resources are left
        }
    }
}
