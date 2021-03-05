using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum KeyInput
{
    keyDown,
    keyUp,
    keyPress
}

public enum MouseInput
{
    leftMouseBtnDown,
    leftMouseBtnPress,
    leftMouseBtnUp,

    rightMouseBtnDown,
    rightMouseBtnPress,
    rightMouseBtnUp,

    wheelDown,
    wheelUp,
    wheelPress,
    wheelScroll
}

public static class InputHandler 
{
    public static void HandleKeyboardInput(KeyCode keyCode, KeyInput keyInput, Action callback)
    {
        switch(keyInput)
        {
            case KeyInput.keyDown:
                if (Input.GetKeyDown(keyCode)) 
                    callback();
                break;
            case KeyInput.keyUp:
                if (Input.GetKeyUp(keyCode)) 
                    callback();
                break;
            case KeyInput.keyPress:
                if (Input.GetKey(keyCode)) 
                    callback();
                break;
        }
    }

    public static void HandleKeyboardInput(KeyInput keyInput, Action callback, params KeyCode[] keycodes)
    {
        switch (keyInput)
        {
            case KeyInput.keyDown:
                if (keycodes.All(keycode => Input.GetKeyDown(keycode)))
                    callback();
                break;
            case KeyInput.keyUp:
                if (keycodes.Any(keycode => Input.GetKeyUp(keycode)))
                    callback();
                break;
            case KeyInput.keyPress:
                if (keycodes.All(keycode => Input.GetKey(keycode)))
                    callback();
                break;
        }
    }

    public static bool IsAllKeyboardInputHandled(KeyInput keyInput, params KeyCode[] keycodes)
    {
        switch (keyInput)
        {
            case KeyInput.keyDown:
                return keycodes.All(keycode => Input.GetKeyDown(keycode));
            case KeyInput.keyUp:
                return keycodes.All(keycode => Input.GetKeyUp(keycode));
            case KeyInput.keyPress:
                return keycodes.All(keycode => Input.GetKey(keycode));
            default:
                return false;                
        }
    }

    public static bool IsAnyKeyboardInputHandled(KeyInput keyInput, params KeyCode[] keycodes)
    {
        switch (keyInput)
        {
            case KeyInput.keyDown:
                return keycodes.Any(keycode => Input.GetKeyDown(keycode));
            case KeyInput.keyUp:
                return keycodes.Any(keycode => Input.GetKeyUp(keycode));
            case KeyInput.keyPress:
                return keycodes.Any(keycode => Input.GetKey(keycode));
            default:
                return false;
        }
    }

    public static bool IsKeyboardInputHanled(KeyInput keyInput, KeyCode keycode)
    {
        switch (keyInput)
        {
            case KeyInput.keyDown:
                return Input.GetKeyDown(keycode);
            case KeyInput.keyUp:
                return Input.GetKeyUp(keycode);
            case KeyInput.keyPress:
                return Input.GetKey(keycode);
            default:
                return false;
        }
    }


    public static void HandleMouseInput(MouseInput mouseInput, Action callback)
    {
        switch (mouseInput)
        {
            // 왼쪽 마우스 버튼 
            case MouseInput.leftMouseBtnDown:
                if (Input.GetMouseButtonDown(0))
                    callback();
                break;
            case MouseInput.leftMouseBtnPress:
                if (Input.GetMouseButton(0))
                    callback();
                break;
            case MouseInput.leftMouseBtnUp:
                if (Input.GetMouseButtonUp(0))
                    callback();
                break;

            // 오른쪽 마우스 버튼
            case MouseInput.rightMouseBtnDown:
                if (Input.GetMouseButtonDown(1))
                    callback();
                break;
            case MouseInput.rightMouseBtnPress:
                if (Input.GetMouseButton(1))
                    callback();
                break;
            case MouseInput.rightMouseBtnUp:
                if (Input.GetMouseButtonUp(1))
                    callback();
                break;

            // 마우스 휠 버튼
            case MouseInput.wheelDown:
                if (Input.GetMouseButtonDown(2))
                    callback();
                break;
            case MouseInput.wheelPress:
                if (Input.GetMouseButton(2))
                    callback();
                break;
            case MouseInput.wheelUp:
                if (Input.GetMouseButtonUp(2))
                    callback();
                break;
            case MouseInput.wheelScroll:
                if (Mathf.Abs(Input.mouseScrollDelta.y) > 0f)
                    callback();
                break;
        }
    }
}
