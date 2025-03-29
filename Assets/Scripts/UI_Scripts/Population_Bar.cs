using System;
using UnityEngine;
using UnityEngine.UI;

public class Population_Bar : MonoBehaviour{
    public Image evilBar;
    public Image neutralBar; // NEUTRAL IS THE BACKGROUND I.E. whats left. It never needs to change.
    public Image goodBar;

    public void SetFillAmounts(Region_Controller regionController) {
        ulong evilPop = regionController.GetEvilPop();
        ulong goodPop = regionController.GetGoodPop();
        ulong totalPop = regionController.GetTotalPop();

        evilBar.fillAmount = (float)evilPop / (float)totalPop;
        goodBar.fillAmount = (float)goodPop / (float)totalPop;
        // The remaining percentage is neutral which is all of the background.
    }

    public void SetFillAmounts(ulong evilPop, ulong goodPop, ulong neutralPop) {
        ulong totalPop = evilPop + neutralPop + goodPop;

        Debug.Log(evilPop / totalPop);
        evilBar.fillAmount = (float)evilPop / (float)totalPop;
        goodBar.fillAmount = (float)goodPop / (float)totalPop;
        // The remaining percentage is neutral which is all of the background.
    }
}
