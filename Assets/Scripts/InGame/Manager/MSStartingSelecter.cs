using System;
using Type;
using Type.Enums.GamePlay;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class MSStartingSelecter
{
    private static readonly sbyte MineValue = (sbyte)MineSweeperTiles.Mine;

    public Vector2Byte FindStartingPosition(sbyte[,] map)
    {
        int ySize = map.GetLength(0);
        int xSize = map.GetLength(1);
        int totalCells = xSize * ySize;

        NativeArray<sbyte> nativeMap = new NativeArray<sbyte>(totalCells, Allocator.TempJob);
        NativeArray<bool> visited = new NativeArray<bool>(totalCells, Allocator.TempJob);
        NativeArray<int> queue = new NativeArray<int>(totalCells, Allocator.TempJob);
        NativeArray<int> result = new NativeArray<int>(1, Allocator.TempJob);

        try
        {
            int idx = 0;
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    nativeMap[idx++] = map[y, x];
                }
            }

            var job = new MSStartingSelecterJob
            {
                Map = nativeMap,
                XSize = xSize,
                YSize = ySize,
                MineValue = MineValue,
                Visited = visited,
                Queue = queue,
                Result = result
            };

            job.Run();

            int finalIdx = result[0];
            return new Vector2Byte((byte)(finalIdx % xSize), (byte)(finalIdx / xSize));
        }
        finally
        {
            nativeMap.Dispose();
            visited.Dispose();
            queue.Dispose();
            result.Dispose();
        }
    }
}

[BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast)]
public struct MSStartingSelecterJob : IJob
{
    [ReadOnly] public NativeArray<sbyte> Map;
    public int XSize;
    public int YSize;
    public sbyte MineValue;

    public NativeArray<bool> Visited;
    public NativeArray<int> Queue;
    public NativeArray<int> Result;

    public void Execute()
    {
        int totalCells = XSize * YSize;
        int head = 0;
        int tail = 0;

        for (int i = 0; i < totalCells; i++)
        {
            if (Map[i] == MineValue)
            {
                Visited[i] = true;
                int cx = i % XSize;
                int cy = i / XSize;
                Queue[tail++] = (cy << 16) | cx;
            }
        }

        if (tail == 0)
        {
            Result[0] = 0;
            return;
        }

        int lastIdx = 0;

        while (head < tail)
        {
            int curr = Queue[head++];
            int cx = curr & 0xFFFF;
            int cy = curr >> 16;
            int currIdx = cy * XSize + cx;
            lastIdx = currIdx;

            bool canUp = cy > 0;
            bool canDown = cy < YSize - 1;
            bool canLeft = cx > 0;
            bool canRight = cx < XSize - 1;

            if (canUp && canLeft)
            {
                int nIdx = currIdx - XSize - 1;
                if (!Visited[nIdx])
                {
                    Visited[nIdx] = true;
                    Queue[tail++] = ((cy - 1) << 16) | (cx - 1);
                }
            }
            if (canUp)
            {
                int nIdx = currIdx - XSize;
                if (!Visited[nIdx])
                {
                    Visited[nIdx] = true;
                    Queue[tail++] = ((cy - 1) << 16) | cx;
                }
            }
            if (canUp && canRight)
            {
                int nIdx = currIdx - XSize + 1;
                if (!Visited[nIdx])
                {
                    Visited[nIdx] = true;
                    Queue[tail++] = ((cy - 1) << 16) | (cx + 1);
                }
            }
            if (canLeft)
            {
                int nIdx = currIdx - 1;
                if (!Visited[nIdx])
                {
                    Visited[nIdx] = true;
                    Queue[tail++] = (cy << 16) | (cx - 1);
                }
            }
            if (canRight)
            {
                int nIdx = currIdx + 1;
                if (!Visited[nIdx])
                {
                    Visited[nIdx] = true;
                    Queue[tail++] = (cy << 16) | (cx + 1);
                }
            }
            if (canDown && canLeft)
            {
                int nIdx = currIdx + XSize - 1;
                if (!Visited[nIdx])
                {
                    Visited[nIdx] = true;
                    Queue[tail++] = ((cy + 1) << 16) | (cx - 1);
                }
            }
            if (canDown)
            {
                int nIdx = currIdx + XSize;
                if (!Visited[nIdx])
                {
                    Visited[nIdx] = true;
                    Queue[tail++] = ((cy + 1) << 16) | cx;
                }
            }
            if (canDown && canRight)
            {
                int nIdx = currIdx + XSize + 1;
                if (!Visited[nIdx])
                {
                    Visited[nIdx] = true;
                    Queue[tail++] = ((cy + 1) << 16) | (cx + 1);
                }
            }
        }

        Result[0] = lastIdx;
    }
}