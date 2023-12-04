using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Common;

public class HorizontalDeplacementSprite : MonoBehaviour
{
    public float speed;
    public float animationSpeed;

    private bool flipped;

    private Vector3 size;

    private static int sortingOrder = -10000;

    void Start()
    {
        size = GetComponent<SpriteRenderer>().sprite.bounds.size;
        GetComponent<SpriteRenderer>().sortingOrder = sortingOrder++;

        flipped = Random.Range(0, 2) == 0;


        float y = Random.Range(lowerLeft.y * 0.9f + size.y * 0.5f, upperRight.y * 0.9f - size.y * 0.5f);

        if (speed == 0f)
        {
            transform.position = new Vector2(0f, lowerLeft.y + size.y * 0.5f);
        }
        else if (flipped) {
            transform.position = new Vector2(lowerLeft.x - size.x * 0.5f, y);
            transform.localScale = new Vector2(-1f, 1f);
            speed *= -1;
        }
        else {
            transform.position = new Vector2(upperRight.x + size.x * 0.5f, y);
        }

        GetComponent<Animator>().speed = animationSpeed;
    }

    void Update()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0f);

        if (flipped == false && transform.position.x < lowerLeft.x - size.x * 0.5f)
        {
            Destroy(gameObject);
        }

        if (flipped == true && transform.position.x > upperRight.x + size.x * 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
