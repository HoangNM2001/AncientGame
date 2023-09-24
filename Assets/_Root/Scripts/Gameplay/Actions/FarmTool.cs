using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class FarmTool : GameComponent
{
    [SerializeField] private List<ParticleSystem> particleList;

    private ParticleSystem currentParticle;

    public void UpdateTool(int currentLevel)
    {
        if (currentParticle != null)
        {
            currentParticle.gameObject.SetActive(false);
        }

        if (particleList.Count > 0)
        {
            currentParticle = particleList[currentLevel - 1];
            currentParticle.gameObject.SetActive(true);

        }
    }

    public void PlayFarmParticle()
    {
        currentParticle.Play();
    }
}
