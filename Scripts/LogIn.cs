using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class LogIn : MonoBehaviour {

    public DatabaseReference db;
    Firebase.Auth.FirebaseAuth auth;

    public Text userNameHolder;

    public Text nameDb;
    public Text lvlDb;
    public Text wonDb;
    public Text lostDb;

    public User user;
    public string anonymusName;
    public GetDataBaseLink databaseScript;

    // Use this for initialization
    void Start () {
        databaseScript = GetComponent<GetDataBaseLink>();
        db = databaseScript.db;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        anonymusName = "";
    }
	
	// Update is called once per frame
	void Update () {
        nameDb.text = user.name;
        lvlDb.text = user.lvl.ToString();
        wonDb.text = user.stats.wonBattles.ToString();
        lostDb.text = user.stats.wonBattles.ToString();
    }


    public void SignInAnonymus() {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            anonymusName = newUser.UserId;
            AddUser();
        });
    }

    public void CreateAccount(Text name) {
        string _name = name.text;
        auth.CreateUserWithEmailAndPasswordAsync(_name, _name).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void LogInAccount(Text name) {
        string _name = name.text;
        auth.SignInWithEmailAndPasswordAsync(_name, _name).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void AddUser() {
        if (db==null) {
            db = databaseScript.db;
        }
        string _name = "";
        if (anonymusName == "") {
            _name = userNameHolder.text;
        }
        else {
            _name = anonymusName;
        }
        if (_name != "") {
            db.Child("Users").Child(_name).Child("name").GetValueAsync().ContinueWith(task => { //Child(_name) is temporal until replacing name for the AndroidId
                if (task.IsCompleted) {
                    DataSnapshot snapshot = task.Result;
                    if (task.Result.Value != null)
                        Debug.Log("exists: " + _name);
                    else {
                        Debug.Log("Not exists: " + _name);
                        User user = new User(_name);
                        Statistics stats = new Statistics();
                        CharacterInfo cInfo = new CharacterInfo();
                        string jsonUser = JsonUtility.ToJson(user);
                        string jsonStats = JsonUtility.ToJson(stats);
                        string jsonCInfo = JsonUtility.ToJson(cInfo);
                        db.Child("Users").Child(_name).SetRawJsonValueAsync(jsonUser);
                    }
                    db.Child("Users").Child(_name).GetValueAsync().ContinueWith(task2 => {
                        if (task2.IsCompleted) {
                            DataSnapshot snapshot2 = task2.Result;
                            UpdateUserData(snapshot2);
                        }
                    });

                }
            });

        }
        else {
            Debug.Log("No User Name Especified");
        }
    }
    public void UpdateUserData(DataSnapshot snapshot) {
        user = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
        Debug.Log(snapshot.GetRawJsonValue());      
    }
    private void OnApplicationQuit() {
        auth.SignOut();
    }
}
