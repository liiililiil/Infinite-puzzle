using System.Collections;
using System.Collections.Generic;
using Type.Enums.GamePlay;
using UnityEngine;

public class EndChanger : MonoBehaviour
{
    Queue<MSCell> mineQueue = new Queue<MSCell>();
    Queue<MSCell> queue = new Queue<MSCell>();
    private void BindMine(GameObject gameObject)
    {
        MSCell cell = gameObject.GetComponent<MSCell>();

        queue.Enqueue(cell);

        if (cell.TileState != MineSweeperTiles.Mine) return;

        mineQueue.Enqueue(cell);
    }

    private void Fail()
    {

        while (mineQueue.Count > 0)
        {
            mineQueue.Dequeue().Transparent();
        }

        while (queue.Count > 0)
        {
            MSCell cell = queue.Dequeue();
            AnimationManager.Instance.delayedQueue.Enqueue(() => TurnColor(new Color(255f / 255f, 80f / 255f, 120f / 255f, 1), cell));
        }
    }
    private void Clear()
    {

        while (mineQueue.Count > 0)
        {
            mineQueue.Dequeue().Transparent();
        }

        while (queue.Count > 0)
        {
            MSCell cell = queue.Dequeue();
            AnimationManager.Instance.delayedQueue.Enqueue(() => TurnColor(new Color(80f / 255f, 255f / 255f, 120f / 255f, 1), cell));
        }
    }
    private void Start()
    {

        MSManager.Instance.OnFail.AddListener(Fail);
        MSManager.Instance.OnClear.AddListener(Clear);
        MSManager.Instance.OnInstantiate.AddListener(BindMine);
    }

    private void TurnColor(Color color, MSCell mSCell)
    {
        if (mSCell.IsOpen) return;

        StartCoroutine(TransparentCoroutine(color, mSCell.gameObject.GetComponent<SpriteRenderer>()));
    }

    private IEnumerator TransparentCoroutine(Color target, SpriteRenderer spriteRenderer)
    {
        Color startColor = spriteRenderer.color;
        Color endColor = target;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, endColor, time);
            yield return null;
        }

        spriteRenderer.color = endColor;
    }
}
