using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using static Common;
using static GameManager;
using static GameManager.GameState;
using static Horse.HorseStyle;
using static Horse.HorseState;
using static UnityEngine.UI.Image;
//
public class Horse : MonoBehaviour
{
    private GameManager gameManager;

    public enum HorseStyle { COLORED, TEXTURED, GRAYSCALE, RAINBOW };
    private HorseStyle horseStyle;

    private GameObject[,,] objects = new GameObject[2, 2, 30];

    private AudioSource audioSource;
    public AudioClip jumpSFX;
    public ParticleSystem particles;

    public enum HorseState { WALKING, JUMPING, RUNNING };
    public HorseState horseState;

    public int horseType;
    public int horseGender;
    
    private float speed;

    static int order;

    public float horseSize;
    public float flipSign;
    
    private Color color;

    public Material rainbowMat;
    public Material grayscaleMat;
    public Material tiledTextureMat;
    public Texture2D[] tiledTextures;
    public Texture2D tiledTexture;

    private int currentFrame;

    public AnimationCurve fadingEasingCurve;

    private GameObject activeBackground;
    [System.NonSerialized] public GameObject activeForeground;

    private static Vector2[,] unicornBackgroundOffsets = new Vector2[2,2];
    private static Vector2[,] jumpAnimationOffsets = new Vector2[2,2];
    private static Vector2 unicornAlignmentOffset = new Vector2(-0.12f, 0.09f);

    public GameObject[] sparkles;
    public GameObject dontTouch;


    static Horse() {
        unicornBackgroundOffsets[BOY,WALK] = new Vector2(0.115f, -0.1f);
        unicornBackgroundOffsets[BOY,JUMP] = new Vector2(0.12f, -0.19f);
        unicornBackgroundOffsets[GIRL,WALK] = new Vector2(0.115f, -0.1f);
        unicornBackgroundOffsets[GIRL,JUMP] = new Vector2(0.18f, -0.16f);

        jumpAnimationOffsets[PONY,BOY] = new Vector2(-0.09f, 0.35f);
        jumpAnimationOffsets[PONY,GIRL] = new Vector2(-0.04f, 0.39f);
        jumpAnimationOffsets[UNICORN,GIRL] = new Vector2(-0.01f, 0.44f);
        jumpAnimationOffsets[UNICORN,BOY] = new Vector2(-0.085f, 0.44f);
    }

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        horseType = Random.Range(0, 2);
        horseGender = Random.Range(0, 2);

        
        float horseWidth = 0f, horseHeight = 0f;

        horseStyle = (HorseStyle)Random.Range(0, 4);

