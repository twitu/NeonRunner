using System;
public class CircularQueue<T>
{
    T[] buffer;
    int nextFree;
    int queueLength;
    
    public CircularQueue(int length) {
        nextFree = 0;
        buffer = new T[length];
    }

    public CircularQueue(int length, T[] objects)
    {
        nextFree = 0;
        buffer = new T[length];
        Array.Copy(objects, 0, buffer, 0, length);
    }

    public T[] GetBuffer() {
        return buffer;
    }

    public T Add(T o)
    {
        T obj = buffer[nextFree];
        buffer[nextFree] = o;
        nextFree = (nextFree + 1) % buffer.Length;
        return obj;
    }

    public T Peek()
    {
        return buffer[nextFree];
    }
}
