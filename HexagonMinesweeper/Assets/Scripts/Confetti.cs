using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Confetti : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The static instance of this script
    /// </summary>
    public static Confetti Instance;
    /// <summary>
    /// The particle system that is attached to this object
    /// </summary>
    private ParticleSystem particle;
    #endregion

    #region Methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        particle = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Play the particle system
    /// </summary>
    public void Play()
    {
        particle.Play();
    }
    #endregion
}
