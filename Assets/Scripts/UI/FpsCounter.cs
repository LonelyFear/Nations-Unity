using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class FpsCounter : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    float frameCounter;
    float timeCounter;
    float lastFramerate;
    public float refreshTime = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timeCounter < refreshTime)
        {
            timeCounter += Time.deltaTime;
            frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            lastFramerate = (float)frameCounter/timeCounter;
            frameCounter = 0;
            timeCounter = 0.0f;
        }
        textMeshPro.text = "FPS: " + Mathf.RoundToInt(lastFramerate).ToString();
    }
}
