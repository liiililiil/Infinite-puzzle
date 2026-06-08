using SimpleActions;
using Type.Utils;
using UnityEngine;

public class AOTHinter : MonoBehaviour
{
    public void AOTHint()
    {
        SimpleEvent<Capsule<Type.Enums.GamePlay.GameStyle>> dummyEvent = new SimpleEvent<Capsule<Type.Enums.GamePlay.GameStyle>>();
        dummyEvent.AddListener(x => { });
    }
}
