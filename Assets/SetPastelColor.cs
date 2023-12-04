/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetPastelColor : MonoBehaviour
{
    public float hue = 60f;

    void Start()
    {
        if (GetComponent<TextMeshPro>() != null)
            GetComponent<TextMeshPro>().color = GetPastelColor(hue);

        if (GetComponent<TextMeshProUGUI>() != null)
            GetComponent<TextMeshProUGUI>().color = GetPastelColor(hue);
    }

    Color GetPastelColor(float hue)
    {
        const float Sl = 1f;
        const float L = 0.72f;

        float H = hue / 360f;

        float V = L + Sl * Mathf.Min(L, 1f - L);

        float S = (V == 0f) ? 0f : 2f * (1f - L / V);

        Color res = Color.HSVToRGB(H, S, V);
        res.a = 0.9f;

        return res;
    }
}
*/