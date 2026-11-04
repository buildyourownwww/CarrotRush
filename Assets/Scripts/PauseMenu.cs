using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
[SerializeField] GameObject pausePanel; // the whole pause screen
[SerializeField] GameObject controls; // joystick, JUMP, FIRE, pause
[SerializeField] string menuScene = "MainMenu";
public static bool IsPaused { get; private set; }
void Start()
{
SetPaused(false);
}
void Update()
{
// Esc for testing in the editor
if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
SetPaused(!IsPaused);
}
public void SetPaused(bool paused)
{
IsPaused = paused;
pausePanel.SetActive(paused);
if (controls != null) controls.SetActive(!paused);
Time.timeScale = paused ? 0f : 1f; // physics and movement stop
}
public void Pause() { SetPaused(true); }
public void Resume() { SetPaused(false); }
public void Restart()
{
Time.timeScale = 1f;
IsPaused = false;
SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
public void ToMenu()
{
Time.timeScale = 1f;
IsPaused = false;
SceneManager.LoadScene(menuScene);
}
// the player switched to another app: pause automatically
void OnApplicationPause(bool appPaused)
{
if (appPaused) SetPaused(true);
}
}