using UnityEngine;

public class FootIK : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastDistance = 1.5f;
    [SerializeField] private float footOffset = 0.1f; // גובה כף הרגל מהמפרק עד הסוליה
    [SerializeField] private float ikWeight = 1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        HandleFoot(AvatarIKGoal.LeftFoot);
        HandleFoot(AvatarIKGoal.RightFoot);
    }

    void HandleFoot(AvatarIKGoal foot)
    {
        Vector3 footPos = animator.GetIKPosition(foot);
        Ray ray = new Ray(footPos + Vector3.up * raycastDistance * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer))
        {
            // מיקום - מגביה את הרגל לגובה הקרקע האמיתי
            Vector3 targetPos = hit.point + Vector3.up * footOffset;
            animator.SetIKPositionWeight(foot, ikWeight);
            animator.SetIKPosition(foot, targetPos);

            // סיבוב - מיישר את כף הרגל לפי שיפוע הקרקע
            Quaternion footRotation = Quaternion.FromToRotation(Vector3.up, hit.normal)
                                       * animator.GetIKRotation(foot);
            animator.SetIKRotationWeight(foot, ikWeight);
            animator.SetIKRotation(foot, footRotation);
        }
        else
        {
            animator.SetIKPositionWeight(foot, 0f);
            animator.SetIKRotationWeight(foot, 0f);
        }
    }
}
