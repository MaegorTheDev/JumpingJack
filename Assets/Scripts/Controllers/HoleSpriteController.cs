using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSpriteController : MonoBehaviour {

    //Dictionary between dataobjects and game objects
    public Dictionary<Hole, GameObject> holeGameObjectMap;
    
    void Start()
    {
        holeGameObjectMap = new Dictionary<Hole, GameObject>();

        //We register callbacks to the world
        GameManager.Instance.World.RegisterOnHoleCreated(OnHoleCreated);
        GameManager.Instance.World.RegisterOnHoleDeleted(OnHoleDelete);

        for (int i = 0; i < GameManager.Instance.World.holes.Count; i++)
        {
            OnHoleCreated(GameManager.Instance.World.holes[i]);
        }
    }

    public void OnHoleCreated(Hole hole)
    {
        float hole_sprite_height = 2f;

        GameObject hole_go = new GameObject();
        hole_go.name = "Hole_";
        hole_go.transform.parent = gameObject.transform;
        hole_go.layer = LayerMask.NameToLayer("Hole");

        SpriteRenderer sprRend = hole_go.AddComponent<SpriteRenderer>() as SpriteRenderer;
        sprRend.drawMode = SpriteDrawMode.Sliced;
        sprRend.sprite = Resources.Load<Sprite>("Black");
        sprRend.size = new Vector2(hole.World.holeWidth, hole_sprite_height);
        sprRend.sortingOrder = 1;

        hole_go.transform.position = new Vector2(hole.X, hole.Y);
        
        BoxCollider2D bc = hole_go.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bc.size = new Vector2(hole.World.holeWidth, hole_sprite_height);
        bc.isTrigger = true;

        hole_go.tag = "Hole";

        holeGameObjectMap.Add(hole, hole_go);

        hole.cbHoleChange += OnHoleChange;
    }

    public void OnHoleChange(Hole hole)
    {
        GameObject hole_go = holeGameObjectMap[hole];
        hole_go.transform.position = new Vector3(hole.X, hole.Y);
    }

    public void OnHoleDelete(Hole hole)
    {
        GameObject hole_go = holeGameObjectMap[hole];
        //Unregister from the world
        hole.cbHoleChange -= OnHoleChange;

        holeGameObjectMap.Remove(hole);
        Destroy(hole_go);
    }
}