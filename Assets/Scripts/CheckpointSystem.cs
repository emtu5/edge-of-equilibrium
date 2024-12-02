#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSystem : MonoBehaviour
{
    private Vector3 respawnPosition;
    private Rigidbody rigidbody;
    public Transform player;
	
	//to delete PlayerPrefs if using the unity editor and not the build version
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
	
	private void OnSceneUnloaded(Scene scene)
	{
		PlayerPrefs.DeleteAll();
	}
		
		
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
		// Load respawn position from PlayerPrefs
		float x = PlayerPrefs.GetFloat("RespawnX");
		float y = PlayerPrefs.GetFloat("RespawnY");
		float z = PlayerPrefs.GetFloat("RespawnZ");
		respawnPosition = new Vector3(x, y, z);
        rigidbody = player.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            respawnPosition = other.transform.position;

            // Save the new respawn position to PlayerPrefs
            PlayerPrefs.SetFloat("RespawnX", respawnPosition.x);
            PlayerPrefs.SetFloat("RespawnY", respawnPosition.y);
            PlayerPrefs.SetFloat("RespawnZ", respawnPosition.z);
            PlayerPrefs.Save();

            float x = PlayerPrefs.GetFloat("RespawnX");
            float y = PlayerPrefs.GetFloat("RespawnY");
            float z = PlayerPrefs.GetFloat("RespawnZ");

            Debug.Log("Checkpoint reached!");
            Debug.Log($"Player respawn point set to: {respawnPosition}");
            Debug.Log($"PlayerPrefs: {x}, {y}, {z}");
        }
    }

    public void RespawnPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        float x = PlayerPrefs.GetFloat("RespawnX"); 
        float y = PlayerPrefs.GetFloat("RespawnY"); 
        float z = PlayerPrefs.GetFloat("RespawnZ");  

		//if PlayerPrefs are not set, no checkpoint has been reached, no changes made to the scene reload
		//if they are player position is changed after scene has been reloaded
		if (PlayerPrefs.HasKey("RespawnX") && PlayerPrefs.HasKey("RespawnY") && PlayerPrefs.HasKey("RespawnZ"))
		{
			respawnPosition = new Vector3(x, y, z);
			player.position = respawnPosition;
			Debug.Log($"Player respawned at: {respawnPosition}");
		}
    }
}
