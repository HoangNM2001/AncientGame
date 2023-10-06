using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LeavesParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem fruitParticle;

    public void ChangeParticleColor(Color newColor)
    {
        var main = fruitParticle.main;
        main.startColor = new ParticleSystem.MinMaxGradient(newColor);
    }
}
