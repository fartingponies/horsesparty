using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class StatusInfo : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public float deltaTime;

    private int frame;

    void Update()
    {
        deltaTime += Time.deltaTime;
        float fps = frame / deltaTime;

        int ponies = GameObject.FindGameObjectsWithTag("Pony").Length;

        fpsText.text = ponies.ToString() + " Ponies\n" + Mathf.Ceil(fps).ToString() + " FPS\n" + SystemInfo.graphicsMemorySize + "/" + SystemInfo.systemMemorySize + " MB";

        if (++frame > 120)
        {
            frame = 1;
            deltaTime = 0f;
        }
    }
}