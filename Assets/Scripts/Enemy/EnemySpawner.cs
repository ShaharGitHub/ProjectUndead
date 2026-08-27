using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private Transform m_followTarget;
    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private Transform m_spawnPoint;

    [Header("Settings:")]
    [SerializeField] private float m_spawnInitDelay;
    [SerializeField] private float m_spawnRepeatTime;

    private Coroutine m_spawnCoroutine;


    private void Start()
    {
        StartEnemySpawn();
    }

    public void StartEnemySpawn()
    {
        if (m_spawnCoroutine != null)
            return;

        m_spawnCoroutine = StartCoroutine(SpawnEnemyRoutine(m_spawnInitDelay, m_spawnRepeatTime));
    }

    public void StopEnemySpawn()
    {
        if (m_spawnCoroutine == null)
            return;

        StopCoroutine(m_spawnCoroutine);
        m_spawnCoroutine = null;
    }

    IEnumerator SpawnEnemyRoutine(float delay = 0, float repeatTime = -1)
    {
        // Allow delay if have
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        do
        {
            SpawnEnemy();

            // Stop spawning if no repeat time
            if (repeatTime < 0)
                break;

            // if there is a repeat time, use it
            yield return new WaitForSeconds(repeatTime);
        }
        while (true);
    }

    private void SpawnEnemy()
    {
        if (m_enemyPrefab == null || m_followTarget == null || m_spawnPoint == null)
            return;

        GameObject enemy = null;
        enemy = Instantiate(m_enemyPrefab, m_spawnPoint.position, Quaternion.identity);
        if (enemy == null)
            return;

        if (enemy.TryGetComponent<EnemyManager>(out EnemyManager enemyManager))
        {
            enemyManager.SetFollowTarget(m_followTarget);
        }

    }
}
