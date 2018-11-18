using System;

namespace BookCollector.Application.Messages
{
    public class InformationMessage
    {
        public string Content { get; set; }
        public string ActionContent { get; set; }
        public Action ActionHandler { get; set; }

        public InformationMessage(string content)
        {
            Content = content;
        }

        public InformationMessage(string content, string actionContent, Action actionHandler)
        {
            Content = content;
            ActionContent = actionContent;
            ActionHandler = actionHandler;
        }
    }
}
