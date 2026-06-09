using UnityEngine;
using Legends.Data;

namespace Legends.Replay
{
    public class ReplayCameraController : MonoBehaviour
    {
        ReplayDirector _director;

        float   _smoothTime         = 0.3f;
        Vector3 _followOffset       = new Vector3(-8f, 4f, 0f);
        Vector3 _focusOffset        = new Vector3(-4f, 3f, 2f);
        Vector3 _wideShotPosition   = new Vector3(-15f, 8f, 0f);
        float   _focusDuration      = 1.5f;

        enum CameraMode { FollowLeader, FocusAthlete, WideShot }

        CameraMode _mode = CameraMode.FollowLeader;
        Vector3    _velocity;
        Transform  _focusTarget;
        float      _focusTimer;
        bool       _wideShotTriggered;

        // Called by ReplayBootstrap after director is created.
        public void Init(ReplayDirector director)
        {
            if (_director != null)
                _director.OnReplayEventFired -= HandleEvent;

            _director = director;
            if (_director != null)
                _director.OnReplayEventFired += HandleEvent;
        }

        void OnDisable()
        {
            if (_director != null)
                _director.OnReplayEventFired -= HandleEvent;
        }

        void HandleEvent(ReplayEvent evt)
        {
            if (evt.EventType == EventTypes.EggDrop)
            {
                var target = _director.GetTransformForAthlete(evt.AthleteId);
                if (target != null)
                {
                    _focusTarget = target;
                    _focusTimer  = _focusDuration;
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
            if (_director == null) return;

            switch (_mode)
            {
                case CameraMode.FollowLeader: UpdateFollowLeader(); break;
                case CameraMode.FocusAthlete: UpdateFocusAthlete(); break;
                case CameraMode.WideShot:     UpdateWideShot();     break;
            }
        }

        void UpdateFollowLeader()
        {
            var leaderTransform = _director.GetLeaderTransform();
            if (leaderTransform == null) return;

            Vector3 targetPos = leaderTransform.position + _followOffset;
            transform.position = Vector3.SmoothDamp(
                transform.position, targetPos, ref _velocity, _smoothTime);
            transform.LookAt(leaderTransform.position);
        }

        void UpdateFocusAthlete()
        {
            if (_focusTarget == null) { _mode = CameraMode.FollowLeader; return; }

            Vector3 targetPos = _focusTarget.position + _focusOffset;
            transform.position = Vector3.SmoothDamp(
                transform.position, targetPos, ref _velocity, _smoothTime);
            transform.LookAt(_focusTarget.position);

            _focusTimer -= Time.deltaTime;
            if (_focusTimer <= 0f)
                _mode = CameraMode.FollowLeader;
        }

        void UpdateWideShot()
        {
            transform.position = Vector3.SmoothDamp(
                transform.position, _wideShotPosition, ref _velocity, _smoothTime);
            transform.LookAt(new Vector3(25f, 0f, 2.4f));
        }
    }
}
