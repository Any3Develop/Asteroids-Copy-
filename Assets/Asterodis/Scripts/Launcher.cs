using System;
using Asterodis.GameBuilder;
using Asterodis.UIWindows;
using DG.Tweening;
using Services.UIService;
using Zenject;

namespace Asterodis
{
    public class Launcher : IInitializable, IDisposable
    {
        private readonly IUIService uiService;
        private readonly IGameBuilder gameBuilder;
        private IGame current;

        public Launcher(IUIService uiService, IGameBuilder gameBuilder)
        {
            this.uiService = uiService;
            this.gameBuilder = gameBuilder;
        }

        public void Initialize()
        {
            DropGame();
            var window = uiService.Show<UIMain>();
            window.OnClick += PlayGame; // not need unsubscribe
            window.SetTitleText("Asteroids");
            window.SetMessageText("Press any where to play.");
            window.PlayVfx(); // TODO move to VFX pool and get/play/return from there
        }

        private void Restart()
        {
            var window = uiService.Show<UIMain>();
            window.OnClick += PlayGame; // not need unsubscribe
            window.SetTitleText("Game Over");
            window.SetMessageText("Press any where to play again.");
        }

        private void PlayGame()
        {
            uiService.Hide<UIMain>();
            GameBuild();
        }

        private void GameBuild()
        {
            DropGame();
            current = gameBuilder.Build();
            current.OnRestartRequired += Restart;
            current.Start();
        }

        private void DropGame()
        {
            current?.Stop();
            current = null;
        }

        public void Dispose()
        {
            DropGame();
        }
    }
}