using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpriteController : MonoBehaviour {

    //Dictionary that links the data objects with the game objects
    public Dictionary<Level, GameObject> levelGameObjectMap;
    //What sprite do I use?
    public Sprite levelSprite;


    void Start()
    {
        levelGameObjectMap = new Dictionary<Level, GameObject>();

        //We register the OnFurnitureCreated to the world
        GameManager.Instance.World.RegisterOnLevelCreated(OnLevelCreated);

        for (int i = 0; i < GameManager.Instance.World.levels.Length; i++)
        {
            OnLevelCreated(GameManager.Instance.World.levels[i]);
        }
    }

    public void OnLevelCreated(Level level)
    {
        //The sprite and object height
        float level_sprite_height = 2f;

        //Set the game object
        GameObject level_go = new GameObject();
        level_go.name = "Level_" + level.LevelNumber;
        level_go.transform.parent = gameObject.transform;
        level_go.layer = LayerMask.NameToLayer("Level");

        //Add sprite render
        SpriteRenderer sprRend = level_go.AddComponent<SpriteRenderer>() as SpriteRenderer;
        sprRend.drawMode = SpriteDrawMode.Sliced;
        sprRend.sprite = levelSprite;
        sprRend.size = new Vector2(level.Width, level_sprite_height);
        
        //Set the position depending on the Y position
        level_go.transform.position = new Vector2(0, level.Y);
        
        //Set the Collider
        BoxCollider2D bc = level_go.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bc.size = new Vector2(level.Width, level_sprite_height);
        bc.isTrigger = true;

        level_go.tag = "Floor";

        levelGameObjectMap.Add(level, level_go);

    }

    //Get the levelGO
    public GameObject GetLevelGOFromLevel(Level level)
    {
        return levelGameObjectMap[level];
    }
}
