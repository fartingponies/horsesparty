using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using static Common;

public class ScrollingLandscape : MonoBehaviour
{
    public int tileOffset;

    private float theta;
    private float backgroundSpeed = 0.1f;
    private float backgroundWidth;
    public float amplitude;

    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer.sortingOrder == 1000)
        {
            backgroundWidth = spriteRenderer.bounds.size.x;
        }
        else
        {
            LandscapeScale parent = transform.parent.GetComponent<LandscapeScale>();
            backgroundWidth = parent.backgroundWidth;
        }

        theta = 0f;
    }

    void FixedUpdate()
    {
        theta += backgroundSpeed * Time.fixedDeltaTime;

        float xOffset = tileOffset * backgroundWidth * 0.85f;
        transform.position = transform.parent.parent.position + new Vector3(Mathf.Sin(theta) * (upperRight.x - backgroundWidth / 2f) * amplitude + xOffset, transform.position.y);

        if (audioSource != null)
        {
            audioSource.panStereo = Mathf.Sin(theta) * amplitude;
        }
    }
}
