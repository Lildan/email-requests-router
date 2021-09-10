using System;

namespace EmailRequestsRouter.Domain
{
    public class MessageAttachment
    {

        public MessageAttachment(byte[] content = null, string name = null, string contentType = "application/octet-stream", string contentId = null)
        {
            this.Name = name;
            this.ContentType = contentType;
            this.ContentId = contentId;
            this.Content = content != null ? Convert.ToBase64String(content) : null;
        }

        public string Name { get; set; }

        /// <summary>
        ///   The raw, Base64 encoded content in this attachment.
        /// </summary>
        public string Content { get; set; }
        public string ContentType { get; set; }
        public string ContentId { get; set; }
    }
}
