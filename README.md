# Stacking Dialogs
A lightweight, UGUI-based dialog system.
- Dialogs are added to a `DialogSpace`, in which newly added dialogs go to the top of a stack.
Only the topmost dialog on the stack is visible.
- Supports opening and closing animations.
- Dialogs can be opened through editor setup, allowing UI designers to set up dialogs.

## Setup
### Basic setup
- Put a GameObject with the `DialogSpace` component in your UI canvas.
  - Optional: Enable the "Is Default Space" checkbox on it.
- Create a dialog prefab with the `Dialog` component on it.

If no other `DialogSpace` is passed to the dialog open method, the dialog will be opened in the default space.

### Adding animations
Initialize default animations by setting `DialogAnimations.DefaultShowAnimation` and/or `DialogAnimations.DefaultHideAnimation`.

Find examples in the snippets of this project ($168).

In addition, each `DialogSpace` can have a custom `ShowAnimation` and/or `HideAnimation`.
Leave them as `null` to fall back to the default animations.
Set them to `DialogAnimations.None` to override default animations with a non-animation.

### Set up event responses
Use `DialogSpace.OpeningFirst` and `DialogSpace.RemovedLast`
to have other UI elements react to when the dialog stack starts or stops being empty.

For example, the ingame HUD could hide and re-appear to not collide with dialogs ($169).

### Dialog content
- When implementing `Awake` in a `Dialog` class, make sure to call `base.Awake();`.
- For UI initialization, override `OnCreated`.

A parameterized dialog can be implemented by inheriting `DialogT<T>`. To open a `DialogT<T>`, a `T` value is expected as a parameter.
- For parameter-specific initialization of a `DialogT<T>`, subscribe to the `ParameterUpdated` event. It will be called once after `OnCreated` and whenever the parameter changes later.

### Input blocking during animations
By default, dialogs create a `RaycastBlocker` to prevent user clicks during animations. The blocker is a full-screen transparent `Image` that toggles `raycastTarget` on and off.

You can customize this behavior by overriding `CreateBlocker()` and/or `SetBlocked(bool blocked)`.

#### No blocker (handle input blocking differently)
```csharp
public class MyDialog : Dialog
{
    protected override void CreateBlocker()
    {
        // Don't create blocker - we handle input blocking globally
    }

    protected override void SetBlocked(bool blocked)
    {
        MyInputManager.SetInputEnabled(!blocked);
    }
}
```

#### Visible blocker (semi-transparent overlay)
```csharp
public class MyDialog : Dialog
{
    [SerializeField] private Color blockerColor = new Color(0, 0, 0, 0.5f);

    protected override void CreateBlocker()
    {
        base.CreateBlocker();

        Blocker.Image.color = blockerColor;
    }
}
```

#### Pre-existing blocker in prefab
```csharp
public class MyDialog : Dialog
{
    [SerializeField] private RaycastBlocker myBlocker;

    protected override void CreateBlocker()
    {
        // Don't create one, use the serialized reference
    }

    protected override void SetBlocked(bool blocked)
    {
        myBlocker.SetBlocked(blocked);
    }
}
```

#### Animated blocker
```csharp
public class MyDialog : Dialog
{
    protected override void CreateBlocker()
    {
        base.CreateBlocker();

        Blocker.Image.color = Color.clear;
    }

    protected override void SetBlocked(bool blocked)
    {
        base.SetBlocked(blocked);

        Blocker.Image.DOFade(blocked ? 0.5f : 0f, 0.2f);
    }
}
```

## Usage
### Opening a dialog
Reference a dialog prefab and call `InstantiateAndOpen` on it (or `InstantiateAndOpenAsync` to wait for it).
You can use code or a UnityEvent.

`InstantiateAndOpenAsync` will immediately return a reference to the dialog after it was (asynchronously) instantiated.
To wait until the dialog has fully opened (after any previous dialog "animated out" and this one "animated in"),
await `WaitUntilOpen()`.

### Closing a dialog
Reference the dialog instance and call `Close` on it (or `CloseAsync` to wait for it).
You can use code or a UnityEvent.

Use `DialogSpace.CloseAll` (or `DialogSpace.CloseAllAsync`) to close all dialogs of a dialog space.

### More notes
- No code is required to test a dialog in the actual game UI; dialog prefabs can be dragged into a dialog space.
- You can pass a `DialogSpace` to a dialog when opening it to have it open in that space.
- You can also create a `DialogSpaceId` ScriptableObject (`TeaSpoons/Stacking Dialogs/Dialog Space Id`), assign it to a `DialogSpace` and pass _that_ to the open method.
This way, you can set up a target `DialogSpace` to open in ahead of its instantiation; for example in a prefab or ScriptableObject.

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/stacking-dialogs.git
```

Pin a release by appending a tag, for example `#v0.17.1`.

### Dependencies

Unity cannot resolve git dependencies automatically, so add these to your project first:

- `com.cysharp.unitask` 2.5.0
- `com.tea-spoons.unitask-toolbox` 0.7.1
- `com.tea-spoons.package-core` 1.4.0
- `com.tea-spoons.addressables-toolbox` 0.5.0

## Optional packages

This package works on its own. It uses the packages below when your project has them (Unity detects them automatically) and simply leaves the related code out when it does not.

| Package | Used for |
|---|---|
| uGUI (`com.unity.ugui`) | The package itself (dialogs are uGUI based). Without it nothing is compiled. |
| UniTask (`com.cysharp.unitask` 2.5.0+) | Animated dialogs: `DialogAnimation`, the `*Async` methods and animated show/hide. **Without UniTask dialogs are instantiated and shown or hidden immediately**; `InstantiateAndOpen`, `Close`, `CloseAll` and `CloseTopmost` work the same way, just without animations. |
| Addressables (`com.unity.addressables` 2.2.2+) and UniTask | `DialogReference` and `DialogTReference`, which load dialog prefabs through Addressables. |

## Change plan

See [CHANGE-PLAN.md](CHANGE-PLAN.md) for what changed before publishing and what is planned next.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
