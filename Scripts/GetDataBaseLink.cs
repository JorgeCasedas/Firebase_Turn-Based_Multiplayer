using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;



public class GetDataBaseLink : MonoBehaviour {

    public DatabaseReference db;
    //public FirebaseApp app;

    private void Awake() {
       
    }
    // Use this for initialization
    void Start () {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {

                //app = FirebaseApp.DefaultInstance; This line of code FREEZES UNITY

                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://basedatataskrk.firebaseio.com/");
                //Line of code below MAKES UNITY WORK HALLELUJAH PRAISE THE MOTHERFUCKING GOD IT TOOK ME 5 FCKING HOURS WITH UNITY CRASHING EVERY SECOND
                //The fcking documentation doesnt say this
                if (FirebaseApp.DefaultInstance.Options.DatabaseUrl != null) FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(FirebaseApp.DefaultInstance.Options.DatabaseUrl);
                db = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        

    }
    
}
