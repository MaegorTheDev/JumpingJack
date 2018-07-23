using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour {

    /// <summary>
    /// The player object
    /// </summary>
    Player playerData; 

    /// <summary>
    /// Game object that represents the player
    /// </summary>
    GameObject playerGameObject;

    /// <summary>
    /// Prefab of the player
    /// </summary>
    public GameObject playerPrefab;

    // Use this for initialization
    void Start () {
        GameManager.Instance.World.RegisterOnPlayerCreated(OnPlayerCreated);
        OnPlayerCreated(GameManager.Instance.World.thePlayer);
    }

    public void OnPlayerCreated(Player player)
    {
        playerData = player;

        //Create the object
        playerGameObject = Instantiate(playerPrefab, new Vector2(player.X, player.Y), Quaternion.identity);
        playerGameObject.name = "Player";
        playerGameObject.transform.parent = gameObject.transform;

        //Add a sprite renderer with height as the size
        SpriteRenderer sprRend = playerGameObject.GetComponentInChildren<SpriteRenderer>();
        sprRend.gameObject.transform.position = new Vector3(0, player.Height/2);
        sprRend.drawMode = SpriteDrawMode.Sliced;
        sprRend.size = new Vector2(player.Height, player.Height);
        sprRend.sortingOrder = 3;

        //Add a box collider with the same size
        BoxCollider2D bc = sprRend.gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bc.size = new Vector2(player.Height, player.Height);

        //Register the two callbacks
        player.RegisterOnCharacterMove(OnPlayerMove);
        player.RegisterOnPlayerStunned(OnPlayerStunned);

        //Add a tag
        playerGameObject.tag = "Player";
    }

    void OnPlayerStunned()
    {
        //Change the color depending on the player state
        if (playerData.Stunned)
        {
            playerGameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        }
        else
        {
            playerGameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }

    public void OnPlayerMove(float x, float y)
    {
        //Update the player position
        playerGameObject.transform.position = new Vector3(x, y);
    }


    public GameObject GetPlayerGameObject()
    {
        //Return the game object
        return playerGameObject;
    }
}
