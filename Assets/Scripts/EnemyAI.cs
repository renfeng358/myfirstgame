using UnityEngine;
using Platformer.Mechanics;

namespace Platformer.Mechanics
{
    public class EnemyAI : MonoBehaviour
    {
        public float attackRange = 1.5f;
        public float chaseRange = 5f;
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4f;

        private Transform player;
        private EnemyController enemyController;
        private PatrolPath.Mover mover;

        private enum State
        {
            Patrolling,
            Chasing,
            Attacking
        }

        private State currentState;

        void Awake()
        {
            enemyController = GetComponent<EnemyController>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Start()
        {
            currentState = State.Patrolling;
            if (enemyController.path != null)
            {
                mover = enemyController.path.CreateMover(patrolSpeed);
            }
        }

        void Update()
        {
            switch (currentState)
            {
                case State.Patrolling:
                    Patrol();
                    break;
                case State.Chasing:
                    Chase();
                    break;
                case State.Attacking:
                    Attack();
                    break;
            }
        }

        private void Patrol()
        {
            if (Vector2.Distance(transform.position, player.position) < chaseRange)
            {
                currentState = State.Chasing;
                return;
            }

            if (mover != null)
            {
                enemyController.control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

        private void Chase()
        {
            if (Vector2.Distance(transform.position, player.position) > chaseRange)
            {
                currentState = State.Patrolling;
                // Reset speed to patrol speed
                if (mover != null)
                {
                    // This is a simplification. The Mover class doesn't have a speed property.
                    // For now, we will just use the EnemyController's maxSpeed.
                }
                return;
            }

            if (Vector2.Distance(transform.position, player.position) < attackRange)
            {
                currentState = State.Attacking;
                return;
            }

            // Move towards the player
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            enemyController.control.move.x = direction;
        }

        private void Attack()
        {
            // Simple attack logic: just stop moving
            enemyController.control.move.x = 0;

            // Add your attack logic here (e.g., play an attack animation, deal damage)
            Debug.Log("Attacking player!");

            if (Vector2.Distance(transform.position, player.position) > attackRange)
            {
                currentState = State.Chasing;
            }
        }

        void OnDrawGizmosSelected()
        {
            // Draw chase range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);

            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
