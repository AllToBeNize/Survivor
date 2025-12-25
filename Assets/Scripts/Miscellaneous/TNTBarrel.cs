using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class TNTBarrel : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionDamage = 50f;
    public float explosionDelay = 1.5f;
    public LayerMask damageLayer;

    [Header("Push Settings")]
    public float pushForce = 5f;
    public float upwardModifier = 0.5f;

    [Header("Visuals & FX")]
    public GameObject explosionEffect;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(ExplosionRoutine());
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) continue;

            var attr = hit.GetComponent<AttributeBase>();
            if (attr != null && attr.IsAlive)
            {
                DamageInfo dmg = new DamageInfo(explosionDamage, gameObject);
                attr.TakeDamage(dmg);
            }

            NavMeshAgent agent = hit.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                Vector3 push = dir * pushForce + Vector3.up * pushForce * upwardModifier;
                agent.Move(push * 0.1f);
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(hit.transform.position, transform.position);
                float force = Mathf.Lerp(pushForce * 10f, 0, distance / explosionRadius);
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
        }

        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
