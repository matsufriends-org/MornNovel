using Arbor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace MornNovel
{
    public class MornNovelSceneChangeState : StateBehaviour
    {
        [SerializeField, Label("null可")] private MornNovelAddress _novelAddress;
        [SerializeField] private MornNovelMono _forcePrefab;
        [Inject] private MornNovelService _novelManager;

        public override void OnStateBegin()
        {
            if (_forcePrefab != null)
            {
                _novelManager.SetNovelPrefab(_forcePrefab);
            }else if (!_novelAddress.IsNullOrEmpty())
            {
                _novelManager.SetNovelAddress(_novelAddress);
            }

            SceneManager.LoadScene(MornNovelGlobal.I.NovelScene);
        }
    }
}