using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 消息类型
/// </summary>
public class MessageType : MonoBehaviour
{
    public struct Test
    {
        public string data;
    }

    public struct IntTest
    {
        public int Value;
    }
    public struct InTest2
    {
        public int Value;
    }
    public struct GameSceneCreating
    {
        public CreateSceneData data;
    }

    public struct GameSceneCreated
    {
        public CreateSceneData data;
    }

    public struct Login
    {

    }
}



public struct CreateSceneData
{
    public bool showLoading;
    public Func<Task> loadingTask;
}