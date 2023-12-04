using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Threading;

using static GameManager;
using UnityEngine.EventSystems;
using System.Collections;

public class Common : MonoBehaviour
{
    private static MonoBehaviour instance;

    private static LocalizationSettings localizationSettings;

    public static GameObject[,,,,] prefabs = null;
    public static Vector2 lowerLeft, upperRight, safeLowerLeft, safeUpperRight;

    public static Vector2[,] unicornBackgroundOffsets = new Vector2[2, 2];
    public static Vector2[,] jumpAnimationOffsets = new Vector2[2, 2];
    public static Vector2 unicornAlignmentOffset = new Vector2(-0.12f, 0.09f);

    private static string[] types = { "pony", "unicorn" };
    private static string[] genders = { "male", "female" };
    private static string[] anims = { "walk", "jump" };
    private static string[] layers = { "bg", "fg" };
    private static int[] animFrames = { 25, 30 };

    public static AnimationCurve easeInOut;

    public new Camera camera;

    static Common()
    {
        unicornBackgroundOffsets[BOY, WALK] = new Vector2(0.115f, -0.1f);
        unicornBackgroundOffsets[BOY, JUMP] = new Vector2(0.12f, -0.19f);
        unicornBackgroundOffsets[GIRL, WALK] = new Vector2(0.115f, -0.1f);
        unicornBackgroundOffsets[GIRL, JUMP] = new Vector2(0.18f, -0.16f);

        jumpAnimationOffsets[PONY, BOY] = new Vector2(-0.09f, 0.35f);
        jumpAnimationOffsets[PONY, GIRL] = new Vector2(-0.04f, 0.39f);
        jumpAnimationOffsets[UNICORN, GIRL] = new Vector2(-0.01f, 0.44f);
        jumpAnimationOffsets[UNICORN, BOY] = new Vector2(-0.085f, 0.44f);
    }

    void Awake()
    {
        instance = this;

        easeInOut = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        //LocalizationSettings.InitializeSynchronously = true;
        //await LocalizationSettings.InitializationOperation.Task;
        
        if (prefabs == null)
        {
            lowerLeft = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f));
            upperRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

            safeLowerLeft = Camera.main.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMin, Screen.safeArea.yMin));
            safeUpperRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMax, Screen.safeArea.yMax));

            //Debug.Log($"Lower left {lowerLeft.x} {lowerLeft.y}\nUpper right {upperRight.x} {upperRight.y}");

            prefabs = new GameObject[2, 2, 2, 2, 30];

            for (int type = 0; type < 2; type++)
            {
                for (int gender = 0; gender < 2; gender++)
                {
                    for (int anim = 0; anim < 2; anim++)
                    {
                        for (int layer = 0; layer < 2; layer++)
                        {
                            if (!(type == UNICORN && layer == BG))
                            {
                                string path = $"horses/{types[type]}_{genders[gender]}_{anims[anim]}_{layers[layer]}_";

                                for (int frame = 0; frame < animFrames[anim]; frame++)
                                {
                                    prefabs[type, gender, anim, layer, frame] = Resources.Load<GameObject>(path + (frame + 1).ToString("D4"));
                                    //Debug.Log("Prefab: " + prefabs[type, gender, anim, layer, frame]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static void FadeToScene(string scene)
    {
        instance.StartCoroutine(FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, scene));
    }

    public static string GetString(string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(SceneManager.GetActiveScene().name, name);
    }

    public static IEnumerator SmoothCoroutine(SmoothAction action, float delay)
    {
        float startTime = Time.time;

        while (Time.time - startTime < delay)
        {
            float t = easeInOut.Evaluate((Time.time - startTime) / delay);
            action(t);
            yield return null;
        }
    }

    public delegate void SmoothAction(float f);
}
