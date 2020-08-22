//  This file is part of Polaris-Terminal - A developer console for Unity.
//  https://github.com/dynamiquel/Polaris-Options
//  Copyright (c) 2020 dynamiquel

//  MIT License
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:

//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.

//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

using System.Collections.Generic;

namespace Polaris.Terminal
{
    /// <summary>
    /// Custom version of a stack inherited from a List.
    /// Unlike a normal stack, this stack allows you to peek an n number of times.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
                var temp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return temp;
            }

            return default;
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
                
            return default;
        }
    }
}