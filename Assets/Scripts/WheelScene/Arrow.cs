using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject wheelObject;
    private Wheel wheel;

    AudioSource tickSFX;

    void Start()
    {
        wheel = wheelObject.GetComponent<Wheel>();

        tickSFX = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("WheelStick"))
        {
            tickSFX.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("PieCut"))
        {
            //Debug.Log("PieCut " + collider.gameObject.name.Substring(3));

            wheel.LastTrigger(collider.gameObject.name.Substring(3)[0] - '0');
        }
    }
}
