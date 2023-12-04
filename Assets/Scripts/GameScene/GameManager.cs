using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

using static Common;
using static SceneFader;
using static GameManager.GameState;
using static GameManager.GameType;
using static Horse.HorseState;


public class GameManager : MonoBehaviour
{
    public static int currentMission;

    public const int
        PONY = 0, UNICORN = 1,
        BOY = 0, GIRL = 1,
        WALK = 0, JUMP = 1,
        BG = 0, FG = 1;

    public enum GameType { UNICORNS_GAME, GIRLS_GAME, PONIES_GAME, BOYS_GAME }
    public static GameType gameType = PONIES_GAME;

    public enum GameState { NULL, INTRO, POPUP, PLAYING, STALLED, FADE_OUT, GAME_OVER, GAME_WON }
    public GameState gameState;


    public Button playButton;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI gameStatusText;

    private static bool shownIntro = false;

    public GameObject progressBar;

    private int hitCount;

    public TextMeshPro missionLabelText;

    public const int maxMistakes = 4;
    public GameObject[] mistakesLarge = new GameObject[maxMistakes];
    public GameObject[] mistakesSmall = new GameObject[maxMistakes];
    public int numMistakes;

    public Material colorVectorMaterial;
    public Material grayscaleVectorMaterial;

    public AudioSource clickSFX, wrongSFX, gameOverSFX, explosionSFX, swipeSFX;

    private float lastGuestZ, lastNonGuestZ;

    public GameObject fireworks;

    public GameObject magnifier;
    private Vector3 magnifierOrigin;
    private Vector3 magnifierLocalScale;
    private Vector3 magnifierLenseLocalScale;

    public GameObject leftLimit, rightLimit;

    public GameObject scrollingStuff;

    public GameObject missedLabel;
    private string missedLabelText;

    public GameObject popupPanel;
    public TextMeshProUGUI popupLabel;

    public CanvasGroup canvasGroup;

    private int hitsTarget;

    

    public GameObject settingsButtonPanel;


    async private void Start()
    {
        await LocalizationSettings.InitializationOperation.Task;

        leftLimit.transform.position = new Vector2(lowerLeft.x - leftLimit.transform.localScale.x * 0.5f, 0f);
        rightLimit.transform.position = new Vector2(upperRight.x + rightLimit.transform.localScale.x * 0.5f, 0f);
        
        Random.InitState(currentMission + 1);

        string[] hitThe = { "taptheunicorns", "tapthegirls", "taptheponies", "taptheboys" };
        missionLabelText.text = GetString(hitThe[(int)gameType]);

        hitCount = 0;

        numMistakes = 0;

        
        lastGuestZ = -500f;
        lastNonGuestZ = -100f;

        magnifier.transform.localScale *= upperRight.x * 0.4f;
        magnifierLocalScale = magnifier.transform.localScale;
        magnifierLenseLocalScale = magnifier.transform.Find("Sphere").localScale;

        SetProgress(0f);

        currentMission = PlayerPrefs.GetInt("currentMission", 0);
        hitsTarget = 15 + 3 * currentMission;

        SetGameState(shownIntro ? POPUP : INTRO);
    }


    public void GoButtonClicked()
    {
        clickSFX.Play();

        Vector3 popupPanelScale = popupPanel.transform.localScale;
        SmoothAction scalePopupPanel = (float t) => { popupPanel.transform.localScale = Vector3.Scale(popupPanelScale, new Vector3(1f - t, 1f - t, 1f)); };
        StartCoroutine(SmoothCoroutine(scalePopupPanel, 1f));

        SetGameState(PLAYING);
    }

    public void PlayButtonClicked()
    {
        clickSFX.Play();

        switch (gameState)
        {
            case INTRO:
                shownIntro = true;
                SmoothAction fadeOutCanvas = (float t) => { canvasGroup.alpha = 1f - t; };
                StartCoroutine(SmoothCoroutine(fadeOutCanvas, 1f));
                FadeToScene("WheelScene");
                break;
            case GAME_WON:
                FadeToScene( "WheelScene");
                break;
            case GAME_OVER:
                FadeToScene( "GameScene");
                break;
        }
    }

