using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
This script will award the player/AI with sins/prayers if all of the countries in this continent are red(player or green(ai).
The sin/prayer reward will vary according to the size of the continent. Oceiania gives a small boost. Asia gives a huge boost.

 if there is one country in the continent that is not the apporopriate colour, then no sins/prayers are awarded.
*/

enum Classification{godControlled, devilControlled, godInclined, devilInclined};
enum gender{Male, female, other};


public class Continent_Controller : MonoBehaviour {
    string _classification;


    // How to show that the player/AI controls a continent? And how to show that bonus?
    // Outline continent with a glow?
    // Some sort of HUD over the middle of the continent

    //Check the populations of the countries inside them. Or check the color of the game objects inside this game object.

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        // according to the classification, give a bonus every day.
        // This class should have a method that is called everyday to set ownership and give appropriate bonuses.
        

	}

    private void SetClassification(Classification classification) {
        // changes the owner of the continent controller to #
        _classification = classification.ToString();
    }

     
}
