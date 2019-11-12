using UnityEngine;

public class StopGoButton : MonoBehaviour {
    public bool pressed;
    public StopGoButton otherButton;

    public Stoplight leftStoplight;
    public Stoplight rightStoplight;

    // Maybe the buttons could be on some kind of timer in addition to hitting them?

    // Start is called before the first frame update
    void Start() {
        if(pressed) {
            // Turn on stoplights
            leftStoplight.Go();
            rightStoplight.Go();
        } else {
            // Turn off stoplights
            leftStoplight.Stop();
            rightStoplight.Stop();
        }
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // If we're hit by any attack object
        if (collision.gameObject.layer == 12) {
            // If we are not already pressed
            if (!pressed) {
                // Press the button
                Press();
            }
        }
    }

    public void Press() {
        // Move button inwards
        float xDelta = 0.3f * -Mathf.Sign(transform.position.x);
        transform.Translate(xDelta, 0f, 0f);
        pressed = true;
        // Turn on stoplights
        leftStoplight.Go();
        rightStoplight.Go();


        // Push opposite button outwards
        xDelta = 0.3f * Mathf.Sign(otherButton.transform.position.x);
        otherButton.transform.Translate(xDelta, 0f, 0f);
        otherButton.pressed = false;
        // Turn off other stoplights
        otherButton.leftStoplight.Stop();
        otherButton.rightStoplight.Stop();
    }
}