    public void SetGameState(GameState newState)
    {
        switch (newState)
        {
            case INTRO:
                popupPanel.SetActive(false);
                missionLabelText.gameObject.SetActive(false);
                progressBar.transform.parent.parent.gameObject.SetActive(false);
                for (int i = 0; i < 4; i++) { mistakesSmall[i].SetActive(false); }
                playButton.gameObject.SetActive(true);
                settingsButtonPanel.SetActive(true);
                buttonText.text = GetString("play");
                gameStatusText.gameObject.SetActive(false);
                break;
            case POPUP:
                popupPanel.SetActive(true);
                string[] toGather = { "unicorns", "girls", "ponies", "boys" };
                popupLabel.text = GetString("mission#") + (currentMission + 1) + "\n" + GetString("gather") + " " + hitsTarget + " " + GetString(toGather[(int)gameType]).ToLower();
                playButton.gameObject.SetActive(false);
                settingsButtonPanel.SetActive(false);
                Vector3 popupPanelScale = popupPanel.transform.localScale;
                SmoothAction scalePopupPanel = (float t) => { popupPanel.transform.localScale = Vector3.Scale(popupPanelScale, new Vector3(t, t, 1.0f)); };
                StartCoroutine(SmoothCoroutine(scalePopupPanel, 1f));
                gameStatusText.gameObject.SetActive(false);
                break;
            case PLAYING:
                missionLabelText.gameObject.SetActive(true);
                progressBar.transform.parent.parent.gameObject.SetActive(true);
                for (int i = 0; i < 4; i++) { mistakesSmall[i].SetActive(true); }
                playButton.gameObject.SetActive(false);
                gameStatusText.gameObject.SetActive(false);
                break;
            case GAME_WON:
                playButton.gameObject.SetActive(true);
                buttonText.text = GetString("nextmission");
                gameStatusText.gameObject.SetActive(true);
                gameStatusText.text = GetString("missioncomplete");

                PlayerPrefs.SetInt("currentMission", ++currentMission);
                PlayerPrefs.Save();
                break;
            case GAME_OVER:
                playButton.gameObject.SetActive(true);
                buttonText.text = GetString("tryagain");
                gameStatusText.gameObject.SetActive(true);
                gameStatusText.text = GetString("gameover");
                break;
            case FADE_OUT:
                Horse[] horses = GameObject.FindObjectsOfType<Horse>();
                foreach (Horse horse in horses)
                {
                    horse.FadeOut();
                }
                break;
        }
        
        gameState = newState;
    }

