﻿using ChatBot.Ultilities;
using System;

namespace ChatBot.Dialogs
{
    [Serializable]
    public class KeyInfoSelectDialog : SelectDialog
    {
        public override void BuildSelectOptionArray()
        {
            SelectOptions = Enum.GetValues(typeof(KeyInfoValueEnum));
        }
    }
}