using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class MenuToggleButton : MonoBehaviour
{
    public void OnMenuButtonPressed()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}
