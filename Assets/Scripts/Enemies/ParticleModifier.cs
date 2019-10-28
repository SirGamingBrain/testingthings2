using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleModifier : MonoBehaviour
{
    public EnemyBase enemy;
    public ParticleSystem spores;
    public ParticleSystem.MainModule sporeMain;
    public ParticleSystem.EmissionModule sporeEmission;
    public ParticleSystem.ColorOverLifetimeModule sporeColor;

    private Gradient sporeAlertGradient;
    private Gradient sporeWaitingGradient;

    // Start is called before the first frame update
    void Start()
    {
        enemy = this.gameObject.GetComponent<EnemyBase>();

        sporeMain = spores.main;

        sporeEmission = spores.emission;

        float alpha = 1.0f;
        sporeColor = spores.colorOverLifetime;
        sporeColor.color = Color.white;
        sporeAlertGradient = new Gradient();
        sporeAlertGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.red, 1.0f), new GradientColorKey(Color.red, 0.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f), new GradientAlphaKey(alpha, 0.0f) });
        sporeWaitingGradient = new Gradient();
        sporeWaitingGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.yellow, 1.0f), new GradientColorKey(Color.yellow, 0.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f), new GradientAlphaKey(alpha, 0.0f) });
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
            sporeColor.color = sporeAlertGradient;
            sporeMain.maxParticles = 8;
        }

        else if (enemy.distracted == true)
        {
            sporeEmission.rateOverTime = new ParticleSystem.MinMaxCurve(1f, 3f);
            sporeColor.color = sporeWaitingGradient;
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
            sporeColor.color = Color.white;
            sporeMain.maxParticles = 5;
        }
    }
}
