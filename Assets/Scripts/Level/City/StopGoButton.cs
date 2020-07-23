using UnityEngine;

public class StopGoButton : MonoBehaviour {
    public bool pressed;
    public StopGoButton otherButton;

    public Stoplight leftStoplight;
    public Stoplight rightStoplight;

    public StopGoLever lever;

    // Maybe the buttons could be on some kind of timer in addition to hitting them?

    // Start is called before the first frame update
    void Start() {
        if(pressed) {
            // Turn off stoplights
            leftStoplight.Stop();
            rightStoplight.Stop();
        } else {
            // Turn on stoplights
            leftStoplight.Go();
            rightStoplight.Go();
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
        float xDelta = 0.5f * -Mathf.Sign(transform.position.x);
        transform.Translate(xDelta, 0f, 0f);
        pressed = true;
        // Turn off our stoplights
        leftStoplight.Stop();
        rightStoplight.Stop();

        // Move the lever
        lever.ChangePosition(this);

        // Turn on other stoplights
        otherButton.leftStoplight.Go();
        otherButton.rightStoplight.Go();
    }

    public void FinishPress() {
        // Push opposite button outwards
        float xDelta = 0.5f * Mathf.Sign(otherButton.transform.position.x);
        otherButton.transform.Translate(xDelta, 0f, 0f);
        otherButton.pressed = false;
    }
}
