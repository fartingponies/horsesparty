using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Common;

public class Cloud : MonoBehaviour
{
    private new SpriteRenderer renderer;

    void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(Random.Range(lowerLeft.x, upperRight.x), Random.Range(lowerLeft.y, upperRight.y));

        SetRandomScale();
    }

    void Update()
    {
        float spriteWidth = renderer.bounds.size.x;

        float dx = -spriteWidth * 0.15f * Time.deltaTime;

        transform.position += new Vector3(dx, 0f);

        if (transform.position.x + spriteWidth * 0.5f < lowerLeft.x)
        {
            SetRandomScale();

            float newSpriteWidth = renderer.bounds.size.x;
            transform.position = new Vector3(upperRight.x * 1.2f + spriteWidth * 0.6f, Random.Range(lowerLeft.y, upperRight.y));
        }
    }

    private void SetRandomScale() {
        float scale = Random.Range(0.25f, 0.8f) * upperRight.x * 0.4f;
        bool flip = Random.Range(0, 2) == 0 ? true : false;
        transform.localScale = new Vector3(flip ? -scale : scale, scale, 1f);
    }
}
