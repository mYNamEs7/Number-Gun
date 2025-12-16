using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Window : MonoBehaviour
{
    public event Action<Window> HideEvent;
    public abstract void Show();
    public virtual void Hide() => HideEvent?.Invoke(this);
}

public class WindowList
{
    public List<Window> windows { get; set; }
    public Window curWindow { get; private set; }
    public int curIndex { get; private set; }

    public event Action EndShowEvent;

    public WindowList() => windows = new();
    public WindowList(List<Window> windows)
    {
        this.windows = new();
        windows.ForEach(x => Add(x));
    }

    public void Add(Window window, bool removeAfterHide = false)
    {
        windows.Add(window);
        InitWindow(window, removeAfterHide);
    }

    public void Push(Window window, bool removeAfterHide = false)
    {
        windows.Insert(0, window);
        InitWindow(window, removeAfterHide);
    }

    public void Remove(Window window)
    {
        windows.Remove(window);
        window.HideEvent -= ShowNext;
    }

    public void Show()
    {
        if (windows.Count == 0)
        {
            Debug.Log("List is Empty");
            return;
        }
        curWindow = windows[0];
        curWindow.Show();
    }

    public void Next()
    {
        if (curWindow) curWindow.Hide();
        else Show();
    }

    void InitWindow(Window window, bool remove = false)
    {
        window.HideEvent += ShowNext;
        if (remove) window.HideEvent += Remove;
    }

    void ShowNext(Window window)
    {
        curIndex = windows.IndexOf(window) + 1;
        if (curIndex >= windows.Count) EndShow();
        else
        {
            curWindow = windows[curIndex];
            curWindow.Show();
        }
    }

    void EndShow()
    {
        EndShowEvent?.Invoke();
        curIndex = 0;
        curWindow = null;
    }
}