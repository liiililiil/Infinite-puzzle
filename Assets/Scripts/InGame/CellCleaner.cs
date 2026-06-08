using System.Collections.Generic;
using UnityEngine;

public class CellCleaner : MonoBehaviour
{
    Stack<GameObject> cells = new Stack<GameObject>();
    public void Clean()
    {
        while (cells.Count > 0)
        {
            Destroy(cells.Pop());
        }
    }

    private void Start()
    {
        MSManager.Instance.OnInstantiate.AddListener(EnQueue);
    }

    private void EnQueue(GameObject gameObject)
    {
        cells.Push(gameObject);
    }
}
