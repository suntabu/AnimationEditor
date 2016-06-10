using UnityEngine;
using System.Collections;
//using TNet;
using System.Collections.Generic;
using System;
using System.Reflection;
using Inlycat;

namespace Inlycat
{
    public class CoroutineRunner : MonoBehaviour
    {
        #region helper methods

        public static IEnumerator ActionOneByOne(List<Action> actions)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                action();
                yield return null;
            }
        }

        public static IEnumerator ActionOneByOneWithDelay(List<Action> actions, float delayTime)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                action();
                yield return new WaitForSeconds(delayTime);
            }
        }

        /// <summary>
        /// some action do after delay
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        public static IEnumerator ActionAfterDelay(Action action, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            action();
        }

        /// <summary>
        /// Generic version
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="delayTime"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IEnumerator ActionAfterDelay<T>(Action<T> action, float delayTime, T para)
        {
            yield return new WaitForSeconds(delayTime);
            action(para);
        }

        public static IEnumerator DelayBetweenActions(Action a, float delayTime, Action b)
        {
            a();
            yield return new WaitForSeconds(delayTime);
            b();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">return a bool decide the loop whether need to be breaked.</param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static IEnumerator InfiniteDoOneAction(Func<bool> action, float interval)
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                var isKeeping = action();
                if (!isKeeping)
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Every frame do an action untill the action return false
        /// To be remind is as each frame time may be not the same, so you'd better multiply Time.deltaTime at your delta value
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerator EveryFrameDoOneAction(Func<bool> action)
        {
            while (true)
            {
                yield return null;
                var isKeeping = action();
                if (!isKeeping)
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Remember dispose the www.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onSucceed"></param>
        /// <returns></returns>
        public static IEnumerator WWWLoader(string path, Action<WWW> onSucceed)
        {
            WWW www = new WWW(path);
            yield return www;
            onSucceed(www);
        }

        #endregion
    }


    /// <summary>
    /// background coroutine manger, for some tasks like large count of files to download or load...
    /// 
    /// </summary>
    public class CoroutineManager : Singleton<CoroutineManager>
    {
        /// <summary>
        /// Max running coroutines num 
        /// </summary>
        public const int MaxRunningCoroutinesCount = 5;

        private CoroutineRunner m_Hoster;


        private CoroutineManager()
        {
            m_Hoster = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
             
            GameObject.DontDestroyOnLoad(m_Hoster);
        }


        public void DoAction(List<Action> actionList)
        {
            m_Hoster.StartCoroutine(CoroutineRunner.ActionOneByOne(actionList));
        }

        public void DoActionWithDelay(List<Action> actionList, float delay)
        {
            m_Hoster.StartCoroutine(CoroutineRunner.ActionOneByOneWithDelay(actionList, delay));
        }

        public void DoAction(IEnumerator coroutine)
        {
            m_Hoster.StartCoroutine(coroutine);
        }
    }


    /// <summary>
    /// Author:gouzhun 
    ///  thread safe singleton base class for inheriting.
    /// </summary>
    /// <typeparam name="T">the generic type</typeparam>
    public class Singleton<T> where T : class
    {
        static object _lock = new object();

        private volatile static T _instance;

        /// <summary>
        /// singleton instance, the object is created when this api first called.
        /// so pay attention to the first calling if you want to do some performance improvement.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            ConstructorInfo constructor = null;
                            // Binding flags exclude public constructors.
                            constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);

                            if (constructor == null || constructor.IsAssembly)
                            {
                                throw new Exception(string.Format("A private or " + "protected constructor is missing for '{0}'.", typeof(T).Name));
                            }
                            // Also exclude internal constructors.
                            _instance = (T)constructor.Invoke(null);
                        }
                    }
                }
                return _instance;
            }

        }

        static Singleton()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public void DestroyInstance()
        {
            _instance = null;
        }

        public virtual void Clear()
        {
            throw new NotImplementedException("Clear methods is not implemented");
        }

    }
}