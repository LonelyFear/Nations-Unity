using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject loadingMenu;

    [SerializeField]
    TextMeshProUGUI loadingText;
    public void PlayGame(){
        print("Thanks for playing!");
        StartCoroutine(loadAsync(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void QuitGame(){
        print("Game Closed, Thanks for playing!");
        Application.Quit();
    }

    IEnumerator loadAsync(int sceneId){
        AsyncOperation playGame = SceneManager.LoadSceneAsync(sceneId);

        gameObject.SetActive(false);
        loadingMenu.SetActive(true);

        while (!playGame.isDone){
            if (loadingText != null){
                loadingText.text = "Loading - " + Mathf.Round(playGame.progress * 100) + "%";
            }

            yield return null;
        }
    }
}
