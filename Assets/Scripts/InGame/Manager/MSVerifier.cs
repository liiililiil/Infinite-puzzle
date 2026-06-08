using System;
using Type;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class MSVerifier
{
    // [변경] 반환 타입을 Task 무관한 순수 튜플 구조로 변경하고 async 제거
    public (sbyte[,], int) Verification(sbyte[,] map, Vector2Byte startAt)
    {
        return ValidateAndFixMap(map, startAt);
    }

    public (sbyte[,], int) ValidateAndFixMap(sbyte[,] map, Vector2Byte startpoint)
    {
        int ySize = map.GetLength(0);
        int xSize = map.GetLength(1);
        int totalCells = xSize * ySize;

        NativeArray<sbyte> nativeMap = new NativeArray<sbyte>(totalCells, Allocator.TempJob);
        NativeArray<bool> revealed = new NativeArray<bool>(totalCells, Allocator.TempJob);
        NativeArray<bool> flagged = new NativeArray<bool>(totalCells, Allocator.TempJob);
        NativeQueue<int> cascadeQueue = new NativeQueue<int>(Allocator.TempJob);
        NativeArray<int> result3BV = new NativeArray<int>(1, Allocator.TempJob);
        NativeArray<bool> visited3BV = new NativeArray<bool>(totalCells, Allocator.TempJob);

        int final3BV = 0;

        try
        {
            // 데이터 1차원 배열로 복사
            for (int y = 0; y < ySize; y++)
                for (int x = 0; x < xSize; x++)
                    nativeMap[y * xSize + x] = map[y, x];

            var job = new MineSweeperSolveJob
            {
                Map = nativeMap,
                XSize = xSize,
                YSize = ySize,
                StartX = startpoint.x,
                StartY = startpoint.y,
                Revealed = revealed,
                Flagged = flagged,
                CascadeQueue = cascadeQueue,
                Result3BV = result3BV,
                Visited3BV = visited3BV
            };

            job.Run();

            final3BV = result3BV[0];

            for (int y = 0; y < ySize; y++)
                for (int x = 0; x < xSize; x++)
                    map[y, x] = nativeMap[y * xSize + x];
        }
        catch (Exception ex)
        {
            Debug.LogError($"Job 실행 중 오류 발생: {ex.Message}");
            throw;
        }
        finally
        {
            // 할당된 임시 네이티브 메모리 해제
            nativeMap.Dispose();
            revealed.Dispose();
            flagged.Dispose();
            cascadeQueue.Dispose();
            result3BV.Dispose();
            visited3BV.Dispose();
        }
        map[startpoint.y, startpoint.x] = -2;

        return (map, final3BV);
    }
}

[BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast)]
public struct MineSweeperSolveJob : IJob
{
    public NativeArray<sbyte> Map;
    public int XSize;
    public int YSize;
    public int StartX;
    public int StartY;

    public NativeArray<bool> Revealed;
    public NativeArray<bool> Flagged;
    public NativeQueue<int> CascadeQueue;

    // 3BV 연산 결과와 방문 체크 버퍼
    public NativeArray<int> Result3BV;
    public NativeArray<bool> Visited3BV;

    private const sbyte MineValue = -1;

    // 난수 생성을 위한 상태값
    private uint randState;
    private const int MaxStartMines = 9;
    public void Execute()
    {
        randState = (uint)(StartX * 374761393 + StartY * 668265263 + 123456789);

        ForceStartAreaSafe();
        RecalculateNumbers();

        int attempts = 0;
        int maxAttempts = 300;

        while (attempts++ < maxAttempts)
        {
            for (int i = 0; i < Map.Length; i++)
            {
                Revealed[i] = false;
                Flagged[i] = false;
            }

            CascadeReveal(StartX, StartY);

            bool progressed;
            do
            {
                progressed = RunDeductionPass();
            }
            while (progressed);

            if (CheckFinalClearState())
            {
                // 완벽히 풀렸을 때 3BV 연산 후 종료
                Result3BV[0] = Calculate3BV();
                return;
            }

            if (!TrySwapMineToFix())
            {
                break;
            }

            RecalculateNumbers();
        }

        // 반복 횟수 초과 시 가장 마지막 맵 상태를 기준으로 3BV 연산
        Result3BV[0] = Calculate3BV();
    }

