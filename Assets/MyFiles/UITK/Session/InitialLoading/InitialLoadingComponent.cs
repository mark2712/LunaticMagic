using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public class InitialLoadingComponent : UIComponent<GameLoadingProgress>
    {
        private ProgressBar _progressBar;
        private IReadOnlyReactiveProperty<float> _progress;

        public InitialLoadingComponent(GameLoadingProgress props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            _progressBar = View?.Q<ProgressBar>("LoadingProgressBar");

            var loading = Props ?? GlobalGame.Session?.GameLoader?.LoadingProgress;
            if (loading != null)
            {
                _progress = Use(loading.Progress);
            }
        }

        public override void Render()
        {
            if (_progressBar == null)
            {
                _progressBar = View?.Q<ProgressBar>("LoadingProgressBar");
            }

            if (_progressBar != null && _progress != null)
            {
                float val = _progress.Value;
                _progressBar.value = val;
                _progressBar.title = $"Загрузка... {Mathf.RoundToInt(val)}%";
            }
        }
    }
}
