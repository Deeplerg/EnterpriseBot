using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VkNet.Utils;

namespace EnterpriseBot.VK.Models.Keyboard
{
    public class LocalKeyboard : ICloneable
    {
        [JsonIgnore]
        public IReadOnlyCollection<IReadOnlyCollection<LocalKeyboardButton>> Buttons
        {
            get => buttons;
            set => buttons = value.Select(collection => collection.ToList()).ToList();
        }
        [JsonProperty]
        private List<List<LocalKeyboardButton>> buttons = new List<List<LocalKeyboardButton>>();

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

        //public static implicit operator LocalKeyboard(LocalKeyboardButton button)
        //{
        //    var builder = new LocalKeyboardBuilder();
        //    builder.AddButton(button);
        //    return builder.Build();
        //}

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
