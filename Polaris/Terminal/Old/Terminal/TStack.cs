using System.Collections.Generic;

namespace Polaris.Debug.Terminal
{
    public class TStack<T>
    {
        public List<T> items = new List<T>();
        public int Count => items.Count;

        public void Push(T item)
        {
            items.Add(item);
        }

        public T Pop()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return temp;
            }
            else
                return default(T);
        }

        // Similar to Stack.Peek but allows you to give a number of passes.
        public T Peek(int entiresToPassBeforePeek = 0)
        {
            if (items.Count > 0)
            {
                T temp;
                // If the number of passes is more than the size of the list,
                // then give the first element.
                if (items.Count < entiresToPassBeforePeek)
                {
                    temp = items[0];
                    return temp;
                }

                temp = items[items.Count - (1 + entiresToPassBeforePeek)];
                return temp;
            }
            else
                return default;
        }
    }
}