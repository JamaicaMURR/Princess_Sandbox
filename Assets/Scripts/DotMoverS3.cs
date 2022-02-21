using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class DotMoverS3 : MainBusUser
{
    public float speed = 4;
    public float leftBorder = -3.75f;
    public float rightBorder = 3.75f;
    public float upBorder = 3.75f;
    public float downBorder = -3.75f;

    public string redZoneDetectorKey = "rzd";
    public string blueZoneDetectorKey = "bzd";
    public string nodeLeftKey = "nodeLeft";
    public string nodeRightKey = "nodeRight";
    public string nodeUpKey = "nodeUp";
    public string nodeDownKey = "nodeDown";
    public string dotXKey = "x";
    public string dotYKey = "y";

    bool _isOnRedZone;
    bool _isOnBlueZone;

    Node left, right, up, down;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "RedZone")
            _isOnRedZone = true;

        if(collision.gameObject.tag == "BlueZone")
            _isOnBlueZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "RedZone")
            _isOnRedZone = false;

        if(collision.gameObject.tag == "BlueZone")
            _isOnBlueZone = false;
    }

    private void Awake()
    {
        ConnectMainBus();
        mainBus.Add(new SignalWrap(() => _isOnRedZone), redZoneDetectorKey);
        mainBus.Add(new SignalWrap(() => _isOnBlueZone), blueZoneDetectorKey);
        mainBus.Add(new RawWrap(() => transform.position.x), dotXKey);
        mainBus.Add(new RawWrap(() => transform.position.y), dotYKey);
    }

    private void Start()
    {
        left = mainBus.Get<Node>(nodeLeftKey);
        right = mainBus.Get<Node>(nodeRightKey);
        up = mainBus.Get<Node>(nodeUpKey);
        down = mainBus.Get<Node>(nodeDownKey);
    }

    private void Update()
    {
        Vector2 resultive = new Vector2();

        if(left.Signal && transform.position.x > leftBorder)
            resultive += Vector2.left;

        if(right.Signal && transform.position.x < rightBorder)
            resultive += Vector2.right;

        if(up.Signal && transform.position.y < upBorder)
            resultive += Vector2.up;

        if(down.Signal && transform.position.y > downBorder)
            resultive += Vector2.down;

        transform.Translate(resultive.normalized * speed * Time.deltaTime);
    }
}
