using Arbor;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace MornNovel
{
    public class MornNovelSceneAddState : StateBehaviour
    {
        [SerializeField, Label("null可")] private string _novelAddress;
        [SerializeField] private StateLink _onNovelEnd;
        [Inject] private MornNovelService _novelManager;

        public override async void OnStateBegin()
        {
            if (!string.IsNullOrEmpty(_novelAddress))
            {
                _novelManager.SetNovelAddress(_novelAddress);
            }

            var scene = SceneManager.GetSceneByName(MornNovelGlobal.I.NovelScene);
            await SceneManager.LoadSceneAsync(scene.buildIndex, LoadSceneMode.Additive);
            while (scene.isLoaded)
            {
                await UniTask.Yield(CancellationTokenOnEnd);
            }

            Transition(_onNovelEnd);
        }
    }
}