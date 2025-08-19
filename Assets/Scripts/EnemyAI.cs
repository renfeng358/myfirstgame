using UnityEngine;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
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

        // Private state
        private Transform playerTransform;
        private AnimationController control;
        private float lastAttackTime;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            // Find the player by tag. This is not the most efficient way, but it's simple.
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }

        void Update()
        {
            // If we don't have a player, we can't do anything smart.
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
                // If there's no path, just stand still.
                control.move.x = 0;
            }
        }

        void CheckForPlayer()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer < detectionRange)
            {
                // A simple line-of-sight check.
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
                // Player is out of range, go back to patrolling.
                currentState = AIState.Patrolling;
                return;
            }

            if (distanceToPlayer < attackRange)
            {
                // Player is in attack range.
                currentState = AIState.Attacking;
                return;
            }

            // Move towards the player.
            float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            control.move.x = direction;
        }

        void Attack()
        {
            // Stop moving to attack.
            control.move.x = 0;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > attackRange)
            {
                // Player moved out of attack range, chase them.
                currentState = AIState.Chasing;
                return;
            }

            // Check if we can attack (cooldown).
            if (Time.time > lastAttackTime + attackCooldown)
            {
                // Perform the attack.
                // For now, a simple damage call. In a real game, this would trigger an animation.
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
