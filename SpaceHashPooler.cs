using System.Collections.Generic;
using Exerussus._1EasyEcs.Scripts.Core;
using Exerussus._1EasyEcs.Scripts.Custom;
using Exerussus.EasyEcsModules.BasicData;
using Leopotam.EcsLite;
using Leopotam.SpaceHash;
using UnityEngine;

namespace ECS.Modules.Exerussus.SpaceHash
{
    public class SpaceHashPooler : IGroupPooler
    {
        public void Initialize(EcsWorld world)
        {
            World = world;
            Position = new(world);
            InitSpaceHash();
            InitFilter();
        }

        [InjectSharedObject] private SpaceHashSettings _spaceHashSettings;
        private List<SpaceHashHit<int>> _result = new(15);
        private List<SpaceHashHit<int>> _resultCustom = new(15);
        private SpaceHash2<int> _spaceHash;
        private int _lastFrame;

        public EcsWorld World { get; private set; }
        public EcsFilter Filter { get; private set; }
        public PoolerModule<MovementData.Position> Position {get; private set; }

        private void InitSpaceHash()
        {
            var s = _spaceHashSettings;
            _spaceHash = new SpaceHash2<int>(s.CellSize, s.MinPoint.x, s.MinPoint.y, s.MaxPoint.x, s.MaxPoint.y);
        }

        private void InitFilter()
        {
            if (_spaceHashSettings.AdditionalMask != null) Filter = _spaceHashSettings.AdditionalMask.Inc<MovementData.Position>().End();
            else Filter = World.Filter<MovementData.Position>().End();
        }
        
        public void Resize(Vector2 minPoint, Vector2 maxPoint, float cellSize)
        {
            _spaceHashSettings.CellSize = cellSize;
            _spaceHashSettings.MinPoint = minPoint;
            _spaceHashSettings.MaxPoint = maxPoint;
            InitSpaceHash();
        }

        public void SetAdditionalMask(EcsWorld.Mask mask)
        {
            _spaceHashSettings.AdditionalMask = mask;
            InitFilter();
        }
        
        private void UpdateSpaceHash()
        {
            _spaceHash.Clear();
            foreach (var entity in Filter)
            {
                ref var positionData = ref Position.Get(entity);
                _spaceHash.Add(entity, positionData.Value.x, positionData.Value.y);
            }
        }
        
        private SpaceHash2<int> TryRefresh()
        {
            var currentFrame = Time.frameCount;
            if (_lastFrame != currentFrame)
            {
                _lastFrame = currentFrame;
                UpdateSpaceHash();
            }
            return _spaceHash;
        }
        
        public void SetSpaceHashObsolete()
        {
            _lastFrame = 0;
        }

        public List<SpaceHashHit<int>> GetAllInRadiusWithYLimit(Vector2 originPosition, float radius, float yLimit)
        {
            _spaceHash = TryRefresh();
            _spaceHash.GetWithYLimit(originPosition.x, originPosition.y, radius, yLimit, false, _result);
            return _result;
        }

        public void GetAllInRadiusWithYLimit(Vector2 originPosition, float radius, float yLimit, List<SpaceHashHit<int>> result)
        {
            _spaceHash = TryRefresh();
            _spaceHash.GetWithYLimit(originPosition.x, originPosition.y, radius, yLimit, false, result);
        }

        public List<SpaceHashHit<int>> GetAllInRadius(Vector2 originPosition, float radius)
        {
            _spaceHash = TryRefresh();
            _spaceHash.Get(originPosition.x, originPosition.y, radius, false, _result);
            return _result;
        }

        public void GetAllInRadius(Vector2 originPosition, float radius, List<SpaceHashHit<int>> result)
        {
            _spaceHash = TryRefresh();
            _spaceHash.Get(originPosition.x, originPosition.y, radius, false, result);
        }
        
        public List<SpaceHashHit<int>> GetAllInRadiusCustomFilter(Vector2 originPosition, float radius, EcsFilter ecsFilter)
        {
            _spaceHash.Clear();
            foreach (var entity in ecsFilter)
            {
                ref var positionData = ref Position.Get(entity);
                _spaceHash.Add(entity, positionData.Value.x, positionData.Value.y);
            }
            
            _spaceHash.Get(originPosition.x, originPosition.y, radius, false, _resultCustom);
            return _resultCustom;
        }
        
        public void GetAllInRadiusCustomFilter(Vector2 originPosition, float radius, EcsFilter ecsFilter, List<SpaceHashHit<int>> result)
        {
            _spaceHash.Clear();
            foreach (var entity in ecsFilter)
            {
                ref var positionData = ref Position.Get(entity);
                _spaceHash.Add(entity, positionData.Value.x, positionData.Value.y);
            }
            
            _spaceHash.Get(originPosition.x, originPosition.y, radius, false, result);
        }
    }
}