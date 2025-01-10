#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSystem : MonoBehaviour
{
    private Vector3 respawnPosition;
    private Rigidbody rigidbody;
    private BallMaterial playerBallMaterial;
    public Transform player;
    public GameObject heart3D;
    private Vector3 originalHeartPosition;

    private bool isInitialized = false;
	
	#if UNITY_EDITOR
    [InitializeOnLoad]
    public class ClearPlayerPrefsOnExitPlayMode
    {
        static ClearPlayerPrefsOnExitPlayMode()
        {
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        static void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Delete PlayerPrefs when exiting Play mode
                PlayerPrefs.DeleteAll();
            }
        }
    }
#endif

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInitialized = false;
    }
	
	private void OnSceneUnloaded(Scene scene)
    {
        PlayerPrefs.DeleteAll();
    }

    void LateUpdate()
    {
        if (!isInitialized)
        {
            InitializePlayerAndComponents();
            isInitialized = true;
        }
    }

    private void InitializePlayerAndComponents()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        if (player == null)
        {
            Debug.LogError("Player Transform not found.");
            return;
        }

        rigidbody = player.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Debug.LogError("Player Rigidbody component not found.");
            return;
        }

        playerBallMaterial = rigidbody.GetComponent<MaterialController>()?.ballMaterial;

        if (heart3D != null)
        {
            originalHeartPosition = heart3D.transform.position;
        }

        // Handle checkpoint data
        SetPlayerPositionAndMaterial();
    }

    private void SetPlayerPositionAndMaterial()
    {
        if (PlayerPrefs.HasKey("RespawnX") && PlayerPrefs.HasKey("RespawnY") && PlayerPrefs.HasKey("RespawnZ"))
        {
            float x = PlayerPrefs.GetFloat("RespawnX");
            float y = PlayerPrefs.GetFloat("RespawnY");
            float z = PlayerPrefs.GetFloat("RespawnZ");
            respawnPosition = new Vector3(x, y, z);
            player.position = respawnPosition;
            Debug.Log($"Player respawned at: {respawnPosition}");
        }
        else
        {
            Debug.Log("No checkpoint data found in PlayerPrefs.");
        }

        LoadBallMaterial();
    }

    public void SaveBallMaterial()
    {
        if (playerBallMaterial != null)
        {
            string path = "BallMaterials/" + playerBallMaterial.name;
            PlayerPrefs.SetString("BallMaterialPath", path);
            Debug.Log("Ball material path saved: " + path);
        }
        else
        {
            Debug.LogWarning("No BallMaterial assigned to save.");
        }
    }

    public void LoadBallMaterial()
    {
        if (PlayerPrefs.HasKey("BallMaterialPath"))
        {
            string path = PlayerPrefs.GetString("BallMaterialPath");
            BallMaterial loadedMaterial = Resources.Load<BallMaterial>(path);

            if (loadedMaterial != null)
            {
                rigidbody.GetComponent<MaterialController>().SetBallMaterial(loadedMaterial);
                Debug.Log("Ball material loaded: " + loadedMaterial.name);
            }
            else
            {
                Debug.LogError("Failed to load BallMaterial from path: " + path);
            }
        }
        else
        {
            Debug.LogWarning("No saved BallMaterial path found in PlayerPrefs.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            respawnPosition = other.transform.position;

            PlayerPrefs.SetFloat("RespawnX", respawnPosition.x);
            PlayerPrefs.SetFloat("RespawnY", respawnPosition.y);
            PlayerPrefs.SetFloat("RespawnZ", respawnPosition.z);
            SaveBallMaterial();
            PlayerPrefs.Save();

            Debug.Log("Checkpoint reached!");
            Debug.Log($"Player respawn point set to: {respawnPosition}");
        }
    }

    public void RespawnPlayer()
    {
        LifeSystem lifeSystem = FindObjectOfType<LifeSystem>();

        if (lifeSystem != null)
        {
            if (LifeSystem.lives <= 1)
            {
                PlayerPrefs.DeleteAll();
                ResetHeart();
                LifeSystem.lives = lifeSystem.startLives;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
            else
            {
                lifeSystem.LoseLife();
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ResetHeart()
    {
        if (heart3D != null)
        {
            heart3D.SetActive(true);
            heart3D.transform.position = originalHeartPosition;
        }

        if (LifeSystem.collectedHearts.Contains(heart3D.name))
        {
            LifeSystem.collectedHearts.Remove(heart3D.name);
        }
    }
}
