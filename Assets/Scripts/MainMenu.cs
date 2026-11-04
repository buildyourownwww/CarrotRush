using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] string firstLevel = "Level1";
    public void Play()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(firstLevel);
    }
    public void Quit()
{
Debug.Log("Quit pressed");
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false; 
#else
    Application.Quit(); 
#endif
    }   
}