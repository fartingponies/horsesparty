using UnityEngine;
using TMPro;
using UnityEngine.UI;

using static Common;
using static GameManager;
using static GameManager.GameType;
using static SceneFader;

public class TutorialChallenge: MonoBehaviour
{    
    public GameObject ponyHead;

    public GameObject[,] buttons = new GameObject[4,4];

    public int mission, invitedType, invitedGender;

    public GameObject title;

    public Button skipButton;


    void Start()
    {
        int leftType = 0, rightType = 0, leftGender = 0, rightGender = 0;

        switch (gameType) {
            case UNICORNS_GAME:
                mission = 0;
                invitedType = leftType = rightType = UNICORN;
                break;
            case GIRLS_GAME:
                mission = 1;
                invitedGender = leftGender = rightGender = GIRL;
                break;
            case PONIES_GAME:
                mission = 0;
                invitedType = leftType = rightType = PONY;
                break;
            case BOYS_GAME:
                mission = 1;
                invitedGender = leftGender = rightGender = BOY;
                break;
        }

        if (mission == 0)
        {
            string line2 = GetString("checkthe") + GetString(leftType == 0 ? "ponies" : "unicorns");

            title.GetComponent<TextMeshPro>().SetText(line2);
            
            leftGender = GIRL;
            rightGender = BOY;
        }
        else
        {
            string line2 = GetString("checkthe") + GetString(rightGender == 0 ? "boys" : "girls");

            title.GetComponent<TextMeshPro>().SetText(line2);

            leftType = PONY;
            rightType = UNICORN;
        }

        int sortingLayer = 0;
        
        float dx = 0.24f * transform.localScale.x;
        float dy = 0.24f * transform.localScale.y;


        GameObject invitedLeft = Instantiate(ponyHead, transform);
        invitedLeft.transform.position = new Vector3(-dx, dy * 2f) + new Vector3(0.2f, -0.15f);
        invitedLeft.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        invitedLeft.GetComponent<HorseHead>().SetCharacter(sortingLayer++, leftType, leftGender);
        GameObject buttonLeft = GameObject.FindGameObjectWithTag("HorseHeadButton");
        buttonLeft.SetActive(false);
        
        GameObject invitedRight = Instantiate(ponyHead, transform);
        invitedRight.transform.position = new Vector3(dx, dy * 2f) + new Vector3(0.2f, -0.15f);
        invitedRight.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        invitedRight.GetComponent<HorseHead>().SetCharacter(sortingLayer++, rightType, rightGender);
        GameObject buttonRight = GameObject.FindGameObjectWithTag("HorseHeadButton");
        buttonRight.SetActive(false);
        

        int[] types = { PONY, PONY, PONY, PONY, PONY, PONY, PONY, PONY, UNICORN, UNICORN, UNICORN, UNICORN, UNICORN, UNICORN, UNICORN, UNICORN };
        int[] genders = { BOY, BOY, BOY, BOY, BOY, BOY, BOY, BOY, GIRL, GIRL, GIRL, GIRL, GIRL, GIRL, GIRL, GIRL };

        Shuffle(types);
        Shuffle(genders);


        for (int col = 0; col < 4; col++)
        {
            for (int row = 0; row < 4; row++)
            {
                GameObject button = Instantiate(ponyHead, transform);
                button.transform.position = transform.position + new Vector3(dx * (col - 1.5f), dy * (row - 1.5f));
                button.transform.localScale = new Vector3(0.11f, 0.11f, 1f);
                button.GetComponent<HorseHead>().SetCharacter(sortingLayer++, types[col * 4 + row], genders[col * 4 + row]);
                buttons[col, row] = button;
            }
        }
    }

    void Shuffle(int[] array)
    {
        // Knuth shuffle algorithm
        for (int t = 0; t < array.Length; t++)
        {
            int tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }
    }

    public void SkipClicked()
    {
        FadeToScene("GameScene");
    }
}
