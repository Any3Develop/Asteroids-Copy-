using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Services.Extensions
{
    public static class TaskExtensions
    {
        public static async Task Async(this Tween tween, CancellationToken token = default)
        {
            if (tween == null)
                return;
            
            var isEnded = false;
            tween.onComplete += WhenCompleted;
            tween.onKill += WhenKilled;
            
            while (Application.isPlaying
                   && !token.IsCancellationRequested 
                   && !isEnded)
            {
                await Task.Yield();
            }            
            
            tween.onComplete -= WhenCompleted;
            tween.onKill -= WhenKilled;

            void WhenCompleted()
            {
                isEnded = !tween.hasLoops || (tween.Loops() - tween.CompletedLoops()) <= 0;
            }

            void WhenKilled()
            {
                isEnded = true;
            }
        }
    }
}