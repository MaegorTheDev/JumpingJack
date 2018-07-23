using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteController : MonoBehaviour
{

    //Dictionary between dataobjects and game objects
    public Dictionary<Enemy, GameObject> enemyGameObjectMap;

    //Sprite masks to tell apart enemies from player
    public Sprite[] enemySpriteMasks;

    void Start()
    {
        enemyGameObjectMap = new Dictionary<Enemy, GameObject>();

        //We register the OnFurnitureCreated to the world
        GameManager.Instance.World.RegisterOnEnemyCreated(OnEnemyChange);
        GameManager.Instance.World.UnregisterOnEnemyCreated(OnEnemyDelete);

        for (int i = 0; i < GameManager.Instance.World.enemies.Count; i++)
        {
            OnEnemyCreated(GameManager.Instance.World.enemies[i]);
        }
    }

    public void OnEnemyCreated(Enemy enemy)
    {

        GameObject enemy_go = new GameObject();
        enemy_go.name = "Enemy_";
        enemy_go.transform.parent = gameObject.transform;
        enemy_go.layer = LayerMask.NameToLayer("Enemy");

        SpriteRenderer sprRend = enemy_go.AddComponent<SpriteRenderer>() as SpriteRenderer;
        sprRend.drawMode = SpriteDrawMode.Sliced;
        sprRend.sprite = Resources.Load<Sprite>("White");
        sprRend.size = new Vector2(enemy.Width, enemy.Width);
        sprRend.sortingOrder = 1;

        //Adds the sprite Mask
        sprRend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        SpriteMask sprMask = enemy_go.AddComponent<SpriteMask>() as SpriteMask;
        sprMask.sprite = enemySpriteMasks[Random.Range(0, enemySpriteMasks.Length - 1)];        

        enemy_go.transform.position = new Vector2(enemy.X, enemy.Y);
        
        BoxCollider2D bc = enemy_go.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bc.size = new Vector2(enemy.Width, enemy.Width);
        bc.isTrigger = true;

        enemyGameObjectMap.Add(enemy, enemy_go);

        //Register
        enemy.RegisterEnemyChange(OnEnemyChange);
    }

    public void OnEnemyChange(Enemy enemy)
    {
        GameObject enemy_go = enemyGameObjectMap[enemy];
        enemy_go.transform.position = new Vector3(enemy.X, enemy.Y);
    }

    public void OnEnemyDelete(Enemy enemy)
    {
        GameObject enemy_go = enemyGameObjectMap[enemy];

        enemy.UnregisterEnemyChange(OnEnemyChange);

        enemyGameObjectMap.Remove(enemy);
        Destroy(enemy_go);
    }
}
