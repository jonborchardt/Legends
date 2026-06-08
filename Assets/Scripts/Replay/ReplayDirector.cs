using System;
using System.Collections.Generic;
using UnityEngine;
using Legends.Data;

namespace Legends.Replay
{
    public class ReplayDirector : MonoBehaviour
    {
        public event Action OnReplayComplete;
        public event Action<ReplayEvent> OnReplayEventFired;

        [SerializeField] GameObject athletePrefab;
        [SerializeField] Material[] athleteMaterials;
        [SerializeField] float replaySpeed = 1f;

        EventResult _result;
        List<GameObject> _spawnedAthletes = new();
        Dictionary<string, AthleteAnimator> _animators  = new();
        Dictionary<string, Transform>       _transforms = new();

        int   _frameIndex;
        int   _eventIndex;
        float _playhead;
        bool  _playing;
        bool  _completeFired;

        public float CurrentTime   => _playhead;
        public float TotalDuration { get; private set; }

        public void Load(EventResult result)
        {
            foreach (var go in _spawnedAthletes)
                Destroy(go);
            _spawnedAthletes.Clear();
            _animators.Clear();
            _transforms.Clear();

            _result        = result;
            _playhead      = 0f;
            _frameIndex    = 0;
            _eventIndex    = 0;
            _playing       = false;
            _completeFired = false;

            TotalDuration = result.Frames.Count > 0
                ? result.Frames[result.Frames.Count - 1].Timestamp
                : 0f;

            for (int i = 0; i < result.Placements.Count; i++)
            {
                string athleteId = result.Placements[i];

                var instance = Instantiate(athletePrefab);
                instance.name = $"Athlete_{athleteId}";
                instance.transform.position = new Vector3(0f, 0f, i * 1.2f);

                if (athleteMaterials != null && athleteMaterials.Length > 0)
                {
                    var renderer = instance.GetComponentInChildren<MeshRenderer>();
                    if (renderer != null)
                        renderer.material = athleteMaterials[i % athleteMaterials.Length];
                }

                _spawnedAthletes.Add(instance);
                _animators[athleteId]  = instance.GetComponent<AthleteAnimator>();
                _transforms[athleteId] = instance.transform;
            }

            Play();
        }

        public void Play()  => _playing = true;
        public void Pause() => _playing = false;

        public Transform GetLeaderTransform()
        {
            if (_result == null || _result.Placements.Count == 0) return null;

            Transform best = null;
            float bestX = float.MinValue;
            foreach (var kvp in _transforms)
            {
                if (kvp.Value.position.x > bestX)
                {
                    bestX = kvp.Value.position.x;
                    best  = kvp.Value;
                }
            }
            return best;
        }

        public Transform GetTransformForAthlete(string athleteId)
        {
            _transforms.TryGetValue(athleteId, out var t);
            return t;
        }

        void Update()
        {
            if (!_playing || _result == null) return;

            _playhead += Time.deltaTime * replaySpeed;

            ApplyFrames();
            FirePendingEvents();

            if (_playhead >= TotalDuration && !_completeFired)
            {
                _completeFired = true;
                _playing = false;
                OnReplayComplete?.Invoke();
            }
        }

        void ApplyFrames()
        {
            if (_result.Frames.Count == 0) return;

            foreach (var athleteId in _result.Placements)
            {
                if (!_transforms.TryGetValue(athleteId, out var t)) continue;
                t.position = GetInterpolatedPosition(athleteId);
            }
        }

        Vector3 GetInterpolatedPosition(string athleteId)
        {
            var frames = _result.Frames;

            ReplayFrame frameA = null, frameB = null;
            for (int i = 0; i < frames.Count; i++)
            {
                if (frames[i].AthleteId != athleteId) continue;
                if (frames[i].Timestamp <= _playhead) frameA = frames[i];
                else if (frameB == null)              frameB = frames[i];
            }

            if (frameA == null) return Vector3.zero;
            if (frameB == null) return ToVector3(frameA.Position);

            float span = frameB.Timestamp - frameA.Timestamp;
            if (span <= 0f) return ToVector3(frameA.Position);

            float t = (_playhead - frameA.Timestamp) / span;
            return Vector3.Lerp(ToVector3(frameA.Position), ToVector3(frameB.Position), t);
        }

        static Vector3 ToVector3(float[] p) => new Vector3(p[0], p[1], p[2]);

        void FirePendingEvents()
        {
            var timeline = _result.Timeline;
            while (_eventIndex < timeline.Count && timeline[_eventIndex].Timestamp <= _playhead)
            {
                var evt = timeline[_eventIndex];
                DispatchAnimation(evt);
                OnReplayEventFired?.Invoke(evt);
                _eventIndex++;
            }
        }

        void DispatchAnimation(ReplayEvent evt)
        {
            if (!_animators.TryGetValue(evt.AthleteId, out var animator)) return;

            bool isFirstPlace = _result.Placements.Count > 0
                && evt.AthleteId == _result.Placements[0];

            switch (evt.EventType)
            {
                case EventTypes.Progress:    animator.PlayCarryRun(); break;
                case EventTypes.Surge:       animator.PlayRun();      break;
                case EventTypes.Stumble:     animator.PlayStumble();  break;
                case EventTypes.EggWobble:   animator.PlayCarryRun(); break;
                case EventTypes.EggRecovery: animator.PlayCarryRun(); break;
                case EventTypes.EggDrop:     animator.PlayFail();     break;
                case EventTypes.Finish:
                    if (isFirstPlace) animator.PlayCelebrate();
                    else              animator.PlayIdle();
                    break;
            }
        }
    }
}
