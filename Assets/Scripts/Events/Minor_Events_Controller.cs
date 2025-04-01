using UnityEngine;
using UnityEngine.UI;
using static Region_Controller;

public class Minor_Events_Controller : MonoBehaviour {
    public Text eventsContentText;

    private string eventsDescr;
    [SerializeField] private Hud_Controller hudController;
    [SerializeField] private Devil_Controller devilController;
    [SerializeField] private God_Controller godController;
    [SerializeField] private Region_Master_Controller regionMasterController;

    [Tooltip("1.0 represents a 100% chance. 0.01 represents a 1.0 chance. Anything above 1.0 is regarded as 100%")]
    [SerializeField] private float randomEventChance;

    void Start() {
        eventsDescr = "Default text";
        eventsContentText.text = eventsDescr;
        Clock.OnDayPassedNotifyMinorEvents += ChanceToCallRandomMinorEvent;
    }

    public void ChanceToCallRandomMinorEvent() {
        // will probably need mutual exlucsion lock/sempahore so that only one event can run at a time. if I have major events etc.
        // 1/20 days mean avg it should do a minor event once every 20 days. Would be cool to do this based upon an "activity level
        // variable, so when the user has not made many actions, and there hasn't been an event in a while, the chance for one
        // increases.
        if (Random.value < randomEventChance) { // TODO CHANGE
            ExecuteRandomMinorEvent();
        }
    }

    private void ExecuteRandomMinorEvent() {
        Clock.Pause();
        Region_Controller region = regionMasterController.GetRandomRegionController();
        string regionName = region.name;

        if (Random.value > 0.3f)
            RandomWar(region.GetComponent<Region_Controller>(), regionName);
        else MurderousCult(region.GetComponent<Region_Controller>(), regionName);

        // Set populations and give resources.
        devilController.DailyShout();
        godController.DailyShout();
    }

    public void RandomWar(Region_Controller regionController, string regionName) {
        // TODO: The number of people should somewhat scale with the number avaialable in the region? (not linearly)
        ulong goodPeopleKilled = regionController.KillPeople(Region_Controller.Alignment.GOOD,  1000, 300000);
        ulong neutralPeopleKilled = regionController.KillPeople(Region_Controller.Alignment.NEUTRAL, 1000, 300000);
        ulong evilPeopleKilled = regionController.KillPeople(Region_Controller.Alignment.EVIL, 1000, 300000);

        eventsDescr = $"A war has broken out in {regionName}. {evilPeopleKilled} evil people, {goodPeopleKilled} good people and {neutralPeopleKilled} neutral people have died in the fighting!";
        eventsContentText.text = eventsDescr;
        hudController.SetEventsPanelActive();
    }

    public void MurderousCult(Region_Controller regionController, string regionName) {
        ulong goodPeopleKilled = regionController.KillPeople(Alignment.GOOD, 1, 10000);

        eventsDescr = $"An evil murderous cult has appeared in {regionName}! {goodPeopleKilled} good people have been massacred!";
        eventsContentText.text = eventsDescr;
        hudController.SetEventsPanelActive();
    }
}
