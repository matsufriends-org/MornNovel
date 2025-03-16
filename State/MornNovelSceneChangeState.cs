using Arbor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace MornNovel
{
    public class MornNovelSceneChangeState : StateBehaviour
    {
        [SerializeField, Label("null可")] private string _novelAddress;
        [Inject] private MornNovelService _novelManager;

        public override void OnStateBegin()
        {
            if (!string.IsNullOrEmpty(_novelAddress))
            {
                _novelManager.SetNovelAddress(_novelAddress);
            }

            SceneManager.LoadScene(MornNovelGlobal.I.NovelScene);
        }
    }
}