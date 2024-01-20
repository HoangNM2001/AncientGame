using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class DirectionTutorial : GameComponent
{
    public int index;
    public bool active;
    public GameObject arrowTutorial;
    public PlayerControllerVariable player;

    private Coroutine _coroutine;
    private GameObject _arrow;
    private Vector3 _dir;
    private float _dis = 0f;
    private float _disMax = 10f;
    private float _timeCheckMin = 1f;
    private float _timeCheck = 0;
    private int _goldBounus = 100;
    private int _expBounus = 20;
    PlayerController _playerActor;

    public void Active()
    {
        
    }
}
