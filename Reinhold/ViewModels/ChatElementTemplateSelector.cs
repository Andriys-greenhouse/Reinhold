using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Reinhold.View;

namespace Reinhold.ViewModels
{
    public class ChatElementTemplateSelector : DataTemplateSelector
    {
        private static DateElementTemplate dateElementTemplate = new DateElementTemplate();
        private static MessageTemplate messageTemplate = new MessageTemplate();
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Message) { return messageTemplate; }
            if (item is DateElement) { return dateElementTemplate; }
            throw new NotImplementedException();
        }
    }
}
