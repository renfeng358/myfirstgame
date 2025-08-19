using UnityEngine;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(AnimationController), typeof(Collider2D), typeof(Health))]
    public class EnemyAI : MonoBehaviour
    {
        public enum AIState
        {
            Patrolling,
            Chasing,
            Attacking
        }

        [Header("Patrol")]
        public PatrolPath path;
        internal PatrolPath.Mover mover;

        [Header("AI Settings")]
        public AIState currentState = AIState.Patrolling;
        public float detectionRange = 5f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1f;

        // Public reference to the loader, set on spawn
        public LevelLoader loader;

        // Private state
        private Transform playerTransform;
        private AnimationController control;
        private float lastAttackTime;
        private Health health;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            health = GetComponent<Health>();
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }

        void Update()
        {
            if (health != null && !health.IsAlive)
            {
                // Report death and disable self
                if (loader != null)
                {
                    loader.EnemyDied(gameObject);
                }
                this.enabled = false;
                return;
            }

            if (playerTransform == null) return;

            switch (currentState)
            {
                case AIState.Patrolling:
                    Patrol();
                    CheckForPlayer();
                    break;
                case AIState.Chasing:
                    Chase();
                    break;
                case AIState.Attacking:
                    Attack();
                    break;
            }
        }

        void Patrol()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
            else
            {
                control.move.x = 0;
            }
        }

        void CheckForPlayer()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer < detectionRange)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (playerTransform.position - transform.position).normalized, detectionRange);
                if (hit.collider != null && hit.collider.transform == playerTransform)
                {
                    currentState = AIState.Chasing;
                }
            }
        }

        void Chase()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > detectionRange)
            {
                currentState = AIState.Patrolling;
                return;
            }

            if (distanceToPlayer < attackRange)
            {
                currentState = AIState.Attacking;
                return;
            }

            float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            control.move.x = direction;
        }

        void Attack()
        {
            control.move.x = 0;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > attackRange)
            {
                currentState = AIState.Chasing;
                return;
            }

            if (Time.time > lastAttackTime + attackCooldown)
            {
                var playerHealth = playerTransform.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.Decrement();
                }
                lastAttackTime = Time.time;
            }
        }
    }
}
