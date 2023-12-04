using System.Collections;
using UnityEngine;
using TMPro;

using static Common;
using static GameManager;

public class HorseHead : MonoBehaviour
{
    public GameObject panel;
    private GameObject bg, fg;

    const float exampleScale = 1.25f;

    public TextMeshPro label;
    public GameObject rightMark, wrongMark;

    int horseType, horseGender;

    AudioSource audioSource;
    public AudioClip rightSFX, wrongSFX, winSFX;

    public AnimationCurve zoomCurve;
    float zoomTime;
    bool zoomed;
    float initialScale;
    int initialSortingLayer;

    static int numChecked;
    static bool playerFailedRound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        initialScale = transform.localScale.x;

        numChecked = 0;

        zoomTime = Time.time - 10f;
    }

    void Update()
    {
        float elapsedTime = Mathf.Min(1.0f, Time.time - zoomTime);
        float zoom = zoomCurve.Evaluate(zoomed ? elapsedTime : 1f - elapsedTime) * 1.5f + 1f;
        transform.localScale = new Vector3(zoom * initialScale, zoom * initialScale, 1.0f);


        if (Input.GetMouseButtonDown(0) || Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (zoomed)
            {
                zoomed = false;
                zoomTime = Time.time;
                StartCoroutine(RestoreSortingLayer());
                return;
            }

            Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetMouseButtonDown(0) ? Input.mousePosition : Input.GetTouch(0).position);
            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld, Camera.main.transform.forward);

            if (hitInformation.collider != null)
            {
                if (gameObject == hitInformation.transform.gameObject && initialSortingLayer > 1 && !rightMark.activeSelf)
                {
                    TutorialChallenge challenge = GameObject.FindGameObjectWithTag("Challenge").GetComponent<TutorialChallenge>();

                    if (challenge.mission == 0)
                    {
                        if (challenge.invitedType == horseType)
                            Right();
                        else
                            Wrong();
                    }
                    else
                    { //mission == 1
                        if (challenge.invitedGender == horseGender)
                            Right();
                        else
                            Wrong();
                    }
                }
            }
        }
    }

    const float dx = 0.225f;
    const float dy = -0.225f;

    public void SetCharacter(int sortingLayer, int type, int gender)
    {
        if (sortingLayer < 2)
        {
            string genderString = GetString(gender == BOY ? "boy" : "girl");
            string typeString = GetString(type == PONY ? "pony" : "unicorn");

            label.SetText(genderString + '\n' + typeString);
        }
        else {
            string genderString = GetString(gender == BOY ? "boy" : "girl").ToUpper();
            string typeString = GetString(type == PONY ? "pony" : "unicorn").ToUpper();

            TutorialChallenge challenge = GameObject.FindGameObjectWithTag("Challenge").GetComponent<TutorialChallenge>();
            label.SetText(challenge.mission == 0 ? typeString : genderString);
        }

        horseType = type;
        horseGender = gender;
        
        initialSortingLayer = sortingLayer;

        bg = Instantiate(prefabs[PONY, horseGender, WALK, BG, 0], transform);
        SpriteRenderer bgSpriteRenderer = bg.GetComponent<SpriteRenderer>();
        bgSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        if (sortingLayer < 2)
        {
            bgSpriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);
            rightMark.transform.localScale = new Vector3(exampleScale, exampleScale, 1.0f);
            rightMark.SetActive(true);
            label.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
        }
        else
        {
            bgSpriteRenderer.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            label.enabled = false;
            label.transform.Translate(new Vector3(0f, 0.25f));
            label.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
            label.sortingOrder = 500;
        }
        bgSpriteRenderer.sortingOrder = 15;
        bg.transform.Translate(new Vector3(dx, dy));
        if (horseType == UNICORN)
        {
            bg.transform.position += Vector3.Scale(unicornBackgroundOffsets[horseGender, WALK], transform.lossyScale);
        }

        fg = Instantiate(prefabs[horseType, horseGender, WALK, FG, 0], transform);
        fg.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        fg.GetComponent<SpriteRenderer>().sortingOrder = 16;
        fg.transform.Translate(new Vector3(dx, dy));

        SpriteMask maskObject = gameObject.GetComponentInChildren<SpriteMask>();
        maskObject.frontSortingOrder = 10000;
        maskObject.backSortingOrder = 0;
        maskObject.transform.Translate(new Vector3(dx, dy));

        SetSortingLayer(initialSortingLayer);
    }

    private void Zoom()
    {
        zoomed = true;
        zoomTime = Time.time;

        SetSortingLayer(-1);
        label.enabled = true;
    }

    private void SetSortingLayer(int sortingLayer)
    {
        int layerID = (sortingLayer == -1 ? SortingLayer.NameToID("Top") : SortingLayer.NameToID("Pony" + sortingLayer));

        bg.GetComponent<Renderer>().sortingLayerID = layerID;
        fg.GetComponent<Renderer>().sortingLayerID = layerID;
        panel.GetComponent<Renderer>().sortingLayerID = layerID;

        SpriteMask maskObject = gameObject.GetComponentInChildren<SpriteMask>();
        maskObject.sortingLayerID = layerID;
        maskObject.frontSortingLayerID = layerID;
        maskObject.backSortingLayerID = layerID;

        
    }

    private void Right() {
        rightMark.SetActive(true);
        audioSource.PlayOneShot(rightSFX);
        StartCoroutine(BlinkGameObject(rightMark, 4, 0.25f, true));

        if (++numChecked >= 8)
        {
            PlayerPrefs.SetInt("TutorialDone" + gameType, 1);

            audioSource.PlayOneShot(winSFX);

            StartCoroutine(NextScene(4f));
        }
    }

    private void Wrong() {
        //playerFailedRound = true;
        wrongMark.SetActive(true);
        StartCoroutine(BlinkGameObject(wrongMark, 4, 0.25f, true));
        audioSource.PlayOneShot(wrongSFX);
        Zoom();
    }

    public IEnumerator RestoreSortingLayer()
    {
        yield return new WaitForSeconds(1.0f);

        SetSortingLayer(initialSortingLayer);

        label.enabled = false;

        wrongMark.SetActive(false);
    }

    public IEnumerator BlinkGameObject(GameObject gameObject, int numBlinks, float seconds, bool isActiveAfter)
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        for (int i = 0; i < numBlinks * 2; i++)
        {
            gameObject.SetActive(!gameObject.activeSelf);
            yield return new WaitForSeconds(seconds);
        }
        gameObject.SetActive(isActiveAfter);
    }

    public IEnumerator NextScene(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        FadeToScene( "GameScene");
    }

}