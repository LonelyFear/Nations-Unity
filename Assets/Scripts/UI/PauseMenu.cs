using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField]
    TimeManager timeManager;

    [SerializeField]
    GameObject pausePanel;

    [SerializeField]
    NationPanel nationPanel;
    public bool open { get; private set; } = false;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (!open){
                open = true;
                timeManager.Pause();
            } else {
                open = false;
                timeManager.Resume();
            }
        }
        pausePanel.SetActive(open);
        if (open){
            if (nationPanel.isActiveAndEnabled){
                nationPanel.Disable();
            }
            timeManager.Pause();
        }
    }
    public void OpenMainMenu(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ReturnToGame(){
        open = false;
        timeManager.Resume();
    }
}
