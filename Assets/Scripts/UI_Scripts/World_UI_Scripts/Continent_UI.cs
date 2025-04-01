using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Continent_UI : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color neutralColour;

    private Color devilDominantColour;
    private Color godDominantColour;

    private Color devilFullControlColour;
    private Color godFullControlColour;

    void Start () {
        neutralColour = new Color (155f/255f, 155f/255f, 155f/255f, 85f/255f);

        devilDominantColour = new Color(255f/255f, 0f, 0f, 85f/255f);
        godDominantColour = new Color(0f, 255f/255f, 0f, 85f / 255f);

        devilFullControlColour = new Color(255f/255f, 0f, 0f, 125f/255f);
        godFullControlColour = new Color(0f, 255f/255f, 0f, 125f/255f);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = neutralColour;
    }
	
	void Update () {
        
    }

    void ChangeHudColour(Color colorToChangeTo) {

    }


}
