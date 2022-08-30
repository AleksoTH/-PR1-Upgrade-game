using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MakeANote : MonoBehaviour
{
    [TextArea]
    [Tooltip("Doesn't do anything. Just comments shown in inspector")]
    public string Notes = "We only login using username, because this is a demo, we aren't interested in scope creep.";
}
