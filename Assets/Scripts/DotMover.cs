using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class DotMover : MainBusUser
{
    public float speed = 2;
    public float leftBorder = -3.75f;
    public float rightBorder = 3.75f;

    public string redZoneDetectorKey = "rzd";
    public string rightHalfDetectorKey = "rhd";
    public string nodeLeftKey = "Left";
    public string nodeRightKey = "Right";

    bool _isOnRedZone;
    bool _isOnRightHalf;

    Node left, right;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isOnRedZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isOnRedZone = false;
    }

    private void Awake()
    {
        ConnectMainBus();
        mainBus.Add(new SignalWrap(() => _isOnRedZone), redZoneDetectorKey);
        mainBus.Add(new SignalWrap(() => _isOnRightHalf), rightHalfDetectorKey);
    }

    private void Start()
    {
        left = mainBus.Get<Node>(nodeLeftKey);
        right = mainBus.Get<Node>(nodeRightKey);
    }

    private void Update()
    {
        if(left.Signal && transform.position.x > leftBorder)
            transform.Translate(Vector2.left * speed * Time.deltaTime);

        if(right.Signal && transform.position.x < rightBorder)
            transform.Translate(Vector2.right * speed * Time.deltaTime);

        if(transform.position.x <= 0)
            _isOnRightHalf = false;
        else
            _isOnRightHalf = true;
    }
}
