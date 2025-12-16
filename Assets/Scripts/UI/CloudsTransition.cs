using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CloudsTransition : MonoBehaviour
{
    public static UnityAction CloudsCloseScreenAction;

    public void CloudsCloseScreen()
    {
        CloudsCloseScreenAction?.Invoke();
        CloudsCloseScreenAction = null;
    }
}
