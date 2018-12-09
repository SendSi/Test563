using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 反射测试类
/// </summary>
public class MyTestNotify
{
    public MyTestNotify()
    {
        if (PlayerPrefs.HasKey(GetType().Name)==false)
            Debug.Log("此程序集=" + Assembly.GetExecutingAssembly().FullName);
        PlayerPrefs.SetString(GetType().Name,GetType().Name);
    }
    public void GetPerson()
    {
        Debug.Log("此程序集=" + Assembly.GetExecutingAssembly().FullName + ",类名=MyTestNotify");
    }
    private void GetPersonPrivate()
    {
        Debug.Log("私有的");
    }

    public void SetPersonName(string strstrstrstrstrstrstrstrstrstr)
    {
        Debug.Log("传入的str=" + strstrstrstrstrstrstrstrstrstr);
    }
    public void SetPersonNameAndAge(int num, string str, TestDto dto, TestDto dto2)
    {
        Debug.Log("传入的num=" + num + ",strstrstrstrstrstrstrstrstrstr=" + str + ",dto.mid=" + dto.mId + ",mName=" + dto.mName);
    }

    public void SetPerson(int age, bool isMan, string name)
    {
        Debug.Log(name + "," + age + (isMan ? "岁,男" : "岁,女"));
    }

    public void SendTestNotify(TestDto dto, TestDto dto2)
    {
        Debug.Log(dto.mId + "," + dto2.mName);//+","+dto.mName);
    }
}
public class TestDto
{
    //火id
    public long mId;
    //火名
    public string mName;
    //大小
    public int mRange;
    //全局否
    public bool mGlobal;
}

