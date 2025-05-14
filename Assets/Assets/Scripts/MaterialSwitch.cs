using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitch : MonoBehaviour
{
    [SerializeField]
    private BallMaterial ballMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<MaterialController>().SetBallMaterial(ballMaterial);
            Debug.Log("Player switched material!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
