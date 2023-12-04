using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelStick : MonoBehaviour
{
    Rigidbody2D rb;
    public float startAngle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float theta = (startAngle + 6f + transform.parent.GetComponent<Rigidbody2D>().rotation) * Mathf.Deg2Rad;

        rb.MovePosition(
            transform.parent.position
            + Vector3.Scale(transform.parent.lossyScale * 2.45f,
                            new Vector2(Mathf.Cos(theta), Mathf.Sin(theta))));
    }
}
