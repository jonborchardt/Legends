using UnityEngine;
using Legends.Data;

namespace Legends.Replay
{
    public class ReplayCameraController : MonoBehaviour
    {
        [SerializeField] ReplayDirector director;
        [SerializeField] float smoothTime            = 0.3f;
        [SerializeField] Vector3 followOffset        = new Vector3(-8f, 4f, 0f);
        [SerializeField] Vector3 focusOffset         = new Vector3(-4f, 3f, 2f);
        [SerializeField] Vector3 wideShotPosition    = new Vector3(-15f, 8f, 0f);
        [SerializeField] float focusDuration         = 1.5f;

        enum CameraMode { FollowLeader, FocusAthlete, WideShot }

        CameraMode _mode = CameraMode.FollowLeader;
        Vector3    _velocity;
        Transform  _focusTarget;
        float      _focusTimer;
        bool       _wideShotTriggered;

        void OnEnable()
        {
            if (director != null)
                director.OnReplayEventFired += HandleEvent;
        }

        void OnDisable()
        {
            if (director != null)
                director.OnReplayEventFired -= HandleEvent;
        }

        void HandleEvent(ReplayEvent evt)
        {
            if (evt.EventType == EventTypes.EggDrop)
            {
                var target = director.GetTransformForAthlete(evt.AthleteId);
                if (target != null)
                {
                    _focusTarget = target;
                    _focusTimer  = focusDuration;
                    _mode = CameraMode.FocusAthlete;
                }
            }

            if (evt.EventType == EventTypes.Finish && !_wideShotTriggered)
            {
                _wideShotTriggered = true;
                _mode = CameraMode.WideShot;
            }
        }

        void Update()
        {
            switch (_mode)
            {
                case CameraMode.FollowLeader: UpdateFollowLeader(); break;
                case CameraMode.FocusAthlete: UpdateFocusAthlete(); break;
                case CameraMode.WideShot:     UpdateWideShot();     break;
            }
        }

        void UpdateFollowLeader()
        {
            var leaderTransform = director.GetLeaderTransform();
            if (leaderTransform == null) return;

            Vector3 targetPos = leaderTransform.position + followOffset;
            transform.position = Vector3.SmoothDamp(
                transform.position, targetPos, ref _velocity, smoothTime);
            transform.LookAt(leaderTransform.position);
        }

        void UpdateFocusAthlete()
        {
            if (_focusTarget == null) { _mode = CameraMode.FollowLeader; return; }

            Vector3 targetPos = _focusTarget.position + focusOffset;
            transform.position = Vector3.SmoothDamp(
                transform.position, targetPos, ref _velocity, smoothTime);
            transform.LookAt(_focusTarget.position);

            _focusTimer -= Time.deltaTime;
            if (_focusTimer <= 0f)
                _mode = CameraMode.FollowLeader;
        }

        void UpdateWideShot()
        {
            transform.position = Vector3.SmoothDamp(
                transform.position, wideShotPosition, ref _velocity, smoothTime);
            transform.LookAt(new Vector3(25f, 0f, 2.4f));
        }
    }
}
