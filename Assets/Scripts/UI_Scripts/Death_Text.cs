using TMPro;
using UnityEngine;

public class DeathText : MonoBehaviour {

    private Vector3 firstTargetPosition;
    private float speed = 0.4f;
    private float distance = 0.8f;

    private Vector3 secondTargetPosition;
    private float secondSpeed = 0.3f;
    private float secondDistance = 0.5f;
   
    private float fontSizeModifier;
    private float fontChangeSpeed = 100.0f;
    private float maxFontSize;

    bool inSecondPhase = false;

    TextMeshProUGUI textComponent;

    private void Start() {
        firstTargetPosition = transform.position;
        firstTargetPosition.y += distance;
        secondTargetPosition = new Vector3(firstTargetPosition.x, firstTargetPosition.y + secondDistance, firstTargetPosition.z);

        textComponent = GetComponent<TextMeshProUGUI>();
        fontSizeModifier = 0.0f; // Should start at zero.
        maxFontSize = textComponent.fontSize * 1.50f;
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
            UpdateFontSize(deltaTime);
            if (transform.position == secondTargetPosition) {
                Destroy(gameObject);
            }
        }
    }

    private void UpdateFontSize(float deltaTime) {
        if (textComponent.fontSize < maxFontSize) {
            textComponent.fontSize += fontSizeModifier;
            fontSizeModifier = fontChangeSpeed * deltaTime;
        }
    }
}

