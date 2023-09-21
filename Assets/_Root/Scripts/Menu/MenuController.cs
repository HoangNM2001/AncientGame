using Pancake.Apex;
using Pancake.Scriptable;
using Pancake.Sound;
using Pancake.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Pancake.SceneFlow
{
    [EditorIcon("script_controller")]
    public class MenuController : GameComponent
    {
        [SerializeField] private Transform canvasUI;
        [SerializeField] private ScriptableEventGetGameObject canvasMaster;
        [Header("BUTTON")] [SerializeField] private Button buttonSetting;
        [SerializeField] private Button buttonTapToPlay;
        [SerializeField] private Button buttonShop;

        [Header("POPUP")] [SerializeField] private PopupShowEvent popupShowEvent;
        [SerializeField, PopupPickup] private string popupShop;

        [SerializeField, PopupPickup] private string popupSetting;

        [Header("OTHER")] [SerializeField] private AudioComponent buttonAudio;
        [SerializeField] private ScriptableEventString changeSceneEvent;

        private void Start()
        {
            buttonSetting.onClick.AddListener(ShowPopupSetting);
            buttonTapToPlay.onClick.AddListener(GoToGameplay);
            buttonShop.onClick.AddListener(ShowPopupShop);
        }

        private void GoToGameplay() { changeSceneEvent.Raise(Constant.GAMEPLAY_SCENE); }

        private void ShowPopupShop() { popupShowEvent.Raise(popupShop, canvasMaster.Raise().transform); }


        private void ShowPopupSetting()
        {
            //buttonAudio.PlayAudio();
            popupShowEvent.Raise(popupSetting, canvasUI);
        }
    }
}