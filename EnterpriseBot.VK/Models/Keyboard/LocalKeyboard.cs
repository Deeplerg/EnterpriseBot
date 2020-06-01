using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseBot.VK.Models.Keyboard
{
    public class LocalKeyboard : ICloneable
    {
        public IReadOnlyCollection<IReadOnlyCollection<LocalKeyboardButton>> Buttons { get; set; }

        public bool IsEmpty
        {
            get
            {
                return !Buttons.All(buttonList => buttonList.Any());
            }
        }

        public int ButtonCount
        {
            get
            {
                return Buttons.Sum(buttonList => buttonList.Count);
            }
        }

        public LocalKeyboardButton this[int index]
        {
            get
            {
                return Buttons.SelectMany(buttonList => buttonList).ToList()[index];
            }
            //set
            //{
            //    Buttons.SelectMany(buttonList => buttonList).ToList()[index] = value;
            //}
        }

        public object Clone()
        {
            LocalKeyboard keyboard = (LocalKeyboard)MemberwiseClone();

            List<List<LocalKeyboardButton>> buttonLists = Buttons.Select(ROCollection => ROCollection.ToList()).ToList();
            //Select() already made a copy, so there's no need to copy the result explicitly

            keyboard.Buttons = buttonLists;
            return keyboard;
        }
    }
}
