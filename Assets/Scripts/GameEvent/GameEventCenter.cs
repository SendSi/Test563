using UnityEngine;
using System.Collections;
using System;


public class GameEventCenter
{
    private static GameEventCenter instance;
    public static GameEventCenter Instance
    {
        get
        {
            if (instance == null)
                instance = new GameEventCenter();
            return instance;
        }
    }
    public void AddGameEventAgent(GameEventAgent gameAgent)
    {
    }

    #region 无参
    public static void AddListener(Events pEvent, Action pAct)
    {
        GameEventAgent.Instance.AddListener(pEvent.mEventName, pAct);
    }

    public static void RemoveListener(Events pEvent, Action pAct)
    {
        GameEventAgent.Instance.RemoveListener(pEvent.mEventName, pAct);
    }

    public static void SendEvent(Events pEvent)
    {
        GameEventAgent.Instance.SendEvent(pEvent.mEventName);
    } 
    #endregion

    #region 一个T
    public static void AddListener<T>(Events<T> pEvent, Action<T> pAct)
    {
        GameEventAgentT<T>.Instance.AddListener(pEvent.mEventName, pAct);
    }

    public static void RemoveListener<T>(Events<T> pEvent, Action<T> pAct)
    {
        GameEventAgentT<T>.Instance.RemoveListener(pEvent.mEventName, pAct);
    }

    public static void SendEvent<T>(Events<T> pEvent,T pPars)
    {
        GameEventAgentT<T>.Instance.SendEvent(pEvent.mEventName,pPars);
    } 
    #endregion

    #region T,T
    public static void AddListener<T, T2>(Events<T, T2> pEvent, Action<T, T2> pAct)
    {
        GameEventAgentTT<T, T2>.Instance.AddListener(pEvent.mEventName, pAct);
    }

    public static void RemoveListener<T, T2>(Events<T, T2> pEvent, Action<T, T2> pAct)
    {
        GameEventAgentTT<T, T2>.Instance.RemoveListener(pEvent.mEventName, pAct);
    }

    public static void SendEvent<T, T2>(Events<T, T2> pEvent, T pPars, T2 pPars2)
    {
        GameEventAgentTT<T, T2>.Instance.SendEvent(pEvent.mEventName, pPars,pPars2);
    }
    #endregion



}


public enum EnumEventName
{
    Back_Setup,
    TestInt,
    TestIntString
}

public static class EventNames
{
    public static readonly Events Back_Setup = new Events(EnumEventName.Back_Setup);
    public static readonly Events<int> TestInt = new Events<int>(EnumEventName.TestInt);
    public static readonly Events<int, string> TestIntString = new Events<int,string>(EnumEventName.TestIntString);

}