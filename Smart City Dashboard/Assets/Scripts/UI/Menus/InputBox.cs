using System;
using TMPro;

public class InputBox : MessageBox
{
    public TMP_InputField Input;
    public Action<bool, string> Callback;
}
