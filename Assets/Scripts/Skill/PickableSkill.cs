using UnityEngine;

public class PickableSkill : MonoBehaviour
{
    [Header("Skill Prefab")]
    public GameObject skillPrefab;       // The actual skill prefab to spawn/equip

    [Header("Debug")]
    public bool debugMode = true;

    private bool picked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (picked) return;

        if (other.CompareTag("Player"))
        {
            Transform playerTransform = other.transform;

            // Find or spawn skill
            if (skillPrefab != null)
            {
                GameObject skillInstance = Instantiate(skillPrefab);
                SkillBase skill = skillInstance.GetComponent<SkillBase>();
                if (skill != null)
                {
                    skill.Equip(playerTransform);

                    if (debugMode)
                        Debug.Log($"{skill.name} picked and equipped by {other.name}");
                }
                else if (debugMode)
                {
                    Debug.LogWarning("PickableSkill: skillPrefab has no SkillBase component!");
                }

                picked = true;
                // Optionally disable or destroy pickup object
                gameObject.SetActive(false);
            }
        }
    }
}
