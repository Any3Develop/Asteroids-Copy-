using System.Collections.Generic;
using System.Linq;
using Services.InputService;
using UnityEngine.InputSystem;
using Zenject;

namespace Asterodis.Input
{
	public class InputInstaller : Installer<InputInstaller>
	{
		public override void InstallBindings()
		{
			var gamePlayInput = new AsteroidsInput();
			gamePlayInput.Enable(); // before binding input actions
			
			Container
				.Bind<AsteroidsInput>() // only to control lifetime, not used anywhere.
				.FromInstance(gamePlayInput)
				.AsSingle()
				.NonLazy();

			Container
				.BindInterfacesTo<InputContextProcessor>()
				.AsSingle()
				.NonLazy();

			BindInputController<MovementActions>(gamePlayInput.Movements.Get());
			BindInputController<WeaponActions>(gamePlayInput.Weapons.Get());
		}

		private void BindInputController<TAction>(InputActionMap actionMap) where TAction : struct
		{
			var args = MapInputActions(actionMap);
			// Generics in assembly with IL2CPP - runtime error, deterministic generation of Generics required.
			// https://forum.unity.com/threads/is-there-any-limitations-to-deserializing-json-on-webgl.1250356/#post-7951618
			var inputController = new InputController<TAction>(args);
			Container
				.BindInterfacesTo<InputController<TAction>>()
				.FromInstance(inputController)
				.AsSingle()
				.NonLazy();
		}

		private IEnumerable<IInputAction> MapInputActions(InputActionMap actionMap)
		{
			return actionMap.actions.Select(inputAction => new InputSystemInputAction(inputAction));
		}
	}
}