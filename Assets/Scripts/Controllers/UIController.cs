using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    //UiObjects reference
    public Text lives;
    public Text lvlText;

    //World reference
    World world;
	void Start () {
        world = GameManager.Instance.World;
    }
	
	void Update () {
        //Just update the value
        //TODO: use callbacks in this instead of checking every frame.
        lives.text = world.Lives + "";
        lvlText.text = "Level " +  world.CurrentDifficulty;
    }
}
