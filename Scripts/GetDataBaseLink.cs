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
                //Line of code below makes unity work 
                //this is not in the documentation
                if (FirebaseApp.DefaultInstance.Options.DatabaseUrl != null) FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(FirebaseApp.DefaultInstance.Options.DatabaseUrl);
                db = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });

        

    }
    
}
