using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : Managers<AnimationManager>
{
    private void Awake()
    {
        Singleton(false);
    }

    // 큐로 사라질 타일을 천천히 사라지게 하기
    public Queue<Action> delayedQueue = new Queue<Action>();

    byte animationCoolFrame = 1;
    byte currentFrame = 0;

    private void FixedUpdate()
    {
        currentFrame++;

        if (currentFrame > animationCoolFrame)
        {
            currentFrame = 0;

            if (delayedQueue.Count > 0)
            {
                int processCount = delayedQueue.Count / 10;
                processCount = Mathf.Max(processCount, 10);
                processCount = Mathf.Min(processCount, delayedQueue.Count);

                for (int i = 0; i < processCount; i++)
                {
                    delayedQueue.Dequeue().Invoke();
                }
            }
        }
    }
}