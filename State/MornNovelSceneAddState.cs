using Arbor;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace MornNovel
{
    public class MornNovelSceneAddState : StateBehaviour
    {
        [SerializeField, Label("null可")] private MornNovelAddress _novelAddress;
        [SerializeField] private StateLink _onNovelEnd;
        [Inject] private MornNovelService _novelManager;

        public override async void OnStateBegin()
        {
            if (!_novelAddress.IsNullOrEmpty())
            {
                _novelManager.SetNovelAddress(_novelAddress);
            }

            await SceneManager.LoadSceneAsync(MornNovelGlobal.I.NovelScene, LoadSceneMode.Additive);
            var scene = SceneManager.GetSceneByName(MornNovelGlobal.I.NovelScene);
            while (scene.isLoaded)
            {
                await UniTask.Yield(CancellationTokenOnEnd);
            }

            Transition(_onNovelEnd);
        }
    }
}