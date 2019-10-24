using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Confetti : MonoBehaviour
{

    public static Confetti Instance;

    private ParticleSystem particle;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        particle = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        particle.Play();
    }
}