    private int Get1D(int x, int y) => y * XSize + x;
    private int GetX(int idx) => idx % XSize;
    private int GetY(int idx) => idx / XSize;

    private uint NextRand()
    {
        randState ^= randState << 13;
        randState ^= randState >> 17;
        randState ^= randState << 5;
        return randState;
    }

    private void ForceStartAreaSafe()
    {
        int minesToMoveCount = 0;

        //  C# Managed Array(int[]) 대신 NativeArray를 Temp 할당자로 생성합니다.
        NativeArray<int> minesToMove = new NativeArray<int>(MaxStartMines, Allocator.Temp);

        for (int dy = -1; dy <= 1; dy++)
        {
            int ny = StartY + dy;
            if (ny < 0 || ny >= YSize) continue;
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = StartX + dx;
                if (nx < 0 || nx >= XSize) continue;

                int idx = Get1D(nx, ny);
                if (Map[idx] == MineValue)
                {
                    Map[idx] = 0; // 지뢰 제거
                    minesToMove[minesToMoveCount++] = idx; // 옮겨야 할 지뢰 저장
                }
            }
        }

        for (int i = 0; i < minesToMoveCount; i++)
        {
            for (int j = 0; j < Map.Length; j++)
            {
                // 조건: 현재 빈 칸(0)이고, 시작지점 3x3 영역 밖이어야 함
                int mx = GetX(j);
                int my = GetY(j);

                // 시작 영역 3x3 내부에 있는지 체크
                bool isInsideStartArea = (Math.Abs(mx - StartX) <= 1 && Math.Abs(my - StartY) <= 1);

                if (Map[j] == 0 && !isInsideStartArea)
                {
                    Map[j] = MineValue;
                    break;
                }
            }
        }

