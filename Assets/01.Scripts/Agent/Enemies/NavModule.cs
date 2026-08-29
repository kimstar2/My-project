using _01.Scripts.Agent.Interface;
using _01.Scripts.GameSystem.GameServices;
using _TevLib.ModuleSystem;
using _TevLib.PolyNavMesh;
using _TevLib.ServiceLocatorSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies
{
    public class NavModule : MonoModule
    {
        [SerializeField] private int maxPathCorner = 16;
        [SerializeField] private float repathInterval = 0.3f; // 경로 재계산 주기
        [SerializeField] private float cornerArriveDistance = 0.2f; // 코너 도착 판정
        
        // private PathAgent _pathAgent; // astar
        private NavAgent2D _pathAgent;
        private IMoveable _mover;

        // private Vector3[] _path;
        private Vector2[] _path;
        private float _cornerArriveSqr;     

        private UniTask<int>? _pathTask;
        private int _pathLength;
        private int _currentIndex;
        private float _repathTimer;

        private IMapService _mapService;
        private bool _isActive; 
        private Vector3 _destination; // 경로
        private Vector3 _requestedDestination; // 이번 경로 계산에 실제로 사용한 목적지

        public bool IsActive => _isActive;
        public bool HasPath => _pathLength > 0;

        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            _pathAgent = owner.GetComponent<NavAgent2D>();
            _mover = owner.GetModule<IMoveable>();
            
            
            _path = new Vector2[maxPathCorner];
            _cornerArriveSqr = cornerArriveDistance * cornerArriveDistance; //sqrMag로 비교함 그래서 제곱값
        }

        private void Start()
        {
            _mapService = ServiceLocator.GetService<IMapService>();
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            if (!_isActive)
            {
                _isActive = true;
                _repathTimer = 0f; // 활성화 직후 바로 경로 재계산
            }
        }

        public void Stop()
        {
            if(_isActive)
                _mover.StopImmediately();
            
            _isActive = false;
            _pathAgent.ResetPath();
            _pathTask = null;
            _pathLength = 0;
            _currentIndex = 0;
            _repathTimer = 0f;
        }

        private void Update()
        {
            if (!_isActive) return;

            //경로 계산이 완료되었다면 반영해주는 작업
            if (_pathTask.HasValue
                && _pathTask.Value.GetAwaiter() is { IsCompleted: true })
            {
                if (_pathTask.Value.Status ==  UniTaskStatus.Succeeded)
                {
                    int count = _pathTask.Value.GetAwaiter().GetResult();
                    if (count > 0)
                    {
                        _pathLength = count;
                        _path[count - 1] = _requestedDestination; // 보정
                        
                        // 코너가 하나 뿐인 경우 목표셀이라 건너뜀
                        _currentIndex = count > 1 ? 1 : 0;
                    }
                }
                _pathTask = null;
            }
                
            // 경로 계산이 완료되지 않았거나. 이미 완료되어서 반영까지 끝났지만 카운터가 돌아갔다면
            _repathTimer -= Time.deltaTime;
            if (_pathTask == null && (_repathTimer <= 0 || _pathLength == 0))
            {
                // 경로 재계산 요청이 들어간다
                Vector3 selfPos = Owner.transform.position;
                _requestedDestination = _destination; // 목표지점 설정
                // Vector3Int startCell = _mapService.GetWorldToCell(selfPos);
                // Vector3Int destCell = _mapService.GetWorldToCell(_destination);
                _pathTask = _pathAgent.GetPath(selfPos, _destination , _path);
                _repathTimer = repathInterval; // 다음 경로 재계산을 위한 타이머 설정
            }
            
            // 그도 아니면 
            // 현재 경로를 따라서 움직이면 됨
            FollowPath();
        }

        private void FollowPath()
        {
            if (_pathLength <= 0 || _currentIndex >= _pathLength) return; // 도착 or 길이 없음
            
            Vector3 selfPos = Owner.transform.position;
            Vector3 delta = _path[_currentIndex] - (Vector2)selfPos;
            delta.z = 0;

            if (delta.sqrMagnitude <= _cornerArriveSqr)
            {
                _currentIndex++;
                if (_currentIndex >= _pathLength)
                    _mover.StopImmediately();
            }
            else
                _mover.SetDirection(delta);
        }
    }
}