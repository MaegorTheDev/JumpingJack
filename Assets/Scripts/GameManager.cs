using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    //Instacia estatica del mundo
    public static GameManager Instance { get; protected set; }

    public World World { get; protected set; }
    public float speed;

    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("Error in GameManager");
        }
        Instance = this;

        CreateWorld();      
    }

    private void Update()
    {
        World.WorldUpdate(Time.deltaTime);
    }

    void CreateWorld()
    {
        //Creacion del mundo logicamente
        World = new World(500, 400, speed);

        Camera.main.gameObject.transform.position = new Vector3(0, 400 / 2, -10) ;
    }

}
