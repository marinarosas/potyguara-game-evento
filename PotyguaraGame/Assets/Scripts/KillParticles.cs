using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillParticles : MonoBehaviour
{
    [SerializeField] private float onExitRemainingLifetime;
    private new ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
        if(particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, particles) <= 0)
        {
            return;
        }
        ParticleSystem.Particle[] particlesArray = particles.ToArray();
        for(int ii=0; ii<particlesArray.Length; ii++)
        {
            if (particlesArray[ii].remainingLifetime <= onExitRemainingLifetime)
            {
                continue;
            }
            particlesArray[ii].remainingLifetime = onExitRemainingLifetime;

            particles = new List<ParticleSystem.Particle>(particlesArray);
            ParticlePhysicsExtensions.SetTriggerParticles(particleSystem, ParticleSystemTriggerEventType.Exit, particles);
        }
    }
}
