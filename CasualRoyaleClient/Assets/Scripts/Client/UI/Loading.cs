using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] Image progressBar;

    private void Start()
    {
        Debug.Log("Start");

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        Debug.Log("LoadScene");
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}