using UnityEngine;

public class HeartCollectible : MonoBehaviour
{
    public string heartID; // unique ID 
    private void Start()
    {
        // we check if this heart has already been collected
        if (LifeSystem.collectedHearts.Contains(heartID))
        {
            gameObject.SetActive(false); // we hide it 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // add a life
            LifeSystem lifeSystem = FindObjectOfType<LifeSystem>();
            if (lifeSystem != null)
            {
                lifeSystem.AddLife();
            }

            // mark this heart as collected
            LifeSystem.collectedHearts.Add(heartID);

            // we hide it
            gameObject.SetActive(false);
        }
    }
}
