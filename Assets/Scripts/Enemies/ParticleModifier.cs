using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleModifier : MonoBehaviour
{
    public EnemyBase enemy;
    public ParticleSystem spores;
    public ParticleSystem.EmissionModule sporeEmission;
    
    // Start is called before the first frame update
    void Start()
    {
        enemy = this.gameObject.GetComponent<EnemyBase>();
        //spores = GetComponent<ParticleSystem>();
        sporeEmission = spores.emission;
        
    }

    // Update is called once per frame
    void Update()
    {
        ModifyParticles();
    }

    public void ModifyParticles()
    {
        //alert 
        //distracted 
        //frozen 
        //waiting

        if (enemy.alertStatus == true)
        {
            sporeEmission.rateOverTime = new ParticleSystem.MinMaxCurve(5f, 10f);
        }

        else if (enemy.distracted == true)
        {
            sporeEmission.rateOverTime = new ParticleSystem.MinMaxCurve(1f, 3f);
        }

        else if (enemy.freeze == true)
        {
            sporeEmission.rateOverTime = 0f;
        }

        else if (enemy.waiting == true)
        {
            sporeEmission.rateOverTime = new ParticleSystem.MinMaxCurve(1f, 2f);
        }

        else
        {
            sporeEmission.rateOverTime = 5f;
        }
    }
}
