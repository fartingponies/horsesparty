using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Common;

public class LandscapeScale : MonoBehaviour
{
    public float backgroundWidth;

    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        float scale = (upperRight.y - lowerLeft.y) / rectTransform.rect.height;
        transform.localScale = new Vector3(scale, scale, 1f);
        backgroundWidth = rectTransform.rect.width;
    }
}
