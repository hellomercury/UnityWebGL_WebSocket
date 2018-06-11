using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Net;
using UnityEngine;
using UnityEngine.Profiling;

public class Test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Stopwatch sp = new Stopwatch();
        sp.Start();
        Profiler.BeginSample("<----------------------------->");

        for (int i = 0; i < 10000; ++i)
        {
            NetProtocolEnum.Q3RDAuthWebGL.ToString();
        }
        Profiler.EndSample();
        sp.Stop();
        UnityEngine.Debug.LogError(sp.ElapsedMilliseconds);

        Stopwatch sp1 = new Stopwatch();
        sp1.Start();
        Profiler.BeginSample(">-----------------------------<");
        for (int i = 0; i < 10000; ++i)
        {
            NetProtocolEnum.Q3RDAuthWebGL.ToCurString();
        }
        Profiler.EndSample();
        sp1.Stop();
        UnityEngine.Debug.LogError(sp1.ElapsedMilliseconds);

    }



    // Update is called once per frame
    void Update()
    {

    }
}

public static class co
{
    public static string ToCurString(this NetProtocolEnum InNetProtocolEnum)
    {
        switch (InNetProtocolEnum)
        {
            case NetProtocolEnum.Q3RDAuthWebGL:
                return "Q3RDAuthWebGL";
            case NetProtocolEnum.Q3RDWSEnter:
                return "Q3RDWSEnter";
            case NetProtocolEnum.QDataUpdate:
                return "QDataUpdate";
            case NetProtocolEnum.QCustomsDataPut:
                return "QCustomsDataPut";
            case NetProtocolEnum.QPing:
                return "QPing";
            default:
                return "";
        }
    }
}
