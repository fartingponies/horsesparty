using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public GameObject fadeOutUIImage;
    private new SpriteRenderer renderer;
    public float fadeSpeed = 1f;

    public enum FadeDirection { In, Out }

    void Awake() {
        renderer = fadeOutUIImage.GetComponent<SpriteRenderer>();

        fadeOutUIImage.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(Fade(FadeDirection.Out));
    }

    public IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = (fadeDirection == FadeDirection.Out) ? 1 : 0;
        float fadeEndValue = (fadeDirection == FadeDirection.Out) ? 0 : 1;

        if (fadeDirection == FadeDirection.Out)
        {
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
            fadeOutUIImage.SetActive(false);
        }
        else
        {
            fadeOutUIImage.SetActive(true);
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string sceneToLoad)
    {
        yield return Fade(fadeDirection);
        SceneManager.LoadScene(sceneToLoad);
    }

    private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == FadeDirection.Out) ? -1 : 1);
    }
}