using System;
using System.Collections;
using System.Collections.Generic;

namespace Procool.Utils
{
    public class PriorityQueue<TKey, TValue> : ICollection<PriorityQueue<TKey, TValue>.Pair>, IEnumerable<PriorityQueue<TKey, TValue>.Pair>
        where TKey: IComparable
    {
        public struct Pair
        {
            public TKey Key;
            public TValue Value;
        }

        private List<Pair> heap;

        public PriorityQueue(int capacity)
        {
            heap = new List<Pair>(capacity);
        }

        public PriorityQueue()
        {
            heap = new List<Pair>();
        }

        public Pair Peak => heap[0];

        // 0 -> root
        // x * 2 + 1 -> left child
        // x * 2 + 2 -> right child
        // (x - 1) >> 2 -> parent
        void UpdateUp(int startIdx)
        {
            for (var node = startIdx; node != 0; node = (node - 1) / 2)
            {
                var parent = (node - 1) / 2;

                if (heap[parent].Key.CompareTo(heap[node].Key) <= 0)
                    break;

                var temp = heap[parent];
                heap[parent] = heap[node];
                heap[node] = temp;
            }
        }

        void UpdateDown(int startIdx)
        {
            var node = startIdx;
            while (node < heap.Count)
            {
                var leftChild = node * 2 + 1;
                var rightChild = node * 2 + 2;
                if (leftChild >= heap.Count)
                    break;
                if (rightChild >= heap.Count)
                {
                    if (heap[leftChild].Key.CompareTo(heap[node].Key) < 0)
                    {
                        (heap[leftChild], heap[node]) = (heap[node], heap[leftChild]);
                        node = leftChild;
                    }
                    else
                        break;
                }
                else if (heap[node].Key.CompareTo(heap[leftChild].Key) <= 0 &&
                         heap[node].Key.CompareTo(heap[rightChild].Key) <= 0)
                {
                    break;
                }
                else if (heap[leftChild].Key.CompareTo(heap[rightChild].Key) < 0)
                {
                    (heap[leftChild], heap[node]) = (heap[node], heap[leftChild]);
                    node = leftChild;
                }
                else
                {
                    (heap[rightChild], heap[node]) = (heap[node], heap[rightChild]);
                    node = rightChild;
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            Add(new Pair()
            {
                Key = key,
                Value = value
            });
        }

        public void Add(Pair item)
        {
            heap.Add(item);
            UpdateUp(heap.Count - 1);
        }

        public Pair Pop()
        {
            if(heap.Count <= 0)
                throw new IndexOutOfRangeException();

            var pair = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            UpdateDown(0);
            return pair;
        }

        public IEnumerator<Pair> GetEnumerator()
        {
            return heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) heap).GetEnumerator();
        }

        public void Clear()
        {
            heap.Clear();
        }

        public bool Contains(Pair item)
        {
            return heap.Contains(item);
        }

        public void CopyTo(Pair[] array, int arrayIndex)
        {
            heap.CopyTo(array, arrayIndex);
        }

        public bool Remove(Pair item)
        {
            return heap.Remove(item);
        }

        public int Count => heap.Count;

        public bool IsReadOnly => false;
    }
}