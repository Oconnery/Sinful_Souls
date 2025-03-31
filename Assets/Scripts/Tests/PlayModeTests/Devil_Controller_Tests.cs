using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine;

/* LIST OF WHAT I WANT TO TEST IN DEVIL CONTROLLER CLASS 
     * test that available demons is not more than max (recurring)
     * test that available banshees is not more than max (recurring)
     * 
     * 
     * 
     * 
*/

namespace Tests {
    public class Devil_Controller_Tests {

        private GameObject gameObject;
        private Devil_Controller devil_Controller;

        [SetUp]
        public void Setup() {
            gameObject = GameObject.Instantiate(new GameObject());
            devil_Controller = gameObject.AddComponent<Devil_Controller>();
        }
           

        //[UnityTest]
        //public  IEnumerator StartingDemonsNotNull() {
        //    Devil_Controller devil_Controller = new Devil_Controller();
        //    yield return null;

        //    Assert.IsNotNull(devil_Controller.());
        //}

        //[UnityTest]
        //public IEnumerator StartingBansheesNotNull() {
        //    Devil_Controller devil_Controller = new Devil_Controller();
        //    yield return null;

        //    Assert.IsNotNull(devil_Controller.startingAvailableBanshees);
        //}

        //[UnityTest]
        //public IEnumerator AvailableDemonsEqualsStartingDemons() {
        //    Devil_Controller devil_Controller = new Devil_Controller();
        //    yield return null;

        //    var available = devil_Controller.GetAvailableAgents();
        //    var starting = devil_Controller.startingAvailableDemons;

        //    Assert.IsTrue(available == starting);
        //}

        //[UnityTest]
        //public IEnumerator MaxDemonsEqualsStartingDemons() {
        //    Devil_Controller devil_Controller = new Devil_Controller();
        //    yield return null;

        //    var max = devil_Controller.maxDeployableDemons;
        //    var starting = devil_Controller.startingAvailableDemons;

        //    Assert.IsTrue(max == starting);
        //}

        //[UnityTest]
        //public IEnumerator AvailableBansheesEqualsStartingBanshees() {
        //    Devil_Controller devil_Controller = new Devil_Controller();
        //    yield return null;

        //    var available = devil_Controller.GetAvailableSecondaryUnits();
        //    var starting = devil_Controller.startingAvailableBanshees;

        //    Assert.IsTrue(available == starting);
        //}

        //[UnityTest]
        //public IEnumerator MaxBansheesEqualsStartingBanshees() {
        //    Devil_Controller devil_Controller = new Devil_Controller();
        //    yield return null;

        //    var max = devil_Controller.maxDeployableBanshees;
        //    var starting = devil_Controller.startingAvailableBanshees;

        //    Assert.IsTrue(max == starting);
        //}

        //[UnityTest]
        //public IEnumerator SpendSinsNegativeParameterThrowsArguementException() {
        //    Devil_Controller devil_Controller = new Devil_Controller();
        //    yield return null;

        //    var startingSins = devil_Controller.GetSins();
        //    devil_Controller.SpendSins(-200f);
        //    var endingSins = devil_Controller.GetSins();
        //    Debug.Log($"Starting sins: {startingSins}. Ending Sins: {endingSins}.");

        //    Assert.Throws(typeof(System.ArgumentException), new TestDelegate(devil_Controller.SpendSins(-200f)));
        //}

        //spending more sins than you have

        [TearDown]
        public void Teardown() {
            GameObject.Destroy(devil_Controller);
            GameObject.Destroy(gameObject);
        }

    }
}