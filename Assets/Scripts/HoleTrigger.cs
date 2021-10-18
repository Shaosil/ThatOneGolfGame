using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    private Collider _trigger;
    private ParticleSystem _effects;

    private void Start()
    {
        _trigger = GetComponent<Collider>();
        _effects = transform.parent.Find("Hole Fireworks").GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == GameManager.Ball.Collider)
        {
            // TODO: TADA
            _effects.Play();
        }
    }
}