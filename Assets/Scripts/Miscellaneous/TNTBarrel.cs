using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TNTBarrel : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionDamage = 50f;
    public float explosionDelay = 1.5f;
    public LayerMask damageLayer; // Layers that can be damaged

    [Header("Visuals & FX")]
    public GameObject explosionEffect;

    private bool activated = false;
    private bool triggerByPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            triggerByPlayer = true;
            activated = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(ExplosionRoutine());
        }
        //else if (other.CompareTag("Enemy"))
        //{
        //    activated = true;
        //    GetComponent<Collider>().enabled = false;

        //    // π÷ŒÔ¥•≈ˆ¡¢º¥±¨’®
        //    StartCoroutine(ExplosionRoutine());
        //}
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
            // ÕÊº“≤ª ‹…À∫¶
            if (hit.CompareTag("Player")) continue;

            var attr = hit.GetComponent<AttributeBase>();
            if (attr != null && attr.IsAlive)
            {
                DamageInfo dmg = new DamageInfo(explosionDamage, gameObject);
                attr.TakeDamage(dmg);
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
