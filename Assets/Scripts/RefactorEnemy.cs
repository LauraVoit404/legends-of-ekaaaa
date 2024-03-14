using UnityEngine;

public class RefactorEnemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats
    {
        public float walkSpeed;
        public float rotateSpeed;
        public float chaseSpeed;
        public bool idle;
        public float explodeDist;
        public int currentPatrolPoint = 0;
    }

    public EnemyStats enemyStats;
    public Transform sight;
    public Transform[] patrolPoints;
    public GameObject enemyExplosionParticles;

    private bool isChasing = false;
    private GameObject player;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isChasing)
            ChasePlayer();
        else
            Patrol();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        Vector3 moveToPoint = patrolPoints[enemyStats.currentPatrolPoint].position;
        transform.position = Vector3.MoveTowards(transform.position, moveToPoint, enemyStats.walkSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, moveToPoint) < 0.01f)
            NextPatrolPoint();
    }

    private void NextPatrolPoint()
    {
        enemyStats.currentPatrolPoint = (enemyStats.currentPatrolPoint + 1) % patrolPoints.Length;
    }

    private void ChasePlayer()
    {
        if (player == null)
            return;

        sight.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(sight);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * enemyStats.chaseSpeed);

        if (Vector3.Distance(transform.position, player.transform.position) < enemyStats.explodeDist)
            Explode();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 9)
            Slip();
    }

    private void Slip()
    {
        transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartChasing(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            StopChasing();
    }

    private void StartChasing(GameObject target)
    {
        player = target;
        enemyStats.idle = false;
        isChasing = true;
    }

    private void StopChasing()
    {
        enemyStats.idle = true;
        isChasing = false;
    }

    private void Explode()
    {
        Instantiate(enemyExplosionParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
