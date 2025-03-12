using TMPro;
using UnityEngine;

public class DeathText : MonoBehaviour {

    private Vector3 firstTargetPosition;
    private float speed = 0.4f;
    private float distance = 0.8f;

    private Vector3 secondTargetPosition;
    private float secondSpeed = 0.3f;
    private float secondDistance = 0.3f;

    bool inSecondPhase = false;

    private float fontSizeIncreaseModifier = 0.0f;
    private float fontSizeDecreaseModifier = 0.0f;
    private float fontSizeChangeSpeed = 1.0f;
    private float originalFontSize;
    private float maxFontSize;
    private float minFontSize;
    bool maxFontSizeReached = false;
    bool minFontSizeReached = false;

    Color zeroAlphaColor;
    private float fadeSpeed = 1.0f;
    private float alphaDecreaseModifier = 0.0f;

    TextMeshProUGUI textComponent;

    private void Start() {
        firstTargetPosition = transform.position;
        firstTargetPosition.y += distance;
        secondTargetPosition = new Vector3(firstTargetPosition.x, firstTargetPosition.y + secondDistance, firstTargetPosition.z);

        textComponent = GetComponent<TextMeshProUGUI>();
        originalFontSize = textComponent.fontSize;
        maxFontSize = textComponent.fontSize * 1.50f;
        minFontSize = textComponent.fontSize * 0.75f;

        zeroAlphaColor = textComponent.color;
        zeroAlphaColor.a = 0.0f;
    }

    // Update is not called when the gameObject is disabled, so no need for a isEnabled check
    private void Update() {
        float deltaTime = Time.deltaTime;
        if (!inSecondPhase) {
            // FIRST PHASE
            transform.position = Vector3.MoveTowards(transform.position, firstTargetPosition, secondSpeed * deltaTime);
            if (transform.position == firstTargetPosition) {
                inSecondPhase = true;
            }
        } else {
            // SECOND PHASE
            transform.position = Vector3.MoveTowards(transform.position, secondTargetPosition, speed * deltaTime);
            UpdateFont(deltaTime);
            if (transform.position == secondTargetPosition) {
                Destroy(gameObject);
                Debug.Log("destroyed");
            }
        }
    }

    // TODO: issue: At the moment the font changes take a different amount of time depending on how 
    // much they have to change size. So each death text should actually do a proportionate amount
    // of change each frame, which should not be equal to the rate of change of other death texts with
    // larger or smaller initial font.
    private void UpdateFont(float deltaTime) { // 12 to 18, and then from 18 to 9 .... so moving 6 and then moving 9 ... so the second one should be * 1.50 speed.
        if (!maxFontSizeReached){
            textComponent.fontSize += fontSizeIncreaseModifier;
            fontSizeIncreaseModifier = fontSizeChangeSpeed * deltaTime * originalFontSize; // include OG font size so the change happens in the same amount of time no matter what font size the text is
            if (textComponent.fontSize >= maxFontSize){
                maxFontSizeReached = true;
            }
        } else if (!minFontSizeReached){
            // Change size
            textComponent.fontSize -= fontSizeDecreaseModifier;
            fontSizeDecreaseModifier = fontSizeChangeSpeed * deltaTime * originalFontSize * 1.50f * 2.0f; // TODO: make these numbers a variable or something. .. 1.5f is the extra amount the decrease has to change in the same amount of frames. 2.0 is because it has to do it in half the time (s=d/t)

            //UpdateFontOpacity(deltaTime);

            if (textComponent.fontSize <= minFontSize) {
                Debug.Log("minFontsize reached");
                minFontSizeReached = true;
            }
        }
    } // 17 to 25.5 to 12.75 ... so 1.5 times as much .. it only reaches 22 in time though ...

    private void UpdateFontOpacity(float deltaTime){
        if (textComponent.color.a != 0.0f){ // TODO: Instead of using Color.clear, use the alpha from above, and compare results.
            textComponent.color = Color.Lerp(textComponent.color, Color.clear, fadeSpeed * deltaTime);
            //textComponent.CrossFadeAlpha
          // .a -= * deltaTime
        }
    }
}

