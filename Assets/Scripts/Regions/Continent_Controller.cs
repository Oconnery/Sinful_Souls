using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 
This script will award the player/AI with sins/prayers if all of the countries in this continent are red(player or green(ai).
The sin/prayer reward will vary according to the size of the continent. Oceiania gives a small boost. Asia gives a huge boost.
 if there is one country in the continent that is not the apporopriate colour, then no sins/prayers are awarded.
*/
public class Continent_Controller : MonoBehaviour {
    //Check the populations of the countries inside them. Or check the color of the game objects inside this game object.
   
    private enum Faction{ GOD, DEVIL, NONE}

    [SerializeField] private GameObject border; // Apply the border when player has clicked on a region

    // This should be called in a different script whenever a region in the continent switches faction.
    public void DesignateOwner(){
       if (CheckIfOwned() != Faction.NONE){
            // Set the owner and apply/turn on associated bonuses.

       }
    }

    // Check if the continent belongs to a faction.
    private Faction CheckIfOwned(){
        return Faction.NONE;
    }

    // TODO: Should be on HUD?
    public void ActivateBorder(){
        border.SetActive(true); 
    }

    public void DeactivateBorder() {
        border.SetActive(false);
    }
}
