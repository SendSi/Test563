using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParamsNotify
{
    public void HandleNotify(TestDto dto)
    {

#if EDITOR_LOG
        Debug.Log("收到消息:" + dto.mId + "," + dto.mName);
#endif

    }
    

}














