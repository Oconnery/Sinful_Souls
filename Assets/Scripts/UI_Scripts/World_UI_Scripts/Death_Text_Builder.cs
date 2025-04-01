using System;
using TMPro;
using UnityEngine;

// TODO: Update to tradional builder design pattern format? Maybe not.
// TODO: Does this class need MonoBehaviour? Probably not.
public class Death_Text_Builder : MonoBehaviour{
    [SerializeField] private Transform worldCanvasTransform;
    [SerializeField] private GameObject evilTextPrefab;
    [SerializeField] private GameObject goodTextPrefab;

    private const float MIN_INITIAL_FONT_SIZE = 14;
    private const float MAX_INITIAL_FONT_SIZE = 72;

    private const ulong DEATHS_FOR_INITIAL_MIN_FONT_SIZE = 100; // At this point the font will be min size
    private const ulong DEATHS_FOR_INITIAL_MAX_FONT_SIZE = 10000; // At this point the font will be max size

    public void InstantiateEvilDeathText(ulong deaths, GameObject region, Vector3 offset) {
        InstantiateDeathText(deaths, region, evilTextPrefab, worldCanvasTransform.TransformPoint(offset));
    }

    public void InstantiateGoodDeathText(ulong deaths, GameObject region, Vector3 offset) {
        InstantiateDeathText(deaths, region, goodTextPrefab, worldCanvasTransform.TransformPoint(offset));
    }

    private void InstantiateDeathText(ulong deaths, GameObject region, GameObject textPrefab, Vector3 offset) {
        Vector3 initialPos = new Vector3((region.transform.position.x + offset.x), (region.transform.position.y + offset.y), 0);
        GameObject textGameObject = Instantiate(textPrefab, initialPos, Quaternion.identity, this.transform);
        TextMeshProUGUI textMeshProUGUI = textGameObject.GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = deaths.ToString();
        textMeshProUGUI.fontSize = CalculateInitialFontSize(deaths);
        textGameObject.name += "_" + region.name;
        textGameObject.SetActive(true);
    }

    private float CalculateInitialFontSize(ulong deaths) {
        // have a few different font sizes for different death levels? or make it progressively bigger each death up until a certain max size (72 or something)
        // lets try make the font bigger progressively with each death.
        switch (deaths) {
            case < DEATHS_FOR_INITIAL_MIN_FONT_SIZE:
                return MIN_INITIAL_FONT_SIZE;
            case > DEATHS_FOR_INITIAL_MAX_FONT_SIZE:
                return MAX_INITIAL_FONT_SIZE;
            default:
                return CalculateInitialFontSizeProgressively(deaths);
        }
    }

    private float CalculateInitialFontSizeProgressively(ulong deaths) {
        float fontSizeDifference = MAX_INITIAL_FONT_SIZE - MIN_INITIAL_FONT_SIZE;
        ulong deathMinMaxDifference = DEATHS_FOR_INITIAL_MAX_FONT_SIZE - DEATHS_FOR_INITIAL_MIN_FONT_SIZE;

        // Calculate how much above the minimum font size the text should be:
        float fontSizeIncrease = (float)Math.Floor(deaths / (deathMinMaxDifference / fontSizeDifference));
        return MIN_INITIAL_FONT_SIZE + fontSizeIncrease;
    }
}
