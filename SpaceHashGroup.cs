using Exerussus._1EasyEcs.Scripts.Core;
using Exerussus._1EasyEcs.Scripts.Custom;
using Leopotam.EcsLite;
using UnityEngine;

namespace Exerussus.EasyEcsModules.SpaceHash
{
    public class SpaceHashGroup : EcsGroup<SpaceHashPooler>
    {
        private SpaceHashSettings _settings = new();

        protected override void SetSharingData(EcsWorld world, GameShare gameShare)
        {
            gameShare.AddSharedObject(_settings);
        }

        protected override void SetInitSystems(IEcsSystems initSystems)
        {
            
        }

        public SpaceHashGroup SetCellSize(float value)
        {
            _settings.CellSize = value;
            return this;
        }

        public SpaceHashGroup SetMinMaxPoints(Vector2 min, Vector2 max)
        {
            _settings.MinPoint = min;
            _settings.MaxPoint = max;
            return this;
        }

        public SpaceHashGroup SetMask(EcsWorld.Mask mask)
        {
            _settings.AdditionalMask = mask;
            return this;
        }
    }
}