        minesToMove.Dispose();
    }

    private void RecalculateNumbers()
    {
        for (int i = 0; i < Map.Length; i++)
        {
            if (Map[i] == MineValue) continue;

            int count = 0;
            int x = GetX(i);
            int y = GetY(i);

            for (int dy = -1; dy <= 1; dy++)
            {
                int ny = y + dy;
                if (ny < 0 || ny >= YSize) continue;
                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx;
                    if (nx < 0 || nx >= XSize || (dx == 0 && dy == 0)) continue;

                    if (Map[Get1D(nx, ny)] == MineValue) count++;
                }
            }
            Map[i] = (sbyte)count;
        }
    }

    private void CascadeReveal(int startX, int startY)
    {
        CascadeQueue.Clear();
        int startIdx = Get1D(startX, startY);
        if (Revealed[startIdx]) return;

        CascadeQueue.Enqueue(startIdx);
        Revealed[startIdx] = true;

        while (CascadeQueue.TryDequeue(out int currIdx))
        {
            if (Map[currIdx] != 0) continue;

            int cx = GetX(currIdx);
            int cy = GetY(currIdx);

            for (int dy = -1; dy <= 1; dy++)
            {
                int ny = cy + dy;
                if (ny < 0 || ny >= YSize) continue;
                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = cx + dx;
                    if (nx < 0 || nx >= XSize || (dy == 0 && dx == 0)) continue;

                    int nIdx = Get1D(nx, ny);
                    if (!Revealed[nIdx] && Map[nIdx] != MineValue)
                    {
                        Revealed[nIdx] = true;
                        CascadeQueue.Enqueue(nIdx);
                    }
                }
            }
        }
    }

    private bool RunDeductionPass()
    {
        bool progress = false;

        for (int idx = 0; idx < XSize * YSize; idx++)
        {
            if (!Revealed[idx] || Map[idx] <= 0)
                continue;

            int x = GetX(idx);
            int y = GetY(idx);
            int flaggedCount = 0;
            int unknownCount = 0;

            for (int dy = -1; dy <= 1; dy++)
            {
                int ny = y + dy;
                if (ny < 0 || ny >= YSize) continue;

                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx;
                    if (nx < 0 || nx >= XSize || (dy == 0 && dx == 0)) continue;

                    int nIdx = Get1D(nx, ny);
                    if (Flagged[nIdx]) flaggedCount++;
                    else if (!Revealed[nIdx]) unknownCount++;
                }
            }

            int remain = Map[idx] - flaggedCount;

            if (remain == 0 && unknownCount > 0)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int ny = y + dy;
                    if (ny < 0 || ny >= YSize) continue;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int nx = x + dx;
                        if (nx < 0 || nx >= XSize || (dy == 0 && dx == 0)) continue;

                        int nIdx = Get1D(nx, ny);
                        if (!Revealed[nIdx] && !Flagged[nIdx])
                        {
                            CascadeReveal(nx, ny);
                            progress = true;
                        }
                    }
                }
            }
            else if (remain == unknownCount && unknownCount > 0)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int ny = y + dy;
                    if (ny < 0 || ny >= YSize) continue;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int nx = x + dx;
                        if (nx < 0 || nx >= XSize || (dy == 0 && dx == 0)) continue;

                        int nIdx = Get1D(nx, ny);
                        if (!Revealed[nIdx] && !Flagged[nIdx])
                        {
                            Flagged[nIdx] = true;
                            progress = true;
                        }
                    }
                }
            }
        }

        return progress;
    }

    private bool TrySwapMineToFix()
    {
        int frontierMine = -1;
        int targetEmpty = -1;

        int mineCandidates = 0;
        int emptyCandidates = 0;

        for (int i = 0; i < Map.Length; i++)
        {
            if (Revealed[i] || Flagged[i]) continue;

            if (Map[i] == MineValue)
            {
                mineCandidates++;
                if ((NextRand() % mineCandidates) == 0) frontierMine = i;
            }
        }

        for (int i = 0; i < Map.Length; i++)
        {
            if (Revealed[i] || Flagged[i] || Map[i] == MineValue) continue;

            emptyCandidates++;
            if ((NextRand() % emptyCandidates) == 0) targetEmpty = i;
        }

        if (frontierMine == -1 || targetEmpty == -1)
            return false;

        Map[frontierMine] = 0;
        Map[targetEmpty] = MineValue;

        return true;
    }

    private bool CheckFinalClearState()
    {
        for (int i = 0; i < Map.Length; i++)
        {
            if (Map[i] != MineValue && !Revealed[i])
                return false;
        }
        return true;
    }

    private int Calculate3BV()
    {
        int bv = 0;

        for (int i = 0; i < Map.Length; i++)
        {
            Visited3BV[i] = false;
        }

        for (int i = 0; i < Map.Length; i++)
        {
            if (Map[i] == 0 && !Visited3BV[i])
            {
                bv++;
                CascadeQueue.Clear();
                CascadeQueue.Enqueue(i);
                Visited3BV[i] = true;

                while (CascadeQueue.TryDequeue(out int currIdx))
                {
                    int cx = GetX(currIdx);
                    int cy = GetY(currIdx);

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int ny = cy + dy;
                        if (ny < 0 || ny >= YSize) continue;
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = cx + dx;
                            if (nx < 0 || nx >= XSize || (dy == 0 && dx == 0)) continue;

                            int nIdx = Get1D(nx, ny);
                            if (!Visited3BV[nIdx] && Map[nIdx] != MineValue)
                            {
                                Visited3BV[nIdx] = true;
                                if (Map[nIdx] == 0)
                                {
                                    CascadeQueue.Enqueue(nIdx);
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < Map.Length; i++)
        {
            if (Map[i] > 0 && !Visited3BV[i] && Map[i] != MineValue)
            {
                bv++;
            }
        }

        return bv;
    }
}