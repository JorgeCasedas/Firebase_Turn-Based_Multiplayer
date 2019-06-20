using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Classes : MonoBehaviour {
    //private static readonly System.Random random = new System.Random();
    //private static readonly object synlock = new object();

    //public static int RandomNumber(int min, int max) {
    //    lock (synlock) {
    //        System.Random r=new System.Random();
    //        r.Next(2,2);
    //         return random.Next(min, max);
    //    }
    //}

}
[System.Serializable]
public class User{
    public string name;
    public int lvl;
    public int build;
    public int inventory;
    public Statistics stats;
    public CharacterInfo cInfo;
    public string room;
    public User() {
    }

    public User(string username) {
        this.name = username;
        lvl = 0;
        build = 0;
        inventory = 0;
        room = "";
    }
}
[System.Serializable]
public class Statistics {
    public int wonBattles;
    public int lostBattles;

    public Statistics() {
        wonBattles = 0;
        lostBattles = 0;
    }

}
[System.Serializable]
public class CharacterInfo {
    public int hp;
    public int strength;
    public int armor;
    public int magicStrength;
    public int magicArmor;
    System.Random r = new System.Random();

    public CharacterInfo() {
        hp = 100;
        strength = r.Next(5, 11);
        armor = 0;
        magicStrength = r.Next(5, 11);
        magicArmor = 0; 
    }

}

