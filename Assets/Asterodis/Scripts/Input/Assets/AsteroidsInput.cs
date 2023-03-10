//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Asterodis/Scripts/Input/Assets/AsteroidsInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @AsteroidsInput : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @AsteroidsInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""AsteroidsInput"",
    ""maps"": [
        {
            ""name"": ""Movements"",
            ""id"": ""a0aa9f97-9b29-475a-b3bb-ba99b90edf3c"",
            ""actions"": [
                {
                    ""name"": ""Acceleration"",
                    ""type"": ""Button"",
                    ""id"": ""eed31809-a057-4536-a61a-1cf15c4ed960"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Rotation"",
                    ""type"": ""Value"",
                    ""id"": ""373b58ee-4014-40d5-8b72-2c0e993662fb"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ef950af4-324a-4c8d-a58b-daabb81c3da8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""405e11a0-3521-4d6c-b216-d51d46545d1e"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f545ee91-3b2e-4410-a0e1-60e3c346db30"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""AD"",
                    ""id"": ""aac0fb90-944e-4341-90ec-77b0d5f6a707"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""4f088195-7108-4889-b038-8df2d5819d10"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""06ad965f-59f1-4d6b-a34c-2411eaf08c3b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""dc700f91-9dbd-41a2-9f49-493c9f963687"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""8ee0340a-16f5-4dd5-a86f-b9b1f3cb9a8c"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""f2e94b33-cb91-4ece-8aef-9eadb3d5716b"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Weapons"",
            ""id"": ""c292d876-e35c-4ac6-862e-543c37001176"",
            ""actions"": [
                {
                    ""name"": ""Fire0"",
                    ""type"": ""Button"",
                    ""id"": ""ef7da917-e746-4d22-ba14-58c9d6988ab9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fire1"",
                    ""type"": ""Button"",
                    ""id"": ""e9ea13b9-9db4-4497-a00d-18dc524e9f74"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""373ebe2a-7a42-4a1a-9576-a6fdf3fcd479"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Fire0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b3a3b3bc-d79f-4199-aee3-362da9eae41c"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Fire0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b402385-dfea-4e5c-a8b2-81d117064c35"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Fire1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bad3de91-14b0-41dc-a350-9010b1f5a20b"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Fire1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Pointer>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Movements
        m_Movements = asset.FindActionMap("Movements", throwIfNotFound: true);
        m_Movements_Acceleration = m_Movements.FindAction("Acceleration", throwIfNotFound: true);
        m_Movements_Rotation = m_Movements.FindAction("Rotation", throwIfNotFound: true);
        // Weapons
        m_Weapons = asset.FindActionMap("Weapons", throwIfNotFound: true);
        m_Weapons_Fire0 = m_Weapons.FindAction("Fire0", throwIfNotFound: true);
        m_Weapons_Fire1 = m_Weapons.FindAction("Fire1", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Movements
    private readonly InputActionMap m_Movements;
    private IMovementsActions m_MovementsActionsCallbackInterface;
    private readonly InputAction m_Movements_Acceleration;
    private readonly InputAction m_Movements_Rotation;
    public struct MovementsActions
    {
        private @AsteroidsInput m_Wrapper;
        public MovementsActions(@AsteroidsInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Acceleration => m_Wrapper.m_Movements_Acceleration;
        public InputAction @Rotation => m_Wrapper.m_Movements_Rotation;
        public InputActionMap Get() { return m_Wrapper.m_Movements; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementsActions set) { return set.Get(); }
        public void SetCallbacks(IMovementsActions instance)
        {
            if (m_Wrapper.m_MovementsActionsCallbackInterface != null)
            {
                @Acceleration.started -= m_Wrapper.m_MovementsActionsCallbackInterface.OnAcceleration;
                @Acceleration.performed -= m_Wrapper.m_MovementsActionsCallbackInterface.OnAcceleration;
                @Acceleration.canceled -= m_Wrapper.m_MovementsActionsCallbackInterface.OnAcceleration;
                @Rotation.started -= m_Wrapper.m_MovementsActionsCallbackInterface.OnRotation;
                @Rotation.performed -= m_Wrapper.m_MovementsActionsCallbackInterface.OnRotation;
                @Rotation.canceled -= m_Wrapper.m_MovementsActionsCallbackInterface.OnRotation;
            }
            m_Wrapper.m_MovementsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Acceleration.started += instance.OnAcceleration;
                @Acceleration.performed += instance.OnAcceleration;
                @Acceleration.canceled += instance.OnAcceleration;
                @Rotation.started += instance.OnRotation;
                @Rotation.performed += instance.OnRotation;
                @Rotation.canceled += instance.OnRotation;
            }
        }
    }
    public MovementsActions @Movements => new MovementsActions(this);

    // Weapons
    private readonly InputActionMap m_Weapons;
    private IWeaponsActions m_WeaponsActionsCallbackInterface;
    private readonly InputAction m_Weapons_Fire0;
    private readonly InputAction m_Weapons_Fire1;
    public struct WeaponsActions
    {
        private @AsteroidsInput m_Wrapper;
        public WeaponsActions(@AsteroidsInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Fire0 => m_Wrapper.m_Weapons_Fire0;
        public InputAction @Fire1 => m_Wrapper.m_Weapons_Fire1;
        public InputActionMap Get() { return m_Wrapper.m_Weapons; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(WeaponsActions set) { return set.Get(); }
        public void SetCallbacks(IWeaponsActions instance)
        {
            if (m_Wrapper.m_WeaponsActionsCallbackInterface != null)
            {
                @Fire0.started -= m_Wrapper.m_WeaponsActionsCallbackInterface.OnFire0;
                @Fire0.performed -= m_Wrapper.m_WeaponsActionsCallbackInterface.OnFire0;
                @Fire0.canceled -= m_Wrapper.m_WeaponsActionsCallbackInterface.OnFire0;
                @Fire1.started -= m_Wrapper.m_WeaponsActionsCallbackInterface.OnFire1;
                @Fire1.performed -= m_Wrapper.m_WeaponsActionsCallbackInterface.OnFire1;
                @Fire1.canceled -= m_Wrapper.m_WeaponsActionsCallbackInterface.OnFire1;
            }
            m_Wrapper.m_WeaponsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Fire0.started += instance.OnFire0;
                @Fire0.performed += instance.OnFire0;
                @Fire0.canceled += instance.OnFire0;
                @Fire1.started += instance.OnFire1;
                @Fire1.performed += instance.OnFire1;
                @Fire1.canceled += instance.OnFire1;
            }
        }
    }
    public WeaponsActions @Weapons => new WeaponsActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    public interface IMovementsActions
    {
        void OnAcceleration(InputAction.CallbackContext context);
        void OnRotation(InputAction.CallbackContext context);
    }
    public interface IWeaponsActions
    {
        void OnFire0(InputAction.CallbackContext context);
        void OnFire1(InputAction.CallbackContext context);
    }
}
