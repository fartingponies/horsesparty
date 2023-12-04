using System.Collections;
using UnityEngine;
using TMPro;
using System;

using static Common;
using static GameManager;
using static Wheel.WheelState;
using Random = UnityEngine.Random;

public class Wheel : MonoBehaviour
{
    Rigidbody2D rb;

    float pieRadius;
    float startAngle;
    float currentAngle;
    float lastAngle;
    float[] lastSpeeds = new float[15];
    int physicsFrame;
    float last = 0f;

    public enum WheelState { READY, DRAGGING, SPINNING, DONE };
    WheelState wheelState;

    public TextMeshPro title;

    public GameObject[] emojis;

    private bool spinAgain;

    private Touch touch;

    private float startRotation;

    public TextMeshPro missionLabel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        title.text = GetString("spinthewheel");
        missionLabel.text = GetString("mission#") + (currentMission + 1);

        pieRadius = transform.Find("Background Circle").lossyScale.x * 0.5f;

        Random.InitState((int)DateTime.Now.Ticks);

        //wheelState = SPINNING;
        //rb.angularVelocity = 430f;
    }

    void SetWheelState(WheelState newState)
    {
        switch (newState)
        {
            case READY:
                startAngle = currentAngle = lastAngle = 0f;
                spinAgain = false;
                rb.freezeRotation = false;
                break;
            case DRAGGING:
                physicsFrame = 0;
                break;
            case SPINNING:
                rb.angularDrag = 0.5f;

                float speed = 0f;

                if (physicsFrame == 0)
                {
                    speed = (currentAngle - lastAngle) / Time.fixedDeltaTime;
                }
                else {
                    int nFrames = Math.Min(physicsFrame, 15);

                    for (int i = 0; i < nFrames; i++)
                    {
                        speed += lastSpeeds[i];
                    }

                    speed /= (float)nFrames;
                }

                rb.angularVelocity = speed; //Mathf.Sign(speed) * Mathf.Min(600f * Random.Range(0.9f, 1.1f), Mathf.Abs(speed));
                startRotation = rb.rotation;
                break;
            case DONE:
                string[] titles = { "unicornsparty", "girlsparty", "poniesparty", "boysparty" };
                StartCoroutine(BlinkTitle(titles[(int)gameType], true));
                break;
        }
        wheelState = newState;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
        }
        else {
            touch.position = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            touch.phase = TouchPhase.Began;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touch.phase = TouchPhase.Ended;
        }
        else if (Input.GetMouseButton(0))
        {
            touch.phase = TouchPhase.Moved;
        }

        if (Input.touchCount == 1 || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);

            bool touchOverWheel = Vector2.Distance(pos, rb.position) <= pieRadius;

            if (wheelState == READY && touchOverWheel && touch.phase == TouchPhase.Began)
            {
                startAngle = rb.rotation - GetTouchAngle(pos);

                SetWheelState(DRAGGING);
            }

            if (wheelState == DRAGGING)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    if (touchOverWheel)
                    {
                        rb.rotation = startAngle + GetTouchAngle(pos);
                    }
                    else
                    {
                        SetWheelState(SPINNING);
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    SetWheelState(SPINNING);
                }
            }
        }

        if (wheelState == SPINNING && Mathf.Approximately(rb.angularVelocity / 10f, 0f))
        {
            rb.freezeRotation = true;

            if (rb.rotation > 1.5f * 360f + startRotation || rb.rotation < 1.5f * -360f + startRotation)
            {
                if (spinAgain)
                {
                    StartCoroutine(BlinkTitle("spinagain", true));
                    SetWheelState(READY);
                }
                else if (wheelState != DONE)
                {
                    SetWheelState(DONE);
                }
            }
            else {
                StartCoroutine(BlinkTitle("spinfaster", false));
                SetWheelState(READY);
            }
        }
    }

    private void FixedUpdate()
    {
        lastAngle = currentAngle;
        currentAngle = rb.rotation;

        //Debug.Log("rb.rotation=" + rb.rotation + " angle " + currentAngle + " lastSpeeds[" + (physicsFrame % 15) + "] = " + (currentAngle - lastAngle) / Time.fixedDeltaTime + " Time.fixedDeltaTime=" + Time.fixedDeltaTime);
        lastSpeeds[physicsFrame++ % 15] = (currentAngle - lastAngle) / Time.fixedDeltaTime;

        rb.angularDrag *= 1.002f;
    }

    public void LastTrigger(int i)
    {
        gameType = (GameType)i;

        spinAgain = i == 4;
    }

    private IEnumerator BlinkTitle(string key, bool blinkEmoji)
    {
        title.text = GetString(key);
        GameObject emoji = emojis[(int)gameType];

        for (int i = 0; i < 12; i++)
        {
            if (blinkEmoji)
            {
                emoji.SetActive(!emoji.activeSelf);
            }
            title.gameObject.SetActive(!title.gameObject.activeSelf);
            yield return new WaitForSeconds(0.3f);
        }

        if (wheelState == DONE)
        {
            //gameType = GameType.UNICORNS_GAME;
            bool tutorialDone = PlayerPrefs.GetInt("TutorialDone" + gameType, 0) == 1;
            FadeToScene(tutorialDone ? "GameScene" : "TutorialScene");
        }
    }

    private float GetTouchAngle(Vector2 pos) {
        float angle = Mathf.Atan2(pos.y - rb.position.y, pos.x - rb.position.x);

        //Lifting
        while (angle < last - Mathf.PI) angle += 2f * Mathf.PI;
        while (angle > last + Mathf.PI) angle -= 2f * Mathf.PI;

        last = angle;

        return angle * Mathf.Rad2Deg;
    }
}
