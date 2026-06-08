using System;
using Type;
using Type.Enums.GamePlay;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class MSGenerator
{
    private static readonly sbyte MineValue = (sbyte)MineSweeperTiles.Mine;

    public sbyte[,] CreatePuzzle(int seed, byte diff, Vector2Byte size)
    {
        int xSize = size.x;
        int ySize = size.y;
        int totalCells = xSize * ySize;

        float ratio = diff / 255.0f;
        int targetMineCount = Math.Max(1, (int)(totalCells * 0.4f * ratio));

        sbyte[,] map = new sbyte[ySize, xSize];
        NativeArray<sbyte> nativeMap = new NativeArray<sbyte>(totalCells, Allocator.TempJob);

        try
        {
            uint validSeed = seed == 0 ? 123456789U : (uint)seed;

            var job = new MSGeneratorJob
            {
                Map = nativeMap,
                XSize = xSize,
                YSize = ySize,
                TargetMineCount = targetMineCount,
                Seed = validSeed,
                MineValue = MineValue
            };

            job.Run();

            int idx = 0;
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    map[y, x] = nativeMap[idx++];
                }
            }
        }
        finally
        {
            nativeMap.Dispose();
        }

        return map;
    }
}

[BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast)]
public struct MSGeneratorJob : IJob
{
    public NativeArray<sbyte> Map;
    public int XSize;
    public int YSize;
    public int TargetMineCount;
    public uint Seed;
    public sbyte MineValue;

    public void Execute()
    {
        int totalCells = XSize * YSize;
        var rand = new Unity.Mathematics.Random(Seed);

        int placedMines = 0;
        while (placedMines < TargetMineCount)
        {
            int idx = rand.NextInt(0, totalCells);

            if (Map[idx] != MineValue)
            {
                Map[idx] = MineValue;
                placedMines++;
            }
        }

        int i = 0;
        for (int cy = 0; cy < YSize; cy++)
        {
            bool canUp = cy > 0;
            bool canDown = cy < YSize - 1;

            for (int cx = 0; cx < XSize; cx++)
            {
                if (Map[i] == MineValue)
                {
                    i++;
                    continue;
                }

                int count = 0;
                bool canLeft = cx > 0;
                bool canRight = cx < XSize - 1;

                if (canUp && canLeft) { if (Map[i - XSize - 1] == MineValue) count++; }
                if (canUp) { if (Map[i - XSize] == MineValue) count++; }
                if (canUp && canRight) { if (Map[i - XSize + 1] == MineValue) count++; }
                if (canLeft) { if (Map[i - 1] == MineValue) count++; }
                if (canRight) { if (Map[i + 1] == MineValue) count++; }
                if (canDown && canLeft) { if (Map[i + XSize - 1] == MineValue) count++; }
                if (canDown) { if (Map[i + XSize] == MineValue) count++; }
                if (canDown && canRight) { if (Map[i + XSize + 1] == MineValue) count++; }

                Map[i] = (sbyte)count;
                i++;
            }
        }
    }
}