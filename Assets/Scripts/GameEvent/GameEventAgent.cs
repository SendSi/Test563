using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameEventAgent : IGameEventAgent
{
    public void RemoveListener(object obj)
    {
    }

    private static GameEventAgent instance;
    public static GameEventAgent Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameEventAgent();
                GameEventCenter.Instance.AddGameEventAgent(instance);
            }
            return instance;
        }
    }

    Dictionary<EnumEventName, GameEventDelegate> mEventList = new Dictionary<EnumEventName, GameEventDelegate>();

    public void AddListener(EnumEventName pEnum, Action pAct)
    {
        GameEventDelegate gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate) == false)
        {
            gameDelegate = new GameEventDelegate();
            mEventList.Add(pEnum, gameDelegate);
        }
        gameDelegate.Add(pAct);
    }
    public void RemoveListener(EnumEventName pEnum, Action pAct)
    {
        GameEventDelegate gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate))
        {
            gameDelegate.Remove(pAct);
        }
    }
    public void SendEvent(EnumEventName pEnum)
    {
        GameEventDelegate gameDelegate;
        if (mEventList.TryGetValue(pEnum, out gameDelegate))
        {
            gameDelegate.Send();
        }
    }
}


public class GameEventDelegate
{
    private Action mAction;
    private bool mNeedUpdate;
    private Delegate[] mDelegateList;

    public void Send()
    {
        if (mAction == null) return;
        ChectUpdate();
        if (mDelegateList == null) return;
        for (int i = 0; i < mDelegateList.Length; i++)
        {
            try
            {
                var tAct = mDelegateList[i] as Action;
                tAct();
            }
            catch (Exception e)
            {
                Debug.LogError(e);                
            }
        }
    }

    public void Add(Action pAct)
    {
        mAction -= pAct;//防止重复了...
        mAction += pAct;
        mNeedUpdate = true;
    }

    public void Remove(Action pAct)
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

public class Events
{
    public readonly EnumEventName mEventName;
    public Events(EnumEventName eventName)
    {
        mEventName = eventName;
    }
}



