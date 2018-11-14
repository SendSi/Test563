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
        Debug.Log("不支持私有的,此程序集=" + Assembly.GetExecutingAssembly().FullName);
    }
    public void GetPerson()
    {
        Debug.Log("此程序集=" + Assembly.GetExecutingAssembly().FullName + ",类名=MyTestNotify");
    }

    public void SetPersonName(string str)
    {
        Debug.Log("传入的str=" + str);
    }
    public void SetPersonNameAndAge(int num,string str)
    {
        Debug.Log("传入的num=" + num+",str="+str);
    }
    public void SendTestNotify(TestDto dto)
    {

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





