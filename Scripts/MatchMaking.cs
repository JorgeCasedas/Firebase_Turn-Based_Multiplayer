using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


public class MatchMaking : MonoBehaviour {

    DatabaseReference db;

    [HideInInspector]
    public User user;
    public List<string> roomsPool = new List<string>();
    public GameObject battleCanvas;
    public GameObject logInCanvas;
    void Start () {
        db = GetComponent<GetDataBaseLink>().db;
    }
	
	void Update () {
		
	}
    public void CheckRooms() {//----------------------OVERWRITE SAFE
        user=GetComponent<LogIn>().user;
        if (db == null) {
            db = GetComponent<GetDataBaseLink>().db;
        }
        roomsPool.Clear();
        db.Child("Rooms").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                string rooms = snapshot.GetRawJsonValue();
                string tempRooms = rooms.Substring(1, rooms.Length-2);
                //----------------------SAVE IN A LIST ALL ROOMS

                while (tempRooms.IndexOf('}') > 0) {
                    Debug.Log(tempRooms.IndexOf('}'));
                    roomsPool.Add(tempRooms.Substring(0, tempRooms.IndexOf('}') + 1));
                    tempRooms = tempRooms.Remove(0, tempRooms.IndexOf('}') + 1);
                    Debug.Log(tempRooms);
                }

                //----------------------CHECK IF THERE IS ANY ROOM WITH THE "PLAYER2" VALUE EMPTY //Make this a void so it can be accessed for all rooms not just the first one
                string freeRoom = "";
                int i = 0;
                while (freeRoom == "" && i<roomsPool.Count) {
                    Debug.Log("inLoop");
                    string tempCheck = roomsPool[i];
                    int start = tempCheck.LastIndexOf(':');
                    int end = tempCheck.IndexOf('}');
                    string player2Value = tempCheck.Substring(start+1, (end - start)-1);
                    Debug.Log(player2Value);
                    if (player2Value == "\"\"") {
                        freeRoom = tempCheck.Substring(tempCheck.IndexOf('"')+1, (tempCheck.IndexOf(':')- tempCheck.IndexOf('"'))-2);
                        Debug.Log(freeRoom);
                    }
                    i++;
                }
                if (freeRoom == "") {
                    CreateRoom();
                }
                else {
                    JoinRoom(freeRoom);
                }
            }
        });
    }
    void CreateRoom() {
        db.Child("Rooms").Child(user.name).Child("Player1").SetValueAsync(user.name);
        db.Child("Rooms").Child(user.name).Child("Player2").SetValueAsync("");
        db.Child("Rooms").Child(user.name).Child("Win").SetValueAsync("");
        user.room = user.name;
        WaitForOpponent();
    }
    void JoinRoom(string roomName) {
        db.Child("Rooms").Child(roomName).Child("Player2").SetValueAsync(user.name);
        db.Child("Users").Child(user.name).Child("room").ValueChanged += ListenForRoom; 
    }
    void WaitForOpponent() {
        Debug.Log("Waiting");
        db.Child("Rooms").Child(user.name).Child("Player2").ValueChanged += ListenForOpponent;
        Debug.Log("OutOfWaiting");
    }
    void ListenForOpponent(object sender, ValueChangedEventArgs args) {   
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string opponentName =(string)args.Snapshot.GetValue(true);
        Debug.Log("The Opponent Found is: " + opponentName);
        if (opponentName != "") {//-----------------------------Opponent Found
            db.Child("Rooms").Child(user.name).Child("Player2").ValueChanged -= ListenForOpponent;
            db.Child("Users").Child(opponentName).Child("room").SetValueAsync(user.name);
            Debug.Log("Opponent found: " + opponentName);
            StartBattle();
            Debug.Log("Started Battle ");
        }
        else {
            Debug.Log("OpponentNotFoundYet");
        }
    }
    void ListenForRoom(object sender, ValueChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string roomName = (string)args.Snapshot.GetValue(true);
        if (roomName != "") {//-----------------------------Opponent Found
            db.Child("Users").Child(user.name).Child("room").ValueChanged -= ListenForRoom;
            Debug.Log("Room found: " + roomName);
            user.room = roomName;
            StartBattle();
        }
        else {
            Debug.Log("RoomNotFoundYet");
        }
    }
    void ListenForVictory(object sender, ValueChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string userVictory = (string)args.Snapshot.GetValue(true);
        if (userVictory != "") {//-----------------------------Opponent Found
            db.Child("Rooms").Child(user.room).Child("Win").ValueChanged -= ListenForVictory;
            Debug.Log(userVictory+ " has won");
            EndBattle(userVictory);
        }
        else {
            Debug.Log("Not Winner Yet");
        }
    }

    void StartBattle() {
        db.Child("Rooms").Child(user.room).Child("Win").ValueChanged += ListenForVictory;
        Debug.Log("Classe aun no Creada");
        battleCanvas.SetActive(true);   //Doesn't work in the editor but works on the phone
        logInCanvas.SetActive(false);   //Doesn't work in the editor but works on the phone
    }
   
    void EndBattle(string winnerName) {
        if(user.name == user.room) {
            if (db.Child("Rooms").Child(user.room).GetValueAsync() != null)
                    db.Child("Rooms").Child(user.room).RemoveValueAsync();
        }
        user.room = null;
        battleCanvas.SetActive(false);  //Doesn't work in the editor but works on the phone
        logInCanvas.SetActive(true);    //Doesn't work in the editor but works on the phone

    }
    public void WinBattle() {
        db.Child("Rooms").Child(user.room).Child("Win").SetValueAsync(user.name);
    }
}

