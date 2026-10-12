using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private float moveSpeed = 1.4f;
    [SerializeField] private float fadeDuration = 0.6f;

    private Color initialColor;
    private float timer;

    public void Setup(double damage, bool isClick, Vector3 spawnPos)
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) { mr.sortingOrder = 10; }

        Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0.7f, 1.1f), 0);
        transform.position = spawnPos + offset;

        if (textMesh != null)
        {
            textMesh.text = NumberFormatter.Format(damage);

            textMesh.enableVertexGradient = false;
            textMesh.overrideColorTags = false;

            if (isClick)
            {
                initialColor = new Color(1f, 0.8f, 0f, 1f);
                textMesh.fontSize = 6f;
            }
            else
            {
                initialColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                textMesh.fontSize = 3.5f;
            }

            textMesh.color = initialColor;
            textMesh.ForceMeshUpdate();
        }

        timer = fadeDuration;
    }

    private void Update()
    {
        transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (textMesh != null)
        {
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            textMesh.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
        }

        if (timer <= 0) Destroy(gameObject);
    }
}