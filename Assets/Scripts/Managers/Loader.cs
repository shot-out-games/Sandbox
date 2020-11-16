using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class Loader : MonoBehaviour
{



    float loadTime = 2.0f;
    private int currentSceneIndex = 0;


    void Awake()
    {

        AudioListener[] allListeners = UnityEngine.Object.FindObjectsOfType<AudioListener>();
        if (allListeners.Length > 1)
        {
            for (int i = 1; i < allListeners.Length; i++)
            {
                DestroyImmediate(allListeners[i]);

            }
        }

        Camera[] allCameras = UnityEngine.Object.FindObjectsOfType<Camera>();
        if (allCameras.Length > 1)
        {
            foreach (Camera cam in allCameras)
            {
                if (cam.gameObject.tag != "MainCamera")
                {
                    DestroyImmediate(cam.gameObject);
                }
            }
        }

        
    }


    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        

        if (SceneManager.GetActiveScene().buildIndex <= 0)
        {
            StartCoroutine(LoadYourAsyncScene(1));
        }

        else
        {
            StartCoroutine(LoadYourAsyncScene(currentSceneIndex));
        }
    }

   

    IEnumerator LoadYourAsyncScene(int sceneIndex)
    {


        yield return new WaitForSeconds(loadTime);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads

        while (!asyncOperation.isDone)
        {
            //Output the current progress
            Debug.Log("Loading progress: " + (asyncOperation.progress * 100) + "%");

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Activate the Scene
                asyncOperation.allowSceneActivation = true;
                float pct = asyncOperation.progress;


            }

            yield return null;


        }

    }




}





