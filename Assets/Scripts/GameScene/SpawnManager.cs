using System.Collections;
using UnityEngine;

using static GameManager;
using static GameManager.GameState;

public class SpawnManager : MonoBehaviour
{
    public GameObject ponyPrefab;

    public GameObject camelPrefab;
    public GameObject copterPrefab;
    public GameObject fairyPrefab;
    public GameObject hedgehogPrefab;
    public GameObject hedgehogJumpingPrefab;
    public GameObject kangarooPrefab;
    public GameObject manBikingPrefab;
    public GameObject manRunningPrefab;
    public GameObject sauropodPrefab;
    public GameObject snailPrefab;
    public GameObject waspPrefab;

    public int maxRandomRange;

    private float z;

    private GameManager gameManager;

    public GameObject scrollingStuff;

    private bool pendingSpawn;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        if (gameManager.gameState == PLAYING)
        {
            if (!pendingSpawn)
            {
                int numHorses = GameObject.FindObjectsByType<Horse>(FindObjectsSortMode.None).Length;

                if (numHorses == 0)
                {
                    pendingSpawn = true;
                    StartCoroutine(SpawnWithDelay());
                }
                else
                {
                    int difficulty = (int)Mathf.Round(30f * (Mathf.Log10(currentMission + 10f) - 1f));

                    if (numHorses == 0 || Random.Range(0, 110 - difficulty) == 0)
                    {
                        Instantiate(ponyPrefab, scrollingStuff.transform);
                        z += 0.1f;
                    }
                }
            }

            if (Random.Range(0, maxRandomRange) == 0) Instantiate(camelPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(copterPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(fairyPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(hedgehogPrefab, scrollingStuff.transform);
            //if (Random.Range(0, maxRandomRange) == 0) Instantiate(hedgehogJumpingPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(kangarooPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(manBikingPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(manRunningPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(sauropodPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(snailPrefab, scrollingStuff.transform);
            if (Random.Range(0, maxRandomRange) == 0) Instantiate(waspPrefab, scrollingStuff.transform);
        }

        if (gameManager.gameState == INTRO || gameManager.gameState == GAME_WON || gameManager.gameState == GAME_OVER)
        {
            if (Random.Range(0, 10) == 0)
            {
                Instantiate(ponyPrefab, scrollingStuff.transform);
            }
        }
    }

    private IEnumerator SpawnWithDelay() {
        yield return new WaitForSeconds(1f);

        Instantiate(ponyPrefab, scrollingStuff.transform);

        pendingSpawn = false;
    }
}