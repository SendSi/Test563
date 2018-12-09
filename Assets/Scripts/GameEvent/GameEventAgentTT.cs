using UnityEngine;
using System.Collections.Generic;
using System;

public class GameEventAgentTT<T,T2> : IGameEventAgent
{
    public void RemoveListener(object obj)
    {
    }

    private static GameEventAgentTT<T,T2> instance;
    public static GameEventAgentTT<T,T2> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameEventAgentTT<T, T2>();
                //GameEventCenter.Instance.AddGameEventAgent(instance);
            }
            return instance;
        }
    }

    Dictionary<EnumEventName, GameEventDelegateTT<T, T2>> mEventList = new Dictionary<EnumEventName, GameEventDelegateTT<T, T2>>();

    public void AddListener(EnumEventName pEnum, Action<T, T2> pAct)
    {
        GameEventDelegateTT<T, T2> gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate) == false)
        {
            gameDelegate = new GameEventDelegateTT<T, T2>();
            mEventList.Add(pEnum, gameDelegate);
        }
        gameDelegate.Add(pAct);
    }
    public void RemoveListener(EnumEventName pEnum, Action<T, T2> pAct)
    {
        GameEventDelegateTT<T, T2> gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate))
        {
            gameDelegate.Remove(pAct);
        }
    }
    public void SendEvent(EnumEventName pEnum, T pPars, T2 pPars2)
    {
        GameEventDelegateTT<T, T2> gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate))
        {
            gameDelegate.Send(pPars,pPars2);
        }
    }
}


public class GameEventDelegateTT<T, T2>
{
    private Action<T, T2> mAction;
    private bool mNeedUpdate;
    private Delegate[] mDelegateList;

    public void Send(T  pPars,T2 pPars2)
    {
        if (mAction == null) return;
        ChectUpdate();
        if (mDelegateList == null) return;
        for (int i = 0; i < mDelegateList.Length; i++)
        {
            try
            {
                var tAct = mDelegateList[i] as Action<T, T2>;
                tAct(pPars,pPars2);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    public void Add(Action<T, T2> pAct)
    {
        mAction -= pAct;//防止重复了...
        mAction += pAct;
        mNeedUpdate = true;
    }

    public void Remove(Action<T, T2> pAct)
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



public class Events<T,T2>
{
    public readonly EnumEventName mEventName;
    public Events(EnumEventName eventName)
    {
        mEventName = eventName;
    }
}
