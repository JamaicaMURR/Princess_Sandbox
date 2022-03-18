using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;
using System;

public class DotMover : MainBusUser
{
    Sink _left, _right, _up, _down;
    Vector2 _resultiveMove;

    Action HandleLeftSink, HandleRightSink, HandleUpSink, HandleDownSink;

    public float Speed { get; set; } = 4;

    public float leftBorder = -3.75f;
    public float rightBorder = 3.75f;
    public float upBorder = 3.75f;
    public float downBorder = -3.75f;

    public string leftSinkKey, rightSinkKey, upSinkKey, downSinkKey;

    private void Awake()
    {
        ConnectMainBus();
    }

    private void Start()
    {
        //
        if(leftSinkKey != string.Empty)
        {
            _left = mainBus.Get<Sink>(leftSinkKey);

            HandleLeftSink = () =>
            {
                if(_left.Signal && transform.position.x > leftBorder)
                    _resultiveMove += Vector2.left;
            };
        }
        else
            HandleLeftSink = () => { };

        //
        if(rightSinkKey != string.Empty)
        {
            _right = mainBus.Get<Sink>(rightSinkKey);

            HandleRightSink = () =>
            {
                if(_right.Signal && transform.position.x < rightBorder)
                    _resultiveMove += Vector2.right;
            };
        }
        else
            HandleRightSink = () => { };

        //
        if(upSinkKey != string.Empty)
        {
            _up = mainBus.Get<Sink>(upSinkKey);

            HandleUpSink = () =>
            {
                if(_up.Signal && transform.position.y < upBorder)
                    _resultiveMove += Vector2.up;
            };
        }
        else
            HandleUpSink = () => { };

        //
        if(downSinkKey != string.Empty)
        {
            _down = mainBus.Get<Sink>(downSinkKey);

            HandleDownSink = () =>
            {
                if(_down.Signal && transform.position.y > downBorder)
                    _resultiveMove += Vector2.down;
            };
        }
        else
            HandleDownSink = () => { };
    }

    private void Update()
    {
        _resultiveMove = Vector2.zero;

        HandleLeftSink();
        HandleRightSink();
        HandleUpSink();
        HandleDownSink();

        transform.Translate(_resultiveMove.normalized * Speed * Time.deltaTime);
    }
}
