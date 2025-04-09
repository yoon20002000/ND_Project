using Unity.Collections;
using Unity.Jobs;

public class NativeMinHeap : INativeDisposable
{
    private NativeList<Node> heap;
    private Allocator allocator;

    public NativeMinHeap(Allocator allocator)
    {
        this.allocator = allocator;
        this.heap = new NativeList<Node>(allocator);
    }

    public int Count => heap.Length;

    public void Insert(Node node)
    {
        heap.Add(node);
        HeapifyUp(heap.Length - 1);
    }

    public Node ExtractMin()
    {
        Node min = heap[0];
        int lastIndex = heap.Length - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);
        HeapifyDown(0);
        return min;
    }

    public bool IsEmpty => heap.IsEmpty;

    public void Clear()
    {
        heap.Clear();
    }

    public JobHandle Dispose(JobHandle inputDeps)
    {
        return heap.Dispose(inputDeps);
    }

    public void Dispose()
    {
        if (heap.IsCreated)
        {
            heap.Dispose();
        }
    }

    private void HeapifyUp(int index)
    {
        while (index > 0) 
        {
            int parent = (index - 1) / 2;
            if (heap[index].FScore >= heap[parent].FScore)
            {
                break;
            }

            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int length = heap.Length;
        while (true)
        {
            int left = (index * 2) + 1;
            int right = left + 1;
            int smallest = index;

            if (left < length && heap[left].FScore < heap[smallest].FScore)
            {
                smallest = left;
            }
            if (right< length && heap[right].FScore < heap[smallest].FScore)
            {
                smallest = right;
            }

            if (smallest == index)
            {
                break;
            }

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        Node temp = heap[a];
        heap[a] = heap[b];
        heap[b] = temp;
    }
}