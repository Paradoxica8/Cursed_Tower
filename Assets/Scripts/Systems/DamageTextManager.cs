using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    [Header("Prefab & Settings")]
    [SerializeField] private GameObject damageTextPrefab;

    private void Awake() => Instance = this;

    private void OnEnable()
    {
        GameEvents.OnDamageDealt += SpawnDamageText;
    }

    private void OnDisable()
    {
        GameEvents.OnDamageDealt -= SpawnDamageText;
    }

    private void SpawnDamageText(double damage, bool isClick)
    {
        if (damageTextPrefab == null) return;

        Enemy currentEnemy = TowerManager.Instance != null ? TowerManager.Instance.GetCurrentEnemy() : null;
        if (currentEnemy == null) return;

        Vector3 spawnPos = currentEnemy.transform.position;

        GameObject obj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);
        DamageText damageText = obj.GetComponent<DamageText>();

        if (damageText != null)
        {
            damageText.Setup(damage, isClick, spawnPos);
        }
    }

    public void Setup(double damage, bool isClick, Vector3 spawnPos)
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = "Default";
            mr.sortingOrder = 10;
        }

        Vector3 offset = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.2f, 0.3f), 0);
        transform.position = spawnPos + offset;
    }
}