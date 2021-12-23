using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VollyHead.Offline;

public class PlayerMoveTest
{

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerMoveLeftTest()
    {
        GameObject playerObj = new GameObject();
        playerObj.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
        Player player = playerObj.AddComponent<Player>();
        player.playerRb = rb;
        player.speed = 100;
        float playerXPosBeforeMove = player.transform.position.x;

        player.inputHorizontal = new Vector2(-1, 0);
        player.Move();

        yield return new WaitForFixedUpdate();

        float playerXPosAfterMove = player.transform.position.x;

        Assert.True(playerXPosBeforeMove > playerXPosAfterMove);
        
    }

    [UnityTest]
    public IEnumerator PlayerMoveRightTest()
    {
        GameObject playerObj = new GameObject();
        playerObj.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
        Player player = playerObj.AddComponent<Player>();
        player.playerRb = rb;
        player.speed = 100;
        float playerXPosBeforeMove = player.transform.position.x;

        player.inputHorizontal = new Vector2(1, 0);
        player.Move();

        yield return new WaitForFixedUpdate();

        float playerXPosAfterMove = player.transform.position.x;

        Assert.True(playerXPosBeforeMove < playerXPosAfterMove);

    }

    [UnityTest]
    public IEnumerator PlayerJumpTest()
    {
        GameObject playerObj = new GameObject();
        playerObj.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
        Player player = playerObj.AddComponent<Player>();
        player.playerRb = rb;
        player.jumpForce = 100f;
        float playerYPosBeforeJump = player.transform.position.y;

        player.Jump();

        yield return new WaitForFixedUpdate();

        float playerYPosAfterJump = player.transform.position.y;

        Assert.True(playerYPosAfterJump > playerYPosBeforeJump);
    }

    [UnityTest]
    public IEnumerator PlayerServeTest()
    {
        GameObject playerObj = new GameObject();
        playerObj.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
        Player player = playerObj.AddComponent<Player>();
        player.playerRb = rb;

        player.servePower = 0;
        player.inputServe = 1;

        player.ServeMode();
        player.IncreasingServePower();

        yield return new WaitForFixedUpdate();


        Assert.True(player.servePower > 0);
    }
}
