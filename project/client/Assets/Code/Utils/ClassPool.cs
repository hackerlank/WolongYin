using System;
using System.Collections.Generic;
using System.Linq;


#region IPoolable
public interface IPoolable
{
    void Create();
    void New();
    void Delete();
}
#endregion

#region ObjectPool
public class ObjectPool
{
    #region Data Members
    private static Dictionary<System.Type, PoolableObject> pools = new Dictionary<Type, PoolableObject>();

    #endregion

    #region New

    public static T New<T>() where T : IPoolable, new()
    {
        T x = default(T);

        if (pools.ContainsKey(typeof(T)))
        {
            x = (T)pools[typeof(T)].Pop();
        }
        else
        {
            lock (pools)
            {
                pools[typeof(T)] = new PoolableObject(10);
            }
        }

        if (x == null)
        {
            x = new T();
            x.Create();
        }

        x.New();

        return x;
    }

    #endregion

    #region Delete
    public static void Delete<T>(T obj) where T : IPoolable
    {
        if (pools.ContainsKey(typeof(T)))
        {
            obj.Delete();
            pools[typeof(T)].Push(obj);
        }
        else
        {
            throw new Exception("ObjectPool.Delete can not be called for object which is not created using ObjectPool.New");
        }
    }

    #endregion

    #region Clear
    public static void Clear<T>() where T : IPoolable
    {
        lock (pools)
        {
            if (pools.ContainsKey(typeof(T)))
            {
                pools[typeof(T)].Clear();
                pools.Remove(typeof(T));
            }
        }
    }

    public static void Clear()
    {
        lock (pools)
        {
            foreach (PoolableObject po in pools.Values)
            {
                po.Clear();
            }

            pools.Clear();
        }
    }
    #endregion
}
#endregion

#region PoolableObject
public class PoolableObject
{
    #region Data Members
    private Stack<IPoolable> pool;

    #endregion

    #region Ctor
    public PoolableObject(int capacity)
    {
        pool = new Stack<IPoolable>(capacity);
    }
    #endregion

    #region Properties
    public Int32 Count
    {
        get { return pool.Count; }
    }
    #endregion

    #region Pop
    public IPoolable Pop()
    {
        lock (pool)
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }

            return null;
        }
    }
    #endregion

    #region Push
    public void Push(IPoolable obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException("Items added to a Pool cannot be null");
        }

        lock (pool)
        {
            pool.Push(obj);
        }
    }
    #endregion

    #region Clear
    public void Clear()
    {
        lock (pool)
        {
            pool.Clear();
        }
    }
    #endregion
}
#endregion
