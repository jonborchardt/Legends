using UnityEngine;

namespace Legends.Replay
{
    [RequireComponent(typeof(Animator))]
    public class AthleteAnimator : MonoBehaviour
    {
        Animator _animator;

        static readonly int IdleHash        = Animator.StringToHash("Idle");
        static readonly int RunHash         = Animator.StringToHash("Run");
        static readonly int CarryRunHash    = Animator.StringToHash("CarryRun");
        static readonly int StumbleHash     = Animator.StringToHash("Stumble");
        static readonly int CelebrateHash   = Animator.StringToHash("Celebrate");
        static readonly int FailHash        = Animator.StringToHash("Fail");

        void Awake() => _animator = GetComponent<Animator>();

        public void PlayIdle()       => _animator.SetTrigger(IdleHash);
        public void PlayRun()        => _animator.SetTrigger(RunHash);
        public void PlayCarryRun()   => _animator.SetTrigger(CarryRunHash);
        public void PlayStumble()    => _animator.SetTrigger(StumbleHash);
        public void PlayCelebrate()  => _animator.SetTrigger(CelebrateHash);
        public void PlayFail()       => _animator.SetTrigger(FailHash);
    }
}
