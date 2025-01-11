using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CongratsScreenController : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    } 
}