        switch (horseStyle)
        {
            case TEXTURED:
                color = Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 1f);
                //color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                tiledTexture = tiledTextures[Random.Range(0, tiledTextures.Length)];
                break;
            case COLORED:
                color = Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 1f);
                //color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                break;
            case GRAYSCALE:
                float gray = Random.Range(0.3f, 1f);
                color = new Color(gray, gray, gray, 1f);
                break;
        }


        float sizeFactor = Random.Range(0.11667f, 0.45f) * upperRight.x;

        ParticleSystem.MainModule ma = particles.main;
        ma.startColor = color;
        ma.startSize = sizeFactor * 0.65f;

        flipSign = Mathf.Sign(Random.Range(-1f, 1f));

        transform.localScale = new Vector3(flipSign * sizeFactor, sizeFactor, 1);



        for (int anim = 0; anim < 2; anim++)
        {
            int nFrames = (anim == JUMP) ? 30 : 25;
            for (int layer = 0; layer < 2; layer++)
            {
                for (int frame = 0; frame < nFrames; frame++)
                {
                    int type = horseType == UNICORN && layer == BG ? PONY : horseType;

                    GameObject obj = Instantiate(Common.prefabs[type,horseGender,anim,layer,frame], transform);

                    if (horseType == UNICORN)
                    {
                        obj.transform.position += Vector3.Scale(unicornAlignmentOffset, transform.localScale);

                        if (layer == BG)
                        {
                            obj.transform.position += Vector3.Scale(unicornBackgroundOffsets[horseGender, anim], transform.localScale);
                        }
                    }

                    if (anim == JUMP)
                    {
                        obj.transform.position += Vector3.Scale(jumpAnimationOffsets[horseType,horseGender], transform.localScale);
                    }

                    if (layer == FG)
                    {
                        obj.transform.position += new Vector3(0f, 0f, -0.001f);
                    }

                    obj.SetActive(anim == WALK && frame == 0);

                    SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                    //renderer.sortingOrder = (layer == BG ? order : order + 1);

                    switch (horseStyle)
                    {
                        case COLORED:
                            if (layer == BG)
                            {
                                renderer.color = color;
                            }
                            break;
                        case TEXTURED:
                            if (layer == BG)
                            {
                                renderer.material = tiledTextureMat;
                                renderer.material.color = color;
                                renderer.material.SetTexture("_CustomTex", tiledTexture);
                            }
                            break;
                        case RAINBOW:
                            if (layer == BG)
                            {
                                renderer.material = rainbowMat;
                            }
                            break;
                        case GRAYSCALE:
                            if (layer == BG)
                            {
                                renderer.color = color;
                            }
                            else {
                                renderer.material = grayscaleMat;
                            }
                            break;
                    }                    

                    if (anim == WALK && layer == FG && frame == 0)
                    {
                        horseWidth = renderer.bounds.size.x;
                        horseHeight = renderer.bounds.size.y;
                    }
                    //Debug.Log($"START {anim} {layer} {frame} {obj}");
                    objects[anim,layer,frame] = obj;
                }
            }
        }

        order += 2;

        SetHorseState(WALKING);
        

        speed = flipSign * sizeFactor * Random.Range(-4f, -0.75f) * Mathf.Log10(currentMission + 10f);
        //Debug.Log($"speed={speed}");

        horseSize *= flipSign * sizeFactor;

        float x = flipSign * (upperRight.x + horseWidth * 0.5f * sizeFactor);
        float y = Random.Range(lowerLeft.y * 0.8f + horseHeight * 0.5f * sizeFactor, upperRight.y * 0.8f - horseHeight * 0.5f * sizeFactor);
        float z = gameManager.GetHorseZ(this);

        transform.position = new Vector3(x, y, z);

        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (gameManager.gameState == INTRO || gameManager.gameState == PLAYING || gameManager.gameState == GAME_OVER || gameManager.gameState == GAME_WON)
        {
            UpdateObjects();

            currentFrame++;
        }

        if (gameManager.gameState == GAME_WON && horseState == HorseState.WALKING && Random.Range(0, 500) == 0)
        {
            SetHorseState(JUMPING);
        }
    }

    void Update()
    {
        /*for (int i = 0; i < sparkles.Length; i++)
        {
            if (gameManager.gameState == PLAYING && horseState == WALKING && gameManager.IsInvited(this))
            {
                sparkles[i].transform.localPosition = new Vector3(Mathf.Sin(currentFrame * 0.05f + i * 0.6f) * 0.7f - 0.7f, sparkles[i].transform.localPosition.y, Mathf.Cos(currentFrame * 0.05f + i * 0.6f));
                sparkles[i].transform.localRotation = Quaternion.Euler(0f, 0f, currentFrame * 2f);
            }
            else
            {
                sparkles[i].SetActive(false);
            }
        }

        dontTouch.SetActive(gameManager.gameState == PLAYING && !gameManager.IsInvited(this));*/

        if (gameManager.gameState != STALLED && gameManager.gameState != FADE_OUT)
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
        }

        if (gameManager.gameState == PLAYING)
        {
            if (horseSize < 0 && transform.position.x > upperRight.x || horseSize > 0 && transform.position.x < lowerLeft.x)
            {
                if (horseState == WALKING && gameManager.IsInvited(this))
                {
                    gameManager.OutOfScreenMistake(this);
                }
            }   
        }

        float margin = horseState == RUNNING ? horseSize * 4f : horseSize;

        if (horseSize < 0 && transform.position.x > upperRight.x - margin || horseSize > 0 && transform.position.x < lowerLeft.x - margin)
        {
            Destroy(gameObject);
        }
    }

    public void SetHorseState(HorseState newState)
    {
        switch (newState)
        {
            case WALKING:
                break;

            case JUMPING:
                audioSource.panStereo = transform.position.x / upperRight.x;
                audioSource.PlayOneShot(jumpSFX);
                break;

            case RUNNING:
                particles.Play(true);
                speed *= 8f;
                break;
        }

        horseState = newState;

        currentFrame = 0;
        UpdateObjects();
    }

    private void UpdateObjects()
    {
        activeBackground?.SetActive(false);
        activeForeground?.SetActive(false);

        int nFrames = horseState == JUMPING ? 30 : 25;
        int horseFrame = ((currentFrame * (int)Mathf.Sqrt(Mathf.Abs(speed * 10f))) / 5) % nFrames;

        int anim = horseState == JUMPING ? JUMP : WALK;
        activeBackground = objects[anim, BG, horseFrame];
        activeBackground?.SetActive(true);

        activeForeground = objects[anim, FG, horseFrame];
        activeForeground?.SetActive(true);

        if (horseState == JUMPING && horseFrame == 29)
        {
            SetHorseState(RUNNING);
        }
    }

    public void Blink()
    {
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 6; i++)
        {
            activeBackground?.SetActive(!activeBackground.activeSelf);
            activeForeground?.SetActive(!activeForeground.activeSelf);

            yield return new WaitForSeconds(0.25f);
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        SmoothAction fadeOut = (float t) => { SetAlpha(1f - t); };
        yield return SmoothCoroutine(fadeOut, 1f);

        gameManager.SetGameState(PLAYING);

        Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        SpriteRenderer bgRenderer = activeBackground.GetComponent<SpriteRenderer>();
        SpriteRenderer fgRenderer = activeForeground.GetComponent<SpriteRenderer>();

        bgRenderer.color = new Color(color.r, color.g, color.b, alpha);
        fgRenderer.color = new Color(1f, 1f, 1f, alpha);

        if (horseStyle == TEXTURED || horseStyle == RAINBOW)
        {
            bgRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
        }
        if (horseStyle == GRAYSCALE)
        {
            fgRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }
}
