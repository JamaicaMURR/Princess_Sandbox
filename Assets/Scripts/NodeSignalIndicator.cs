using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Princess;

public class NodeSignalIndicator : MainBusUser
{
    Node _node;
    SpriteRenderer _indicatorRenderer;

    public string indicatorName = "Indicator";
    public string nodeBusKey;

    public float periodOfRefreshing;

    public Sprite trueSprite;
    public Sprite falseSprite;


    private void Awake()
    {
        ConnectMainBus();

        _indicatorRenderer = transform.Find(indicatorName).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _node = mainBus.Get<Node>(nodeBusKey);
        StartCoroutine(Refresh());
    }

    void ChangeAtTrue() => _indicatorRenderer.sprite = trueSprite;

    void ChangeAtFalse() => _indicatorRenderer.sprite = falseSprite;

    // TODO: realize on events
    IEnumerator Refresh()
    {
        while(true)
        {
            if(_node.Signal)
                ChangeAtTrue();
            else
                ChangeAtFalse();

            yield return new WaitForSeconds(periodOfRefreshing);
        }
    }
}
