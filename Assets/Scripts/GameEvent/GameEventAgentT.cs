using UnityEngine;
using System.Collections.Generic;
using System;

public class GameEventAgentT<T> : IGameEventAgent
{
    public void RemoveListener(object obj)
    {
    }

    private static GameEventAgentT<T> instance;
    public static GameEventAgentT<T> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameEventAgentT<T>();
                //GameEventCenter.Instance.AddGameEventAgent(instance);
            }
            return instance;
        }
    }

    Dictionary<EnumEventName, GameEventDelegateT<T>> mEventList = new Dictionary<EnumEventName, GameEventDelegateT<T>>();

    public void AddListener(EnumEventName pEnum, Action<T> pAct)
    {
        GameEventDelegateT<T> gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate) == false)
        {
            gameDelegate = new GameEventDelegateT<T>();
            mEventList.Add(pEnum, gameDelegate);
        }
        gameDelegate.Add(pAct);
    }
    public void RemoveListener(EnumEventName pEnum, Action<T> pAct)
    {
        GameEventDelegateT<T> gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate))
        {
            gameDelegate.Remove(pAct);
        }
    }
    public void SendEvent(EnumEventName pEnum,T pPars)
    {
        GameEventDelegateT<T> gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate))
        {
            gameDelegate.Send(pPars);
        }
    }
}


public class GameEventDelegateT<T>
{
    private Action<T> mAction;
    private bool mNeedUpdate;
    private Delegate[] mDelegateList;

    public void Send(T pPars)
    {
        if (mAction == null) return;
        ChectUpdate();
        if (mDelegateList == null) return;
        for (int i = 0; i < mDelegateList.Length; i++)
        {
            try
            {
                var tAct = mDelegateList[i] as Action<T>;
                tAct(pPars);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    public void Add(Action<T> pAct)
    {
        mAction -= pAct;//防止重复了...
        mAction += pAct;
        mNeedUpdate = true;
    }

    public void Remove(Action<T> pAct)
    {
        mAction -= pAct;
        mNeedUpdate = true;
    }
    void ChectUpdate()
    {
        if (mNeedUpdate)
        {
            mNeedUpdate = false;
            if (mAction != null)
                mDelegateList = mAction.GetInvocationList();
            else
                mDelegateList = null;
        }
    }

}



public class Events<T>
{
    public readonly EnumEventName mEventName;
    public Events(EnumEventName eventName)
    {
        mEventName = eventName;
    }
}