    void Update()
    {
        if (gameState == PLAYING)
        {
            Vector3 touchPosWorld = Vector3.zero;

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            if (Input.GetMouseButtonDown(0))
                touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (touchPosWorld != Vector3.zero)
            {
                RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld, Camera.main.transform.forward);

                if (hitInformation.collider != null)
                {
                    GameObject obj = hitInformation.transform.gameObject;

                    if (obj.CompareTag("Pony"))
                    {
                        Horse horse = obj.GetComponent<Horse>();

                        //horse.SetHorseState(Horse.HorseState.JUMPING);

                        if (horse.horseState == WALKING)
                        {
                            HitHorse(horse);
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (gameState == GAME_WON)
        {
            if (Random.Range(0, 15) == 0)
            {
                explosionSFX.Play();

                GameObject newFireworks = Instantiate(fireworks);
                newFireworks.transform.position = new Vector3(Random.Range(lowerLeft.x, upperRight.x), Random.Range(0f, upperRight.y), -100f);

                ParticleSystem particles = newFireworks.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = particles.main;
                particles.transform.localScale = new Vector3(upperRight.x * 0.2f, upperRight.x * 0.2f, 1f);
                main.startColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                main.startSize = Random.Range(0.1f, 0.2f);
                main.startSpeed = Random.Range(5f, 10f);

                particles.emission.SetBurst(0, new ParticleSystem.Burst(0f, new ParticleSystem.MinMaxCurve(Random.Range(100, 250))));

                Destroy(newFireworks, main.duration);
            }
        }
    }

    public bool IsInvited(Horse horse)
    {
        switch (gameType)
        {
            case UNICORNS_GAME:
                return horse.horseType == UNICORN;
            case GIRLS_GAME:
                return horse.horseGender == GIRL;
            case PONIES_GAME:
                return horse.horseType == PONY;
            case BOYS_GAME:
                return horse.horseGender == BOY;
        }

        return false;
    }

    public float GetHorseZ(Horse horse)
    {
        lastGuestZ -= 0.002f;
        lastNonGuestZ -= 0.002f;

        return IsInvited(horse) ? lastGuestZ : lastNonGuestZ;
    }

    private void SetProgress(float progress)
    {
        progressBar.transform.localScale = new Vector3(0.985f * progress, 0.75f, 1.0f);
        progressBar.transform.localPosition = new Vector3(0.985f * (1f - progress) * -0.5f, 0f);
    }

    public void HitHorse(Horse horse)
    {
        if (IsInvited(horse))
        {
            horse.SetHorseState(JUMPING);

            if (++hitCount == hitsTarget)
            {
                SetGameState(GameState.GAME_WON);
            }

            SetProgress((float)hitCount / (float)hitsTarget);

            horse.GetComponent<PolygonCollider2D>().enabled = false;
        }
        else
        {
            SetGameState(STALLED);

            string[] uninvitedLabels = { "pony", "boy", "unicorn", "girl" };
            missedLabelText = GetString(uninvitedLabels[(int)gameType]) + "\n" + GetString((int)gameType < 2 ? "uninvitedM" : "uninvitedF");

            StartCoroutine(ShowOutOfScreen(horse));
        }
    }

    public void OutOfScreenMistake(Horse horse)
    {
        SetGameState(STALLED);

        string[] missedLabels = { "unicorn", "girl", "pony", "boy" };
        missedLabelText = GetString(missedLabels[(int)gameType]) + "\n" + GetString((int)gameType < 2 ? "missedF" : "missedM");

        StartCoroutine(ShowOutOfScreen(horse));
    }

    public void Mistake(Horse horse)
    {
        SetGameState(STALLED);

        Vector3 scale = new Vector3(IsInvited(horse) ? horse.flipSign : -horse.flipSign, 1f, 1f);
        magnifier.transform.localScale = Vector3.Scale(magnifierLocalScale, scale);
        magnifier.transform.Find("Sphere").localScale = Vector3.Scale(magnifierLenseLocalScale, scale);

        Transform headMarker = horse.transform.Find("HeadMarker");

        float magnifierX = Mathf.Sign(headMarker.position.x) * lowerLeft.x * 1.5f;
        float magnifierY = Mathf.Sign(headMarker.position.y) * lowerLeft.y * 1.5f;

        magnifier.transform.position = magnifierOrigin = new Vector3(magnifierX, magnifierY, magnifier.transform.position.z);

        
        SendMagnifier(headMarker.position);

        for (int i = 0; i < maxMistakes; i++)
        {
            Vector3 pos = mistakesLarge[i].transform.position;
            pos.y = headMarker.position.y > 0f ? lowerLeft.y * 0.5f : upperRight.y * 0.5f;
            mistakesLarge[i].transform.position = pos;
        }

        if (numMistakes < maxMistakes - 1)
        {
            wrongSFX.Play();

            horse.Blink();

            StartCoroutine(BlinkMistake(numMistakes++));
        }
        else {
            gameOverSFX.Play();

            horse.Blink();

            for (int i = 0; i < maxMistakes; i++)
            {
                StartCoroutine(BlinkMistake(i));
            }
        }
    }

    private IEnumerator BlinkMistake(int mistakeIndex)
    {
        for (int i = 0; i < maxMistakes; i++)
        {
            mistakesLarge[i].SetActive(true);
        }

        SpriteRenderer largeMistakeRenderer = mistakesLarge[mistakeIndex].GetComponent<SpriteRenderer>();
        SpriteRenderer smallMistakeRenderer = mistakesSmall[mistakeIndex].GetComponent<SpriteRenderer>();

        for (int i = 0; i < 6; i++)
        {
            Material newMaterial = i % 2 == 0 ? grayscaleVectorMaterial : colorVectorMaterial;
            largeMistakeRenderer.material = smallMistakeRenderer.material = newMaterial;

            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < maxMistakes; i++)
        {
            mistakesLarge[i].SetActive(false);
        }

        if (mistakeIndex == maxMistakes - 1)
        {
            //SetGameState(GAME_OVER);
        }
    }

    private void SendMagnifier(Vector2 target)
    {
        StartCoroutine(MoveMagnifier(target));
    }

    private IEnumerator MoveMagnifier(Vector3 head)
    {
        yield return new WaitForSeconds(1.25f);

        Vector3 origin = magnifier.transform.position;
        Vector3 lenseOffset = Vector3.Scale(new Vector2(1.05f, -1.23f), magnifier.transform.localScale);
        Vector3 target = head + lenseOffset;
        target.z = magnifier.transform.position.z;

        SmoothAction moveIn = (float t) => { magnifier.transform.position = Vector3.Lerp(origin, target, t); };
        yield return SmoothCoroutine(moveIn, 0.75f);

        yield return new WaitForSeconds(1f);

        TextMeshPro label = Instantiate(missedLabel).GetComponent<TextMeshPro>();
        label.transform.localScale = new Vector3(upperRight.x * 0.4f * 0.75f, upperRight.x * 0.3f * 0.75f, 1f);
        if (head.y > upperRight.y * 0.5f)
            label.transform.position = head - new Vector3(0f, upperRight.x * 0.6f);
        else
            label.transform.position = head + new Vector3(0f, upperRight.x * 0.6f);
        label.text = missedLabelText;

        yield return new WaitForSeconds(1f);

        SmoothAction moveOut = (float t) => { magnifier.transform.position = Vector3.Lerp(target, magnifierOrigin, t); label.alpha = 1f - t; };
        yield return SmoothCoroutine(moveOut, 0.75f);

        Destroy(label);

        //SetGameState(numMistakes == 4 ? GAME_OVER : FADE_OUT);
    }

    private IEnumerator ShowOutOfScreen(Horse horse)
    {
        horse.transform.position = new Vector3(horse.transform.position.x, horse.transform.position.y, -600f);

        Transform headMarker = horse.transform.Find("HeadMarker");

        Vector3 origin = scrollingStuff.transform.position;
        Vector3 target = origin;
        target.x = horse.flipSign * (lowerLeft.x + 1.85f * magnifierLocalScale.x) - headMarker.transform.position.x;

        if (horse.flipSign * target.x > 0f)
        {
            SmoothAction scrollOut = (float t) => { scrollingStuff.transform.position = Vector3.Lerp(origin, target, t); };
            yield return SmoothCoroutine(scrollOut, 0.75f);
        }

        Mistake(horse);

        yield return new WaitForSeconds(4.5f);

        if (horse.flipSign * target.x > 0f)
        {
            SmoothAction scrollIn = (float t) => { scrollingStuff.transform.position = Vector3.Lerp(target, origin, t); };
            yield return SmoothCoroutine(scrollIn, 0.75f);
        }

        SetGameState(numMistakes == maxMistakes - 1 ? GAME_OVER : FADE_OUT);
    }
}