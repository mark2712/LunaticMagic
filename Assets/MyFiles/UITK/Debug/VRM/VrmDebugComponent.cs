using UniRx;
using System;
using UnityEngine.UIElements;
using UniVRM10;
using System.IO;
using UnityEngine;

namespace UITK
{
    public class VrmDebugComponent : UIComponent<Type>
    {
        public VrmDebugComponent(Type props, string key = "0") : base(props, key) { }
        private SpawnVrm _spawnVrm;

        public override void Init()
        {
            _spawnVrm = GlobalGame.UIGlobalState.SpawnVrm;

            // реактивность кнопки Clone
            Use(_spawnVrm.HasAny).Subscribe(hasAny =>
            {
                View?.Q<Button>("CloneButton")?.SetEnabled(hasAny);
            });
        }

        public override void Render()
        {
            var spawnBtn = View.Q<Button>("SpawnButton");
            var spawnAllBtn = View.Q<Button>("SpawnAllButton");
            var cloneBtn = View.Q<Button>("CloneButton");

            RegisterCallback<ClickEvent>(spawnBtn, _ =>
            {
                var s = _spawnVrm.SpawnOneAsync(
                    Path.Combine(DataPathManager.ModelsVRM, "mainVRM1.vrm")
                );
            });

            RegisterCallback<ClickEvent>(spawnAllBtn, _ =>
            {
                var s = _spawnVrm.SpawnAllAsync(DataPathManager.ModelsVRM);
            });

            RegisterCallback<ClickEvent>(cloneBtn, _ =>
            {
                _spawnVrm.CloneLast();
            });
        }
    }
}