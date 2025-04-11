using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct PathFindingSystem : ISystem
{
    private EntityQuery gridCellQuery;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PathFinding>();
        state.RequireForUpdate<GridCell>();
        gridCellQuery = state.GetEntityQuery(ComponentType.ReadOnly<GridCell>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        StageLoader stageLoader = SystemAPI.GetSingleton<StageLoader>();
        int width = stageLoader.Width;
        int height = stageLoader.Height;
        
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach ((RefRO<PathFinding> pathFinding, Entity entity) in SystemAPI.Query<RefRO<PathFinding>>().WithEntityAccess())
        {
            int2 start = pathFinding.ValueRO.Start;
            int2 end = pathFinding.ValueRO.End;

            
            NativeArray<GridCell> gridCells = gridCellQuery.ToComponentDataArray<GridCell>(Allocator.Temp);

            NativeList<int2> path = FindPath(start, end, width, height, gridCells);
            
            DynamicBuffer<PathPosition> buffer = ecb.AddBuffer<PathPosition>(entity);
            
            foreach (int2 pos in path)
            {
                buffer.Add(new PathPosition() { Position = pos });
            }
            ecb.RemoveComponent<PathFinding>(entity);

            gridCells.Dispose();
            path.Dispose();
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        // 만약 적군 이동 경로를 막는 스킬을 넣게 되면 경로 계산이 매번 달라지게 하거나, 아니면 그냥 벽 비비게 해야 됨.
        state.Enabled = false;
    }

    private NativeList<int2> FindPath(int2 start, int2 end, int width, int height, NativeArray<GridCell> grid)
    {
        NativeList<int2> path = new NativeList<int2>(Allocator.Temp);

        NativeArray<bool> visited = new NativeArray<bool>(width * height, Allocator.Temp);
        NativeArray<int2> cameFrom = new NativeArray<int2>(width * height, Allocator.Temp);
        NativeArray<int> gScore = new NativeArray<int>(width * height, Allocator.Temp);

        for (int i = 0; i < gScore.Length; ++i)
        {
            gScore[i] = int.MaxValue;
        }

        NativeMinHeap openSet = new NativeMinHeap(Allocator.Temp);
        int startIndex = start.y * width + start.x;
        gScore[startIndex] = 0;
        openSet.Insert(new Node { Position = start, FScore = Heuristic(start, end) });

        int2[] directions = new int2[]
        {
            new int2(0, 1),
            new int2(0, -1),
            new int2(-1, 0),
            new int2(1, 0),
        };

        bool found = false;
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.ExtractMin();
            int2 current = currentNode.Position;

            if (current.Equals(end))
            {
                found = true;
                break;
            }
            
            int currentIndex = current.y * width + current.x;
            visited[currentIndex] = true;

            for (int i = 0; i < directions.Length; ++i)
            {
                int2 neighbor = current + directions[i];
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height)
                {
                    continue;
                }
                
                int neighborIndex = neighbor.y * width + neighbor.x;
                if (grid[neighborIndex].IsWalkable == false || visited[neighborIndex])
                {
                    continue;
                }

                int tentativeG = gScore[currentIndex] + 1;
                if (tentativeG < gScore[neighborIndex])
                {
                    gScore[neighborIndex] = tentativeG;
                    cameFrom[neighborIndex] = current;
                    
                    int fScore = tentativeG + Heuristic(neighbor, end);
                    openSet.Insert(new Node
                    {
                        Position = neighbor, FScore = fScore
                    });
                }
            }
        }

        if (found)
        {
            int2 current = end;
            while (current.Equals(start) == false)
            {
                path.Add(current);
                current = cameFrom[current.y * width + current.x];
            }
            path.Add(start);
            ReversePath(ref path);
        }

        visited.Dispose();
        cameFrom.Dispose();
        gScore.Dispose();
        openSet.Dispose();

        return path;
    }

    private int Heuristic(int2 start, int2 end)
    {
        return math.abs(start.x-end.x) + math.abs(start.y-end.y);
    }
    private void ReversePath(ref NativeList<int2> path)
    {
        for (int i = 0; i < path.Length / 2; i++)
        {
            int j = path.Length - 1 - i;
            int2 temp = path[i];
            path[i] = path[j];
            path[j] = temp;
        }
    }
}
